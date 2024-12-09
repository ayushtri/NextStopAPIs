﻿namespace NextStopAPIs.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
