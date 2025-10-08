using Microsoft.AspNetCore.Mvc;

namespace CsvToVcfConverter.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet("/health")]
        public IActionResult Check()
        {
            return Content("OK", "text/plain");
        }
    }
}