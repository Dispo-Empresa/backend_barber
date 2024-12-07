using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly ApplicationContext context;
        public UserRepository(ApplicationContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await context.Appointments
                                .Include("Services.Service")
                                .Include("Customer")
                                .Where(w => w.AcceptedUserId == id && w.Date >= getUserAppointmentsDTO.StartDate && w.Date <= getUserAppointmentsDTO.EndDate)
                                .OrderBy(x => x.Date)
                                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetUserByBusinessAsync(long businessId)
        {
            return await context.Users.Include(i => i.BusinessUnity)
                                .Where(w => w.BusinessUnityId == businessId)
                                .ToListAsync();
        }

        public async Task<List<UserSchedule>> GetValidDaysSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await context.UserSchedules.Where(x => x.UserId == id && !x.IsRest)
                                              .ToListAsync();
        }

        public async Task<User> GetWithAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Users.Include("Appointments.Services.Service")
                                      .Include(i => i.Schedules)
                                      .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<long> GetIdByPhone(CancellationToken cancellationToken, string phone)
        {
            return await context.Users.Where(x => x.Phone == phone)
                                      .Select(s => s.Id)
                                      .FirstOrDefaultAsync();
        }

        public async Task<User> GetByPhoneWithBusinessUnitiesAsync(CancellationToken cancellationToken, string phone)
        {
            return await context.Users.Include("BusinessUnity.Company")
                                      .FirstOrDefaultAsync(w => w.Phone == phone);
        }

        public async Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug)
        {
            return await context.Users.Include("BusinessUnity.Company")
                                      .FirstOrDefaultAsync(w => w.Slug == userSlug && w.BusinessUnity != null && w.BusinessUnity.Company != null && w.BusinessUnity.Company.Slug == companySlug);
        }

        public async Task<List<UserDTO>> GetByCompanyId(CancellationToken cancellationToken, long companyId)
        {
            return await context.Users.Include("BusinessUnity")
                                      .Where(w => w.BusinessUnity != null && w.BusinessUnity.CompanyId == companyId)
                                      .Select(s => new UserDTO
                                      {
                                          Id = s.Id,
                                          Name = s.Name,
                                          Phone = s.Phone,
                                          Picture = string.Empty,
                                          Status = s.Status
                                      })
                                      .ToListAsync(cancellationToken);
        }

        public async Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Users.Include(i => i.BusinessUnity)
                                      .Include(i => i.Appointments).ThenInclude(i => i.Services)
                                      .Where(w => w.Id == id)
                                      .Select(s => new UserDetailDTO
                                      {
                                          Id = s.Id,
                                          Name = s.Name,
                                          Phone = s.Phone,
                                          Picture = string.Empty,
                                          Status = s.Status,
                                          Link = s.Slug,
                                          Schedules = s.Schedules.Where(w => !w.IsRest).Select(s => new ScheduleDTO
                                          {
                                              StartDate = s.StartDate,
                                              EndDate = s.EndDate,
                                              DayOfWeek = s.DayOfWeek,
                                              DayOff = s.DayOff
                                          }).ToList(),
                                          Services = s.Appointments.SelectMany(s => s.Services).Select(s => s.Service).Select(s => new ServiceDetailDTO
                                          {
                                              Id = s.Id,
                                              Description = s.Description,
                                          }).ToList()
                                      })
                                      .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id)
        {
            return await context.UserServices
                            .Where(w => w.UserId == id)
                            .Select(s => new ServiceInformationDTO
                            {
                                Id = s.Service.Id,
                                Description = s.Service.Description,
                                Duration = s.Service.Duration,
                                Price = s.Service.Price
                            }).ToListAsync();
        }
    }
}
