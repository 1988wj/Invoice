using System;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Test_Invoice
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            bool createDB, createBase;
            string connectionString, database;
            createDB = createBase = false;
            #region ------检测并载入配置文件中的连接字符串------
            try
            {
                XElement _XE = XElement.Load(Global.ConfigPath);
                try
                {
                    database = _XE.Element("Database").Value.Trim();
                    if (database.Length == 0)
                        throw new NullReferenceException();
                }
                catch (Exception)
                {
                    throw new NullReferenceException("Database");
                }
                try
                {
                    connectionString = _XE.Element("ConnectionString").Value.Trim();
                    if (connectionString.Length == 0)
                        throw new NullReferenceException();
                }
                catch (System.NullReferenceException)
                {
                    throw new NullReferenceException("ConnectionString");
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "未找到配置文件\"\\Infos\\Config.xml\"", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            catch (System.Xml.XmlException e)
            {
                MessageBox.Show(e.Message, "配置文件\"\\Infos\\Config.xml\"错误!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            catch (System.NullReferenceException e)
            {
                string _errorString = "请在\"\\Infos\\Config.xml\"中配置正确的连接字符串和数据库名\r\n如下:\r\n<Infos>\r\n    <Database>[数据库名]</Database>\r\n    <ConnectionString>\r\n        Data Source = [数据库IP地址];\r\n        User ID = [数据库用户名];\r\n        Password = [数据库密码];\r\n        CharSet = utf8;\r\n    </ConnectionString>\r\n--其他模板配置信息请勿改动--\r\n</Infos> ";
                MessageBox.Show(_errorString, string.Format("配置文件中标记<{0} />错误!", e.Message), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "\"\\Infos\\Config.xml\"未分类错误!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            #endregion ------检测并载入配置文件中的连接字符串------

            #region ------检测并实例化数据库类------
            try
            {
                Global.SQL = new MySQL(connectionString, database);
            }
            catch (System.Data.DataException e)
            {
                switch (e.Message)
                {
                    case "无数据库":
                        createBase = true;
                        break;
                    case "无数据表":
                        createBase = false;
                        break;
                    default:
                        Application.Current.Shutdown();
                        return;
                }
                if (MessageBoxResult.Yes == MessageBox.Show("是否初始化数据库?", e.Message, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly))
                {
                    createDB = true;
                }
                else
                {
                    Application.Current.Shutdown();
                    return;
                }
            }
            catch (System.FormatException e)
            {
                MessageBox.Show(string.Format("请检查\"数据库\"和\"连接字符串\"是否配置正确!\r\n以下是连接失败返回的错误信息:\r\n{0}", e.Message), "无法连接到数据库!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "\"SQL\"未分类错误!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
                return;
            }
            #endregion ------检测并实例化数据库类------

            #region------创建数据库并实例化数据库类------
            if (createDB)
            {
                try
                {
                    Global.SQL = new MySQL(connectionString, database, createBase);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "\"创建数据库\"未分类错误!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    Application.Current.Shutdown();
                    return;
                }

            }
            #endregion------创建数据库并实例化数据库类------

            //------载入数据------
            Global.Initialize();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            e.Handled = true;
        }
    }
}
