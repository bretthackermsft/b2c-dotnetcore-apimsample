﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using b2cdotnetcorecallapi.Models;
using WebApp_OpenIDConnect_DotNet;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Identity.Client;
using WebApp_OpenIDConnect_DotNet.Models;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace b2cdotnetcorecallapi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        AzureAdB2COptions AzureAdB2COptions;
        public HomeController(IOptions<AzureAdB2COptions> azureAdB2COptions, ILogger<HomeController> logger)
        {
            _logger = logger;
            AzureAdB2COptions = azureAdB2COptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = string.Format("Claims available for the user {0}", (User.FindFirst("name")?.Value));
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Api()
        {
            string responseString = "";
            try
            {
                // Retrieve the token with the specified scopes
                var scope = AzureAdB2COptions.ApiScopes.Split(' ');
                string signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                IConfidentialClientApplication cca =
                ConfidentialClientApplicationBuilder.Create(AzureAdB2COptions.ClientId)
                    .WithRedirectUri(AzureAdB2COptions.GetRedirectUrl(Request, AzureAdB2COptions.CallbackPath))
                    .WithClientSecret(AzureAdB2COptions.ClientSecret)
                    .WithB2CAuthority(AzureAdB2COptions.Authority)
                    .Build();
                new MSALStaticCache(signedInUserID, this.HttpContext).EnablePersistence(cca.UserTokenCache);

                var accounts = await cca.GetAccountsAsync();
                AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault())
                    .ExecuteAsync();

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, AzureAdB2COptions.ApiUrl);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        responseString = await response.Content.ReadAsStringAsync();
                        break;
                    case HttpStatusCode.Unauthorized:
                        responseString = $"Please sign in again. {response.ReasonPhrase}";
                        break;
                    default:
                        responseString = $"Error calling API. StatusCode=${response.StatusCode}";
                        break;
                }
            }
            catch (MsalUiRequiredException ex)
            {
                responseString = $"Session has expired. Please sign in again. {ex.Message}";
            }
            catch (Exception ex)
            {
                responseString = $"Error calling API: {ex.Message}";
            }

            ViewData["Payload"] = $"{responseString}";
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
