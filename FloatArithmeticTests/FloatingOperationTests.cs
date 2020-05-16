using FloatArithmetic;
using Xunit;

namespace DynamicTest.Tests
{
    public class FloatingOperationTests
    {
        [Fact]
        public void AddWithoutOverflow()
        {
            var floatOp = new IE754Operations();

            // 12.75 + 3 = 15.75
            // 1100.11 + 11.00 = 10000.11 Fixed 
            // 0_10000010_10011000000000000000000 +0_10000000_10000000000000000000000 =
            // 0_10000010_10011000000000000000000 +0_10000010_01100000000000000000000 =
            // 0_10000010_11111000000000000000000 Float

            var actual = floatOp.FloatAdd(0,
                new byte[] {1, 0, 0, 0, 0, 0, 1, 0},
                new byte[] {1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                //
                0, new byte[] {1, 0, 0, 0, 0, 0, 0, 0},
                new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            );

            Assert.Equal(0, actual.sign);
            Assert.Equal(new byte[] {1, 0, 0, 0, 0, 0, 1, 0}, actual.exp);
            Assert.Equal(new byte[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                actual.mantissa);
        }


        [Fact]
        public void AddWithOverflow()
        {
            var floatOp = new IE754Operations();

            // 12.75 + 3.50 = 16.25
            // 1100.11 + 11.1 = 10000.01 Fixed 
            // 0_10000010_10011000000000000000000 +0_10000000_11000000000000000000000 =
            // 0_10000010_10011000000000000000000 +0_10000010_00110000000000000000000 =
            // 0_10000011_00000100000000000000000 Float

            var actual = floatOp.FloatAdd(0,
                new byte[] {1, 0, 0, 0, 0, 0, 1, 0},
                new byte[] {1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                //
                0, new byte[] {1, 0, 0, 0, 0, 0, 0, 0},
                new byte[] {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            );

            Assert.Equal(0, actual.sign);
            Assert.Equal(new byte[] {1, 0, 0, 0, 0, 0, 1, 1}, actual.exp);
            Assert.Equal(new byte[] {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                actual.mantissa);
        }
    }
}