
using System;
namespace Adhesive.AppInfoCenter
{
    public interface IStateService : IDisposable
    {
        void Init(StateServiceConfiguration configuration);
    }
}
