using biometricService.Interfaces;
using biometricService.Models;
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

        [HttpGet("computer-motherboard/{idNumber}")]
        public ActionResult<ComputerConfigResponse> GetComputerMotherboardId(string idNumber)
        {
            return _configService.GetComputerMotherboardSerialNumber(idNumber);
        }

        [HttpPost("store-computer-serial-number")]
        public void PostComputerConfig(ComputerConfigRequest request)
        {
            _configService.StoreComputerConfig(request);
        }
    }
}
