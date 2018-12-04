using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiModule.Classes
{
    internal static class LoraDecoders
    {
        private static string DecoderGpsSensor(byte[] payload, uint fport)
        {
            var result = Encoding.ASCII.GetString(payload);
            string[] values = result.Split(':');
            return String.Format("{{\"latitude\": {0} , \"longitude\": {1}}}", values[0], values[1]);
        }

        private static string DecoderTemperatureSensor(byte[] payload, uint fport)
        {
            var result = Encoding.ASCII.GetString(payload);
            return String.Format("{{\"temperature\": {0}}}", result);
        }

        private static string DecoderValueSensor(byte[] payload, uint fport)
        {
            var result = Encoding.ASCII.GetString(payload);
            return String.Format("{{\"value\": {0}}}", result);
        }

        private static string DecoderPmi(byte[] payload, uint fport)
        {
            var raw_pdu = payload;
            string result = "";

            // convert byte[] to string
            var raw_pdu_string = Encoding.ASCII.GetString(raw_pdu);
            Console.WriteLine($"Raw packet in Ascii: {raw_pdu_string}");

            // convert hex string to integer
            var raw_pdu_value = int.Parse(raw_pdu_string, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Raw packet in Hex: {raw_pdu_value}");

            // check UPLINK TYPE first 2-bits
            var uplink_type = raw_pdu_value & 0x03; // 0x03 == 0b11
            Console.WriteLine($"Uplink Type: {uplink_type}");
            result += $"{{\"uplink\": {uplink_type}, ";

            // check uplink type: 0
            if (uplink_type == 0)
            {
                // check fill level next 7-bits
                var fill_level = ((raw_pdu_value >> 2) & 0b1111111);
                Console.WriteLine($"FILL LEVEL:% {fill_level}");
                result += $"\"fill level\": {fill_level}, ";

                // check temperature next 7-bits
                var raw_temp = ((raw_pdu_value >> (2 + 7)) & 0b1111111);

                // negate 40 from raw reading
                raw_temp = raw_temp - 40;
                Console.WriteLine($"TEMPERATURE(C): {raw_temp}");
                result += $"\"temperature (c)\": {raw_temp}, ";

                // check battery level next 6-bits
                var raw_batt = ((raw_pdu_value >> (2 + 7 + 7)) & 0b111111);

                // multiply by2 from raw reading
                raw_batt = raw_batt * 2;
                Console.WriteLine($"BATTERY:(PERCENTAGE): {raw_batt}");
                result += $"\"battery (percentage)\": {raw_batt}, ";

                // check extended function next 2-bits
                var ext_fun = ((raw_pdu_value >> (2 + 7 + 7 + 6)) & 0b11);
                Console.WriteLine($"EXTENDED FUNCTION: {ext_fun}");
                result += $"\"extended function\": {ext_fun}";

            }
            // check uplink type: 1
            if (uplink_type == 1)
            {
                // check raw distance next 9-bits
                var raw_distance = ((raw_pdu_value >> 2) & 0b111111111);
                Console.WriteLine($"RAW DISTANCE(CM): {raw_distance}");
                result += $"\"raw distance (cm)\": {raw_distance}, ";

                // check temperature next 7-bits
                var raw_temp = ((raw_pdu_value >> (2 + 9)) & 0b1111111);

                // negate 40 from raw reading
                raw_temp = raw_temp - 40;
                Console.WriteLine($"TEMPERATURE(C): {raw_temp}");
                result += $"\"temperature (c)\": {raw_temp}, ";

                // check battery level next 6-bits
                var raw_batt = ((raw_pdu_value >> (2 + 9 + 7)) & 0b111111);

                // multiply by 2 from raw reading
                raw_batt = raw_batt * 2;
                Console.WriteLine($"BATTERY:(PERCENTAGE): {raw_batt}");
                result += $"\"battery (percentage)\": {raw_batt}";
            }

            result += "}";

            return (result);
        }
    }
}
