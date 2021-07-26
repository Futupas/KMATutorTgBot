using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Helpers
{
    /// <summary>
    /// Useless class, but it can be useful in future
    /// </summary>
    internal static class MultiDimensionalToOneDImensionalHelper
    {
        public static IEnumerable<T> Convert2DimensionalTo1Dimensional<T>(IEnumerable<IEnumerable<T>> arr)
        {
            if (arr == null) throw new ArgumentNullException("Your array cannot be null");
            foreach (var arrSub in arr)
            {
                foreach (var el in arrSub)
                {
                    yield return el;
                }
            }
        }
    }
}
