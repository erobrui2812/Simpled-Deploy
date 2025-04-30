using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Simpled.Dtos.Teams;
using Simpled.Dtos.Teams.TeamMembers;
using Simpled.Exception;
using Simpled.Hubs;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamRepository _teamRepo;
        private readonly ITeamMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHubContext<BoardHub> _hub;

        public TeamsController(
            ITeamRepository teamRepo,
            ITeamMemberRepository memberRepo,
            IUserRepository userRepo,
            IHubContext<BoardHub> hub)
        {
            _teamRepo = teamRepo;
            _memberRepo = memberRepo;
            _userRepo = userRepo;
            _hub = hub;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Obtiene todos los equipos en los que participa el usuario (owner o miembro).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamReadDto>), 200)]
        public async Task<IActionResult> GetMyTeams()
        {
            var list = await _teamRepo.GetAllByUserAsync(CurrentUserId);
            return Ok(list);
        }

        /// <summary>
        /// Obtiene un equipo por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeamReadDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeam(Guid id)
        {
            var team = await _teamRepo.GetByIdAsync(id);
            return team is null
                ? NotFound("Equipo no encontrado.")
                : Ok(team);
        }

        /// <summary>
        /// Crea un nuevo equipo. El usuario autenticado será el owner.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TeamReadDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _teamRepo.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetTeam), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza el nombre de un equipo (solo owner).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] TeamUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            try
            {
                await _teamRepo.UpdateAsync(dto, CurrentUserId);
                return NoContent();
            }
            catch (ForbiddenException)
            {
                return Forbid("Solo el owner puede modificar este equipo.");
            }
            catch (NotFoundException)
            {
                return NotFound("Equipo no encontrado.");
            }
        }

        /// <summary>
        /// Elimina un equipo (solo owner).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            try
            {
                await _teamRepo.DeleteAsync(id, CurrentUserId);
                return NoContent();
            }
            catch (ForbiddenException)
            {
                return Forbid("Solo el owner puede eliminar este equipo.");
            }
            catch (NotFoundException)
            {
                return NotFound("Equipo no encontrado.");
            }
        }

        /// <summary>
        /// Obtiene los miembros de un equipo.
        /// </summary>
        [HttpGet("{teamId}/members")]
        [ProducesResponseType(typeof(IEnumerable<TeamMemberDto>), 200)]
        public async Task<IActionResult> GetMembers(Guid teamId)
        {
            var list = await _memberRepo.GetMembersAsync(teamId);
            return Ok(list);
        }

        /// <summary>
        /// Agrega un miembro a un equipo (solo owner) y notifica en tiempo real.
        /// </summary>
        [HttpPost("{teamId}/members")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddMember(Guid teamId, [FromBody] TeamMemberCreateDto dto)
        {
            if (teamId != dto.TeamId)
                return BadRequest("TeamId mismatch.");

            try
            {
                await _memberRepo.AddMemberAsync(dto, CurrentUserId);

                var team = await _teamRepo.GetByIdAsync(teamId);
                var invited = await _userRepo.GetUserByIdAsync(dto.UserId);
                if (team is null) return NotFound("Equipo no encontrado.");
                if (invited is null) return NotFound("Usuario no encontrado.");

                await _hub.Clients
                    .User(invited.Email.ToLower())
                    .SendAsync("TeamInvitationReceived", new
                    {
                        teamName = team.Name,
                        role = dto.Role
                    });

                return Ok("Miembro agregado e invitación enviada.");
            }
            catch (ForbiddenException)
            {
                return Forbid("Solo el owner puede agregar miembros.");
            }
        }

        /// <summary>
        /// Actualiza el rol de un miembro (solo owner).
        /// </summary>
        [HttpPut("{teamId}/members")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateMember(Guid teamId, [FromBody] TeamMemberUpdateDto dto)
        {
            if (teamId != dto.TeamId)
                return BadRequest("TeamId mismatch.");

            try
            {
                await _memberRepo.UpdateMemberAsync(dto, CurrentUserId);
                return NoContent();
            }
            catch (ForbiddenException)
            {
                return Forbid("Solo el owner puede actualizar roles.");
            }
        }

        /// <summary>
        /// Elimina un miembro de un equipo (solo owner).
        /// </summary>
        [HttpDelete("{teamId}/members/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
        {
            try
            {
                await _memberRepo.RemoveMemberAsync(teamId, userId, CurrentUserId);
                return NoContent();
            }
            catch (ForbiddenException)
            {
                return Forbid("Solo el owner puede eliminar miembros.");
            }
        }
    }
}
