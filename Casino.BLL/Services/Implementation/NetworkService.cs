using System.Net;
using System.Net.Sockets;
using Casino.BLL.Services.Interfaces;

namespace Casino.BLL.Services.Implementation;

public class NetworkService : INetworkService
{
    public string GetUserLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public async Task<string> GetUserPublicIpAddressAsync()
    {
        var url = "https://api.ipify.org";
        var request = new HttpClient();
        var response = (await request.GetStringAsync(new Uri(url))).Trim();
        return response;
    }
}