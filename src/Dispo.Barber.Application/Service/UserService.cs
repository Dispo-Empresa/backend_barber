﻿using Dispo.Barber.Application.Repository;
using System.Data;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Extension;
using Dispo.Barber.Domain.Exception;
using AutoMapper;

namespace Dispo.Barber.Application.Service
{
    public class UserService(IMapper mapper, IUserRepository repository) : IUserService
    {
        public async Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> services)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new VersionNotFoundException();
            }

            user.ServicesUser.AddRange(services.Select(s => new ServiceUser
            {
                UserId = user.Id,
                ServiceId = s
            }).ToList());

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            user.Password = PasswordEncryptor.HashPassword(changePasswordDTO.Password);

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            user.Status = changeStatusDTO.Status;

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO)
        {
            var user = mapper.Map<User>(createUserDTO);
            user.Password = "";
            user.Schedules.AddRange(BuildNormalDays());
            if (user.IsPending())
            {
                // TODO: Completar.
            }

            await repository.AddAsync(cancellationToken, user);
            await repository.SaveChangesAsync(cancellationToken);

            if (createUserDTO.Services.Any())
                await AddServiceToUserAsync(cancellationToken, user.Id, createUserDTO.Services);
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await repository.GetAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);
        }

        public async Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone)
        {
            return await repository.GetIdByPhone(cancellationToken, phone);
        }

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetValidDaysSchedulesAsync(cancellationToken, id);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO)
        {
            var user = await repository.GetAsync(cancellationToken, id);
            if (user is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(updateUserDTO.Name))
            {
                user.Name = updateUserDTO.Name;
            }

            if (!string.IsNullOrEmpty(updateUserDTO.Surname))
            {
                user.Surname = updateUserDTO.Surname;
            }

            if (!string.IsNullOrEmpty(updateUserDTO.Phone))
            {
                user.Phone = updateUserDTO.Phone;
            }

            user.Password = PasswordEncryptor.HashPassword(updateUserDTO.Password);

            repository.Update(user);
            await repository.SaveChangesAsync(cancellationToken);
        }

        private List<UserSchedule> BuildNormalDays() => [
            new(DayOfWeek.Monday, "08:00", "18:00", false, false),
            new(DayOfWeek.Monday, "12:00", "13:30", true, false),
            new(DayOfWeek.Tuesday, "08:00", "18:00", false, false),
            new(DayOfWeek.Tuesday, "12:00", "13:30", true, false),
            new(DayOfWeek.Wednesday, "08:00", "18:00", false, false),
            new(DayOfWeek.Wednesday, "12:00", "13:30", true, false),
            new(DayOfWeek.Thursday, "08:00", "18:00", false, false),
            new(DayOfWeek.Thursday, "12:00", "13:30", true, false),
            new(DayOfWeek.Friday, "08:00", "18:00", false, false),
            new(DayOfWeek.Friday, "12:00", "13:30", true, false),
            new(DayOfWeek.Saturday, "08:00", "18:00", false, false),
            new(DayOfWeek.Saturday, "12:00", "13:30", true, false),
            new(DayOfWeek.Sunday, "08:00", "18:00", false, false),
            new(DayOfWeek.Sunday, "12:00", "13:30", true, false),
        ];
    }
}