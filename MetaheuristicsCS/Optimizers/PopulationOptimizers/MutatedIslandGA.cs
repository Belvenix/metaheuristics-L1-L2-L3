using Crossovers;
using EvaluationsCLI;
using Generators;
using Mutations;
using Optimizers.PopulationOptimizers;
using Selections;
using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Optimizers.PopulationOptimizers
{
    class MutatedIslandGA : GeneticAlgorithm<bool>
    {
        protected GeneticAlgorithm<bool> mutantIsland;
        public MutatedIslandGA(IEvaluation<bool> evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, ACrossover crossover, IMutation<bool> mutation, int populationSize)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, populationSize)
        {
            var moreMutation = new BinaryBitFlipMutation(1, evaluation);
            int mutatedPop = populationSize / 10 % 2 == 1 ? populationSize / 10 - 1 : populationSize / 10;
            mutatedPop = mutatedPop == 0 ? 2 : mutatedPop;
            mutantIsland = new GeneticAlgorithm<bool>(evaluation, stopCondition, generator, selection, crossover, moreMutation, mutatedPop);
            mutantIsland.Initialize();
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            bool baseResult = base.RunIteration(itertionNumber, startTime);

            mutantIsland.RunIteration();
            if (mutantIsland.Result.BestValue > Result.BestValue)
            {
                Combine();
            }

            return baseResult;
        }

        protected void Combine()
        {
            for (int i = 0; i < populationSize / 10; i++)
            {
                population[i] = mutantIsland.population[i];
            }
        }

    }
}
