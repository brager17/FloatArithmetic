using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FloatArithmetic;
using Xunit;

namespace DynamicTest.Tests
{
    public class FloatOperationTests
    {
        private const int bit = 4;
        FloatOperation floatOperation = new FloatOperation() {bit = bit};

        [Fact]
        public void A()
        {
        }

        [Fact]
        public void Simple()
        {
            var a = new byte[] {0, 1, 0, 1};
            var b = new byte[] {0, 0, 1, 0};
            var r = floatOperation.Add(a, b);
            Assert.Equal(new byte[] {0, 1, 1, 1}, r);
            var rAsInt = floatOperation.AddResultAsInt(a, b);
            Assert.Equal(7, rAsInt);
        }


        [Fact]
        public void PositivePlusNegative()
        {
            var a = new byte[] {0, 1, 0, 1};
            var b = new byte[] {1, 0, 1, 0};
            var r = floatOperation.Add(a, b);
            Assert.Equal(new byte[] {0, 0, 1, 1}, r);
            var rAsInt = floatOperation.AddResultAsInt(a, b);
            Assert.Equal(3, rAsInt);
        }

        public static IEnumerable<object[]> MassiveAddTestData()
        {
            return from firstSign in new byte[] {0, 1}
                from i1 in new byte[] {0, 1}
                from i2 in new byte[] {0, 1}
                from i3 in new byte[] {0, 1}
                from secondSigh in new byte[] {0, 1}
                from j1 in new byte[] {0, 1}
                from j2 in new byte[] {0, 1}
                from j3 in new byte[] {0, 1}
                select new object[] {firstSign, i1, i2, i3, secondSigh, j1, j2, j3};
        }

        [Theory]
        [MemberData(nameof(MassiveAddTestData))]
        public void MassiveAddTest(byte firstSign, byte i1, byte i2, byte i3, byte secondSigh, byte j1, byte j2,
            byte j3)
        {
            var first = firstSign == 1 && i1 == 0 && i2 == 0 && i3 == 0
                ? 1 << (bit - 1)
                : i1 * 4 + i2 * 2 + i3;

            var second = secondSigh == 1 && j1 == 0 && j2 == 0 && j3 == 0
                ? 1 << (bit - 1)
                : j1 * 4 + j2 * 2 + j3;

            second = secondSigh == 1 ? -second : second;
            first = firstSign == 1 ? -first : first;

            var result = first + second;


            if (result >= 8 || result < -8)
            {
                Assert.Throws<Exception>(() =>
                    floatOperation.AddResultAsInt(
                        new[] {firstSign, i1, i2, i3},
                        new[] {secondSigh, j1, j2, j3}
                    ));
            }

            else
            {
                var actual = floatOperation.AddResultAsInt(
                    new[] {firstSign, i1, i2, i3},
                    new[] {secondSigh, j1, j2, j3}
                );

                Assert.Equal(result, actual);
            }
        }

