using System.Diagnostics;

namespace FloatArithmetic
{
    public class IE754Operations
    {
        public (byte sign, byte[] exp, byte[] mantissa) FloatAdd(byte sign1, byte[] exp1, byte[] mantissa1, byte sign2,
            byte[] exp2, byte[] mantissa2)
        {
            var floatOperationForExponent = new FloatOperation() {bit = 9};

            // constants
            var zero9 = new byte[9];
            var one9 = new byte[9];
            one9[^1] = 1;
            //

            // expansion
            //  exponents expansion
            var expandedExp1 = new byte[9];
            var expandedExp2 = new byte[9];
            exp1.CopyTo(expandedExp1, 1);
            exp2.CopyTo(expandedExp2, 1);

            // mantisses expansion 
            var expandedFirstMantissa = new byte[25];
            expandedFirstMantissa[0] = sign1;
            expandedFirstMantissa[1] = 1;
            mantissa1.CopyTo(expandedFirstMantissa, 2);

            var expandedSecondMantissa = new byte[25];
            expandedSecondMantissa[0] = sign2;
            expandedSecondMantissa[1] = 1;
            mantissa2.CopyTo(expandedSecondMantissa, 2);
            //
            //

            byte[] resultExponent;
            var differentExponent = floatOperationForExponent.SubInAdditionalCode(expandedExp1, expandedExp2);
            var differentExponentSign = differentExponent[0];

            if (differentExponentSign == 1)
            {
                resultExponent = expandedExp2;
                while (!Equals(differentExponent, zero9))
                {
                    expandedFirstMantissa = floatOperationForExponent.ShiftRight(expandedFirstMantissa);
                    differentExponent = floatOperationForExponent.Add(differentExponent, one9);
                }
            }
            else
            {
                resultExponent = expandedExp1;
                while (!floatOperationForExponent.Equals(differentExponent, zero9))
                {
                    expandedSecondMantissa = floatOperationForExponent.ShiftRight(expandedSecondMantissa);
                    differentExponent = floatOperationForExponent.SubInAdditionalCode(differentExponent, one9);
                }
            }


            var floatOperationForMantisses = new FloatOperation {bit = 25};
            var resultMantissa =
                floatOperationForMantisses.Add(expandedFirstMantissa, expandedSecondMantissa, out var overflow);


            byte resultSign;
            if (overflow)
            {
                // overflow can be only the both number has same sign
                Debug.Assert(sign1 == sign2);
                resultSign = sign1;
                resultExponent = floatOperationForExponent.Add(resultExponent, new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 1});
                // when overflow we lost low-order bit
                resultMantissa = resultMantissa[1..^1];
            }
            else
            {
                resultSign = resultMantissa[0];
                resultMantissa = resultMantissa[2..];
            }

            var result = (sign1: resultSign, exponent: resultExponent[1..], mantissa: resultMantissa);
            return result;
        }
    }
}