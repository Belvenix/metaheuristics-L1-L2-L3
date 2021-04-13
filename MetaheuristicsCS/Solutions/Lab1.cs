using System;
using System.Collections.Generic;
using EvaluationsCLI;
using MetaheuristicsCS.Optimizers.Complex;
using MetaheuristicsCS.StopConditions;
using Optimizers;
using StopConditions;

namespace MetaheuristicsCS.Solutions
{
    class Lab1 : ALab<bool>
    {
        private List<String> Lab1CheckBinaryProblems(int? seed)
        {
            return Lab1CheckoptimizerAgainstBinaryProblems<BinaryRandomSearch>(seed);
        }

        private List<String> Lab1CheckoptimizerAgainstBinaryProblems<O>(int? seed, int maxIter = 500) where O : AOptimizer<bool>
        {
            List<String> resultData = new List<String>();

            IEvaluation<bool>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);

                var optimizer = (O)Activator.CreateInstance(typeof(O), problem, stopCondition, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab1CheckGreedyAgainstBinaryProblems(int? seed, int maxIter = 500)
        {
            List<String> resultData = new List<String>();

            IEvaluation<bool>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                var optimizer = new BinaryGreedyOptimizer(problem, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab1CheckRGSAgainstBinaryProblems(int conditionType, int? seed, int maxIter = 100)
        {
            List<String> resultData = new List<String>();

            IEvaluation<bool>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                AStopCondition greedyStopCondition;
                switch (conditionType)
                {
                    case 1:
                        greedyStopCondition = new IterationsStopCondition(problem.dMaxValue, 1);
                        break;
                    case 2:
                        greedyStopCondition = new IterationsStopCondition(problem.dMaxValue, 4);
                        break;
                    case 3:
                    default:
                        greedyStopCondition = new SignificantImprovementsStopCondition(problem.dMaxValue, maxIter / 10);
                        break;
                }

                var optimizer = new BinaryGreedyRSOptimizer(problem, stopCondition, greedyStopCondition, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }
        protected override IEvaluation<bool>[] GenerateProblems()
        {
            int[] genes = { 5, 10, 20, 30, 40, 50 };
            int[] standardBlocks = { 1, 2, 4, 6, 8, 10 };
            int[] bimodalBlocks = { 1, 2, 3, 4, 5 };
            int[] isgGenes = { 25, 49, 100, 484 };
            int[] nkLandscapeGenes = { 10, 50, 100, 200 };
            int nProblems = genes.Length +  //OneMax
                standardBlocks.Length +  //Order-5 deceptive concatenation
                bimodalBlocks.Length + //Bimodal-10 deceptive concatenation
                isgGenes.Length +   //Ising Sping Glass
                nkLandscapeGenes.Length;    //NK fitness landscapes

            //int[] genes = { 5 };
            //int[] standardBlocks = { 1 };
            //int[] bimodalBlocks = { 1 };
            //int[] isgGenes = { 25 };
            //int[] nkLandscapeGenes = { 10 };
            //int nProblems = genes.Length +  //OneMax
            //    standardBlocks.Length +  //Order-5 deceptive concatenation
            //    bimodalBlocks.Length + //Bimodal-10 deceptive concatenation
            //    isgGenes.Length +   //Ising Sping Glass
            //    nkLandscapeGenes.Length;    //NK fitness landscapes

            int iterator = 0;
            IEvaluation<bool>[] problems = new IEvaluation<bool>[nProblems];

            //OneMax
            foreach (int gene in genes)
            {
                problems[iterator] = new CBinaryOneMaxEvaluation(gene);
                iterator++;
            }

            //Order-5 deceptive concatenation
            foreach (int block in standardBlocks)
            {
                problems[iterator] = new CBinaryStandardDeceptiveConcatenationEvaluation(5, block);
                iterator++;
            }

            //Bimodal-10 deceptive concatenation
            foreach (int block in bimodalBlocks)
            {
                problems[iterator] = new CBinaryBimodalDeceptiveConcatenationEvaluation(10, block);
                iterator++;
            }

            //Ising Sping Glass
            foreach (int gene in isgGenes)
            {
                problems[iterator] = new CBinaryIsingSpinGlassEvaluation(gene);
                iterator++;
            }

            //NK fitness landscapes
            foreach (int gene in nkLandscapeGenes)
            {
                problems[iterator] = new CBinaryNKLandscapesEvaluation(gene);
                iterator++;
            }

            return problems;
        }

        private List<String> Lab1SeedRun(int? seed, bool debug = true)
        {
            List<String> resultData = new List<String>();
            if (debug) Console.WriteLine("Benchmarked problems: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab1CheckBinaryProblems(seed));

            if (debug) Console.WriteLine("Greedy implementation results with benchmarked problems " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab1CheckGreedyAgainstBinaryProblems(seed));

            if (debug) Console.WriteLine("Greedy Random Search implementation results (greedyRepeats=1) " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab1CheckRGSAgainstBinaryProblems(1, seed));

            if (debug) Console.WriteLine("Greedy Random Search implementation results (greedyRepeats=4) " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab1CheckRGSAgainstBinaryProblems(2, seed));

            if (debug) Console.WriteLine("Greedy Random Search implementation results (greedyRepeats=custom) " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab1CheckRGSAgainstBinaryProblems(3, seed));
            return resultData;
        }

        public override void Run()
        {
            List<String> resultData = new List<String>();
            int[] seeds =
            {
                834423,  40287,   -446237, 397333,  -969750,
                -801238, -467687, -696199, -535363, 40016,
                296612,  -451761, 337344,  -442403, 757117,
                528488,  -713088, -230556, -741328, -123544,
                -823157, -586503, -257014, 52247,   207557
            };
            int i = 1;
            Console.WriteLine("Start Lab1: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            foreach (int seed in seeds)
            {
                resultData.AddRange(Lab1SeedRun(seed));

                Console.WriteLine("Finished " + (i+1).ToString() + " seed of 25 for Lab1: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }
            Console.WriteLine("Finished first Lab and saving: " + DateTime.Now);

            this.SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\lab1.txt", resultData);
        }

        protected override string FormatOptimizerParameters(AOptimizer<bool> optimizer)
        {
            String parametry = "";

            // C# 7 switch z sprawdzeniem klas
            switch (optimizer)
            {
                case BinaryRandomSearch brs:
                    parametry += "None";
                    break;

                case BinaryGreedyOptimizer bgo:
                    if (bgo.StopCondition.GetType() == typeof(IterationsStopCondition))
                    {
                        parametry += ((IterationsStopCondition)bgo.StopCondition).maxIterationNumber;
                    }
                    break;

                case BinaryGreedyRSOptimizer bgrso:
                    if (bgrso.greedyOptimizer.StopCondition.GetType() == typeof(IterationsStopCondition))
                    {
                        parametry += ((IterationsStopCondition)bgrso.greedyOptimizer.StopCondition).maxIterationNumber;
                    }
                    else if (bgrso.greedyOptimizer.StopCondition.GetType() == typeof(SignificantImprovementsStopCondition))
                    {
                        parametry += ((SignificantImprovementsStopCondition)bgrso.greedyOptimizer.StopCondition).maxIterationNumber + ", ";
                        parametry += ((SignificantImprovementsStopCondition)bgrso.greedyOptimizer.StopCondition).improvementEpsilon;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            return parametry;
            
        }
    }
}
