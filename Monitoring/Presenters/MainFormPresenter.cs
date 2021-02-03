using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

using Monitoring.Views;
using Monitoring.Models;
using Monitoring.Common;

namespace Monitoring.Presenters
{
    public class MainFormPresenter : Presener<IMainFormView>
    {
        private readonly IMainFormView _mainFormView;
        private IApplicationController _applicationController;
        private List<WebSite> _webSiteList;
        private List<Task> _taskList = new List<Task>();
        private DataGridView _dataGridView = new DataGridView();
        private ContextMenuStrip _contextMenuStrip = new ContextMenuStrip();
        private int _selectedDataGridViewRow = 0;
        private readonly string _dataFileName = ConfigurationSettings.AppSettings["dataFileName"];
        private bool _isAvialability;
        private bool _enable = true;

        #region StatusCode
        private static IEnumerable<HttpStatusCode> onlineStatusCodes = new[]
        {
            HttpStatusCode.Accepted,
            HttpStatusCode.Found,
            HttpStatusCode.OK,
        };
        #endregion

        public MainFormPresenter(IApplicationController applicationController, IMainFormView mainFormView) : base(applicationController, mainFormView)
        {
            _mainFormView = mainFormView;
            _applicationController = applicationController;
            _mainFormView.FormLoad += () => WebSiteListLoad();
            _mainFormView.Close += () => WebSiteListSave();
            _mainFormView.WebSiteAdd += () => WebSiteAdd();
            _mainFormView.WebSiteDelete += () => WebSiteDelete();
            _mainFormView.WebSiteChange += () => WebSiteChange();
            _mainFormView.ContextMenuOpen += () => ContextMenuOpen();
            _dataGridView = _mainFormView.GetDataGridView();
            _contextMenuStrip = _mainFormView.GetContextMenuStrip();

        }

        public List<WebSite> GetWebSiteList()
        {
            return _webSiteList;
        }

        public new void Run()
        {
            _mainFormView.Show();
        }

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage);
        }

        private void WebSiteAdd()
        {
            try
            {
                WebSite _webSite = new WebSite("", "https://", "");
                Controller.Run<SecondaryFormPresenter, WebSite>(_webSite);
                if (_webSite.Name != "")
                {
                    _webSiteList.Add(_webSite);
                    Task task = new Task(() => WebSiteCheck(_webSite));
                    _taskList.Add(task);
                    _taskList[_taskList.Count - 1].Start();
                    task = null;
                    _dataGridView.DataSource = null;
                    _dataGridView.DataSource = _webSiteList;
                }
                else
                    _webSite = null;
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private void WebSiteDelete()
        {
            try
            {
                WebSite _webSite = _webSiteList[_selectedDataGridViewRow];
                _webSite = null;
                _webSiteList.RemoveAt(_selectedDataGridViewRow);
                _taskList.RemoveAt(_selectedDataGridViewRow);
                _dataGridView.DataSource = null;
                _dataGridView.DataSource = _webSiteList;
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private async void WebSiteChange()
        {
            try
            {
                WebSite _webSite = _webSiteList[_selectedDataGridViewRow];
                await Task.Run(() => Controller.Run<SecondaryFormPresenter, WebSite>(_webSite));
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private void WebSiteListLoad()
        {
            try
            {
                //Если имя файла не указано
                if (_dataFileName == "" || _dataFileName == null)
                {
                    ShowErrorMessage("Путь к файлу не указан");
                    return;
                }

                //Если файл не существует
                if (!File.Exists(_dataFileName))
                {
                    using (File.Create(Directory.GetCurrentDirectory() + "\\" + _dataFileName)) ;
                }

                using (StreamReader sr = new StreamReader(_dataFileName))
                {
                    if (sr.ReadToEnd() == "")
                    {
                        sr.Close();
                        using (StreamWriter sw = new StreamWriter(_dataFileName))
                        {
                            sw.WriteLine("SampleName#https://www.youtube.com#3000");
                        }
                    }
                }

                using (StreamReader sr = new StreamReader(_dataFileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        try
                        {
                            string[] arr = str.Split('#');
                            if (arr[1][0] == 'w')
                                arr[1] = "https://" + arr[1];
                            WebSite website = new WebSite(arr[0], arr[1], arr[2]);
                            if (_webSiteList == null)
                                _webSiteList = new List<WebSite>();
                            _webSiteList.Add(website);
                            Task task = new Task(() => WebSiteCheck(website));
                            _taskList.Add(task);
                            task = null;
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage("" + ex);
                        }
                    }
                }
                _dataGridView.DataSource = _webSiteList;
                ThreadCreate();
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private void WebSiteListSave()
        {
            try
            {
                if (_webSiteList == null)
                {
                    ShowErrorMessage("Список сайтов пуст!");
                    return;
                }

                //Если имя файла не указано
                if (_dataFileName == "" || _dataFileName == null)
                {
                    ShowErrorMessage("Путь к файлу не указан");
                    return;
                }

                //Если файл не существует
                if (!File.Exists(_dataFileName))
                {
                    using (File.Create(Directory.GetCurrentDirectory() + "\\" + _dataFileName)) ;
                }

                using (StreamWriter sw = new StreamWriter(_dataFileName, false))
                {
                    for (int i = 0; i < _webSiteList.Count; i++)
                    {
                        try
                        {
                            WebSite webSite = _webSiteList[i];
                            sw.WriteLine(webSite.Name + '#' + webSite.URL + '#' + webSite.AvialabilityCheckInterval);
                        }
                        catch (Exception ex)
                        {
                            ShowErrorMessage("" + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private void ThreadCreate()
        {
            try
            {
                var BackgroundThread = new Thread(() => WebSiteUpdata());
                BackgroundThread.IsBackground = true;
                BackgroundThread.Start();
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }

        private void WebSiteUpdata()
        {
            var StopWatch = Stopwatch.StartNew();
            {
                try
                {
                    for (int i = 0; i < _webSiteList.Count; i++)
                    {
                        _taskList[i].Start();
                    }
                }
                catch(Exception ex)
                {
                    ShowErrorMessage("" + ex);
                }
            }
            StopWatch.Stop();
        }

        private void WebSiteCheck(WebSite webSite)
        {
            do
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(webSite.URL);
                    request.Timeout = Convert.ToInt16(webSite.AvialabilityCheckInterval) - 1;
                    request.AllowAutoRedirect = false;
                    request.Method = "HEAD";
                    _isAvialability = false;
                    var StopWatch = Stopwatch.StartNew();
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            _isAvialability = true;
                            webSite.SetStatus("Available");
                            webSite.SetPing(Convert.ToString(StopWatch.ElapsedMilliseconds));
                        }
                        else
                        {
                            _isAvialability = true;
                            webSite.SetStatus("Founded");
                            webSite.SetPing("    ---    ");
                        }
                    }
                    StopWatch.Stop();
                    request = null;
                    StopWatch = null;
                }
                catch (WebException wex)
                {
                    _isAvialability = false;
                    webSite.SetStatus("Not Available");
                    webSite.SetPing("    ---    ");
                    //ShowErrorMessage("" + wex);
                }
                Thread.Sleep(Convert.ToInt16(webSite.AvialabilityCheckInterval));
            } while (_enable);
        }

        private void ContextMenuOpen()
        {
            try
            {
                _selectedDataGridViewRow = _dataGridView.CurrentRow.Index;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("" + ex);
            }
        }
    }
}