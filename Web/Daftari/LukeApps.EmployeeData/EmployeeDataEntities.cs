namespace LukeApps.EmployeeData
{
    using System.Data.Entity;
    using TrackerEnabledDbContext.Common.Extensions;

    public partial class EmployeeDataEntities : DbContext
    {
        public EmployeeDataEntities()
            : base("EmployeeDataEntities")
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}