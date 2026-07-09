using AvaloniaTestDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaTestDemo.Services;

public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            // 连接字符串
            optionsBuilder.UseSqlServer("Server=.;Database=mytest;Trusted_Connection=True;Encrypt=False");
    }
}