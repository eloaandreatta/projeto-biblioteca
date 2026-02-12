using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pBiblioteca.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbBook> TbBooks { get; set; }

    public virtual DbSet<TbFine> TbFines { get; set; }

    public virtual DbSet<TbLoan> TbLoans { get; set; }

    public virtual DbSet<TbReservation> TbReservations { get; set; }

    public virtual DbSet<TbReservationBook> TbReservationBooks { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=3536");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("biblioteca", "loan_status", new[] { "ACTIVE", "RETURNED", "LATE", "CANCELED" })
            .HasPostgresEnum("biblioteca", "reservation_status", new[] { "WAITING", "NOTIFIED", "PICKED_UP", "CANCELED", "EXPIRED" });

        modelBuilder.Entity<TbBook>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("tb_book_pkey");

            entity.ToTable("tb_book", "biblioteca");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .HasColumnName("isbn");
            entity.Property(e => e.Author).HasColumnName("author");
            entity.Property(e => e.Availablequantity).HasColumnName("availablequantity");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Publicationyear).HasColumnName("publicationyear");
            entity.Property(e => e.Publisher).HasColumnName("publisher");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Totalquantity).HasColumnName("totalquantity");
        });

        modelBuilder.Entity<TbFine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_fine_pkey");

            entity.ToTable("tb_fine", "biblioteca");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Dailyrate)
                .HasPrecision(10, 2)
                .HasColumnName("dailyrate");
            entity.Property(e => e.Dayslate).HasColumnName("dayslate");
            entity.Property(e => e.Ispaid).HasColumnName("ispaid");
            entity.Property(e => e.LoanId)
                .ValueGeneratedOnAdd()
                .HasColumnName("loan_id");
            entity.Property(e => e.Paymentdate).HasColumnName("paymentdate");
            entity.Property(e => e.UserCpf)
                .HasMaxLength(11)
                .HasColumnName("user_cpf");

            entity.HasOne(d => d.Loan).WithMany(p => p.TbFines)
                .HasForeignKey(d => d.LoanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_fine_loan");

            entity.HasOne(d => d.UserCpfNavigation).WithMany(p => p.TbFines)
                .HasForeignKey(d => d.UserCpf)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_fine_user");
        });

        modelBuilder.Entity<TbLoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_loan_pkey");

            entity.ToTable("tb_loan", "biblioteca");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookIsbn)
                .HasMaxLength(13)
                .HasColumnName("book_isbn");
            entity.Property(e => e.Duedate).HasColumnName("duedate");
            entity.Property(e => e.Loandate).HasColumnName("loandate");
            entity.Property(e => e.Returndate).HasColumnName("returndate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserCpf)
                .HasMaxLength(11)
                .HasColumnName("user_cpf");

            entity.HasOne(d => d.BookIsbnNavigation).WithMany(p => p.TbLoans)
                .HasForeignKey(d => d.BookIsbn)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_loan_book");

            entity.HasOne(d => d.UserCpfNavigation).WithMany(p => p.TbLoans)
                .HasForeignKey(d => d.UserCpf)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_loan_user");
        });

        modelBuilder.Entity<TbReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tb_reservation_pkey");

            entity.ToTable("tb_reservation", "biblioteca");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expirationdate).HasColumnName("expirationdate");
            entity.Property(e => e.Notifieddate).HasColumnName("notifieddate");
            entity.Property(e => e.Reservationdate).HasColumnName("reservationdate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserCpf)
                .HasMaxLength(11)
                .HasColumnName("user_cpf");

            entity.HasOne(d => d.UserCpfNavigation).WithMany(p => p.TbReservations)
                .HasForeignKey(d => d.UserCpf)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_reservation_users");
        });

        modelBuilder.Entity<TbReservationBook>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_reservation_book", "biblioteca");

            entity.Property(e => e.BookIsbn)
                .HasMaxLength(13)
                .HasColumnName("book_isbn");
            entity.Property(e => e.ReservationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("reservation_id");

            entity.HasOne(d => d.BookIsbnNavigation).WithMany()
                .HasForeignKey(d => d.BookIsbn)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_reservationb_book");

            entity.HasOne(d => d.Reservation).WithMany()
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_reservationb_reservation");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.HasKey(e => e.Cpf).HasName("tb_user_pkey");

            entity.ToTable("tb_user", "biblioteca");

            entity.HasIndex(e => e.Email, "tb_user_email_key").IsUnique();

            entity.HasIndex(e => e.Telephone, "tb_user_telephone_key").IsUnique();

            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Loanlimits).HasColumnName("loanlimits");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Registrationdate)
                .HasDefaultValueSql("now()")
                .HasColumnName("registrationdate");
            entity.Property(e => e.Telephone).HasColumnName("telephone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
