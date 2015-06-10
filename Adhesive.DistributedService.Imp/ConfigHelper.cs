

namespace Adhesive.DistributedService.Imp
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.Xml;

    internal class ConfigHelper
    {
        private static readonly MethodInfo methodDeserializeElement;
        private static readonly MethodInfo methodCreateBehavior;

        static ConfigHelper()
        {
            methodDeserializeElement = typeof(ConfigurationElement).GetMethod("DeserializeElement", BindingFlags.NonPublic | BindingFlags.Instance);
            methodCreateBehavior = typeof(BehaviorExtensionElement).GetMethod("CreateBehavior", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static Binding CreateBinding(string type, string xml)
        {
            Binding binding = null;
            StandardBindingElement element = null;

            switch (type)
            {
                case "NetTcpBinding":
                    binding = new NetTcpBinding();
                    element = new NetTcpBindingElement();
                    break;
                case "BasicHttpBinding":
                    binding = new BasicHttpBinding();
                    element = new BasicHttpBindingElement();
                    break;
                case "WSHttpBinding":
                    binding = new WSHttpBinding();
                    element = new WSHttpBindingElement();
                    break;
                case "NetNamedPipeBinding":
                    binding = new NetNamedPipeBinding();
                    element = new NetNamedPipeBindingElement();
                    break;
                case "NetMsmqBinding":
                    binding = new NetMsmqBinding();
                    element = new NetMsmqBindingElement();
                    break;
                default:
                    binding = new NetTcpBinding();
                    element = new NetTcpBindingElement();
                    break;
            }

            Deserialize(xml, element);
            element.ApplyConfiguration(binding);

            return binding;
        }

        internal static void Deserialize(string xml, ConfigurationElement element)
        {
            var rdr = new XmlTextReader(new StringReader(xml));
            rdr.Read();
            rdr.ReadSubtree();
            methodDeserializeElement.Invoke(element, new object[] { rdr, false });
        }

        internal static void SetBehavior<TBehavior>(ICollection<TBehavior> collection, BehaviorExtensionElement element)
        {
            var behavior = (TBehavior)methodCreateBehavior.Invoke(element, null);
            foreach (var item in collection)
            {
                if (item.GetType() == behavior.GetType())
                {
                    collection.Remove(item);
                    break;
                }
            }
            collection.Add(behavior);
        }

        internal static string CreateAddress(string protocol, string ip, int port, string name)
        {
            if (protocol == "net.msmq")
            {
                return string.Concat(protocol, "://", ip, "/private/" + name);
            }
            else
            {
                return string.Concat(protocol, "://", ip, ":", port, "/", name);
            }
        }
    }
}
