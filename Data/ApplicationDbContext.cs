using Experiment_Image_Bulky.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Experiment_Image_Bulky.Data
{
    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        
        public DbSet<Product> Products { get; set; }
        
    }
}
