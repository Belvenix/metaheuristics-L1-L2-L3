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

namespace MetaheuristicsCS.Optimizers.PopulationOptimizers
{
    class EncodedGA<Element> : AOptimizer<Element>
    {
        protected readonly AGenerator<Element> generator;
        protected readonly ASelection selection;
        protected readonly IMutation<Element> mutation;

        protected readonly int populationSize;
        public List<EncodedIndividual<Element>> population;
        public readonly int? seed;
        public ACrossover crossover;

        public EncodedGA(IEvaluation<Element> evaluation, AStopCondition stopCondition, AGenerator<Element> generator,
                                ASelection selection, ACrossover crossover, IMutation<Element> mutation, int populationSize, int? seed)
            : base(evaluation, stopCondition)
        {
            this.generator = generator;
            this.selection = selection;
            this.mutation = mutation;

            this.populationSize = populationSize;
            population = new List<EncodedIndividual<Element>>();
            this.seed = seed;
            this.crossover = crossover;
        }

        protected override sealed void Initialize(DateTime startTime)
        {
            population.Clear();
            for (int i = 0; i < populationSize; ++i)
            {
                population.Add(CreateIndividual());
            }

            Evaluate();
            CheckNewBest();
        }

        protected void Evaluate()
        {
            foreach (Individual<Element> individual in population)
            {
                individual.Evaluate(Evaluation);
            }
        }

        protected void Select()
        {
            selection.SelectEncoded(ref population);
        }

        protected void Mutate()
        {
            foreach (Individual<Element> individual in population)
            {
                individual.Mutate(mutation);
            }
        }

        protected bool CheckNewBest(bool onlyImprovements = true)
        {
            Individual<Element> bestInPopulation = population[0];
            for (int i = 1; i < population.Count; ++i)
            {
                if (population[i].Fitness > bestInPopulation.Fitness)
                {
                    bestInPopulation = population[i];
                }
            }

            return CheckNewBest(bestInPopulation.Genotype, bestInPopulation.Fitness, onlyImprovements);
        }

        protected EncodedIndividual<Element> CreateIndividual(List<Element> genotype = null)
        {
            if (genotype == null)
            {
                genotype = generator.Create(Evaluation.iSize);
            }

            return new EncodedIndividual<Element>(genotype, this.seed);
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            Select();

            Crossover();
            Evaluate();

            bool foundNewBest = CheckNewBest();

            Mutate();
            Evaluate();

            return CheckNewBest() || foundNewBest;
        }

        protected void Crossover()
        {
            for (int i = 0; i < population.Count; i += 2)
            {
                Individual<Element> parent1 = population[i];
                Individual<Element> parent2 = population[i + 1];

                List<Element> offspringGenotype1 = new List<Element>(parent1.Genotype);
                List<Element> offspringGenotype2 = new List<Element>(parent2.Genotype);

                if (crossover.Crossover(parent1.Genotype, parent2.Genotype, offspringGenotype1, offspringGenotype2))
                {
                    population[i] = CreateIndividual(offspringGenotype1);
                    population[i + 1] = CreateIndividual(offspringGenotype2);
                }
            }
        }
    }
}
