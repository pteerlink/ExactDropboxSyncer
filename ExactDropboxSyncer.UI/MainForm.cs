using System;
using System.Threading;
using System.Windows.Forms;

namespace ExactDropboxSyncer.UI
{
	public partial class MainForm : Form
	{
        private ISyncerFactory syncerFactory;

        public MainForm(ISyncerFactory syncerFactory)
        {
            this.syncerFactory = syncerFactory;
            InitializeComponent();
        }

        private bool syncing;
        private bool Syncing 
        {
            get { return syncing; }
            set
            {
                var statusText = value ? "Syncing" : "Idle";

                if (InvokeRequired)
                    Invoke(new Action(() => labelStatusText.Text = statusText));
                else
                    labelStatusText.Text = statusText;    
                
                syncing = value;
            }
        }
        private void buttonSync_Click(object sender, EventArgs e)
        {
            Sync();
        }

        private void Sync()
	    {
            if (Syncing) return;
            Syncing = true;

	        var t = new Thread(DoSync);
	        t.SetApartmentState(ApartmentState.STA);
	        t.Start();
	    }

	    private void DoSync()
	    {
	        var syncer = syncerFactory.GetSyncer();
	        syncer.SyncFiles();
	        
            Syncing = false;
	    }
	}
}
