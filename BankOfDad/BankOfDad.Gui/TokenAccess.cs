using SecureStorage = Microsoft.Maui.Storage.SecureStorage;

namespace BankOfDad.Gui;

public static class TokenAccess
{
    private static string _insecureToken = null;

    public static async Task<string> GetToken()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            return _insecureToken;
        }

        return await SecureStorage.GetAsync("token");
    }

    public static async Task SetToken(string token)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            _insecureToken = token;
        }
        else
        {
            await SecureStorage.SetAsync("token", token);
        }
    }

    public static void RemoveToken()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            _insecureToken = null;
        }
        else
        {
            SecureStorage.Remove("token");
        }
    }
}
