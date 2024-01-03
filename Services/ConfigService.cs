using Azure;
using biometricService.Interfaces;
using biometricService.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.AccountManagement;
using System.Management;

namespace biometricService.Services
{
    public class ConfigService : IConfigService
    {
        public ComputerConfigResponse GetComputerMotherboardSerialNumber()
        {
            string serialNumber = string.Empty;
            var response = new ComputerConfigResponse();
            try
            {
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * from Win32_BaseBoard");
                foreach (ManagementObject mo in mbs.Get())
                {
                    serialNumber = mo["SerialNumber"].ToString().Trim();
                }

                if (serialNumber.IsNullOrEmpty())
                {
                    response.ErrorMessage = "Computer Motherboard number not found";
                    response.Success = false;
                    return response;
                }

                response.ComputerMotherboardSerialNumber = serialNumber;
                response.Success = true;

                return response;
            }
            catch (Exception e)
            {

                response.ErrorMessage = e.Message.ToString();
                response.Success = false;
                return response;
            }
        }

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
