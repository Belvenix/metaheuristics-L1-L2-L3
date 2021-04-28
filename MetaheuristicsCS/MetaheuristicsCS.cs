using System;
using System.Collections.Generic;
using System.Linq;
using EvaluationsCLI;
using MetaheuristicsCS.Mutations;
using MetaheuristicsCS.Optimizers.Complex;
using MetaheuristicsCS.Solutions;
using MetaheuristicsCS.StopConditions;
using Mutations;
using Optimizers;
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
            var sol1 = new Lab1();
            //var sol2 = new Lab2();
            //var sol3 = new Lab3();
            sol1.Run(seeds);
            //sol2.Run(seeds);
            //sol3.Run(seeds);
            Console.WriteLine("Finished :)");
            Console.ReadKey();
            
        }
    }
}
