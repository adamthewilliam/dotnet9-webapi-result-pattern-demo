using Dotnet9.WebApi.ResultPattern.Demo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.WebApi.ResultPattern.Demo.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<PenguinModel> Penguins => Set<PenguinModel>();
}