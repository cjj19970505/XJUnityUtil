using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XJUnityUtil.Debug.Tester
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        UnityAppCommManager UnityAppCommManager { get; }
        public MainWindow()
        {
            InitializeComponent();
            UnityAppCommManager = new UnityAppCommManager();
            LocalBackendCommToUnity localBackendCommToUnity = new LocalBackendCommToUnity(UnityAppCommManager, 8043);

            UnityAppCommManager.CommToUnity = localBackendCommToUnity;
            

        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            UnityAppCommManager.CommToUnity.SendMessage(SendTextBox.Text);
        }
    }
}
