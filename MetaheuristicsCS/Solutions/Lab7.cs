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
    class Lab7 : ALab<bool>
    {
        private void CrossoverDC(int seed)
        {
            var problems = GenerateProblems();
            var crossovers = new List<ACrossover>() { new OnePointCrossover(0.5, seed), new UniformCrossover(0.5, seed)};
            foreach (var crossover in crossovers)
            {
                var t = AG(seed, problems, crossover);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab7-normal.txt", t);
                
            }
        }

        private void DsmAnalysis(int seed)
        {
            var problems = GenerateProblems();
            var crossovers = new List<ACrossover>() { new OnePointCrossover(0.5, seed), new UniformCrossover(0.5, seed) };
            foreach (var crossover in crossovers)
            {
                var t = AGDsm(seed, problems, crossover);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab7-normal-dsm.txt", t);
            }
        }

        private List<String> AGDsm(int seed, IEvaluation<bool>[] problems, ACrossover crossover)
        {
            List<String> results = new List<String>();

            foreach (var evaluation in problems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 1000);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                DSMGA ga = new DSMGA(evaluation, stopCondition, generator, selection, crossover, mutation, 50);

                ga.Run();

                results.Add(FormatSave(ga));
            }

            return results;

        }

        private List<String> AGDsmWithCrossover(int seed, IEvaluation<bool>[] problems)
        {
            List<String> results = new List<String>();

            foreach (var evaluation in problems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 1000);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                GAWithDSM dsmGAWithCross = new GAWithDSM(evaluation, stopCondition, generator, selection, mutation, 50, 0.5, 0.5);

                dsmGAWithCross.Run();

                results.Add(FormatSave(dsmGAWithCross));
            }

            return results;

        }


        private List<String> AG(int seed, IEvaluation<bool>[] problems, ACrossover crossover)
        {
            List<String> results = new List<String>();

            foreach (var evaluation in problems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 1000);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                GeneticAlgorithm<bool> ga = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 50);

                ga.Run();

                results.Add(FormatSave(ga));
            }

            return results;
            
        }
        
        private List<String> AGShuffled(int seed, IEvaluation<bool>[] problems, ACrossover crossover)
        {
            List<String> results = new List<String>();

            foreach (var evaluation in problems)
            {
                IterationsStopCondition stopCondition = new IterationsStopCondition(evaluation.dMaxValue, 1000);

                BinaryRandomGenerator generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
                BinaryBitFlipMutation mutation = new BinaryBitFlipMutation(1.0 / evaluation.iSize, evaluation, seed);
                TournamentSelection selection = new TournamentSelection(2, seed);

                EncodedGA<bool> ga = new EncodedGA<bool>(evaluation, stopCondition, generator, selection, crossover, mutation, 50, seed);

                ga.Run();

                results.Add(FormatSave(ga));
            }

            return results;
        }

        private void CrossoverShuffledDC(int seed)
        {
            var problems = GenerateProblems();
            var crossovers = new List<ACrossover>() { new OnePointCrossover(0.5, seed), new UniformCrossover(0.5, seed) };
            foreach (var crossover in crossovers)
            {
                var t = AGShuffled(seed, problems, crossover);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab7-shuffled.txt", t);
            }
        }

        public override void Run(int[] seeds)
        {
            bool debug = true;

            var problems = GenerateProblems();

            int i = 1;

            Console.WriteLine("Started experiments" + " " + DateTime.Now.ToString("HH:mm:ss.fff"));

            foreach (var seed in seeds)
            {
                //CrossoverDC(seed);
                //CrossoverShuffledDC(seed);
                //DsmAnalysis(seed);
                var f = AGDsmWithCrossover(seed, problems);
                SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab7-ga-with-dsm.txt", f);

                if (debug) Console.WriteLine("Finished " + i.ToString() + " seed of " + seeds.Length.ToString() + " for " + this.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }

            DsmAnalysis(seeds[0]);

        }

        protected override string FormatOptimizerParameters(AOptimizer<bool> optimizer)
        {
            String parameters = "";
            switch (optimizer)
            {
                case GeneticAlgorithm<bool> ga:
                    parameters += ga.crossover.GetType().Name;
                    break;
                case EncodedGA<bool> ega:
                    parameters += ega.crossover.GetType().Name;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return parameters;
        }

        protected override IEvaluation<bool>[] GenerateProblems()
        {
            int[] concatStandardN = { 3, 5, 10, 20 };
            int[] concatBimodalN = { 5, 10, 15, 20 };
            int[] standardBlocks = { 1, 2, 4, 6, 8, 10 };
            int[] bimodalBlocks = { 1, 2, 3, 4, 5 };
            int nProblems = concatStandardN.Length * standardBlocks.Length +  //Order-5 deceptive concatenation
                concatBimodalN.Length * bimodalBlocks.Length; //Bimodal-10 deceptive concatenation

            int iterator = 0;
            IEvaluation<bool>[] problems = new IEvaluation<bool>[nProblems];

            //Order-n deceptive concatenation, where n in { 3, 5, 10, 20 }
            foreach (int n in concatStandardN)
            {
                foreach (int block in standardBlocks)
                {
                    problems[iterator] = new CBinaryStandardDeceptiveConcatenationEvaluation(n, block);
                    iterator++;
                }
            }

            //Bimodal-n deceptive concatenation, where n in { 5, 10, 15, 20 }
            foreach (int n in concatBimodalN)
            {
                foreach (int block in bimodalBlocks)
                {
                    problems[iterator] = new CBinaryBimodalDeceptiveConcatenationEvaluation(n, block);
                    iterator++;
                }
            }

            return problems;
        }
    }
}
