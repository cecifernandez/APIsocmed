﻿namespace APISocMed.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false; 
        public bool IsUsed { get; set; } = false;
        public int UserId { get; set; } 
        public User User { get; set; } = null!;
    }
}
