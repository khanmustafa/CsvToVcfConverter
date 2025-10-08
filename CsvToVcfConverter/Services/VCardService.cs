using System.Text;
using CsvToVcfConverter.Models;

namespace CsvToVcfConverter.Services
{
    public class VCardService
    {
        public string ConvertContactsToVCard(List<Contact> contacts)
        {
            var vcardBuilder = new StringBuilder();

            foreach (var contact in contacts)
            {
                vcardBuilder.AppendLine("BEGIN:VCARD");
                vcardBuilder.AppendLine("VERSION:3.0");
                vcardBuilder.AppendLine($"FN:{contact.Name}");
                vcardBuilder.AppendLine($"N:{contact.Name};;;;");
                
                if (!string.IsNullOrWhiteSpace(contact.PhoneNumber))
                {
                    // Clean phone number (remove spaces, dashes, etc.)
                    var cleanPhone = contact.PhoneNumber.Trim();
                    vcardBuilder.AppendLine($"TEL;TYPE=CELL:{cleanPhone}");
                }
                
                vcardBuilder.AppendLine("END:VCARD");
                vcardBuilder.AppendLine();
            }

            return vcardBuilder.ToString();
        }
    }
}