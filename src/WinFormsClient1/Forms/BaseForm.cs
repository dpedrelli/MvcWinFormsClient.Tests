using FastMember;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

//https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
namespace WinFormsClient1.Forms
{
    public partial class BaseForm : Form
    {
        private OidcClient _oidcClient;
        protected OidcClient OidcClient
        {
            get
            {
                if (_oidcClient == null) { InitializeOidcClient(); }
                return _oidcClient;
            }
        }
        private HttpClientHandler _handler;
        private HttpClient _apiClient;
        protected HttpClient ApiClient
        {
            get
            {
                if (_apiClient == null) { InitializeHttpClient(); }
                return _apiClient;
            }
        }

        private string currentAccessToken { get; set; }
        private string currentIdentityToken { get; set; }
        private string currentRefreshToken { get; set; }

        private const string antiforgeryTokenName = "__RequestVerificationToken";
        private string antiforgeryTokenValue = "";

        private void InitializeOidcClient()
        {
            var options = new OidcClientOptions
            {
                Authority = "http://localhost:5000",
                ClientId = "exe1",
                ClientSecret = "secret",
                Scope = "openid profile offline_access mvcapi1",
                RedirectUri = "http://127.0.0.1:7890",
                FilterClaims = false,
                Browser = new SystemBrowser(port: 7890, path: null, browserPath: null)
            };
            _oidcClient = new OidcClient(options);
        }

        private void InitializeHttpClient()
        {
            _handler = new HttpClientHandler();
            _apiClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost:60176/") };
            
            //_apiClient = new HttpClient { BaseAddress = new Uri("http://localhost:60176/") };

            
            //using (var handler = new HttpClientHandler { UseCookies = false })
            //using (var client = new HttpClient(handler) { BaseAddress = baseUrl })
            //{
            //    var msg = new HttpRequestMessage(HttpMethod.Get, getActionUrl(controller + @"/GetPage"));
            //    setCookies(ref msg);
            //    response = client.SendAsync(msg).Result;
            //    updateAntiforgeryToken(response);
            //    updateCookies(response);
            //}

        }

        protected async Task GetAntiforgeryToken(string controller = "Home", string action = "GetAntiforgeryToken")
        {
            ApiClient.SetBearerToken(currentAccessToken);
            var response = await ApiClient.GetAsync(controller + @"/" + action);
            UpdateAntiforgeryToken(response);
        }

        private void UpdateAntiforgeryToken(HttpResponseMessage message)
        {
            string result = message.Content.ReadAsStringAsync().Result;
            var index1 = result.IndexOf(antiforgeryTokenName);
            if (index1 < 0) { return; }
            var index2 = result.IndexOf(@"value=""", index1) + 7;
            var index3 = result.IndexOf(@"""", index2);
            antiforgeryTokenValue = result.Substring(index2, index3 - index2);

            // HttpClient is not instianted with each call.
            // The first call to GetAntiforgeryToken retrieves and sets the .AspNetCore.Antiforgery. cookie, for the HttpClient object.
        }

        protected FormUrlEncodedContent GenerateContent(string paramValue, string paramName = "parameter")
        {
            //ApiClient.SetBearerToken(currentAccessToken);
            var pairs = new List<KeyValuePair<string, string>>();
            if (antiforgeryTokenValue.Length > 0) pairs.Add(new KeyValuePair<string, string>(antiforgeryTokenName, antiforgeryTokenValue));
            if (paramValue.Length > 0) pairs.Add(new KeyValuePair<string, string>(paramName, paramValue));
            return new FormUrlEncodedContent(pairs);
        }

        protected async Task Login(bool getAntiforgeryToken)
        {
            var result = await OidcClient.LoginAsync();
            if (!result.IsError)
            {
                currentAccessToken = result.AccessToken;
                currentIdentityToken = result.IdentityToken;
                currentRefreshToken = result.RefreshToken;
                ApiClient.SetBearerToken(currentAccessToken);
                if (getAntiforgeryToken)
                {
                    await GetAntiforgeryToken();
                }
            }
        }

        protected void BindData<T>(List<T> data, DataTable dataTable) where T : class
        {
            using (var reader = ObjectReader.Create<T>(data))
            {
                dataTable.Load(reader);
            }
        }

    }
}
