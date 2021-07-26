using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var buttons = new string[][]
            {
                new [] { "1", "2", "3" },
                new [] { "4", "5", "6" },
                new [] { "7", "8", "9" },
            };
            var buttons1Dimensional = (buttons as IEnumerable<IEnumerable<string>>)
                .Aggregate((accumulator, element) => accumulator.Concat(element));

            Console.WriteLine(string.Join(", ", buttons1Dimensional));
        }
    }
}
