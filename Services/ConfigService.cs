using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.AccountManagement;
using System.Management;

namespace biometricService.Services
{
    public class ConfigService : IConfigService
    {
        private readonly AppDbContext _context;

        private readonly HttpClient _httpClient = new HttpClient();
        public ConfigService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }
        public ComputerConfigResponse StoreComputerConfig(ComputerConfigRequest request)
        {
            var query = _context.ConfigSettings
                .FirstOrDefault(x => x.IdNumber == request.IdNumber);

            var response = new ComputerConfigResponse();

            if (query != null)
            {
                response.ErrorMessage = $"The user with this ID number {query.IdNumber}, has assigned computer";
                response.Success = false;
                response.ComputerMotherboardSerialNumber = query.ComputerUniqueNumber;

                return response;
            }

            var configSetting = new ConfigSetting
            {
                ComputerUniqueNumber = request.ComputerSerialNumber,
                IdNumber = request.IdNumber,
                ComputerName = request.ComputerName,
            };

            _context.Add(configSetting);
            _context.SaveChanges();

            return response;
        }
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

        public async Task<RegisterServiceResponse> RegisterUserComputerDetails(string userId)
        {
            string apiEndpoint = $"http://localhost:5000/api/machineDetails/register-service/{userId}";

            HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<RegisterServiceResponse>();
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

        }
    }
}