﻿// <auto-generated />
using System;
using CandyKeeper.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CandyKeeper.DAL.Migrations
{
    [DbContext(typeof(CandyKeeperDbContext))]
    partial class CandyKeeperDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CandyKeeper.DAL.Entities.CityEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.DistrictEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Districts");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.OwnershipTypeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("OwnershipTypes");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.PackagingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Packaging");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductDeliveryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.HasIndex("SupplierId");

                    b.ToTable("ProductDeliveries");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductForSaleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PackagingId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductDeliveryId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<int>("Volume")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PackagingId");

                    b.HasIndex("ProductDeliveryId");

                    b.HasIndex("ProductId");

                    b.HasIndex("StoreId");

                    b.ToTable("ProductForSales");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductTypeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.StoreEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("NumberOfEmployees")
                        .HasColumnType("int");

                    b.Property<int>("OwnershipTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StoreNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("YearOfOpened")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DistrictId");

                    b.HasIndex("OwnershipTypeId");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.SupplierEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OwnershipTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("OwnershipTypeId");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("StoreSupplierLink", b =>
                {
                    b.Property<int>("StoresId")
                        .HasColumnType("int");

                    b.Property<int>("SuppliersId")
                        .HasColumnType("int");

                    b.HasKey("StoresId", "SuppliersId");

                    b.HasIndex("SuppliersId");

                    b.ToTable("StoreSupplierLink");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.DistrictEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.CityEntity", "City")
                        .WithMany("Districts")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductDeliveryEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.StoreEntity", "Store")
                        .WithMany("ProductDeliveries")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.SupplierEntity", "Supplier")
                        .WithMany("ProductDeliveries")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Store");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.ProductTypeEntity", "ProductType")
                        .WithMany("Products")
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductForSaleEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.PackagingEntity", "Packaging")
                        .WithMany("ProductForSales")
                        .HasForeignKey("PackagingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.ProductDeliveryEntity", "ProductDelivery")
                        .WithMany("ProductForSales")
                        .HasForeignKey("ProductDeliveryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.ProductEntity", "Product")
                        .WithMany("ProductForSales")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.StoreEntity", "Store")
                        .WithMany("ProductForSales")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Packaging");

                    b.Navigation("Product");

                    b.Navigation("ProductDelivery");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.StoreEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.DistrictEntity", "District")
                        .WithMany("Stores")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.OwnershipTypeEntity", "OwnershipType")
                        .WithMany("Stores")
                        .HasForeignKey("OwnershipTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("District");

                    b.Navigation("OwnershipType");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.SupplierEntity", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.CityEntity", "City")
                        .WithMany("Suppliers")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.OwnershipTypeEntity", "OwnershipType")
                        .WithMany("Suppliers")
                        .HasForeignKey("OwnershipTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("OwnershipType");
                });

            modelBuilder.Entity("StoreSupplierLink", b =>
                {
                    b.HasOne("CandyKeeper.DAL.Entities.StoreEntity", null)
                        .WithMany()
                        .HasForeignKey("StoresId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CandyKeeper.DAL.Entities.SupplierEntity", null)
                        .WithMany()
                        .HasForeignKey("SuppliersId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.CityEntity", b =>
                {
                    b.Navigation("Districts");

                    b.Navigation("Suppliers");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.DistrictEntity", b =>
                {
                    b.Navigation("Stores");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.OwnershipTypeEntity", b =>
                {
                    b.Navigation("Stores");

                    b.Navigation("Suppliers");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.PackagingEntity", b =>
                {
                    b.Navigation("ProductForSales");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductDeliveryEntity", b =>
                {
                    b.Navigation("ProductForSales");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductEntity", b =>
                {
                    b.Navigation("ProductForSales");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.ProductTypeEntity", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.StoreEntity", b =>
                {
                    b.Navigation("ProductDeliveries");

                    b.Navigation("ProductForSales");
                });

            modelBuilder.Entity("CandyKeeper.DAL.Entities.SupplierEntity", b =>
                {
                    b.Navigation("ProductDeliveries");
                });
#pragma warning restore 612, 618
        }
    }
}
