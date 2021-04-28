using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    class Shuffler
    {
        private readonly UniformIntegerRandom rng;

        public Shuffler(int? seed = null)
        {
            rng = new UniformIntegerRandom(seed);
        }

        public void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; --i)
            {
                Utils.Swap(list, i, rng.Next(i + 1));
            }
        }

        public List<int> GenereteShuffledOrder(int listCount, Random rnd)
        {
            List<int> optOrder = Enumerable.Range(0, listCount).ToList();
            for (var i = optOrder.Count; i > 0; i--)
                Swap(optOrder, 0, rnd.Next(0, i));
            return optOrder;
        }

        private void Swap(List<int> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
