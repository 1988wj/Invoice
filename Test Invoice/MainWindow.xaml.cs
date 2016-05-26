using System;
using System.Windows;
using System.Windows.Navigation;

namespace Test_Invoice
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //主窗口设置
            double winWidth = SystemParameters.WorkArea.Width;
            double winHeight = SystemParameters.WorkArea.Height;
            if (1280 < winWidth) Width = 1280;
            else Width = winWidth;
            if (800 < winHeight) Height = 800;
            else Height = winHeight;
           
            //初始化智能下拉列表功能
            SmartList.Initialization(66, 100, 200, Global.SmartPath);
        }

        private void NavigationWindow_Closed(object sender, System.EventArgs e)
        {
            //保存智能下拉列表功能
            SmartList.Save();
            //保存智能下拉列表功能
        }
    }
}
