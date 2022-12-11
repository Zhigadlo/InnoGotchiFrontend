namespace InnoGotchi.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddHttpClientForPet(this IServiceCollection services, string baseRoot)
        {
            services.AddHttpClient("Pets", httpClient =>
            {
                httpClient.BaseAddress = new Uri(baseRoot + "/Pets");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            });
        }

        public static void AddHttpClientForUser(this IServiceCollection services, string baseRoot)
        {
            services.AddHttpClient("Users", httpClient =>
            {
                httpClient.BaseAddress = new Uri(baseRoot + "/Users");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            });
        }
    }
}
