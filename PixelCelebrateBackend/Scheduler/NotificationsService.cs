using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PixelCelebrateBackend.Dtos;
using PixelCelebrateBackend.Entities;
using System.Data;

namespace PixelCelebrateBackend.Scheduler
{
    public class NotificationsService
    {
        private PixelCelebrateDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NotificationsService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            var optionsBuilder = new DbContextOptionsBuilder<PixelCelebrateDbContext>().
                UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            _context = new PixelCelebrateDbContext(optionsBuilder.Options);
            _mapper = mapper;
        }

        private void ReloadDbContext(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PixelCelebrateDbContext>().
                UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            _context = new PixelCelebrateDbContext(optionsBuilder.Options);
        }

        public int GetBirthdayNotification()
        {
            ReloadDbContext(_configuration);
            Configuration? configuration = _context.Configurations.FirstOrDefault(c => c.Title == "Birthday Notification");
            if (configuration == null)
            {
                return -1;
            }

            return configuration.Value;
        }

        public List<GetUserDto> GetBirthdayUsers(DateOnly date)
        {
            ReloadDbContext(_configuration);
            List<User> users = _context.Users.Where(u => u.Birthdate.Month == date.Month && u.Birthdate.Day == date.Day).ToList();
            return _mapper.Map<List<GetUserDto>>(users);
        }

        public List<GetUserDto> GetOtherUsers(DateOnly date)
        {
            ReloadDbContext(_configuration);
            List<User> users = _context.Users.Where(u => u.Birthdate.Month != date.Month || u.Birthdate.Day != date.Day).ToList();
            return _mapper.Map<List<GetUserDto>>(users);
        }
    }
}