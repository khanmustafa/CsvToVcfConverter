namespace CsvToVcfConverter.Models
{
    public class ColumnSelectionViewModel
    {
        public List<string> AvailableColumns { get; set; } = new List<string>();
        public string SelectedNameColumn { get; set; } = string.Empty;
        public string SelectedPhoneColumn { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string TempFilePath { get; set; } = string.Empty;
    }
}