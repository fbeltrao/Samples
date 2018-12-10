using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DynamicJsonParsing
{
    class TypedJson
    {

        [JsonExtensionData]
        public Dictionary<string, object> ExtraData { get; } = new Dictionary<string, object>();

        public List<Rxpk> rxpk { get; set; }
    }

    public class Rxpk
    {
        public int tmst { get; set; }
        public int chan { get; set; }
        public int rfch { get; set; }
        public double freq { get; set; }
        public int stat { get; set; }
        public string modu { get; set; }
        public string datr { get; set; }
        public string codr { get; set; }
        public double lsnr { get; set; }
        public int rssi { get; set; }
        public int size { get; set; }
        public string data { get; set; }
    }

}
