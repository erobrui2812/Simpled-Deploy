using AutoMapper;
using TaskBoard.api.Models.Dtos.BoardDtos;
using TaskBoard.api.Models;

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

            // Mapeo de permisos
            CreateMap<Board, BoardPermissionsDto>()
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore())
                .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CanInvite, opt => opt.Ignore());
        }
    }
}

