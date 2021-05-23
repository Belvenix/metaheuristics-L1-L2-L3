using System.Collections.Generic;
using MetaheuristicsCS.Optimizers.PopulationOptimizers;
using Optimizers.PopulationOptimizers;

namespace Selections
{
    abstract class ASelection
    {
        public void Select<Element>(ref List<Individual<Element>> population)
        {
            List<Individual<Element>> newPopulation = new List<Individual<Element>>(population.Count);

            AddToNewPopulation(population, newPopulation);
            population = newPopulation;
        }

        protected abstract void AddToNewPopulation<Element>(List<Individual<Element>> population, List<Individual<Element>> newPopulation);

        public void SelectEncoded<Element>(ref List<EncodedIndividual<Element>> population)
        {
            List<EncodedIndividual<Element>> newPopulation = new List<EncodedIndividual<Element>>(population.Count);

            AddToNewPopulationEncoded(population, newPopulation);
            population = newPopulation;
        }

        protected abstract void AddToNewPopulationEncoded<Element>(List<EncodedIndividual<Element>> population, List<EncodedIndividual<Element>> newPopulation);
    }
}
