using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using System.Threading.Tasks;


namespace Dispo.Barber.Application.Service
{
    public class InformationChatService(IUnitOfWork unitOfWork, IMapper mapper) : IinformationChatService
    {
        public async Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken,long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var company = await companyRepository.GetAsync(cancellationToken,companyId);

                    if (company == null)
                    {
                        throw new Exception($"Empresa com o ID {companyId} não encontrada.");
                    }

                    var businessIdTask = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetIdByCompanyAsync(company.Id);
                    var usersTask = await unitOfWork.GetRepository<IUserRepository>().GetUserByBusinessAsync(businessIdTask);
                    var companyServicesTask = await companyRepository.GetServicesByCompanyAsync(company.Id);

                    var listServices = await unitOfWork.GetRepository<IServiceRepository>().GetListServiceAsync(companyServicesTask);

                    var informationChat = new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = mapper.Map<List<UserInformationDTO>>(usersTask),
                        Services = mapper.Map<List<ServiceInformationDTO>>(listServices),
                        BusinessUnities = businessIdTask

                    };

                    return informationChat;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para a empresa com ID {companyId}.", ex);
            }
        }

        public async Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var userRepository = unitOfWork.GetRepository<IUserRepository>();
                    var user = await userRepository.GetAsync(cancellationToken, idUser);

                if (!user.BusinessUnityId.HasValue)
                {
                    throw new Exception($"Barbeiro com o ID {idUser} não possui unidade de negócio.");
                }
                    var businessUnity =  await unitOfWork.GetRepository<IBusinessUnityRepository>().GetAsync(cancellationToken, user.BusinessUnityId.Value);

                if (businessUnity == null)
                {
                    throw new Exception($"Barbeiro com o ID {idUser} não possui unidade de negócio.");
                }
                    var company = await unitOfWork.GetRepository<ICompanyRepository>().GetAsync(cancellationToken, businessUnity.CompanyId);

                    var services = await unitOfWork.GetRepository<IServiceUserRepository>().GetServicesByUserId(idUser);

                    var informationChat = new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = new List<UserInformationDTO> { mapper.Map<UserInformationDTO>(user) },
                        Services = mapper.Map<List<ServiceInformationDTO>>(services),
                        BusinessUnities = user.BusinessUnityId.Value
                    };

                    return informationChat;

                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para barbeiro com ID {idUser}.", ex);
            }
        }

        public async Task<InformationChatDTO> GetInformationChatByIdService(List<long> idServices)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var userList = await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var serviceRepository = unitOfWork.GetRepository<IServiceUserRepository>();
                    return await serviceRepository.GetUsersByServiceId(idServices);
                });

                var informationChat = new InformationChatDTO
                {
                    User = mapper.Map<List<UserInformationDTO>>(userList)
                };

                return informationChat;

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para barbeiro com ID {1}.", ex);
            }
        }

        private string GetDayOfWeekString(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                0 => "Dom.",
                1 => "Seg.",
                2 => "Ter.",
                3 => "Qua.",
                4 => "Qui.",
                5 => "Sex.",
                6 => "Sab.",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Dia da semana inválido")
            };
        }

        public async Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
                var userSchedules = await userScheduleRepository.GetScheduleByUserId(idUser);

                var scheduleList = userSchedules.Select(schedule => new DayScheduleDto
                {
                    DayOfWeek = GetDayOfWeekString((int)schedule.DayOfWeek) + ".", 
                    StartDate = schedule.DayOff ? null : schedule.StartDate, 
                    EndDate = schedule.DayOff ? null : schedule.EndDate, 
                    IsRest = schedule.IsRest,
                    DayOff = schedule.DayOff
                }).ToList();

                return scheduleList;
            });
        }


        async Task<List<InformationAppointmentChatDto>> IinformationChatService.GetAvailableDateTimessByUserIdAsync(CancellationToken cancellationToken, long idUser)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointments = await appointmentRepository.GetAppointmentByUserIdSync(cancellationToken, idUser);

                var appointmentDtos = appointments.Select(appointment => new InformationAppointmentChatDto
                {
                    Date = appointment.Date.ToString("yyyy-MM-dd"),  
                    Hour = appointment.Date.ToString("HH:mm"),       
                    DayOfWeek = appointment.Date.ToString("ddd", new System.Globalization.CultureInfo("pt-BR")) 
                }).ToList();

                return appointmentDtos;

            });
        }
    }
}
