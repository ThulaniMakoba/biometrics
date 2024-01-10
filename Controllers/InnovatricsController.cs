using biometricService.Interfaces;
using biometricService.Models;
using Microsoft.AspNetCore.Mvc;

namespace biometricService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InnovatricsController : ControllerBase
    {
        private readonly IInnovatricsService _innovatricsService;

        public InnovatricsController(IInnovatricsService innovatricsService)
        {
            _innovatricsService = innovatricsService;
        }

        [HttpPost("innovatrics-create-customer")]
        public async Task<IActionResult> CreateCustomer()
        {
            var response = await _innovatricsService.CreateInnovatricsCustomer();

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Error calling Innovatrics API");
            }
        }

        [HttpPut("create-liveness/{customerId}")]
        public async Task<IActionResult> CreateLiveness(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                return BadRequest("Error customerId is empty");
            }

            var response = await _innovatricsService.CreateLiveness(customerId);

            return Ok(response);
        }

        [HttpPost("passive-liveness-selfie/{customerId}")]
        public async Task<IActionResult> CreateLivenesSelfie(Guid customerId, CreateLivenessSelfieRequest request)
        {
            if (customerId == Guid.Empty)
            {
                return BadRequest("Error customerId is empty");
            }

            await _innovatricsService.CreateLivenessSelfie(customerId, request);

            return Ok();
        }

        [HttpPost("evaluate-passive-liveness/{customerId}")]
        public async Task<IActionResult> EvaluateLivenesSelfie(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                return BadRequest("Error customerId is empty");
            }

            var response = await _innovatricsService.EvaluateLivenesSelfie(customerId);

            return Ok(response);
        }

        [HttpPost("create-reference-face")]
        public async Task<IActionResult> CreateReferenceFace(CreateReferenceFaceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }

            var response = await _innovatricsService.CreateReferenceFace(request);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Error calling Innovatrics API");
            }
        }

        [HttpPost("create-reference-face-with-out-background")]
        public async Task<IActionResult> CreateReferenceFaceWithOutBackground(CreateReferenceFaceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }

            var response = await _innovatricsService.CreateReferenceFaceWithOutBackGround(request);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Error calling Innovatrics API");
            }
        }
    }
}
