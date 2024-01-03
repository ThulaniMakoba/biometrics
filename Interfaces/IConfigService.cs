using biometricService.Models;
using biometricService.Models.Responses;

namespace biometricService.Interfaces
{
    public interface IConfigService
    {
        ComputerConfigResponse GetComputerSid();
        ComputerConfigResponse GetComputerMotherboardSerialNumber(string idNumber);
        ComputerConfigResponse StoreComputerConfig(ComputerConfigRequest request);
    }
}
