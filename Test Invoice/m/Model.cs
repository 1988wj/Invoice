using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Test_Invoice
{
    /// <summary>
    /// 公司模型(导入)
    /// </summary>
    class CompanyModelImport : CompanyModel, INotifyPropertyChanged
    {
        bool _选择;
        public bool 选择 { get { return _选择; } set { _选择 = value; OnPropertyChanged("选择"); } }
        public System.Windows.Visibility 显示选择 { get { return (this.Comment == "可导入" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden); } }

        //实现INotifyPropertyChanged事件-实时更新页面绑定数据
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// 公司模型(管理)
    /// </summary>
    class CompanyModelManage : CompanyModel, INotifyPropertyChanged
    {
        string _结算, _联系人;
        bool _选择;
        public bool 选择 { get { return _选择; } set { _选择 = value; OnPropertyChanged("显示选择"); } }
        public string 显示选择 { get { return (_选择 ? "√" : string.Empty); } }
        public string 显示结算 { get { return _结算; } }
        public string 显示联系人 { get { return _联系人; } }
        public CompanyModelManage(CompanyModel companyInfo)
        {
            ID = companyInfo.ID;
            用户ID = companyInfo.用户ID;
            图片ID = companyInfo.图片ID;
            名称 = companyInfo.名称;
            拼音 = companyInfo.拼音;
            发货单分页码 = companyInfo.发货单分页码;
            累计金额 = companyInfo.累计金额;
            赊欠金额 = companyInfo.赊欠金额;
            赊欠 = companyInfo.赊欠;
            税号 = companyInfo.税号;
            地址 = companyInfo.地址;
            电话 = companyInfo.电话;
            银行 = companyInfo.银行;
            账号 = companyInfo.账号;
            Comment = companyInfo.Comment;
            CreateTime = companyInfo.CreateTime;
            UpdateTime = companyInfo.UpdateTime;

            if (赊欠)
                _结算 = string.Format("累计消费:{0},其中 {1} 未付", 累计金额.ToString("0.00#"), 赊欠金额.ToString("0.00#"));
            else
                _结算 = string.Format("现金客户,累计消费:{0}", 累计金额.ToString("0.00#"));
            //_联系人 = "";
            foreach (var item in Global.AllContacts.Where(_func => _func.公司ID == ID).Select(_func => _func.名称))
            {
                _联系人 += string.Format("[{0}]", item);
            }
        }

        //实现INotifyPropertyChanged事件-实时更新页面绑定数据
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// 联系人模型(管理)
    /// </summary>
    class ContactModelManage : ContactModel, INotifyPropertyChanged
    {
        string _公司, _性别, _快递;
        bool _选择;
        public bool 选择 { get { return _选择; } set { _选择 = value; OnPropertyChanged("显示选择"); } }
        public string 显示选择 { get { return (_选择 ? "√" : string.Empty); } }
        public string 显示公司 { get { return _公司; } }
        public string 显示性别 { get { return _性别; } }
        public string 显示快递 { get { return _快递; } }

        public string 公司简称 { get { return Other.ShortCompanyName(_公司); } }
        public string 手机优先 { get { if (手机.Length != 0) return 手机; else if (电话.Length != 0) return 电话; else return 传真; } }
        public string 电话优先 { get { if (电话.Length != 0) return 电话; else if (手机.Length != 0) return 手机; else return 传真; } }
        public string 传真优先 { get { if (传真.Length != 0) return 传真; else if (电话.Length != 0) return 电话; else return 手机; } }
        public ContactModelManage(ContactModel contactInfo)
        {
            ID = contactInfo.ID;
            用户ID = contactInfo.用户ID;
            公司ID = contactInfo.公司ID;
            图片ID = contactInfo.图片ID;
            快递ID = contactInfo.快递ID;
            名称 = contactInfo.名称;
            拼音 = contactInfo.拼音;
            昵称 = contactInfo.昵称;
            地址 = contactInfo.地址;
            手机 = contactInfo.手机;
            电话 = contactInfo.电话;
            传真 = contactInfo.传真;
            性别 = contactInfo.性别;
            Comment = contactInfo.Comment;
            CreateTime = contactInfo.CreateTime;
            UpdateTime = contactInfo.UpdateTime;
            //_公司名称
            CompanyModel companyInfo = Global.AllCompanies.FirstOrDefault(_func => _func.ID == 公司ID);
            if (companyInfo != null)
                _公司 = companyInfo.名称;
            else
                _公司 = "未找到所属公司";
            //_显示性别
            if (性别 == null)
                _性别 = "保密";
            else if (性别 == true)
                _性别 = "先生";
            else
                _性别 = "女士";
            //_快递
            if (快递ID == null || 快递ID == Guid.Empty)
                _快递 = "无";
            else
            {
                ExpressModel expressInfo = Global.AllExpresses.FirstOrDefault(_func => _func.ID == 快递ID);
                if (expressInfo == null)
                    _快递 = "无";
                else
                    _快递 = expressInfo.名称;
            }
        }

        //实现INotifyPropertyChanged事件-实时更新页面绑定数据
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// 发货单模型(管理)
    /// </summary>
    class InvoiceModelManage : InvoiceModel, INotifyPropertyChanged
    {
        bool _选择;
        public bool 选择 { get { return _选择; } set { _选择 = value; OnPropertyChanged("选择"); } }
        public string 公司名称 { get; set; }
        public string 日期 { get { return (打印 ? 打印时间.ToShortDateString() : CreateTime.ToShortDateString()); } }
        public string 说明
        {
            get
            {
                if (作废)
                {
                    return "已作废";
                }
                else if (付款)
                {
                    return "已对账付款";
                }
                else if (对账)
                {
                    return "已对账";
                }
                else if (打印)
                {
                    return "已打印可对账";
                }
                return "未打印";
            }
        }

        //public ManageInvoiceModel(InvoiceModel invoiceModel)
        //{
        //    ID = invoiceModel.ID;
        //    用户ID = invoiceModel.用户ID;
        //    公司ID = invoiceModel.公司ID;
        //    对账单ID = invoiceModel.对账单ID;
        //    联系人 = invoiceModel.联系人;
        //    金额 = invoiceModel.金额;
        //    总页码 = invoiceModel.总页码;
        //    分页码 = invoiceModel.分页码;
        //    打印 = invoiceModel.打印;
        //    对账 = invoiceModel.对账;
        //    付款 = invoiceModel.付款;
        //    作废 = invoiceModel.作废;
        //    打印时间 = invoiceModel.打印时间;
        //    Comment = invoiceModel.Comment;
        //    CreateTime = invoiceModel.CreateTime;
        //    UpdateTime = invoiceModel.UpdateTime;
        //}

        //实现INotifyPropertyChanged事件-实时更新页面绑定数据
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// 发货单模型(打印)
    /// </summary>
    class InvoiceModelPrint : InvoiceModel
    {
        /// <summary>
        /// 64字符
        /// </summary>
        public string 公司名称 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 赊欠金额 { get; set; }
    }
    /// <summary>
    /// 发货清单模型(编辑)
    /// </summary>
    class InvoiceListModelEdit : InvoiceListModel, INotifyPropertyChanged
    {
        public decimal 编辑数量 { get { return 数量; } set { 数量 = value; OnPropertyChanged("显示金额"); } }
        public decimal 编辑单价 { get { return 单价; } set { 单价 = value; OnPropertyChanged("显示金额"); } }
        public string 显示金额 { get { return (数量 * 单价).ToString("0.00#"); } }

        //实现INotifyPropertyChanged事件-实时更新页面绑定数据
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class InvoiceListModelManage : InvoiceListModel
    {
        public string 显示数量 { get { return 数量.ToString("0.###"); } }
        public string 显示单价 { get { return 单价.ToString("0.00#"); } }
        public string 显示金额 { get { return (数量 * 单价).ToString("0.00#"); } }
    }

    /// <summary>
    /// 打印标签模型
    /// </summary>
    class LabelPrintModel
    {

        public List<LabelModel> 标签 { get; set; }
        /// <summary>
        /// 起始点矫正
        /// 单位:1/100英寸
        /// </summary>
        public System.Drawing.PointF 起始点 { get; set; }
        /// <summary>
        /// 单位:1/100英寸
        /// </summary>
        public System.Drawing.Printing.PaperSize 纸张尺寸 { get; set; }
        /// <summary>
        /// 文本达到长度后自动换行
        /// 单位:1/100英寸
        /// </summary>
        public int 文本长度 { get; set; }
        /// <summary>
        /// 根据[快递模板模型]初始化
        /// </summary>
        /// <param name="expInfo">快递模板模型</param>
        /// <param name="textLength">文本达到长度后自动换行</param>
        public LabelPrintModel(ExpressModel expInfo, int textLength)
        {
            标签 = expInfo.标签;
            起始点 = new System.Drawing.PointF(expInfo.水平平移 / 0.254f, expInfo.垂直平移 / 0.254f);
            纸张尺寸 = new System.Drawing.Printing.PaperSize(expInfo.名称, expInfo.宽, expInfo.高);
            文本长度 = textLength;
        }
    }
    /// <summary>
    /// 快递模板模型(xml)
    /// </summary>
    public class ExpressModel
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 名称 { get; set; }
        /// <summary>
        /// 单位:1/100英寸
        /// </summary>
        public int 宽 { get; set; }
        /// <summary>
        /// 单位:1/100英寸
        /// </summary>
        public int 高 { get; set; }
        /// <summary>
        /// 单位:毫米(用户可输入-99~99)
        /// </summary>
        public int 水平平移 { get; set; }
        /// <summary>
        /// 单位:毫米(用户可输入-99~99)
        /// </summary>
        public int 垂直平移 { get; set; }
        public List<LabelModel> 标签 { get; set; }
    }
    /// <summary>
    /// 清单模板模型(xml)
    /// </summary>
    class BillModel
    {
        /// <summary>
        /// 打印纸名称
        /// </summary>
        public System.Drawing.Printing.PaperSize 纸张尺寸 { get; set; }
        /// <summary>
        /// 打印纸与打印区域左右两边空白
        /// 单位:毫米(用户可输入0~99)
        /// </summary>
        public int 水平边距 { get; set; }
        /// <summary>
        /// 打印位置整体水平位移
        /// 单位:毫米(用户可输入-99~99)
        /// </summary>
        public int 水平平移 { get; set; }
        /// <summary>
        /// 打印纸与打印区域上下两边空白
        /// 单位:毫米(用户可输入0~99)
        /// </summary>
        public int 垂直边距 { get; set; }
        /// <summary>
        /// 打印位置整体垂直位移
        /// 单位:毫米(用户可输入-99~99)
        /// </summary>
        public int 垂直平移 { get; set; }
        /// <summary>
        /// 表格文字字体
        /// 整张表只能用相同字体
        /// 表头字体自动加粗
        /// </summary>
        public string 表字体 { get; set; }
        /// <summary>
        /// 表格文字字号
        /// 整张表只能用相同字号
        /// 单位:逻辑像素,以1/96英寸为单位
        /// </summary>
        public int 表字号 { get; set; }
        /// <summary>
        /// 表格位于虚拟行的行号
        /// </summary>
        public int 表行号 { get; set; }
        /// <summary>
        /// 虚拟行列表
        /// </summary>
        public List<RowModel> 段落 { get; set; }
        /// <summary>
        /// 打印标签列表
        /// </summary>
        public List<LabelModel> 标签 { get; set; }
        /// <summary>
        /// 表格列信息列表
        /// </summary>
        public List<TableColumnModel> 表列 { get; set; }
    }
    /// <summary>
    /// 模板标签模型
    /// </summary>
    public class LabelModel
    {
        /// <summary>
        /// 所需绘制的文本
        /// </summary>
        public string 文本 { get; set; }
        /// <summary>
        /// 绘制文本的字体
        /// </summary>
        public string 字体 { get; set; }
        /// <summary>
        /// 绘制文本的字号
        /// 单位:逻辑像素,以1/96英寸为单位
        /// </summary>
        public double 字号 { get; set; }
        /// <summary>
        /// 绝对定位全局距离,相对定位虚拟行内距离
        /// 单位:逻辑像素,以1/96英寸为单位
        /// </summary>
        public double 水平边距 { get; set; }
        /// <summary>
        /// 相对定位
        /// 靠左"Left",居中"Center",靠右"Right"
        /// </summary>
        public string 水平对齐 { get; set; }
        /// <summary>
        /// 绝对定位全局距离,相对定位虚拟行内距离
        /// 单位:逻辑像素,以1/96英寸为单位
        /// </summary>
        public double 垂直边距 { get; set; }
        /// <summary>
        /// 相对定位
        /// 靠左"Left",居中"Center",靠右"Right"
        /// </summary>
        public string 垂直对齐 { get; set; }
        /// <summary>
        /// 相对定位虚拟行的行号
        /// </summary>
        public int 行号 { get; set; }
        /// <summary>
        /// 是否为绝对定位
        /// </summary>
        public bool 绝对定位 { get; set; }
        public LabelModel() { }
        /// <summary>
        /// 用TextBox初始化
        /// </summary>
        public LabelModel(System.Windows.Controls.TextBox text)
        {
            文本 = text.Text;
            字体 = text.FontFamily.Source;
            字号 = text.FontSize;
            水平边距 = text.Margin.Left;
            垂直边距 = text.Margin.Top;
            绝对定位 = true;
        }
        /// <summary>
        /// 用TextBox初始化,并根据缩放系数还原字号和边距
        /// </summary>
        /// <param name="text">TextBox</param>
        /// <param name="zoomCoefficient">缩放系数</param>
        public LabelModel(System.Windows.Controls.TextBox text, double zoomCoefficient)
        {
            文本 = text.Text;
            字体 = text.FontFamily.Source;
            字号 = text.FontSize / zoomCoefficient;
            水平边距 = text.Margin.Left / zoomCoefficient;
            垂直边距 = text.Margin.Top / zoomCoefficient;
            绝对定位 = true;
        }
        /// <summary>
        /// 用TextBlock初始化,并根据缩放系数还原字号和边距
        /// </summary>
        /// <param name="text">TextBlock</param>
        /// <param name="zoomCoefficient">缩放系数</param>
        public LabelModel(System.Windows.Controls.TextBlock text, double zoomCoefficient)
        {
            文本 = text.Text;
            字体 = text.FontFamily.Source;
            字号 = text.FontSize / zoomCoefficient;
            水平边距 = text.Margin.Left / zoomCoefficient;
            垂直边距 = text.Margin.Top / zoomCoefficient;
            绝对定位 = true;
        }
    }
    /// <summary>
    /// 模板行模型
    /// </summary>
    public class RowModel
    {
        /// <summary>
        /// 相对定位虚拟行行号
        /// </summary>
        public int 行号 { get; set; }
        /// <summary>
        /// 标签相对定位的行高(行间距)非表内行高度
        /// </summary>
        public int 行高 { get; set; }
    }
    /// <summary>
    /// 模板表列模型
    /// </summary>
    public class TableColumnModel
    {
        /// <summary>
        /// 绑定的后台数据名称
        /// </summary>
        public string 绑定 { get; set; }
        /// <summary>
        /// 单据打印及显示的表头名称
        /// </summary>
        public string 表头 { get; set; }
        /// <summary>
        /// 列的比例宽度(在打印时会计算为绝对宽度)
        /// </summary>
        public double 列宽 { get; set; }
        /// <summary>
        /// 靠左"Left",居中"Center",靠右"Right"
        /// </summary>
        public string 水平对齐 { get; set; }
        /// <summary>
        /// 当前列的X坐标(打印时系统会自动生成)
        /// </summary>
        public double X { get; set; }
    }

}
