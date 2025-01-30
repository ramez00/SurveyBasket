using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SurveyBasket.Models;

namespace SurveyBasket.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Poll> Polls { get; set; }
}
