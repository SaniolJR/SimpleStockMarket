using Microsoft.EntityFrameworkCore;
using Entities;

namespace Database;
public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    public virtual DbSet<Stock> Stocks {get; set;}
    public virtual DbSet<Wallet> Wallets {get; set;}
    public virtual DbSet<AuditLog> AuditLogs {get; set;}
}