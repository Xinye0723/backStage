using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backStage.Models;

public partial class MovieContext : DbContext
{
    public MovieContext()
    {
    }



    public virtual DbSet<FavoriteMovie> FavoriteMovies { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberViewRecord> MemberViewRecords { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieGroup> MovieGroups { get; set; }

    public virtual DbSet<MovieRating> MovieRatings { get; set; }

    public virtual DbSet<MovieReview> MovieReviews { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatStatus> SeatStatuses { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    public virtual DbSet<Snack> Snacks { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=movie;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteMovie>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__876A67357C7DED0D");

            entity.Property(e => e.FavoriteId).HasColumnName("favoriteID");
            entity.Property(e => e.FavoritedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("favoritedAt");
            entity.Property(e => e.MemberId).HasColumnName("memberID");
            entity.Property(e => e.MovieId).HasColumnName("movieID");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("member");

            entity.Property(e => e.MemberId)
                .ValueGeneratedNever()
                .HasColumnName("memberID");
            entity.Property(e => e.MemberBirthDate)
                .HasColumnType("datetime")
                .HasColumnName("memberBirthDate");
            entity.Property(e => e.MemberEmail)
                .HasMaxLength(50)
                .HasColumnName("memberEmail");
            entity.Property(e => e.MemberGender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("memberGender");
            entity.Property(e => e.MemberImg)
                .HasMaxLength(255)
                .HasColumnName("memberImg");
            entity.Property(e => e.MemberIntroSelf)
                .HasMaxLength(100)
                .HasColumnName("memberIntroSelf");
            entity.Property(e => e.MemberName)
                .HasMaxLength(10)
                .HasColumnName("memberName");
            entity.Property(e => e.MemberPassword)
                .HasMaxLength(20)
                .HasColumnName("memberPassword");
            entity.Property(e => e.MemberPermission)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("memberPermission");
        });

        modelBuilder.Entity<MemberViewRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.ToTable("memberViewRecord");

            entity.Property(e => e.RecordId)
                .ValueGeneratedNever()
                .HasColumnName("recordID");
            entity.Property(e => e.MemberId)
                .HasMaxLength(10)
                .HasColumnName("memberID");
            entity.Property(e => e.MovieId).HasColumnName("movieID");
            entity.Property(e => e.MovieNameChinese)
                .HasMaxLength(100)
                .HasColumnName("movieName_Chinese");
            entity.Property(e => e.MovieNameEnglish)
                .HasMaxLength(100)
                .HasColumnName("movieName_English");
            entity.Property(e => e.WatchDate)
                .HasColumnType("datetime")
                .HasColumnName("watchDate");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__42EB372EE86C4B0C");

            entity.Property(e => e.MovieId).HasColumnName("movieID");
            entity.Property(e => e.BoxOffice).HasColumnName("boxOffice");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Director)
                .HasMaxLength(100)
                .HasColumnName("director");
            entity.Property(e => e.Distributor)
                .HasMaxLength(100)
                .HasColumnName("distributor");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.IsEnded).HasColumnName("isEnded");
            entity.Property(e => e.IsNowShowing).HasColumnName("isNowShowing");
            entity.Property(e => e.IsReleased).HasColumnName("isReleased");
            entity.Property(e => e.IsUpcoming).HasColumnName("isUpcoming");
            entity.Property(e => e.MovieNameChinese)
                .HasMaxLength(100)
                .HasColumnName("movieName_Chinese");
            entity.Property(e => e.MovieNameEnglish)
                .HasMaxLength(100)
                .HasColumnName("movieName_English");
            entity.Property(e => e.MovieRatingId).HasColumnName("movieRatingID");
            entity.Property(e => e.Plot).HasColumnName("plot");
            entity.Property(e => e.PosterPicture)
                .HasMaxLength(200)
                .HasColumnName("posterPicture");
            entity.Property(e => e.Production)
                .HasMaxLength(100)
                .HasColumnName("production");
            entity.Property(e => e.ReleaseDate).HasColumnName("releaseDate");
            entity.Property(e => e.Starring)
                .HasMaxLength(100)
                .HasColumnName("starring");
            entity.Property(e => e.TrailerUrl)
                .HasMaxLength(200)
                .HasColumnName("trailerUrl");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("viewCount");

            entity.HasMany(d => d.Tags).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieTags_Tags"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieTags_Movies"),
                    j =>
                    {
                        j.HasKey("MovieId", "TagId").HasName("PK__MovieTag__27E4F7396F05AA9D");
                        j.ToTable("MovieTags");
                        j.IndexerProperty<int>("MovieId").HasColumnName("movieID");
                        j.IndexerProperty<int>("TagId").HasColumnName("tagID");
                    });
        });

        modelBuilder.Entity<MovieGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__movieGro__88C102ADE1513D49");

            entity.ToTable("movieGroup");

            entity.Property(e => e.GroupId)
                .ValueGeneratedNever()
                .HasColumnName("groupID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createTime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(100)
                .HasColumnName("groupName");
            entity.Property(e => e.GroupNote)
                .HasMaxLength(255)
                .HasColumnName("groupNote");
            entity.Property(e => e.LeaderMemberId).HasColumnName("leaderMemberID");
            entity.Property(e => e.MaxMembers).HasColumnName("maxMembers");
            entity.Property(e => e.MovieId).HasColumnName("movieID");
            entity.Property(e => e.ShowTimeId).HasColumnName("showTimeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<MovieRating>(entity =>
        {
            entity.HasKey(e => e.MovieRatingId).HasName("PK__MovieRat__785C5BB6D12A7F8D");

            entity.HasIndex(e => e.RatingCode, "UQ__MovieRat__602ABCCAB50B617C").IsUnique();

            entity.Property(e => e.MovieRatingId).HasColumnName("movieRatingID");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.RatingCode)
                .HasMaxLength(10)
                .HasColumnName("ratingCode");
        });

        modelBuilder.Entity<MovieReview>(entity =>
        {
            entity.HasKey(e => e.MovieReviewId).HasName("PK__MovieRev__7CAE733346EF48CA");

            entity.Property(e => e.MovieReviewId).HasColumnName("movieReviewID");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(true)
                .HasColumnName("isPublic");
            entity.Property(e => e.MemberId).HasColumnName("memberID");
            entity.Property(e => e.MovieId).HasColumnName("movieID");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("reviewedAt");
        });

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

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__registra__A3DB1415F64220DE");

            entity.ToTable("registration");

            entity.Property(e => e.RegistrationId)
                .ValueGeneratedNever()
                .HasColumnName("registrationID");
            entity.Property(e => e.GroupId).HasColumnName("groupID");
            entity.Property(e => e.MemberId).HasColumnName("memberID");
            entity.Property(e => e.Members).HasColumnName("members");
            entity.Property(e => e.RegistrationDate)
                .HasColumnType("datetime")
                .HasColumnName("registrationDate");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Group).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__registrat__group__03F0984C");
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
            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
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

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("staffID");
            entity.Property(e => e.StaffEmail)
                .HasMaxLength(50)
                .HasColumnName("staffEmail");
            entity.Property(e => e.StaffName)
                .HasMaxLength(10)
                .HasColumnName("staffName");
            entity.Property(e => e.StaffPassword)
                .HasMaxLength(20)
                .HasColumnName("staffPassword");
            entity.Property(e => e.StaffPermission)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("staffPermission");
            entity.Property(e => e.StaffPhone).HasColumnName("staffPhone");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__50FC01770A9A1EB3");

            entity.HasIndex(e => e.TagName, "UQ__Tags__288C385129599960").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tagID");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("tagName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
