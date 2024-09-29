using System.Text;

namespace MbUtils.Transmission;

internal static class StringExtensions
{
   internal static string ToBase64(this string value)
   {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
   }

   internal static bool HasValue(this string? value)
   {
      return !string.IsNullOrWhiteSpace(value);
   }
}
