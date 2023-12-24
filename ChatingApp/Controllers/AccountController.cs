using ChatingApp.Data;
using ChatingApp.Dto;
using ChatingApp.Entities;
using ChatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatingApp.Controllers
{
   
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ItokenService _tokenService;

        public AccountController(DataContext context , ItokenService tokenService)
        {
                _context= context;
                 _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await UserExists(dto.UserName)) return BadRequest("User Name is taken");

            using var hmac = new HMACSHA512();
            AppUser user = new AppUser()
            {
                UserName = dto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Add(user);
           await   _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.GreateToken(user)
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(s => s.UserName == username.ToLower());
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            AppUser user= await _context.Users.SingleOrDefaultAsync(s=>s.UserName ==  loginDto.UserName);
            if (user is null) return Unauthorized("invalid UserName");

            var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0 ; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.GreateToken(user)
            };

        }
    }
}
