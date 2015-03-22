using System;
using ExactDropboxSyncer.Dropbox;

namespace ExactDropboxSyncer.Tests
{
    class DropboxFileMock : IDropboxFile
    {
        private readonly byte[] content;

        public DropboxFileMock(byte[] content, string filePath, DateTime modified)
        {
            this.content = content;
            FilePath = filePath;
            Modified = modified;
        }

        public string FilePath { get; set; }
        public DateTime Modified { get; set; }
        public byte[] GetFileContent()
        {
            return content;
        }
    }
}