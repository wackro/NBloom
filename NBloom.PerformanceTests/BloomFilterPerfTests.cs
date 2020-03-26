using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBloom.PerformanceTests
{
    class BloomFilterPerfTests
    {
        static Stopwatch _stopwatch = new Stopwatch();
        static void Output(object o) => Console.WriteLine(o.ToString());

        static void Main(string[] args)
        {
            var b = new BoolArrayBloomFilter<int>(100000, 0.001f, x => BitConverter.GetBytes(x));
            var b2 = new CompactBloomFilter<int>(100000, 0.001f, x => BitConverter.GetBytes(x));
            int[] testInputs = new int[100000];

            foreach (var i in Enumerable.Range(0, 100000))
            {
                testInputs[i] = i;
            }

            // 6000
            Output(MeasureTime(() => b.Add(testInputs), () => b.Reset(), 100, 5));

            // 7700
            Output(MeasureTime(() => b2.Add(testInputs), () => b2.Reset(), 100, 5));

            // 3
            Output(MeasureTime(() => b.Contains(1), () => b.Reset(), 10000, 5));

            // 3
            Output(MeasureTime(() => b2.Contains(1), () => b2.Reset(), 10000, 5));

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
                Output($"Run {r}: {_stopwatch.ElapsedMilliseconds}");
                _stopwatch.Reset();

                cleanup();
            }

            return times.Average();
        }
    }
}
