using Azure.Identity;
using Microsoft.Graph;
using Ordo.Models;

namespace Ordo.Api
{
    public static class GraphClientHelper
    {
        internal static GraphServiceClient GetAuthenticatedGraphClient(AppSettings appSettings)
        {
            // Create a client credential using Azure.Identity
            var clientSecretCredential = new ClientSecretCredential(
                appSettings.TenantId,
                appSettings.ClientId,
                appSettings.ClientSecret
            );

            // Create and return a GraphServiceClient
            var graphClient = new GraphServiceClient(clientSecretCredential);
            return graphClient;
        }
    }
}
