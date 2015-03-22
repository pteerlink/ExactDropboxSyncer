using System;
using ExactOnline.Client.Models;

namespace ExactDropboxSyncer.Exact
{
    public interface IExactDocumentCategoryService : IExactEntityService<DocumentCategory>
    {
        Guid GetByGuidByCategoryName(string name);
    }
}
