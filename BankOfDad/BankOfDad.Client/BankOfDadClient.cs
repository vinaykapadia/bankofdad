using System.Net;
using System.Net.Http.Headers;
using System.Text;
using BankOfDad.Models;
using Newtonsoft.Json;

namespace BankOfDad.Client;

public class BankOfDadClient
{
    private readonly HttpClient _httpClient;

    private const string GetAllAccounts = "/api/bankaccounts";
    private const string GetMyAccount = "/api/bankaccounts/me";
    private const string GetAccountById = "/api/bankaccounts/{0}";
    private const string PostTransaction = "/api/bankaccounts/transaction";
    private const string Authenticate = "/api/auth";

    public BankOfDadClient(string baseUrl)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<AuthResponse> PostAuth(string username, string password)
    {
        var request = new AuthRequest
        {
            Username = username,
            Password = password
        };

        return await Post<AuthResponse>(Authenticate, request);
    }

    public void SetToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    public async Task<IEnumerable<BankAccountResponse>> GetAccounts()
    {
        return await Get<IEnumerable<BankAccountResponse>>(GetAllAccounts);
    }

    public async Task<BankAccountResponse> GetAccount()
    {
        return await Get<BankAccountResponse>(GetMyAccount);
    }

    public async Task<BankAccountResponse> GetAccount(int id)
    {
        var url = string.Format(GetAccountById, id);
        return await Get<BankAccountResponse>(url);
    }

    public async Task<TransactionResponse> AddTransaction(int id, int toId, decimal amount, string description)
    {
        var request = new TransactionRequest
        {
            From = id,
            To = toId,
            Amount = amount,
            Description = description
        };

        return await Post<TransactionResponse>(PostTransaction, request);
    }

    private async Task<T> Post<T>(string url, object data)
    {
        try
        {
            var payload = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, payload);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default(T);
        }
    }

    private async Task<T> Get<T>(string url)
    {
        try
        {
            var json = await _httpClient.GetStringAsync(url);
            var obj = JsonConvert.DeserializeObject<T>(json)
                      ?? throw new WebException("Error making call.");
            return obj;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return default(T);
    }
}
