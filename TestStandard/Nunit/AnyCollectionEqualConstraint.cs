using System.Collections.Generic;
using JuliaNET.Stdlib;
using NUnit.Framework.Constraints;

namespace TestStandard.Nunit
{
    public class AnyCollectionEqualConstraint : Constraint
    {
        private readonly IEnumerable<Any> _expected;

        public AnyCollectionEqualConstraint(IEnumerable<Any> expected) : base(expected)
        {
            _expected = expected;
        }

        public override string Description => string.Join(", ", _expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is IEnumerable<Any> any)
            {
                return new ConstraintResult(this, actual, NunitAnyCollectionEqualityComparer.AreEqual(_expected, any));
            }

            return null;
        }
    }
}
