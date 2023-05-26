using System;
using WebShop.Main.Context;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;

namespace WebShop.Main.BusinessLogic
{
	public class LoggerBL:  ILoggerBL
	{
        private ShopContext _context;

        public LoggerBL(ShopContext context)
        {
            _context = context;
        }
        public void AddLog(LoggerLevel loggerLevel, string message)
        {
            TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

            DateTime utcTime = DateTime.UtcNow;

            DateTime newDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ukraineTimeZone);

            _context.loggers.Add(new Logger { LoggerId = Guid.NewGuid(), Message = message, LoggerLevel = loggerLevel, LogTime = newDate});

            _context.SaveChanges();
        }
    }
}

