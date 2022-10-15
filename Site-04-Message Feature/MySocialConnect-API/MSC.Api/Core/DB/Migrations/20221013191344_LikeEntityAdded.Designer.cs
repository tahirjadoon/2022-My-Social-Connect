﻿// <auto-generated />
using System;
using MSC.Api.Core.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MSC.Api.Core.DB.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221013191344_LikeEntityAdded")]
    partial class LikeEntityAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("MSC.Api.Core.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Interests")
                        .HasColumnType("TEXT");

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("TEXT");

                    b.Property<string>("LookingFor")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuId");

                    b.HasIndex("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MSC.Api.Core.Entities.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AppUserId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMain")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PublicId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("MSC.Api.Core.Entities.UserLike", b =>
                {
                    b.Property<int>("SourceUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LikedUserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SourceUserId", "LikedUserId");

                    b.HasIndex("LikedUserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("MSC.Api.Core.Entities.Photo", b =>
                {
                    b.HasOne("MSC.Api.Core.Entities.AppUser", "AppUser")
                        .WithMany("Photos")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("MSC.Api.Core.Entities.UserLike", b =>
                {
                    b.HasOne("MSC.Api.Core.Entities.AppUser", "LikedUser")
                        .WithMany("UsersLikedMe")
                        .HasForeignKey("LikedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MSC.Api.Core.Entities.AppUser", "SourceUser")
                        .WithMany("UsersILiked")
                        .HasForeignKey("SourceUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LikedUser");

                    b.Navigation("SourceUser");
                });

            modelBuilder.Entity("MSC.Api.Core.Entities.AppUser", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("UsersILiked");

                    b.Navigation("UsersLikedMe");
                });
#pragma warning restore 612, 618
        }
    }
}
