using Crossovers;
using EvaluationsCLI;
using Generators;
using Mutations;
using Optimizers.PopulationOptimizers;
using Selections;
using StopConditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetaheuristicsCS.Optimizers.PopulationOptimizers
{
    class DSMGA : GeneticAlgorithm<bool>
    {
        int[][,] ContingencyMatrixTable;
        protected double[,] DsmMatrix;
        List<List<int>> Combinations;
        bool saveDsm = true;

        static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public DSMGA(IEvaluation<bool> evaluation, AStopCondition stopCondition, AGenerator<bool> generator,
                                ASelection selection, ACrossover crossover, IMutation<bool> mutation, int populationSize)
            : base(evaluation, stopCondition, generator, selection, crossover, mutation, populationSize)
        {
            Combinations = GetKCombs(Enumerable.Range(0, evaluation.iSize), 2)
                .Select(x => x.ToList()).ToList();
            ResetContingencyMatrixTable();
        }

        private void ResetContingencyMatrixTable()
        {
            ContingencyMatrixTable = new int[Combinations.Count][,];
            for (int i = 0; i < Combinations.Count; i++)
            {
                ContingencyMatrixTable[i] = new int[2,2];
            }
            DsmMatrix = new double[Evaluation.iSize, Evaluation.iSize];
            for (int i = 0; i < Evaluation.iSize; i++)
            {
                DsmMatrix[i, i] = 1;
            }
        }

        protected override bool RunIteration(long itertionNumber, DateTime startTime)
        {
            var t = base.RunIteration(itertionNumber, startTime);

            ResetContingencyMatrixTable();
            CalculateContingencyMatrixTable();
            CalculateDsmMatrix();

            if (saveDsm)
            {
                if (itertionNumber % 50 == 0)
                {
                    SaveToFile(@"C:\Users\jbelter\Desktop\metaheuristics-master\metaheuristics-master\wyniki\lab7\lab7-normal-dsm-" + Evaluation.GetType().Name + Evaluation.iSize + "iter" + itertionNumber.ToString() + @".txt", SaveDsmMatrix());
                }
            }

            return t;
        }

        private List<String> SaveDsmMatrix()
        {
            List<String> rows = new List<string>();
            for (int i = 0; i < Evaluation.iSize; i++)
            {
                String row = "";
                for (int j = 0; j < Evaluation.iSize; j++)
                {
                    if (j != 0)
                    {
                        row += ", ";
                    }
                    row += Math.Round(DsmMatrix[i, j], 4).ToString();
                }
                rows.Add(row);
            }
            return rows;
        }

        protected void SaveToFile(String filepath, List<String> lines)
        {
            using (StreamWriter sw = File.CreateText(filepath))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        private void CalculateContingencyMatrixTable()
        {
            foreach (var p in population)
            {
                int i = 0;
                foreach (var c in Combinations)
                {
                    int c0 = c[0], c1 = c[1];
                    AddToContingencyTable(ref ContingencyMatrixTable[i], p.Genotype[c0], p.Genotype[c1]);
                    i++;
                }
            }
        }

        private void CalculateDsmMatrix()
        {
            int i = 0;
            foreach (var c in Combinations)
            {
                int c0 = c[0], c1 = c[1];
                double phi = CalculatePhiCoefficient(ContingencyMatrixTable[i]);
                DsmMatrix[c0, c1] = phi;
                DsmMatrix[c1, c0] = phi;
                i++;
            }
        }
       
        private double CalculatePhiCoefficient(int[,] ct)
        {
            double phi = (ct[0, 0] * ct[1, 1] - ct[0, 1] * ct[1, 0]) /
                Math.Sqrt(
                        (ct[0, 0] + ct[0, 1]) *
                        (ct[1, 0] + ct[1, 1]) *
                        (ct[0, 1] + ct[1, 1]) *
                        (ct[0, 0] + ct[1, 0])
                    );
            return phi;
        }

        private void AddToContingencyTable(ref int[,] t, bool x, bool y)
        {
            int row = x ? 1 : 0;
            int col = y ? 1 : 0;
            t[row, col] += 1;
        }
    }
}
