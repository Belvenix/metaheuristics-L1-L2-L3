using EvaluationsCLI;
using Mutations;
using Optimizers;
using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Optimizers.Complex
{
    class RealES11Scouting : RealEvolutionStrategy11
    {
        public readonly int nScouts;
        public readonly double scoutingMultiplier;
        public bool maxScoutingWalking;

        public RealES11Scouting(IEvaluation<double> evaluation, AStopCondition stopCondition, ARealMutationES11Adaptation mutationAdaptation, int? seed)
            :this(evaluation, stopCondition, mutationAdaptation, seed, 5, 2)
        {

        }

        public RealES11Scouting(IEvaluation<double> evaluation, AStopCondition stopCondition, ARealMutationES11Adaptation mutationAdaptation, int? seed, int nScouts = 5, double scoutingMultiplier = 2)
            :base(evaluation, stopCondition, mutationAdaptation, seed)
        {
            this.nScouts = nScouts;
            this.scoutingMultiplier = scoutingMultiplier;
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            List<double> candidateSolution = new List<double>(Result.BestSolution);

            mutationAdaptation.Mutation.Mutate(candidateSolution);
            double candidateValue = Evaluation.dEvaluate(candidateSolution);

            mutationAdaptation.Adapt(Result.BestValue, Result.BestSolution, candidateValue, candidateSolution);
            var check = CheckNewBest(candidateSolution, candidateValue);

            if (Scout(candidateSolution, itertionNumber))
            {
                mutationAdaptation.Mutation.MultiplySigmas(scoutingMultiplier);
            }
            else
            {
                mutationAdaptation.Mutation.MultiplySigmas(1 / scoutingMultiplier);
            }

            return check;
        }

        private bool Scout(List<double> candidateSolution, long itertionNumber)
        {
            var sigmas = new List<double>(mutationAdaptation.Mutation.Sigmas());
            var tempMutation = new RealGaussianMutation(sigmas, Evaluation);
            if (!maxScoutingWalking)
            {
                tempMutation.MultiplySigmas(itertionNumber);
            }
            else
            {
                tempMutation.MultiplySigmas(itertionNumber/4);
            }
            
            for (int i = 0; i < nScouts; i++)
            {
                List<double> testSolution = new List<double>(candidateSolution);

                maxScoutingWalking = tempMutation.Mutate(testSolution);
                double testValue = Evaluation.dEvaluate(testSolution);
                double bestValue = Result != null ? Result.BestValue : Double.MinValue;
                if (testValue > bestValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
