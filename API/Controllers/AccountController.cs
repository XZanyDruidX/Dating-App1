using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        public AccountController( DataContext context)
        {
            this.context = context;
           
        }

        [HttpPost("register")]  //POST: api/account/register
        public async Task<ActionResult<AppUser>> Register (RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest ("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if ( user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < ComputeHash.Length; i++)
            {
                if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await this.context.Users.AnyAsync( x => x.UserName == username.ToLower());
        }

        
    }
}
