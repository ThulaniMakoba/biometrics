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

        [HttpPost("probe-face")]
        public async Task<IActionResult> ProbeFace(ProbeFaceRequest request)
        {
            if (request == null)
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
    }
}
