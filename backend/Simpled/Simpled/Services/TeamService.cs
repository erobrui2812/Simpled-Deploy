using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Teams;
using Simpled.Exception;
using Simpled.Models;
using Simpled.Repository;

namespace Simpled.Services
{
    public class TeamService : ITeamRepository, ITeamMemberRepository
    {
        private readonly SimpledDbContext _context;

        public TeamService(SimpledDbContext context)
        {
            _context = context;
        }

        // ITeamRepository

        public async Task<IEnumerable<TeamReadDto>> GetAllByUserAsync(Guid userId)
        {
            // Equipos donde es owner o miembro
            var teams = await _context.Teams
                .Include(t => t.Owner)
                .Include(t => t.Members).ThenInclude(tm => tm.User)
                .Where(t => t.OwnerId == userId
                         || t.Members.Any(tm => tm.UserId == userId))
                .ToListAsync();

            return teams.Select(t => new TeamReadDto
            {
                Id = t.Id,
                Name = t.Name,
                OwnerId = t.OwnerId,
                OwnerName = t.Owner!.Name,
                Members = t.Members.Select(m => new TeamMemberDto
                {
                    UserId = m.UserId,
                    UserName = m.User!.Name,
                    Role = m.Role
                })
            });
        }

        public async Task<TeamReadDto?> GetByIdAsync(Guid id)
        {
            var team = await _context.Teams
                .Include(t => t.Owner)
                .Include(t => t.Members).ThenInclude(tm => tm.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return null;

            return new TeamReadDto
            {
                Id = team.Id,
                Name = team.Name,
                OwnerId = team.OwnerId,
                OwnerName = team.Owner!.Name,
                Members = team.Members.Select(m => new TeamMemberDto
                {
                    UserId = m.UserId,
                    UserName = m.User!.Name,
                    Role = m.Role
                })
            };
        }

        public async Task<TeamReadDto> CreateAsync(TeamCreateDto dto, Guid ownerId)
        {
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = ownerId
            };
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return new TeamReadDto
            {
                Id = team.Id,
                Name = team.Name,
                OwnerId = team.OwnerId,
                OwnerName = (await _context.Users.FindAsync(ownerId))!.Name,
                Members = Array.Empty<TeamMemberDto>()
            };
        }

        public async Task<bool> UpdateAsync(TeamUpdateDto dto, Guid ownerId)
        {
            var team = await _context.Teams.FindAsync(dto.Id);
            if (team == null) throw new NotFoundException("Equipo no encontrado.");
            if (team.OwnerId != ownerId) throw new ApiException("No tienes permisos para modificar este equipo.", 403);

            team.Name = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid ownerId)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) throw new NotFoundException("Equipo no encontrado.");
            if (team.OwnerId != ownerId) throw new ApiException("No tienes permisos para eliminar este equipo.", 403);

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // ITeamMemberRepository

        public async Task<IEnumerable<TeamMemberDto>> GetMembersAsync(Guid teamId)
        {
            var members = await _context.TeamMembers
                .Include(tm => tm.User)
                .Where(tm => tm.TeamId == teamId)
                .ToListAsync();

            return members.Select(m => new TeamMemberDto
            {
                UserId = m.UserId,
                UserName = m.User!.Name,
                Role = m.Role
            });
        }

        public async Task AddMemberAsync(TeamMemberCreateDto dto, Guid ownerId)
        {
            var team = await _context.Teams.FindAsync(dto.TeamId);
            if (team == null) throw new NotFoundException("Equipo no encontrado.");
            if (team.OwnerId != ownerId) throw new ApiException("No tienes permisos para invitar a este equipo.", 403);

            bool exists = await _context.TeamMembers.AnyAsync(tm =>
                tm.TeamId == dto.TeamId && tm.UserId == dto.UserId);
            if (exists) throw new ApiException("Usuario ya es miembro del equipo.", 409);

            _context.TeamMembers.Add(new TeamMember
            {
                TeamId = dto.TeamId,
                UserId = dto.UserId,
                Role = dto.Role
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateMemberAsync(TeamMemberUpdateDto dto, Guid ownerId)
        {
            var team = await _context.Teams.FindAsync(dto.TeamId);
            if (team == null) throw new NotFoundException("Equipo no encontrado.");
            if (team.OwnerId != ownerId) throw new ApiException("No tienes permisos para modificar miembros.", 403);

            var member = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == dto.TeamId && tm.UserId == dto.UserId);
            if (member == null) throw new NotFoundException("Miembro no encontrado.");

            member.Role = dto.Role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberAsync(Guid teamId, Guid userId, Guid ownerId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null) throw new NotFoundException("Equipo no encontrado.");
            if (team.OwnerId != ownerId) throw new ApiException("No tienes permisos para eliminar miembros.", 403);

            var member = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
            if (member == null) throw new NotFoundException("Miembro no encontrado.");

            _context.TeamMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
