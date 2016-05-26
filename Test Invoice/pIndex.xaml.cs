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

namespace Test_Invoice
{
    /// <summary>
    /// pIndex.xaml 的交互逻辑
    /// </summary>
    public partial class pIndex : Page
    {
        public pIndex( )
        {
            InitializeComponent();
            WindowTitle = string.Format("{0} - {1}", Global.User.公司名称, Global.User.版本);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button _B = (Button)sender;
            string buttonContext = _B.Content.ToString();
            switch (buttonContext)
            {
                case "开送货单":
                    this.NavigationService.Navigate(new v.InvoiceEdit());
                    break;
                case "送货单管理":
                    this.NavigationService.Navigate(new v.InvoiceManage());
                    break;
                case "打印快递":
                    this.NavigationService.Navigate(new v.ExpressPrint());
                    break;
                case "快递单管理":
                    //this.NavigationService.Navigate(new v.InvoiceEdit());
                    break;
                case "公司管理":
                    this.NavigationService.Navigate(new v.CompanyManage());
                    break;
                case "联系人管理":
                    this.NavigationService.Navigate(new v.ContactManage());
                    break;
            }
        }
    }
}
