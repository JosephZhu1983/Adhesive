

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Text;

    internal static class ExtensionMethods
    {
        internal static string GetMd5Hash(this string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        internal static bool VerifyMd5Hash(this string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        internal static ServiceDescription GetCurrentServiceDescription(this OperationContext context)
        {
            if (context == null) return null;
            return context.Host.Description;
        }

        internal static OperationDescription GetCurrentOperationDescription(this OperationContext context)
        {
            string action = context.IncomingMessageHeaders.Action;
            EndpointAddress epa = context.Channel.LocalAddress;
            ServiceDescription hostDesc = context.Host.Description;
            ServiceEndpoint ep = hostDesc.Endpoints.Find(epa.Uri);
            DispatchOperation dispatchOperation = context.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o => o.Action == action);
            OperationDescription od = null;
            if (ep != null)
            {
                od = ep.Contract.Operations.Find(dispatchOperation.Name);
            }
            return od;
        }

        internal static T GetApplicationContext<T>(this Message message) where T : ApplicationContext
        {
            if (message.Headers.FindHeader(ApplicationContext.ContextHeaderLocalName, ApplicationContext.ContextHeaderNamespace) >= 0)
            {
                T context = message.Headers.GetHeader<T>(ApplicationContext.ContextHeaderLocalName, ApplicationContext.ContextHeaderNamespace);
                if (context != null)
                {
                    return context;
                }
            }

            return null;
        }

        internal static void SetApplicationContext<T>(this Message message, T context) where T : ApplicationContext
        {
            MessageHeader<T> contextHeader = new MessageHeader<T>(context);
            message.Headers.Add(contextHeader.GetUntypedHeader(ApplicationContext.ContextHeaderLocalName, ApplicationContext.ContextHeaderNamespace));
        }
    }
}
