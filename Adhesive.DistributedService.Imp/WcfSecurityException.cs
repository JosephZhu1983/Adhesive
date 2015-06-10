

namespace Adhesive.DistributedService.Imp
{
    using System;

    internal class WcfSecurityException : Exception
    {
        internal WcfSecurityException(string message) : base(message) { }
    }
}
