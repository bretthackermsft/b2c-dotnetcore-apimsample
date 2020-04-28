# b2c-dotnetcore-apimsample

Sample in .Net Core 3.0, configured to authenticate to Azure AD B2C and sending an access token
to an API protected by Azure API Management. Documents to review:

Setting up APIM (I used the "AnonymousAPIForAPIM" project to init this in an Azure Web App):
https://docs.microsoft.com/en-us/azure/api-management/add-api-manually

The APIM docs for setting up B2C:
https://docs.microsoft.com/en-us/azure/api-management/howto-protect-backend-frontend-azure-ad-b2c

The B2C docs for setting up APIM:
https://docs.microsoft.com/en-us/azure/active-directory-b2c/secure-api-management?tabs=applications

(those docs both speak to the same thing from different perspectives - including both for all perspectives.)


Derived from this repo:
https://github.com/Azure-Samples/active-directory-b2c-dotnetcore-webapp