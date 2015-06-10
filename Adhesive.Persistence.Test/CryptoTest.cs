using System;
using Adhesive.Persistence.Imp;
using NUnit.Framework;

namespace Adhesive.Persistence.Test
{
    [TestFixture]
    public class CryptoTest
    {
        [Test]
        public void TestEncrypt()
        {
            string encryptedPassword = CryptoUtil.Encrypt("aic!@#aic#@!");
            Console.WriteLine(encryptedPassword);
        }
        [Test]
        public void TestDescrypt()
        {
            string descryptedPassword = CryptoUtil.Decrypt("9djGOh3dP5AxjRixWicFUw==");
            Console.WriteLine(descryptedPassword);
        }
    }
}
