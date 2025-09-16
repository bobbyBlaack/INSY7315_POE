using NewDawnProperties.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace NewDawnProperties.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet <CaretakerAssignmentModel> CaretakerAssignment {  get; set; }

        public DbSet<PropertyModel> Property { get; set; }

        public DbSet<RoomModel> Rooms { get; set; }

        public DbSet<TenantAssignmentModel> TenantAssignment { get; set; }

        public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
