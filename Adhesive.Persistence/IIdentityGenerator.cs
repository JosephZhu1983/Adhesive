
namespace Adhesive.Persistence
{
    /// <summary>
    /// 唯一标识生成器接口
    /// </summary>
    public interface IIdentityGenerator
    {
        object Empty { get; }
        object NewId();
    }
}
