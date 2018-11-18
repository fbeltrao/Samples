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
                var isNetworkFile = true;
                long position = 0L;

                string line = null;
                var inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var reader = new StreamReader(inStream);
                if (isNetworkFile)
                {
                    while (true)
                    {
                        inStream.Seek(position, SeekOrigin.Begin);
                        while ((line = await reader.ReadLineAsync()) != null) 
                        {
                            if (line.Length > 0 && line[0] == 0)
                            {
                                await Task.Delay(1000);
                                break;
                            }
                            position = inStream.Position;
                            Console.WriteLine("Read: " + line);
                        }

                        reader.Dispose();
                        inStream.Dispose();
                        await Task.Delay(100);
                        inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        reader = new StreamReader(inStream);
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
