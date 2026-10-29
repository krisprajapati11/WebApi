using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.GenericResponse;
using MyFirstApi.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace MyFirstApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

       

        public async Task<Tuple<int, TokenDto>> LoginUser(UserDto dto)
        {
            try
            {
                var tokenDto = new TokenDto();

                if (dto == null)
                {
                    tokenDto.Message = "Please Fill All the details!";

                    return new Tuple<int, TokenDto>(0, tokenDto);
                }
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if(existingUser == null)
                {
                    tokenDto.Message = "This User Not Exist, Please Login";
                   
                    return new Tuple<int, TokenDto>(0, tokenDto);
                }

                //if(existingUser.Password != dto.Password)
                //{
                //    return new Tuple<int,string>(1, "PassWord Incorrect");
                //}
                var passwordhasher = new PasswordHasher<string>();

                var verifyPassword = passwordhasher.VerifyHashedPassword(dto.Email, existingUser.Password, dto.Password);
                
                if(verifyPassword == PasswordVerificationResult.Success)
                {


                    //UserDto user = new();
                    //user.Email = dto.Email;
                    //user.Id = existingUser.Id;

                    UserDto user = new()
                    {
                        Id = existingUser.Id,
                        Name = existingUser.Name,
                        Email = existingUser.Email

                    };



                    var token = GetJwtToken(user);
                    tokenDto.Token = token;
                    tokenDto.Message = "Login Successfully";
                    

                    return new Tuple<int,TokenDto> ( 2, tokenDto);
                }
                else if(verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    UserDto user = new()
                    {
                        Id = existingUser.Id,
                        Name = existingUser.Name,
                        Email = existingUser.Email

                    };

                    var token = GetJwtToken(user);
                     
                    existingUser.Password = PasswordHashing(dto);
                    _context.Users.Update(existingUser);
                    _context.SaveChanges();

                    tokenDto.Token = token;
                    tokenDto.Message = "Login Successfullly , New Has Generated";


                    return new Tuple<int, TokenDto>(2, tokenDto);

                }
                else if(verifyPassword == PasswordVerificationResult.Failed)
                {
                    tokenDto.Message = "Password Incorrect";
                    return new Tuple<int, TokenDto>(1, tokenDto);
                }
                tokenDto.Message = "This User Not Exist , Please Login";

                return new Tuple<int, TokenDto>(1, tokenDto);


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Tuple<int,string>> RegisterUser(UserDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Name))
                {
                    return new Tuple<int, string>(0,"Enter all the details(Email,Password and Name");
                }
                var existingUser = await _context.Users.AnyAsync(x => x.Email == dto.Email);

                if(existingUser)
                {
                    return new Tuple<int, string>(0, "This User is already exist, Please register with new User!");
                }

                _context.Users.Add(new Entities.User
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = PasswordHashing(dto),
                }); 

                _context.SaveChanges();

                return new Tuple<int, string>(1, "User Registered Successfully");
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string PasswordHashing(UserDto dto)
        {
            var passwordhasher = new PasswordHasher<string>();

            var hash = passwordhasher.HashPassword(dto.Email, dto.Password);

            return hash;

        }

        public string GetJwtToken(UserDto dto)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,dto.Name),
                new Claim(ClaimTypes.NameIdentifier,dto.Id.ToString())
            };

            var secretkey = _configuration["JWT:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "krish-client",
                audience: "krish-backend",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: creds

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
 
    }
}
