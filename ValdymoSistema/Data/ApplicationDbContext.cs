﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ValdymoSistema.Data.Entities;

namespace ValdymoSistema.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Light> Lights { get; set; }
        public DbSet<LightsGroup> LightsGroups { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Trigger> Triggers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }
    }
}