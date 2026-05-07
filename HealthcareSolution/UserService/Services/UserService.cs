using UserService.Models;
using UserService.Repositories;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public Task<User?> GetByUsernameAsync(string username) => _repo.GetByUsernameAsync(username);

        public Task<User> CreateAsync(User user) => _repo.AddAsync(user);
    }
}
