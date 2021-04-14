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
        private static void ReportOptimizationResult<Element>(OptimizationResult<Element> optimizationResult)
        {
            Console.WriteLine("value: {0}", optimizationResult.BestValue);
            Console.WriteLine("\twhen (time): {0}s", optimizationResult.BestTime);
            Console.WriteLine("\twhen (iteration): {0}", optimizationResult.BestIteration);
            Console.WriteLine("\twhen (FFE): {0}", optimizationResult.BestFFE);
        }

        private static void Lab3CMAES(IEvaluation<double> evaluation, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 1000);

            CMAES cmaes = new CMAES(evaluation, stopCondition, 1, seed);

            cmaes.Run();

            ReportOptimizationResult(cmaes.Result);
        }

        private static void Lab3SphereCMAES(int? seed)
        {
            Lab3CMAES(new CRealSphereEvaluation(10), seed);
        }

        private static void Lab3Sphere10CMAES(int? seed)
        {
            Lab3CMAES(new CRealSphere10Evaluation(10), seed);
        }

        private static void Lab3EllipsoidCMAES(int? seed)
        {
            Lab3CMAES(new CRealEllipsoidEvaluation(10), seed);
        }

        private static void Lab3Step2SphereCMAES(int? seed)
        {
            Lab3CMAES(new CRealStep2SphereEvaluation(10), seed);
        }

        private static void Lab3RastriginCMAES(int? seed)
        {
            Lab3CMAES(new CRealRastriginEvaluation(10), seed);
        }

        private static void Lab3AckleyCMAES(int? seed)
        {
            Lab3CMAES(new CRealAckleyEvaluation(10), seed);
        }
        
        private static IEvaluation<double>[] generateDoubleProblems()
        {
            int[] genes = { 2, 5, 10 };
            int nProblems = genes.Length * 4;
            IEvaluation<double>[] problems = new IEvaluation<double>[nProblems];

            int iterator = 0;

            //Sphere
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealSphereEvaluation(gene);
                iterator++;
            }

            //Sphere-10
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealSphere10Evaluation(gene);
                iterator++;
            }

            //Ellipsoid
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealEllipsoidEvaluation(gene);
                iterator++;
            }

            //Step-2 Sphere
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealStep2SphereEvaluation(gene);
                iterator++;
            }
            return problems;
        }

        private static void Lab2CheckContinuousProblems(int? seed)
        {
            Console.WriteLine("Benchmarked problems (No Mutation Adaptation):");
            Lab2CheckAdaptationAgainstContinuousProblems<RealNullRealMutationES11Adaptation>(seed);

            Console.WriteLine("Simulated Annealing MA");
            Lab2CheckAdaptationAgainstContinuousProblems<RealSimulatedAnnealingMutationAdaptation>(seed);

            Console.WriteLine("Igniting Simulated Annealing MA problems:");
            Lab2CheckAdaptationAgainstContinuousProblems<RealIgnitingSimAnnMutationAdaptation>(seed);

            Console.WriteLine("Simulated Annealing MA with Domain Knowledge:");
            Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<RealDomainKnowledgeSimAnnAdaptationMutation>(seed);

            Console.WriteLine("Igniting Simulated Annealing MA with Domain Knowledge:");
            Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<RealDomainKnowledgeIgnSimAnnMutationAdaptation>(seed);
        }

        private static void Lab2CheckAdaptationAgainstContinuousProblems<A>(int? seed, int maxIter = 1000) where A : ARealMutationES11Adaptation
        {
            IEvaluation<double>[] benchmarkProblems = generateDoubleProblems();
            foreach (var problem in benchmarkProblems)
            {
                problem.pcConstraint.tGetLowerBound(0);                    ;
                List<double> sigmas = Enumerable.Repeat(0.1, problem.iSize).ToList();

                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                RealGaussianMutation mutation = new RealGaussianMutation(sigmas, problem, seed);
                var mutationAdaptation = (A)Activator.CreateInstance(typeof(A), mutation);

                RealEvolutionStrategy11 optimizer = new RealEvolutionStrategy11( problem, stopCondition, mutationAdaptation, seed);

                optimizer.Run();

                ReportOptimizationResult(optimizer.Result);
            }
        }

        private static void Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<A>(int? seed, int maxIter = 1000) where A : ARealMutationES11Adaptation
        {
            IEvaluation<double>[] benchmarkProblems = generateDoubleProblems();
            foreach (var problem in benchmarkProblems)
            {
                problem.pcConstraint.tGetLowerBound(0); ;
                List<double> sigmas = Enumerable.Repeat(0.1, problem.iSize).ToList();

                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                RealGaussianMutation mutation = new RealGaussianMutation(sigmas, problem, seed);
                var mutationAdaptation = (A)Activator.CreateInstance(typeof(A), mutation, problem);

                RealEvolutionStrategy11 optimizer = new RealEvolutionStrategy11(problem, stopCondition, mutationAdaptation, seed);

                Console.WriteLine("Problem " + problem.GetType().Name + ", " + problem.iSize.ToString());

                optimizer.Run();

                ReportOptimizationResult(optimizer.Result);
            }
        }

        private static void Lab3CheckoptimizerAgainstContinuousProblems<O>(int? seed, int maxIter = 1000) where O : AOptimizer<double>
        {
            IEvaluation<double>[] benchmarkProblems = generateDoubleProblems();
            foreach (var problem in benchmarkProblems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);

                var optimizer = (O)Activator.CreateInstance(typeof(O), problem, stopCondition, 1,  seed);

                optimizer.Run();

                ReportOptimizationResult(optimizer.Result);
            }
        }

        private static void Lab3CheckContinuousProblems(int? seed)
        {
            Lab3CheckoptimizerAgainstContinuousProblems<CMAES>(seed);
        }

        static void Main(string[] args)
        {
            int[] seeds = {
                -793664, -421376, -168249, 115931,  -557820,
                -272699, 672083,  584213,  505012,  -429238,
                -593089, 841130,  -973074, -291614, 236629,
                -359627, -36357,  691113,  -857379, 573398,
                -139481, -604859, 669982,  -187522, 632728
            };
            var sol1 = new Lab1();
            var sol2 = new Lab2();
            var sol3 = new Lab3();
            sol1.Run(seeds);
            sol2.Run(seeds);
            sol3.Run(seeds);
            Console.WriteLine("Finished :)");
            Console.ReadKey();
            
        }
    }
}
