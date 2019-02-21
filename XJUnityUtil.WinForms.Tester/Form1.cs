using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XJUnityUtil.WinForms.Tester
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser ChromeBrowser;
        WinformCommToUnity WinformCommToUnity;
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();



        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            if (e.IsBrowserInitialized)
            {

            }

        }

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            //settings.CefCommandLineArgs.Add("disable-gpu", "0");
            //settings.CefCommandLineArgs.Add("disable-webgl", "0");

            Cef.Initialize(settings);
            Cef.EnableHighDPISupport();

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            string page = string.Format(@"{0}\webglout\index.html", Application.StartupPath);
            page = "C:\\Users\\cjj19\\Documents\\Project\\LiChenProj\\WinFormTest\\webglout\\index.html";
            ChromeBrowser = new ChromiumWebBrowser(page);
            WinformCommToUnity = new WinformCommToUnity(ChromeBrowser);
            ChromeBrowser.RegisterJsObject("cefUnityHandlerObject", new WinformCommToUnity.CefUnityHandlerObject(WinformCommToUnity));
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            ChromeBrowser.BrowserSettings = browserSettings;



            this.Controls.Add(ChromeBrowser);
            ChromeBrowser.Dock = DockStyle.Fill;


        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Show Dev Tools");
            DateTime beforeSend = DateTime.Now;
            var response = await WinformCommToUnity.SendStringMessageForResultAsync("FUCK", "YOU", "SON", "OF", "Bitch");
            System.Diagnostics.Debug.WriteLine((DateTime.Now - beforeSend).TotalMilliseconds);
            //WinformCommToUnity.SendStringMessage("FUCK", "YOU", "SON", "OF", "Bitch");
        }

        private void OpenDevToolButton_Click(object sender, EventArgs e)
        {
            ChromeBrowser.ShowDevTools();
        }
    }


}
