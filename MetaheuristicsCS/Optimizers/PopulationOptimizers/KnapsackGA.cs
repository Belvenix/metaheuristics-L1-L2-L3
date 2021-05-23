using System;
using System.Collections.Generic;

using Crossovers;
using EvaluationsCLI;
using Generators;
using Mutations;
using Optimizers.PopulationOptimizers;
using Selections;
using StopConditions;
using Utility;

namespace MetaheuristicsCS.Optimizers.PopulationOptimizers
{
    public enum EffectType
    {
        Lamarck = 0,
        Baldwin = 1
    };

    class KnapsackGA : GeneticAlgorithm<bool>
    {
        private int? seed;
        public readonly EffectType effectType;
        public KnapsackGA(CBinaryKnapsackEvaluation evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, ACrossover crossover, IMutation<bool> mutation, int populationSize, EffectType effType, int? seed)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, populationSize)
        {
            this.seed = seed;
            effectType = effType;
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            bool foundNewBest = base.RunIteration(itertionNumber, startTime);

            GreedyOptimize();

            return CheckNewBest() || foundNewBest;
        }

        private void GreedyOptimize()
        {
            List<Individual<bool>> populationToOptimize; 
            if (effectType == EffectType.Baldwin)
            {
                populationToOptimize = new List<Individual<bool>>(this.population);
            }
            else
            {
                populationToOptimize = population;
            }
            foreach (var ind in populationToOptimize)
            {
                if (isValid(ind))
                {
                    AddItems(ind);
                }
                else
                {
                    RemoveItems(ind);
                }
                ind.Evaluate(Evaluation);
            }
        }

        private bool isValid(Individual<bool> individual)
        {
            if (((CBinaryKnapsackEvaluation)Evaluation).dCalculateWeight(individual.Genotype) >
                ((CBinaryKnapsackEvaluation)Evaluation).dCapacity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RemoveItems(Individual<bool> individual)
        {
            Shuffler shuffler = new Shuffler(this.seed);
            List<int> order;
            if (this.seed == null)
            {
                order = shuffler.GenereteShuffledOrder(populationSize, new Random());
            }
            else
            {
                order = shuffler.GenereteShuffledOrder(populationSize, new Random(this.seed.Value));
            }
            foreach (var geneIndex in order)
            {
                individual.Genotype[geneIndex] = false;
                if (isValid(individual))
                {
                    break;
                }
            }
        }

        private void AddItems(Individual<bool> individual)
        {
            Shuffler shuffler = new Shuffler(this.seed);
            List<int> order;
            if (this.seed == null)
            {
                order = shuffler.GenereteShuffledOrder(populationSize, new Random());
            }
            else
            {
                order = shuffler.GenereteShuffledOrder(populationSize, new Random(this.seed.Value));
            }
            foreach (var geneIndex in order)
            {
                individual.Genotype[geneIndex] = true;
                if (!isValid(individual))
                {
                    individual.Genotype[geneIndex] = false;
                    break;
                }
            }
        }

    }
}
