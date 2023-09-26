using GNSW.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class VehicleContext
    {
        public class CountryContext : DbContext
        {
            public DbSet<VehicleReference> VMake { get; set; }
            public DbSet<VehicleModelReference> VModel { get; set; }

        }
    }
}