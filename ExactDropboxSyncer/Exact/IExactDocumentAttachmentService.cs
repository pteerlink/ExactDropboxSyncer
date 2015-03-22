using System;
using System.Collections.Generic;
using ExactOnline.Client.Models;

namespace ExactDropboxSyncer.Exact
{
    public interface IExactDocumentAttachmentService : IExactEntityService<DocumentAttachment>
    {
        IList<DocumentAttachment> GetByDocumentId(Guid guid);
    }
}
