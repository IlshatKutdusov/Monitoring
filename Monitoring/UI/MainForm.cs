using System;
using System.Windows.Forms;

using Monitoring.Views;

namespace Monitoring.UI
{
    public partial class MainForm : Form, IMainFormView
    {
        protected ApplicationContext _context;

        public MainForm(ApplicationContext context)
        {
            _context = context;
            InitializeComponent();
            this.Load += (sender, args) => Invoke(FormLoad);
            this.FormClosing += (sender, args) => Invoke(Close);
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            contextMenuStrip1.Opening += (sender, args) => Invoke(ContextMenuOpen);
            WebSiteAddToolStripMenuItem.Click += (sender, args) => Invoke(WebSiteAdd);
            WebSiteChangeToolStripMenuItem.Click += (sender, args) => Invoke(WebSiteChange);
            WebSiteDeleteToolStripMenuItem.Click += (sender, args) => Invoke(WebSiteDelete);
        }

        private void Invoke(Action action)
        {
            if (action != null)
                action();
        }

        public event Action FormLoad;
        public event Action WebSiteAdd;
        public event Action WebSiteDelete;
        public event Action WebSiteChange;
        public event Action ContextMenuOpen;
        public new event Action Close;

        public new void Show()
        {
            _context.MainForm = this;
            Application.Run(_context);
        }

        public DataGridView GetDataGridView()
        {
            return dataGridView1;
        }

        public ContextMenuStrip GetContextMenuStrip()
        {
            return contextMenuStrip1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }
    }
}