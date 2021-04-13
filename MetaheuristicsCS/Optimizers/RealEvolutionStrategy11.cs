using System;
using System.Collections.Generic;

using EvaluationsCLI;
using Generators;
using Mutations;
using StopConditions;

namespace Optimizers
{
    class RealEvolutionStrategy11 : AOptimizer<double>
    {
        // Akcesory zmienione do zapisu do pliku
        public readonly ARealMutationES11Adaptation mutationAdaptation;
        private readonly RealRandomGenerator randomGeneration;

        public RealEvolutionStrategy11(IEvaluation<double> evaluation, AStopCondition stopCondition, ARealMutationES11Adaptation mutationAdaptation, int? seed = null)
            : base(evaluation, stopCondition)
        {
            this.mutationAdaptation = mutationAdaptation;
            randomGeneration = new RealRandomGenerator(evaluation.pcConstraint, seed);
        }

        protected override void Initialize(DateTime startTime)
        {
            List<double> initialSolution = randomGeneration.Create(Evaluation.iSize);

            CheckNewBest(initialSolution, Evaluation.dEvaluate(initialSolution));
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            List<double> candidateSolution = new List<double>(Result.BestSolution);

            mutationAdaptation.Mutation.Mutate(candidateSolution);
            double candidateValue = Evaluation.dEvaluate(candidateSolution);

            mutationAdaptation.Adapt(Result.BestValue, Result.BestSolution, candidateValue, candidateSolution);

            return CheckNewBest(candidateSolution, candidateValue);
        }
    }
}
