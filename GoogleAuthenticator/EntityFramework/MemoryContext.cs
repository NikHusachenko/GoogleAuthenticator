using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoogleAuthenticator.EntityFramework;

public sealed class MemoryContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public MemoryContext(DbContextOptions<MemoryContext> options) : base(options) { }
}