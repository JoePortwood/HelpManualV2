using Microsoft.EntityFrameworkCore;
using HelpManual.Entities;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace HelpManual.Entities
{
    public class HelpManualDbContext : DbContext
    {
        public HelpManualDbContext()
        {

        }
        public HelpManualDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserAccess> UserAccess { get; set; }

        public DbSet<ControlType> ControlTypes { get; set; }

        public DbSet<ObjectType> ObjectTypes { get; set; }

        public DbSet<FormObject> FormObject { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ControlType>()
                //.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false)
                .ToTable("ControlType");
            //.Property<bool>("IsDeleted");
            modelBuilder.Entity<ObjectType>()
                //.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false)
                .ToTable("ObjectType");
            //.Property<bool>("IsDeleted");
            modelBuilder.Entity<FormObject>()
                //.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false)
                .ToTable("FormObject");
            //.Property<bool>("IsDeleted");

            modelBuilder.Entity<UserAccess>()
                .ToTable("UserAccess");

            //Disables cascading deletes e.g. when deleting a object type deleting the form object as well.
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //This section is required for calling the stored procedure to retrieve the user Id from Active Directory
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Environment.CurrentDirectory)//AppDomain.CurrentDomain.BaseDirectory)
             .AddJsonFile("appsettings.json")
             .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("HelpManualConnection"));
        }

        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
        //    OnBeforeSaving();
        //    return base.SaveChanges(acceptAllChangesOnSuccess);
        //}

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    OnBeforeSaving();
        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        //private void OnBeforeSaving()
        //{
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                entry.CurrentValues["IsDeleted"] = false;
        //                break;

        //            case EntityState.Deleted:
        //                entry.State = EntityState.Modified;
        //                entry.CurrentValues["IsDeleted"] = true;
        //                break;
        //        }
        //    }
        //}
    }
}
