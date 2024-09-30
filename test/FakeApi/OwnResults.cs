using System.Text;

namespace FakeApi;

public static class OwnResults
{
   public static IResult ResponseFromFile(string fileName)
   {
      var fileContent = File.ReadAllText(Path.Combine("responses", fileName));
      return Results.Content(fileContent, "application/json", Encoding.UTF8);
   }
}
