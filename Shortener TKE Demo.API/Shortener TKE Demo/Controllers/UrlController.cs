using Microsoft.AspNetCore.Mvc;
using Shortener_TKE_Demo.Contracts;
using Shortener_TKE_Demo.Services;

namespace Shortener_TKE_Demo.Controllers
{
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _service;
        private readonly ILogger<UrlController> _logger;

        public UrlController(
            IUrlService service,
            ILogger<UrlController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("api/shorten")]
        [ProducesResponseType(typeof(ShortenUrlResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<ShortenUrlResponseDto> Shorten([FromBody] ShortenUrlRequestDto request)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            _logger.LogInformation("Shorten request received for {LongUrl}", request.LongUrl);

            var result = _service.Shorten(request.LongUrl, baseUrl);

            _logger.LogInformation("Short url created/resolved: {ShortCode}", result.ShortCode);

            return Ok(result);
        }

        [HttpGet("{shortCode}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RedirectToLongUrl([FromRoute] string shortCode)
        {
            if (_service.Resolve(shortCode, out var longUrl))
            {
                _logger.LogInformation("Redirecting code {ShortCode} -> {LongUrl}", shortCode, longUrl);
                return Redirect(longUrl);
            }

            _logger.LogWarning("ShortCode not found: {ShortCode}", shortCode);
            return NotFound();
        }
    }
}
