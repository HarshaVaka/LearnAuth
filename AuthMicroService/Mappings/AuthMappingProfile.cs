using AuthMicroService.DTOs;
using AuthMicroService.Entities;
using AutoMapper;

namespace AuthMicroService.Mappings
{
    public class AuthMappingProfile:Profile
    {
        public AuthMappingProfile() 
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            CreateMap<User, GenerateAccessTokenDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.UserRoles.Select(ur => ur.Role.RoleName).ToArray()));

            // GenerateAccessTokenDto → User
            CreateMap<GenerateAccessTokenDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
        }
    }
}
