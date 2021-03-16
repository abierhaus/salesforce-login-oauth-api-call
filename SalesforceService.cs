﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace salesforce_login_oauth_api_call
{
    public interface ISalesforceService
    {
        Task<string> TestCallAsync();
    }


    public class SalesforceAuthentificationResponse
    {
        public string access_token { get; set; }
        public string instance_url { get; set; }
  
    }

    public class SalesforceService : ISalesforceService
    {
        public SalesforceService(IConfiguration configuration)
        {
            Username = configuration.GetSection("Salesforce")["Username"];
            Password = configuration.GetSection("Salesforce")["Password"];
            Token = configuration.GetSection("Salesforce")["Token"];
            ClientSecret = configuration.GetSection("Salesforce")["ClientSecret"];
            ClientId = configuration.GetSection("Salesforce")["ClientId"];
            LoginEndpoint = configuration.GetSection("Salesforce")["LoginEndpoint"];
            ApiEndpoint = configuration.GetSection("Salesforce")["ApiEndpoint"];
        }

        private string Username { get; }
        private string Password { get; }

        private string Token { get; }

        private string ClientId { get; }
        private string ClientSecret { get; }


        private string LoginEndpoint { get; }
        private string ApiEndpoint { get; }


        /// <summary>
        /// Make a Test GET Call. Returns the available Salesforce Releases/ Version numbers
        /// </summary>
        /// <returns></returns>
        public async Task<string> TestCallAsync()
        {
            //Login
            var salesforceAuthentificationResponse = await Login();

            using var client = new HttpClient();
            
            //Arrage request object
            var request = new HttpRequestMessage(HttpMethod.Get, $"{salesforceAuthentificationResponse.instance_url}{ApiEndpoint}");
            request.Headers.Add("Authorization", $"Bearer {salesforceAuthentificationResponse.access_token}");

            //Send request with Auth Token 
            var response = await client.SendAsync(request);

            //Get results back
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Login to Salesforce and return the login object that contains the access token and instance url
        /// </summary>
        /// <returns></returns>
        public async Task<SalesforceAuthentificationResponse> Login()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", $"{Password}{Token}")
            });
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(LoginEndpoint),
                Content = content
            };
            var responseMessage = await httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();
            var salesforceAuthentificationResponse = System.Text.Json.JsonSerializer.Deserialize<SalesforceAuthentificationResponse>(response);

            return salesforceAuthentificationResponse;
        }
    }
}