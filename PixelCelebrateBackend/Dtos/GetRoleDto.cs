using System.ComponentModel.DataAnnotations;

namespace PixelCelebrateBackend.Dtos
{
    public class GetRoleDto
    {
        public Guid Id { get; set; }
        public string RoleTitle { get; set; }
    }
}