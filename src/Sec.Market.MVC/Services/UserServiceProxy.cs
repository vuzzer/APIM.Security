using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class UserServiceProxy : IUserService
    {
        private readonly HttpClient _httpClient;

        private const string _userApiUrl = "api/users/";
        private readonly Subscription _subscription;
        private readonly ITokenAcquisition _tokenAcquisition;

        public UserServiceProxy(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IOptions<Subscription> options)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _subscription = options.Value;
        }

        public async Task Ajouter(User user)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsync(_userApiUrl + $"&subscription-key={_subscription.Key}", content);

            response.EnsureSuccessStatusCode();
        }

        public Task<User> Obtenir(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> Obtenir(string email, string pwd)
        {
            var queryParameters = new Dictionary<string, string>
                  {
                    { "email", email },
                    { "pwd", pwd}
                };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);

            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync(_userApiUrl + "GetUser?" + queryString + $"&subscription-key={_subscription.Key}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<User>();
              else
                  return null;
        }

        public async Task<List<User>> ObtenirTout()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>(_userApiUrl);
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string>());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
