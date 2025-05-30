﻿using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.BoardInvitations;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;

namespace Simpled.Services
{
    /// <summary>
    /// Servicio para la gestión de invitaciones a tableros.
    /// Implementa IBoardInvitationRepository.
    /// </summary>
    public class BoardInvitationService : IBoardInvitationRepository
    {
        private readonly SimpledDbContext _context;

        public BoardInvitationService(SimpledDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las invitaciones pendientes por email.
        /// </summary>
        /// <param name="email">Email del usuario.</param>
        /// <returns>Lista de invitaciones.</returns>
        public async Task<IEnumerable<BoardInvitationReadDto>> GetAllByEmailAsync(string email)
        {
            return await _context.BoardInvitations
                .Where(i => i.Email == email && !i.Accepted)
                .Include(i => i.Board)
                .Select(i => new BoardInvitationReadDto
                {
                    Id = i.Id,
                    BoardId = i.BoardId,
                    BoardName = i.Board!.Name,
                    Role = i.Role,
                    Token = i.Token,
                    Accepted = i.Accepted,
                    CreatedAt = i.CreatedAt
                }).ToListAsync();
        }

        /// <summary>
        /// Obtiene una invitación por su token.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <returns>DTO de la invitación o null si no existe.</returns>
        public async Task<BoardInvitationReadDto?> GetByTokenAsync(string token)
        {
            var i = await _context.BoardInvitations.Include(i => i.Board)
                .FirstOrDefaultAsync(i => i.Token == token);
            if (i == null) return null;

            return new BoardInvitationReadDto
            {
                Id = i.Id,
                BoardId = i.BoardId,
                BoardName = i.Board!.Name,
                Role = i.Role,
                Token = i.Token,
                Accepted = i.Accepted,
                CreatedAt = i.CreatedAt
            };
        }

        /// <summary>
        /// Crea una nueva invitación a un tablero.
        /// </summary>
        /// <param name="dto">Datos de la invitación.</param>
        /// <returns>Entidad BoardInvitation creada.</returns>
        /// <exception cref="ApiException">Si ya existe una invitación pendiente.</exception>
        public async Task<BoardInvitation> CreateAsync(BoardInvitationCreateDto dto)
        {
            if (dto.BoardId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Role))
                throw new ApiException("El ID del tablero, el email y el rol son obligatorios.", 400);
            var exists = await _context.BoardInvitations.AnyAsync(i =>
                i.BoardId == dto.BoardId && i.Email == dto.Email && !i.Accepted);

            if (exists)
                throw new ApiException("Ya existe una invitación pendiente para este usuario en este tablero.", 409);

            var invitation = new BoardInvitation
            {
                Id = Guid.NewGuid(),
                BoardId = dto.BoardId,
                Email = dto.Email.ToLower(),
                Role = dto.Role,
                Token = Guid.NewGuid().ToString()
            };

            _context.BoardInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            return invitation;
        }

        /// <summary>
        /// Acepta una invitación a un tablero.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <param name="userId">ID del usuario que acepta.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        /// <exception cref="NotFoundException">Si la invitación no existe o ya fue aceptada.</exception>
        public async Task<bool> AcceptAsync(string token, Guid userId)
        {
            var invitation = await _context.BoardInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);

            if (invitation == null)
                throw new NotFoundException("Invitación no encontrada o ya aceptada.");

            _context.BoardMembers.Add(new BoardMember
            {
                BoardId = invitation.BoardId,
                UserId = userId,
                Role = invitation.Role
            });

            invitation.Accepted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Rechaza una invitación a un tablero.
        /// </summary>
        /// <param name="token">Token de la invitación.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        /// <exception cref="NotFoundException">Si la invitación no existe o ya fue procesada.</exception>
        public async Task<bool> RejectAsync(string token)
        {
            var invitation = await _context.BoardInvitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.Accepted);

            if (invitation == null)
                throw new NotFoundException("Invitación no encontrada o ya procesada.");

            _context.BoardInvitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
