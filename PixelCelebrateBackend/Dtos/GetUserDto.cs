using System.ComponentModel.DataAnnotations;

namespace PixelCelebrateBackend.Dtos
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public DateOnly Birthdate { get; set; }
        public string UserRole { get; set; }
    }
}