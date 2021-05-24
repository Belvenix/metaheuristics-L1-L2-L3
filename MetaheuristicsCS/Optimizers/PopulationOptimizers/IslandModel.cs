using Crossovers;
using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Crossovers;
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
    class IslandModel : GeneticAlgorithm<bool>
    {
        public List<GAStallDetection<bool>> islands { get; private set; }
        public int islandCount { get; private set; }

        public int globalPopulationSize { get; private set; }
        public int maxNoChangeDetections { get; private set; }

        public APopulationCrossover<bool> populationCrossover { get; private set; }
        public IslandModel(IEvaluation<bool> evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, ACrossover crossover, IMutation<bool> mutation, 
                                int globalPopulationSize, int islandCount, int maxNoChangeDetections,
                                APopulationCrossover<bool> populationCrossover)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, 0)
        {
            this.islandCount = islandCount;
            this.globalPopulationSize = globalPopulationSize;
            maxNoChangeDetections = maxNoChangeDetections;
            this.populationCrossover = populationCrossover;
            if (globalPopulationSize % islandCount != 0)
            {
                throw new Exception("liczba wysp jest nie podzielna przez wielkosc calej populacji");
            }
        }

        public override void Run()
        {
            base.Run();
            ExtractIslands();
        }

        protected override void Initialize(DateTime startTime)
        {
            islands = new List<GAStallDetection<bool>>();
            for (int i = 0; i < islandCount; i++)
            {
                var GASD = new GAStallDetection<bool>(Evaluation, StopCondition, generator, selection, crossover, mutation,
                    globalPopulationSize / islandCount, maxNoChangeDetections);
                GASD.Initialize();
                islands.Add(GASD);
            }
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            bool anyFoundNewBest = false;
            foreach (var ga in islands)
            {
                var t = ga.RunIteration();
                if (t) anyFoundNewBest = true;
            }

            IslandCrossover();

            return anyFoundNewBest;
        }

        private void IslandCrossover()
        {
            for (int i = 0; i < islands.Count; i++)
            {
                if (islands[i].StallDetected)
                {
                    var otherIslandPopulations = new List<List<Individual<bool>>>();
                    for (int j = 0; j < islandCount; j++)
                    {
                        if (i != j)
                        {
                            otherIslandPopulations.Add(islands[i].population);
                        }
                    }
                    List<Individual<bool>> newIslandPopulation = populationCrossover.Crossover(islands[i].population, otherIslandPopulations);
                    islands[i].population = newIslandPopulation;
                }
            }
        }

        private void ExtractIslands()
        {
            population = new List<Individual<bool>>();
            foreach (var island in islands)
            {
                population.AddRange(island.population);
            }
        }
    }
}
