
using System;

namespace Adhesive.Config
{
    public class ConfigItemValueUpdateArguments : EventArgs
    {
        public string Id { get; set; }
        public ConfigItemValueUpdateArguments(string id)
        {
            Id = id;
        }
    }
    public delegate void ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments arguments);

}
