using System;
using System.Windows.Forms;

using Monitoring.Views;

namespace Monitoring.UI
{
    public partial class SecondaryForm : Form, ISecondaryFormView
    {
        protected ApplicationContext _context;

        public SecondaryForm(ApplicationContext context)
        {
            _context = context;
            InitializeComponent();
            buttonAccept.Click += (sender, args) => WebSiteUpload();
        }

        private void Invoke(Action action)
        {
            if (action != null)
                action();
        }

        public new void Show()
        {
            ShowDialog(new SecondaryForm(_context));
        }

        public event Action WebSiteUpload;

        public TextBox GetTextBoxName()
        {
            return textBoxName;
        }

        public TextBox GetTextBoxURL()
        {
            return textBoxURL;
        }

        public TextBox GetTextBoxCheckInterval()
        {
            return textBoxCheckInterval;
        }
    }
}