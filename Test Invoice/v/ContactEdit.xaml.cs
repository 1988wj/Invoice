using System;
using System.Windows;
using System.Windows.Data;

namespace Test_Invoice.v
{
    /// <summary>
    /// ContactEdit.xaml 的交互逻辑
    /// </summary>
    public partial class ContactEdit : SmartWindow
    {
        vm.ContactEditVM contactEditVM;
        ListenMessage listenMessage;

        /// <summary>
        /// 新建联系人
        /// </summary>
        public ContactEdit()
        {
            InitializeComponent();
            contactEditVM = new vm.ContactEditVM();

            DeleteB.Visibility = Visibility.Collapsed;
            Initialize();
        }
        /// <summary>
        /// 编辑联系人
        /// </summary>
        /// <param name="ID">联系人ID</param>
        public ContactEdit(Guid ID)
        {
            InitializeComponent();
            contactEditVM = new vm.ContactEditVM(ID);
            Initialize();
        }
        /// <summary>
        /// 初始化构造函数重载共同部分
        /// </summary>
        private void Initialize()
        {
            InitializeSmart(BodyG);
            this.DataContext = contactEditVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = contactEditVM });
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
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }

    }
}
