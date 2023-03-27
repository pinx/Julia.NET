using JuliaNET.Stdlib;

namespace TestStandard.Nunit
{
    public class NunitAnyEqualityComparer
    {
        public static bool AreEqual(Any x,
                                    Any y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }
    }
}
