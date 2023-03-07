namespace InnoGotchi.Web.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static void AddHttpClient(this IServiceCollection services, string baseRoot, string name)
        {
            services.AddHttpClient(name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(baseRoot + "/" + name);
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
