using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class OrderServiceProxy : IOrderService
    {
        private readonly HttpClient _httpClient;

        private const string _orderApiUrl = "api/orders/";
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly Subscription _subscription;

        public OrderServiceProxy(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IOptions<Subscription> options)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _subscription = options.Value;
        }
        public async Task Ajouter(OrderData orderData)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");
            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsync(_orderApiUrl + $"&subscription-key={_subscription.Key}", content);

            response.EnsureSuccessStatusCode();
        }

        public Task<Order> Obtenir(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Order>> ObtenirSelonUser(int userId)
        {
            await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<List<Order>>(_orderApiUrl + "?userId=" + userId + $"&subscription-key={_subscription.Key}");
        }

        public Task Supprimer(int id)
        {
            throw new NotImplementedException();
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string>());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
