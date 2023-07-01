﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PulseDataAccess.Data;

#nullable disable

namespace PulseDataAccess.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20230516175638_addIMageUrlToProduct")]
    partial class addIMageUrlToProduct
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PulseModels.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "SciFi"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "History"
                        });
                });

            modelBuilder.Entity("PulseModels.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price100")
                        .HasColumnType("float");

                    b.Property<double>("Price50")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "J.R.R. Tolkien",
                            CategoryId = 1,
                            Description = "The Lord of the Rings is an epic high-fantasy novel written by English author and scholar J. R. R. Tolkien.",
                            ISBN = "9780544003415",
                            ImageUrl = "",
                            ListPrice = 20.0,
                            Price = 18.0,
                            Price100 = 12.0,
                            Price50 = 15.0,
                            Title = "The Lord of the Rings"
                        },
                        new
                        {
                            Id = 2,
                            Author = "J.R.R. Tolkien",
                            CategoryId = 2,
                            Description = "The Hobbit, or There and Back Again is a children's fantasy novel by English author J. R. R. Tolkien.",
                            ISBN = "9780544003415",
                            ImageUrl = "",
                            ListPrice = 10.0,
                            Price = 8.0,
                            Price100 = 6.0,
                            Price50 = 7.0,
                            Title = "The Hobbit"
                        },
                        new
                        {
                            Id = 3,
                            Author = "Ron Parker",
                            CategoryId = 3,
                            Description = "Preasent vitae sodales libero. Praesent mole.",
                            ISBN = "SOTJ1111111101",
                            ImageUrl = "",
                            ListPrice = 30.0,
                            Price = 27.0,
                            Price100 = 20.0,
                            Price50 = 25.0,
                            Title = "Rock in the Ocean"
                        });
                });

            modelBuilder.Entity("PulseModels.Models.Product", b =>
                {
                    b.HasOne("PulseModels.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
