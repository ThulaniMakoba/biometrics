﻿using biometricService.Interfaces;
using biometricService.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("echo")]
        public IActionResult Get()
        {
            var data = new { message = "Echoiing.....!" };
            return Ok(data);
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
        public async Task<IActionResult> ProbeFace(CreateReferenceFaceRequest request)
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

        [HttpPost("validate-authentication-option")]
        public async Task<IActionResult> ValidateAuthenticationOption(AuthenticationOptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is empty");
            }

            var response = await _userService.ValidateAuthenticationOption(request);

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
