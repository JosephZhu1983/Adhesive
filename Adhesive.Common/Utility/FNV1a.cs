
using System;
using System.Security.Cryptography;

namespace Adhesive.Common
{
    public class FNV1a : HashAlgorithm
    {
        private const uint Prime = 16777619;
        private const uint Offset = 2166136261;

        protected uint CurrentHashValue;

        public FNV1a()
        {
            this.HashSizeValue = 32;
            this.Initialize();
        }

        public override void Initialize()
        {
            this.CurrentHashValue = Offset;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int end = ibStart + cbSize;

            for (int i = ibStart; i < end; i++)
            {
                this.CurrentHashValue = (this.CurrentHashValue ^ array[i]) * FNV1a.Prime;
            }
        }

        protected override byte[] HashFinal()
        {
            this.CurrentHashValue += this.CurrentHashValue << 13;
            this.CurrentHashValue ^= this.CurrentHashValue >> 7;
            this.CurrentHashValue += this.CurrentHashValue << 3;
            this.CurrentHashValue ^= this.CurrentHashValue >> 17;
            this.CurrentHashValue += this.CurrentHashValue << 5;
            return BitConverter.GetBytes(this.CurrentHashValue);
        }
    }
}
