using System.Collections.Generic;
using System.Linq;
using JuliaNET.Core;
using JuliaNET.Stdlib;
using NUnit.Framework;

namespace TestStandard
{
    public class Tests
    {
        [OneTimeSetUp]
        public void Setup() => Julia.Init();

        [OneTimeTearDown]
        public void TearDown() => Julia.Exit();

        [Test]
        public void JuliaInitialized()
        {
            Assert.That(Julia.IsInitialized, Is.True, "Julia Not Initialized");
        }

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
            var m1 = new[] { 2, 3, 4 };
            var m2 = new[] { 3, 4, 5 };
            JModule mod = Julia.Eval(@"module T; add(m1, m2) = m1 .+ m2;end");
            IEnumerable<Any> y = mod.GetFunction("add").Invoke(new Any(m1), new Any(m2));
            var expected = new[] { 5, 7, 9 }.Select(val => new Any(val));
            Assert.That(y, Nunit.Is.EqualToJuliaEnumerable(expected), "Array Add Failure");
        }

        [Test]
        public void MatrixMath()
        {
            var m1 = new[,] { { 1, 1 }, { 2, 2 } };
            var m2 = new[,] { { 3, 3 }, { 4, 4 } };
            JModule mod = Julia.Eval(@"module T; add(m1, m2) = m1 .+ m2;end");
            IEnumerable<Any> y = mod.GetFunction("add").Invoke(new Any(m1), new Any(m2));
            // Rows and columns are reversed
            var expected = new[,] { { 4, 6 }, { 4, 6 } }.Cast<int>().Select(val => new Any(val));
            Assert.That(y, Nunit.Is.EqualToJuliaEnumerable(expected), "Matrix Add Failure");
        }
    }
}
