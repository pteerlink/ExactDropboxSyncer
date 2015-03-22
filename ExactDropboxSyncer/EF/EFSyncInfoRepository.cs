using System.Collections.Generic;
using System.Linq;
using ExactDropboxSyncer.Model;

namespace ExactDropboxSyncer.EF
{
    public class EFSyncInfoRepository : ISyncInfoRepository
    {
        private readonly SyncInfoContext context;

        public EFSyncInfoRepository()
        {
            context = new SyncInfoContext();
        }

        public IEnumerable<SyncInfo> GetAll(long dropboxAccountId, int divisionId)
        {
            return FilterByExactAndDropboxId(dropboxAccountId, divisionId);
        }

        public SyncInfo GetByDropboxPath(long dropboxAccountId, int divisionId, string path)
        {
            return FilterByExactAndDropboxId(dropboxAccountId, divisionId).FirstOrDefault(info => info.DropboxPath == path);
        }

        public bool Add(SyncInfo syncInfo)
        {
            context.SyncInfos.Add(syncInfo);
            context.SaveChanges();
            return true;
        }

        public bool Update(SyncInfo syncInfo)
        {
            context.SaveChanges();
            return true;
        }

        public bool Remove(SyncInfo syncInfo)
        {
            context.SyncInfos.Remove(syncInfo);
            context.SaveChanges();
            return true;
        }

        private IQueryable<SyncInfo> FilterByExactAndDropboxId(long dropboxAccountId, int divisionId)
        {
            return context.SyncInfos.Where(info => info.DropboxAccountId == dropboxAccountId && info.ExactDivisionId == divisionId);
        }
    }
}
