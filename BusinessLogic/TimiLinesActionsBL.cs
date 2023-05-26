using System;
using Microsoft.EntityFrameworkCore;
using sushi_backend.Context;
using sushi_backend.Interfaces;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;

namespace sushi_backend.BusinessLogic
{

    public class TimeLinesActionsBL : ITimeLinesActionsBL
    {
        private ShopContext _context;

        public TimeLinesActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<List<TimeLine>> ShowTimeLines()
            => await _context.timeLines.ToListAsync();

        public async Task<User> GetUser(Guid userId)
            => await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        

        public async Task<bool> AddTimeLine(DateTime from, DateTime to, TimeConfig timeConfig, bool isOpen, string note,int priority)
        {
            _context.timeLines.Add(new TimeLine()
            {
                From = from,
                To = to,
                TimeConfig = timeConfig,
                IsOpen = isOpen,
                Note = note,
                Priority = priority,
            });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditTimeLine(EditTimeLineModel model)
        {

            var timeline = await _context.timeLines.FirstOrDefaultAsync(x => x.TimeLineId == model.TimeLineId);

            if(timeline != null && model.IsOpen != null && model.From != null && model.Note != null && model.To != null && model.TimeConfig != null && model.Priority != null)
            {
                timeline.To = model.To;
                timeline.IsOpen = model.IsOpen;
                timeline.Priority = model.Priority;
                timeline.TimeConfig = model.TimeConfig;
                timeline.Note = model.Note;
                timeline.From = model.From;

                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteTimeLine(Guid timeLineId)
        {
            var timeline = await _context.timeLines.FirstOrDefaultAsync(x => x.TimeLineId == timeLineId);

            if (timeline != null)
            {
                _context.timeLines.Remove(timeline);

                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CloseShopTillToday()
        {
            TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

            DateTime utcTime = DateTime.UtcNow;

            DateTime newDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ukraineTimeZone);

            var timeLines = await _context.timeLines.ToListAsync();

            var curentTimeLines = new List<TimeLine>();

            foreach (var item in timeLines)
            {
                if (item.To.Day == newDate.Day || item.From.Day == newDate.Day || (item.TimeConfig == TimeConfig.Repeat && item.From.DayOfWeek == newDate.DayOfWeek || item.TimeConfig == TimeConfig.Repeat && item.To.DayOfWeek == newDate.DayOfWeek))
                {
                    curentTimeLines.Add(item);
                }
            }

            _context.timeLines.Add(new TimeLine()
            {
                From = newDate,
                To = newDate.AddHours(23 - newDate.Hour),
                TimeConfig = TimeConfig.Once,
                IsOpen = false,
                Note = "Негайне закритя",
                Priority = curentTimeLines.Max(x=>x.Priority) +1,
            });

            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<bool> CheckShopWork()
        {
            TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

            DateTime utcTime = DateTime.UtcNow;

            DateTime newDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ukraineTimeZone);

            var timeLines = await _context.timeLines.ToListAsync();
   
            var curentTimeLines = new List<TimeLine>();

            foreach(var item in timeLines)
            {
                if(item.To.Day == newDate.Day || item.From.Day == newDate.Day ||(item.TimeConfig == TimeConfig.Repeat && item.From.DayOfWeek == newDate.DayOfWeek || item.TimeConfig == TimeConfig.Repeat && item.To.DayOfWeek == newDate.DayOfWeek))
                {
                    curentTimeLines.Add(item);
                }
            }

            bool isOpen = false;

            int highestPriority = int.MinValue;

            foreach (var timeline in curentTimeLines)
            {
                if (newDate >= timeline.From && newDate <= timeline.To)
                {
                    if (timeline.Priority > highestPriority)
                    {
                        highestPriority = timeline.Priority;

                        isOpen = timeline.IsOpen;

                    }
                }
            }

            return isOpen;
        }
	}
}

