using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvToVcfConverter.Models;

namespace CsvToVcfConverter.Services
{
    public class CsvService
    {
        public List<string> GetColumnNames(Stream csvStream)
        {
            var columns = new List<string>();
            
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                if (csv.HeaderRecord != null)
                {
                    columns.AddRange(csv.HeaderRecord);
                }
            }
            
            return columns;
        }
        
        public List<Contact> ParseCsvFile(Stream csvStream, string nameColumn, string phoneColumn)
        {
            var contacts = new List<Contact>();
            
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                
                var nameIndex = csv.GetFieldIndex(nameColumn);
                var phoneIndex = csv.GetFieldIndex(phoneColumn);
                
                if (nameIndex == -1 || phoneIndex == -1)
                {
                    return contacts;
                }
                
                while (csv.Read())
                {
                    var name = csv.GetField(nameIndex)?.Trim();
                    var phone = csv.GetField(phoneIndex)?.Trim();
                    
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(phone))
                    {
                        contacts.Add(new Contact
                        {
                            Name = name,
                            PhoneNumber = phone
                        });
                    }
                }
            }
            
            return contacts;
        }

        public List<Contact> ParseCsvFile(Stream csvStream)
        {
            var contacts = new List<Contact>();
            
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ContactMap>();
                contacts = csv.GetRecords<Contact>().ToList();
            }
            
            return contacts;
        }
    }

    public class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
        {
            // Map CSV columns to Contact properties
            // This handles various possible column names including the user's specific columns
            Map(m => m.Name).Name("Formatted Name", "Name", "Full Name", "Contact Name", "formatted name", "name", "full name", "contact name");
            Map(m => m.PhoneNumber).Name("WhatsApp No.", "WhatsApp No", "Phone", "Phone Number", "Contact Number", "Mobile", "Cell", "whatsapp no.", "whatsapp no", "phone", "phone number", "contact number", "mobile", "cell");
        }
    }
}