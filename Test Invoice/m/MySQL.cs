using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace Test_Invoice
{
    /// <summary>
    /// 数据存储至MySQL数据库
    /// </summary>
    class MySQL
    {
        string ConnectionString;
        string Database;
        /// <summary>
        /// 初始化
        /// 检测"连接字符串"和"数据库名"是否正确
        /// 请配合异常处理
        /// </summary>
        /// <param name="connectionString">MySQL连接字符串</param>
        /// <param name="database">MySQL数据库名称</param>
        public MySQL(string connectionString, string database)
        {
            ConnectionString = connectionString;
            Database = database;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(string.Format("use `{0}`;", Database), conn);
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    //conn.Close();
                    throw new FormatException(e.Message);
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    //conn.Close();
                    throw new DataException("无数据库");
                }
                cmd.CommandText = string.Format("select count(*) from information_schema.tables where table_schema = '{0}';", Database);
                if (cmd.ExecuteScalar().ToString() == "0")
                {
                    //conn.Close();
                    throw new DataException("无数据表");
                }

                ConnectionString += string.Format("Database={0};", Database);
                //conn.Close();
            }
        }
        /// <summary>
        /// 创建数据库(只在程序首次运行时执行)
        /// </summary>
        /// <param name="connectionString">MySQL连接字符串</param>
        /// <param name="database">MySQL数据库名称</param>
        /// <param name="createBase">true:创建整个数据库,fales:库已存在只建内容</param>
        public MySQL(string connectionString, string database, bool createBase)
        {
            ConnectionString = connectionString;
            Database = database;
            if (createBase)
            {
                using (MySqlConnection conn0 = new MySqlConnection(ConnectionString))
                {
                    MySqlCommand cmd0 = new MySqlCommand();
                    cmd0.Connection = conn0;
                    cmd0.CommandType = CommandType.Text;
                    cmd0.CommandText = string.Format("CREATE DATABASE `{0}`;", Database);
                    conn0.Open();
                    cmd0.ExecuteNonQuery();
                    //conn0.Close();
                }
            }

            ConnectionString += string.Format("Database={0};", Database);


            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                #region ------MySQL数据库建表及存储过程------
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = System.IO.File.ReadAllText("Infos\\mysql.txt");
                conn.Open();
                cmd.ExecuteNonQuery();
                //conn.Close();
                #endregion ------MySQL数据库建表及存储过程------

                #region ------载入Xml公司信息------
                //IEnumerable<CompanyModel> comList = XElement.Load("Infos\\c1.xml").Elements().Select(_func => new CompanyModel
                //{
                //    ID = Guid.Parse(_func.Attribute("ID").Value),
                //    名称 = _func.Element("名称").Value,
                //    拼音 = _func.Element("拼音").Value,
                //    税号 = _func.Element("税号").Value,
                //    地址 = _func.Element("地址").Value,
                //    电话 = _func.Element("电话").Value,
                //    银行 = _func.Element("银行").Value,
                //    账号 = _func.Element("帐号").Value
                //});
                #endregion ------载入Xml公司信息------

                #region ------载入Xml联系人信息------
                //IEnumerable<ContactModel> conList = XElement.Load("Infos\\c2.xml").Elements().Select(_func => new ContactModel
                //{
                //    ID = Guid.Parse(_func.Attribute("ID").Value),
                //    公司ID = Guid.Parse(_func.Element("单位ID").Value),
                //    名称 = _func.Element("名称").Value,
                //    拼音 = _func.Element("拼音").Value,
                //    地址 = _func.Element("地址").Value,
                //    手机 = _func.Element("手机").Value,
                //    电话 = _func.Element("电话").Value,
                //    传真 = _func.Element("传真").Value,
                //    Comment = _func.Element("备注").Value
                //});
                #endregion ------载入Xml联系人信息------
                //剔除无联系人公司
                //List<CompanyModel> comList2 = comList.Where(_func => conList.Count(_func2 => _func2.公司ID == _func.ID) != 0).ToList();

                #region ------公司信息导入MySQL数据库------
                //MySqlCommand cmd2 = new MySqlCommand("InsertCompany", conn);
                //cmd2.CommandType = CommandType.StoredProcedure;
                //MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                //MySqlParameter p1 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
                //MySqlParameter p2 = new MySqlParameter("@p图片ID", MySqlDbType.VarChar, 36);
                //MySqlParameter p3 = new MySqlParameter("@p名称", MySqlDbType.VarChar, 64);
                //MySqlParameter p4 = new MySqlParameter("@p拼音", MySqlDbType.VarChar, 256);
                //MySqlParameter p5 = new MySqlParameter("@p赊欠", MySqlDbType.Bit);
                //MySqlParameter p6 = new MySqlParameter("@p税号", MySqlDbType.VarChar, 32);
                //MySqlParameter p7 = new MySqlParameter("@p地址", MySqlDbType.VarChar, 128);
                //MySqlParameter p8 = new MySqlParameter("@p电话", MySqlDbType.VarChar, 32);
                //MySqlParameter p9 = new MySqlParameter("@p银行", MySqlDbType.VarChar, 64);
                //MySqlParameter pa = new MySqlParameter("@p账号", MySqlDbType.VarChar, 32);
                //MySqlParameter pb = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
                //cmd2.Parameters.Add(p0);
                //cmd2.Parameters.Add(p1);
                //cmd2.Parameters.Add(p2);
                //cmd2.Parameters.Add(p3);
                //cmd2.Parameters.Add(p4);
                //cmd2.Parameters.Add(p5);
                //cmd2.Parameters.Add(p6);
                //cmd2.Parameters.Add(p7);
                //cmd2.Parameters.Add(p8);
                //cmd2.Parameters.Add(p9);
                //cmd2.Parameters.Add(pa);
                //cmd2.Parameters.Add(pb);
                //p1.Value = Guid.Empty;
                //p2.Value = null;
                //p5.Value = true;
                //pb.Value = null;
                //conn.Open();
                //foreach (var item in comList2)
                //{
                //    p0.Value = item.ID;
                //    p3.Value = item.名称;
                //    p4.Value = item.拼音;
                //    p6.Value = item.税号;
                //    p7.Value = item.地址;
                //    p8.Value = item.电话;
                //    p9.Value = item.银行;
                //    pa.Value = item.账号;
                //    cmd2.ExecuteNonQuery();
                //}
                //conn.Close();
                #endregion ------公司信息导入MySQL数据库------

                #region ------联系人息导入MySQL数据库------
                //MySqlCommand cmd3 = new MySqlCommand("InsertContact", conn);
                //cmd3.CommandType = CommandType.StoredProcedure;
                //MySqlParameter pp0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                //MySqlParameter pp1 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
                //MySqlParameter pp2 = new MySqlParameter("@p公司ID", MySqlDbType.Guid);
                //MySqlParameter pp3 = new MySqlParameter("@p图片ID", MySqlDbType.VarChar, 36);
                //MySqlParameter pp4 = new MySqlParameter("@p名称", MySqlDbType.VarChar, 64);
                //MySqlParameter pp5 = new MySqlParameter("@p拼音", MySqlDbType.VarChar, 256);
                //MySqlParameter pp6 = new MySqlParameter("@p昵称", MySqlDbType.VarChar, 64);
                //MySqlParameter pp7 = new MySqlParameter("@p地址", MySqlDbType.VarChar, 128);
                //MySqlParameter pp8 = new MySqlParameter("@p手机", MySqlDbType.VarChar, 32);
                //MySqlParameter pp9 = new MySqlParameter("@p电话", MySqlDbType.VarChar, 32);
                //MySqlParameter ppa = new MySqlParameter("@p传真", MySqlDbType.VarChar, 32);
                //MySqlParameter ppb = new MySqlParameter("@p性别", MySqlDbType.Bit);
                //MySqlParameter ppc = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
                //cmd3.Parameters.Add(pp0);
                //cmd3.Parameters.Add(pp1);
                //cmd3.Parameters.Add(pp2);
                //cmd3.Parameters.Add(pp3);
                //cmd3.Parameters.Add(pp4);
                //cmd3.Parameters.Add(pp5);
                //cmd3.Parameters.Add(pp6);
                //cmd3.Parameters.Add(pp7);
                //cmd3.Parameters.Add(pp8);
                //cmd3.Parameters.Add(pp9);
                //cmd3.Parameters.Add(ppa);
                //cmd3.Parameters.Add(ppb);
                //cmd3.Parameters.Add(ppc);
                //pp1.Value = Guid.Empty;
                //pp3.Value = null;
                //pp6.Value = null;
                //ppb.Value = null;
                //conn.Open();
                //foreach (var item in conList)
                //{
                //    pp0.Value = item.ID;
                //    pp2.Value = item.公司ID;
                //    pp4.Value = item.名称;
                //    pp5.Value = item.拼音;
                //    pp7.Value = item.地址;
                //    pp8.Value = item.手机;
                //    pp9.Value = item.电话;
                //    ppa.Value = item.传真;
                //    ppc.Value = item.Comment;
                //    cmd3.ExecuteNonQuery();
                //}
                //conn.Close();
                #endregion ------联系人息导入MySQL数据库------
            }
        }

        /// <summary>
        /// ID查询公司信息
        /// </summary>
        /// <typeparam name="T">公司模型</typeparam>
        /// <param name="id">公司ID</param>
        public T IDSelectCompany<T>(Guid id) where T : CompanyModel, new()
        {
            T r;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Company` WHERE `ID` = @pID;", conn);
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = id;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    r = new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        图片ID = (dr["图片ID"] != DBNull.Value ? (Guid?)dr["图片ID"] : null),
                        名称 = dr["名称"].ToString(),
                        拼音 = dr["拼音"].ToString(),
                        发货单分页码 = (int)dr["发货单分页码"],
                        累计金额 = (decimal)dr["累计金额"],
                        赊欠金额 = (decimal)dr["赊欠金额"],
                        赊欠 = Convert.ToBoolean(dr["赊欠"]),
                        税号 = dr["税号"].ToString(),
                        地址 = dr["地址"].ToString(),
                        电话 = dr["电话"].ToString(),
                        银行 = dr["银行"].ToString(),
                        账号 = dr["账号"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    };
                    dr.Close();
                    //conn.Close();
                }
                else
                {
                    dr.Close();
                    //conn.Close();
                    throw new ArgumentOutOfRangeException("Guid id", id, string.Format("无效的Company[ID] '{0}'", id));
                }
            }
            return r;
        }
        /// <summary>
        /// ID查询联系人信息
        /// </summary>
        /// <typeparam name="T">联系人模型</typeparam>
        /// <param name="id">联系人ID</param>
        public T IDSelectContact<T>(Guid id) where T : ContactModel, new()
        {
            T r;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Contact` WHERE `ID` = @pID;", conn);
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = id;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    r = new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        公司ID = (Guid)dr["公司ID"],
                        图片ID = (dr["图片ID"] != DBNull.Value ? (Guid?)dr["图片ID"] : null),
                        快递ID = (dr["快递ID"] != DBNull.Value ? (Guid?)dr["快递ID"] : null),
                        名称 = dr["名称"].ToString(),
                        拼音 = dr["拼音"].ToString(),
                        昵称 = dr["昵称"].ToString(),
                        地址 = dr["地址"].ToString(),
                        手机 = dr["手机"].ToString(),
                        电话 = dr["电话"].ToString(),
                        传真 = dr["传真"].ToString(),
                        性别 = (dr["性别"] != DBNull.Value ? Convert.ToBoolean(dr["性别"]) : new bool?()),
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    };
                    dr.Close();
                    //conn.Close();
                }
                else
                {
                    dr.Close();
                    //conn.Close();
                    throw new ArgumentOutOfRangeException("Guid id", id, string.Format("无效的Contact[ID] '{0}'", id));
                }
            }
            return r;
        }
        /// <summary>
        /// ID查询发货单信息
        /// </summary>
        /// <typeparam name="T">发货单模型</typeparam>
        /// <param name="id">发货单ID</param>
        public T IDSelectInvoice<T>(Guid id) where T : InvoiceModel, new()
        {
            T r;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Invoice` WHERE `ID` = @pID;", conn);
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = id;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    r = new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        公司ID = (Guid)dr["公司ID"],
                        对账单ID = (dr["对账单ID"] != DBNull.Value ? (Guid?)dr["对账单ID"] : null),
                        联系人 = dr["联系人"].ToString(),
                        金额 = (decimal)dr["金额"],
                        总页码 = (int)dr["总页码"],
                        分页码 = (int)dr["分页码"],
                        打印 = Convert.ToBoolean(dr["打印"]),
                        对账 = Convert.ToBoolean(dr["对账"]),
                        付款 = Convert.ToBoolean(dr["付款"]),
                        作废 = Convert.ToBoolean(dr["作废"]),
                        打印时间 = (DateTime)dr["打印时间"],
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    };
                    dr.Close();
                    //conn.Close();
                }
                else
                {
                    dr.Close();
                    //conn.Close();
                    throw new ArgumentOutOfRangeException("Guid id", id, string.Format("无效的Invoice[ID] '{0}'", id));
                }
            }
            return r;
        }
        /// <summary>
        /// ID查询用户信息
        /// </summary>
        /// <typeparam name="T">用户模型</typeparam>
        /// <param name="id">用户ID</param>
        public T IDSelectUser<T>(Guid id) where T : UserModel, new()
        {
            T r;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `User` WHERE `ID` = @pID;", conn);
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = id;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    r = new T()
                    {
                        ID = (Guid)dr["ID"],
                        公司名称 = dr["公司名称"].ToString(),
                        联系人 = dr["联系人"].ToString(),
                        地址 = dr["地址"].ToString(),
                        电话 = dr["电话"].ToString(),
                        传真 = dr["传真"].ToString(),
                        手机 = dr["手机"].ToString(),
                        版本 = dr["版本"].ToString(),
                        默认赊欠 = Convert.ToBoolean(dr["默认赊欠"]),
                        发货单总页码 = (int)dr["发货单总页码"],
                        对账单总页码 = (int)dr["对账单总页码"],
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    };
                    dr.Close();
                    //conn.Close();
                }
                else
                {
                    dr.Close();
                    //conn.Close();
                    throw new ArgumentOutOfRangeException("Guid id", id, string.Format("无效的User[ID] '{0}'", id));
                }
            }
            return r;
        }
        /// <summary>
        /// 其他ID查询发货清单信息
        /// </summary>
        /// <typeparam name="T">发货清单模型</typeparam>
        /// <param name="id">发货单ID或对账单ID</param>
        /// <param name="isInvoiceID">true:发货单ID false:对账单ID</param>
        /// <returns>创建时间`CreateTime`升序,内部备注`Comment`升序</returns>
        public List<T> OtherIDSelectInvoiceList<T>(Guid id, bool isInvoiceID) where T : InvoiceListModel, new()
        {
            string selectString;
            if (isInvoiceID)
                selectString = "SELECT * FROM `InvoiceList` WHERE `发货单ID` = @pID ORDER BY `Comment` ASC;";
            else
                selectString = "SELECT * FROM `InvoiceList` WHERE `发货单ID` IN (SELECT ID FROM `Invoice` WHERE `对账单ID` = @pID) ORDER BY `CreateTime` ASC, `Comment` ASC;";

            List<T> r = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(selectString, conn);
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = id;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    r.Add(new T()
                    {
                        ID = (Guid)dr["ID"],
                        发货单ID = (Guid)dr["发货单ID"],
                        名称 = dr["名称"].ToString(),
                        单位 = dr["单位"].ToString(),
                        数量 = (decimal)dr["数量"],
                        单价 = (decimal)dr["单价"],
                        编号 = dr["编号"].ToString(),
                        规格 = dr["规格"].ToString(),
                        颜色 = dr["颜色"].ToString(),
                        备注 = dr["备注"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    });
                }
                dr.Close();
                //conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 获取全部公司信息
        /// </summary>
        /// <typeparam name="T">公司模型</typeparam>
        /// <returns>更新时间`UpdateTime`降序</returns>
        public List<T> SelectAllCompanies<T>() where T : CompanyModel, new()
        {
            List<T> r = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Company` ORDER BY `UpdateTime` DESC;", conn);
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    r.Add(new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        图片ID = (dr["图片ID"] != DBNull.Value ? (Guid?)dr["图片ID"] : null),
                        名称 = dr["名称"].ToString(),
                        拼音 = dr["拼音"].ToString(),
                        发货单分页码 = (int)dr["发货单分页码"],
                        累计金额 = (decimal)dr["累计金额"],
                        赊欠金额 = (decimal)dr["赊欠金额"],
                        赊欠 = Convert.ToBoolean(dr["赊欠"]),
                        税号 = dr["税号"].ToString(),
                        地址 = dr["地址"].ToString(),
                        电话 = dr["电话"].ToString(),
                        银行 = dr["银行"].ToString(),
                        账号 = dr["账号"].ToString(),
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    });
                }
                dr.Close();
                //conn.Close();
            }
            return r;
        }
        /// <summary>
        /// 获取全部联系人信息
        /// </summary>
        /// <typeparam name="T">联系人模型</typeparam>
        /// <returns>更新时间`UpdateTime`降序</returns>
        public List<T> SelectAllContacts<T>() where T : ContactModel, new()
        {
            List<T> r = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Contact ORDER BY `UpdateTime` DESC;", conn);
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    r.Add(new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        公司ID = (Guid)dr["公司ID"],
                        图片ID = (dr["图片ID"] != DBNull.Value ? (Guid?)dr["图片ID"] : null),
                        快递ID = (dr["快递ID"] != DBNull.Value ? (Guid?)dr["快递ID"] : null),
                        名称 = dr["名称"].ToString(),
                        拼音 = dr["拼音"].ToString(),
                        昵称 = dr["昵称"].ToString(),
                        地址 = dr["地址"].ToString(),
                        手机 = dr["手机"].ToString(),
                        电话 = dr["电话"].ToString(),
                        传真 = dr["传真"].ToString(),
                        性别 = (dr["性别"] != DBNull.Value ? Convert.ToBoolean(dr["性别"]) : new bool?()),
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    });
                }
                dr.Close();
                //conn.Close();
            }
            return r;
        }
        /// <summary>
        /// 获取全部对账单信息
        /// </summary>
        /// <typeparam name="T">对账单模型</typeparam>
        /// <returns>更新时间`UpdateTime`降序</returns>
        public List<T> SelectAllStatement<T>() where T : StatementModel, new()
        {
            List<T> r = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Statement ORDER BY `UpdateTime` DESC;", conn);
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    r.Add(new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        公司ID = (Guid)dr["公司ID"],
                        标题 = dr["标题"].ToString(),
                        起始日期 = (DateTime)dr["起始日期"],
                        截止日期 = (DateTime)dr["截止日期"],
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    });
                }
                dr.Close();
                //conn.Close();
            }
            return r;
        }
        /// <summary>
        /// 获取指定发货单信息
        /// </summary>
        /// <typeparam name="T">发货单模型</typeparam>
        /// <param name="where">只能为(未打印,已打印,已对账,已作废)这四个值中的其中一个</param>
        /// <param name="userID">当前用户ID</param>
        /// <returns>更新时间`UpdateTime`降序</returns>
        public List<T> WhereSelectInvoices<T>(string where, Guid userID) where T : InvoiceModel, new()
        {
            string selectString;
            switch (where)
            {
                case "未打印":
                    selectString = "SELECT * FROM Invoice WHERE `用户ID` = @p用户ID and `打印` = 0 and `对账` = 0 and `作废` = 0 ORDER BY `UpdateTime` DESC;";
                    break;
                case "已打印":
                    selectString = "SELECT * FROM Invoice WHERE `用户ID` = @p用户ID and `打印` = 1 and `对账` = 0 and `作废` = 0 ORDER BY `UpdateTime` DESC;";
                    break;
                case "已对账":
                    selectString = "SELECT * FROM Invoice WHERE `用户ID` = @p用户ID and `对账` = 1 and `作废` = 0 ORDER BY `UpdateTime` DESC;";
                    break;
                case "已作废":
                    selectString = "SELECT * FROM Invoice WHERE `用户ID` = @p用户ID and `作废` = 1 ORDER BY `UpdateTime` DESC;";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("where", where, "只能为(未打印,已打印,已对账,已作废)这四个值中的其中一个!");
            }
            List<T> r = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(selectString, conn);
                MySqlParameter p0 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
                cmd.Parameters.Add(p0);
                p0.Value = userID;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    r.Add(new T()
                    {
                        ID = (Guid)dr["ID"],
                        用户ID = (Guid)dr["用户ID"],
                        公司ID = (Guid)dr["公司ID"],
                        对账单ID = (dr["对账单ID"] != DBNull.Value ? (Guid?)dr["对账单ID"] : null),
                        联系人 = dr["联系人"].ToString(),
                        金额 = (decimal)dr["金额"],
                        总页码 = (int)dr["总页码"],
                        分页码 = (int)dr["分页码"],
                        打印 = Convert.ToBoolean(dr["打印"]),
                        对账 = Convert.ToBoolean(dr["对账"]),
                        付款 = Convert.ToBoolean(dr["付款"]),
                        作废 = Convert.ToBoolean(dr["作废"]),
                        打印时间 = (DateTime)dr["打印时间"],
                        Comment = dr["Comment"].ToString(),
                        CreateTime = (DateTime)dr["CreateTime"],
                        UpdateTime = (DateTime)dr["UpdateTime"]
                    });
                }
                dr.Close();
                //conn.Close();
            }
            return r;
        }


        /// <summary>
        /// 保存公司信息
        /// </summary>
        /// <typeparam name="T">公司模型</typeparam>
        /// <param name="companyInfo">新建用到(用户ID, 图片ID, 名称, 拼音, 赊欠, 税号, 地址, 电话, 银行, 账号, Comment)更新还用的(ID, 发货单分页码, 累计金额, 赊欠金额)</param>
        /// <param name="isNewInfo">true:新建 false:更新</param>
        public void SaveCompany<T>(T companyInfo, bool isNewInfo) where T : CompanyModel
        {
            #region------验证------
            if (companyInfo == null || companyInfo.名称 == null)
            {
                throw new ArgumentNullException("名称", "[名称]不能为空!");
            }
            else
            {
                companyInfo.名称 = companyInfo.名称.Trim();
                if (companyInfo.名称.Length == 0)
                    throw new ArgumentNullException("名称", "[名称]不能为空!");
                else if (companyInfo.名称.Length > 64)
                    throw new ArgumentOutOfRangeException("名称", "[名称]大于64字符");
            }
            if (companyInfo.拼音 != null)
            {
                companyInfo.拼音 = companyInfo.拼音.Trim();
                if (companyInfo.拼音.Length > 256)
                    companyInfo.拼音 = companyInfo.拼音.Substring(0, 256);
            }
            if (companyInfo.税号 != null)
            {
                companyInfo.税号 = companyInfo.税号.Trim();
                if (companyInfo.税号.Length > 32)
                    throw new ArgumentOutOfRangeException("税号", "[税号]大于32字符");
            }
            if (companyInfo.地址 != null)
            {
                companyInfo.地址 = companyInfo.地址.Trim();
                if (companyInfo.地址.Length > 128)
                    throw new ArgumentOutOfRangeException("地址", "[地址]大于128字符");
            }
            if (companyInfo.电话 != null)
            {
                companyInfo.电话 = companyInfo.电话.Trim();
                if (companyInfo.电话.Length > 32)
                    throw new ArgumentOutOfRangeException("电话", "[电话]大于32字符");
            }
            if (companyInfo.银行 != null)
            {
                companyInfo.银行 = companyInfo.银行.Trim();
                if (companyInfo.银行.Length > 64)
                    throw new ArgumentOutOfRangeException("银行", "[银行]大于64字符");
            }
            if (companyInfo.账号 != null)
            {
                companyInfo.账号 = companyInfo.账号.Trim();
                if (companyInfo.账号.Length > 32)
                    throw new ArgumentOutOfRangeException("账号", "[账号]大于32字符");
            }
            if (companyInfo.Comment != null)
            {
                companyInfo.Comment = companyInfo.Comment.Trim();
                if (companyInfo.Comment.Length > 128)
                    throw new ArgumentOutOfRangeException("Comment", "[备注]大于128字符");
            }
            #endregion------验证------

            if (isNewInfo)
                companyInfo.ID = Guid.NewGuid();

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SaveCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("@pID", MySqlDbType.Guid) { Value = companyInfo.ID });

                cmd.Parameters.Add(new MySqlParameter("@p用户ID", MySqlDbType.Guid) { Value = companyInfo.用户ID });

                cmd.Parameters.Add(new MySqlParameter("@p图片ID", MySqlDbType.VarChar, 36) { Value = companyInfo.图片ID });

                cmd.Parameters.Add(new MySqlParameter("@p名称", MySqlDbType.VarChar, 64) { Value = companyInfo.名称 });

                cmd.Parameters.Add(new MySqlParameter("@p拼音", MySqlDbType.VarChar, 256) { Value = companyInfo.拼音 });

                cmd.Parameters.Add(new MySqlParameter("@p发货单分页码", MySqlDbType.Int32) { Value = companyInfo.发货单分页码 });

                cmd.Parameters.Add(new MySqlParameter("@p累计金额", MySqlDbType.Decimal) { Value = companyInfo.累计金额 });

                cmd.Parameters.Add(new MySqlParameter("@p赊欠金额", MySqlDbType.Decimal) { Value = companyInfo.赊欠金额 });

                cmd.Parameters.Add(new MySqlParameter("@p赊欠", MySqlDbType.Bit) { Value = companyInfo.赊欠 });

                cmd.Parameters.Add(new MySqlParameter("@p税号", MySqlDbType.VarChar, 32) { Value = companyInfo.税号 });

                cmd.Parameters.Add(new MySqlParameter("@p地址", MySqlDbType.VarChar, 128) { Value = companyInfo.地址 });

                cmd.Parameters.Add(new MySqlParameter("@p电话", MySqlDbType.VarChar, 32) { Value = companyInfo.电话 });

                cmd.Parameters.Add(new MySqlParameter("@p银行", MySqlDbType.VarChar, 64) { Value = companyInfo.银行 });

                cmd.Parameters.Add(new MySqlParameter("@p账号", MySqlDbType.VarChar, 32) { Value = companyInfo.账号 });

                cmd.Parameters.Add(new MySqlParameter("@pComment", MySqlDbType.VarChar, 128) { Value = companyInfo.Comment });

                cmd.Parameters.Add(new MySqlParameter("@pIsNewInfo", MySqlDbType.Bit) { Value = isNewInfo });

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                if (i != 1)
                {
                    throw new DBConcurrencyException(string.Format("共保存了 {0} 条记录!", i));
                }
            }
        }
        /// <summary>
        /// 保存联系人信息
        /// </summary>
        /// <typeparam name="T">联系人模型</typeparam>
        /// <param name="contactInfo">用到(ID, 用户ID, 公司ID, 图片ID, 名称, 拼音, 昵称, 地址, 手机, 电话, 传真, 性别, Comment)</param>
        /// <param name="isNewInfo">true:新建 false:更新</param>
        public void SaveContact<T>(T contactInfo, bool isNewInfo) where T : ContactModel
        {
            #region------验证------
            if (contactInfo == null || contactInfo.名称 == null)
            {
                throw new ArgumentNullException("名称", "[名称]不能为空!");
            }
            else
            {
                contactInfo.名称 = contactInfo.名称.Trim();
                if (contactInfo.名称.Length == 0)
                    throw new ArgumentNullException("名称", "[名称]不能为空!");
                else if (contactInfo.名称.Length > 64)
                    throw new ArgumentOutOfRangeException("名称", "[名称]大于64字符");
            }
            if (contactInfo.拼音 != null)
            {
                contactInfo.拼音 = contactInfo.拼音.Trim();
                if (contactInfo.拼音.Length > 256)
                    contactInfo.拼音 = contactInfo.拼音.Substring(0, 256);
            }
            if (contactInfo.昵称 != null)
            {
                contactInfo.昵称 = contactInfo.昵称.Trim();
                if (contactInfo.昵称.Length > 64)
                    throw new ArgumentOutOfRangeException("昵称", "[昵称]大于64字符");
            }
            if (contactInfo.地址 != null)
            {
                contactInfo.地址 = contactInfo.地址.Trim();
                if (contactInfo.地址.Length > 128)
                    throw new ArgumentOutOfRangeException("地址", "[地址]大于128字符");
            }
            if (contactInfo.手机 != null)
            {
                contactInfo.手机 = contactInfo.手机.Trim();
                if (contactInfo.手机.Length > 32)
                    throw new ArgumentOutOfRangeException("手机", "[手机]大于32字符");
            }
            if (contactInfo.电话 != null)
            {
                contactInfo.电话 = contactInfo.电话.Trim();
                if (contactInfo.电话.Length > 32)
                    throw new ArgumentOutOfRangeException("电话", "[电话]大于32字符");
            }
            if (contactInfo.传真 != null)
            {
                contactInfo.传真 = contactInfo.传真.Trim();
                if (contactInfo.传真.Length > 32)
                    throw new ArgumentOutOfRangeException("传真", "[传真]大于32字符");
            }
            if (contactInfo.Comment != null)
            {
                contactInfo.Comment = contactInfo.Comment.Trim();
                if (contactInfo.Comment.Length > 128)
                    throw new ArgumentOutOfRangeException("Comment", "[备注]大于128字符");
            }
            #endregion------验证------
            if (isNewInfo)
                contactInfo.ID = Guid.NewGuid();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SaveContact", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("@pID", MySqlDbType.Guid) { Value = contactInfo.ID });

                cmd.Parameters.Add(new MySqlParameter("@p用户ID", MySqlDbType.Guid) { Value = contactInfo.用户ID });

                cmd.Parameters.Add(new MySqlParameter("@p公司ID", MySqlDbType.Guid) { Value = contactInfo.公司ID });

                cmd.Parameters.Add(new MySqlParameter("@p图片ID", MySqlDbType.VarChar, 36) { Value = contactInfo.图片ID });

                cmd.Parameters.Add(new MySqlParameter("@p名称", MySqlDbType.VarChar, 64) { Value = contactInfo.名称 });

                cmd.Parameters.Add(new MySqlParameter("@p拼音", MySqlDbType.VarChar, 256) { Value = contactInfo.拼音 });

                cmd.Parameters.Add(new MySqlParameter("@p昵称", MySqlDbType.VarChar, 64) { Value = contactInfo.昵称 });

                cmd.Parameters.Add(new MySqlParameter("@p地址", MySqlDbType.VarChar, 128) { Value = contactInfo.地址 });

                cmd.Parameters.Add(new MySqlParameter("@p手机", MySqlDbType.VarChar, 32) { Value = contactInfo.手机 });

                cmd.Parameters.Add(new MySqlParameter("@p电话", MySqlDbType.VarChar, 32) { Value = contactInfo.电话 });

                cmd.Parameters.Add(new MySqlParameter("@p传真", MySqlDbType.VarChar, 32) { Value = contactInfo.传真 });

                cmd.Parameters.Add(new MySqlParameter("@p性别", MySqlDbType.Bit) { Value = contactInfo.性别 });

                cmd.Parameters.Add(new MySqlParameter("@pComment", MySqlDbType.VarChar, 128) { Value = contactInfo.Comment });

                cmd.Parameters.Add(new MySqlParameter("@pIsNewInfo", MySqlDbType.Bit) { Value = isNewInfo });

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                if (i != 1)
                {
                    throw new DBConcurrencyException(string.Format("共保存了 {0} 条记录!", i));
                }
            }
        }


        /// <summary>
        /// 保存发货单及对应清单
        /// </summary>
        /// <param name="invoiceInfo">用到(ID, 用户ID, 公司ID, 联系人, Comment)</param>
        /// <param name="invoiceListInfos"></param>
        /// <param name="isUpdate">false:新建,true:更新</param>
        public void SaveInvoice<TInvoice, TInvoiceList>(TInvoice invoiceInfo, List<TInvoiceList> invoiceListInfos, bool isUpdate) where TInvoice : InvoiceModel where TInvoiceList : InvoiceListModel
        {
            if (invoiceInfo == null)
                throw new ArgumentNullException("InvoiceInfo", "[InvoiceInfo]不能为空!");
            if (invoiceListInfos == null || invoiceListInfos.Count == 0)
                throw new ArgumentNullException("InvoiceListInfos", "[InvoiceListInfos]不能为空!");

            if (isUpdate)
            {   //更新
                if (invoiceInfo.ID == Guid.Empty)
                    throw new ArgumentNullException("ID", "[ID]不能为空!");
            }
            else
            {   //新建
                invoiceInfo.ID = Guid.NewGuid();
            }
            if (invoiceInfo.联系人 != null)
            {
                invoiceInfo.联系人 = invoiceInfo.联系人.Trim();
                if (invoiceInfo.联系人.Length > 64)
                    throw new ArgumentOutOfRangeException("联系人", "[联系人]大于64字符");
            }
            if (invoiceInfo.Comment != null)
            {
                invoiceInfo.Comment = invoiceInfo.Comment.Trim();
                if (invoiceInfo.Comment.Length > 128)
                    throw new ArgumentOutOfRangeException("Comment", "[备注]大于128字符");
            }

            int i = 0;
            decimal money = 0;
            foreach (var item in invoiceListInfos)
            {
                money += item.数量 * item.单价;
                if (item.名称 != null)
                {
                    item.名称 = item.名称.Trim();
                    if (item.名称.Length == 0)
                        throw new ArgumentNullException("名称", "[清单名称]不能为空!");
                    else if (item.名称.Length > 64)
                        throw new ArgumentOutOfRangeException("名称", "[清单名称]大于64字符");
                }
                else
                    throw new ArgumentNullException("名称", "[清单名称]不能为空!");
                if (item.单位 != null)
                {
                    item.单位 = item.单位.Trim();
                    if (item.单位.Length > 16)
                        throw new ArgumentOutOfRangeException("单位", "[清单单位]大于16字符");
                }
                else
                    item.单位 = "pcs";
                if (item.编号 != null)
                {
                    item.编号 = item.编号.Trim();
                    if (item.编号.Length > 32)
                        throw new ArgumentOutOfRangeException("编号", "[清单编号]大于32字符");
                }
                if (item.规格 != null)
                {
                    item.规格 = item.规格.Trim();
                    if (item.规格.Length > 32)
                        throw new ArgumentOutOfRangeException("规格", "[清单规格]大于32字符");
                }
                if (item.颜色 != null)
                {
                    item.颜色 = item.颜色.Trim();
                    if (item.颜色.Length > 32)
                        throw new ArgumentOutOfRangeException("颜色", "[清单颜色]大于32字符");
                }
                if (item.备注 != null)
                {
                    item.备注 = item.备注.Trim();
                    if (item.备注.Length > 32)
                        throw new ArgumentOutOfRangeException("备注", "[清单备注]大于32字符");
                }
                if (item.Comment != null)
                {
                    item.Comment = i.ToString("00") + item.Comment.Trim();
                    if (item.Comment.Length > 128)
                        throw new ArgumentOutOfRangeException("Comment", "[清单隐藏备注]大于128字符");
                }
                else
                    item.Comment = i.ToString("00");
                i++;
            }
            invoiceInfo.金额 = money;
            invoiceInfo.打印时间 = DateTime.Now;

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd;
                if (isUpdate)//更新
                    cmd = new MySqlCommand("UpdateInvoice", conn);
                else//新建
                    cmd = new MySqlCommand("InsertInvoice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                MySqlParameter p1 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
                MySqlParameter p2 = new MySqlParameter("@p公司ID", MySqlDbType.Guid);
                MySqlParameter p3 = new MySqlParameter("@p联系人", MySqlDbType.VarChar, 64);
                MySqlParameter p4 = new MySqlParameter("@p金额", MySqlDbType.Decimal);
                MySqlParameter p5 = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
                cmd.Parameters.Add(p0);
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                p0.Value = invoiceInfo.ID;
                p1.Value = invoiceInfo.用户ID;
                p2.Value = invoiceInfo.公司ID;
                p3.Value = invoiceInfo.联系人;
                p4.Value = invoiceInfo.金额;
                p5.Value = invoiceInfo.Comment;
                conn.Open();
                cmd.ExecuteNonQuery();
                //conn.Close();

                if (isUpdate)
                {   //更新 删除原始清单
                    MySqlCommand cmd3 = new MySqlCommand("DELETE FROM `InvoiceList` WHERE `发货单ID` = @p发货单ID;", conn);
                    MySqlParameter ppp0 = new MySqlParameter("@p发货单ID", MySqlDbType.Guid);
                    cmd3.Parameters.Add(ppp0);
                    ppp0.Value = invoiceInfo.ID;
                    //conn.Open();
                    cmd3.ExecuteNonQuery();
                    //conn.Close();
                }

                MySqlCommand cmd2 = new MySqlCommand("InsertInvoiceList", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                MySqlParameter pp0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                MySqlParameter pp1 = new MySqlParameter("@p发货单ID", MySqlDbType.Guid);
                MySqlParameter pp2 = new MySqlParameter("@p名称", MySqlDbType.VarChar, 64);
                MySqlParameter pp3 = new MySqlParameter("@p单位", MySqlDbType.VarChar, 16);
                MySqlParameter pp4 = new MySqlParameter("@p数量", MySqlDbType.Decimal);
                MySqlParameter pp5 = new MySqlParameter("@p单价", MySqlDbType.Decimal);
                MySqlParameter pp6 = new MySqlParameter("@p编号", MySqlDbType.VarChar, 32);
                MySqlParameter pp7 = new MySqlParameter("@p规格", MySqlDbType.VarChar, 32);
                MySqlParameter pp8 = new MySqlParameter("@p颜色", MySqlDbType.VarChar, 32);
                MySqlParameter pp9 = new MySqlParameter("@p备注", MySqlDbType.VarChar, 32);
                MySqlParameter pp10 = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
                cmd2.Parameters.Add(pp0);
                cmd2.Parameters.Add(pp1);
                cmd2.Parameters.Add(pp2);
                cmd2.Parameters.Add(pp3);
                cmd2.Parameters.Add(pp4);
                cmd2.Parameters.Add(pp5);
                cmd2.Parameters.Add(pp6);
                cmd2.Parameters.Add(pp7);
                cmd2.Parameters.Add(pp8);
                cmd2.Parameters.Add(pp9);
                cmd2.Parameters.Add(pp10);

                pp1.Value = invoiceInfo.ID;
                //conn.Open();
                foreach (var item in invoiceListInfos)
                {
                    pp0.Value = Guid.NewGuid();
                    pp2.Value = item.名称;
                    pp3.Value = item.单位;
                    pp4.Value = item.数量;
                    pp5.Value = item.单价;
                    pp6.Value = item.编号;
                    pp7.Value = item.规格;
                    pp8.Value = item.颜色;
                    pp9.Value = item.备注;
                    pp10.Value = item.Comment;
                    cmd2.ExecuteNonQuery();
                }
                //conn.Close();
            }
        }
        /// <summary>
        /// 保存对账单及相关信息
        /// </summary>
        /// <param name="statementInfo">请提供正确的[用户ID]</param>
        /// <param name="invoiceInfos"></param>
        public void SaveStatement<T>(StatementModel statementInfo, List<T> invoiceInfos) where T : InvoiceModel
        {
            Guid CompanyID;
            if (invoiceInfos == null || invoiceInfos.Count == 0)
            {
                throw new ArgumentNullException("List<T> InvoiceInfos", "发货单空");
            }
            if (!invoiceInfos.All(_func => _func.打印))
            {
                throw new ArgumentOutOfRangeException("List<T> InvoiceInfos", invoiceInfos, "含未打印");
            }
            if (!invoiceInfos.All(_func => !_func.对账))
            {
                throw new ArgumentOutOfRangeException("List<T> InvoiceInfos", invoiceInfos, "含已对账");
            }
            if (!invoiceInfos.All(_func => !_func.作废))
            {
                throw new ArgumentOutOfRangeException("List<T> InvoiceInfos", invoiceInfos, "含已作废");
            }
            CompanyID = invoiceInfos[0].公司ID;
            if (!invoiceInfos.All(_func => _func.公司ID == CompanyID))
            {
                throw new ArgumentOutOfRangeException("List<T> InvoiceInfos", invoiceInfos, "公司不同");
            }
            //按打印时间排序
            invoiceInfos = invoiceInfos.OrderBy(_func => _func.打印时间).ToList();
            statementInfo.ID = Guid.NewGuid();
            statementInfo.公司ID = CompanyID;
            statementInfo.起始日期 = invoiceInfos.First().打印时间;
            statementInfo.截止日期 = invoiceInfos.Last().打印时间;
            statementInfo.标题 = string.Format("{0}对账单 {1}-{2}", Other.ShortCompanyName(IDSelectCompany<CompanyModel>(CompanyID).名称), statementInfo.起始日期.ToShortDateString().Replace("-", "").Replace("/", ""), statementInfo.截止日期.ToShortDateString().Replace("-", "").Replace("/", ""));

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("InsertStatement", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                MySqlParameter p1 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
                MySqlParameter p2 = new MySqlParameter("@p公司ID", MySqlDbType.Guid);
                MySqlParameter p3 = new MySqlParameter("@p标题", MySqlDbType.VarChar, 64);
                MySqlParameter p4 = new MySqlParameter("@p起始日期", MySqlDbType.DateTime);
                MySqlParameter p5 = new MySqlParameter("@p截止日期", MySqlDbType.DateTime);
                MySqlParameter p6 = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
                cmd.Parameters.Add(p0);
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                cmd.Parameters.Add(p6);
                p0.Value = statementInfo.ID;
                p1.Value = statementInfo.用户ID;
                p2.Value = statementInfo.公司ID;
                p3.Value = statementInfo.标题;
                p4.Value = statementInfo.起始日期;
                p5.Value = statementInfo.截止日期;
                if (statementInfo.Comment != null)
                    p6.Value = statementInfo.Comment;
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                if (i != 1)
                {
                    throw new DBConcurrencyException(string.Format("共插入了 {0} 条记录!", i));
                }

                MySqlCommand cmd2 = new MySqlCommand("UPDATE `Invoice` SET `对账单ID` = @p对账单ID, `对账` = b'1' WHERE `ID` = @pID;", conn);
                MySqlParameter pp0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                MySqlParameter pp1 = new MySqlParameter("@p对账单ID", MySqlDbType.Guid);
                cmd2.Parameters.Add(pp0);
                cmd2.Parameters.Add(pp1);
                pp1.Value = statementInfo.ID;
                i = 0;
                //conn.Open();
                foreach (var item in invoiceInfos)
                {
                    pp0.Value = item.ID;
                    i += cmd2.ExecuteNonQuery();
                }
                //conn.Close();
                if (i != invoiceInfos.Count)
                {
                    throw new DBConcurrencyException(string.Format("共 {0} 条记录,只更新了 {1} 条记录!", invoiceInfos.Count, i));
                }
            }
        }
        /// <summary>
        /// 发货单标记为打印,并返回打印的必要信息
        /// </summary>
        /// <param name="id">发货单ID</param>
        /// <param name="payment">true:已付款,false:未付款</param>
        /// <returns></returns>
        public InvoiceModelPrint PrintInvoice(Guid id, bool payment)
        {
            InvoiceModelPrint r;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("PrintInvoice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
                MySqlParameter p1 = new MySqlParameter("@p付款", MySqlDbType.Bit);
                cmd.Parameters.Add(p0);
                cmd.Parameters.Add(p1);
                p0.Value = id;
                p1.Value = payment;
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    r = new InvoiceModelPrint()
                    {
                        联系人 = dr["联系人"].ToString(),
                        金额 = (decimal)dr["金额"],
                        总页码 = (int)dr["总页码"],
                        分页码 = (int)dr["分页码"],
                        打印时间 = (DateTime)dr["打印时间"],
                        Comment = dr["Comment"].ToString(),
                        公司名称 = dr["名称"].ToString(),
                        赊欠金额 = (decimal)dr["赊欠金额"],
                    };
                    dr.Close();
                    //conn.Close();
                }
                else
                {
                    dr.Close();
                    //conn.Close();
                    throw new ArgumentOutOfRangeException("Guid id", id, string.Format("无效的Invoice[ID] '{0}'", id));
                }
            }
            return r;
        }

        /// <summary>
        /// 导出对账单
        /// </summary>
        /// <param name="statementInfo">对账单信息用到(ID, 名称)</param>
        public string ExportStatement(StatementModel statementInfo)
        {
            string r;
            List<InvoiceListModel> InvoiceLists = OtherIDSelectInvoiceList<InvoiceListModel>(statementInfo.ID, false);
            //InvoiceLists = InvoiceLists.OrderBy(_func => _func.CreateTime).ThenBy(_func => _func.Comment).ToList();
            r = statementInfo.标题;
            r += "\r\n日期\t款号\t名称 颜色\t规格\t数量\t单位\t单价\t金额\t备注\r\n";
            int i = 2;
            foreach (var item in InvoiceLists)
            {
                i++;
                r += string.Format("{0}\t{1}\t{2} {3}\t{4}\t{5}\t{6}\t{7}\t=E{9}*G{9}\t{8}\r\n", item.CreateTime.ToShortDateString(), item.编号, item.名称, item.颜色, item.规格, item.数量, item.单位, item.单价, item.备注, i);
            }
            r += string.Format("\t\t\t\t\t\t合计:\t=SUM(H3:H{0})\t", i);
            return r;
        }
        /// <summary>
        /// 导入公司信息
        /// 名称 不能为空
        /// 拼音 自动根据 名称 生成首字母+空格+拼音 字符串
        /// </summary>
        /// <param name="CompanyInfos">CompanyModel或其派生类</param>
        /// <param name="UserID">用户ID</param>
        /// <returns>成功导入的公司数量</returns>
        public int ImportCompanyInfos<T>(List<T> companyInfos, Guid userID, bool credit) where T : CompanyModel
        {
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            MySqlCommand cmd2 = new MySqlCommand("InsertCompany", conn);
            cmd2.CommandType = CommandType.StoredProcedure;
            MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.Guid);
            MySqlParameter p1 = new MySqlParameter("@p用户ID", MySqlDbType.Guid);
            MySqlParameter p2 = new MySqlParameter("@p图片ID", MySqlDbType.VarChar, 36);
            MySqlParameter p3 = new MySqlParameter("@p名称", MySqlDbType.VarChar, 64);
            MySqlParameter p4 = new MySqlParameter("@p拼音", MySqlDbType.VarChar, 256);
            MySqlParameter p5 = new MySqlParameter("@p赊欠", MySqlDbType.Bit);
            MySqlParameter p6 = new MySqlParameter("@p税号", MySqlDbType.VarChar, 32);
            MySqlParameter p7 = new MySqlParameter("@p地址", MySqlDbType.VarChar, 128);
            MySqlParameter p8 = new MySqlParameter("@p电话", MySqlDbType.VarChar, 32);
            MySqlParameter p9 = new MySqlParameter("@p银行", MySqlDbType.VarChar, 64);
            MySqlParameter pa = new MySqlParameter("@p账号", MySqlDbType.VarChar, 32);
            MySqlParameter pb = new MySqlParameter("@pComment", MySqlDbType.VarChar, 128);
            cmd2.Parameters.Add(p0);
            cmd2.Parameters.Add(p1);
            cmd2.Parameters.Add(p2);
            cmd2.Parameters.Add(p3);
            cmd2.Parameters.Add(p4);
            cmd2.Parameters.Add(p5);
            cmd2.Parameters.Add(p6);
            cmd2.Parameters.Add(p7);
            cmd2.Parameters.Add(p8);
            cmd2.Parameters.Add(p9);
            cmd2.Parameters.Add(pa);
            cmd2.Parameters.Add(pb);

            p1.Value = userID;
            p2.Value = null;
            p5.Value = credit;
            pb.Value = null;

            int r = 0;
            conn.Open();
            foreach (var item in companyInfos)
            {
                //不能为空字段
                if (item.名称 == null || item.名称.Length == 0) continue;

                p0.Value = Guid.NewGuid();
                p3.Value = item.名称.Trim();
                p4.Value = item.拼音.Trim();
                if (item.税号 != null)
                    p6.Value = item.税号.Trim();
                if (item.地址 != null)
                    p7.Value = item.地址.Trim();
                if (item.电话 != null)
                    p8.Value = item.电话.Trim();
                if (item.银行 != null)
                    p9.Value = item.银行.Trim();
                if (item.账号 != null)
                    pa.Value = item.账号.Trim();
                r += cmd2.ExecuteNonQuery();
            }
            conn.Close();
            return r;
        }



        public void InvalidInvoice()
        {
            //CompanyModel r;
            //using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            //{
            //    MySqlCommand cmd = new MySqlCommand("SELECT count(*) FROM `Company` WHERE `ID` = @pID ;", conn);
            //    MySqlParameter p0 = new MySqlParameter("@pID", MySqlDbType.VarChar, 256);
            //    cmd.Parameters.Add(p0);
            //    p0.Value = " 00000000-0000-0000-0000-000000000000 ";
            //    conn.Open();
            //    MySqlDataReader dr = cmd.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        string  i = dr[0].ToString();

            //    }
            //    dr.Close();
            //    conn.Close();
            //}
        }
    }
}