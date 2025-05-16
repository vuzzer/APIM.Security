using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class CustomerReviewServiceProxy : ICustomerReviewService
    {
        private readonly HttpClient _httpClient;

        private const string _customerReviewApiUrl = "api/customerreviews/";

        private readonly Subscription _subscription;
        private readonly ITokenAcquisition _tokenAcquisition;

        public CustomerReviewServiceProxy(HttpClient httpClient, IOptions<Subscription> options, ITokenAcquisition tokenAcquisition)
        {
            _httpClient = httpClient;
            _subscription = options.Value;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task Ajouter(CustomerReview customerReview)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(customerReview), Encoding.UTF8, "application/json");

            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsync(_customerReviewApiUrl + $"&subscription-key={_subscription.Key}", content);

            response.EnsureSuccessStatusCode();
        }

        public Task<CustomerReview> Obtenir(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CustomerReview>> ObtenirSelonProduit(int productId)
        {
           await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<List<CustomerReview>>(_customerReviewApiUrl + "?productId=" + productId + $"&subscription-key={_subscription.Key}");
          
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string>());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
