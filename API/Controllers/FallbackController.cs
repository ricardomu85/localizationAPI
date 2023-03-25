using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FallbackController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("WebAPI Localizacion corriento");
        }
    }
}