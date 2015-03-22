using System;
using System.Linq;
using ExactOnline.Client.Models;

namespace ExactDropboxSyncer.Exact
{
    public class ExactDocumentCategoryService : ExactEntityService<DocumentCategory>, IExactDocumentCategoryService
    {
        public ExactDocumentCategoryService(IExactOnlineApi api) : base(api)
        {           
        }

        public Guid GetByGuidByCategoryName(string name)
        {
            var result = GetQuery().Select("ID").Where("Description+eq+'" + name + "'").Get();
            return result.Single().ID;
        }
    }
}
