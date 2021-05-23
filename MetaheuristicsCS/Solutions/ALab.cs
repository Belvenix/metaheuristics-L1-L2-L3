using EvaluationsCLI;
using Optimizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Solutions
{
    abstract class ALab<E>
    {
        protected static void ReportOptimizationResult(OptimizationResult<E> optimizationResult, bool debug = false)
        {
            if (debug) Console.WriteLine("value: {0}", optimizationResult.BestValue);
            if (debug) Console.WriteLine("\twhen (time): {0}s", optimizationResult.BestTime);
            if (debug) Console.WriteLine("\twhen (iteration): {0}", optimizationResult.BestIteration);
            if (debug) Console.WriteLine("\twhen (FFE): {0}", optimizationResult.BestFFE);
            
        }

        protected static String FormatOptimizationResult(OptimizationResult<E> optimizationResult)
        {
            return optimizationResult.BestValue.ToString() + ", " +
                optimizationResult.BestTime.ToString() + ", " +
                optimizationResult.BestIteration.ToString() + ", " +
                optimizationResult.BestFFE.ToString() + ", ";
        }

        protected String FormatSave(AOptimizer<E> optimizer)
        {
            if (!optimizer.divergenceException)
            {
                String optimizerName = optimizer.GetType().Name;
                String optimizerParameters = FormatOptimizerParameters(optimizer);
                String optimizerResult = FormatOptimizationResult(optimizer.Result);
                String problemParameters = FormatProblemParameters(optimizer.Evaluation);

                return optimizerName + ", " +
                    optimizerParameters + ", " +
                    problemParameters + ", " +
                    optimizerResult;
            }
            else
            {
                String optimizerName = optimizer.GetType().Name;
                String optimizerParameters = FormatOptimizerParameters(optimizer);
                String problemParameters = FormatProblemParameters(optimizer.Evaluation);

                return optimizerName + ", " +
                    optimizerParameters + ", " +
                    problemParameters + ", " +
                    "diverged";
            }
        }

        protected void SaveToFile(String filepath, List<String> lines)
        {
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    foreach (var line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    foreach (var line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        public abstract void Run(int[] seeds);

        protected abstract IEvaluation<E>[] GenerateProblems();

        protected abstract String FormatOptimizerParameters(AOptimizer<E> optimizer);

        protected String FormatProblemParameters(IEvaluation<E> problem) 
        {
            String parametry = "";
            parametry += problem.GetType().Name + ", " + problem.iSize.ToString();
            return parametry;
        }
    }
}
