using System;
using System.Windows.Forms;

namespace Monitoring.Views
{
    public interface ISecondaryFormView : IView
    {
        event Action WebSiteUpload;

        TextBox GetTextBoxName();
        TextBox GetTextBoxURL();
        TextBox GetTextBoxCheckInterval();
    }
}
