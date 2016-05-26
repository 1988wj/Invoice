using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Test_Invoice.v
{
    /// <summary>
    /// CompanyManage.xaml 的交互逻辑
    /// </summary>
    public partial class CompanyManage : Page
    {
        vm.CompanyManageVM companyManageVM;
        ListenMessage listenMessage;


        public CompanyManage()
        {
            InitializeComponent();
            this.WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, "公司管理");
            companyManageVM = new vm.CompanyManageVM();
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            this.DataContext = companyManageVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = companyManageVM });
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
                    case "删除信息":
                        MessageBox.Show("删除公司很危险,暂不实现此功能", "完成", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    case "无选中项":
                        MessageBox.Show("请选中需要编辑的公司信息!", "无选中项", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    case "导入信息":
                        MessageBox.Show(
    "只支持增值税开票软件导出的客户编码文本文件(*.txt),\r\n格式为:\r\n{客户编码}[分隔符]\"~~\"\r\n// 每行格式 :\r\n// 编码~~名称~~简码~~税号~~地址电话~~银行账号~~邮件地址~~备注~~身份证校验",
    "公司信息导入说明", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        //获取文件
                        OpenFileDialog openFD = new OpenFileDialog();
                        openFD.Title = "选择导入文件";
                        openFD.Filter = "文本文件|*.txt";
                        if (openFD.ShowDialog() == true)
                        {
                            this.NavigationService.Navigate(new v.CompanyImport(openFD.FileName));
                        }
                        break;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CompanyModelManage companyInfo = DataLV.SelectedItem as CompanyModelManage;
            if (companyInfo != null)
            {
                v.CompanyEdit comEdit = new v.CompanyEdit(companyInfo.ID);
                comEdit.ShowDialog();
                companyManageVM.Refresh();
            }
        }
        private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            companyManageVM.SelectItem(DataLV.SelectedIndex);
        }
    }
}
