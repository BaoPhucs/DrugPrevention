﻿using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IUserManagementRepository
    {
        
        
        Task<bool> UpdateAsync(User user);


        Task<User?> GetByIdAsync(int id);
        Task<User?> GetCurrentUserAsync(int currentUserId);
    }
}
