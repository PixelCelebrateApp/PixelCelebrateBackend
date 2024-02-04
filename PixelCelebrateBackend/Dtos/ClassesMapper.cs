using AutoMapper;
using PixelCelebrateBackend.Entities;

namespace PixelCelebrateBackend.Dtos
{
    public class ClassesMapper : Profile
    {
        public ClassesMapper()
        {
            CreateMap<AddUserDto, User>().ForMember(dest => dest.UserRole, opt => opt.Ignore());
            CreateMap<User, GetUserDto>().ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.RoleTitle));
            CreateMap<User, ViewUsersDto>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.DayOfBirth, opt => opt.MapFrom(src => src.Birthdate.Day))
                .ForMember(dest => dest.MonthOfBirth, opt => opt.MapFrom(src => src.Birthdate.Month))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.RoleTitle));
            CreateMap<Role, GetRoleDto>().IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}