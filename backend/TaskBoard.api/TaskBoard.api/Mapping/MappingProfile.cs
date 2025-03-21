using AutoMapper;
using TaskBoard.api.Models.Dtos.BoardDtos;

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
        }
    }
}

