using System.Text;
using AutoMapper;
using Dispo.Barber.API;
using Dispo.Barber.API.Profiles;
using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Bundle.Entities;
using Dispo.Barber.Bundle.Services;
using Dispo.Barber.Infrastructure.Context;
using Dispo.Barber.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Application.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        }
    );
    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

// Twilio configuration using IOptions<T>
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddTransient<ISmsService, SmsService>(provider =>
{
    var twilioSettings = provider.GetRequiredService<IOptions<TwilioSettings>>().Value;
    return new SmsService(twilioSettings.AccountSid, twilioSettings.AuthToken, twilioSettings.PhoneNumber, twilioSettings.PhoneNumberWhats);
});

// Register repositories
builder.Services.AddTransient<DbContext, ApplicationContext>();
builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IBusinessUnityRepository, BusinessUnityRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IServiceUserRepository, ServiceUserRepository>();

// Register services
builder.Services.AddTransient<ICompanyAppService, CompanyAppService>();
builder.Services.AddTransient<IUserAppService, UserAppService>();
builder.Services.AddTransient<IAppointmentAppService, AppointmentAppService>();
builder.Services.AddTransient<IDashboardAppService, DashboardAppService>();
builder.Services.AddTransient<IServiceAppService, ServiceAppService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IinformationChatService, InformationChatService>();
builder.Services.AddTransient<ICustomerAppService, CustomerAppService>();
builder.Services.AddTransient<IBusinessUnityAppService, BusinessUnityAppService>();
builder.Services.AddTransient<IScheduleAppService, ScheduleAppService>();
builder.Services.AddTransient<IAuthAppService, AuthAppService>();

builder.Services.AddTransient<IMigrationManager, MigrationManager>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// Database connection
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


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
         ValidAudience = Environment.GetEnvironmentVariable("JWT_ISSUER"),
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")))
     };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using (var scope = app.Services.CreateScope())
//{
//    var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
//    migrationManager.Migrate();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x.AllowAnyHeader()
      .AllowAnyMethod()
      .WithOrigins("http://localhost:7173", "http://localhost:3001", "http://localhost:3000", "http://localhost", "http://192.168.3.21:3000", "http://192.168.3.21"));

app.MapControllers();

app.Run();
