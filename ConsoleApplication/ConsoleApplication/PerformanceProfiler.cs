using System;
using System.Diagnostics;

namespace ConsoleApplication
{
    public class PerformanceProfiler
    {
        public Tuple<int, T> CalculateInitializationSize<T>(Func<T> createObject)
        {
            var startBytes = System.GC.GetTotalMemory(true);
            var @object = createObject();
            var stopBytes = System.GC.GetTotalMemory(true);

            var memorySizeint = (int)(stopBytes - startBytes);

            return new Tuple<int, T>(memorySizeint, @object);
        }

        public double ElapsedMiliseconds(Action action)
        {
            var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            action.Invoke();

            stopwatch.Stop();

            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}
