using Crossovers;
using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Crossovers;
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
    class Lab6 : ALab<bool>
    {
        public override void Run(int[] seeds)
        {
            bool debug = true;
            var problems = GenerateProblems();
            var t = Lab6FindStallingParameter(problems, seeds);
            SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab6-limping-results.txt", t.Item1);
            SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab6-limping-counter.txt", t.Item2);

            //t = Lab6BinaryGA()
            //int i = 1;
            //foreach (var seed in seeds)
            //{
            //    var t = Lab6StallDetectionMechanism(problems, seed);
            //    if (debug) Console.WriteLine("Island Model problems: " + DateTime.Now.ToString("HH:mm:ss.fff") + " | " + i.ToString() + "/" + seeds.Length.ToString());
            //    SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab6-island-model.txt", t);
            //    i++;
            //}
        }

        public Tuple<List<String>, List<String>> Lab6FindStallingParameter(IEvaluation<bool>[] problems, int[] seeds)
        {
            List<String> stuckResults = new List<String>();
            List<String> results = new List<String>();
            double[] pMuts = new double[] { 0.01, 0.001, 0.0001, 0.00000001 };
            int[] popSizes = new int[] { 100, 50, 20, 4 };

            int iter = 1;
            int maxI = pMuts.Length * popSizes.Length;
            foreach (var pMut in pMuts)
            {
                foreach (var popSize in popSizes)
                {
                    Dictionary<String, int> problemStallCounter = new Dictionary<String, int>();
                    foreach (var seed in seeds)
                    {
                        TournamentSelection selection = new TournamentSelection(4, seed);
                        OnePointCrossover crossover = new OnePointCrossover(.5);
                        foreach (var problem in problems)
                        {
                            results.Add(Lab6BinaryGA(problem, selection, crossover, pMut, popSize, problemStallCounter, seed));
                        }
                    }
                    Console.WriteLine("\nParameters - mutation probability: " + pMut.ToString() + ", population size: " + 
                        popSize.ToString() + " /// " + DateTime.Now.ToString("HH:mm:ss.fff") + " | " + iter.ToString() + "/" + maxI.ToString());

                    stuckResults.Add(pMut.ToString() + ", " + popSize.ToString() + ", " + String.Join(", ", problemStallCounter.Select(x => x.Value).ToArray()));
                    problemStallCounter.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
                    iter++;
                }
            }
            return new Tuple<List<String>, List<String>>(results, stuckResults);
        }

        private String Lab6BinaryGA(IEvaluation<bool> evaluation, ASelection selection, ACrossover crossover, double pMut, int popSize, Dictionary<String, int> problemStallCounter, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(pMut, evaluation, seed);

            GAStallDetection<bool> ga = new GAStallDetection<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, popSize, 20);

            ga.Run();
            //Console.WriteLine(evaluation.GetType().Name + " : " + ga.countStalling);
            DictionaryCounterUpdate(problemStallCounter, evaluation.GetType().Name, ga.countStalling);
            //ReportOptimizationResult(ga.Result);

            return FormatSave(ga);
        }

        private void DictionaryCounterUpdate(Dictionary<String, int> dict, String key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        private List<String> Lab6StallDetectionMechanism(IEvaluation<bool>[] problems, int seed)
        {
            List<String> results = new List<String>();
            foreach (var problem in problems)
            {
                TournamentSelection selection = new TournamentSelection(5, seed);
                OnePointCrossover crossover = new OnePointCrossover(.5);
                double pMut = 0.00000001;
                int popSize = 20;

                // Hyperparameters
                // Number of islands
                int[] islands = new int[] { 2, 4, 5, 10 };

                // Sposob wyboru populacji do krzyżowania z utkniętą populacją
                PopulationChoosingMethod[] methods = new PopulationChoosingMethod[] { 
                    PopulationChoosingMethod.One, 
                    PopulationChoosingMethod.Random, 
                    PopulationChoosingMethod.Random 
                };

                // Sposob wykrycia utknięcia
                int[] maxNCDs = new int[] { 5, 10, 20, 30 };

                // Grid seach po podanych parametrach
                foreach (var n in islands)
                {
                    foreach (var m in methods)
                    {
                        foreach (var mncd in maxNCDs)
                        {
                            results.Add(Lab6BinaryIslandModel(problem, selection, crossover, pMut, popSize,
                                n, m, mncd,
                                seed));
                        }
                    }
                }
            }
            return results;
        }

        private String Lab6BinaryIslandModel(IEvaluation<bool> evaluation, ASelection selection, ACrossover crossover, double pMut, int popSize, int islandCount, PopulationChoosingMethod method, int maxNCD, int? seed)
        {
            IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 100);

            BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(pMut, evaluation, seed);

            APopulationCrossover<bool> populationCrossover = new ParametrizedPopulationCrossover<bool>(crossover, .5, method);

            IslandModel im = new IslandModel(evaluation, stopCondition, generator, selection, crossover, 
                mutation, popSize, islandCount, maxNCD, populationCrossover);

            GAStallDetection<bool> ga = new GAStallDetection<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 20, 5);

            ga.Run();

            ReportOptimizationResult(ga.Result);

            return FormatSave(ga);
        }


        protected override string FormatOptimizerParameters(AOptimizer<bool> optimizer)
        {
            string parameters = "";
            switch (optimizer)
            {
                case IslandModel im:
                    parameters += im.islandCount + ", " +
                        ((ParametrizedPopulationCrossover<bool>)im.populationCrossover).Method + ", " +
                        im.maxNoChangeDetections;

                    break;
                case GAStallDetection<bool> gasd:
                    parameters += gasd.populationSize + ", " +
                        gasd.mutation.Probability + ", " +  
                        gasd.MaxNCD;
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
