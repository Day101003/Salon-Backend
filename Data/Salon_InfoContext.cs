using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Salon_Info.Models;

namespace Salon_Info.Data
{
    public class Salon_InfoContext : DbContext
    {
        public Salon_InfoContext (DbContextOptions<Salon_InfoContext> options)
            : base(options)
        {
        }

        public DbSet<Salon_Info.Models.Category> Categorie { get; set; } = default!;
        public DbSet<Salon_Info.Models.Audit> Auditoria { get; set; } = default!;
        public DbSet<Salon_Info.Models.User> User { get; set; } = default!;
        public DbSet<Salon_Info.Models.Service> Service { get; set; } = default!;
        public DbSet<Salon_Info.Models.Product> Product { get; set; } = default!;
        public DbSet<Salon_Info.Models.Cita> Cita { get; set; } = default!;
    }
}
