
using System;
using System.Collections.Generic;
using System.Text;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    internal class ClientNodeLocator
    {
        private List<uint> keys;
        private Dictionary<uint, ClientNode> servers;
        private bool isInitialized = false;
        private readonly int ServerAddressMutations = 1000;

        internal void Initialize(List<ClientNode> nodes)
        {
            keys = new List<uint>();
            servers = new Dictionary<uint, ClientNode>();

            foreach (ClientNode node in nodes)
            {
                List<uint> tmpKeys = GenerateKeys(node, ServerAddressMutations * (byte)node.Weight);

                tmpKeys.ForEach(key =>
                {
                    this.servers[key] = node;
                    this.keys.Add(key);
                });
            }

            this.keys.Sort();

            this.isInitialized = true;
        }

        internal ClientNode LocateNode(string key)
        {
            if (!this.isInitialized)
                throw new InvalidOperationException("ClientNodeLocator还没有初始化");

            if (key == null)
                throw new ArgumentNullException("key");

            uint itemKeyHash = BitConverter.ToUInt32(new FNV1a().ComputeHash(Encoding.Unicode.GetBytes(key)), 0);

            int foundIndex = this.keys.BinarySearch(itemKeyHash);

            if (foundIndex < 0)
            {
                foundIndex = ~foundIndex;

                if (foundIndex == 0)
                {
                    foundIndex = this.keys.Count - 1;
                }
                else if (foundIndex >= this.keys.Count)
                {
                    foundIndex = 0;
                }
            }

            if (foundIndex < 0 || foundIndex > this.keys.Count)
                return null;


            return this.servers[this.keys[foundIndex]];

        }

        private List<uint> GenerateKeys(ClientNode node, int numberOfKeys)
        {
            List<uint> k = new List<uint>(numberOfKeys);

            for (int i = 0; i < numberOfKeys; i++)
            {
                string s = String.Concat(node.Name, "-", i);

                byte[] data = new FNV1a().ComputeHash(Encoding.ASCII.GetBytes(s));

                k.Add(BitConverter.ToUInt32(data, 0));
            }

            return k;
        }
    }
}
