using MyFirstApi.DTOs;

namespace MyFirstApi.IServices
{
    public interface IAuthService
    {
        Task<Tuple<int, TokenDto>> LoginUser(UserDto dto);
        Task<Tuple<int, string>> RegisterUser(UserDto dto);
    }
}
