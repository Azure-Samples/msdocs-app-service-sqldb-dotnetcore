using System.Diagnostics;
using System.Web;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace DotNetCoreSqlDb.Service
{
    public class ETradeTokenService
    {

        public static User CreateToken(CoreDbContext context, User userLocal)
        {
            string authorizeUrl = "https://us.etrade.com";
            var client = new RestClient(userLocal.EtradeBaseUrl);
            
            // Step 1: fetch the request token
            client.Authenticator = OAuth1Authenticator.ForRequestToken(userLocal.ConsumerKey, userLocal.ConsumerSecret, "oob");
            var request = new RestRequest("oauth/request_token");
            var tokenResponse = client.Execute(request);


            // Step 1.a: parse response 
            var tokenQueryString = HttpUtility.ParseQueryString(tokenResponse.Content);
            var requestToken = tokenQueryString["oauth_token"];
            var requestTokenSecret = tokenQueryString["oauth_token_secret"];

            // Step 2: direct to authorization page
            var authorizeClient = new RestClient(authorizeUrl);
            var authorizeRequest = new RestRequest("e/t/etws/authorize");
            authorizeRequest.AddParameter("key", userLocal.ConsumerKey);
            authorizeRequest.AddParameter("token", requestToken);

            tokenResponse = client.Execute(authorizeRequest);
            Console.WriteLine("Request tokens: " + tokenResponse.Content);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = authorizeClient.BuildUri(authorizeRequest).ToString(),
                UseShellExecute = true
            };
            Process.Start(psi);

            Console.Write("Provide auth code:");
            var verifier = Console.ReadLine();

            // Step 3: fetch access token
            var accessTokenRequest1 = new RestRequest("oauth/access_token");
            client.Authenticator = OAuth1Authenticator.ForAccessToken(userLocal.ConsumerKey, userLocal.ConsumerSecret, requestToken, requestTokenSecret, verifier);

            DateTimeOffset dateTime = DateTime.UtcNow;
            long timestamp = dateTime.ToUnixTimeSeconds();
            var nonce = Guid.NewGuid().ToString();

            var response1 = client.Execute(accessTokenRequest1);

            // Step 3.a: parse response 
            var qs1 = HttpUtility.ParseQueryString(response1.Content);
            var accessToken = qs1["oauth_token"];
            var accessTokenSecret = qs1["oauth_token_secret"];

            userLocal.RequestToken = requestToken;
            userLocal.RequestTokenSecret = requestTokenSecret;
            userLocal.Verifier = verifier; 
            userLocal.AccessToken = accessToken;
            userLocal.AccessTokenSecret = accessTokenSecret;
            userLocal.TokenCreatedDate = DateTime.Now;

            context.User.Update(userLocal);
            context.SaveChanges();

            client = new RestClient(userLocal.EtradeBaseUrl);
            var quoteRequest = new RestRequest(userLocal.EtradeBaseUrl + "/v1/accounts/list");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(userLocal.ConsumerKey, userLocal.ConsumerSecret, userLocal.AccessToken, userLocal.AccessTokenSecret);
            var response = client.Execute(quoteRequest);

            if (response.Content.Contains("token_expired"))
            {
                Console.WriteLine($"{userLocal.UserName}: Token Expired - {DateTimeOffset.Now}");
               
            }
            else
            {
                Console.WriteLine($" {userLocal.UserName}: Token Created and Tested Successfully - {DateTimeOffset.Now}");
            }

            return userLocal;
        }
    }
}
