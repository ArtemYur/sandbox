using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication
{
    class Program
    {
        private const int MaxValue = 80000;//50000000;

        static void Main(string[] args)
        {
            //CompareIntGeneration();

            var performanceProfiler = new PerformanceProfiler();

            var (intArrayByteSize, intArray) = 
                performanceProfiler
                    .CalculateInitializationSize(
                        () => { return new int[MaxValue]; });

            var (hashSetByteSize, hashSet) =
                performanceProfiler
                    .CalculateInitializationSize(() =>
                    {
                        return new HashSet<int>(MaxValue);
                    });

            var (uhashSetByteSize, uHashSet) = 
                 performanceProfiler
                    .CalculateInitializationSize(() =>
                    {
                        return new HashSet<int>();
                    });

            var (dictionaryByteSize, dictionary) =
                performanceProfiler
                    .CalculateInitializationSize(() =>
                    {
                        return new Dictionary<int, int>(MaxValue);
                    });

            var (uDictionaryByteSize, uDictionary) =
                performanceProfiler
                    .CalculateInitializationSize(() =>
                    {
                        return new Dictionary<int, int>();
                    });


            Console.WriteLine(
                $"Size fraction: (hashSetWithSize / intArr) = {hashSetByteSize / (float)intArrayByteSize}");

            Console.WriteLine(
                $"Size fraction: (uhashSetByteSize / intArr) = {uhashSetByteSize / (float)intArrayByteSize}");

            Console.WriteLine(
                $"Size fraction: (dictionarySize / intArr) = {dictionaryByteSize / (float)intArrayByteSize}");

            Console.WriteLine(
                $"Size fraction: (uDictionarySize / intArr) = {uDictionaryByteSize / (float)intArrayByteSize}");

            Console.WriteLine();


            var intArrayInitializationMiliseconds =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { intArray[number] = number; });
                });

            var hashSetInitializationMiliseconds =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { hashSet.Add(number); });
                });

            var uHashSetInitializationMiliseconds =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { uHashSet.Add(number); });
                });

            var dictionaryInitializationMiliseconds =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { dictionary.Add(number, number); });
                });

            var uDictionaryInitializationMiliseconds =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { uDictionary.Add(number, number); });
                });

            Console.WriteLine(
                $"Init Time fraction: (hashSetInitMs / intArrayInitMs) = {(float)hashSetInitializationMiliseconds / (float)intArrayInitializationMiliseconds}");
            Console.WriteLine(
                $"Init Time fraction: (uHashSetInitMs / intArrayInitMs) = {(float)uHashSetInitializationMiliseconds / (float)intArrayInitializationMiliseconds}");
            Console.WriteLine(
                $"Init Time fraction: (dictionaryInitMs / intArrayInitMs) = {(float)dictionaryInitializationMiliseconds / (float)intArrayInitializationMiliseconds}");
            Console.WriteLine(
                $"Init Time fraction: (uDictionaryInitMs / intArrayInitMs) = {(float)uDictionaryInitializationMiliseconds / (float)intArrayInitializationMiliseconds}");

            Console.WriteLine();

            var intArraySearchMs =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { var res = intArray.First(x => x == number); });
                });

            var hashSetSearchMs =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { hashSet.TryGetValue(number, out var res); });
                });

            var uHashSetSearchMs =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { uHashSet.TryGetValue(number, out var res); });
                });

            var dictionarySearchMs =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { dictionary.TryGetValue(number, out var res); });
                });

            var uDictionarySearchMs =
                performanceProfiler.ElapsedMiliseconds(
                () =>
                {
                    GenerateintegerValues(number => { uDictionary.TryGetValue(number, out var res); });
                });

            Console.WriteLine(
                $"Time fraction (intArrayInitMs / intArraySearchMs) = {(float)intArrayInitializationMiliseconds / (float)intArraySearchMs}");
            Console.WriteLine();

            Console.WriteLine(
                $"Search Time fraction: (hashSetSearchMs / intArraySearchMs) = {(float)hashSetSearchMs / (float)intArraySearchMs}");

            Console.WriteLine(
                $"Search Time fraction: (uHashSetSearchMs / intArraySearchMs) = {(float)uHashSetSearchMs / (float)intArraySearchMs}");

            Console.WriteLine(
                $"Search Time fraction: (dictionarySearchMs / intArraySearchMs) = {(float)dictionarySearchMs / (float)intArraySearchMs}");

            Console.WriteLine(
                $"Search Time fraction: (uDictionarySearchMs / intArraySearchMs) = {(float)uDictionarySearchMs / (float)intArraySearchMs}");

            Console.ReadKey();
        }

        private static void CompareIntGeneration()
        {
            var startBytes = System.GC.GetTotalMemory(true);

            var intArray = new int[MaxValue];

            var stopBytes = System.GC.GetTotalMemory(true);

            var memorySizeint = (int)(stopBytes - startBytes);
            Console.WriteLine("Size is " + memorySizeint);

            var elapsedint =
                ExecuteWithProfile(
                    () => GenerateintegerValues(number => intArray[number] = number));




            startBytes = System.GC.GetTotalMemory(true);

            var hashSet = new HashSet<int>(MaxValue);

            //hashSet.Clear();

            stopBytes = System.GC.GetTotalMemory(true);

            var memorySizeHash = stopBytes - startBytes;

            Console.WriteLine("Size is " + memorySizeHash);

            var elapsedHash =
                ExecuteWithProfile(
                    () => GenerateintegerValues(number => hashSet.Add(number)));


            Console.WriteLine();

            Console.WriteLine($"Size fraction = {(float)memorySizeHash / (float)memorySizeint} ");

            Console.WriteLine($"Time fraction = {(float)elapsedHash / (float)elapsedint}");
        }

        private static double ExecuteWithProfile(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            action.Invoke();

            stopwatch.Stop();

            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalSeconds}");

            return stopwatch.Elapsed.TotalSeconds;
        }

        private static void GenerateintegerValues(Action<int> numberConsumer)
        {
            for (var i = 0; i < MaxValue; i++)
            {
                numberConsumer(i);
            }
        }
    }
}
