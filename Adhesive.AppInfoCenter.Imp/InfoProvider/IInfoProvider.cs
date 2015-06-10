
namespace Adhesive.AppInfoCenter.Imp
{
    public interface IInfoProvider
    {
        void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info);
    }
}
