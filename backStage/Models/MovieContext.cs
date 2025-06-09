using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backStage.Models;

public partial class MovieContext : DbContext
{

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatStatus> SeatStatuses { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    public virtual DbSet<Snack> Snacks { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF2F55C8E9");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.OrderNumbers).HasDefaultValue(1);
            entity.Property(e => e.OrderPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CDF6148DF");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SeatNumber).HasMaxLength(5);
            entity.Property(e => e.SeatRow).HasMaxLength(5);
            entity.Property(e => e.TicketType).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Orders");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713D3974E8C2F");

            entity.ToTable("Seat");

            entity.HasIndex(e => new { e.TheaterNumber, e.SeatRow, e.SeatNumber }, "UQ_Seat_TheaterRowNumber").IsUnique();

            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SeatNumber).HasMaxLength(5);
            entity.Property(e => e.SeatRow).HasMaxLength(5);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<SeatStatus>(entity =>
        {
            entity.HasKey(e => e.SeatStatusId).HasName("PK__SeatStat__1E9E583389B64A0C");

            entity.ToTable("SeatStatus");

            entity.HasIndex(e => new { e.ShowTimeId, e.SeatId }, "UQ_SeatStatus_Unique").IsUnique();

            entity.Property(e => e.SeatStatusId).HasColumnName("SeatStatusID");
            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Available");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Seat).WithMany(p => p.SeatStatuses)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeatStatus_Seat");

            entity.HasOne(d => d.ShowTime).WithMany(p => p.SeatStatuses)
                .HasForeignKey(d => d.ShowTimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeatStatus_ShowTime");
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.ShowTimeId).HasName("PK__ShowTime__DF1BC9FFACAC5CEE");

            entity.ToTable("ShowTime");

            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.ScreenType).HasMaxLength(50);
            entity.Property(e => e.ShowTime1).HasColumnName("ShowTime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Snack>(entity =>
        {
            entity.HasKey(e => e.SnackId).HasName("PK__Snack__320A85EB17D61B69");

            entity.ToTable("Snack");

            entity.Property(e => e.SnackId).HasColumnName("SnackID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SnackName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
