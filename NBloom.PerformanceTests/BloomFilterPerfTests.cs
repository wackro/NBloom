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

        static void Main(string[] args)
        {
            var b = new BoolArrayBloomFilter<int>(200000, 0.0001f, x => BitConverter.GetBytes(x));
            var b2 = new CompactBloomFilter<int>(200000, 0.0001f, x => BitConverter.GetBytes(x));
            void Output(object o) => Console.WriteLine(o.ToString());
            int[] testInputs = new int[200000];

            foreach (var i in Enumerable.Range(0, 200000))
            {
                testInputs[i] = i;
            }

            // 700
            Output(MeasureTime(() => b.Add(testInputs), () => b.Clear(), 100, 5));

            // 1300
            Output(MeasureTime(() =>b2.Add(testInputs), () =>  b2.Clear(), 100, 5));

            // 2.2
            Output(MeasureTime(() => b.Contains(1), () => b.Clear(), 10000, 5));

            // 1.4
            Output(MeasureTime(() => b2.Contains(1), () => b2.Clear(), 10000, 5));

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
