using EvaluationsCLI;
using Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Mutations
{
    class RealDomainKnowledgeSimAnnAdaptationMutation : RealSimulatedAnnealingMutationAdaptation
    {
        private readonly IEvaluation<double> problem;

        public RealDomainKnowledgeSimAnnAdaptationMutation(RealGaussianMutation mutation, IEvaluation<double> problem)
            :this(mutation, problem, .25, 1000, .05)
        {
        }

        public RealDomainKnowledgeSimAnnAdaptationMutation(RealGaussianMutation mutation, IEvaluation<double> problem, double initialScopePercent = .25, double startTemperature = 1000, double temperatureDropCoef = .05)
            : base(mutation, startTemperature, temperatureDropCoef)
        {
            this.problem = problem;

            // Resetujemy tutaj wartosci sigm, ktore zostaly przemnozone w podstawowej funkcji
            mutation.MultiplySigmas(1/startTemperature);

            // Inicjalizacja sigm za pomocą wiedzy o problemie
            for (int i = 0; i < problem.iSize; i++)
            {
                mutation.Sigma(i, initialScopePercent * Math.Abs(problem.pcConstraint.tGetUpperBound(i) - problem.pcConstraint.tGetLowerBound(i)));
            }
        }
    }
}
