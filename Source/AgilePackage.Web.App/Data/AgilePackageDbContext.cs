using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace AgilePackage.Web.App.Data
{
    public class AgilePackageDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        private IConfiguration Configuration { get; }

        public AgilePackageDbContext(DbContextOptions<AgilePackageDbContext> options) : base(options) { }

        public AgilePackageDbContext(DbContextOptions<AgilePackageDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"));
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Retrospective> Retrospectives { get; set; }
        public DbSet<RetrospectivePost> RetrospectivePosts { get; set; }
        public DbSet<RetrospectivePostVote> RetrospectivePostVotes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadRoom> LeadRooms { get; set; }
        public DbSet<Daily> Dailies { get; set; }
    }
}
