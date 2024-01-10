using biometricService.Models;
using biometricService.Models.Responses;

namespace biometricService.Interfaces
{
    public interface IConfigService
    {
        ComputerConfigResponse GetComputerSid();
        ComputerConfigResponse GetComputerMotherboardSerialNumber();
        ComputerConfigResponse StoreComputerConfig(ComputerConfigRequest request);
        Task<RegisterServiceResponse> RegisterUserComputerDetails(string userId);
    }
}
