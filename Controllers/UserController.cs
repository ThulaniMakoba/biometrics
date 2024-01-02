using biometricService.Interfaces;
using biometricService.Models;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;


namespace biometricService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpPost("innovatrics-create-customer")]
        //public async Task<IActionResult> CreateCustomer()
        //{
        //    var response = await _userService.CreateInnovatricsCustomer();

        //    if (response != null)
        //    {
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        return BadRequest("Error calling Innovatrics API");
        //    }
        //}

        //[HttpPut("create-liveness/{customerId}")]
        //public async Task<IActionResult> CreateLiveness(Guid customerId)
        //{
        //    if (customerId == Guid.Empty)
        //    {
        //        return BadRequest("Error customerId is empty");
        //    }

        //    var response = await _userService.CreateLiveness(customerId);

        //    return Ok(response);
        //}

        //[HttpPost("passive-liveness-selfie/{customerId}")]
        //public async Task<IActionResult> CreateLivenesSelfie(Guid customerId, CreateLivenessSelfieRequest request)
        //{
        //    if (customerId == Guid.Empty)
        //    {
        //        return BadRequest("Error customerId is empty");
        //    }

        //    await _userService.CreateLivenessSelfie(customerId, request);

        //    return Ok();
        //}

        //[HttpPost("evaluate-passive-liveness/{customerId}")]
        //public async Task<IActionResult> EvaluateLivenesSelfie(Guid customerId)
        //{
        //    if (customerId == Guid.Empty)
        //    {
        //        return BadRequest("Error customerId is empty");
        //    }

        //    var response = await _userService.EvaluateLivenesSelfie(customerId);

        //    return Ok(response);
        //}

        [HttpPost("innovatrics-create-reference-face")]
        public async Task<IActionResult> CreateReferenceFace(CreateReferenceFaceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }

            var response = await _userService.CreateReferenceFace(request);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Error calling Innovatrics API");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUser(user);

            return Ok(result);
        }

        [HttpPost("innovatrics-probe-face")]
        public async Task<IActionResult> ProbeFace(ProbeFaceRequest request)
        {
            if (request == null)//GET WindowsSID
            {
                return BadRequest("Request is empty");
            }

            var response = await _userService.ProbeReferenceFace(request);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Error calling Innovatrics API");
            }

        }

        [HttpPost("verify-user")]
        public async Task<IActionResult> VerifyUser(VerificationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }

            var response = await _userService.VerifyUser(request);

            return Ok(response);
        }
    }
}
