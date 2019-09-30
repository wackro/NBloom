using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NBloom.PerformanceTests
{
    class SimpleBloomFilterPerfTests
    {
        static Stopwatch _stopwatch = new Stopwatch();
        static HashFunction<string>[] hashFunctions = new HashFunction<string>[]
            {
                new HashFunction<string>(x => (uint)x.GetHashCode()),
                new HashFunction<string>(x => (uint)x.GetHashCode() + 1),
                new HashFunction<string>(x => (uint)x.GetHashCode() + 2),
            };

        static SimpleBloomFilter<string> b = new SimpleBloomFilter<string>(20000, hashFunctions);

        static void Output(object o) => Console.WriteLine(o.ToString());

        static void Main(string[] args)
        {
            //Output(MeasureTime(() => b.Add("hello"), 100000, 100));
            Output(MeasureTime(() => b.Clear(), 1000, 10));

            Console.ReadLine();
        }

        private static double MeasureTime(Action a, int iterations, int runs)
        {
            var times = new List<long>();

            for (var r = 0; r < runs; r++)
            {
                _stopwatch.Start();

                for (var i = 0; i < iterations; i++)
                {
                    a();
                }

                times.Add(_stopwatch.ElapsedMilliseconds);
                _stopwatch.Reset();

                b.Clear();
            }

            return times.Average();
        }
    }
}
