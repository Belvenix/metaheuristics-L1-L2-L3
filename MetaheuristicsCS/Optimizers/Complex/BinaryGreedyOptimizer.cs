using EvaluationsCLI;
using Generators;
using MetaheuristicsCS.Utility;
using Optimizers;
using StopConditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaheuristicsCS.Optimizers.Complex
{
    class BinaryGreedyOptimizer : AOptimizer<bool>
    {
        private List<bool> optimizedSolution;
        private readonly Random rnd;

        public BinaryGreedyOptimizer(IEvaluation<bool> evaluation, List<bool> solution, AStopCondition stopCondition = null, int? seed = null)
            : base(evaluation, stopCondition ?? new IterationsStopCondition(evaluation.dMaxValue, 1))
        {
            optimizedSolution = solution;
            if (seed == null)
            {
                rnd = new Random();
            }
            else
            {
                rnd = new Random(seed.Value);
            }
        }

        public BinaryGreedyOptimizer(IEvaluation<bool> evaluation, AStopCondition stopCondition = null, int? seed = null)
            : this(evaluation, null, stopCondition, seed)
        {
        }

        public BinaryGreedyOptimizer(IEvaluation<bool> evaluation, int? seed = null)
            : this(evaluation, new IterationsStopCondition(evaluation.dMaxValue, 1), seed)
        {
        }

        public void setSolution(List<bool> solution)
        {
            AGenerator<bool> generator = new BinaryRandomGenerator(Evaluation.pcConstraint, null);
            optimizedSolution = solution ?? generator.Create(Evaluation.iSize);
        }

        protected override void Initialize(DateTime startTime)
        {
            if (optimizedSolution == null)
            {
                setSolution(null);
            }
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            List<int> optOrder = Shuffler.GenereteShuffledOrder(optimizedSolution.Count, rnd);
            foreach (int order in optOrder)
            {
                optimizedSolution[order] = !optimizedSolution[order];
                if (!CheckNewBest(optimizedSolution, Evaluation.dEvaluate(optimizedSolution)))
                {
                    optimizedSolution[order] = !optimizedSolution[order];
                }
            }
            return CheckNewBest(optimizedSolution, Evaluation.dEvaluate(optimizedSolution));
        }
    }
}
