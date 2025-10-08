using OfficeOpenXml;
using CsvToVcfConverter.Models;

namespace CsvToVcfConverter.Services
{
    public class ExcelService
    {
        public List<string> GetColumnNames(Stream excelStream)
        {
            var columns = new List<string>();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                
                if (worksheet?.Dimension != null)
                {
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(headerValue))
                        {
                            columns.Add(headerValue);
                        }
                    }
                }
            }
            
            return columns;
        }
        
        public List<Contact> ParseExcelFile(Stream excelStream, string nameColumn, string phoneColumn)
        {
            var contacts = new List<Contact>();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                
                if (worksheet?.Dimension == null)
                {
                    return contacts;
                }
                
                var nameColumnIndex = -1;
                var phoneColumnIndex = -1;
                
                // Find the specified column indices
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                    
                    if (string.Equals(headerValue, nameColumn, StringComparison.OrdinalIgnoreCase))
                    {
                        nameColumnIndex = col;
                    }
                    else if (string.Equals(headerValue, phoneColumn, StringComparison.OrdinalIgnoreCase))
                    {
                        phoneColumnIndex = col;
                    }
                }
                
                // Extract data if both columns are found
                if (nameColumnIndex > 0 && phoneColumnIndex > 0)
                {
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var name = worksheet.Cells[row, nameColumnIndex].Value?.ToString()?.Trim();
                        var phone = worksheet.Cells[row, phoneColumnIndex].Value?.ToString()?.Trim();
                        
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
            }
            
            return contacts;
        }

        public List<Contact> ParseExcelFile(Stream excelStream)
        {
            var contacts = new List<Contact>();
            
            // Set the license context for EPPlus (NonCommercial for free use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Get the first worksheet
                
                if (worksheet == null || worksheet.Dimension == null)
                {
                    return contacts;
                }
                
                // Find the header row and column indices
                var nameColumnIndex = -1;
                var phoneColumnIndex = -1;
                
                // Search for headers in the first row
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                    
                    if (string.IsNullOrEmpty(headerValue))
                        continue;
                        
                    // Check for name column variations
                    if (IsNameColumn(headerValue))
                    {
                        nameColumnIndex = col;
                    }
                    // Check for phone column variations
                    else if (IsPhoneColumn(headerValue))
                    {
                        phoneColumnIndex = col;
                    }
                }
                
                // If we found both columns, extract the data
                if (nameColumnIndex > 0 && phoneColumnIndex > 0)
                {
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var name = worksheet.Cells[row, nameColumnIndex].Value?.ToString()?.Trim();
                        var phone = worksheet.Cells[row, phoneColumnIndex].Value?.ToString()?.Trim();
                        
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
            }
            
            return contacts;
        }
        
        private bool IsNameColumn(string headerValue)
        {
            var nameHeaders = new[] { "formatted name", "name", "full name", "contact name" };
            return nameHeaders.Any(h => h.Equals(headerValue, StringComparison.OrdinalIgnoreCase));
        }
        
        private bool IsPhoneColumn(string headerValue)
        {
            var phoneHeaders = new[] { "whatsapp no.", "whatsapp no", "phone", "phone number", "contact number", "mobile", "cell" };
            return phoneHeaders.Any(h => h.Equals(headerValue, StringComparison.OrdinalIgnoreCase));
        }
    }
}