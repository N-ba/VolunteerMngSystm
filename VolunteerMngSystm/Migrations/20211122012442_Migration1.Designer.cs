﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VolunteerMngSystm.Data;

namespace VolunteerMngSystm.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20211122012442_Migration1")]
    partial class Migration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("VolunteerMngSystm.Models.Expertise", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Expertises");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Organisations", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Industry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Organisation_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganisationsID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Requests", b =>
                {
                    b.Property<int>("VolunteeringTask_ID")
                        .HasColumnType("int");

                    b.Property<int>("Users_ID")
                        .HasColumnType("int");

                    b.Property<int?>("UsersID")
                        .HasColumnType("int");

                    b.Property<int?>("VolunteeringTaskID")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VolunteeringTask_ID", "Users_ID");

                    b.HasIndex("UsersID");

                    b.HasIndex("VolunteeringTaskID");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.SelectedExpertise", b =>
                {
                    b.Property<int>("Expertise_ID")
                        .HasColumnType("int");

                    b.Property<int>("Users_ID")
                        .HasColumnType("int");

                    b.Property<string>("Proof")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Expertise_ID", "Users_ID");

                    b.HasIndex("Users_ID");

                    b.ToTable("SelectedExpertise");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Users", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Forename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Personal_ID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone_number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Postal_Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VolunteeringTaskID")
                        .HasColumnType("int");

                    b.Property<string>("street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("VolunteeringTaskID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.VolunteeringTask", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime_of_Task")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("End_Time_of_Task")
                        .HasColumnType("time");

                    b.Property<int>("Expertise_ID")
                        .HasColumnType("int");

                    b.Property<string>("MapLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Organisation_ID")
                        .HasColumnType("int");

                    b.Property<int?>("OrganisationsID")
                        .HasColumnType("int");

                    b.Property<string>("Postal_Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("accVolNum")
                        .HasColumnType("int");

                    b.Property<int>("numOfVols")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("OrganisationsID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Requests", b =>
                {
                    b.HasOne("VolunteerMngSystm.Models.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UsersID");

                    b.HasOne("VolunteerMngSystm.Models.VolunteeringTask", "VolunteeringTask")
                        .WithMany()
                        .HasForeignKey("VolunteeringTaskID");

                    b.Navigation("Users");

                    b.Navigation("VolunteeringTask");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.SelectedExpertise", b =>
                {
                    b.HasOne("VolunteerMngSystm.Models.Expertise", "Expertise")
                        .WithMany("SelectedExpertise")
                        .HasForeignKey("Expertise_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VolunteerMngSystm.Models.Users", "User")
                        .WithMany("SelectedExperise")
                        .HasForeignKey("Users_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expertise");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Users", b =>
                {
                    b.HasOne("VolunteerMngSystm.Models.VolunteeringTask", null)
                        .WithMany("Volunteers")
                        .HasForeignKey("VolunteeringTaskID");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.VolunteeringTask", b =>
                {
                    b.HasOne("VolunteerMngSystm.Models.Organisations", "Organisations")
                        .WithMany()
                        .HasForeignKey("OrganisationsID");

                    b.Navigation("Organisations");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Expertise", b =>
                {
                    b.Navigation("SelectedExpertise");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.Users", b =>
                {
                    b.Navigation("SelectedExperise");
                });

            modelBuilder.Entity("VolunteerMngSystm.Models.VolunteeringTask", b =>
                {
                    b.Navigation("Volunteers");
                });
#pragma warning restore 612, 618
        }
    }
}
