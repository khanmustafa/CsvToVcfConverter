using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CsvService _csvService;
    private readonly ExcelService _excelService;
    private readonly VCardService _vCardService;
    private readonly FileStorageService _fileStorageService;

    public HomeController(ILogger<HomeController> logger, CsvService csvService, ExcelService excelService, VCardService vCardService, FileStorageService fileStorageService)
    {
        _logger = logger;
        _csvService = csvService;
        _excelService = excelService;
        _vCardService = vCardService;
        _fileStorageService = fileStorageService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid file.";
            return RedirectToAction("Index");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (fileExtension != ".csv" && fileExtension != ".xlsx" && fileExtension != ".xls")
        {
            TempData["Error"] = "Please upload a CSV or Excel file (.csv, .xlsx, .xls).";
            return RedirectToAction("Index");
        }

        try
        {
            // Save file temporarily
            var tempFilePath = _fileStorageService.SaveTempFile(file);
            
            // Get column names
            List<string> columns;
            using (var stream = _fileStorageService.GetFileStream(tempFilePath))
            {
                if (fileExtension == ".csv")
                {
                    columns = _csvService.GetColumnNames(stream);
                }
                else
                {
                    columns = _excelService.GetColumnNames(stream);
                }
            }

            if (columns.Count == 0)
            {
                _fileStorageService.DeleteTempFile(tempFilePath);
                TempData["Error"] = "No columns found in the file. Please ensure your file has a header row.";
                return RedirectToAction("Index");
            }

            var viewModel = new ColumnSelectionViewModel
            {
                AvailableColumns = columns,
                FileName = file.FileName,
                TempFilePath = tempFilePath
            };

            return View("SelectColumns", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing uploaded file");
            TempData["Error"] = "An error occurred while processing the file. Please ensure it's a valid CSV or Excel file.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult ConvertToVcf(ColumnSelectionViewModel model)
    {
        if (string.IsNullOrEmpty(model.TempFilePath) || !System.IO.File.Exists(model.TempFilePath))
        {
            TempData["Error"] = "File not found. Please upload the file again.";
            return RedirectToAction("Index");
        }

        if (string.IsNullOrEmpty(model.SelectedNameColumn) || string.IsNullOrEmpty(model.SelectedPhoneColumn))
        {
            TempData["Error"] = "Please select both name and phone columns.";
            var columns = GetColumnsFromFile(model.TempFilePath);
            model.AvailableColumns = columns;
            return View("SelectColumns", model);
        }

        try
        {
            var contacts = new List<Contact>();
            var fileExtension = Path.GetExtension(model.TempFilePath).ToLowerInvariant();
            
            using (var stream = _fileStorageService.GetFileStream(model.TempFilePath))
            {
                if (fileExtension == ".csv")
                {
                    contacts = _csvService.ParseCsvFile(stream, model.SelectedNameColumn, model.SelectedPhoneColumn);
                }
                else
                {
                    contacts = _excelService.ParseExcelFile(stream, model.SelectedNameColumn, model.SelectedPhoneColumn);
                }
            }

            // Clean up temp file
            _fileStorageService.DeleteTempFile(model.TempFilePath);

            if (contacts.Count == 0)
            {
                TempData["Error"] = $"No valid contacts found using columns '{model.SelectedNameColumn}' and '{model.SelectedPhoneColumn}'.";
                return RedirectToAction("Index");
            }

            var vcfContent = _vCardService.ConvertContactsToVCard(contacts);
            var fileName = Path.GetFileNameWithoutExtension(model.FileName) + ".vcf";
            
            TempData["Success"] = $"Successfully converted {contacts.Count} contacts to VCF format.";
            return File(System.Text.Encoding.UTF8.GetBytes(vcfContent), "text/vcard", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting file to VCF");
            _fileStorageService.DeleteTempFile(model.TempFilePath);
            TempData["Error"] = "An error occurred while processing the file.";
            return RedirectToAction("Index");
        }
    }

    private List<string> GetColumnsFromFile(string filePath)
    {
        try
        {
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            using (var stream = _fileStorageService.GetFileStream(filePath))
            {
                if (fileExtension == ".csv")
                {
                    return _csvService.GetColumnNames(stream);
                }
                else
                {
                    return _excelService.GetColumnNames(stream);
                }
            }
        }
        catch
        {
            return new List<string>();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
