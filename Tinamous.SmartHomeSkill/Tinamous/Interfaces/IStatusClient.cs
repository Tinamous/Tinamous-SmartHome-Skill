using System.Threading.Tasks;

namespace Tinamous.SmartHome.Tinamous
{
    public interface IStatusClient
    {
        Task PostStatusMessageAsync(string token, string message);
    }
}