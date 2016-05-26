using System;
using System.Linq;
using System.Windows;
using System.Threading;

namespace Test_Invoice.vm
{
    class ContactEditVM : ViewModelBase
    {
        //------私有变量------
        bool isNewInfo, allowRun;
        string _名称, _公司名称, _标题, _快递, _说明;
        CompanyModel 公司;
        //------绑定属性------
        public ContactModel 联系人信息 { get; set; }
        public string 名称 { get { return _名称; } set { _名称 = value; OnPropertyChanged("显示拼音"); } }
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
                        //延时,毫米
                        Thread.Sleep(666);
                        公司 = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == _公司名称);
                        if (公司 != null)
                        {
                            联系人信息.地址 = 公司.地址;
                            联系人信息.电话 = 公司.电话;
                            OnPropertyChanged("联系人信息");
                            OnPropertyChanged("显示拼音");
                        }
                        allowRun = true;
                    }, null);
                }
            }
        }
        public bool 男 { get; set; }
        public bool 女 { get; set; }

        public string 显示标题 { get { return _标题; } }
        public string 显示快递 { get { return _快递; } }
        public string 显示说明 { get { return _说明; } }
        public string 显示拼音 { get { return Other.CompanyNameToSpell(string.Format("{0} {1}", _名称, _公司名称)); } }
        //------绑定命令------
        public DelegateCommand 保存CMD { get; set; }
        public DelegateCommand 删除CMD { get; set; }
        /// <summary>
        /// 新建联系人
        /// </summary>
        public ContactEditVM()
        {
            联系人信息 = new ContactModel();
            isNewInfo = true;
            _标题 = string.Format("{0} - 联系人管理>>新建联系人", Global.User.公司名称);
            _快递 = "无";
            _说明 = "新建联系人";
            Initialize();
        }
        /// <summary>
        /// 编辑联系人
        /// </summary>
        /// <param name="ID">联系人ID</param>
        public ContactEditVM(Guid ID)
        {
            联系人信息 = Global.SQL.IDSelectContact<ContactModel>(ID);
            if (联系人信息 != null)
            {
                isNewInfo = false;
                //------名称------
                _名称 = 联系人信息.名称;
                //------公司------
                Guid _公司ID = 联系人信息.公司ID;
                CompanyModel companyInfo = Global.AllCompanies.FirstOrDefault(_func => _func.ID == _公司ID);
                if (companyInfo == null)
                    _公司名称 = "未找到所在公司";
                else
                    _公司名称 = companyInfo.名称;
                //------性别------
                if (联系人信息.性别 == true)
                    男 = true;
                else if (联系人信息.性别 == false)
                    女 = true;
                //------标题------
                _标题 = string.Format("{0} - 联系人管理>>编辑[{1}]信息", Global.User.公司名称, _名称);
                //------_快递ID------
                Guid? _快递ID = 联系人信息.快递ID;
                if (_快递ID == null || _快递ID == Guid.Empty)
                    _快递 = "无";
                else
                {
                    ExpressModel expressInfo = Global.AllExpresses.FirstOrDefault(_func => _func.ID == _快递ID);
                    if (expressInfo == null)
                        _快递 = "无";
                    else
                        _快递 = expressInfo.名称;
                }
                //------_说明------
                _说明 = string.Format("ID:{0}\r\n信息创建于 {1}", 联系人信息.ID, 联系人信息.CreateTime.ToLongDateString());
                if (联系人信息.CreateTime != 联系人信息.UpdateTime)
                {
                    _说明 += string.Format("\r\n最后一次更改与{0}", 联系人信息.UpdateTime.ToLongDateString());
                }
                //------初始化构造函数重载共同部分------
                Initialize();
            }
            else
            {
                throw new ArgumentOutOfRangeException("[联系人ID]无效");
            }
        }
        /// <summary>
        /// 初始化构造函数重载共同部分
        /// </summary>
        private void Initialize()
        {
            allowRun = true;
            保存CMD = new DelegateCommand(Save);
            删除CMD = new DelegateCommand(Delete);
        }
        private void Save(object o)
        {
            //------_名称, _公司为空处理------
            if (_名称 == null) { SendMessage("[名称]为空", "请填写正确的联系人[名称]"); return; }
            else
            {
                _名称 = _名称.Trim();
                if (_名称.Length == 0) { SendMessage("[名称]为空", "请填写正确的联系人[名称]"); return; }
            }
            if (_公司名称 == null) { SendMessage("[公司名称]为空", "请填写正确的[公司名称]"); return; }
            else
            {
                _公司名称 = _公司名称.Trim();
                if (_公司名称.Length == 0) { SendMessage("[公司名称]为空", "请填写正确的[公司名称]"); return; }
            }
            //------公司信息处理------
            CompanyModel companyInfo = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == _公司名称);
            if (companyInfo == null)
            {
                MessageBoxResult _result = MessageBox.Show("输入的公司名称不存在\r\n是否新建此公司", "公司不存在", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                if (_result == MessageBoxResult.Yes)
                {   //------新建公司------
                    companyInfo = new CompanyModel()
                    {
                        用户ID = Global.User.ID,
                        名称 = _公司名称,
                        拼音 = Other.CompanyNameToSpell(_公司名称),
                        赊欠 = Global.User.默认赊欠,
                        地址 = 联系人信息.地址
                    };
                    if (联系人信息.电话 == null || 联系人信息.电话.Length == 0)
                    {
                        if (联系人信息.手机 == null || 联系人信息.手机.Length == 0)
                            companyInfo.电话 = 联系人信息.传真;
                        else
                            companyInfo.电话 = 联系人信息.手机;
                    }
                    else
                        companyInfo.电话 = 联系人信息.电话;
                    //保存新公司
                    try
                    {
                        Global.SQL.SaveCompany(companyInfo, true);
                    }
                    catch (Exception e)
                    {
                        SendMessage("SQL.SaveCompany:", e.ToString());
                    }
                    Global.LoadCompanies();
                }
                else { SendMessage("[公司名称]错误", "请填写正确的[公司名称]"); return; }
            }
            else//------名称在所属公司非唯一处理------
            {
                int _count = Global.AllContacts.Count(_func => _func.名称 == _名称 && _func.公司ID == companyInfo.ID);
                if (_count == 1 && _名称 != 联系人信息.名称 || _count > 1)
                {
                    SendMessage("[名称]重复", "此联系人已存在!"); return;
                }
            }

            if (男)
                联系人信息.性别 = true;
            else if (女)
                联系人信息.性别 = false;
            else
                联系人信息.性别 = null;

            联系人信息.用户ID = Global.User.ID;
            联系人信息.公司ID = companyInfo.ID;
            联系人信息.名称 = _名称;
            联系人信息.拼音 = Other.CompanyNameToSpell(string.Format("{0} {1}", _名称, _公司名称));
            try
            {
                Global.SQL.SaveContact(联系人信息, isNewInfo);
            }
            catch (Exception e)
            {
                SendMessage("SQL.SaveContact:", e.ToString());
            }
            Global.LoadContacts();
            SendMessage("保存成功", null);
        }
        private void Delete(object o) { SendMessage("删除成功", null); }
    }
}
