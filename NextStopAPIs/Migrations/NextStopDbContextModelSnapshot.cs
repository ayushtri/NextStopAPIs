﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NextStopAPIs.Data;

#nullable disable

namespace NextStopAPIs.Migrations
{
    [DbContext(typeof(NextStopDbContext))]
    partial class NextStopDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NextStopAPIs.Models.AdminAction", b =>
                {
                    b.Property<int>("ActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActionId"));

                    b.Property<DateTime>("ActionTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ActionId");

                    b.HasIndex("AdminId");

                    b.ToTable("AdminActions");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("TotalFare")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("BookingId");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Bus", b =>
                {
                    b.Property<int>("BusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BusId"));

                    b.Property<string>("Amenities")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("BusName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("BusNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("BusType")
                        .HasColumnType("int");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<int>("TotalSeats")
                        .HasColumnType("int");

                    b.HasKey("BusId");

                    b.HasIndex("BusNumber")
                        .IsUnique();

                    b.HasIndex("OperatorId");

                    b.ToTable("Buses");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<string>("FeedbackText")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("FeedbackId");

                    b.HasIndex("BookingId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("NotificationType")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("PaymentId");

                    b.HasIndex("BookingId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("NextStopAPIs.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Route", b =>
                {
                    b.Property<int>("RouteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RouteId"));

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Distance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("EstimatedTime")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RouteId");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleId"));

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("BusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Fare")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("RouteId")
                        .HasColumnType("int");

                    b.HasKey("ScheduleId");

                    b.HasIndex("BusId");

                    b.HasIndex("RouteId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("NextStopAPIs.Models.ScheduleSeats", b =>
                {
                    b.Property<int>("ScheduleSeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleSeatId"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<int>("SeatId")
                        .HasColumnType("int");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("ScheduleSeatId");

                    b.HasIndex("BookingId");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("SeatId");

                    b.ToTable("ScheduleSeats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Seat", b =>
                {
                    b.Property<int>("SeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SeatId"));

                    b.Property<int>("BusId")
                        .HasColumnType("int");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("SeatId");

                    b.HasIndex("BusId");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.SeatLog", b =>
                {
                    b.Property<int>("SeatLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SeatLogId"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("BusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateBooked")
                        .HasColumnType("datetime2");

                    b.Property<string>("Seats")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SeatLogId");

                    b.ToTable("SeatLogs");
                });

            modelBuilder.Entity("NextStopAPIs.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NextStopAPIs.Models.AdminAction", b =>
                {
                    b.HasOne("NextStopAPIs.Models.User", "Admin")
                        .WithMany("AdminActions")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Booking", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Schedule", "Schedule")
                        .WithMany("Bookings")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NextStopAPIs.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Schedule");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Bus", b =>
                {
                    b.HasOne("NextStopAPIs.Models.User", "Operator")
                        .WithMany("Buses")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Feedback", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Notification", b =>
                {
                    b.HasOne("NextStopAPIs.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Payment", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Schedule", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Bus", "Bus")
                        .WithMany("Schedules")
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NextStopAPIs.Models.Route", "Route")
                        .WithMany("Schedules")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("Route");
                });

            modelBuilder.Entity("NextStopAPIs.Models.ScheduleSeats", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Booking", "Booking")
                        .WithMany("ScheduleSeats")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NextStopAPIs.Models.Schedule", "Schedule")
                        .WithMany("ScheduleSeats")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NextStopAPIs.Models.Seat", "Seat")
                        .WithMany("ScheduleSeats")
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Schedule");

                    b.Navigation("Seat");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Seat", b =>
                {
                    b.HasOne("NextStopAPIs.Models.Bus", "Bus")
                        .WithMany("Seats")
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Booking", b =>
                {
                    b.Navigation("ScheduleSeats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Bus", b =>
                {
                    b.Navigation("Schedules");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Route", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Schedule", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("ScheduleSeats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.Seat", b =>
                {
                    b.Navigation("ScheduleSeats");
                });

            modelBuilder.Entity("NextStopAPIs.Models.User", b =>
                {
                    b.Navigation("AdminActions");

                    b.Navigation("Bookings");

                    b.Navigation("Buses");

                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
