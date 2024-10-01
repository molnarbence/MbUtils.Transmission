using System.Text;

namespace FakeApi;

public static class OwnResults
{
   public static async Task<IResult> ResponseFromFileAsync(string fileName)
   {
      var fileContent = await File.ReadAllTextAsync(Path.Combine("responses", fileName));
      return Results.Content(fileContent, "application/json", Encoding.UTF8);
   }
}
