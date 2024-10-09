﻿// <auto-generated />
using System;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dispo.Barber.Persistence.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240917223551_DurationColumn")]
    partial class DurationColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Appointment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AcceptedUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("AcceptedUserObservation")
                        .HasColumnType("text");

                    b.Property<DateTime>("AccomplishedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("BusinessUnityId")
                        .HasColumnType("bigint");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

                    b.Property<string>("CustomerObservation")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ServiceId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AcceptedUserId");

                    b.HasIndex("BusinessUnityId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.BusinessUnity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint");

                    b.Property<string>("Complement")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("BusinessUnities");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Company", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Logo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Service", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.ServiceCompany", b =>
                {
                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint");

                    b.Property<long>("ServiceId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.HasKey("CompanyId", "ServiceId");

                    b.HasIndex("ServiceId");

                    b.ToTable("CompanyServices");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<long?>("BusinessUnityId")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BusinessUnityId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.UserService", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("ServiceId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "ServiceId");

                    b.HasIndex("ServiceId");

                    b.ToTable("UserServices");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Appointment", b =>
                {
                    b.HasOne("Dispo.Barber.Domain.Entities.User", "AcceptedUser")
                        .WithMany()
                        .HasForeignKey("AcceptedUserId");

                    b.HasOne("Dispo.Barber.Domain.Entities.BusinessUnity", "BusinessUnity")
                        .WithMany()
                        .HasForeignKey("BusinessUnityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dispo.Barber.Domain.Entities.User", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dispo.Barber.Domain.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AcceptedUser");

                    b.Navigation("BusinessUnity");

                    b.Navigation("Customer");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.BusinessUnity", b =>
                {
                    b.HasOne("Dispo.Barber.Domain.Entities.Company", "Company")
                        .WithMany("BusinessUnities")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.ServiceCompany", b =>
                {
                    b.HasOne("Dispo.Barber.Domain.Entities.Company", "Company")
                        .WithMany("CompanyServices")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dispo.Barber.Domain.Entities.Service", "Service")
                        .WithMany("CompanyServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.User", b =>
                {
                    b.HasOne("Dispo.Barber.Domain.Entities.BusinessUnity", "BusinessUnity")
                        .WithMany()
                        .HasForeignKey("BusinessUnityId");

                    b.Navigation("BusinessUnity");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.UserService", b =>
                {
                    b.HasOne("Dispo.Barber.Domain.Entities.Service", "Service")
                        .WithMany("UserServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dispo.Barber.Domain.Entities.User", "User")
                        .WithMany("UserServices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Company", b =>
                {
                    b.Navigation("BusinessUnities");

                    b.Navigation("CompanyServices");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.Service", b =>
                {
                    b.Navigation("CompanyServices");

                    b.Navigation("UserServices");
                });

            modelBuilder.Entity("Dispo.Barber.Domain.Entities.User", b =>
                {
                    b.Navigation("UserServices");
                });
#pragma warning restore 612, 618
        }
    }
}