using AutoMapper;
using Dispo.Barber.API.Profiles;
using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Infrastructure.Context;
using Dispo.Barber.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DbContext, ApplicationContext>();
builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IBusinessUnityRepository, BusinessUnityRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();

builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<ICompanyAppService, CompanyAppService>();
builder.Services.AddTransient<IUserAppService, UserAppService>();
builder.Services.AddTransient<IAppointmentAppService, AppointmentAppService>();
builder.Services.AddTransient<IDashboardAppService, DashboardAppService>();
builder.Services.AddTransient<IServiceAppService, ServiceAppService>();
builder.Services.AddTransient<ICustomerAppService, CustomerAppService>();
builder.Services.AddTransient<IBusinessUnityAppService, BusinessUnityAppService>();
builder.Services.AddTransient<IScheduleAppService, ScheduleAppService>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<ApplicationContext>(opt => opt
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRequestTimeouts();

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<ServiceProfile>();
    cfg.AddProfile<CompanyProfile>();
    cfg.AddProfile<BusinessUnityProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<AppointmentProfile>();
    cfg.AddProfile<CustomerProfile>();
});

IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
