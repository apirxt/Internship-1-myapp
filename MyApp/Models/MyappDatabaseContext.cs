using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

public partial class MyappDatabaseContext : DbContext
{
    public MyappDatabaseContext()
    {
    }

    public MyappDatabaseContext(DbContextOptions<MyappDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemClient> ItemClients { get; set; }

    public virtual DbSet<SerialNumber> SerialNumbers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-UDRLDTQ; Initial Catalog=myapp_database; Integrated Security=True; Pooling=False; Encrypt=False; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CategoryId");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Items_1");

            entity.HasOne(d => d.Category).WithMany(p => p.Items).HasConstraintName("FK_Items_CategoryId");
        });

        modelBuilder.Entity<ItemClient>(entity =>
        {
            entity.HasOne(d => d.Client).WithMany(p => p.ItemClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItemClients_Clients1");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItemClients_Items");
        });

        modelBuilder.Entity<SerialNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SerialNumbers_1");

            entity.HasOne(d => d.Item).WithMany(p => p.SerialNumbers).HasConstraintName("FK_SerialNumbers_SerialNumbers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
