// src/Repository/IActivityLogRepository.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simpled.Models;
using Simpled.Dtos.ActivityLogs;

namespace Simpled.Repository
{
 
    public interface IActivityLogRepository
    {
  
        Task<IEnumerable<ActivityLogReadDto>> GetByItemIdAsync(Guid itemId);

    
        Task AddAsync(ActivityLog log);
    }
}
