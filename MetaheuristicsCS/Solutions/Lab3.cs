using EvaluationsCLI;
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

            int i = 0;

            Console.WriteLine("Start Lab3: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            foreach (int seed in seeds)
            {
                resultData.AddRange(Lab3CheckCmaes(seed));
                if (debug) Console.WriteLine("Finished " + (i + 1).ToString() + " seed of " + seeds.Length.ToString() + " for " + this.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }

            if (debug) Console.WriteLine("Finished third Lab and saving: " + DateTime.Now);
            this.SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\lab3.txt", resultData);
        }

        private List<String> Lab3CheckCmaes(int? seed)
        {
            return Lab3CheckoptimizerAgainstContinuousProblems<CMAES>(seed);
        }

        private List<String> Lab3CheckoptimizerAgainstContinuousProblems<O>(int? seed, int maxIter = 1000) where O : AOptimizer<double>
        {
            List<String> resultData = new List<String>();

            IEvaluation<double>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);

                var optimizer = (O)Activator.CreateInstance(typeof(O), problem, stopCondition, 1, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab3CheckMineHistory(int? seed)
        {
            List<String> resultData = new List<String>();

            return resultData;
        }

        private List<String> Lab3CheckSpaceAnalysisAroundSol(int? seed)
        {
            List<String> resultData = new List<String>();

            return resultData;
        }

        protected override string FormatOptimizerParameters(AOptimizer<double> optimizer)
        {
            String parametry = "";
            if(optimizer.GetType() == typeof(CMAES))
            {
                parametry += "None";
            }
            else
            {
                throw new NotImplementedException();
            }
            return parametry;
        }
    }
}
