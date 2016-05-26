using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test_Invoice.vm
{
    class ExpressPrintVM : ViewModelBase
    {
        //------私有变量------
        bool allowRun;
        string _搜索;
        object _选中快递, _选中项;
        List<ExpressModel> _快递列表;
        List<ContactModelManage> _联系人表;
        //------绑定属性------
        public string 搜索
        {
            get { return _搜索; }
            set
            {
                _搜索 = value;
                if (allowRun)
                {
                    allowRun = false;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        //延时,毫米
                        Thread.Sleep(666);
                        if (_搜索 == null)
                            显示表 = _联系人表;
                        else
                        {
                            _搜索 = _搜索.Trim().ToLower();
                            if (_搜索.Length == 0)
                                显示表 = _联系人表;
                            else
                                显示表 = _联系人表.Where(_func => _func.拼音.Contains(_搜索)).ToList();
                        }
                        OnPropertyChanged("显示表");
                        allowRun = true;
                    }, null);
                }
            }
        }
        public object 选中快递 { get { return _选中快递; } set { _选中快递 = value; OnPropertyChanged("选中模板"); } }
        public object 选中项
        {
            get { return _选中项; }
            set
            {
                _选中项 = value;
                if (_选中项 != null)
                {
                    Guid? id = ((ContactModelManage)_选中项).快递ID;
                    if (id != null)
                    {
                        _选中快递 = _快递列表.FirstOrDefault(func => func.ID == id);
                        OnPropertyChanged("选中快递");
                    }
                }
                OnPropertyChanged("选中项");
            }
        }
        public List<ContactModelManage> 显示表 { get; set; }
        public List<ExpressModel> 显示快递列表 { get { return _快递列表; } }

        public ExpressPrintVM()
        {
            Initialize();
        }
        /// <summary>
        /// 初始化构造函数重载共同部分
        /// </summary>
        private void Initialize()
        {
            allowRun = true;
            _快递列表 = Global.AllExpresses;
            _联系人表 = Global.AllContacts.Select(_func => new ContactModelManage(_func)).ToList();
            _选中快递 = _快递列表.FirstOrDefault();
            显示表 = _联系人表;
        }
    }
}
