using biometricService.Interfaces;
using biometricService.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Management;

namespace biometricService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _configService;
        public ConfigController(IConfigService configService) { _configService = configService; }

        [HttpGet("computer-sid")]
        public ActionResult<ComputerConfigResponse> GetComputerSid()
        {          
            return _configService.GetComputerSid();
        }

        [HttpGet("computer-motherboard")]
        public ActionResult<ComputerConfigResponse> GetComputerMotherboardId()
        {
            return _configService.GetComputerMotherboardSerialNumber();
        }
    }
}
