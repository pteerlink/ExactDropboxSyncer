using System;
using System.Linq;
using ExactDropboxSyncer.Dropbox;
using ExactOnline.Client.Models;
using ExactOnline.Client.Sdk.Controllers;
using ExactOnline.Client.Sdk.Exceptions;

namespace ExactDropboxSyncer.Exact
{
	public class ExactDocumentStore : IExactDocumentStore
	{
	    private readonly IExactOnlineApi api;
        private readonly IExactEntityService<Document> documentService;
        private readonly IExactDocumentAttachmentService  documentAttachmentService;
	    private readonly IExactDocumentCategoryService documentCategoryService;

        public ExactDocumentStore(IExactOnlineApi api, IExactEntityService<Document> documentService, IExactDocumentAttachmentService documentAttachmentService, IExactDocumentCategoryService documentCategoryService)
        {
            this.api = api;
            this.documentService = documentService;
            this.documentAttachmentService = documentAttachmentService;
            this.documentCategoryService = documentCategoryService;
        }

	    public ExactDocumentStore(string exactOnlineUrl, IExactOnlineOAuthIAccessTokenProvider oAuthIAccessTokenProvider)
		{
            api = new ExactOnlineApiClient(
                new ExactOnlineClient(exactOnlineUrl, oAuthIAccessTokenProvider.GetAccessToken)
            );
            documentService = new ExactEntityService<Document>(api);
            documentAttachmentService = new ExactDocumentAttachmentService(api);
            documentCategoryService = new ExactDocumentCategoryService(api);
		}

		public int GetDivisionId()
		{
		    return api.GetDivision();
		}

		public Guid Add(IDropboxFile file)
		{
            var document = new Document { Type = GetDefaultTypeId(), Category = GetDefaultDocumentCategoryId() };
		    UpdateDocumentModel(file, document);

			var insertedDocument = documentService.Create(document);
            if (insertedDocument == null)
				throw new Exception("Document was not inserted");

			try
			{
                var documentAttachment = new DocumentAttachment { Document = insertedDocument.ID };
			    UpdateDocumentDocumentAttachmentModel(file, documentAttachment);

			    var insertedDocumentAttachment = documentAttachmentService.Create(documentAttachment);
                if (insertedDocumentAttachment == null)
					throw new Exception("Attachment was not inserted");
			}
			catch
			{
                documentService.Delete(insertedDocument);
				throw;
			}

            return insertedDocument.ID;
		}

	    public void Update(Guid guid, IDropboxFile file)
		{
			var document = documentService.Get(guid);
            UpdateDocumentModel(file, document);

            var documentUpdated = documentService.Update(document);
            if (!documentUpdated)
                throw new Exception("Document was not updated");

            var documentAttachment = documentAttachmentService.GetByDocumentId(guid).FirstOrDefault();
			if (documentAttachment == null)
			{
                documentAttachment = new DocumentAttachment { Document = document.ID };
                UpdateDocumentDocumentAttachmentModel(file, documentAttachment);
			    var insertedDocumentAttachment = documentAttachmentService.Create(documentAttachment);
                if (insertedDocumentAttachment == null)
                    throw new Exception("Attachment was not updated");
			}
			else
			{
                UpdateDocumentDocumentAttachmentModel(file, documentAttachment);
                var documentAttachmentUpdated = documentAttachmentService.Update(documentAttachment);
                if (!documentAttachmentUpdated)
                    throw new Exception("Attachment was not updated");
			}
		}

        private static void UpdateDocumentDocumentAttachmentModel(IDropboxFile file, DocumentAttachment documentAttachment)
        {
            documentAttachment.FileName = GetFileName(file);
            documentAttachment.Attachment = file.GetFileContent();
        }
        private static void UpdateDocumentModel(IDropboxFile file, Document document)
        {
            document.DocumentDate = file.Modified;
            document.Modified = file.Modified;
            document.Subject = GetFileName(file);
        }

		public void Delete(Guid guid)
		{
			var document = documentService.Get(guid);
            documentService.Delete(document);
		}

		public bool ContainsDocument(Guid guid)
		{
		    try
		    {
                var document = documentService.Get(guid);
                return document != null;
            }
		    catch (NotFoundException)
		    {
		        return false;
		    }
        }

        private Guid GetDefaultDocumentCategoryId()
        {
            return documentCategoryService.GetByGuidByCategoryName("Employees"); //TODO: Setting van maken
        }

        private static int GetDefaultTypeId()
        {
            return 55; // Diversen
        }

	    private static string GetFileName(IDropboxFile file)
	    {
	        return System.IO.Path.GetFileName(file.FilePath);
	    }
    }
}
