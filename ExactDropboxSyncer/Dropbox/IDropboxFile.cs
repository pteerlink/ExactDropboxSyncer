using System;

namespace ExactDropboxSyncer.Dropbox
{
    public interface IDropboxFile
    {
        string FilePath { get; }
        DateTime Modified { get; }
        byte[] GetFileContent();

    }
}