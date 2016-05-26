using System;
using System.Collections.Generic;
using System.Linq;

namespace Test_Invoice.vm
{
    class CompanyEditVM : ViewModelBase
    {
        //------私有变量------
        bool isNewInfo;
        string _名称, _标题, _说明;
        List<string> _联系人;
        //------绑定属性------
        public CompanyModel 公司信息 { get; set; }
        public string 名称 { get { return _名称; } set { _名称 = value; OnPropertyChanged("显示拼音"); } }

        public string 显示标题 { get { return _标题; } }
        public string 显示说明 { get { return _说明; } }
        public string 显示拼音 { get { return _名称 == null ? string.Empty : Other.CompanyNameToSpell(_名称); } }
        //------绑定命令------
        public DelegateCommand 保存CMD { get; set; }
        public DelegateCommand 删除CMD { get; set; }
        /// <summary>
        /// 新建公司信息
        /// </summary>
        public CompanyEditVM()
        {
            公司信息 = new CompanyModel() { 赊欠 = Global.User.默认赊欠 };
            isNewInfo = true;
            _联系人 = new List<string>();
            _标题 = string.Format("{0} - 公司管理>>新建公司", Global.User.公司名称);
            _说明 = "新建公司";
            Initialize();
        }
        /// <summary>
        /// 编辑公司信息
        /// </summary>
        /// <param name="ID">公司ID</param>
        public CompanyEditVM(Guid ID)
        {
            公司信息 = Global.SQL.IDSelectCompany<CompanyModel>(ID);
            if (公司信息 != null)
            {
                isNewInfo = false;
                //------名称------
                _名称 = 公司信息.名称;
                //------联系人------
                _联系人 = Global.AllContacts.Where(_func => _func.公司ID == 公司信息.ID).Select(_func => _func.名称).ToList();
                //------标题------
                _标题 = string.Format("{0} - 公司管理>>编辑[{1}]信息", Global.User.公司名称, _名称);
                //------_说明------
                _说明 = string.Format("ID:{0}\r\n信息创建于 {1}", 公司信息.ID, 公司信息.CreateTime.ToLongDateString());
                if (公司信息.CreateTime != 公司信息.UpdateTime)
                {
                    _说明 += string.Format("\r\n最后一次更改与{0}", 公司信息.UpdateTime.ToLongDateString());
                }
                if (_联系人 != null && _联系人.Count != 0)
                {
                    _说明 += "\r\n其下有\r\n(";
                    foreach (var item in _联系人)
                    {
                        _说明 += item + "　";
                    }
                    _说明 += string.Format(")\r\n共计 {0} 个联系人", _联系人.Count);
                }
                else
                {
                    _说明 += "\r\n其下没有联系人";
                }
                //------初始化构造函数重载共同部分------
                Initialize();
            }
            else
            {
                throw new ArgumentNullException("[公司信息]无效");
            }
        }
        /// <summary>
        /// 初始化构造函数重载共同部分
        /// </summary>
        private void Initialize()
        {
            保存CMD = new DelegateCommand(Save);
            删除CMD = new DelegateCommand(Delete);
        }
        private void Save(object o)
        {
            //------_名称为空处理------
            if (_名称 == null) { SendMessage("[名称]为空", "请填写正确的联系人[名称]"); return; }
            else
            {
                _名称 = _名称.Trim();
                if (_名称.Length == 0) { SendMessage("[名称]为空", "请填写正确的联系人[名称]"); return; }
            }
            //------名称非唯一处理------
            int _count = Global.AllCompanies.Count(_func => _func.名称.Contains(_名称));
            if (_count == 1 && !公司信息.名称.Contains(_名称) || _count > 1)
            {
                SendMessage("[名称]重复", "此公司已存在!"); return;
            }

            公司信息.用户ID = Global.User.ID;
            公司信息.名称 = _名称;
            公司信息.拼音 = Other.CompanyNameToSpell(_名称);
            try
            {
                Global.SQL.SaveCompany(公司信息, isNewInfo);
            }
            catch (Exception e)
            {
                SendMessage("SQL.SaveCompany:", e.ToString());
            }
            Global.LoadCompanies();
            SendMessage("保存成功", null);
        }
        private void Delete(object o) { SendMessage("删除成功", null); }
    }
}
