using Crossovers;
using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Optimizers.PopulationOptimizers;
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
    class Lab4 : ALab<bool>
    {
        private void firstExercise(int seed, bool debug = true)
        {
            int[] popSizes = new int[] { 10, 20, 40, 60, 100, 200 };
            IEvaluation<bool>[] problems = GenerateProblems();
            foreach (var p in problems)
            {
                List<String> problemResults = new List<String>();
                foreach (var popSize in popSizes)
                {
                    problemResults.Add(
                        Lab4BinaryGaPopulation(p, popSize, seed)
                        );
                }
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab4-population-research.txt", problemResults);
                if (debug) Console.WriteLine("Finished first exercise " + p.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
        }

        private void secondExercise(int seed, bool debug = true)
        {
            IEvaluation<bool>[] problems = GenerateProblems();
            double[] mutationProbabilities = new double[] { 0, 0.01, 0.05, 0.1, 0.2, 0.5, 1};
            double[] crossoverProbabilites = new double[] { 0, 0.01, 0.05, 0.1, 0.2, 0.5, 1};
            foreach (var p in problems)
            {
                List<String> problemResults = new List<String>();
                foreach (var mp in mutationProbabilities)
                {
                    foreach (var cp in crossoverProbabilites)
                    {
                        problemResults.Add(Lab4BinaryGaMutationCrossover(p, mp, cp, seed));
                    }
                }
                    
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab4-crossover-mutation-research.txt", problemResults);
                if (debug) Console.WriteLine("Finished second exercise " + p.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
        }

        private String Lab4BinaryGaPopulation(IEvaluation<bool> evaluation, int popSize, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 500);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1 / evaluation.iSize, evaluation, seed);

            OnePointCrossover crossover = new OnePointCrossover(.5, seed);
            TournamentSelection selection = new TournamentSelection(5, seed);

            GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, popSize);

            ga.Run();

            return FormatSave(ga);
        }

        private String Lab4BinaryGaMutationCrossover(IEvaluation<bool> evaluation, double pMut, double pCro, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 500);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(pMut, evaluation, seed);

            OnePointCrossover crossover = new OnePointCrossover(pCro, seed);
            TournamentSelection selection = new TournamentSelection(5, seed);

            GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 50);

            ga.Run();

            return FormatSave(ga);
        }

        private String Lab4BinaryGA(IEvaluation<bool> evaluation, ASelection selection, ACrossover crossover, double pMut, int popSize, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(pMut, evaluation, seed);

            GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 20);

            ga.Run();

            return FormatSave(ga);

            //ReportOptimizationResult(ga.Result);
        }

        private static void Lab4BinaryMutatedGA(IEvaluation<bool> evaluation, ASelection selection, ACrossover crossover, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(0, evaluation, seed);

            MutatedIslandGA miga = new MutatedIslandGA(evaluation, stopCondition, generator, selection, crossover, mutation, 20);

            miga.Run();

            ReportOptimizationResult(miga.Result);
        }

        //private void Lab4Max3SAT(int? seed)
        //{
        //    Console.WriteLine("Trap Max3SAT GA");
        //    Lab4BinaryGA(new CBinaryMax3SatEvaluation(100),
        //                 new TournamentSelection(2, seed),
        //                 new OnePointCrossover(0.5, seed),
        //                 0,
        //                 seed);
        //    Console.WriteLine("Trap Max3SAT MIGA");
        //    Lab4BinaryMutatedGA(new CBinaryMax3SatEvaluation(100),
        //                 new TournamentSelection(2, seed),
        //                 new OnePointCrossover(0.5, seed),
        //                 seed);
        //}

        //private void Lab4TrapTournamentSelectionOnePointCrossover(int? seed)
        //{
        //    Console.WriteLine("Trap Tournament GA");
        //    Lab4BinaryGA(new CBinaryStandardDeceptiveConcatenationEvaluation(3, 50),
        //                 new TournamentSelection(2, seed),
        //                 new OnePointCrossover(0.5, seed),
        //                 0,
        //                 seed);
        //    Console.WriteLine("Trap Tournament MIGA");
        //    Lab4BinaryMutatedGA(new CBinaryStandardDeceptiveConcatenationEvaluation(3, 50),
        //                 new TournamentSelection(2, seed),
        //                 new OnePointCrossover(0.5, seed),
        //                 seed);
        //}

        //private void Lab4TrapRouletteWheelSelectionUniformCrossover(int? seed)
        //{
        //    Console.WriteLine("Trap Roulette GA");
        //    Lab4BinaryGA(new CBinaryStandardDeceptiveConcatenationEvaluation(3, 50),
        //                 new RouletteWheelSelection(seed),
        //                 new UniformCrossover(0.5, seed),
        //                 0,
        //                 seed);
        //    Console.WriteLine("Trap Roulette MIGA");
        //    Lab4BinaryMutatedGA(new CBinaryStandardDeceptiveConcatenationEvaluation(3, 50),
        //                 new RouletteWheelSelection(seed),
        //                 new UniformCrossover(0.5, seed),
        //                 seed);
        //}

        public override void Run(int[] seeds)
        {
            bool debug = true;
            int i = 1;
            if (debug) Console.WriteLine("Started experiments" + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
            foreach (int seed in seeds)
            {
                firstExercise(seed);
                secondExercise(seed);
                if (debug) Console.WriteLine("Finished " + i.ToString() + " seed of " + seeds.Length.ToString() + " for " + this.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }
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
