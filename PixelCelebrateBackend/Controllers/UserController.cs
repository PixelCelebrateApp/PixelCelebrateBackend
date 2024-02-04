using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelCelebrateBackend.Dtos;
using PixelCelebrateBackend.Entities;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace PixelCelebrateBackend.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly PixelCelebrateDbContext _context;
        private readonly IMapper _mapper;

        public UserController(PixelCelebrateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public ActionResult<GetUserDto> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User? dbUser = _context.Users.Include(u => u.UserRole).FirstOrDefault(u => u.Username == loginUserDto.Username);
            if (dbUser == null)
            {
                return NotFound();
            }

            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(loginUserDto.Password),
                      dbUser.Salt, 100000, HashAlgorithmName.SHA512, 64);
            bool areEqual = CryptographicOperations.FixedTimeEquals(hashedPassword, Convert.FromHexString(dbUser.Password));
            if (!areEqual)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GetUserDto>(dbUser));
        }

        [HttpGet("{id}")]
        public ActionResult<GetUserDto> GetUser(Guid id)
        {
            User? user = _context.Users.Include(u => u.UserRole).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GetUserDto>(user));
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<ViewUsersDto>> GetAllUsers()
        {
            IEnumerable<User> users = _context.Users.Include(u => u.UserRole).ToList();
            return Ok(_mapper.Map<IEnumerable<ViewUsersDto>>(users));
        }
    }
}