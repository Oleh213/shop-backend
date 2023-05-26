using System;
using System.Collections.Generic;
using WebShop.Main.Conext;
using Microsoft.EntityFrameworkCore;
using WebShop.Main.Context;
using sushi_backend.Context;

namespace WebShop.Main.DBContext
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
        }
        public DbSet<User> users { get; set; }

        public DbSet<Category> categories { get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<Order> orders { get; set; }

        public DbSet<Promocode> promocodes { get; set;}

        public DbSet<OrderList> orderLists { get; set; }

        public DbSet<Logger> loggers { get; set; }

        public DbSet<DeliveryOptions> deliveryOptions { get; set; }

        public DbSet<ImagesSlider> imagesSliders { get; set; }

        public DbSet<ProductOption> productOptions { get; set; }

        public DbSet<MagazineSettings> magazineSettings { get; set; }

        public DbSet<TimeLine> timeLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Category>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .IsRequired()
                .HasForeignKey(x => x.CategoryId);          

            modelBuilder.Entity<OrderList>()
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderLists)
                .IsRequired()
                .HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<OrderList>()
                .HasOne(x => x.Product)
                .WithMany()
                .IsRequired()
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(x => x.DeliveryOptions)
                .WithOne(x => x.Order)
                .HasForeignKey<DeliveryOptions>(x => x.OrderId);

            modelBuilder.Entity<ProductOption>()
                .HasMany(x => x.Products)
                .WithOne(x => x.ProductOption)
                .HasForeignKey(x => x.ProductOptionsId);
            
            modelBuilder.Entity<User>().HasKey(s => new { s.UserId });

            modelBuilder.Entity<Order>().HasKey(s => new { s.OrderId });

            modelBuilder.Entity<OrderList>().HasKey(s => new { s.OrderListId });

            modelBuilder.Entity<Product>().HasKey(s => new { s.ProductId });

            modelBuilder.Entity<Category>().HasKey(s => new { s.CategoryId });

            modelBuilder.Entity<Logger>().HasKey(s => new { s.LoggerId });

            modelBuilder.Entity<DeliveryOptions>().HasKey(s => new { s.DeliveryOptionsId });

            modelBuilder.Entity<ImagesSlider>().HasKey(s => new { s.ImagesSliderId });

            modelBuilder.Entity<ProductOption>().HasKey(s => new { s.ProductOptionsId });

            modelBuilder.Entity<MagazineSettings>().HasKey(s => new { s.MagazineSettingsId });

            modelBuilder.Entity<TimeLine>().HasKey(s => new { s.TimeLineId });
        }
    }
}