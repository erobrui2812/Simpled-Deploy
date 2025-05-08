using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/items/{itemId:guid}/activity")]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogRepository _activityRepo;

        public ActivityLogsController(IActivityLogRepository activityRepo)
        {
            _activityRepo = activityRepo;
        }

        /// <summary>
        /// Obtiene el historial de actividad de un ítem.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLogs(Guid itemId)
        {
            var logs = await _activityRepo.GetByItemIdAsync(itemId);
            return Ok(logs);
        }
    }
}
