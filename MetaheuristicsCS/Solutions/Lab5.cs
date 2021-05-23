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
    class Lab5 : ALab<bool>
    {
        private List<String> PenalizedKnapsack(int? seed, bool penalized)
        {
            List<String> results = new List<String>();
            var evaluations = GenerateProblems();
            foreach (var evaluation in evaluations)
            {
                ((CBinaryKnapsackEvaluation)evaluation).bSetPenalized(penalized);
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                OnePointCrossover crossover = new OnePointCrossover(0.5, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 50);

                ga.Run();

                results.Add(FormatSave(ga));

                ReportOptimizationResult(ga.Result, debug: true);
            }
            return results;

        }

        private List<String> GreedyKnapsack(int? seed, bool penalized, EffectType type)
        {
            List<String> results = new List<String>();
            var evaluations = GenerateProblems();
            foreach (var evaluation in evaluations)
            {
                ((CBinaryKnapsackEvaluation)evaluation).bSetPenalized(penalized);
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                OnePointCrossover crossover = new OnePointCrossover(0.5, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                KnapsackGA ga = new KnapsackGA(((CBinaryKnapsackEvaluation)evaluation), stopCondition, generator, selection, crossover, mutation, 50, type, seed);

                ga.Run();

                results.Add(FormatSave(ga));

                ReportOptimizationResult(ga.Result, debug: true);
            }
            return results;
        }

        public override void Run(int[] seeds)
        {
            Console.WriteLine("Doing example lab5");
            int i = 1;
            foreach (var seed in seeds)
            {
                // Normal Knapsack
                Console.WriteLine("Normal Knapsack");
                var t = PenalizedKnapsack(seed, penalized: false);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-normal-knapsack.txt", t);

                // Penalized Knapsack
                Console.WriteLine("\nPenalized Knapsack");
                t = PenalizedKnapsack(seed, penalized: true);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-penalized-knapsack.txt", t);

                // Normal Greedy Knapsack Baldwin
                Console.WriteLine("\nNormal Greedy Knapsack Baldwin");
                t = GreedyKnapsack(seed, penalized: false, type: EffectType.Baldwin);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-normal-greedy-knapsack-baldwin.txt", t);


                // Penalized Greedy Knapsack Baldwin
                Console.WriteLine("\nPenalized Greedy Knapsack Baldwin");
                t = GreedyKnapsack(seed, penalized: true, type: EffectType.Baldwin);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-penalized-greedy-knapsack-baldwin.txt", t);


                // Normal Greedy Knapsack Lamarck
                Console.WriteLine("\nNormal Greedy Knapsack Lamarck");
                t = GreedyKnapsack(seed, penalized: false, type: EffectType.Lamarck);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-normal-greedy-knapsack-lamarck.txt", t);


                // Penalized Greedy Knapsack Lamarck
                Console.WriteLine("\nPenalized Greedy Knapsack Lamarck");
                t = GreedyKnapsack(seed, penalized: true, type: EffectType.Lamarck);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab5-penalized-greedy-knapsack-lamarck.txt", t);


                Console.WriteLine("Finished seed " + i.ToString() + " of " + seeds.Length);
                i++;
            }
            
        }

        protected override string FormatOptimizerParameters(AOptimizer<bool> optimizer)
        {
            string parameters = "";
            return parameters;
        }

        protected override IEvaluation<bool>[] GenerateProblems()
        {
            IEvaluation<bool>[] evaluations = new IEvaluation<bool>[] {
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_1_100_1000_1),
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_1_500_1000_1),
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_2_100_1000_1),
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_2_500_1000_1),
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_3_100_1000_1),
                new CBinaryKnapsackEvaluation(EBinaryKnapsackInstance.knapPI_3_500_1000_1)
        };
            
            return evaluations;
        }
    }
}
