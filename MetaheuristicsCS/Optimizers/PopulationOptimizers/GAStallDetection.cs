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
    class GAStallDetection<Element> : GeneticAlgorithm<Element>
    {
        public bool StallDetected { get; private set; } = false; 
        public int NoChangeDetected = 0;
        public int MaxNCD { get; private set; }
        public GAStallDetection(IEvaluation<Element> evaluation, AStopCondition stopCondition, AGenerator<Element> generator,
                                ASelection selection, ACrossover crossover, IMutation<Element> mutation, int populationSize, 
                                int maxNoChangeDetections)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, populationSize)
        {
            MaxNCD = maxNoChangeDetections;
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            bool foundNewBest = base.RunIteration(itertionNumber, startTime);
            NoChangeDetected = !foundNewBest ? NoChangeDetected + 1 : 0;
            StallDetected = NoChangeDetected >= MaxNCD;
            if (StallDetected) Console.WriteLine("Stall detected in iteration: " + itertionNumber.ToString() );
            return foundNewBest;
        }
    }
}
