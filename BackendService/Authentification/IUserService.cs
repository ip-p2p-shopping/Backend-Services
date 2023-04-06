public interface IUserService
{
    Task RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    // Task<User> GetByIdAsync(string id);
}