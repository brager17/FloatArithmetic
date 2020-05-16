using System;

namespace FloatArithmetic
{
    public class FloatOperation
    {
        public byte bit = 8;


        public bool Equals(byte[] bytes1, byte[] bytes2)
        {
            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes2[i] != bytes1[i])
                {
                    return false;
                }
            }

            return true;
        }

        // bytes1 and bytes2 represents in addition code
        public bool Greater(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1[0] == 1 && bytes2[0] == 0)
                return false;
            if (bytes1[0] == 0 && bytes2[0] == 1)
                return true;

            var sign = bytes1[0];
            for (int i = 1; i < bytes1.Length; i++)
            {
                if (bytes1[i] > bytes2[i])
                {
                    return true;
                }
            }

            return false;
        }

        public byte[] ShiftRight(byte[] bytes, byte count = 1)
        {
            if (count == 0) return bytes;

            var shiftCount = Math.Min(bytes.Length, count);
            var signByte = bytes[0];

            for (int i = bytes.Length - 1; i - shiftCount > 0; i--)
            {
                bytes[i] = bytes[i - shiftCount];
            }

            for (int i = 0; i < shiftCount; i++)
            {
                bytes[1 + i] = signByte;
            }

            return bytes;
        }

        public byte[] LeftShift(byte[] bytes, byte count = 1)
        {
            if (count == 0) return bytes;

            for (int i = 0; i < count; i++)
            {
                bytes = LeftShiftByOne(bytes);
            }

            return bytes;
        }

        public byte[] LeftShiftByOne(byte[] bytes)
        {
            if (CheckOverflow(bytes[0], bytes[1]))
            {
                throw new Exception("Overflow");
            }

            for (int i = 0; i < bytes.Length - 1; i++)
            {
                bytes[i] = bytes[i + 1];
            }

            bytes[^1] = 0;

            return bytes;
        }

        // both of number positive
        // public byte[] SubDirectCode(byte[] one, byte[] two, out bool isOverflow)
        // {
        //     
        // }

        public byte[] AddDirectCode(byte[] one, byte[] two, out bool isOverflow)
        {
            var result = AddElementsOfTwoArrays(one, two, out var carry, out var higherDigitCarry);
            isOverflow = CheckOverflow(carry, higherDigitCarry);
            return result;
        }

        public byte[] SubInAdditionalCode(byte[] one, byte[] two)
        {
            var copyOne = new byte[one.Length];
            one.CopyTo(copyOne,0);
            var copyTwo = new byte[two.Length];
            two.CopyTo(copyTwo,0);
            
            // expensive check
            if (!IsNull(copyTwo))
            {
                copyTwo[0] = (byte) (copyTwo[0] == 1 ? 0 : 1);
            }

            var result = Add(copyOne, copyTwo);
            return result;
        }


        public byte[] AdditionCode(byte[] target)
        {
            if (target[0] == 0) return target;
            var reverted = Revert(target);
            var arr = new byte[bit];
            arr[^1] = 1;
            // throwIIOverflow is false for minimum number, bit = 8, AdditionCode(1000) = 1000, it's a different way for minimum number
            // Addition(1000) = 1(111+1) = 1(1000) <- overflow, if ignore it we would get (1000) and return this number  
            var result = Add(reverted, arr, false);
            result[0] = 1;
            return result;
        }

        public byte[] Add(byte[] bytes1, byte[] bytes2, out bool isOverflow)
        {
            if (bytes1.Length != bit || bytes2.Length != bit)
                throw new ArgumentException();

            bytes1 = AdditionCode(bytes1);
            bytes2 = AdditionCode(bytes2);

            var result = new byte[bit];
            byte carry = 0;
            byte higherDigitCarry = 0;

            for (int i = bit - 1; i >= 0; i--)
            {
                var sum = bytes1[i] + bytes2[i] + carry;
                if (sum > 1)
                {
                    carry = 1;
                    result[i] = (byte) (sum % 2);
                }
                else
                {
                    carry = 0;
                    result[i] = (byte) sum;
                }

                if (i == 1)
                    higherDigitCarry = carry;
            }

            isOverflow = CheckOverflow(carry, higherDigitCarry);
            return result;
        }

        public byte[] Add(byte[] bytes1, byte[] bytes2, bool throwIfOverflow = true)
        {
            if (bytes1.Length != bit || bytes2.Length != bit)
                throw new ArgumentException();

            bytes1 = AdditionCode(bytes1);
            bytes2 = AdditionCode(bytes2);

            var result = AddElementsOfTwoArrays(bytes1, bytes2, out var carry, out var higherDigitCarry);

            if (throwIfOverflow && CheckOverflow(carry, higherDigitCarry))
                throw new Exception("Overflow");

            return result;
        }

        private byte[] AddElementsOfTwoArrays(byte[] bytes1, byte[] bytes2, out byte carry, out byte higherDigitCarry)
        {
            if (bytes1.Length != bytes2.Length)
                throw new ArgumentException();
            var length = bytes1.Length;

            var result = new byte[length];
            carry = 0;
            higherDigitCarry = 0;

            for (int i = length - 1; i >= 0; i--)
            {
                var sum = bytes1[i] + bytes2[i] + carry;
                if (sum > 1)
                {
                    carry = 1;
                    result[i] = (byte) (sum % 2);
                }
                else
                {
                    carry = 0;
                    result[i] = (byte) sum;
                }

                if (i == 1)
                    higherDigitCarry = carry;
            }

            return result;
        }

        public int AddResultAsInt(byte[] a, byte[] b)
        {
            var result = Add(a, b);
            return ToNumber(result);
        }

        public int SubResultAsInt(byte[] a, byte[] b)
        {
            var result = SubInAdditionalCode(a, b);
            return ToNumber(result);
        }

        public bool IsNull(Byte[] arr)
        {
            if (arr[0] != 0) return false;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] == 1)
                {
                    return false;
                }
            }

            return true;
        }

        // result in addition code
        public int ToNumber(byte[] result)
        {
            var isNegative = result[0] == 1;
            if (isNegative)
            {
                result = AdditionCode(result);
            }

            var sum = 0;
            for (int i = bit - 1; i >= 1; i--)
            {
                sum += result[i] * (1 << (bit - 1 - i));
            }

            if (isNegative)
            {
                if (sum == 0)
                {
                    return -1 << (bit - 1);
                }

                return -sum;
            }

            return sum;
        }

        private bool CheckOverflow(byte signCarry, byte higherDigitCarry)
        {
            switch (signCarry, higherDigitCarry)
            {
                // bit = 4
                // a = 5 =0101
                // b = 2 = 0010
                // sum = 0111 = 7 
                case (0, 0):
                    return false;
                // bit = 4
                // a = 4 = 0100
                // b = 4 = 0100
                // s = 10000 = 8 overflow
                case (0, 1):
                    return true;
                // bit = 4
                // a = -4 1010
                // b = -4 1010
                // s = 10100 = -8 overflow!
                case (1, 0):
                    return true;
                // bit = 4
                // a = 4  = 0100 
                // b = -4 = 1100
                // s = 10000 (cut higher digit) = 0000 = 0
                case (1, 1):
                    return false;
            }

            throw new Exception();
        }

        // 1 
        public byte[] Revert(byte[] bytes)
        {
            var result = new byte[bit];
            for (var i = 0; i < bytes.Length; i++)
            {
                result[i] = bytes[i] == 0 ? (byte) 1 : (byte) 0;
            }

            return result;
        }
    }
}