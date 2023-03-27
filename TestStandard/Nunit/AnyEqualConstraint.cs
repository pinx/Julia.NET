using JuliaNET.Stdlib;
using NUnit.Framework.Constraints;

namespace TestStandard.Nunit
{
    public class AnyEqualConstraint : Constraint
    {
        private readonly Any _expected;

        public AnyEqualConstraint(Any expected) : base(expected)
        {
            _expected = expected;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is Any any)
            {
                return new ConstraintResult(this, actual, NunitAnyEqualityComparer.AreEqual(_expected, any));
            }

            return null;
        }
    }
}
