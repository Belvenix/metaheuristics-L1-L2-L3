using Crossovers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace MetaheuristicsCS.Crossovers
{
    class DSMCrossover : ACrossover
    {
        private readonly UniformIntegerRandom geneRNG;
        public double[,] DsmMatrix;
        public readonly double Threshold;
        public DSMCrossover(double probability, double threshold, double[,] dsmMatrix, int? seed = null)
            : base(probability, seed)
        {
            geneRNG = new UniformIntegerRandom(seed);
            Threshold = threshold;
            DsmMatrix = dsmMatrix;
        }


        protected override bool Cross<Element>(List<Element> parent1, List<Element> parent2, List<Element> offspring1, List<Element> offspring2)
        {
            // W pierwszej iteracji i tak nie mamy macierzy dsm!
            if (DsmMatrix == null)
            {
                return false;
            }
            int len = parent1.Count;
            int sg1 = geneRNG.Next(0, len), sg2 = geneRNG.Next(0, len);
            List<int> candidates1 = new List<int>(), candidates2 = new List<int>();

            for (int i = 0; i < len; i++)
            {
                if (sg1 != i && DsmMatrix[sg1, i] >= Threshold)
                {
                    candidates1.Add(i);
                }
                if (sg2 != i && DsmMatrix[sg2, i] >= Threshold)
                {
                    candidates2.Add(i);
                }
            }

            foreach (var c in candidates1)
            {
                offspring1[c] = parent2[c];
            }

            foreach (var c in candidates2)
            {
                offspring2[c] = parent1[c];
            }

            // Ustawiamy dsm na null, zeby nie dalo sie dwa razy tej samej uzyc!
            DsmMatrix = null;

            return true;
        }
    }
}
