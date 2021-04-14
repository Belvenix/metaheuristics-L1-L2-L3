using System.Collections.Generic;

using EvaluationsCLI;
using Utility;

namespace Mutations
{
    class RealGaussianMutation : ARandomMutation<double>
    {
        private readonly BoolRandom uniformRNG;
        private readonly NormalRealRandom gaussianRNG;

        private readonly List<double> sigmas;

        public RealGaussianMutation(double sigma, IEvaluationProfile<double> evaluationProfile, int? seed = null, double probability = 1.0)
            : base(probability, evaluationProfile)
        {
            uniformRNG = new BoolRandom(seed);
            gaussianRNG = new NormalRealRandom(seed);

            sigmas = new List<double>(evaluationProfile.iSize);
            for(int i = 0; i < evaluationProfile.iSize; ++i)
            {
                sigmas.Add(sigma);
            }
        }

        public RealGaussianMutation(List<double> sigmas, IEvaluationProfile<double> evaluationProfile, int? seed = null, double probability = 1.0)
            : base(probability, evaluationProfile)
        {
            uniformRNG = new BoolRandom(seed);
            gaussianRNG = new NormalRealRandom(seed);

            this.sigmas = new List<double>(sigmas);
        }

        public override bool Mutate(List<double> solution)
        {
            bool successfulMutation = false;

            for(int i = 0; i < solution.Count; ++i)
            {
                if (uniformRNG.Next(Probability))
                {
                    // Poprawiono kod, ponieważ w poprzednim stanie nie był sprawdzany zakres przez co zdarza sie ze wychodzi poza zakres i nie moze wrocic
                    double tempSolution = 0;
                    int counter = 0;
                    do
                    {
                        counter++;
                        tempSolution = solution[i];
                        tempSolution += gaussianRNG.Next(0.0, sigmas[i]);
                        if (counter > 100) return false;
                    } while (!evaluationProfile.pcConstraint.bIsFeasible(i, tempSolution));
                    solution[i] = tempSolution;
                    //do
                    //{
                    //    solution[i] += gaussianRNG.Next(0.0, sigmas[i]);
                    //} while (!evaluationProfile.pcConstraint.bIsFeasible(i, solution[i]));

                    successfulMutation = true;
                }
            }

            return successfulMutation;
        }

        public void MultiplySigmas(double multiplier)
        {
            for (int i = 0; i < sigmas.Count; ++i)
            {
                sigmas[i] *= multiplier;
            }
        }

        public double Sigmas(int index)
        {
            return sigmas[index];
        }

        public List<double> Sigmas()
        {
            return sigmas;
        }

        public void Sigma(int index, double value)
        {
            sigmas[index] = value;
        }

        public void Sigmas(double sigma)
        {
            for (int i = 0; i < sigmas.Count; ++i)
            {
                sigmas[i] = sigma;
            }
        }
    }
}
