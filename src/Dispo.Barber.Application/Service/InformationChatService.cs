using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;


namespace Dispo.Barber.Application.Service
{
    public class InformationChatService(IUnitOfWork unitOfWork, IMapper mapper) : IinformationChatService
    {
        public async Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken,long companyId)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var company = await companyRepository.GetAsync(cancellationToken,companyId);

                    if (company == null)
                    {
                        throw new Exception($"Empresa com o ID {companyId} não encontrada.");
                    }

                    var businessIdTask = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetIdByCompanyAsync(company.Id);
                    var usersTask = await unitOfWork.GetRepository<IUserRepository>().GetUserByBusinessAsync(businessIdTask);
                    var companyServicesTask = await unitOfWork.GetRepository<ICompanyRepository>().GetServicesByCompanyAsync(company.Id);

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
                var cancellationTokenSource = new CancellationTokenSource();

                return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
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
    }
}
