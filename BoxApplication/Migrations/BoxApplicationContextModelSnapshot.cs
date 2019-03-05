﻿// <auto-generated />
using System;
using BoxApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BoxApplication.Migrations
{
    [DbContext(typeof(BoxApplicationContext))]
    partial class BoxApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BoxApplication.Models.ActiveDirectoryUser", b =>
                {
                    b.Property<string>("ADEmail")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ADDateInactive");

                    b.Property<string>("ADFirstName");

                    b.Property<string>("ADStatus");

                    b.Property<string>("ADUsername");

                    b.HasKey("ADEmail");

                    b.ToTable("ActiveDirectoryUsers");
                });

            modelBuilder.Entity("BoxApplication.Models.ApplicationAction", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Type");

                    b.Property<string>("User");

                    b.HasKey("ID");

                    b.ToTable("ApplicationActions");
                });

            modelBuilder.Entity("BoxApplication.Models.BoxUsers", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Login");

                    b.Property<string>("Name");

                    b.Property<long>("SpaceUsed");

                    b.HasKey("ID");

                    b.ToTable("BoxUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
