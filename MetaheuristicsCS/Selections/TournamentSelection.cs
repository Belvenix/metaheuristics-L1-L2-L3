using System.Collections.Generic;
using MetaheuristicsCS.Optimizers.PopulationOptimizers;
using Optimizers.PopulationOptimizers;
using Utility;

namespace Selections
{
    sealed class TournamentSelection : ASelection
    {
        private readonly Shuffler shuffler;
        private readonly int? seed;

        private readonly int size;

        public TournamentSelection(int size, int? seed = null)
        {
            shuffler = new Shuffler(seed);
            this.seed = seed;

            this.size = size;
        }

        protected override void AddToNewPopulation<Element>(List<Individual<Element>> population, List<Individual<Element>> newPopulation)
        {
            List<int> indices = Utils.CreateIndexList(population.Count);

            for(int i = 0; i < population.Count; ++i)
            {
                shuffler.Shuffle(indices);

                newPopulation.Add(new Individual<Element>(TournamentWinner(population, indices)));
            }
        }

        private Individual<Element> TournamentWinner<Element>(List<Individual<Element>> population, List<int> indices)
        {
            Individual<Element> winner = population[indices[0]];
            for(int i = 1; i < size; ++i)
            {
                if (population[indices[i]].Fitness > winner.Fitness)
                {
                    winner = population[indices[i]];
                }
            }

            return winner;
        }

        protected override void AddToNewPopulationEncoded<Element>(List<EncodedIndividual<Element>> population, List<EncodedIndividual<Element>> newPopulation)
        {
            List<int> indices = Utils.CreateIndexList(population.Count);

            for (int i = 0; i < population.Count; ++i)
            {
                shuffler.Shuffle(indices);

                newPopulation.Add(new EncodedIndividual<Element>(TournamentWinnerEncoded(population, indices).Genotype, this.seed));
            }
        }

        private EncodedIndividual<Element> TournamentWinnerEncoded<Element>(List<EncodedIndividual<Element>> population, List<int> indices)
        {
            EncodedIndividual<Element> winner = population[indices[0]];
            for (int i = 1; i < size; ++i)
            {
                if (population[indices[i]].Fitness > winner.Fitness)
                {
                    winner = population[indices[i]];
                }
            }

            return winner;
        }

    }
}
