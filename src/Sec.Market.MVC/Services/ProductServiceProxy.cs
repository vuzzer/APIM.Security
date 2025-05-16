using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class ProductServiceProxy : IProductService
    {
        private readonly HttpClient _httpClient;

        private const string _produitApiUrl = "api/products/";

        private readonly ITokenAcquisition _tokenAcquisition;

        private readonly Subscription _subscription;

        public ProductServiceProxy(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IOptions<Subscription> options)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _subscription = options.Value;
        }

        public async Task Ajouter(Product product)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsync(_produitApiUrl + $"&subscription-key={_subscription.Key}", content);

            response.EnsureSuccessStatusCode();
        }

        public Task Modifier(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> Obtenir(int id)
        {
            await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<Product>(_produitApiUrl + id + $"&subscription-key={_subscription.Key}");
        }

        public async Task<List<Product>> ObtenirSelonFiltre(string? filtre)
        {
            await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<List<Product>>(_produitApiUrl + "?filter=" + filtre + $"&subscription-key={_subscription.Key}");
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
