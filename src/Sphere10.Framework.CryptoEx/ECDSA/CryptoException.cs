using System;

namespace Sphere10.Framework.CryptoEx
{
    internal class CryptoException : ApplicationException
    {
        internal CryptoException(string error) : base(error)
        {
        }
    }
}