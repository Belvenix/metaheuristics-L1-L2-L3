using EvaluationsCLI;
using MetaheuristicsCS.Optimizers.Complex;
using Mutations;
using Optimizers;
using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Solutions
{
    class Lab3 : Lab2
    {
        public override void Run(int[] seeds)
        {
            bool debug = true;
            List<String> resultData = new List<String>();

            int i = 1;

            Console.WriteLine("Start Lab3: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            foreach (int seed in seeds)
            {
                Lab3CheckAll(seed);
                if (debug) Console.WriteLine("Finished " + i.ToString() + " seed of " + seeds.Length.ToString() + " for " + this.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }

            if (debug) Console.WriteLine("Finished third Lab: " + DateTime.Now);
            
        }

        private void Lab3CheckAll(int? seed, bool debug = true)
        {
            List<String> t;
            if (debug) Console.WriteLine("CMAES: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            t = Lab3CheckoptimizerAgainstContinuousProblems<CMAES>(seed);
            SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\wyniki\lab3-benchmark.txt", t);

            if (debug) Console.WriteLine("ES(1+1) one-fifth: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            t = Lab3CheckMineHistory(seed);
            SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\wyniki\lab3-one-fifth.txt", t);

            if (debug) Console.WriteLine("ES(1+1) scouting: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            t = Lab3CheckSpaceAnalysisAroundSol(seed);
            SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\wyniki\lab3-scouting.txt", t);
        }

        private List<String> Lab3CheckoptimizerAgainstContinuousProblems<O>(int? seed, int maxIter = 1000) where O : AOptimizer<double>
        {
            List<String> resultData = new List<String>();

            IEvaluation<double>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                Console.WriteLine(problem);
                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);

                var optimizer = (O)Activator.CreateInstance(typeof(O), problem, stopCondition, 1, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab3CheckoptimizerAgainstContinuousProblems(int? seed, int maxIter = 1000)
        {
            List<String> resultData = new List<String>();

            IEvaluation<double>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                problem.pcConstraint.tGetLowerBound(0);
                List<double> sigmas = Enumerable.Repeat(0.1, problem.iSize).ToList();

                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                RealGaussianMutation mutation = new RealGaussianMutation(sigmas, problem, seed);
                var mutationAdaptation = new RealNullRealMutationES11Adaptation(mutation);

                RealEvolutionStrategy11 optimizer = new RealES11Scouting(problem, stopCondition, mutationAdaptation, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab3CheckMineHistory(int? seed)
        {
            return Lab2CheckAdaptationAgainstContinuousProblems<RealOneFifthRuleMutationES11Adaptation>(seed);
        }

        private List<String> Lab3CheckSpaceAnalysisAroundSol(int? seed)
        {
            return Lab3CheckoptimizerAgainstContinuousProblems(seed); ;
        }

        protected override string FormatOptimizerParameters(AOptimizer<double> optimizer)
        {
            String parametry = "";
            if(optimizer.GetType() == typeof(CMAES))
            {
                parametry += "None";
            }
            else if (optimizer.GetType() == typeof(RealES11Scouting))
            {
                parametry += "Scouting" + ", " +
                    ((RealES11Scouting)optimizer).nScouts.ToString() + ", " +
                    ((RealES11Scouting)optimizer).scoutingMultiplier.ToString();
            }
            else if (optimizer.GetType() == typeof(RealEvolutionStrategy11))
            {
                if (((RealEvolutionStrategy11)optimizer).mutationAdaptation.GetType() == typeof(RealOneFifthRuleMutationES11Adaptation))
                {
                    parametry += "OneFifthRule" + ", " +
                       ((RealOneFifthRuleMutationES11Adaptation)((RealEvolutionStrategy11)optimizer).mutationAdaptation).archiveSize.ToString() + ", " +
                       ((RealOneFifthRuleMutationES11Adaptation)((RealEvolutionStrategy11)optimizer).mutationAdaptation).modifier.ToString();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return parametry;
        }
    }
}
