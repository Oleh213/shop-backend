﻿using System;
using System.ComponentModel.DataAnnotations;
using WebShop.Main.Conext;

namespace WebShop.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		public string Email { get; set; }

        public UserRole UserRole { get; set; }

    }
}

