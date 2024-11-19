using Dispo.Barber.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceUser>().HasKey(sc => new { sc.UserId, sc.ServiceId });
            modelBuilder.Entity<ServiceCompany>().HasKey(sc => new { sc.CompanyId, sc.ServiceId });
            modelBuilder.Entity<ServiceAppointment>().HasKey(sc => new { sc.AppointmentId, sc.ServiceId });
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<BusinessUnity> BusinessUnities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceUser> UserServices { get; set; }
        public DbSet<ServiceCompany> CompanyServices { get; set; }
        public DbSet<UserSchedule> UserSchedules { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceAppointment> AppointmentServices { get; set; }
    }
}
