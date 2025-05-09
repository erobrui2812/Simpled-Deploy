using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.BoardMembers;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;
using Simpled.Validators;
using FluentValidation;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de miembros de tableros.
    /// Implementa IBoardMemberRepository.
    /// </summary>
    public class BoardMemberService : IBoardMemberRepository
    {
        private readonly SimpledDbContext _context;

        public BoardMemberService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los miembros de todos los tableros.
        /// </summary>
        /// <returns>Lista de miembros.</returns>
        public async Task<IEnumerable<BoardMemberReadDto>> GetAllAsync()
        {
            return await _context.BoardMembers
                .Select(m => new BoardMemberReadDto
                {
                    BoardId = m.BoardId,
                    UserId = m.UserId,
                    Role = m.Role
                }).ToListAsync();
        }

        /// <summary>
        /// Obtiene los miembros de un tablero específico.
        /// </summary>
        /// <param name="boardId">ID del tablero.</param>
        /// <returns>Lista de miembros del tablero.</returns>
        public async Task<IEnumerable<BoardMemberReadDto>> GetByBoardIdAsync(Guid boardId)
        {
            return await _context.BoardMembers
                .Where(m => m.BoardId == boardId)
                .Select(m => new BoardMemberReadDto
                {
                    BoardId = m.BoardId,
                    UserId = m.UserId,
                    Role = m.Role
                }).ToListAsync();
        }

        /// <summary>
        /// Obtiene un miembro específico por IDs de tablero y usuario.
        /// </summary>
        /// <param name="boardId">ID del tablero.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>DTO del miembro o null si no existe.</returns>
        public async Task<BoardMemberReadDto?> GetByIdsAsync(Guid boardId, Guid userId)
        {
            var member = await _context.BoardMembers.FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId);
            if (member == null)
                return null;

            return new BoardMemberReadDto
            {
                BoardId = member.BoardId,
                UserId = member.UserId,
                Role = member.Role
            };
        }

        /// <summary>
        /// Añade un nuevo miembro a un tablero.
        /// </summary>
        /// <param name="dto">Datos del miembro a añadir.</param>
        /// <exception cref="ApiException">Si el miembro ya existe.</exception>
        public async Task AddAsync(BoardMemberCreateDto dto)
        {
            var validator = new BoardMemberCreateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);
            bool exists = await _context.BoardMembers.AnyAsync(m =>
                m.BoardId == dto.BoardId && m.UserId == dto.UserId);
            if (exists)
                throw new ApiException("El miembro ya existe en el tablero.", 409);

            var newMember = new BoardMember
            {
                BoardId = dto.BoardId,
                UserId = dto.UserId,
                Role = dto.Role
            };

            _context.BoardMembers.Add(newMember);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Añade varios miembros a tableros.
        /// </summary>
        /// <param name="dtos">Lista de miembros a añadir.</param>
        public async Task AddManyAsync(List<BoardMemberCreateDto> dtos)
        {
            var newMembers = new List<BoardMember>();

            foreach (var dto in dtos)
            {
                bool exists = await _context.BoardMembers.AnyAsync(m =>
                    m.BoardId == dto.BoardId && m.UserId == dto.UserId);

                if (!exists)
                {
                    newMembers.Add(new BoardMember
                    {
                        BoardId = dto.BoardId,
                        UserId = dto.UserId,
                        Role = dto.Role
                    });
                }
            }

            _context.BoardMembers.AddRange(newMembers);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza el rol de un miembro en un tablero.
        /// </summary>
        /// <param name="dto">Datos del miembro a actualizar.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        /// <exception cref="NotFoundException">Si el miembro no existe.</exception>
        public async Task<bool> UpdateAsync(BoardMemberUpdateDto dto)
        {
            var validator = new BoardMemberUpdateValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ApiException(validationResult.Errors[0].ErrorMessage, 400);
            var existing = await _context.BoardMembers.FirstOrDefaultAsync(m =>
                m.BoardId == dto.BoardId && m.UserId == dto.UserId);
            if (existing == null)
                throw new NotFoundException("Miembro no encontrado.");

            existing.Role = dto.Role;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un miembro de un tablero.
        /// </summary>
        /// <param name="boardId">ID del tablero.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        /// <exception cref="NotFoundException">Si el miembro no existe.</exception>
        public async Task<bool> DeleteAsync(Guid boardId, Guid userId)
        {
            var member = await _context.BoardMembers.FirstOrDefaultAsync(m =>
                m.BoardId == boardId && m.UserId == userId);
            if (member == null)
                throw new NotFoundException("Miembro no encontrado.");

            _context.BoardMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
