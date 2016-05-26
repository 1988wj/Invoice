using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Test_Invoice.v
{
    /// <summary>
    /// InvoiceManage.xaml 的交互逻辑
    /// </summary>
    public partial class InvoiceManage : Page
    {
        vm.InvoiceManageVM invoiceManageVM;
        ListenMessage listenMessage;
        List<SmartInfo> 公司名称Smart;

        public InvoiceManage()
        {
            InitializeComponent();
            this.WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, "发货单管理");
            invoiceManageVM = new vm.InvoiceManageVM();
            Initialize();
        }
        private void Initialize()
        {
            this.DataContext = invoiceManageVM;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = invoiceManageVM });
            listenMessage.MessageChangedEvent += ListenMessage_MessageChangedEvent;

            公司名称Smart = Global.AllCompanies.Select(_func => new SmartInfo()
            {
                Text = _func.名称,
                Letter = _func.拼音
            }).ToList();
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
                    case "对账完成":
                        string StatementString = msg.Body;

                        SaveFileDialog saveFD = new SaveFileDialog();
                        saveFD.Title = "保存对账单";
                        saveFD.Filter = "文本文件|*.xls";
                        saveFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        saveFD.FileName = StatementString.Substring(0, StatementString.IndexOf("\r\n"));
                        if (saveFD.ShowDialog() == true)
                        {
                            File.WriteAllText(saveFD.FileName, StatementString, Encoding.Default);
                        }
                        break;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _CB = sender as CheckBox;
            if (_CB.IsChecked == true)
            {
                DPStart.Visibility = Visibility.Visible;
                DPEnd.Visibility = Visibility.Visible;
            }
            else
            {
                DPStart.Visibility = Visibility.Collapsed;
                DPEnd.Visibility = Visibility.Collapsed;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatementRB.IsChecked == true)
            {
                StatementB.Visibility = Visibility.Visible;
            }
            else
            {
                StatementB.Visibility = Visibility.Collapsed;
            }
        }



        #region    ----------在页面中ListBox和TextBox的事件写法----------
        /// <summary>
        /// comNameTB实现智能列表TextBox的GotFocus事件
        /// </summary>
        private void comNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true, 公司名称Smart);
        }
        //------以上一个为自定义的------
        private void SmartLB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.LBKeyDown(SmartLB, e); }
        private void SmartLB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) { SmartList.FillText(SmartLB); }
        private void SmartTB_LostFocusNotUpdate(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, false); }
        private void SmartTB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.TBKeyDown(SmartLB, e); }
        private void SmartTB_TextChanged(object sender, TextChangedEventArgs e) { SmartList.TBChanged(SmartLB); }
        #endregion ----------在页面中ListBox和TextBox的事件写法----------
    }
}