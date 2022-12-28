using Julia.NET.Stdlib;
using NUnit.Framework;

namespace JuliadotNETTest
{
    public class Tests
    {
        [OneTimeSetUp]
        public void Setup() => Julia.NET.Core.Julia.Init();

        [OneTimeTearDown]
        public void TearDown() => Julia.NET.Core.Julia.Exit();

        [Test]
        public void JuliaInitialized() => Assert.True(Julia.NET.Core.Julia.IsInitialized, "Julia Not Initialized");

        [Test]
        public void TypeConversion()
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual((int)(new Any(5) + 2), 5 + 2, "Integer Conversion Failure");
                Assert.AreEqual((string)new Any("Hi"), "Hi", "String Conversion Failure");
                Assert.AreEqual(new Any(new[] { 2, 3, 4 }).Length, 3, "Array Conversion Failure");
            });
        }

        [Test]
        public void ArrayMath()
        {
            var f = Julia.NET.Core.Julia.Eval(@"add!(m1, m2) = m1 .+= m2");
            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };
            f.Invoke(new Any(m1), new Any(m2));
            Assert.AreEqual(m1, new[] { 5, 7, 9 }, "Array Add Failure");
        }
    }
}
