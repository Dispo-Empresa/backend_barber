using System.Diagnostics;
using System.Text;
using AutoMapper;
using Dispo.Barber.API.Hubs;
using Dispo.Barber.API.Middleware;
using Dispo.Barber.Application.AppServices;
using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Application.Profiles;
using Dispo.Barber.Bundle.Services;
using Dispo.Barber.Domain.Cache;
using Dispo.Barber.Domain.Integration;
using Dispo.Barber.Domain.Providers;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Infrastructure.Cache;
using Dispo.Barber.Infrastructure.Context;
using Dispo.Barber.Infrastructure.Integration;
using Dispo.Barber.Infrastructure.Providers;
using Dispo.Barber.Infrastructure.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    }
);

builder.Services.AddSwaggerGenNewtonsoftSupport();


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

builder.Services.AddMemoryCache();

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
builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<ICacheManager, CacheManager>();
builder.Services.AddTransient<IHubIntegration, HubIntegration>();
builder.Services.AddTransient<ITwillioMessageSenderProvider, TwillioMessageSenderProvider>();

// Register services
builder.Services.AddScoped<ICompanyAppService, CompanyAppService>();
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IAppointmentAppService, AppointmentAppService>();
builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();
builder.Services.AddScoped<IServiceAppService, ServiceAppService>();
builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
builder.Services.AddScoped<IBusinessUnityAppService, BusinessUnityAppService>();
builder.Services.AddScoped<IScheduleAppService, ScheduleAppService>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
builder.Services.AddScoped<ITokenConfirmationService, TokenConfirmationService>();

// Register appServices
builder.Services.AddScoped<IInformationChatService, InformationChatService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBusinessUnityService, BusinessUnityService>();
builder.Services.AddScoped<IInformationSuggestionAppService, InformationSuggestionAppService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBlacklistService, BlacklistService>();
builder.Services.AddScoped<ITokenConfirmationAppService, TokenConfirmationAppService>();

builder.Services.AddScoped<IMigrationManager, MigrationManager>();
builder.Services.AddHttpContextAccessor();
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
    cfg.AddProfile<ScheduleProfile>();
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

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Dispo.Barber"))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.EnrichWithHttpRequest = async (activity, httpRequest) =>
                {
                    if ((httpRequest.Method == HttpMethod.Post.Method || httpRequest.Method == HttpMethod.Put.Method) && !httpRequest.HasFormContentType)
                    {
                        httpRequest.EnableBuffering();
                        using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, leaveOpen: true);
                        string requestBody = await reader.ReadToEndAsync();
                        httpRequest.Body.Position = 0;
                        activity.SetTag("http.request.body", requestBody);
                    }
                };
            })
            .AddEntityFrameworkCoreInstrumentation(o => o.SetDbStatementForText = true)
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://172.19.234.142:4317");
            });
    });

builder.Services.AddMemoryCache();

//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    {
//        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
//        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition => new FixedWindowRateLimiterOptions
//        {
//            PermitLimit = 10,
//            Window = TimeSpan.FromSeconds(1),
//            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//            QueueLimit = 0
//        });
//    });

//    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
//});

var app = builder.Build();

//app.UseRateLimiter();

app.UseMiddleware<AuthorizationMiddleware>()
   .UseMiddleware<ExceptionHandlingMiddleware>();

app.Use(async (context, next) =>
{
    var activity = Activity.Current;

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        activity?.SetTag("error.message", ex.Message);
        activity?.SetTag("error.stacktrace", ex.StackTrace);
        activity?.SetTag("error.type", ex.GetType().Name);
        throw;
    }
});

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

# if DEBUG
using (var scope = app.Services.CreateScope())
{
    var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
    migrationManager.Migrate();
}
# endif

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("BARBER_FIREBASE_ACCOUNT"))
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x.AllowAnyHeader()
      .AllowAnyMethod()
      .WithOrigins("http://localhost:7173", "http://localhost:3001", "http://localhost:3000", "http://localhost", "http://192.168.3.21:3000", "http://192.168.3.21", "https://chat.dispo-api.online", "https://aura.dispo-api.online"));

app.MapControllers();

app.MapHub<NotificationHub>("/notification");

app.UseStaticFiles();

app.Run();
