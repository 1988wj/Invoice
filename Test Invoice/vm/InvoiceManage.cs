using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test_Invoice.vm
{
    class InvoiceManageVM : ViewModelBase
    {
        bool allowRun;
        string _公司名称;
        bool _已打印, _全选;

        object _选中项;
        public string 公司名称
        {
            get { return _公司名称; }
            set
            {
                _公司名称 = value;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    if (公司 == null)
                    {
                        公司 = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == _公司名称);
                        if (公司 != null)
                        {
                            Refresh();
                        }
                    }
                    else
                    {
                        公司 = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == _公司名称);
                        Refresh();
                    }
                }, null);
            }
        }

        public bool 未打印 { set { if (value) { Select("未打印"); } } }
        public bool 已打印 { get { return _已打印; } set { _已打印 = value; if (_已打印) { Select("已打印"); } } }
        public bool 已作废 { set { if (value) { Select("已作废"); } } }
        public bool 已对账 { set { if (value) { Select("已对账"); } } }

        bool _日期筛选;
        DateTime _开始日期, _结束日期;
        public bool 日期筛选 { get { return _日期筛选; } set { _日期筛选 = value; Refresh(); } }
        public DateTime 开始日期 { get { return _开始日期; } set { _开始日期 = value; Refresh(); } }
        public DateTime 结束日期 { get { return _结束日期; } set { _结束日期 = value; Refresh(); } }

        public object 选中项
        {
            get { return _选中项; }
            set
            {
                _选中项 = value;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    InvoiceModel _InvoiceInfo = _选中项 as InvoiceModel;
                    if (_InvoiceInfo != null)
                    {
                        清单表 = Global.SQL.OtherIDSelectInvoiceList<InvoiceListModelManage>(_InvoiceInfo.ID, true);
                        OnPropertyChanged("清单表");
                    }
                });
            }
        }
        public bool 全选
        {
            get { return _全选; }
            set
            {
                _全选 = value;
                foreach (var item in 显示表)
                {
                    item.选择 = _全选;
                }
                OnPropertyChanged("显示表");
            }
        }



        CompanyModel 公司;
        List<InvoiceModelManage> 发货单表;
        public List<InvoiceListModelManage> 清单表 { get; set; }
        public IEnumerable<InvoiceModelManage> 显示表 { get; set; }

        public DelegateCommand 对账CMD { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InvoiceManageVM()
        {
            //_公司名称 = "";

            _日期筛选 = false;
            _结束日期 = DateTime.Now.Date;
            _开始日期 = _结束日期.AddMonths(-2);

            allowRun = true;
            已打印 = true;
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            对账CMD = new DelegateCommand(Statement);
        }
        private void Statement(object o)
        {
            StatementModel StatementInfo = new StatementModel()
            {
                用户ID = Global.User.ID
            };

            try
            {
                Global.SQL.SaveStatement(StatementInfo, 显示表.Where(_func => _func.选择).ToList());
                SendMessage("对账完成", Global.SQL.ExportStatement(StatementInfo));
            }
            catch (ArgumentNullException)
            {
                SendMessage("发货单空", null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                SendMessage("选择错误", e.Message);
            }
            catch (System.Data.DBConcurrencyException e)
            {
                SendMessage("写入错误", e.Message);
            }
            catch (Exception e)
            {
                SendMessage("其他错误", e.Message);
            }
        }

        /// <summary>
        /// 从数据库筛选指定数据
        /// </summary>
        /// <param name="where">只能为(未打印,已打印,已对账,已作废)这四个值中的其中一个!</param>
        private void Select(string where)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                发货单表 = Global.SQL.WhereSelectInvoices<InvoiceModelManage>(where, Global.User.ID);
                CompanyModel _公司;
                foreach (var item in 发货单表)
                {
                    _公司 = Global.AllCompanies.FirstOrDefault(_func => _func.ID == item.公司ID);
                    if (_公司 != null)
                    {
                        item.公司名称 = _公司.名称;
                    }
                    else
                    {
                        item.公司名称 = "无公司(可能公司已删除或刚新建)!";
                    }
                }
                Refresh();
            });
        }
        /// <summary>
        /// 根据公司及日期条件筛选并刷新显示表数据
        /// </summary>
        private void Refresh()
        {
            if (allowRun)
            {
                allowRun = false;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //延时,毫米
                    Thread.Sleep(568);
                    if (_日期筛选)
                    {
                        if (公司 != null)
                        {
                            显示表 = 发货单表.Where(_func => _func.公司ID == 公司.ID && _func.打印时间 >= 开始日期 && _func.打印时间.Date <= 结束日期);
                        }
                        else
                        {
                            显示表 = 发货单表.Where(_func => _func.打印时间 >= 开始日期 && _func.打印时间.Date <= 结束日期);
                        }
                    }
                    else if (公司 != null)
                    {
                        显示表 = 发货单表.Where(_func => _func.公司ID == 公司.ID);
                    }
                    else
                    {
                        显示表 = 发货单表;
                    }

                    OnPropertyChanged("显示表");
                    allowRun = true;
                }, null);
            }
        }
    }
}
