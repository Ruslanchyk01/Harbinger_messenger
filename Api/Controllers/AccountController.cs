using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class AccountController : BasicApiController
    {
        private readonly DataContext _context;
        private readonly ITokenCreator _tokenCreator;
        public AccountController(DataContext context, ITokenCreator tokenCreator)
        {
            _tokenCreator = tokenCreator;
            _context = context;

        }

        [HttpPost("registration")]
        public async Task<ActionResult<UserDTO>> Register(RegistrationDTO registrationDTO)
        {
            if(await IsUserExists(registrationDTO.UserName))
                return BadRequest("User name is taken");
            using var hmac = new HMACSHA512();

            var appUser = new AppUser
            {
                UserName = registrationDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(appUser);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserName = appUser.UserName,
                Token = _tokenCreator.CreateToken(appUser)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var appUser = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if(appUser == null) 
                return Unauthorized("Invalid User name");
            
            using var hmac = new HMACSHA512(appUser.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != appUser.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }
            
            return new UserDTO
            {
                UserName = appUser.UserName,
                Token = _tokenCreator.CreateToken(appUser)
            };
        }

        private async Task<bool> IsUserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}