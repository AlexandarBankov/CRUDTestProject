﻿namespace Management.Models
{
    public class RegisterUserModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string TenantId { get; set; }
    }
}
