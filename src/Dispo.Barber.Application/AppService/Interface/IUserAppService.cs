﻿using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IUserAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO);

        Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, AddServiceToUserDTO addServiceToUserDTO);

        Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO);

        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO);

        Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO);

        Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone);
    }
}