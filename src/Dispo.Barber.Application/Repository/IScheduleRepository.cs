using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IScheduleRepository : IRepositoryBase<UserSchedule>
    {
        Task<List<UserSchedule>> GetScheduleByUserId(long userId);
        Task<List<UserSchedule>> GetScheduleByUserDayOfWeek(long userId, DayOfWeek dayOfWeek);
    }
}
