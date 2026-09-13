using System;

namespace PasswordProtectionSystem.Services
{
    public static class SaltService
    {
        
        public static string GenerateSalt(string username, string userId, DateTime timestamp)
        {
            string combined = username + userId + timestamp.ToString("O"); // ISO 8601 format
            return HashService.ComputeSha256(combined);
        }
    }
}