        [Fact]
        public void MassiveSubTest()
        {
            foreach (var firstSign in new byte[]
            {
                0, 1
            })
            {
                foreach (var i1 in new byte[] {0, 1})
                {
                    foreach (var i2 in new byte[] {0, 1})
                    {
                        foreach (var i3 in new byte[] {0, 1})
                        {
                            foreach (var secondSigh in new byte[] {0, 1})
                            {
                                foreach (var j1 in new byte[] {0, 1})
                                {
                                    foreach (var j2 in new byte[] {0, 1})
                                    {
                                        foreach (var j3 in new byte[] {0, 1})
                                        {
                                            if (firstSign == 1 && i1 == 0 && i2 == 0 && i3 == 0)
                                                continue;
                                            if (secondSigh == 1 && j1 == 0 && j2 == 0 && j3 == 0)
                                                continue;
                                            var first = i1 * 4 + i2 * 2 + i3;
                                            var second = j1 * 4 + j2 * 2 + j3;

                                            var result = (firstSign, secondSigh) switch
                                            {
                                                (0, 0) => first - second,
                                                (0, 1) => first - -second,
                                                (1, 0) => -first - second,
                                                (1, 1) => -first - -second
                                            };


                                            try
                                            {
                                                if (result >= 8 || result <= -8)
                                                {
                                                    Assert.Throws<Exception>(() =>
                                                        floatOperation.SubResultAsInt(
                                                            new[] {firstSign, i1, i2, i3},
                                                            new[] {secondSigh, j1, j2, j3}
                                                        ));
                                                }

                                                else
                                                {
                                                    var actual = floatOperation.SubResultAsInt(
                                                        new[] {firstSign, i1, i2, i3},
                                                        new[] {secondSigh, j1, j2, j3}
                                                    );

                                                    Assert.Equal(result, actual);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static object[][] GenerateDataForShifts()
        {
            var result = new[]
            {
                new object[] {0},
                new object[] {1},
                new object[] {2},
                new object[] {3},
            };
            return result;
        }

        [Fact]
        public void SimpleShift()
        {
            var actual = floatOperation.ShiftRight(new byte[] {1, 1, 1, 1}, 2);
            var actualValue = floatOperation.ToNumber(actual);
            Assert.Equal(-1, actualValue);
        }

        [MemberData(nameof(GenerateDataForShifts))]
        [Theory]
        public void ShiftRightTest(int shift)
        {
            var r = floatOperation.ToNumber(new byte[] {1, 1, 1, 0});
            foreach (var firstSign in new byte[]
            {
                0, 1
            })
            {
                foreach (var i1 in new byte[] {0, 1})
                {
                    foreach (var i2 in new byte[] {0, 1})
                    {
                        foreach (var i3 in new byte[] {0, 1})
                        {
                            if (firstSign == 1 && i1 == 0 && i2 == 0 && i3 == 0)
                                continue;

                            shift = shift < 0 ? shift : -shift;
                            var arr = new[] {firstSign, i1, i2, i3};
                            var copy = new byte[arr.Length];
                            arr.CopyTo((Span<byte>) copy);
                            var num = i1 * 4 + i2 * 2 + i3 * 1;
                            if (firstSign == 1)
                            {
                                num = -num;
                            }

                            var expected = shift > 0 ? num >> shift : num << -shift;

                            arr = floatOperation.AdditionCode(arr);
                            copy = floatOperation.AdditionCode(copy);

                            int number;
                            byte[] actual;
                            try
                            {
                                if (expected < -8 || expected >= 8)
                                {
                                    var exception = Assert.Throws<Exception>(
                                        () => shift > 0
                                            ? floatOperation.ShiftRight(arr, (byte) shift)
                                            : floatOperation.LeftShift(arr, (byte) -shift)
                                    );

                                    Assert.Equal("Overflow", exception.Message);
                                }
                                else
                                {
                                    actual = shift > 0
                                        ? floatOperation.ShiftRight(arr, (byte) shift)
                                        : floatOperation.LeftShift(arr, (byte) -shift);

                                    number = floatOperation.ToNumber(actual);
                                    Assert.Equal(expected, number);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);

                                actual = shift > 0
                                    ? floatOperation.ShiftRight(copy, (byte) shift)
                                    : floatOperation.LeftShift(copy, (byte) -shift);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void SimpleLeftShift()
        {
            var bytes = new byte[] {1, 1, 0, 1};
            var actual = floatOperation.LeftShift(bytes, 1);
            Assert.Equal(new byte[]
            {
                1, 0, 1, 0
            }, actual);
        }

        [Fact]
        public void OverflowLeftShift()
        {
            var bytes = new byte[] {1, 0, 0, 1};
            var exception = Assert.Throws<Exception>(() => floatOperation.LeftShift(bytes, 1));
            Assert.Equal("Overflow", exception.Message);
        }

        [Fact]
        public void AdditionalCodeMinimumNumber__ReturnsMinimumNumber()
        {
            var bytes = new byte[] {1, 0, 0, 0};
            var additionCode = floatOperation.AdditionCode(bytes);
            Assert.Equal(bytes, additionCode);
        }

        [MemberData(nameof(GenerateDataForShifts))]
        [Theory]
        public void MassLeftShift(int shift)
        {
            var r = floatOperation.ToNumber(new byte[] {1, 1, 1, 0});
            foreach (var firstSign in new byte[]
            {
                0, 1
            })
            {
                foreach (var i1 in new byte[] {0, 1})
                {
                    foreach (var i2 in new byte[] {0, 1})
                    {
                        foreach (var i3 in new byte[] {0, 1})
                        {
                            if (firstSign == 1 && i1 == 0 && i2 == 0 && i3 == 0)
                                continue;

                            var arr = new[] {firstSign, i1, i2, i3};
                            var copy = new byte[arr.Length];
                            arr.CopyTo((Span<byte>) copy);

                            var num = i1 * 4 + i2 * 2 + i3 * 1;
                            if (firstSign == 1)
                            {
                                num = -num;
                            }

                            var expected = num << shift;

                            arr = floatOperation.AdditionCode(arr);
                            copy = floatOperation.AdditionCode(copy);

                            int number;
                            byte[] actual;
                            try
                            {
                                if (expected < -8 || expected >= 8)
                                {
                                    var exception = Assert.Throws<Exception>(
                                        () => floatOperation.LeftShift(arr, (byte) shift)
                                    );

                                    Assert.Equal("Overflow", exception.Message);
                                }
                                else
                                {
                                    actual = floatOperation.LeftShift(arr, (byte) shift);

                                    number = floatOperation.ToNumber(actual);
                                    Assert.Equal(expected, number);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);

                                actual = floatOperation.LeftShift(copy, (byte) shift);
                            }
                        }
                    }
                }
            }
        }
    }
}