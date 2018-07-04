using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace UserEmailConfirmation
{
    /// <summary>
    /// Orchestration of a user registration
    /// </summary>
    public static class UserRegistrationOrchestator
    {
        [FunctionName(nameof(UserRegistrationOrchestration))]
        public static async Task<bool> UserRegistrationOrchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var input = context.GetInput<UserRegistrationInput>();

            // 1. Send confirmation email
            var sendConfirmationEmailInput = new SendConfirmationEmailInput
            {
                RegistrationConfirmationURL = $"{input.BaseUri}/emailconfirmation/{input.InstanceId}",
                Email = input.Email,
                Name = input.Name
            };

            await context.CallActivityAsync(nameof(SendConfirmationEmailActivity), sendConfirmationEmailInput);

            // 2. Setup timer and wait for external event to be executed. Whatever comes first continues            
            using (var cts = new CancellationTokenSource())
            {
                var expiredAt = context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds(30));
                var customStatus = new UserRegistrationOrchestatorStatus { Text = "Waiting for email confirmation", ExpireAt = expiredAt };
#if DEBUG
                // only send the confirmation URL back in debug mode
                customStatus.RegistrationConfirmationURL = sendConfirmationEmailInput.RegistrationConfirmationURL;
#endif
                context.SetCustomStatus(customStatus);

                var confirmationButtonClicked = context.WaitForExternalEvent<bool>("EmailConfirmationReceived");
                var timeout = context.CreateTimer(expiredAt, cts.Token);

                var winner = await Task.WhenAny(confirmationButtonClicked, timeout);

                // Cancel the timer if it has not yet been completed
                if (!timeout.IsCompleted)
                    cts.Cancel();

                if (winner == confirmationButtonClicked)
                {
                    // TODO: add here logic to actually create the user in your database
                    context.SetCustomStatus(new UserRegistrationOrchestatorStatus { Text = "Email activation succeeded" });                    
                    return true;
                }
                else
                {
                    context.SetCustomStatus(new UserRegistrationOrchestatorStatus { Text = "Email activation timed out" });
                    return false;
                }
            }                         
        }

        [FunctionName(nameof(SendConfirmationEmailActivity))]
        public static void SendConfirmationEmailActivity(            
            [ActivityTrigger] SendConfirmationEmailInput input,
            [SendGrid(ApiKey = "sendGridApiKey")] out SendGridMessage message,
            TraceWriter log)
        {
            
            message = new SendGridMessage();
            message.AddTo(input.Email);
            message.SetSubject("[Azure Durable Function Demo] - User registration confirmation");

            var htmlContent = new StringBuilder();
            htmlContent
                .AppendLine("<html>")
                .AppendLine($"<head><meta name=\"viewport\" content=\"width=device-width\" /><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><title>Email confirmation</title>")
                .AppendLine("<body>")
                .AppendLine($"<p>Hello {input.Name}</p>")
                .AppendLine($"<p>To activate your account open this page: <a href=\"{input.RegistrationConfirmationURL}\">{input.RegistrationConfirmationURL}</a></p>")
                .AppendLine("</body></html>");

            message.AddContent("text/html", htmlContent.ToString());
            message.SetFrom(new EmailAddress("no-reply@mycompany.com"));

            log.Info($"Email sent to {input.Email} with confirmation URL {input.RegistrationConfirmationURL}");
        }


        /// <summary>
        /// Handles the Email link to confirm the user registration
        /// </summary>
        /// <param name="client"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>        
        [FunctionName(nameof(EmailConfirmationHandler))]         
        public static async Task<HttpResponseMessage> EmailConfirmationHandler(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "emailconfirmation/{instanceId}")] HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient client, 
            string instanceId,
            TraceWriter log)
        {
            var instanceStatus = await client.GetStatusAsync(instanceId);
            if (instanceStatus?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                if (instanceStatus.CustomStatus != null)
                {
                    var status = instanceStatus.CustomStatus.ToObject<UserRegistrationOrchestatorStatus>();
                    if (status.ExpireAt.HasValue &&
                        status.ExpireAt.Value < DateTime.UtcNow)
                    {
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent($"Registration confirmation expired {DateTime.UtcNow.Subtract(status.ExpireAt.Value).TotalSeconds} seconds ago")
                        };
                    }
                }

                await client.RaiseEventAsync(instanceId, "EmailConfirmationReceived", true);

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("Your user registration has been received")
                };
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("Registration confirmation is not valid")
                };
            }
        }
        
        [FunctionName(nameof(UserRegistration))]
        public static async Task<HttpResponseMessage> UserRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            try
            {
                var input = JsonConvert.DeserializeObject<UserRegistrationInput>(await req.Content.ReadAsStringAsync());
                if (input == null ||
                    string.IsNullOrEmpty(input.Name) ||
                    string.IsNullOrEmpty(input.Email)
                    )
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { Content = new StringContent("Invalid request payload") };
                }

                var baseUri = new StringBuilder()
                    .Append(req.RequestUri.Scheme)
                    .Append("://")
                    .Append(req.RequestUri.Host);

                if (!req.RequestUri.IsDefaultPort)
                    baseUri.Append(':').Append(req.RequestUri.Port);
                baseUri.Append("/api");

                input.BaseUri = baseUri.ToString();
                input.InstanceId = $"{input.Email}-{DateTime.UtcNow.Ticks}";

                // Instance Id will be <email address>-<current ticks>
                await starter.StartNewAsync(nameof(UserRegistrationOrchestration), input.InstanceId, input);

                log.Info($"Started orchestration with ID = '{input.InstanceId}'.");

                return starter.CreateCheckStatusResponse(req, input.InstanceId);
            }
            catch (Exception ex)
            {
                log.Error($"Error starting registration process", ex);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) { Content = new StringContent("Internal error") };
            }
        }
    }
}