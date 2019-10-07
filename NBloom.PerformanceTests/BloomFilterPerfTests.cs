using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NBloom.Hashing;

namespace NBloom.PerformanceTests
{
    class BloomFilterPerfTests
    {
        static Stopwatch _stopwatch = new Stopwatch();
        static HashFunction<int>[] hashFunctions = new HashFunction<int>[]
            {
                new HashFunction<int>(x => (uint)x.GetHashCode()),
                new HashFunction<int>(x => (uint)x.GetHashCode() + 1),
                new HashFunction<int>(x => (uint)x.GetHashCode() + 2),
            };

        static BoolArrayBloomFilter<int> b = new BoolArrayBloomFilter<int>(2000000000, hashFunctions);
        static CompactBloomFilter<int> b2 = new CompactBloomFilter<int>(2000000000, hashFunctions);

        static void Output(object o) => Console.WriteLine(o.ToString());

        static int[] testInputs = new int[100000];

        static void Main(string[] args)
        {
            foreach(var i in Enumerable.Range(0, 100000))
            {
                testInputs[i] = i;
            }

            // 700
            Output(MeasureTime(() =>
            {
                b.Add(testInputs);
            },
            () =>
            {
                b.Clear();
            }, 100, 5));

            // 1300
            Output(MeasureTime(() =>
            {
                b2.Add(testInputs);
            },
            () =>
            {
                b2.Clear();
            }, 100, 5));

            // 2.2
            Output(MeasureTime(() =>
            {
                b.Contains(1);
            },
            () =>
            {
                b.Clear();
            }, 10000, 5));

            // 1.4
            Output(MeasureTime(() =>
            {
                b2.Contains(1);
            },
            () =>
            {
                b2.Clear();
            }, 10000, 5));

            Console.ReadLine();
        }

        private static double MeasureTime(Action a, Action cleanup, int iterations, int runs)
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

                cleanup();
            }

            return times.Average();
        }
    }
}
