using System;
using System.Windows.Forms;

namespace Monitoring.Views
{
    public interface IMainFormView : IView
    {
        event Action FormLoad;

        event Action WebSiteAdd;
        event Action WebSiteDelete;
        event Action WebSiteChange;
        event Action ContextMenuOpen;

        new event Action Close;

        DataGridView GetDataGridView();
        ContextMenuStrip GetContextMenuStrip();
    }
}