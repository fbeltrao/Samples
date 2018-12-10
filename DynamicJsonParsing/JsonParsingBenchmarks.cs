using System;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicJsonParsing
{
    [MemoryDiagnoser]
    public class JsonParsingBenchmarks
    {
        const int REPETITIONS = 100000;
        //const string json = @"{"rxpk":[{"tmst":44239419,"chan":2,"rfch":1,"freq":868.500000,"stat":1,"modu":"LORA","datr":"SF7BW125","codr":"4/5","lsnr":6.8,"rssi":-43,"size":15,"data":"gFVVVVUAMwAIMtJdEIT/"}]}";
        const string json = @"{""extraProp"": ""hello"", ""extraObj"":{ ""foo"": ""bar"" }, ""rxpk"":[{""tmst"":44239419,""chan"":2,""rfch"":1,""freq"":868.500000,""stat"":1,""modu"":""LORA"",""datr"":""SF7BW125"",""codr"":""4/5"",""lsnr"":6.8,""rssi"":-43,""size"":15,""data"":""gFVVVVUAMwAIMtJdEIT/""}]}";

        [Benchmark]
        public static void ParseTypedObject()
        {
            for (var i=0; i < REPETITIONS; ++i)
            {
                var typedJson = JsonConvert.DeserializeObject<TypedJson>(json);
                if (typedJson == null)
                    throw new Exception();

                var extraProp = typedJson.ExtraData["extraProp"];
                var extraObj = typedJson.ExtraData["extraObj"];
                if (extraProp == null || extraObj == null)
                    throw new Exception();
            }
        }

        [Benchmark]
        public static void ParseJObject()
        {
            for (var i=0; i < REPETITIONS; ++i)
            {
                var objJson = (JObject)JsonConvert.DeserializeObject(json);
                if (objJson == null)
                    throw new Exception();

                var extraProp = objJson["extraProp"];
                var extraObj = objJson["extraObj"];
                if (extraProp == null || extraObj == null)
                    throw new Exception();
            }
        }

        [Benchmark]
        public static void ParseDynamicObject()
        {
            for (var i=0; i < REPETITIONS; ++i)
            {
                dynamic dynamicJson = JsonConvert.DeserializeObject(json);
                if (dynamicJson == null)
                    throw new Exception();

                var extraProp = dynamicJson.extraProp;
                var extraObj = dynamicJson.extraObj;
                if (extraProp == null || extraObj == null)
                    throw new Exception();
            }
        }


        public static void TryOut()
        {
            var typedJson = JsonConvert.DeserializeObject<TypedJson>(json);
            Console.WriteLine(typedJson.ExtraData["extraProp"]);
            Console.WriteLine(typedJson.ExtraData["extraObj"]);
            Console.WriteLine($"Typed: {JsonConvert.SerializeObject(typedJson)}");

        
            var objJson = (JObject)JsonConvert.DeserializeObject(json);
            //Console.WriteLine(objJson.GetType().Name);
            Console.WriteLine(objJson["extraProp"]);
            Console.WriteLine(objJson["extraObj"]);
            Console.WriteLine($"JObject: {JsonConvert.SerializeObject(objJson)}");

            dynamic dynamicJson = JsonConvert.DeserializeObject(json);
            //Console.WriteLine(dynamicJson.GetType().Name);
            Console.WriteLine(dynamicJson["extraProp"]);
            Console.WriteLine(dynamicJson["extraObj"]);
            Console.WriteLine($"Dynamic: {JsonConvert.SerializeObject(typedJson)}");

            Console.ReadLine();
        }

    }
}
