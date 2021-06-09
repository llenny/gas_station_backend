using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static gas_station.Startup;

namespace gas_station.Models
{
    public class DBContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<DeliveryItem> DeliveryItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<FillingColumn> FillingColumns { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Storage> Storage { get; set; }

        public DbSet<IdentityRole> IdentityRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //      base.OnModelCreating(builder);

            modelBuilder.Entity<User>().ToTable("Users");
  //          modelBuilder.Entity<Role>().ToTable("Roles");
        }

        public DBContext(DbContextOptions<DBContext> dbContextOptions) : base(dbContextOptions)
        {
            
            Database.EnsureCreated();
            if (!Stations.Any())
            {
                Stations.Add(new Station
                {
                    Name = "Центральная станция BirstTime",
                    Address = "г. Апчхи, Центральная улица, дом 132",
                    Status = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                Stations.Add(new Station
                {
                    Name = "BirstTime станция 1",
                    Address = "г. Апчхи, Шелковая улица, дом 583",
                    Status = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                Stations.Add(new Station
                {
                    Name = "BirstTime станция 2",
                    Address = "г. Апчхи, Сладенькая улица, дом 4",
                    Status = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                Stations.Add(new Station
                {
                    Name = "BirstTime станция 3",
                    Address = "г. Апчхи, Нежная улица, дом 78",
                    Status = true,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                SaveChanges();
            };
            if (!Positions.Any())
            {
                Positions.Add(new Position
                {
                    Name = "Администратор",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                Positions.Add(new Position
                {
                    Name = "Сотрудник",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                SaveChanges();
            };
            if (!FillingColumns.Any())
            {
                FillingColumns.Add(new FillingColumn
                {
                    Code = "1",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 1
                });
                FillingColumns.Add(new FillingColumn
                {
                    Code = "2",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 1
                });
                FillingColumns.Add(new FillingColumn
                {
                    Code = "3",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 1
                });
                FillingColumns.Add(new FillingColumn
                {
                    Code = "1",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 2
                });
                FillingColumns.Add(new FillingColumn
                {
                    Code = "2",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 2
                });
                FillingColumns.Add(new FillingColumn
                {
                    Code = "3",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    StationId = 2
                });
                SaveChanges();
            };
            if (!FuelTypes.Any())
            {
                FuelTypes.Add(new FuelType
                {
                    Name = "Бензин",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                FuelTypes.Add(new FuelType
                {
                    Name = "Дизель",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                FuelTypes.Add(new FuelType
                {
                    Name = "Четкий вид топлива",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                FuelTypes.Add(new FuelType
                {
                    Name = "Топливная нефть",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                SaveChanges();
            };
            if (!Fuels.Any())
            {
                Fuels.Add(new Fuel
                {
                    Name = "80",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 1
                });
                Fuels.Add(new Fuel
                {
                    Name = "92",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 1
                });
                Fuels.Add(new Fuel
                {
                    Name = "95",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 1
                });
                Fuels.Add(new Fuel
                {
                    Name = "98",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 1
                });
                Fuels.Add(new Fuel
                {
                    Name = "ДТ",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 2
                });
                Fuels.Add(new Fuel
                {
                    Name = "Пропан",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 4
                });
                Fuels.Add(new Fuel
                {
                    Name = "Метан",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 4
                });
                Fuels.Add(new Fuel
                {
                    Name = "Какое-то топливо",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    FuelTypeId = 3
                });
                SaveChanges();
            };
            if (!Employees.Any())
            {
                Employees.Add(new Employee
                {
                    FirstName = "Иванов",
                    LastName = "Иоан",
                    MiddleName = "Иванович",
                    Birthdate = new DateTime(1995, 1, 1),
                    Code = 1111,
                    Status = true,
                    PositionId = 1,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                Employees.Add(new Employee
                {
                    FirstName = "Петров",
                    LastName = "Петя",
                    MiddleName = "Никитич",
                    Code = 1112,
                    Birthdate = new DateTime(1997, 1, 1),
                    Status = true,
                    PositionId = 2,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                SaveChanges();
            };
            
        }
    }
}
