using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.api.Data;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Utils;

namespace TaskBoard.api.Filters
{
    public class BoardAccessFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _context;

        public BoardAccessFilter(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var boardId = (Guid)context.ActionArguments["boardId"]!;
            var userId = context.HttpContext.User.GetUserId();

            var hasAccess = await _context.BoardMembers
                .AnyAsync(m => m.BoardId == boardId && m.UserId == userId);

            if (!hasAccess)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
