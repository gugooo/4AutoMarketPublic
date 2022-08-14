using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace _4AutoMarket.Models
{
    public class UserDBContext : IdentityDbContext<User>
    {
        public virtual DbSet<Catalog> Catalogs { get; set; }
        public virtual DbSet<Atribute> Atributes { get; set; }
        public virtual DbSet<ProductIMG> ProductIMGs { get; set; }
        public virtual DbSet<AtributeValue> AtributeValues { get; set; }
        public virtual DbSet<ProductAtributeValue> ProductAtributeValues { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<CultureData> CultureDatas { get; set; }
        public virtual DbSet<ProductAtributes> ProductAtributes { get; set; }
        public virtual DbSet<HomePageSection> HomePageSections { get; set; }
        public virtual DbSet<HomePageSectionData> HomePageSectionDatas { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }
        public virtual DbSet<GlobalSetings> GlobalSetings { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<LiveChatSession> LiveChatSessions { get; set; }
        public virtual DbSet<SerchHistory> SerchHistories { get; set; }
        public virtual DbSet<SubscribeEmail> SubscribeEmails { get; set; }
        public virtual DbSet<MailingHistory> MailingHistories { get; set; }
        public virtual DbSet<RequestCell> RequestCells { get; set; }
        public virtual DbSet<RequestEmail> RequestEmails { get; set; }
        public virtual DbSet<IdramPaymentHistory> IdramPaymentHistories { get; set; }
        public virtual DbSet<SerchKeys> SerchKeys { get; set; }
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AtributeValue>().HasOne(_ => _.FK_Atribute).WithMany(_ => _.Values).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CultureData>().HasOne(_ => _.FK_Title).WithMany(_ => _.CultureTitle).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CultureData>().HasOne(_ => _.FK_Description).WithMany(_ => _.CultureDescription).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CultureData>().HasOne(_ => _.FK_Brand).WithMany(_ => _.CultureBrand).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CultureData>().HasOne(_ => _.FK_HP_Title).WithMany(_ => _.Title).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CultureData>().HasOne(_ => _.FK_HP_Description).WithMany(_ => _.Description).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>().HasOne(_ => _.FK_UserSend).WithMany(_ => _.UserSend).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(_ => _.FK_AdminSend).WithMany(_ => _.AdminSend).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Catalog>().HasOne(_ => _.FatherCatalog).WithMany(_ => _.ChaildCatalogs).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HomePageSectionData>().HasOne(_ => _.FK_HomePageSection).WithMany(_ => _.HomePageSectionDatas).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Atribute>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<AtributeValue>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<Catalog>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<HomePageSection>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<HomePageSectionData>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<Product>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ProductType>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ProductAtributeValue>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ProductIMG>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<User>().Property(_ => _._Index).ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Review>().HasOne(_ => _.FK_Product).WithMany(_ => _.Reviews).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Review>().HasOne(_ => _.FK_User).WithMany(_ => _.Reviews).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>().HasOne(_ => _.User).WithMany(_ => _.Orders).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<OrderAtributeValue>().HasOne(_ => _.ProductInfo).WithMany(_ => _.AtributeAndValue).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<OrderProductInfo>().HasOne(_ => _.Order).WithMany(_ => _.ProductsInfo).OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
