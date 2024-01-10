namespace biometricService.Interfaces
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint);
        Task<TResult> PostAsync<TRequest, TResult>(string endpoint, TRequest data);
        Task<TResult> PutAsync<TRequest, TResult>(string endpoint, TRequest data);
        Task<TResult> PutAsync<TResult>(string endpoint);
    }
}