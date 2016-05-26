using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test_Invoice.vm
{
    class InvoiceEditVM : ViewModelBase
    {
        bool? _现金客户;
        bool _更改, allowRun;
        string _公司名称;
        CompanyModel _收货公司;

        public string 公司名称
        {
            get { return _公司名称; }
            set
            {
                _公司名称 = value;
                if (allowRun)
                {
                    allowRun = false;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        Thread.Sleep(500);
                        _公司名称 = _公司名称.Trim();
                        _收货公司 = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == _公司名称);
                        if (_收货公司 != null)
                        {
                            if (_现金客户 != !_收货公司.赊欠)
                            {
                                _现金客户 = !_收货公司.赊欠;
                                SendMessage("现金客户" + !_收货公司.赊欠, null);
                            }
                        }
                        else
                        {
                            if (_现金客户 != null)
                            {
                                _现金客户 = null;
                                SendMessage("现金客户null", null);
                            }
                        }
                        allowRun = true;
                    }, null);
                }
            }
        }
        public bool 已付款 { get; set; }

        public InvoiceModel 发货单 { get; set; }
        public List<InvoiceListModelEdit> 发货清单 { get; set; }


        public DelegateCommand 打印CMD { get; set; }
        public DelegateCommand 保存CMD { get; set; }
        public DelegateCommand 预览CMD { get; set; }

        public InvoiceEditVM()
        {
            发货单 = new InvoiceModel();
            发货清单 = new List<InvoiceListModelEdit>();
            _更改 = false;
            Initialize();
        }
        public InvoiceEditVM(Guid InvoiceID)
        {
            发货单 = Global.SQL.IDSelectInvoice<InvoiceModel>(InvoiceID);
            发货清单 = Global.SQL.OtherIDSelectInvoiceList<InvoiceListModelEdit>(InvoiceID, true);
            _更改 = true;
            Initialize();
        }
        public InvoiceEditVM(InvoiceModel InvoiceInfo)
        {
            发货单 = InvoiceInfo;
            发货清单 = Global.SQL.OtherIDSelectInvoiceList<InvoiceListModelEdit>(发货单.ID, true);
            _更改 = true;
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            allowRun = true;
            打印CMD = new DelegateCommand(Print);
            保存CMD = new DelegateCommand(Save);
            预览CMD = new DelegateCommand(Preview);
        }
        private void Print(object o) { Submit("打印"); }
        private void Save(object o) { Submit("保存"); }
        private void Preview(object o) { Submit("预览"); }
        /// <summary>
        /// 提交信息
        /// </summary>
        /// <param name="cmd">只能为("打印","保存","预览")中的一个</param>
        private void Submit(string cmd)
        {
            //验证公司信息
            if (_收货公司 == null)
            {
                SendMessage("VM.SaveInvoice:数据为空", "请选择正确的收货公司");
                return;
            }
            //验证清单信息
            List<InvoiceListModelEdit> invoiceList = 发货清单.Where(_func => _func.名称 != null && _func.名称.Length != 0 || _func.数量 != 0).ToList();
            if (invoiceList.Count == 0)
            {
                SendMessage("VM.SaveInvoice:数据为空", "请填写正确的清单信息");
                return;
            }
            switch (cmd)
            {
                case "打印":
                    if (SaveInvoice(invoiceList))
                    {
                        if (PrintInvoice(invoiceList, false))
                        {
                            SendMessage("保存成功", "数据已打印完成!");
                        }
                    }
                    return;
                case "保存":
                    if (SaveInvoice(invoiceList))
                    {
                        SendMessage("保存成功", "数据已暂存,可在管理页面重新选择编辑!");
                    }
                    return;
                case "预览":
                    PrintInvoice(invoiceList, true);
                    return;
            }
        }
        /// <summary>
        /// 保存发货单及清单信息
        /// </summary>
        /// <param name="invoiceList">整理过的发货清单</param>
        private bool SaveInvoice(List<InvoiceListModelEdit> invoiceList)
        {

            发货单.公司ID = _收货公司.ID;
            发货单.用户ID = Global.User.ID;
            //数据库中页码会在SaveInvoice中更新,内存中数据页码也要及时更新,否则预览页面会有Bug
            Global.User.发货单总页码++;
            _收货公司.发货单分页码++;
            //保存发货单及清单
            try
            {
                Global.SQL.SaveInvoice(发货单, invoiceList, _更改);
                return true;
            }
            catch (ArgumentNullException e)
            {
                SendMessage("SQL.SaveInvoice:数据为空", e.Message);
                return false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                SendMessage("SQL.SaveInvoice:数据溢出", e.Message);
                return false;
            }
            catch (Exception e)
            {
                SendMessage("SQL.SaveInvoice:存储错误", e.Message);
                return false;
            }
        }
        /// <summary>
        /// 打印或预览发货单
        /// </summary>
        /// <param name="invoiceList">整理过的发货清单</param>
        /// <param name="isPreview">true:预览,false:打印</param>
        private bool PrintInvoice(List<InvoiceListModelEdit> invoiceList, bool isPreview)
        {
            InvoiceModelPrint invoicePrintInfo;
            BillModel invoiceTemplate;
            //配置打印信息
            if (isPreview)
            {
                decimal money = invoiceList.Sum(_func => _func.单价 * _func.数量);
                invoicePrintInfo = new InvoiceModelPrint()
                {
                    联系人 = 发货单.联系人,
                    金额 = money,
                    总页码 = Global.User.发货单总页码 + 1,
                    分页码 = _收货公司.发货单分页码 + 1,
                    打印时间 = DateTime.Now,
                    Comment = 发货单.Comment,
                    公司名称 = _收货公司.名称,
                    赊欠金额 = _收货公司.赊欠金额 + money
                };
            }
            else
            {
                try
                {
                    invoicePrintInfo = Global.SQL.PrintInvoice(发货单.ID, 已付款);
                }
                catch (Exception e)
                {
                    SendMessage("打印失败", "SQL.PrintInvoice:" + e.Message);
                    return false;
                }
            }
            //获取模板
            try
            {
                invoiceTemplate = XmlLinq.GetInvoiceTemplate(invoicePrintInfo, 已付款);
            }
            catch (Exception e)
            {
                SendMessage("打印失败", "XmlLinq.GetInvoiceTemplate:" + e.Message);
                return false;
            }
            //打印
            try
            {
                BillPrint<InvoiceListModelEdit> print = new BillPrint<InvoiceListModelEdit>(invoiceTemplate, invoiceList);
                if (isPreview)
                    print.PreviewPage();
                else
                    print.PrintPage();
                return true;
            }
            catch (Exception e)
            {
                SendMessage("打印失败", "BillPrint:" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// View控制删除[发货清单]的行
        /// </summary>
        /// <param name="SelectedIndex">第几行从 0 开始</param>
        public void Delete(int SelectedIndex)
        {
            if (发货清单.Count > SelectedIndex)
                发货清单.RemoveAt(SelectedIndex);
        }
        /// <summary>
        /// View获取总金额
        /// </summary>
        public string TotalAmount()
        {
            decimal _dec = 发货清单.Sum(_func => _func.单价 * _func.数量);
            return Other.MoneyToCN(_dec) + "　" + _dec.ToString("C");
        }
    }
}
