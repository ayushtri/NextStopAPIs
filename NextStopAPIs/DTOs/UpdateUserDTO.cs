﻿namespace NextStopAPIs.DTOs
{
    public class UpdateUserDTO
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
