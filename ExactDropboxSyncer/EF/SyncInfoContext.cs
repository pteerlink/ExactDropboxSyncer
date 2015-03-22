using System.Data.Entity;
using ExactDropboxSyncer.Model;

namespace ExactDropboxSyncer.EF
{
    class SyncInfoContext : DbContext
    {
        public DbSet<SyncInfo> SyncInfos { get; set; }
    }
}
