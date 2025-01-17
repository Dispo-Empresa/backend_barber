﻿using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface ICompanyAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO);

        Task<List<Company>> GetAllAsync(CancellationToken cancellationToken);

        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO);

        Task<Company> GetAsync(CancellationToken cancellationToken, long id);

        Task<List<UserDTO>> GetUsersAsync(CancellationToken cancellationToken, long companyId);
    }
}
