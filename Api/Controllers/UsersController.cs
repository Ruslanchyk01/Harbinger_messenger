using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BasicApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDTO, user);
            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Fail update user");
        }

        [HttpPost("addphoto")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile formFile)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(formFile);

            if(result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0)
                photo.IsMain = true;

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDTO>(photo));
            }
            
            return BadRequest("Fail add photo");

        }

        [HttpPut("setmainphoto/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            
            if(photo.IsMain)
                return BadRequest("This photo is already main");
            
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if(currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Fail change main photo");
        }

        // [HttpDelete("deletephoto/{photoId}")]
        // public async Task<ActionResult> DeletePhoto(int photoId)
        // {
        //     var user = await _userRepository
        // }
    }
}