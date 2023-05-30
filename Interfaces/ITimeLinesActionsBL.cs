using System;
using sushi_backend.Context;
using sushi_backend.Models;
using WebShop.Main.Conext;

namespace sushi_backend.Interfaces
{
	public interface ITimeLinesActionsBL
	{
        Task<List<TimeLine>> ShowTimeLines();

        Task<User> GetUser(Guid userId);

        Task<bool> AddTimeLine(DateTime from, DateTime to, TimeConfig timeConfig, bool isOpen, string note, int priority);

        Task<bool> EditTimeLine(EditTimeLineModel model);

        Task<bool> CheckShopWork();

        Task<bool> CloseShopTillToday();

        Task<bool> OpenShopTillToday();

        Task<bool> DeleteTimeLine(Guid timeLineId);
    }
}

