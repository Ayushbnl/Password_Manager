﻿namespace Password_Manager.Models
{
    public class PasswordEntry
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? App { get; set; }
        public string? UserName { get; set; }
        public string? EncryptedPassword { get; set; }
    }

}
