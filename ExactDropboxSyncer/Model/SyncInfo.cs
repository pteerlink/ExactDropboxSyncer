using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExactDropboxSyncer.Model
{
    public class SyncInfo
	{
        [Key, Column(Order = 0)]
        public long DropboxAccountId { get; set; }
        [Key, Column(Order = 1)]
        public string DropboxPath { get; set; }
        [Key, Column(Order = 2)]
        public int ExactDivisionId { get; set; }
        [Key, Column(Order = 3)]
        public Guid ExactId { get; set; }
        public DateTime LastModified { get; set; }
	}
}