using System;
using System.Collections.Generic;
using System.Linq;
using EvaluationsCLI;
using MetaheuristicsCS.Mutations;
using Mutations;
using Optimizers;
using StopConditions;

namespace MetaheuristicsCS.Solutions
{
    class Lab2 : ALab<double>
    {
        private List<String> Lab2CheckContinuousProblems(int? seed, bool debug=true)
        {
            List<String> resultData = new List<String>();

            if (debug) Console.WriteLine("Benchmarked problems (No Mutation Adaptation): " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab2CheckAdaptationAgainstContinuousProblems<RealNullRealMutationES11Adaptation>(seed));

            if (debug) Console.WriteLine("Simulated Annealing MA " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab2CheckAdaptationAgainstContinuousProblems<RealSimulatedAnnealingMutationAdaptation>(seed));

            if (debug) Console.WriteLine("Igniting Simulated Annealing MA problems: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab2CheckAdaptationAgainstContinuousProblems<RealIgnitingSimAnnMutationAdaptation>(seed));

            if (debug) Console.WriteLine("Simulated Annealing MA with Domain Knowledge: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<RealDomainKnowledgeSimAnnAdaptationMutation>(seed));

            if (debug) Console.WriteLine("Igniting Simulated Annealing MA with Domain Knowledge: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            resultData.AddRange(Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<RealDomainKnowledgeIgnSimAnnMutationAdaptation>(seed));

            return resultData;
        }

        private List<String> Lab2CheckAdaptationAgainstContinuousProblems<A>(int? seed, int maxIter = 1000) where A : ARealMutationES11Adaptation
        {
            List<String> resultData = new List<String>();

            IEvaluation<double>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                problem.pcConstraint.tGetLowerBound(0); ;
                List<double> sigmas = Enumerable.Repeat(0.1, problem.iSize).ToList();

                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                RealGaussianMutation mutation = new RealGaussianMutation(sigmas, problem, seed);
                var mutationAdaptation = (A)Activator.CreateInstance(typeof(A), mutation);

                RealEvolutionStrategy11 optimizer = new RealEvolutionStrategy11(problem, stopCondition, mutationAdaptation, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        private List<String> Lab2CheckAdaptationAgainstContinuousProblemsWithDomainKnowledge<A>(int? seed, int maxIter = 1000) where A : ARealMutationES11Adaptation
        {
            List<String> resultData = new List<String>();

            IEvaluation<double>[] benchmarkProblems = GenerateProblems();
            foreach (var problem in benchmarkProblems)
            {
                problem.pcConstraint.tGetLowerBound(0); ;
                List<double> sigmas = Enumerable.Repeat(0.1, problem.iSize).ToList();

                IterationsStopCondition stopCondition = new IterationsStopCondition(problem.dMaxValue, maxIter);
                RealGaussianMutation mutation = new RealGaussianMutation(sigmas, problem, seed);
                var mutationAdaptation = (A)Activator.CreateInstance(typeof(A), mutation, problem);

                RealEvolutionStrategy11 optimizer = new RealEvolutionStrategy11(problem, stopCondition, mutationAdaptation, seed);

                optimizer.Run();

                resultData.Add(FormatSave(optimizer));

                ReportOptimizationResult(optimizer.Result);
            }

            return resultData;
        }

        public override void Run(int[] seeds)
        {
            bool debug = true;
            List<String> resultData = new List<String>();
            
            int i = 0;

            Console.WriteLine("Start Lab2: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            foreach (int seed in seeds)
            {
                resultData.AddRange(Lab2CheckContinuousProblems(seed));
                if (debug) Console.WriteLine("Finished " + (i + 1).ToString() + " seed of " + seeds.Length.ToString() + " for " + this.GetType().Name + " " + DateTime.Now.ToString("HH:mm:ss.fff"));
                i++;
            }

            if (debug) Console.WriteLine("Finished second Lab and saving: " + DateTime.Now);
            this.SaveToFile(@"C:\Users\jbelter\Desktop\2021.03.16 metaheuristics-master\metaheuristics-master\lab2.txt", resultData);
        }

        protected override IEvaluation<double>[] GenerateProblems()
        {
            int[] genes = { 2, 5, 10, 25, 50, 75 };
            int nProblems = genes.Length * 6;
            IEvaluation<double>[] problems = new IEvaluation<double>[nProblems];

            int iterator = 0;

            //Sphere
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealSphereEvaluation(gene);
                iterator++;
            }

            //Sphere-10
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealSphere10Evaluation(gene);
                iterator++;
            }

            //Ellipsoid
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealEllipsoidEvaluation(gene);
                iterator++;
            }

            //Step-2 Sphere
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealStep2SphereEvaluation(gene);
                iterator++;
            }

            //Te problemy będą przydatne do 3 laborek

            //Rastrigin
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealRastriginEvaluation(gene);
                iterator++;
            }

            //Ackley
            foreach (var gene in genes)
            {
                problems[iterator] = new CRealAckleyEvaluation(gene);
                iterator++;
            }

            return problems;
        }

        protected override string FormatOptimizerParameters(AOptimizer<double> optimizer)
        {
            if (optimizer.GetType() == typeof(RealEvolutionStrategy11))
            {
                var ma = ((RealEvolutionStrategy11)optimizer).mutationAdaptation;
                String MaName = ma.GetType().Name;
                String parametry = "";

                // C# 7 switch z sprawdzeniem klas
                switch (ma)
                {
                    case RealIgnitingSimAnnMutationAdaptation irsa:
                        parametry += irsa.ignitingCoefficient.ToString() + ", ";
                        parametry += irsa.temperatureDropCoef.ToString() + ", ";
                        parametry += irsa.startingTemperature.ToString();
                        break;

                    case RealSimulatedAnnealingMutationAdaptation rsa:
                        parametry += rsa.temperatureDropCoef.ToString() + ", ";
                        parametry += rsa.startingTemperature.ToString();
                        break;
                    case RealNullRealMutationES11Adaptation rn:
                        parametry += "None";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return MaName + ", " + parametry;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
