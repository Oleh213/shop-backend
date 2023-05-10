﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WebShop.Main.Context;

namespace WebShop.Main.Conext
{
    public class User
    {
        public Guid UserId {get;set;}

        public UserRole Role {get;set;}

        public string Name { get; set; }

        public string Password { get; set; }

        public DateTime RegistData { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }

    public enum UserRole {
        Admin,
        User,
        Manager
    }
}