using biometricService.Interfaces;
using biometricService.Models.Responses;
using System.DirectoryServices.AccountManagement;

namespace biometricService.Services
{
    public class ConfigService : IConfigService
    {
        public ComputerConfigResponse GetComputerSid()
        {
            string computerName = Environment.MachineName;
            var response = new ComputerConfigResponse();
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(context, computerName);

                    if (computer != null)
                    {
                        response.ComputerSidNumber = computer.Sid.Value.ToString();
                        response.Success = true;

                        return response;
                    }
                    else
                    {
                        response.ErrorMessage = "Computer not found in Active Directory";
                        response.Success = false;
                        return response;
                    }
                }
            }
            catch (Exception e)
            {

                response.ErrorMessage = e.Message.ToString();
                response.Success = false;
                return response;
            }
        }
    }
}
