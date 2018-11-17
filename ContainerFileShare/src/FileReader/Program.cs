using System;
using System.IO;
using System.Threading.Tasks;

namespace FileReader
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

                
                using (var inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // go to the end of the file
                    inStream.Seek(0, SeekOrigin.End);

                    
                    using (var reader = new StreamReader(inStream))
                    {
                        string line = null;
                        while (true)
                        {
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                Console.WriteLine("Read: " + line);
                            }

                            await Task.Delay(100);
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
