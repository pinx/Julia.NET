using JuliaNET.Core;
using JuliaNET.Stdlib;
using NUnit.Framework;

namespace JuliadotNETTest
{
    public class Tests
    {
        [OneTimeSetUp]
        public void Setup() => Julia.Init();

        [OneTimeTearDown]
        public void TearDown() => Julia.Exit();

        [Test]
        public void JuliaInitialized() => Assert.That(Julia.IsInitialized, Is.True, "Julia Not Initialized");

        [Test]
        public void TypeConversion()
        {
            Assert.Multiple(() =>
            {
                Assert.That((int)(new Any(5) + 2), Is.EqualTo(5 + 2), "Integer Conversion Failure");
                Assert.That((string)new Any("Hi"), Is.EqualTo("Hi"), "String Conversion Failure");
                Assert.That(new Any(new[] { 2, 3, 4 }).Length, Is.EqualTo(3), "Array Conversion Failure");
            });
        }

        [Test]
        public void ArrayMath()
        {
            var f = Julia.Eval(@"add!(m1, m2) = m1 .+= m2");
            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };
            f.Invoke(new Any(m1), new Any(m2));
            Assert.That(m1, Is.EqualTo(new[] { 5, 7, 9 }), "Array Add Failure");
        }
    }
}
