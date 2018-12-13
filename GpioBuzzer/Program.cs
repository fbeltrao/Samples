using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GpioBuzzer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get a reference to the pin you need to use.
            // All 3 methods below are exactly equivalent
            // var blinkingPin = Pi.Gpio[0];
            // blinkingPin = Pi.Gpio[WiringPiPin.Pin00];
            var buzzerPin = Pi.Gpio.Pin08;

            // Configure the pin as an output
            buzzerPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 2; i++)
            {
                isOn = !isOn;
                if (isOn)
                    buzzerPin.Write(1);
                else
                    buzzerPin.Write(0);
                    
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
