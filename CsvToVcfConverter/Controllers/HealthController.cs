using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CsvToVcfConverter.Controllers
{
    [AllowAnonymous]
    public class HealthController : Controller
    {
        [HttpGet("/health")]
        [HttpHead("/health")]
        public IActionResult Check()
        {
            return Content("OK", "text/plain");
        }
    }
}