using System;
using WebShop.Main.Conext;
using WebShop.Models;

namespace sushi_backend.Interfaces
{
	public interface IEmailSender
	{
        bool SentEmail(string Message);

    }
}

