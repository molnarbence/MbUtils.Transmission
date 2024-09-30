using MbUtils.Transmission;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure
public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddTransmissionClient(this IServiceCollection services, TransmissionClientConfiguration configuration)
   {
      services
         .AddHttpClient<ITransmissionClient, TransmissionClient>(client => TransmissionClientFactory.ConfigureHttpClient(client, configuration));

      return services;
   }

   public static IServiceCollection AddTransmissionClient(this IServiceCollection services, IConfiguration configuration)
   {
      var config = new TransmissionClientConfiguration();
      configuration.Bind(config);

      return services.AddTransmissionClient(config);
   }
}
