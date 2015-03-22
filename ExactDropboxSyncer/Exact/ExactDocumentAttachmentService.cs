using System;
using System.Collections.Generic;
using ExactOnline.Client.Models;

namespace ExactDropboxSyncer.Exact
{
    public class ExactDocumentAttachmentService : ExactEntityService<DocumentAttachment>, IExactDocumentAttachmentService
    {
        public ExactDocumentAttachmentService(IExactOnlineApi api) : base(api)
        {
        }

        public IList<DocumentAttachment> GetByDocumentId(Guid guid)
        {
            return GetQuery().Select("ID").Where("Document+eq+guid'" + guid + "'").Get();
        }
    }
}