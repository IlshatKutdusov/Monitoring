using System;
using System.Windows.Forms;

using Monitoring.Common;
using Monitoring.Presenters;
using Monitoring.Views;
using Monitoring.UI;

namespace Monitoring
{
    static class Program
    {
        public static readonly ApplicationContext Context = new ApplicationContext();

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var controller = new ApplicationController(new InjectAdapter())
                .RegisterView<IMainFormView, MainForm>()
                .RegisterView<ISecondaryFormView, SecondaryForm>()
                .RegisterInstance(new ApplicationContext());

            controller.Run<MainFormPresenter>();
        }
    }
}
