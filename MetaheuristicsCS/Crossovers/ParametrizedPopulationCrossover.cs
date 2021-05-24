using Crossovers;
using Optimizers.PopulationOptimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Crossovers
{
    public enum PopulationChoosingMethod
    {
        One = 0,
        Random = 1,
        All = 2
    };

    class ParametrizedPopulationCrossover<Element> : APopulationCrossover<Element>
    {
        public PopulationChoosingMethod Method { get; private set; }
        public ParametrizedPopulationCrossover(ACrossover crossover, double prob, PopulationChoosingMethod method, int? seed = null)
            :base(crossover, prob, seed)
        {
            Method = method;
        }
        public override List<Individual<Element>> ChooseParentPopulation(in List<List<Individual<Element>>> parentPopulations)
        {
            List<Individual<Element>> possibleParents = new List<Individual<Element>>();
            switch (Method)
            {
                case PopulationChoosingMethod.One:
                    int popIndexOne = integerRNG.Next(0, parentPopulations.Count);
                    possibleParents.AddRange(parentPopulations[popIndexOne]);
                    break;

                case PopulationChoosingMethod.Random:
                    List<int> randomIndexes = Utility.Utils.CreateIndexList(parentPopulations.Count);
                    int popIndexRandom = integerRNG.Next(0, parentPopulations.Count);
                    for (int i = 0; i < popIndexRandom; i++)
                    {
                        possibleParents.AddRange(parentPopulations[randomIndexes[i]]);
                    }
                    break;

                case PopulationChoosingMethod.All:
                    foreach (var parentPopulation in parentPopulations)
                    {
                        possibleParents.AddRange(parentPopulation);
                    }
                    break;
            }
            return possibleParents;
        }
    }
}
