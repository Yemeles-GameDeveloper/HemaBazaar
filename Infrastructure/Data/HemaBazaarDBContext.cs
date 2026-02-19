using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class HemaBazaarDBContext : IdentityDbContext<AppUser,AppRole,int>
    {

        public HemaBazaarDBContext()
        {
            
        }

        public HemaBazaarDBContext(DbContextOptions<HemaBazaarDBContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer
                ("Server=.;Database=HemaBazaarDB;Trusted_Connection=True;TrustServerCertificate=true;");
            base.OnConfiguring(optionsBuilder);
        }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // <-- en üste al (Identity için)

            //Rol oluşturmak için.
            builder.Entity<AppRole>().HasData
                (
                  new AppRole {Id = 1, Name = "Admin", NormalizedName ="ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString()},
                  new AppRole {Id = 2, Name = "UserApp", NormalizedName = "USERAPP",ConcurrencyStamp = Guid.NewGuid().ToString()}
                );

            // Konfigürasyonları uygula
            var configurationAssembly = Assembly.GetExecutingAssembly();
            builder.ApplyConfigurationsFromAssembly(configurationAssembly);

            // ---- BURAYA EKLE ----
            builder.Entity<Payment>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Prevent multiple cascade paths: AppUser->Cart->Purchase AND AppUser->Purchase
            builder.Entity<Purchase>()
                .HasOne(p => p.Cart)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.CartId)
                .OnDelete(DeleteBehavior.NoAction);
                                                    // ----------------------

            // Global query filter (IsActive)
            // Exclude Cart from global filter so we can view purchase history with inactive carts
            var entities = builder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType) 
                    && t.ClrType != typeof(Cart) 
                    && t.ClrType != typeof(Payment));

            foreach (var entity in entities)
            {
                var parameter = Expression.Parameter(entity.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseEntity.IsActive));
                var condition = Expression.Equal(prop, Expression.Constant(true));
                var lambda = Expression.Lambda(condition, parameter);

                builder.Entity(entity.ClrType).HasQueryFilter(lambda);
            }
        }



        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries<BaseEntity>();
            foreach (var e in entities)
            {
                if (e.State == EntityState.Added)
                {
                    e.Entity.IsActive = true;
                    e.Entity.CreatedDate = DateTime.Now;
                }
                if (e.State == EntityState.Modified)
                {
                    e.Entity.UpdatedDate = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

       
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet <CustomOrder> CustomOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }



    }
}
