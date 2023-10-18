using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoUrlToChatBot.MigratedFromSite.Services
{
    public interface ICheckAsciiCompatible
    {
        string RemoveNonAsciiCharacters(string text);
    }

    public class CheckAsciiCompatible : ICheckAsciiCompatible
    {
        public string RemoveNonAsciiCharacters(string text)
        {
            StringBuilder asciiString = new StringBuilder();

            foreach (char c in text)
            {
                if ((int)c < 128) // Check if the character is within the ASCII range
                {
                    asciiString.Append(c); // Append it to the result string
                }
            }

            return asciiString.ToString();
        }
    }
}
