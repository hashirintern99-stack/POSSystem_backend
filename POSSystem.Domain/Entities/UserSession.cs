using System;

namespace POSSystem.Domain.Entities
{
    public class UserSession
    {
        public Guid SessionId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}