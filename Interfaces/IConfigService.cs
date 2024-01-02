using biometricService.Models.Responses;

namespace biometricService.Interfaces
{
    public interface IConfigService
    {
        ComputerConfigResponse GetComputerSid();
    }
}
