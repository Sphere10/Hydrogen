using System;

namespace Sphere10.Framework
{
    internal class CryptoException : ApplicationException
    {
        internal CryptoException(string error) : base(error)
        {
        }
    }
}