using OfficeOpenXml;
using System.IO;

namespace CreateSampleExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Contacts");
                
                // Add headers
                worksheet.Cells[1, 1].Value = "Timestamp";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Age";
                worksheet.Cells[1, 4].Value = "WhatsApp No.";
                worksheet.Cells[1, 5].Value = "Gender";
                worksheet.Cells[1, 6].Value = "Location";
                worksheet.Cells[1, 7].Value = "Formatted Name";
                
                // Add sample data
                worksheet.Cells[2, 1].Value = "2024-01-01 10:30:00";
                worksheet.Cells[2, 2].Value = "John";
                worksheet.Cells[2, 3].Value = 30;
                worksheet.Cells[2, 4].Value = "+1-555-123-4567";
                worksheet.Cells[2, 5].Value = "Male";
                worksheet.Cells[2, 6].Value = "New York";
                worksheet.Cells[2, 7].Value = "John Doe";
                
                worksheet.Cells[3, 1].Value = "2024-01-01 11:15:00";
                worksheet.Cells[3, 2].Value = "Jane";
                worksheet.Cells[3, 3].Value = 25;
                worksheet.Cells[3, 4].Value = "+1-555-987-6543";
                worksheet.Cells[3, 5].Value = "Female";
                worksheet.Cells[3, 6].Value = "Los Angeles";
                worksheet.Cells[3, 7].Value = "Jane Smith";
                
                worksheet.Cells[4, 1].Value = "2024-01-01 12:00:00";
                worksheet.Cells[4, 2].Value = "Bob";
                worksheet.Cells[4, 3].Value = 35;
                worksheet.Cells[4, 4].Value = "555-555-5555";
                worksheet.Cells[4, 5].Value = "Male";
                worksheet.Cells[4, 6].Value = "Chicago";
                worksheet.Cells[4, 7].Value = "Bob Johnson";
                
                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();
                
                // Save the file
                var fileInfo = new FileInfo(@"d:\Learnings\csvToVcard\sample_contacts.xlsx");
                package.SaveAs(fileInfo);
            }
            
            Console.WriteLine("Sample Excel file created successfully!");
        }
    }
}