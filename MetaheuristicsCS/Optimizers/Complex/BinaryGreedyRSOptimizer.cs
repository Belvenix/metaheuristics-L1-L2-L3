using EvaluationsCLI;
using Generators;
using Optimizers;
using StopConditions;
using System;
using System.Collections.Generic;

namespace MetaheuristicsCS.Optimizers.Complex
{
    class BinaryGreedyRSOptimizer : AOptimizer<bool>
    {
        private readonly AGenerator<bool> generator;

        //ograniczenie seta
        public BinaryGreedyOptimizer greedyOptimizer { get; private set; }

        public BinaryGreedyRSOptimizer(IEvaluation<bool> evaluation, AStopCondition stopCondition, 
            AStopCondition greedyStopCondition, int? seed = null)
            : base(evaluation, stopCondition)
        {
            generator = new BinaryRandomGenerator(evaluation.pcConstraint, seed);
            greedyOptimizer = new BinaryGreedyOptimizer(evaluation, null, greedyStopCondition, seed);
        }

        protected override void Initialize(DateTime startTime)
        {

        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            List<bool> solution = generator.Create(Evaluation.iSize);
            
            greedyOptimizer.setSolution(solution);

            greedyOptimizer.Run();

            solution = greedyOptimizer.Result.BestSolution;

            return CheckNewBest(solution, Evaluation.dEvaluate(solution));
        }
    }
}
