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
    class IslandModel : GeneticAlgorithm<bool>
    {
        public List<GAStallDetection<bool>> islands { get; private set; }
        public int islandCount { get; private set; }

        public int globalPopulationSize { get; private set; }
        public int maxNCD { get; private set; }
        public IslandModel(IEvaluation<bool> evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, ACrossover crossover, IMutation<bool> mutation, 
                                int globalPopulationSize, int islandCount, int maxNoChangeDetections)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, 0)
        {
            this.islandCount = islandCount;
            this.globalPopulationSize = globalPopulationSize;
            maxNCD = maxNoChangeDetections;
            if (globalPopulationSize % islandCount != 0)
            {
                throw new Exception("liczba wysp jest nie podzielna przez wielkosc calej populacji");
            }
        }

        protected override void Initialize(DateTime startTime)
        {
            islands = new List<GAStallDetection<bool>>();
            for (int i = 0; i < islandCount; i++)
            {
                var GASD = new GAStallDetection<bool>(Evaluation, StopCondition, generator, selection, crossover, mutation, globalPopulationSize / islandCount, maxNCD);
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
                if (t)
                {
                    anyFoundNewBest = true;
                }
            }
            return anyFoundNewBest;
        }

        public override void Run()
        {
            base.Run();
            ExtractIslands();
        }

        private void ExtractIslands()
        {

        }
    }
}
