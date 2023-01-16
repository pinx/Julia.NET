using JuliaNET.Utils;
using NUnit.Framework;

namespace TestStandard
{
    public class MatrixTest
    {
        [Test]
        public void JaggedArray_ShouldBecomeMatrix()
        {
            var jaggedArray = new[] { new[] { 1, 1 }, new[] { 2, 2 } };
            var matrix = jaggedArray.ToMatrix();
            var expected = new[,] { { 1, 1 }, { 2, 2 } };
            Assert.That(matrix, Is.EqualTo(expected));
        }

        [Test]
        public void RectangularJaggedArray_ShouldBecomeMatrix()
        {
            var jaggedArray = new[] { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 3, 3 } };
            var matrix = jaggedArray.ToMatrix();
            var expected = new[,] { { 1, 1 }, { 2, 2 }, { 3, 3 } };
            Assert.That(matrix, Is.EqualTo(expected));
        }
    }
}
