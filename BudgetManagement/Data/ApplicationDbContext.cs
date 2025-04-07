using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace ExpenseManagment.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		private readonly IAuthenticatedUserAccessor _authenticatedUser;
		private readonly AuditSettings _settings;
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserAccessor authenticatedUser, AuditSettings settings)
			: base(options)
		{
			_authenticatedUser = authenticatedUser;
			_settings = settings;
		}
		public ApplicationDbContext()
		{

		}
		public virtual DbSet<AccountEntity> AccountEntities { get; set; }
		public virtual DbSet<Project> Projects { get; set; }
		public virtual DbSet<Expence> Expences { get; set; }
		public virtual DbSet<Invoice> Invoices { get; set; }
		public virtual DbSet<Transaction> Transactions { get; set; }
		public virtual DbSet<SallaryMapping> SallaryMappings { get; set; }
		public virtual DbSet<EffectivePercentProjectMapping> EffectivePercentProjectMappings { get; set; }
		public virtual DbSet<GeneratedSallary> GeneratedSallaries { get; set; }
		public virtual DbSet<GeneratedSallaryPercentile> GeneratedSallaryPercentiles { get; set; }
		public virtual DbSet<AuditLog> AuditLogs { get; set; }
		public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }



		public async Task<bool> DbSaveChangesAsync()
		{
			try
			{
				if (!_settings.AuditEnabled)
				{
					await base.SaveChangesAsync();
					return true;
				}

				var userId = _authenticatedUser.UserId;
				var username = _authenticatedUser.Username;
				var auditEntries = await OnBeforeSaveChangesAsync(userId, username);
				var result = await base.SaveChangesAsync();
				await OnAfterSaveChangesAsync(auditEntries);
				return true;
			}
			catch (DbUpdateException ex)
			{
				Log.Error(ex, "Error occurred while saving changes to the database.");
				throw;
			}
		}


		private async Task<List<AuditLog>> OnBeforeSaveChangesAsync(string userId, string username)
		{
			ChangeTracker.DetectChanges();

			var auditEntries = new List<AuditLog>();
			foreach (var entry in ChangeTracker.Entries())
			{
				if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !_settings.TableList.Contains(entry.Entity.GetType().Name))
				{
					continue;
				}


				var auditEntry = new AuditLog
				{
					TableName = entry.Metadata.GetTableName(),
					ActionType = entry.State.ToString(),
					Timestamp = DateTime.UtcNow,
					UserId = userId,
					UserEmail = username,
				};

				// Get the primary key values
				var keyValues = entry.Metadata.FindPrimaryKey()
					.Properties
					.Select(x => entry.Property(x.Name).CurrentValue?.ToString());

				auditEntry.KeyValues = string.Join(", ", keyValues.Where(x => x != null));

				foreach (var property in entry.Properties)
				{
					string columnName = property.Metadata.GetColumnName();
					if (_settings.SkipColumns.Contains(property.Metadata.Name))
						continue;

					if (entry.State == EntityState.Added)
					{
						auditEntry.NewValues += $"{columnName}: {property.CurrentValue?.ToString()}, ";
					}
					else if (entry.State == EntityState.Deleted)
					{
						auditEntry.OldValues += $"{columnName}: {property.OriginalValue?.ToString()}, ";
					}
					else if (entry.State == EntityState.Modified && property.IsModified)
					{
						auditEntry.OldValues += $"{columnName}: {property.OriginalValue?.ToString()}, ";
						auditEntry.NewValues += $"{columnName}: {property.CurrentValue?.ToString()}, ";
					}
				}

				auditEntries.Add(auditEntry);
			}

			return auditEntries;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}



		private async Task OnAfterSaveChangesAsync(List<AuditLog> auditEntries)
		{
			foreach (var auditEntry in auditEntries)
			{
				await this.AuditLogs.AddAsync(auditEntry);
			}

			if (auditEntries.Any())
			{
				await base.SaveChangesAsync();
			}
		}


	}
}
