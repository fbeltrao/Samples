using System;
using System.IO;
using System.Threading.Tasks;

namespace FileWriter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var filePath = "./data";
                if (args != null && args.Length > 0) 
                {
                    filePath = args[0];
                }

                var fileName = Path.Combine(filePath, "shared.txt");

                
                using (var outStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var writer = new StreamWriter(outStream))
                    {
                        var count = 0L;
                        while (true)
                        {
                            var text = $"[{DateTime.UtcNow.ToShortTimeString()}] {count}";
                            Console.WriteLine("Wrote:" + text);
                            await writer.WriteLineAsync(text);
                            await writer.FlushAsync();
                            count++;

                            await Task.Delay(10);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.ExitCode = 1;
            }
        }
    }
}
