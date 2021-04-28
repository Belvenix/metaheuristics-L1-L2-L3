using System.Collections.Generic;

using EvaluationsCLI;
using Mutations;

namespace Optimizers.PopulationOptimizers
{
    class Individual<Element>
    {
        protected bool evaluated;

        public List<Element> Genotype { get; protected set; }
        public double Fitness { get; protected set; }

        public Individual(List<Element> genotype)
        {
            evaluated = false;

            Genotype = genotype;
            Fitness = double.NegativeInfinity;
        }

        public Individual(Individual<Element> other)
        {
            evaluated = other.evaluated;

            Genotype = new List<Element>(other.Genotype);
            Fitness = other.Fitness;
        }

        public virtual double Evaluate(IEvaluation<Element> evaluation)
        {
            if(!evaluated)
            {
                Fitness = evaluation.dEvaluate(Genotype);
                evaluated = true;
            }

            return Fitness;
        }

        public bool Mutate(IMutation<Element> mutation)
        {
            if(mutation.Mutate(Genotype))
            {
                evaluated = false;

                return true;
            }

            return false;
        }
    }
}
