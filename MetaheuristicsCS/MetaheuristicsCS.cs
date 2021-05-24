using System;
using System.Collections.Generic;
using System.Linq;
using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Mutations;
using MetaheuristicsCS.Optimizers.Complex;
using MetaheuristicsCS.Solutions;
using MetaheuristicsCS.StopConditions;
using Mutations;
using Optimizers;
using Optimizers.PopulationOptimizers;
using StopConditions;


namespace MetaheuristicsCS
{
    class MetaheuristicsCS
    {

        static void Main(string[] args)
        {
            int[] seeds = {
                -793664, -421376, -168249, 115931,  -557820,
                -272699, 672083,  584213,  505012,  -429238,
                -593089, 841130,  -973074, -291614, 236629,
                -359627, -36357,  691113,  -857379, 573398,
                -139481, -604859, 669982,  -187522, 632728
            };

            //int[] seeds =
            //{
            //    1
            //};
            //var sol1 = new Lab1();
            //var sol2 = new Lab2();
            //var sol3 = new Lab3();
            //var sol4 = new Lab4();
            //var sol5 = new Lab5();
            //var sol6 = new Lab6();
            //var sol7 = new Lab7();
            //sol1.Run(seeds);
            //sol2.Run(seeds);
            //sol3.Run(seeds);
            //sol4.Run(seeds);
            //sol5.Run(seeds);
            //sol6.Run(seeds);
            //sol7.Run(seeds);

            int[] arr = { 7, 8, 2, 3, 1, 5 };
            int[] sortedIndexArray = arr.Select((r, i) => new { Value = r, Index = i })
                                        .OrderBy(t => t.Value)
                                        .Select(p => p.Index)
                                        .Reverse()
                                        .ToArray();
            foreach (int item in sortedIndexArray)
                Console.WriteLine(item);

            Console.WriteLine("Finished :)");
            Console.ReadKey();
            
        }
        static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
