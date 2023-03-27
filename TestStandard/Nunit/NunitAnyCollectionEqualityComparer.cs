using System.Collections.Generic;
using JuliaNET.Stdlib;

namespace TestStandard.Nunit
{
    public class NunitAnyCollectionEqualityComparer
    {
        public static bool AreEqual(IEnumerable<Any> x,
                                    IEnumerable<Any> y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            using var expectedEnum = x.GetEnumerator();
            using var actualEnum = y.GetEnumerator();

            while (expectedEnum.MoveNext() && actualEnum.MoveNext())
            {
                if (expectedEnum.Current != actualEnum.Current)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
