using Azure;
using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.AccountManagement;
using System.Management;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace biometricService.Services
{
    public class ConfigService : IConfigService
    {
        private readonly AppDbContext _context;
        public ConfigService(AppDbContext context)
        {
            _context = context;
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
        public ComputerConfigResponse GetComputerMotherboardSerialNumber(string idNumber)
        {
            var response = new ComputerConfigResponse();

            if (string.IsNullOrWhiteSpace(idNumber))
            {
                response.ErrorMessage = "Missing ID number";
                response.Success = false;

                return response;
            }

            var configSetting = _context.ConfigSettings
                .FirstOrDefault(c => c.IdNumber == idNumber);

            response.Success = configSetting == null ? false : true;
            response.ErrorMessage = configSetting == null ?
                $"The user with this ID number {idNumber}, is not assigned to a computer" : string.Empty;

            response.ComputerMotherboardSerialNumber = configSetting != null ? configSetting.ComputerUniqueNumber : string.Empty;

            return response;
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
