using biometricService.Interfaces;
using biometricService.Models;
using Microsoft.AspNetCore.Mvc;


namespace biometricService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService, IHttpClientFactory httpClientFactory)
        {
            _userService = userService;
        }

        [HttpPost("innovatrics-create-customer")]
        public async Task<IActionResult> CreateCustomer()
        {
            var response = await _userService.CreateInnovatricsCustomer();

            if (response?.id != null)
            {
                return Ok(response.id);
            }
            else
            {
                return BadRequest("Error calling Innpvatrics API");
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

            return Ok();
        }
    }
}
