using AutoMapper;
using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Models;
using TaskBoard.api.Models.Dtos.AuthDtos;
using Microsoft.AspNetCore.Identity;

namespace TaskBoard.api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BoardCreateDto, Board>();
            CreateMap<Board, BoardResponseDto>();
            CreateMap<Column, ColumnDto>();
            CreateMap<Item, ItemDto>();

            CreateMap<Board, BoardPermissionsDto>()
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore())
                .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CanInvite, opt => opt.Ignore());

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom<UserRolesResolver>());
        }
    }

    public class UserRolesResolver : IValueResolver<User, UserProfileDto, List<string>>
    {
        private readonly UserManager<User> _userManager;

        public UserRolesResolver(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public List<string> Resolve(
            User source,
            UserProfileDto destination,
            List<string> destMember,
            ResolutionContext context)
        {
            var roles = _userManager.GetRolesAsync(source).GetAwaiter().GetResult();
            return roles.ToList();
        }
    }

}



