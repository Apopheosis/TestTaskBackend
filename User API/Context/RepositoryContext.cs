using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using User_API.Models;
using User_API.Models.Configuration;

namespace Repository;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}