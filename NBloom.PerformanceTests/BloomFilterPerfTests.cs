using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBloom.PerformanceTests
{
    class BloomFilterPerfTests
    {
        static void Output(string testName, object o) => Console.WriteLine($"{testName} {o}");

        static void Main(string[] args)
        {
            var b = new BoolArrayBloomFilter<int>(10000, 0.001f, x => BitConverter.GetBytes(x));
            var b2 = new CompactBloomFilter<int>(10000, 0.001f, x => BitConverter.GetBytes(x));

            var b3 = new BoolArrayBloomFilter<int>(10000, 0.001f, x => BitConverter.GetBytes(x), true);
            var b4 = new CompactBloomFilter<int>(10000, 0.001f, x => BitConverter.GetBytes(x), true);


            int[] testInputs = new int[10000];

            foreach (var i in Enumerable.Range(0, 10000))
            {
                testInputs[i] = i;
            }

            Action test1 = () => Output("boolarray add()", MeasureTime(() => b.Add(testInputs), () => b.Reset(), 100, 10));
            Action test2 = () => Output("compact add()", MeasureTime(() => b2.Add(testInputs), () => b2.Reset(), 100, 10));

            Action test3 = () => Output("boolarray threadsafe add()", MeasureTime(() => b2.Add(testInputs), () => b2.Reset(), 100, 10));
            Action test4 = () => Output("compact threadsafe add()", MeasureTime(() => b2.Add(testInputs), () => b2.Reset(), 100, 10));

            Task.WaitAll(new[] { Task.Run(test1), Task.Run(test2), Task.Run(test3), Task.Run(test4) });

            Console.ReadLine();
        }

        private static double MeasureTime(Action a, Action postRun, int iterations, int runs)
        {
            var stopwatch = new Stopwatch();
            var times = new List<long>();

            for (var r = 0; r < runs; r++)
            {
                stopwatch.Start();

                for (var i = 0; i < iterations; i++)
                {
                    a();
                }

                times.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();

                postRun();
            }

            return times.Average();
        }
    }
}
