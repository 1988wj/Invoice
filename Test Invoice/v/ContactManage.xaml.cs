using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Test_Invoice.v
{
    /// <summary>
    /// ContactManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContactManage : Page
    {
        vm.ContactManageVM contactManageVM;
        ListenMessage listenMessage;


        public ContactManage()
        {
            InitializeComponent();
            this.WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, "联系人管理");
            contactManageVM = new vm.ContactManageVM();
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            this.DataContext = contactManageVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = contactManageVM });
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
                    case "无选中项":
                        MessageBox.Show("请选中需要编辑的联系人!", msg.Head, MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContactModelManage contactInfo = DataLV.SelectedItem as ContactModelManage;
            if (contactInfo != null)
            {
                v.ContactEdit conEdit = new v.ContactEdit(contactInfo.ID);
                conEdit.ShowDialog();
                contactManageVM.Refresh();
            }
        }
        private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            contactManageVM.SelectItem(DataLV.SelectedIndex);
        }


    }
}
