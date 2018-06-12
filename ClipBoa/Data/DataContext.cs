using ClipBoa.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ClipBoa.Data
{
    public class DataContext : DbContext
    {
        // Entities to map
        public DbSet<TransferText> TransferTexties { get; set; }


        public DataContext() : base("DBContext")
        {
            //if (!base.Database.Exists())
            //    base.Database.CreateIfNotExists();

        }

        private void CreateDataBase(Database database)
        {
            database.CreateIfNotExists();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}
