
namespace Adhesive.Persistence
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string contextName);
    }
}
