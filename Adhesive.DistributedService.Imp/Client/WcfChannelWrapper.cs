

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.ServiceModel;

    internal sealed class WcfChannelWrapper<T> : IDisposable where T : class
    {
        private readonly T channel;

        public WcfChannelWrapper(T channel)
        {
            this.channel = channel;
        }

        public T Channel { get { return channel; } }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                var commObj = channel as ICommunicationObject;
                if (commObj != null)
                {
                    try
                    {
                        commObj.Close();
                    }
                    catch (CommunicationException)
                    {
                        commObj.Abort();
                    }
                    catch (TimeoutException)
                    {
                        commObj.Abort();
                    }
                    catch (Exception)
                    {
                        commObj.Abort();
                        throw;
                    }
                }

            }

            disposed = true;
        }

        ~WcfChannelWrapper()
        {
            Dispose(false);
        }

        #endregion

    }
}
