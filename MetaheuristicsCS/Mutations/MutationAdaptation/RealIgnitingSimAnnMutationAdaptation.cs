using Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Mutations
{
    class RealIgnitingSimAnnMutationAdaptation : RealSimulatedAnnealingMutationAdaptation
    {
        public readonly double ignitingCoefficient;

        public RealIgnitingSimAnnMutationAdaptation(RealGaussianMutation mutation)
            :this(mutation, 1000, .05, 1.1)
        {

        }

        public RealIgnitingSimAnnMutationAdaptation(RealGaussianMutation mutation, double startingTemperature = 1000, double temperatureDropCoef = .05, double ignitingCoefficient = 1.1)
            : base(mutation, startingTemperature, temperatureDropCoef)
        {
            if (ignitingCoefficient > 1)
            {
                this.ignitingCoefficient = ignitingCoefficient;
            }
            else
            {
                this.ignitingCoefficient = 1.1;
            }
        }

        public override void Adapt(double beforeMutationValue, List<double> beforeMutationSolution, double afterMutationValue, List<double> afterMutationSolution)
        {
            if (afterMutationValue > beforeMutationValue)
            {
                Mutation.MultiplySigmas(ignitingCoefficient);
            }
            base.Adapt(beforeMutationValue, beforeMutationSolution, afterMutationValue, afterMutationSolution);
        }
    }
}
