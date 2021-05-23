using Crossovers;
using EvaluationsCLI;
using Generators;
using Mutations;
using Optimizers;
using Optimizers.PopulationOptimizers;
using Selections;
using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Solutions
{
    class Lab6 : ALab<bool>
    {
        public override void Run(int[] seeds)
        {
            var problems = GenerateProblems();
            foreach (var seed in seeds)
            {
                var t = Lab6FindLimpingParameter(problems, seed);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab6-limping-results.txt", t);
            }
        }

        public List<String> Lab6FindLimpingParameter(IEvaluation<bool>[] problems, int seed)
        {
            List<String> results = new List<String>();
            foreach (var problem in problems)
            {
                TournamentSelection selection = new TournamentSelection(2, seed);
                OnePointCrossover crossover = new OnePointCrossover(0.01);
                double pMut = 0.0001;
                int popSize = 50;
                results.Add(Lab6BinaryGA(problem, selection, crossover, pMut, popSize, seed));
            }
            return results;
        }

        private String Lab6BinaryGA(IEvaluation<bool> evaluation, ASelection selection, ACrossover crossover, double pMut, int popSize, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(pMut, evaluation, seed);

            GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 20);

            ga.Run();

            ReportOptimizationResult(ga.Result);

            return FormatSave(ga);
        }

        protected override string FormatOptimizerParameters(AOptimizer<bool> optimizer)
        {
            string parameters = "";
            switch (optimizer)
            {
                case GeneticAlgorithm<bool> ga:
                    parameters += ga.crossover.Probability.ToString() +
                        ga.mutation.Probability.ToString() +
                        ga.populationSize.ToString();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return parameters;
        }

        protected override IEvaluation<bool>[] GenerateProblems()
        {
            int[] max3Satblocks = { 5, 10, 20, 30, 40, 50 };
            int[] isgGenes = { 25, 49, 100, 484 };
            int[] nkLandscapeGenes = { 10, 50, 100 };
            int[] standardDeceptiveBlocks = { 10, 50, 100 };

            int nProblems = max3Satblocks.Length +  //Max3Sat problem
                isgGenes.Length +                   //Ising Sping Glass
                nkLandscapeGenes.Length +           //NK fitness landscapes
                standardDeceptiveBlocks.Length;     //Standard Deceptive problem with length 3

            int iterator = 0;
            IEvaluation<bool>[] problems = new IEvaluation<bool>[nProblems];

            //Max3Sat problem
            foreach (var genes in max3Satblocks)
            {
                problems[iterator] = new CBinaryMax3SatEvaluation(genes);
                iterator++;
            }

            //Ising Sping Glass
            foreach (var genes in isgGenes)
            {
                problems[iterator] = new CBinaryIsingSpinGlassEvaluation(genes);
                iterator++;
            }

            //NK fitness landscapes
            foreach (var genes in nkLandscapeGenes)
            {
                problems[iterator] = new CBinaryNKLandscapesEvaluation(genes);
                iterator++;
            }

            //Standard Deceptive problem with length 3
            foreach (var functions in standardDeceptiveBlocks)
            {
                problems[iterator] = new CBinaryStandardDeceptiveConcatenationEvaluation(3, functions);
                iterator++;
            }

            return problems;
        }
    }
}
