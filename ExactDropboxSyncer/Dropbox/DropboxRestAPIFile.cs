using System;
using System.IO;
using System.Threading.Tasks;
using DropboxRestAPI;
using DropboxRestAPI.Models.Core;

namespace ExactDropboxSyncer.Dropbox
{
    class DropboxRestAPIFile : IDropboxFile
	{
        private readonly MetaData file;
        private readonly IClient client;

        public DropboxRestAPIFile(MetaData file, IClient client)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (client == null)
                throw new ArgumentNullException("client");

            this.file = file;
            this.client = client;
        }

	    public string FilePath
		{
			get { return file.path; }
		}

		public DateTime Modified 
		{
            get { return DateTime.Parse(file.modified); }
		}

		public byte[] GetFileContent()
		{
			using (var downloadStream = new MemoryStream())
			{
                Task.WaitAll(client.Core.Metadata.FilesAsync(file.path, downloadStream));
                return downloadStream.ToArray();
			}
		}
	}
}
