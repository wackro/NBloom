using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NBloom.PerformanceTests
{
    class SimpleBloomFilterPerfTests
    {
        static Stopwatch _stopwatch = new Stopwatch();
        static HashFunction<int>[] hashFunctions = new HashFunction<int>[]
            {
                new HashFunction<int>(x => (uint)x.GetHashCode()),
                new HashFunction<int>(x => (uint)x.GetHashCode() + 1),
                new HashFunction<int>(x => (uint)x.GetHashCode() + 2),
            };

        static SimpleBloomFilter<int> b = new SimpleBloomFilter<int>(2000000000, hashFunctions);

        static void Output(object o) => Console.WriteLine(o.ToString());

        static int[] testInputs = new int[100000];

        static void Main(string[] args)
        {
            foreach(var i in Enumerable.Range(0, 100000))
            {
                testInputs[i] = i;
            }

            // 1025
            Output(MeasureTime(() =>
            {
                b.Add(testInputs);
            }, 100, 5));

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
