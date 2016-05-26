using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Test_Invoice.v
{
    /// <summary>
    /// InvoiceEdit.xaml 的交互逻辑
    /// </summary>
    public partial class InvoiceEdit : Page
    {
        vm.InvoiceEditVM invoiceEditVM;
        ListenMessage listenMessage;
        List<SmartInfo> 公司名称Smart;

        public InvoiceEdit()
        {
            InitializeComponent();
            this.WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, "新建发货单");
            invoiceEditVM = new vm.InvoiceEditVM();

            TooltipTB.Visibility = Visibility.Visible;
            CashRB.Visibility = Visibility.Collapsed;
            CreditRB.Visibility = Visibility.Collapsed;
            TotalAmountTB.Text = "";

            Initialize();
        }
        private void Initialize()
        {
            this.DataContext = invoiceEditVM;
            invoiceDG.ItemsSource = invoiceEditVM.发货清单;
            listenMessage = new ListenMessage();
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = invoiceEditVM });
            listenMessage.MessageChangedEvent += ListenMessage_MessageChangedEvent;

            公司名称Smart = Global.AllCompanies.Select(_func => new SmartInfo()
            {
                Text = _func.名称,
                Letter = _func.拼音
            }).ToList();
        }

        private void Refresh()
        {

            TooltipTB.Visibility = Visibility.Visible;
            CashRB.Visibility = Visibility.Collapsed;
            CreditRB.Visibility = Visibility.Collapsed;
            TotalAmountTB.Text = "";
            invoiceEditVM = new vm.InvoiceEditVM();
            this.DataContext = invoiceEditVM;
            invoiceDG.ItemsSource = invoiceEditVM.发货清单;
            BindingOperations.SetBinding(listenMessage, ListenMessage.MessageProperty, new Binding("Message") { Source = invoiceEditVM });
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
                    case "现金客户null":
                        TooltipTB.Visibility = Visibility.Visible;
                        CashRB.Visibility = Visibility.Collapsed;
                        CreditRB.Visibility = Visibility.Collapsed;
                        break;
                    case "现金客户False":
                        TooltipTB.Visibility = Visibility.Collapsed;
                        CashRB.Visibility = Visibility.Visible;
                        CreditRB.Visibility = Visibility.Visible;
                        CreditRB.IsChecked = true;
                        break;
                    case "现金客户True":
                        TooltipTB.Visibility = Visibility.Collapsed;
                        CashRB.Visibility = Visibility.Visible;
                        CreditRB.Visibility = Visibility.Collapsed;
                        CashRB.IsChecked = true;
                        break;
                    case "保存成功":
                    case "打印失败":
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        Refresh();
                        return;
                    default:
                        MessageBox.Show(msg.Body, msg.Head, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                }
            }
        }



        /// <summary>
        /// 金额更改后更改总金额
        /// </summary>
        private void Money_TextChanged(object sender, TextChangedEventArgs e)
        {
            TotalAmountTB.Text = invoiceEditVM.TotalAmount();
        }
        /// <summary>
        /// 删除行
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int i = invoiceDG.SelectedIndex;
            invoiceDG.ItemsSource = null;
            invoiceEditVM.Delete(i);
            invoiceDG.ItemsSource = invoiceEditVM.发货清单;
            TotalAmountTB.Text = invoiceEditVM.TotalAmount();
        }
        /// <summary>
        /// 阻止Backspace键返回上级页面
        /// </summary>
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) { e.Handled = true; }
        }
        /// <summary>
        /// 点击单元格直接进入编辑状态
        /// </summary>
        private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            invoiceDG.BeginEdit();
        }
        /// <summary>
        /// 按空格当前单元格进入编辑状态
        /// </summary>
        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                invoiceDG.BeginEdit();
                e.Handled = true;
            }
        }
        #region    ----------输入数字事件集合----------
        /// <summary>
        /// 数字TextBox失去焦点
        /// 检查输入合法性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decimal_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox _TB = sender as TextBox;
            if (null != _TB)
            {
                try { _TB.Text = decimal.Parse(_TB.Text).ToString(); }
                catch { _TB.Text = "0"; }
            }
        }
        /// <summary>
        /// 数字TextBox按键按下
        /// Decimal数字输入规则
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decimal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox _TB = sender as TextBox;
            if (null != _TB)
            {
                //1~9 小键盘数字,键盘数字并且没组合键修饰(Ctrl,Shift,Alt,Win)
                if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9 || e.Key >= Key.D1 && e.Key <= Key.D9 && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {   //0开头并且光标在0后面->删除首位0>>继续
                    if (_TB.Text.StartsWith("0") && _TB.SelectionStart == 1) { _TB.Text = _TB.Text.Substring(1); }
                    //-0开头并且光标在0后面->删除首位0>>光标放负号后>>继续
                    else if (_TB.Text.StartsWith("-0") && _TB.SelectionStart == 2) { _TB.Text = "-" + _TB.Text.Substring(2); _TB.SelectionStart = 1; }
                    //光标在首位并且有负号->终止
                    else if (_TB.Text.StartsWith("-") && _TB.SelectionStart == 0) { e.Handled = true; }
                }
                //0 小键盘数字,键盘数字并且没组合键修饰(Ctrl,Shift,Alt,Win)
                else if (e.Key == Key.NumPad0 || e.Key == Key.D0 && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {   //空,.开头,光标在第二位并且(只有-,-.开头)->继续
                    if (_TB.Text == "" || _TB.Text.StartsWith(".") || _TB.SelectionStart == 1 && (_TB.Text == "-" || _TB.Text.StartsWith("-."))) { }
                    //光标在首位,光标在第二位并且(-开头,0开头),光标在第三位并且-0开头->终止
                    else if (_TB.SelectionStart == 0 && _TB.SelectedText.Length == 0 || _TB.SelectionStart == 1 && (_TB.Text.StartsWith("-") || _TB.Text.StartsWith("0")) || _TB.SelectionStart == 2 && _TB.Text.StartsWith("-0")) { e.Handled = true; }
                }
                //. 小键盘,键盘并且没组合键修饰(Ctrl,Shift,Alt,Win)
                else if (e.Key == Key.Decimal || (e.Key == Key.OemPeriod && e.KeyboardDevice.Modifiers == ModifierKeys.None))
                {
                    //有.并且没选中点,有-并且光标在首位并且没选中负号->终止
                    if (_TB.Text.Contains(".") && !_TB.SelectedText.Contains(".") || _TB.Text.StartsWith("-") && _TB.SelectionStart == 0 && !_TB.SelectedText.StartsWith("-")) { e.Handled = true; }
                }
                //负号并且Negative允许输入负数 小键盘,键盘并且没组合键修饰(Ctrl,Shift,Alt,Win)
                else if (e.Key == Key.Subtract || (e.Key == Key.OemMinus && e.KeyboardDevice.Modifiers == ModifierKeys.None))
                {   //保存当前光标位
                    int _selectionStart = _TB.SelectionStart;
                    //首位有负号->删除负号>>光标不在首位前移一位>>终止
                    if (_TB.Text.StartsWith("-"))
                    {
                        _TB.Text = _TB.Text.Substring(1);
                        if (_selectionStart != 0) { _TB.SelectionStart = _selectionStart - 1; }
                        e.Handled = true;
                    }
                    //首位无负号->添加负号>>索引位后移一位>>终止
                    else
                    {
                        _TB.Text = "-" + _TB.Text;
                        _TB.SelectionStart = _selectionStart + 1;
                        e.Handled = true;
                    }
                }
                //退格,删除.回车,左,右,Tab->继续
                else if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right) { }
                //Ctrl+(C,V,Z,X)->继续
                else if ((e.Key == Key.C || e.Key == Key.V || e.Key == Key.Z || e.Key == Key.X) && e.KeyboardDevice.Modifiers == ModifierKeys.Control) { }
                //其他->终止
                else { e.Handled = true; }
            }
        }
        #endregion ----------输入数字事件集合----------
        /// <summary>
        /// 文本框加载时自动获取焦点
        /// </summary>
        private void TextBox_Loaded(object sender, RoutedEventArgs e) { TextBox _TB = sender as TextBox; if (null != _TB) { _TB.Focus(); _TB.SelectionStart = _TB.Text.Length; } }
        #region    ----------在页面中ListBox和TextBox的事件写法----------
        /// <summary>
        /// comNameTB实现智能列表TextBox的GotFocus事件
        /// </summary>
        private void comNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true, 公司名称Smart);
        }
        /// <summary>
        /// conNameTB实现智能列表TextBox的GotFocus事件
        /// </summary>
        private void conNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyString = comNameTB.Text.Trim();
                Guid keyID;
                if (keyString.Length == 0)//公司名称为空,检索个人客户
                {
                    keyID = Guid.Empty;
                }
                else
                {
                    CompanyModel _a = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == keyString);
                    if (_a != null)//找到公司ID,检索单位所属客户
                    {
                        keyID = _a.ID;

                    }
                    else//找不到公司ID,直接返回
                    {
                        return;
                    }
                }
                List<SmartInfo> smartList = Global.AllContacts.Where(_func => _func.公司ID == keyID).Select(_func => new SmartInfo()
                {
                    Text = _func.名称,
                    Letter = _func.拼音
                }).ToList();

                SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true, smartList);
            }
            catch (Exception)
            {
                MessageBox.Show("标记(Mark)错误!");
                return;
            }
        }
        //------以上两个为自定义的------
        private void SmartLB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.LBKeyDown(SmartLB, e); }
        private void SmartLB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) { SmartList.FillText(SmartLB); }
        private void SmartTB_GotFocus(object sender, RoutedEventArgs e) { SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true); }
        private void SmartTB_LostFocus(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, true); }
        private void SmartTB_LostFocusNotUpdate(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, false); }
        private void SmartTB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.TBKeyDown(SmartLB, e); }
        private void SmartTB_TextChanged(object sender, TextChangedEventArgs e) { SmartList.TBChanged(SmartLB); }
        #endregion ----------在页面中ListBox和TextBox的事件写法----------
    }
}
