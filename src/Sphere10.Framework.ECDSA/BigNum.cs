using Org.BouncyCastle.Math;

namespace Sphere10.Framework
{
    internal class BigNum
    {
        private BigInteger _bigInteger;

        internal BigNum() : this(0)
        {
        }

        internal BigNum(long initialValue)
        {
            _bigInteger = BigInteger.Zero;
            Value = initialValue;
        }

        internal BigNum(string hexaValue) : this(0) => HexaValue = hexaValue;

        internal string HexaValue
        {
            get => _bigInteger.ToByteArrayUnsigned().ToHexaString();
            init
            {
                if (!ECCrypto.IsHexString(value)) throw new CryptoException($"Invalid Hexadecimal value: {value}");
                _bigInteger = new BigInteger(value, 16);
            }
        }

        internal byte[] RawValue
        {
            get => (_bigInteger.SignValue < 0 ? _bigInteger.Negate() : _bigInteger).ToByteArrayUnsigned();
            set => _bigInteger = new BigInteger(1, value);
        }

        internal string DecimalValue
        {
            get => _bigInteger.ToString();
            set => _bigInteger = new BigInteger(value);
        }

        internal long Value
        {
            get => _bigInteger.LongValue;
            init => _bigInteger = new BigInteger(value.ToString());
        }

        internal BigNum Copy() => new()
        {
            _bigInteger = _bigInteger.Clone()
        };

        internal BigNum Add(BigNum value)
        {
            _bigInteger = _bigInteger.Add(value._bigInteger);
            return this;
        }

        internal BigNum Add(long value) => Add(new BigNum(value));

        internal BigNum Sub(BigNum value)
        {
            _bigInteger = _bigInteger.Subtract(value._bigInteger);
            return this;
        }

        internal BigNum Sub(long value) => Sub(new BigNum(value));

        internal BigNum Multiply(BigNum value)
        {
            _bigInteger = _bigInteger.Multiply(value._bigInteger);
            return this;
        }

        internal BigNum Multiply(long value) => Multiply(new BigNum(value));

        internal BigNum Divide(BigNum value)
        {
            _bigInteger = _bigInteger.Divide(value._bigInteger);
            return this;
        }

        internal BigNum Divide(long value) => Divide(new BigNum(value));

        internal void Divide(BigNum dividend, BigNum remainder)
        {
            var tmp = _bigInteger.DivideAndRemainder(dividend._bigInteger);
            _bigInteger = tmp[0];
            remainder._bigInteger = tmp[1];
        }

        internal BigNum LShift(int bits)
        {
            _bigInteger = _bigInteger.ShiftLeft(bits);
            return this;
        }

        internal BigNum RShift(int bits)
        {
            _bigInteger = _bigInteger.ShiftRight(bits);
            return this;
        }

        internal int CompareTo(BigNum value) => _bigInteger.CompareTo(value._bigInteger);

        internal int CompareTo(long value) => CompareTo(new BigNum(value));

        internal bool IsZero() => DecimalValue == "0";

        internal static string HexaToDecimal(string hexString) => new BigNum(hexString).DecimalValue;

        internal static BigNum TargetToHashRate(uint encodedTarget)
        {
            //Target is 2 parts: First byte(A) is "0" bits on the left. Bytes 1,2,3(B) are number after first "1" bit
            //Example: Target 23FEBFCE
            //partA: 23-> 35 decimal
            //partB: FEBFCE
            //Target to Hash rate Formula:
            //Result = 2 ^ partA + ((2 ^ (partA - 24)) * partB)

            var result = new BigNum(2);
            var partA = encodedTarget >> 24;
            result._bigInteger = result._bigInteger.Pow((int) partA);
            var partB = (encodedTarget << 8) >> 8;
            if (partA < 24)
            {
                // exponent is negative... 2 ^ (partA - 24)
                partB >>= (int) (24 - partA);
                result._bigInteger = result._bigInteger.Add(new BigInteger(partB.ToString()));
                return result;
            }

            var bi = BigInteger.Two;
            bi = bi.Pow((int) ((long) partA - 24));
            bi = bi.Multiply(new BigInteger(partB.ToString()));
            result._bigInteger = result._bigInteger.Add(bi);
            return result;
        }
    }
}