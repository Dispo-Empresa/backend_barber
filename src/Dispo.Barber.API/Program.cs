using System.Globalization;
using System;
using System.Text;
using AutoMapper;
using Dispo.Barber.API;
using Dispo.Barber.API.Hubs;
using Dispo.Barber.API.Profiles;
using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Bundle.Entities;
using Dispo.Barber.Bundle.Services;
using Dispo.Barber.Infrastructure.Context;
using Dispo.Barber.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

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
builder.Services.AddTransient<IServiceAppointmentRepository, ServiceAppointmentRepository>();

// Register services
builder.Services.AddScoped<ICompanyAppService, CompanyAppService>();
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IAppointmentAppService, AppointmentAppService>();
builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();
builder.Services.AddScoped<IServiceAppService, ServiceAppService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IinformationChatService, InformationChatService>();
builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
builder.Services.AddScoped<IBusinessUnityAppService, BusinessUnityAppService>();
builder.Services.AddScoped<IScheduleAppService, ScheduleAppService>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBusinessUnityService, BusinessUnityService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMigrationManager, MigrationManager>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Database connection
builder.Services.AddDbContext<ApplicationContext>(opt => opt
                            .UseNpgsql(Environment.GetEnvironmentVariable("BARBER_CONNECTION_STRING")));



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

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
    migrationManager.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x.AllowAnyHeader()
      .AllowAnyMethod()
      .WithOrigins("http://localhost:7173", "http://localhost:3001", "http://localhost:3000", "http://localhost", "http://192.168.3.21:3000", "http://192.168.3.21"));

app.MapControllers();

app.MapHub<NotificationHub>("/notification");

app.Run();
