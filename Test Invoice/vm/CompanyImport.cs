using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Test_Invoice.vm
{
    class CompanyImportVM : ViewModelBase
    {
        bool _全选;
        public bool 全选
        {
            get { return _全选; }
            set
            {
                _全选 = value;
                foreach (var item in 导入公司信息表)
                {
                    if (item.Comment == "可导入")
                    {
                        item.选择 = _全选;
                    }
                }
            }
        }

        public List<CompanyModelImport> 导入公司信息表 { get; set; }

        public DelegateCommand 导入CMD { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CompanyImportVM(string filePath)
        {
            Initialize();
            if (File.Exists(filePath))
            {
                try
                {
                    formatCompanyInfos(filePath);
                }
                catch (Exception)
                {
                    SendMessage("数据错误", null);
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            _全选 = true;
            导入CMD = new DelegateCommand(Import);
        }
        private void Import(object o)
        {
            List<CompanyModelImport> ImportList = 导入公司信息表.Where(_func => _func.选择).ToList();
            foreach (var item in ImportList)
            {
                item.拼音 = Other.CompanyNameToSpell(item.名称);
            }
            try
            {
                int i = Global.SQL.ImportCompanyInfos(ImportList, Global.User.ID, Global.User.默认赊欠);
                Global.LoadCompanies();
                SendMessage("导入成功", i.ToString());
            }
            catch (Exception e)
            {
                SendMessage("其他错误", e.ToString());
            }
        }
        /// <summary>
        /// 格式化导入信息
        /// </summary>
        /// <param name="comtxtPath"></param>
        private void formatCompanyInfos(string comtxtPath)
        {
            //读取数据流
            FileStream fs = new FileStream(comtxtPath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, Other.GetEncoding(fs));
            //放置数据流中的每行数据,先读取一行取出分隔符
            string s1 = sr.ReadLine();
            string s2 = Regex.Match(s1, "\"[^\"]*\"").Value.Replace("\"", "");
            //分隔符为空赋以默认分隔符,第一行数据可能是数据行,不能读取下一行数据
            if (s2 == "") { s2 = "~~"; }
            //成功获取分隔符后,跳过分隔符定义行,读取下一行数据
            else { s1 = sr.ReadLine(); }
            //正则表达式 r1拆分字段 r2电话手机号 r3银行帐号
            Regex r1 = new Regex(s2);
            Regex r2 = new Regex(@"\d{11}|\d{7,8}|\d{3,4}-\d{7,8}");
            Regex r3 = new Regex(@"\d+-\d+|\d+");
            //记录导入行数
            int i1 = 0;
            int i2 = 0;
            导入公司信息表 = new List<CompanyModelImport>();
            //循环导入直至读完全部行数
            while (s1 != null)
            {   //调整全角字符
                s1 = Other.SBC2DBC(s1);
                //跳过注释行
                if (!s1.StartsWith("//"))
                {   //去除不必要的空格,引号
                    s1 = s1.Replace(" ", "").Replace("\"", "");

                    //正则表达式~~拆分字符串
                    string[] sList = r1.Split(s1);
                    //拆分后必须有6组及以上字符串才能执行导入操作
                    //否则会程序会执行错误
                    if (sList.Length > 5)
                    {
                        导入公司信息表.Add(new CompanyModelImport()
                        {
                            名称 = sList[1],
                            税号 = sList[3],
                            地址 = r2.Split(sList[4])[0],
                            电话 = r2.Match(sList[4]).Value,
                            银行 = r3.Split(sList[5])[0],
                            账号 = r3.Match(sList[5]).Value
                        });
                        //一行数据导入成功,读取下一行
                        i1++;
                    }
                }
                //下一行
                s1 = sr.ReadLine();
            }
            //关闭数据流释放资源
            sr.Close();

            //检索信息是否在本地存在 根据税号
            //获取本地公司信息
            foreach (var item in 导入公司信息表)
            {
                if (Global.AllCompanies.Any(_func => item.税号.Length != 0 && _func.税号 == item.税号 || item.名称.Contains(_func.名称)))
                {
                    item.Comment = "信息已存在";
                    item.选择 = false;
                }
                else if (null == item.名称 || 0 == item.名称.Length)
                {
                    item.Comment = "名称不能为空";
                    item.选择 = false;
                }
                else
                {
                    item.Comment = "可导入";
                    item.选择 = true;
                    i2++;
                }
            }
            //排序
            导入公司信息表 = 导入公司信息表.OrderBy(_func => _func.Comment).ThenBy(_func => _func.名称).ToList();
            SendMessage("读取成功", string.Format("检测到 {0} 条信息\r\n其中有 {1} 条可导入信息!", i1, i2));
        }
    }
}
