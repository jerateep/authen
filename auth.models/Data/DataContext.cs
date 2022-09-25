

using auth.models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace auth.models.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions<DataContext> options)
          : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string Constr = "Data Source=localhost;Initial Catalog=authDB;User Id=sa;Password=Abc@12345;Connection Lifetime=30;Pooling=True;Min Pool Size=5;Max Pool Size=100;Connection TimeOut=60;";
                optionsBuilder.UseSqlServer(Constr, o => o.EnableRetryOnFailure());
            }
        }
        public DbSet<TBL_Hmac> TBL_Hmac { get; set; }
        public DbSet<TBL_User> TBL_User { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //}
    }
}
