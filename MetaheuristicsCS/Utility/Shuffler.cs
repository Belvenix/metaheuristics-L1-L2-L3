using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaheuristicsCS.Utility
{
    static class Shuffler
    {

        public static List<int> GenereteShuffledOrder(int listCount, Random rnd)
        {
            List<int> optOrder = Enumerable.Range(0, listCount).ToList();
            for (var i = optOrder.Count; i > 0; i--)
                Swap(optOrder, 0, rnd.Next(0, i));
            return optOrder;
        }

        private static void Swap(List<int> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
