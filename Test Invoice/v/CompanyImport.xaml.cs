using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Test_Invoice.v
{
    /// <summary>
    /// CompanyImport.xaml 的交互逻辑
    /// </summary>
    public partial class CompanyImport : Page
    {
        vm.CompanyImportVM companyImportVM;
        ListenMessage listenMessage;


        public CompanyImport(string filePath)
        {
            InitializeComponent();
            this.WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, "公司管理>>导入公司信息");
            companyImportVM = new vm.CompanyImportVM(filePath);
            Initialize();
        }
        private void Initialize()
        {
            this.DataContext = companyImportVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = companyImportVM });
            listenMessage.MessageChangedEvent += ListenMessage_MessageChangedEvent;
        }
        /// <summary>
        /// 消息处理
        /// </summary>
        private void ListenMessage_MessageChangedEvent()
        {
            MessageModel msg = listenMessage.GetMessage;
            if (msg != null)
            {
                switch (msg.Head)
                {
                    case "Quit":
                        if (this.NavigationService.CanGoBack)
                        {
                            this.NavigationService.GoBack();
                        }
                        return;
                    case "数据错误":
                        MessageBox.Show("所选文件数据格式错误!", "数据错误", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        if (this.NavigationService.CanGoBack)
                        {
                            this.NavigationService.GoBack();
                        }
                        return;
                    case "导入成功":
                        MessageBox.Show(string.Format("共计 {0} 条信息被成功导入!", msg.Body), "导入成功", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        if (this.NavigationService.CanGoBack)
                        {
                            this.NavigationService.GoBack();
                        }
                        return;
                    case "读取成功":
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }
    }
}
