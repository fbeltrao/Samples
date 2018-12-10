using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using BenchmarkDotNet.Running;

namespace DynamicJsonParsing
{
    class Program
    {
        static void Main(string[] args)
        {
            // var summary = BenchmarkRunner.Run<JsonParsingBenchmarks>();
            JsonParsingBenchmarks.TryOut();
            Console.ReadLine();
        }
    }
}
