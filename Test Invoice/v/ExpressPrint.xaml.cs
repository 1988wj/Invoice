using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Test_Invoice.v
{
    /// <summary>
    /// ExpressPrint.xaml 的交互逻辑
    /// </summary>
    public partial class ExpressPrint : Page
    {
        vm.ExpressPrintVM expressPrintVM;
        ListenMessage listenMessage;
        ExpressGrid expressGrid;

        public ExpressPrint()
        {
            InitializeComponent();
            expressPrintVM = new vm.ExpressPrintVM();
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            this.DataContext = expressPrintVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = expressPrintVM });
            listenMessage.MessageChangedEvent += ListenMessage_MessageChangedEvent;

            expressGrid = new ExpressGrid(expGrid, true);
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
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpressModel expTempLate = expCB.SelectedValue as ExpressModel;
            if (expTempLate != null)
            {
                double zoom = 0.8;

                expressGrid.ShowExpress(expTempLate, zoom);
                expBorder.Width = expGrid.Width - 88 * zoom;
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var a = expressGrid.GetLabelList();
            LabelPrintModel lpm = new LabelPrintModel((ExpressModel)expressPrintVM.选中快递, 300);
            lpm.标签 = a;

            LabelPrint p = new LabelPrint(lpm, "快递");
            p.AdvancedPrint();
        }
    }
}
