using System;
using MyLibrary;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var v1 = 3123;
            var v2 = 1.45;
            Console.WriteLine($"{v1} + {v2} = {new Calculator().Sum(v1, v2)}");
        }
    }
}
