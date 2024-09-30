using System.Net.Http.Headers;
using System.Text;

namespace MbUtils.Transmission;
public class TransmissionClientFactory
{
   public static ITransmissionClient Create(TransmissionClientConfiguration configuration)
   {
      var httpClient = new HttpClient();
      ConfigureHttpClient(httpClient, configuration);
      return new TransmissionClient(httpClient);
   }

   internal static void ConfigureHttpClient(HttpClient client, TransmissionClientConfiguration configuration)
   {
      client.BaseAddress = new Uri(configuration.BaseUrl);
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
          "Basic",
          Convert.ToBase64String(Encoding.ASCII.GetBytes($"{configuration.Username}:{configuration.Password}"))
          );
   }
}
