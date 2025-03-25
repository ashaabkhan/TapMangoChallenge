using TapMangoChallenge.Models;
using TapMangoChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace TapMangoChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly RateLimiterService _rateLimiter;

        public SmsController(RateLimiterService rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        // POST /sms/can-send
        [HttpPost("can-send")]
        public IActionResult CanSend([FromBody] SmsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest(new { canSend = false, message = "Phone number is required." });
            
            var phoneRegex = new Regex(@"^(?=\+?[1-9]\d{9,14}$)\+?[1-9]\d{9,14}$");
            if (!phoneRegex.IsMatch(request.PhoneNumber))
                return BadRequest(new { canSend = false, message = "Invalid phone number format." });
            
            bool allowed = _rateLimiter.CanSend(request.PhoneNumber);

            if (!allowed)
            {
                return Ok(new
                {
                    canSend = false,
                    message = "Limit exceeded: This message cannot be sent at this time."
                });
            }

            return Ok(new
            {
                canSend = true,
                message = "Message can be sent."
            });
        }
    }
}