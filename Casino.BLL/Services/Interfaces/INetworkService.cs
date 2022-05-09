namespace Casino.BLL.Services.Interfaces;

public interface INetworkService
{
    string GetUserLocalIpAddress();
    Task<string> GetUserPublicIpAddressAsync();
}