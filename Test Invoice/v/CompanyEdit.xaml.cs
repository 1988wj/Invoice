using System;
using System.Windows;
using System.Windows.Data;

namespace Test_Invoice.v
{
    /// <summary>
    /// CompanyEdit.xaml 的交互逻辑
    /// </summary>
    public partial class CompanyEdit : Window
    {
        vm.CompanyEditVM companyEditVM;
        ListenMessage listenMessage;

        /// <summary>
        /// 新建公司信息
        /// </summary>
        public CompanyEdit()
        {
            InitializeComponent();
            companyEditVM = new vm.CompanyEditVM();

            DeleteB.Visibility = Visibility.Collapsed;
            EditWP.Visibility = Visibility.Collapsed;
            Initialize();
        }
        /// <summary>
        /// 编辑公司信息
        /// </summary>
        /// <param name="ID">公司ID</param>
        public CompanyEdit(Guid ID)
        {
            InitializeComponent();
            companyEditVM = new vm.CompanyEditVM(ID);
            Initialize();
        }
        /// <summary>
        /// 初始化构造函数重载共同部分
        /// </summary>
        private void Initialize()
        {
            this.DataContext = companyEditVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = companyEditVM });
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
                        this.Close();
                        return;
                    case "保存成功":
                        MessageBox.Show("保存成功", "完成", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        this.Close();
                        return;
                    case "删除成功":
                        MessageBox.Show("删除公司很危险,暂不实现此功能", "完成", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        this.Close();
                        return;
                    case "名称为空":
                        MessageBox.Show("请输入公司名称", "公司名称不能为空", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    case "名称存在":
                        MessageBox.Show("公司名称存在", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }
    }
}