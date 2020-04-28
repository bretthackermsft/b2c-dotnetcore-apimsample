using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp_OpenIDConnect_DotNet
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";
        public string TenantUrl => $"https://{Tenant}.b2clogin.com";
        public string AzureAdB2CInstance => $"{TenantUrl}/tfp";
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string CallbackPath { get; set; }
        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}.onmicrosoft.com/{DefaultPolicy}/v2.0";
        //public string Authority => $"{TenantUrl}/{Tenant}.onmicrosoft.com/{DefaultPolicy}/v2.0";


        //B2C 'authority' Uri should have at least 3 segments in the path (i.e. https://<host>/tfp/<tenant>/<policy>/...)

        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }

        public static string GetRedirectUrl(HttpRequest request, string callbackPath)
        {
            var cloudHost = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

            string appBaseUrl = string.Format("{0}://{1}{2}", request.Scheme, (cloudHost ?? request.Host.ToString()), request.PathBase);
            return appBaseUrl + callbackPath;
        }
    }
}
