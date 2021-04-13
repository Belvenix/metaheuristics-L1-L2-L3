using EvaluationsCLI;
using Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Mutations
{
    class RealDomainKnowledgeIgnSimAnnMutationAdaptation : RealIgnitingSimAnnMutationAdaptation
    {
        private readonly IEvaluation<double> problem;

        // Mniejszy initialScopePercent jest spowodowany tym, ze metoda może się rozpędzać
        public RealDomainKnowledgeIgnSimAnnMutationAdaptation(RealGaussianMutation mutation, IEvaluation<double> problem)
            : this(mutation, problem, .1, 1000, .05, 1.1)
        {

        }

        public RealDomainKnowledgeIgnSimAnnMutationAdaptation(RealGaussianMutation mutation, IEvaluation<double> problem, 
            double initialScopePercent = .1, double startTemperature = 1000, double temperatureDropCoef = .05, double ignitingCoefficient = 1.1)
            : base(mutation, startTemperature, temperatureDropCoef, ignitingCoefficient)
        {
            this.problem = problem;

            // Resetujemy tutaj wartosci sigm, ktore zostaly przemnozone w podstawowej funkcji
            mutation.MultiplySigmas(1 / startTemperature);

            // Inicjalizacja sigm za pomocą wiedzy o problemie
            for (int i = 0; i < problem.iSize; i++)
            {
                mutation.Sigma(i, initialScopePercent * Math.Abs(problem.pcConstraint.tGetUpperBound(i) - problem.pcConstraint.tGetLowerBound(i)));
            }
        }
    }
}
