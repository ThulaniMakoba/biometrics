﻿namespace biometricService.Interfaces
{
    public interface ITokenService
    {
        HttpRequestMessage AddBearerToken(HttpRequestMessage request, string token);
        Task<string> GetToken();
    }
}