using System;
using System.Collections.Generic;

using EvaluationsCLI;
using MathNet.Numerics;
using StopConditions;
using Utility;

namespace Optimizers
{
    abstract class AOptimizer<Element>
    {
        public OptimizationResult<Element> Result { get; protected set; }

        // Dodano gettera aby mozna bylo pobierac do zapisu do pliku
        public IEvaluation<Element> Evaluation { get; protected set; }

        // Dodano gettera aby mozna bylo pobierac do zapisu do pliku
        public AStopCondition StopCondition { get; protected set; }

        private long iterationNumber;
        private DateTime startTime;
        public bool divergenceException {get; protected set;}

        public AOptimizer(IEvaluation<Element> evaluation, AStopCondition stopCondition)
        {
            Result = null;
            this.Evaluation = evaluation;
            this.StopCondition = stopCondition;
            this.divergenceException = false;
        }

        public void Initialize()
        {
            Result = null;
            iterationNumber = 0;
            startTime = DateTime.UtcNow;

            Initialize(startTime);
        }

        public bool RunIteration()
		{
			return RunIteration(iterationNumber++, startTime);
		}

        public bool ShouldStop()
        {
            return StopCondition.Stop((Result != null) ? Result.BestValue : double.NegativeInfinity, iterationNumber, Evaluation.iFFE, startTime);
        }

        public void Run()
        {
            Initialize();

            while (!ShouldStop())
            {
                try
                {
                    RunIteration();
                }
                catch (NonConvergenceException exception)
                {
                    Console.WriteLine(exception.GetType());
                    break;
                }
            }
        }

        protected abstract void Initialize(DateTime startTime);
        protected abstract bool RunIteration(long itertionNumber, DateTime startTime);

        protected bool CheckNewBest(List<Element> solution, double value, bool onlyImprovements = true)
        {
            if (Result == null || value > Result.BestValue || value == Result.BestValue && !onlyImprovements)
            {
                Result = new OptimizationResult<Element>(value, solution, iterationNumber, Evaluation.iFFE, TimeUtils.DurationInSeconds(startTime));

                return true;
            }

            return false;
        }
    }
}
