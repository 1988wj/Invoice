﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test_Invoice.vm
{
    class ContactManageVM : ViewModelBase
    {
        //------私有变量------
        bool allowRun, _全选;
        string _搜索;
        List<ContactModelManage> _联系人表;
        //------绑定属性------
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
            }
        }
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
                                显示表 = _联系人表.Where(_func => _func.选择 || _func.拼音.Contains(_搜索)).ToList();// || _func.名称.Contains(_搜索) || _func.显示公司.Contains(_搜索)
                        }
                        OnPropertyChanged("显示表");
                        allowRun = true;
                    }, null);
                }
            }
        }
        public object 选中项 { get; set; }
        public List<ContactModelManage> 显示表 { get; set; }
        //------绑定命令------
        public DelegateCommand 插入CMD { get; set; }
        public DelegateCommand 更新CMD { get; set; }
        public DelegateCommand 删除CMD { get; set; }
        public DelegateCommand 导入CMD { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContactManageVM()
        {
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            插入CMD = new DelegateCommand(Insert);
            更新CMD = new DelegateCommand(Update);
            删除CMD = new DelegateCommand(Delete);
            导入CMD = new DelegateCommand(Import);
            allowRun = true;
            Refresh();
        }
        private void Insert(object o)
        {
            v.ContactEdit conEdit = new v.ContactEdit();
            conEdit.ShowDialog();
            Refresh();
        }
        private void Update(object o)
        {
            ContactModel contactInfo = 选中项 as ContactModel;
            if (contactInfo != null)
            {
                v.ContactEdit conEdit = new v.ContactEdit(contactInfo.ID);
                conEdit.ShowDialog();
                Refresh();
            }
            else
            {
                SendMessage("无选中项", null);
            }
        }
        private void Delete(object o) { SendMessage("删除信息", null); }
        private void Import(object o) { SendMessage("导入信息", null); }
        /// <summary>
        /// 刷新显示数据
        /// </summary>
        public void Refresh()
        {
            _联系人表 = Global.AllContacts.Select(_func => new ContactModelManage(_func)).ToList();
            if (_搜索 == null)
            {
                显示表 = _联系人表;
                OnPropertyChanged("显示表");
            }
            else
                搜索 = _搜索;
        }
        /// <summary>
        /// 选择指定行
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            if (itemIndex > -1 && itemIndex < 显示表.Count)
                显示表[itemIndex].选择 = !显示表[itemIndex].选择;
        }
    }
}
