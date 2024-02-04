using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelCelebrateBackend.Dtos;
using PixelCelebrateBackend.Entities;
using PixelCelebrateBackend.Scheduler;
using System.Security.Cryptography;
using System.Text;

namespace PixelCelebrateBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly PixelCelebrateDbContext _context;
        private readonly IMapper _mapper;
        private readonly NotificationsScheduler _scheduler;

        public AdminController(PixelCelebrateDbContext context, IMapper mapper, NotificationsScheduler scheduler)
        {
            _context = context;
            _mapper = mapper;
            _scheduler = scheduler;
        }

        [HttpPost]
        public ActionResult<GetUserDto> AddUser([FromBody] AddUserDto addUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User? dbUser = _context.Users.FirstOrDefault(u => u.Email == addUserDto.Email || u.Username == addUserDto.Username);
            if (dbUser != null)
            {
                return Conflict("There is already a user registered with this email/username!");
            }

            Role? role = _context.Roles.FirstOrDefault(r => r.Id == addUserDto.UserRole);
            if (role == null)
            {
                return NotFound("There is no such role!");
            }

            byte[] Salt = RandomNumberGenerator.GetBytes(64);
            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(addUserDto.Password), Salt, 100000,
                HashAlgorithmName.SHA512, 64);
            addUserDto.Password = Convert.ToHexString(hashedPassword);

            User user = _mapper.Map<User>(addUserDto);
            user.UserRole = role;
            user.Salt = Salt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(AddUser), _mapper.Map<GetUserDto>(user));
        }

        [HttpGet("roles")]
        public ActionResult<IEnumerable<GetRoleDto>> GetRoles()
        {
            IEnumerable<Role> roles = _context.Roles.ToList();
            return Ok(_mapper.Map<IEnumerable<GetRoleDto>>(roles));
        }

        [HttpPost("configuration")]
        public ActionResult<int> ChangeBirthdayNotification([FromBody] ChangeConfigurationDto changeConfigurationDto)
        {
            Configuration? configuration = _context.Configurations.FirstOrDefault(c => c.Title == "Birthday Notification");
            if (configuration == null)
            {
                return BadRequest();
            }
            configuration.Value = changeConfigurationDto.Value;
            _context.SaveChanges();

            //restart scheduler:
            Console.WriteLine("Schedule value: " + _scheduler._notificationValue);
            _scheduler.StopAsync().ConfigureAwait(false);
            _scheduler.Start();

            return Ok(configuration.Value);
        }

        [HttpGet("configuration")]
        public ActionResult<int> GetBirthdayNotification()
        {
            Configuration? configuration = _context.Configurations.FirstOrDefault(c => c.Title == "Birthday Notification");
            if (configuration == null)
            {
                return BadRequest();
            }

            return Ok(configuration.Value);
        }
    }
}