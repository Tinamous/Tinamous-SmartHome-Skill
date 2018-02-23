using System.Threading.Tasks;

namespace Tinamous.SmartHome.Tinamous
{
    public interface ITinamousRestClient
    {
        Task Post<T>(string token, string apiUri, T t);
        Task<T> GetAsJsonAsync<T>(string token, string apiUri);
    }
}