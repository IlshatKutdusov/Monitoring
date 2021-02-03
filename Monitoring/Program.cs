using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightInject;
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

            //ServiceContainer container = new ServiceContainer();

            //container.RegisterInstance<ApplicationContext>(Context);
            //container.Register<IMainFormView, MainForm>();
            //container.Register<MainFormPresenter>();

            //ApplicationController controller = new ApplicationController(container);
            //controller.Run<MainFormPresenter>();

            var controller = new ApplicationController(new InjectAdapter())
                .RegisterView<IMainFormView, MainForm>()
                .RegisterView<ISecondaryFormView, SecondaryForm>()
                .RegisterInstance(new ApplicationContext());

            controller.Run<MainFormPresenter>();
        }
    }
}
