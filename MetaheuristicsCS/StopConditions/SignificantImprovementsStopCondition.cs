using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.StopConditions
{
    class SignificantImprovementsStopCondition : AStopCondition
    {
        public readonly long maxIterationNumber;
        public readonly double improvementEpsilon;
        private double previousBestValue;

        public SignificantImprovementsStopCondition(double maxValue, long maxIterationNumber, double improvementEpsilon = 1E-1, double epsilon = double.Epsilon)
            : base(maxValue, epsilon)
        {
            this.maxIterationNumber = maxIterationNumber;
            this.improvementEpsilon = improvementEpsilon;
            reset();
        }

        public void reset()
        {
            previousBestValue = Double.MinValue;
        }

        public override bool Stop(double bestValue, long iterationNumber, long FFE, DateTime startTime)
        {
            if (previousBestValue - bestValue > improvementEpsilon)
            {
                previousBestValue = bestValue;
                return Stop(iterationNumber, FFE, startTime);
            }
            else
            {
                reset();
                return true;
            }
        }

        protected override bool Stop(long iterationNumber, long FFE, DateTime startTime)
        {
            return iterationNumber >= maxIterationNumber;
        }
    }
}
