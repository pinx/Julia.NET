using System.Collections.Generic;
using JuliaNET.Stdlib;

namespace TestStandard.Nunit
{
    public class Is : NUnit.Framework.Is
    {
        public static AnyEqualConstraint EqualToJuliaValue(Any expected)
        {
            return new AnyEqualConstraint(expected);
        }

        public static AnyCollectionEqualConstraint EqualToJuliaEnumerable(IEnumerable<Any> expected)
        {
            return new AnyCollectionEqualConstraint(expected);
        }
    }
}
