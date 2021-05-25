using Crossovers;
using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Crossovers;
using MetaheuristicsCS.Optimizers.PopulationOptimizers;
using Mutations;
using Selections;
using StopConditions;
using System;

namespace MetaheuristicsCS.Solutions
{
    class GAWithDSM : DSMGA
    {
        public GAWithDSM(IEvaluation<bool> evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, IMutation<bool> mutation, int populationSize,
                                double crossProb, double threshold)
            : base(evaluation, stopCondition, generator, selection, new DSMCrossover(crossProb, threshold, null), mutation, populationSize)
        {

        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            bool foundNewBest = base.RunIteration(itertionNumber, startTime);

            ((DSMCrossover)crossover).DsmMatrix = DsmMatrix;

            return foundNewBest;
        }
    }
}