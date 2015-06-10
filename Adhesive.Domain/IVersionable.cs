
namespace Adhesive.Domain
{
    public interface IVersionable
    {
        long RowVersion { get; set; }
    }
}
