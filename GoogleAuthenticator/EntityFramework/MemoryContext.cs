using GoogleAuthenticator.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoogleAuthenticator.EntityFramework;

public sealed class MemoryContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public MemoryContext(DbContextOptions<MemoryContext> options) : base(options) { }
}