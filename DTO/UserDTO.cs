﻿using System;
namespace WebShop.Main.DTO
{
    public class UserDTO
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public byte UserRole { get; set; }
    }
}

