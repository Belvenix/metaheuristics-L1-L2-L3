using Crossovers;
using Optimizers.PopulationOptimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MetaheuristicsCS.Crossovers
{
    abstract class APopulationCrossover<Element>
    {
        protected readonly BoolRandom crossoverRNG;
        protected readonly UniformIntegerRandom integerRNG;
        protected ACrossover CrossoverMethod;

        public double Probability { get; set; }
       
        public APopulationCrossover(ACrossover crossover, double prob, int? seed = null)
        {
            CrossoverMethod = crossover;

            Probability = prob;

            crossoverRNG = new BoolRandom(seed);

            integerRNG = new UniformIntegerRandom(seed);
        }

        // in keyword oznacza, ze nie mozna modyfikowac danego elementu
        public List<Individual<Element>> Crossover(in List<Individual<Element>> parentPopulation, in List<List<Individual<Element>>> parentPopulations)
        {
            List<Individual<Element>> offsprings = new List<Individual<Element>>();
            foreach (var parentIndividual in parentPopulation)
            {
                List<Individual<Element>> chosenParentPopulation = ChooseParentPopulation(parentPopulations);
                int chosenParentIndex = integerRNG.Next(0, chosenParentPopulation.Count);
                var offspring = Cross(parentIndividual, chosenParentPopulation[chosenParentIndex]);
                offsprings.Add(offspring);
            }
            

            return offsprings;
        }

        private Individual<Element> Cross(in Individual<Element> parent1, in Individual<Element> parent2)
        {
            Individual<Element> offspring1 = new Individual<Element>(parent1.Genotype);
            Individual<Element> offspring2 = new Individual<Element>(parent2.Genotype);

            CrossoverMethod.Crossover(parent1.Genotype, parent2.Genotype, offspring1.Genotype, offspring2.Genotype);

            return crossoverRNG.Next() ? 
                new Individual<Element>(offspring1.Genotype) :
                new Individual<Element>(offspring2.Genotype);
        }

        public abstract List<Individual<Element>> ChooseParentPopulation(in List<List<Individual<Element>>> parentPopulations);

    }
}
