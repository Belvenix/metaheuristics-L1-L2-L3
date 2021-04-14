using Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Mutations
{
    class RealSimulatedAnnealingMutationAdaptation : ARealMutationES11Adaptation
    {
        public readonly double temperatureDropCoef;
        public readonly double startingTemperature;

        public RealSimulatedAnnealingMutationAdaptation(RealGaussianMutation mutation)
            : this(mutation, 1000, .05)
        {

        }

        public RealSimulatedAnnealingMutationAdaptation(RealGaussianMutation mutation, double startingTemperature = 1000, double temperatureDropCoef = .02)
            : base(mutation)
        {

            this.temperatureDropCoef = temperatureDropCoef > 0 && temperatureDropCoef < 1 ? temperatureDropCoef : .02;
            this.startingTemperature = startingTemperature > 0 ? startingTemperature : 1000;

            Mutation.MultiplySigmas(startingTemperature);
        }

        public override void Adapt(double beforeMutationValue, List<double> beforeMutationSolution, double afterMutationValue, List<double> afterMutationSolution)
        {
            Mutation.MultiplySigmas(1 - temperatureDropCoef);
        }
    }
}
