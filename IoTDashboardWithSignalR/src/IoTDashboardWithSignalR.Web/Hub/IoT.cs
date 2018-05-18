using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTDashboardWithSignalR.Web
{
    public class IoT : Hub
    {
        const string Build_All_GroupName = "all";
        const string Build_1_Group_Name = "building_1";
        const string Build_2_Group_Name = "building_2";
        const string Build_3_Group_Name = "building_3";
        const string CurrentBuildingKey = "currentBuilding";
        private RetryPolicy retryPolicy;

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Build_1_Group_Name);                
            });

            await retryPolicy.ExecuteAsync(async () =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Build_2_Group_Name);
            });

            await retryPolicy.ExecuteAsync(async () =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Build_3_Group_Name);
            });

            this.Context.Items[CurrentBuildingKey] = Build_All_GroupName;

        }
     
        [Authorize]
        public async Task MonitorBuilding(string name)
        {
            var expectedGroups = new HashSet<string>();
            var allGroups = new string[] { Build_1_Group_Name, Build_2_Group_Name, Build_3_Group_Name };
            foreach (var groupName in allGroups)
            {
                if (name == Build_All_GroupName || name == groupName)
                    expectedGroups.Add(groupName);
            }


            var currentGroups = new HashSet<string>();
            if (this.Context.Items.TryGetValue(CurrentBuildingKey, out var currentGroupValue))
            {
                switch (currentGroupValue.ToString())
                {
                    case Build_All_GroupName:
                        foreach (var g in allGroups)
                            currentGroups.Add(g);
                        break;

                    default:
                        currentGroups.Add(currentGroupValue.ToString());
                        break;
                }
            }

            foreach (var groupToAbondon in currentGroups.Except(expectedGroups))
            {
                await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupToAbondon);
            }

            foreach (var groupToJoin in expectedGroups.Except(currentGroups))
            {
                await Groups.AddToGroupAsync(this.Context.ConnectionId, groupToJoin);
            }
            
            this.Context.Items[CurrentBuildingKey] = name;
        }
    }
}
