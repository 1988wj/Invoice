using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Test_Invoice
{
    /// <summary>
    /// 生成智能列表的行信息
    /// </summary>
    public class SmartInfo
    {
        /// <summary>
        /// 智能显示文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 智能检索拼音及字母
        /// </summary>
        public string Letter { get; set; }
        ///// <summary>
        ///// 显示排序权重
        ///// </summary>
        //public int Weight { get; set; }
        ///// <summary>
        ///// 最后更新时间
        ///// </summary>
        //public DateTime UpdateDate { get; set; }
    }
    /// <summary>
    /// 绑定在显示智能列表ListBox的Tag上
    /// </summary>
    class SmartTag
    {
        /// <summary>
        /// 当前焦点所在TextBox
        /// </summary>
        public TextBox _TB { get; set; }
        /// <summary>
        /// 标记(Mark)对应的列表
        /// </summary>
        public List<SmartInfo> _List { get; set; }
        /// <summary>
        /// 记录原始文本(Text),用于判断是否更改过
        /// </summary>
        public string _Text { get; set; }
    }
    /// <summary>
    /// Window类添加Smart功能
    /// </summary>
    public abstract class SmartWindow : Window
    {
        Grid BodyGrid;
        ListBox SmartLB;
        List<SmartInfo> CompanyNameList;
        /// <summary>
        /// 继承此类必须在初始化时调用此函数
        /// </summary>
        /// <param name="bodyGrid">主Grid</param>
        public void InitializeSmart(Grid bodyGrid)
        {
            BodyGrid = bodyGrid;

            SmartLB = new ListBox()
            {
                IsTabStop = false,
                MaxHeight = 200,
                MinWidth = 88,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Hidden
            };
            //SmartLB光标独立依赖属性必须设置
            FocusManager.SetIsFocusScope(SmartLB, true);
            SmartLB.PreviewKeyDown += SmartLB_PreviewKeyDown;
            SmartLB.PreviewMouseDoubleClick += SmartLB_PreviewMouseDoubleClick;
            BodyGrid.Children.Add(SmartLB);

            CompanyNameList = Global.AllCompanies.Select(_func => new SmartInfo()
            {
                Text = _func.名称,
                Letter = _func.拼音
            }).ToList();
        }

        private void SmartLB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.LBKeyDown(SmartLB, e); }
        private void SmartLB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) { SmartList.FillText(SmartLB); }
        /// <summary>
        /// 文本框加载完成->
        /// 自动获取焦点
        /// </summary>
        protected void SmartTB_Loaded(object sender, RoutedEventArgs e) { TextBox _TB = sender as TextBox; if (null != _TB) { _TB.Focus(); _TB.SelectionStart = _TB.Text.Length; } }
        /// <summary>
        /// 文本框获得逻辑焦点->
        /// 生成文本框Tag对应标记(Mark)的SmartInfo列表
        /// </summary>
        protected void SmartTB_GotFocus(object sender, RoutedEventArgs e) { SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyGrid, true); }
        /// <summary>
        /// 文本框丢失逻辑焦点->
        /// 更新文本框Tag对应标记(Mark)的权重及时间
        /// </summary>
        protected void SmartTB_LostFocus(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, true); }
        /// <summary>
        /// 文本框丢失逻辑焦点->
        /// 不更新(适用于用户给定SmartInfo列表)
        /// </summary>
        protected void SmartTB_LostFocusNotUpdate(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, false); }
        /// <summary>
        /// 文本框有键盘键按下->
        /// 上下键,逻辑焦点给SmartLB
        /// </summary>
        protected void SmartTB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.TBKeyDown(SmartLB, e); }
        /// <summary>
        /// 文本框内容改变->
        /// 筛选SmartInfo列表显示到SmartLB
        /// </summary>
        protected void SmartTB_TextChanged(object sender, TextChangedEventArgs e) { SmartList.TBChanged(SmartLB); }
        /// <summary>
        /// 文本框获得逻辑焦点->
        /// comNameTB专用
        /// </summary>
        protected void comNameTB_GotFocus(object sender, RoutedEventArgs e) { SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyGrid, true, CompanyNameList); }
        ///// <summary>
        ///// 文本框获得逻辑焦点->
        ///// conNameTB专用
        ///// </summary>
        //protected void conNameTB_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string keyString = comNameTB.Text.Trim();
        //        Guid keyID;
        //        if (keyString.Length == 0)//公司名称为空,检索个人客户
        //        {
        //            keyID = Guid.Empty;
        //        }
        //        else
        //        {
        //            CompanyModel _a = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == keyString);
        //            if (_a != null)//找到公司ID,检索单位所属客户
        //            {
        //                keyID = _a.ID;

        //            }
        //            else//找不到公司ID,直接返回
        //            {
        //                return;
        //            }
        //        }
        //        List<SmartInfo> smartList = Global.AllContacts.Where(_func => _func.公司ID == keyID).Select(_func => new SmartInfo()
        //        {
        //            Text = _func.名称,
        //            Letter = _func.拼音
        //        }).ToList();

        //        SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true, smartList);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("标记(Mark)错误!");
        //        return;
        //    }
        //}
    }
    /// <summary>
    /// Page类添加Smart功能
    /// </summary>
    public abstract class SmartPage : Page
    {
        Grid BodyGrid;
        ListBox SmartLB;
        List<SmartInfo> CompanyNameList;
        /// <summary>
        /// 继承此类必须在初始化时调用此函数
        /// </summary>
        /// <param name="bodyGrid">主Grid</param>
        public void InitializeSmart(Grid bodyGrid)
        {
            BodyGrid = bodyGrid;

            SmartLB = new ListBox()
            {
                IsTabStop = false,
                MaxHeight = 200,
                MinWidth = 88,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Hidden
            };
            //SmartLB光标独立依赖属性必须设置
            FocusManager.SetIsFocusScope(SmartLB, true);
            SmartLB.PreviewKeyDown += SmartLB_PreviewKeyDown;
            SmartLB.PreviewMouseDoubleClick += SmartLB_PreviewMouseDoubleClick;
            BodyGrid.Children.Add(SmartLB);

            CompanyNameList = Global.AllCompanies.Select(_func => new SmartInfo()
            {
                Text = _func.名称,
                Letter = _func.拼音
            }).ToList();
        }

        private void SmartLB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.LBKeyDown(SmartLB, e); }
        private void SmartLB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) { SmartList.FillText(SmartLB); }
        /// <summary>
        /// 文本框加载完成->
        /// 自动获取焦点
        /// </summary>
        protected void SmartTB_Loaded(object sender, RoutedEventArgs e) { TextBox _TB = sender as TextBox; if (null != _TB) { _TB.Focus(); _TB.SelectionStart = _TB.Text.Length; } }
        /// <summary>
        /// 文本框获得逻辑焦点->
        /// 生成文本框Tag对应标记(Mark)的SmartInfo列表
        /// </summary>
        protected void SmartTB_GotFocus(object sender, RoutedEventArgs e) { SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyGrid, true); }
        /// <summary>
        /// 文本框丢失逻辑焦点->
        /// 更新文本框Tag对应标记(Mark)的权重及时间
        /// </summary>
        protected void SmartTB_LostFocus(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, true); }
        /// <summary>
        /// 文本框丢失逻辑焦点->
        /// 不更新(适用于用户给定SmartInfo列表)
        /// </summary>
        protected void SmartTB_LostFocusNotUpdate(object sender, RoutedEventArgs e) { SmartList.TBLostFocus(SmartLB, false); }
        /// <summary>
        /// 文本框有键盘键按下->
        /// 上下键,逻辑焦点给SmartLB
        /// </summary>
        protected void SmartTB_PreviewKeyDown(object sender, KeyEventArgs e) { SmartList.TBKeyDown(SmartLB, e); }
        /// <summary>
        /// 文本框内容改变->
        /// 筛选SmartInfo列表显示到SmartLB
        /// </summary>
        protected void SmartTB_TextChanged(object sender, TextChangedEventArgs e) { SmartList.TBChanged(SmartLB); }
        /// <summary>
        /// 文本框获得逻辑焦点->
        /// comNameTB专用
        /// </summary>
        protected void comNameTB_GotFocus(object sender, RoutedEventArgs e) { SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyGrid, true, CompanyNameList); }
        ///// <summary>
        ///// 文本框获得逻辑焦点->
        ///// conNameTB专用
        ///// </summary>
        //protected void conNameTB_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string keyString = comNameTB.Text.Trim();
        //        Guid keyID;
        //        if (keyString.Length == 0)//公司名称为空,检索个人客户
        //        {
        //            keyID = Guid.Empty;
        //        }
        //        else
        //        {
        //            CompanyModel _a = Global.AllCompanies.FirstOrDefault(_func => _func.名称 == keyString);
        //            if (_a != null)//找到公司ID,检索单位所属客户
        //            {
        //                keyID = _a.ID;

        //            }
        //            else//找不到公司ID,直接返回
        //            {
        //                return;
        //            }
        //        }
        //        List<SmartInfo> smartList = Global.AllContacts.Where(_func => _func.公司ID == keyID).Select(_func => new SmartInfo()
        //        {
        //            Text = _func.名称,
        //            Letter = _func.拼音
        //        }).ToList();

        //        SmartList.TBGotFocus(sender as TextBox, SmartLB, BodyG, true, smartList);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("标记(Mark)错误!");
        //        return;
        //    }
        //}
    }
    /// <summary>
    /// 对TextBox智能生成历史ListBox
    /// 采用多线程技术,避免连续快速输入及粘贴时的不必要性能开销
    /// </summary>
    public static class SmartList
    {
        #region    ----------私有变量----------
        /// <summary>
        /// 允许延时运行(有任务更新时为false,更新完成后改为true)
        /// </summary>
        private static bool allowRun;
        /// <summary>
        /// 按键刚被按下
        /// </summary>
        private static bool keyDown;
        /// <summary>
        /// 最多显示记录数量
        /// 默认值 0 表示全部显示
        /// </summary>
        private static int maxDisplayCount;
        /// <summary>
        /// 每个标记(Mark)最多存储的记录数量
        /// 多余记录在执行Save时自动删除
        /// 默认值 0 表示不自动删除记录
        /// </summary>
        private static int maxListCount;
        /// <summary>
        /// 线程休眠时间(毫秒)
        /// </summary>
        private static int threadTimeout;
        /// <summary>
        /// 总XML表的IO路径
        /// </summary>
        private static string smartPath;
        /// <summary>
        /// 总XML表
        /// </summary>
        private static XElement smartXE;
        #endregion ----------私有变量----------

        #region    ----------初始化,保存----------
        /// <summary>
        /// 初始化功能
        /// 加载外部XML文档
        /// 请在使用功能之前执行;
        /// </summary>
        /// <param name="MaxDisplayCount">最多显示记录数量 0 表示全部显示</param>
        /// <param name="MaxListCount">每个标记(Mark)最多存储的记录数量 0 表示不自动删除记录</param>
        /// <param name="ThreadTimeout">线程休眠时间(毫秒)</param>
        /// <param name="SmartPath">外部XML文档路径</param>
        public static void Initialization(int MaxDisplayCount, int MaxListCount, int ThreadTimeout, string SmartPath)
        {
            //初始化
            allowRun = false;
            maxDisplayCount = Math.Abs(MaxDisplayCount);
            maxListCount = Math.Abs(MaxListCount);
            threadTimeout = Math.Abs(ThreadTimeout);
            smartPath = SmartPath;
            //分线程初始化smartXE
            ThreadPool.QueueUserWorkItem(Load);
        }
        /// <summary>
        /// //初始化 smartXE
        /// </summary>
        /// <param name="o"></param>
        private static void Load(object o)
        {
            //文件存在->载入>>继续
            if (File.Exists(smartPath))
            {
                smartXE = XElement.Load(smartPath);
            }
            else//不存在->创建>>继续
            {
                string path = Path.GetDirectoryName(smartPath);
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                smartXE = new XElement("Infos");
                smartXE.Add(
                    new XComment("m Mark 标记"),
                    new XComment("t Text 文本"),
                    new XComment("l Letter 字母及拼音"),
                    new XComment("w Weight 排序权重"),
                    new XComment("u UpdateDate 最后更新时间")
                    );
                smartXE.Save(smartPath);
            }
            //初始化完成可运行
            allowRun = true;
        }
        /// <summary>
        /// 整理标签后保存
        /// 请在程序退出时执行
        /// </summary>
        public static void Save()
        {
            if (smartXE != null)
            {
                //遍历标记(Mark)
                foreach (XElement item in smartXE.Elements())
                {
                    //对标记内标签排序(先按权重降序)//,再按时间降序)
                    var testXE = item.Elements().OrderByDescending(_func => int.Parse(_func.Attribute("w").Value)).ThenByDescending(_func => _func.Attribute("u").Value).AsEnumerable();
                    //MaxListCount不为 0 并且比testXE总数小->删除多余标签>>继续
                    if (maxListCount != 0 && maxListCount < testXE.Count())
                    {
                        testXE = testXE.Take(maxListCount);
                    }
                    //整理后的标签替换原有标签
                    item.ReplaceNodes(testXE);
                }
                //保存总XML表
                smartXE.Save(smartPath);
            }
        }
        #endregion ----------初始化保存----------

        #region    ----------ListBox和TextBox的事件方法----------
        /// <summary>
        /// 内容填充方法ListBox_PreviewMouseDoubleClick
        /// </summary>
        /// <param name="smartLB"></param>
        public static void FillText(ListBox smartLB)
        {
            if (smartLB.SelectedItem != null)
            {
                //获取选中的字符串
                string itemString = smartLB.SelectedItem.ToString();
                //字符串不为空->smartTag不为空->更新所属文本框的文本>>光标移到文本末尾>>继续
                if (itemString.Length != 0)
                {
                    SmartTag smartTag = smartLB.Tag as SmartTag;
                    if (smartTag != null)
                    {
                        smartTag._TB.Text = itemString;
                        smartTag._TB.Focus();
                        smartTag._TB.SelectionStart = itemString.Length;
                        smartLB.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        /// <summary>
        /// Key = Enter,内容填充
        /// Key = Up且ListBox选中第一项,TextBox设为焦点
        /// </summary>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        /// <param name="e">KeyEventArgs</param>
        public static void LBKeyDown(ListBox smartLB, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FillText(smartLB);
                e.Handled = true;
            }
            else if (e.Key == Key.Up && smartLB.SelectedIndex == 0)
            {
                SmartTag a = smartLB.Tag as SmartTag;
                if (a != null && a._TB != null)
                {
                    a._TB.Focus();
                    e.Handled = true;
                }
            }
        }
        /// <summary>
        /// 内容更改方法
        /// TextBox_TextChanged
        /// </summary>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        public static void TBChanged(ListBox smartLB)
        {   //允许运行(线程池中没有延时更新任务)
            if (allowRun && keyDown)
            {
                keyDown = false;
                SmartTag smartTag = smartLB.Tag as SmartTag;
                //有smartTag并且里面有markXEList记录
                if (smartTag != null && smartTag._List.Count != 0)
                {
                    //马上建立分线程了,在线程结束前阻止其他调用TBChanged的命令执行
                    allowRun = false;
                    //在线程池中添加在分线程中执行的方法委托
                    //采用匿名方法
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        //延时,毫米
                        Thread.Sleep(threadTimeout);
                        //------延时时间到后------
                        List<string> _StringList;
                        int _ListCount;
                        string keyword = "";

                        //在控件关联线程中获取控件的Text
                        smartLB.Dispatcher.Invoke(() => keyword = smartTag._TB.Text);
                        //去除关键字前后空白并把字母设为小写
                        keyword = keyword.Trim().ToLower();

                        //关键字为空->返回smartMark对应标记(Mark)的所有文本"Text"列表 >> 继续
                        if (keyword.Length == 0)
                        {
                            _StringList = smartTag._List.Select(_func => _func.Text).ToList();
                        }
                        else//其他->根据关键字检索smartMark对应标记(Mark),返回对应文本"Text"列表>>继续
                        {
                            _StringList = smartTag._List.Where(_func => _func.Letter.Contains(keyword) || _func.Text.Contains(keyword)).Select(_func => _func.Text).ToList();
                        }

                        _ListCount = _StringList.Count;

                        //_ListCount为0,_ListCount为1并且和senderTB相同->隐藏smartLB >> 返回
                        if (_ListCount == 0 || _ListCount == 1 && _StringList.Contains(keyword))
                        {
                            //在控件关联线程中更新控件
                            smartLB.Dispatcher.Invoke(() => smartLB.Visibility = Visibility.Hidden);
                        }
                        else
                        {
                            //_StringList总数超过MaxDisplayCount->截取指定数量 >> 修改列表总数 >> 继续
                            if (maxDisplayCount != 0 && maxDisplayCount < _ListCount)
                            {
                                _StringList = _StringList.Take(maxDisplayCount).ToList();
                            }
                            //绑定数据源 >> 显示smartLB
                            //在控件关联线程中更新控件
                            smartLB.Dispatcher.Invoke(() =>
                            {
                                smartLB.ItemsSource = _StringList;
                                smartLB.Visibility = Visibility.Visible;
                            });
                        }
                        allowRun = true;
                    });
                }
                else
                {
                    keyDown = false;
                    smartLB.Visibility = Visibility.Hidden;
                }
            }
        }
        /// <summary>
        /// 获得焦点方法
        /// 根据senderTB.Tag保存的标记(Mark)生成列表
        /// TextBox_GotFocus
        /// </summary>
        /// <param name="senderTB">触发事件的TextBox</param>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        /// <param name="ancestorUI">smartLB的父级控件</param>
        /// <param name="showList">加载时马上显示列表</param>
        public static void TBGotFocus(TextBox senderTB, ListBox smartLB, UIElement ancestorUI, bool showList)
        {
            List<SmartInfo> smartList;
            try
            {
                smartList = smartXE.Element(senderTB.Tag.ToString()).Elements().Select(_func => new SmartInfo()
                {
                    Text = _func.Attribute("t").Value,
                    Letter = _func.Attribute("l").Value
                }).ToList();
            }
            catch (Exception)
            {
                //MessageBox.Show("标记(Mark)错误!");
                //return;
                smartList = new List<SmartInfo>();
            }
            TBGotFocus(senderTB, smartLB, ancestorUI, showList, smartList);
        }
        /// <summary>
        /// 获得焦点方法
        /// 根据自定义List<SmartInfo>生成列表
        /// TextBox_GotFocus
        /// </summary>
        /// <param name="senderTB">触发事件的TextBox</param>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        /// <param name="ancestorUI">smartLB的父级控件</param>
        /// <param name="showList">加载时马上显示列表</param>
        /// <param name="smartList">自定义SmartInfo清单</param>
        public static void TBGotFocus(TextBox senderTB, ListBox smartLB, UIElement ancestorUI, bool showList, List<SmartInfo> smartList)
        {
            //在smartLB的Tag中保存当前SmartTag
            smartLB.Tag = new SmartTag()
            {
                _TB = senderTB,
                _List = smartList,
                _Text = senderTB.Text
            };

            //定位smartLB至senderTB下方并设置宽度与其相同
            Point p = senderTB.TranslatePoint(new Point(0, senderTB.ActualHeight), ancestorUI);
            smartLB.Margin = new Thickness(p.X, p.Y, 0, 0);
            smartLB.Width = senderTB.ActualWidth;

            if (showList)
            {
                keyDown = true;
                TBChanged(smartLB);
            }
        }
        /// <summary>
        /// SmartLB为显示状态,键盘上下键,SmartLB设为焦点
        /// 内部keyDown设为true
        /// </summary>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        /// <param name="e">KeyEventArgs</param>
        public static void TBKeyDown(ListBox smartLB, KeyEventArgs e)
        {
            if (smartLB.Visibility == Visibility.Visible && (e.Key == Key.Up || e.Key == Key.Down))
                smartLB.Focus();
            else
                keyDown = true;
        }
        /// <summary>
        /// 失去焦点方法
        /// TextBox_LostFocus
        /// </summary>
        /// <param name="smartLB">用于显示列表的ListBox</param>
        public static void TBLostFocus(ListBox smartLB, bool Update)
        {
            if (Update)
            {
                SmartTag smartTag = smartLB.Tag as SmartTag;
                if (smartTag != null && smartTag._TB != null && smartTag._TB.Tag != null)
                {
                    string _text = smartTag._TB.Text.Trim();
                    if (_text.Length != 0 && _text != smartTag._Text)
                    {
                        UpdateMark(smartTag._TB.Tag.ToString(), _text);
                    }
                }
            }
            //隐藏smartLB>>清除Tag绑定的TextBox>>清除集合源
            smartLB.Visibility = Visibility.Hidden;
            smartLB.Tag = null;
            smartLB.ItemsSource = null;
        }
        /// <summary>
        /// 更新标记
        /// </summary>
        /// <param name="smartMark">标记名称</param>
        /// <param name="textString">需更新的字符串</param>
        private static void UpdateMark(string smartMark, string textString)
        {
            //载入对应标记
            XElement markXE = smartXE.Element(smartMark);
            //标记不存在->创建>>添加至smartXE>>继续
            if (markXE == null)
            {
                markXE = new XElement(smartMark);
                smartXE.Add(markXE);
            }

            //获取文本对应标签
            XElement textXE = markXE.Elements().FirstOrDefault(_func => _func.Attribute("t").Value == textString);
            //文本标签不存在->创建>>继续
            if (null == textXE)
            {
                markXE.AddFirst(new XElement("m",
                   new XAttribute("t", textString),
                   new XAttribute("l", Chinese2Spell.Convert(textString, false, "[字母] [拼音]").ToLower()),
                   new XAttribute("w", "1"),
                   new XAttribute("u", DateTime.Now.ToShortDateString())));
            }
            else//存在->权重+1>>更新时间>>继续
            {
                textXE.Attribute("w").Value = (int.Parse(textXE.Attribute("w").Value) + 1).ToString();
                textXE.Attribute("u").Value = DateTime.Now.ToShortDateString();
            }

        }
        #endregion ----------TextBox和ListBox的事件方法----------
    }

    /// <summary>
    /// GB2312汉字转拼音和首字母静态类
    /// </summary>
    public static class Chinese2Spell
    {
        #region 属性数据定义
        private static int[] firstValue = new int[403]
        {
            0xB0A1,0xB0A3,0xB0B0,0xB0B9,0xB0BC,0xB0C5,0xB0D7,0xB0DF,0xB0EE,0xB0FA,0xB1AD,0xB1BC,0xB1C0,0xB1C6,0xB1DE,0xB1EA,
            0xB1EE,0xB1F2,0xB1F8,0xB2A3,0xB2B6,0xB2C1,0xB2C2,0xB2CD,0xB2D4,0xB2D9,0xB2DE,0xD7FA,0xB2E3,0xB2E5,0xB2F0,0xB2F3,
            0xB2FD,0xB3AC,0xB3B5,0xB3BB,0xB3C5,0xB3D4,0xB3E4,0xB3E9,0xB3F5,0xB4A7,0xB4A8,0xB4AF,0xB4B5,0xB4BA,0xB4C1,0xB4C3,
            0xB4CF,0xB4D5,0xB4D6,0xB4DA,0xB4DD,0xB4E5,0xB4E8,0xB4EE,0xB4F4,0xB5A2,0xB5B1,0xB5B6,0xB5C2,0xB5C5,0xB5CC,0xD7FA,
            0xB5DF,0xB5EF,0xB5F8,0xB6A1,0xB6AA,0xB6AB,0xB6B5,0xB6BC,0xB6CB,0xB6D1,0xB6D5,0xB6DE,0xB6EA,0xD7FA,0xB6F7,0xB6F8,
            0xB7A2,0xB7AA,0xB7BB,0xB7C6,0xB7D2,0xB7E1,0xB7F0,0xB7F1,0xB7F2,0xB8C1,0xB8C3,0xB8C9,0xB8D4,0xB8DD,0xB8E7,0xB8F8,
            0xB8F9,0xB8FB,0xB9A4,0xB9B3,0xB9BC,0xB9CE,0xB9D4,0xB9D7,0xB9E2,0xB9E5,0xB9F5,0xB9F8,0xB9FE,0xBAA1,0xBAA8,0xBABB,
            0xBABE,0xBAC7,0xBAD9,0xBADB,0xBADF,0xBAE4,0xBAED,0xBAF4,0xBBA8,0xBBB1,0xBBB6,0xBBC4,0xBBD2,0xBBE7,0xBBED,0xBBF7,
            0xBCCE,0xBCDF,0xBDA9,0xBDB6,0xBDD2,0xBDED,0xBEA3,0xBEBC,0xBEBE,0xBECF,0xBEE8,0xBEEF,0xBEF9,0xBFA6,0xBFAA,0xBFAF,
            0xBFB5,0xBFBC,0xBFC0,0xBFCF,0xBFD3,0xBFD5,0xBFD9,0xBFDD,0xBFE4,0xBFE9,0xBFED,0xBFEF,0xBFF7,0xC0A4,0xC0A8,0xC0AC,
            0xC0B3,0xC0B6,0xC0C5,0xC0CC,0xC0D5,0xC0D7,0xC0E2,0xC0E5,0xC1A9,0xC1AA,0xC1B8,0xC1C3,0xC1D0,0xC1D5,0xC1E0,0xC1EF,
            0xC1FA,0xC2A5,0xC2AB,0xC2BF,0xC2CD,0xC2D3,0xC2D5,0xC2DC,0xD7FA,0xC2E8,0xC2F1,0xC2F7,0xC3A2,0xC3A8,0xC3B4,0xC3B5,
            0xC3C5,0xC3C8,0xC3D0,0xC3DE,0xC3E7,0xC3EF,0xC3F1,0xC3F7,0xC3FD,0xC3FE,0xC4B1,0xC4B4,0xD7FA,0xC4C3,0xC4CA,0xC4CF,
            0xC4D2,0xC4D3,0xC4D8,0xC4D9,0xC4DB,0xC4DC,0xC4DD,0xC4E8,0xC4EF,0xC4F1,0xC4F3,0xC4FA,0xC4FB,0xC5A3,0xC5A7,0xD7FA,
            0xC5AB,0xC5AE,0xC5AF,0xC5B0,0xC5B2,0xC5B6,0xC5B7,0xC5BE,0xC5C4,0xC5CA,0xC5D2,0xC5D7,0xC5DE,0xC5E7,0xC5E9,0xC5F7,
            0xC6AA,0xC6AE,0xC6B2,0xC6B4,0xC6B9,0xC6C2,0xC6CA,0xC6CB,0xC6DA,0xC6FE,0xC7A3,0xC7B9,0xC7C1,0xC7D0,0xC7D5,0xC7E0,
            0xC7ED,0xC7EF,0xC7F7,0xC8A6,0xC8B1,0xC8B9,0xC8BB,0xC8BF,0xC8C4,0xC8C7,0xC8C9,0xC8D3,0xC8D5,0xC8D6,0xC8E0,0xC8E3,
            0xC8ED,0xC8EF,0xC8F2,0xC8F4,0xC8F6,0xC8F9,0xC8FD,0xC9A3,0xC9A6,0xC9AA,0xC9AD,0xC9AE,0xC9AF,0xC9B8,0xC9BA,0xC9CA,
            0xC9D2,0xC9DD,0xC9E9,0xC9F9,0xCAA6,0xCAD5,0xCADF,0xCBA2,0xCBA4,0xCBA8,0xCBAA,0xCBAD,0xCBB1,0xCBB5,0xCBB9,0xCBC9,
            0xCBD1,0xCBD5,0xCBE1,0xCBE4,0xCBEF,0xCBF2,0xCBFA,0xCCA5,0xCCAE,0xCCC0,0xCCCD,0xCCD8,0xCCD9,0xCCDD,0xCCEC,0xCCF4,
            0xCCF9,0xCCFC,0xCDA8,0xCDB5,0xCDB9,0xCDC4,0xCDC6,0xCDCC,0xCDCF,0xCDDA,0xCDE1,0xCDE3,0xCDF4,0xCDFE,0xCEC1,0xCECB,
            0xCECE,0xCED7,0xCEF4,0xCFB9,0xCFC6,0xCFE0,0xCFF4,0xD0A8,0xD0BD,0xD0C7,0xD0D6,0xD0DD,0xD0E6,0xD0F9,0xD1A5,0xD1AB,
            0xD1B9,0xD1C9,0xD1EA,0xD1FB,0xD2AC,0xD2BB,0xD2F0,0xD3A2,0xD3B4,0xD3B5,0xD3C4,0xD3D8,0xD4A7,0xD4BB,0xD4C5,0xD4D1,
            0xD4D4,0xD4DB,0xD4DF,0xD4E2,0xD4F0,0xD4F4,0xD4F5,0xD4F6,0xD4FA,0xD5AA,0xD5B0,0xD5C1,0xD5D0,0xD5DA,0xD5E4,0xD5F4,
            0xD6A5,0xD6D0,0xD6DB,0xD6E9,0xD7A5,0xD7A7,0xD7A8,0xD7AE,0xD7B5,0xD7BB,0xD7BD,0xD7C8,0xD7D7,0xD7DE,0xD7E2,0xD7EA,
            0xD7EC,0xD7F0,0xD7F2
        };
        private static string[] pyName = new string[403]
        {
            "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben","Beng","Bi","Bian","Biao",
            "Bie","Bin","Bing","Bo","Bu","Ca","Cai","Can","Cang","Cao","Ce","Cen","Ceng","Cha","Chai","Chan",
            "Chang","Chao","Che","Chen","Cheng","Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci",
            "Cong","Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De","Deng","Di","Dia",
            "Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui","Dun","Duo","E","Ei","En","Er",
            "Fa","Fan","Fang","Fei","Fen","Feng","Fo","Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei",
            "Gen","Geng","Gong","Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han","Hang",
            "Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan","Huang","Hui","Hun","Huo","Ji",
            "Jia","Jian","Jiang","Jiao","Jie","Jin","Jing","Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan",
            "Kang","Kao","Ke","Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo","La",
            "Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang","Liao","Lie","Lin","Ling","Liu",
            "Long","Lou","Lu","Lv","Luan","Lue","Lun","Luo","M","Ma","Mai","Man","Mang","Mao","Me","Mei",
            "Men","Meng","Mi","Mian","Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","N","Na","Nai","Nan",
            "Nang","Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning","Niu","Nong","Nou",
            "Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan","Pang","Pao","Pei","Pen","Peng","Pi",
            "Pian","Piao","Pie","Pin","Ping","Po","Pou","Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing",
            "Qiong","Qiu","Qu","Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou","Ru",
            "Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen","Seng","Sha","Shai","Shan","Shang",
            "Shao","She","Shen","Sheng","Shi","Shou","Shu","Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song",
            "Sou","Su","Suan","Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian","Tiao",
            "Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai","Wan","Wang","Wei","Wen","Weng",
            "Wo","Wu","Xi","Xia","Xian","Xiang","Xiao","Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun",
            "Ya","Yan","Yang","Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun","Za",
            "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan","Zhang","Zhao","Zhe","Zhen","Zheng",
            "Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan","Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan",
            "Zui","Zun","Zuo"
        };
        private static string[] secondValue = new string[403]
        {
            "吖嗄腌锕","捱嗳嗌嫒瑷暧砹锿霭","谙埯揞犴庵桉铵鹌黯","","坳拗嗷岙廒遨媪骜獒聱螯鏊鳌鏖","艹茇菝岜灞钯粑鲅魃","捭呗掰","阪坂钣瘢癍舨","蒡浜","勹葆孢煲鸨褓趵龅","孛陂邶蓓悖碚鹎褙鐾鞴","畚坌贲锛","嘣甏","萆匕俾荜荸薜吡哔狴庳愎滗濞弼妣婢嬖璧畀铋秕裨筚箅篦舭襞跸髀","匾弁苄忭汴缏煸砭碥窆褊蝙笾鳊","婊骠杓飑飙飚镖镳瘭裱鳔髟",
            "蹩","傧豳缤玢槟殡膑镔髌鬓","禀冫邴摒","亳啵饽檗擘礴钹鹁簸跛踣","卟逋瓿晡钚钸醭","嚓礤","","孱骖璨粲黪","","嘈漕螬艚","恻","岑涔","噌","猹馇汊姹杈楂槎檫锸镲衩","侪钗瘥虿","冁谄蒇廛忏潺澶羼婵骣觇禅镡蟾躔",
            "伥鬯苌菖徜怅惝阊娼嫦昶氅鲳","怊晁焯耖","坼屮砗","伧谌谶抻嗔宸琛榇碜龀","丞埕枨柽塍瞠铖铛裎蛏酲","傺坻墀茌叱哧啻嗤彳饬媸敕眙眵鸱瘛褫蚩螭笞篪豉踟魑","茺忡憧铳舂艟","俦帱惆瘳雠","亍刍怵憷绌杵楮樗褚蜍蹰黜","搋啜嘬膪踹","舛遄巛氚钏舡","怆","陲棰槌","莼鹑蝽","辶辍踔龊","茈呲祠鹚糍",
            "苁淙骢琮璁枞","楱辏腠","蔟徂猝殂酢蹙蹴","汆撺爨镩","萃啐悴璀榱毳隹","忖皴","厝嵯脞锉矬痤鹾蹉","耷哒嗒怛妲沓疸褡笪靼鞑","埭甙呔岱迨骀绐玳黛","儋萏啖澹殚赕眈瘅聃箪","谠凼菪宕砀裆","刂叨忉氘焘纛","锝","噔嶝戥磴镫簦","氐籴诋谛邸荻嘀娣柢棣觌砥碲睇镝羝骶","嗲",
            "丶阽坫巅玷钿癜癫簟踮","铞铫貂鲷","垤堞揲喋牒瓞耋蹀鲽","仃啶玎腚碇町铤疔耵酊","铥","垌咚岽峒氡胨胴硐鸫","蔸窦蚪篼","芏嘟渎椟牍蠹笃髑黩","椴煅簖","怼憝碓镦","沌炖砘礅盹趸","咄哚沲缍柁铎裰踱","噩谔垩苊莪萼呃愕阏屙婀轭腭锇锷鹗颚鳄","诶","蒽摁","佴迩珥铒鸸鲕",
            "垡砝","蕃蘩幡梵燔畈蹯","匚邡彷枋钫舫鲂","芾狒悱淝妃绯榧腓斐扉镄痱蜚篚翡霏鲱","偾瀵棼鲼鼢","俸酆葑唪沣砜","","缶","匐凫阝郛芙苻茯莩菔拊呋幞怫滏艴孚驸绂绋桴赙祓砩黻黼罘稃馥蚨蜉蝠蝮麸趺跗鲋鳆","伽尬尕尜旮钆","丐陔垓戤赅","坩苷尴擀泔淦澉绀橄旰矸疳酐","戆罡筻","睾诰郜藁缟槔槁杲锆","鬲仡哿圪塥嗝纥搿膈硌镉袼虼舸骼","",
            "亘茛哏艮","哽赓绠鲠","珙肱蚣觥","佝诟岣遘媾缑枸觏彀笱篝鞲","嘏诂菰呱崮汩梏轱牯牿臌毂瞽罟钴锢鸪鹄痼蛄酤觚鲴鹘","卦诖栝胍鸹","掴","倌莞掼涫盥鹳矜鳏","咣犷桄胱","匦刿庋宄妫桧炅晷皈簋鲑鳜","衮绲磙鲧","馘埚呙帼崞猓椁虢聒蜾蝈","铪","嗨胲醢","邗菡撖瀚晗焓顸颔蚶鼾","沆绗颃",
            "蒿薅嗥嚆濠灏昊皓颢蚝","诃劾壑嗬阖曷盍颌翮","","","蘅珩桁","黉訇讧荭蕻薨闳泓","堠後逅瘊篌糇鲎骺","冱唿囫岵猢怙惚浒滹琥槲轷觳烀煳戽扈祜瓠鹕鹱笏醐斛","骅桦砉铧","踝","郇奂萑擐圜獾洹浣漶寰逭缳锾鲩鬟","隍徨湟潢遑璜肓癀蟥篁鳇","诙茴荟蕙咴哕喙隳洄浍彗缋珲晖恚虺蟪麾","诨馄阍溷","劐藿攉嚯夥灬钬锪镬耠蠖","丌亟乩剞佶偈诘墼芨芰荠蒺蕺掎叽咭哜唧岌嵴洎彐屐骥畿玑楫殛戟戢觊犄齑矶羁嵇稷瘠虮笈笄暨跻跽霁鲚鲫髻麂",
            "郏葭岬浃迦珈戛胛恝铗镓痂瘕袷蛱笳袈跏","僭谏谫菅蒹搛囝湔蹇謇缣枧楗戋戬牮犍毽腱睑锏鹣裥笕翦趼踺鲣鞯","茳洚绛缰犟礓耩糨豇","佼僬艽茭挢噍峤徼湫姣敫皎鹪蛟醮跤鲛","卩拮喈嗟婕孑桀碣讦疖颉蚧羯鲒骱","卺荩堇噤馑廑妗缙瑾槿赆觐钅衿","刭儆阱菁獍憬泾迳弪婧肼胫腈旌靓","冂迥扃","僦啾阄柩桕鸠鹫赳鬏","倨讵苣苴莒掬遽屦琚椐榘榉橘犋飓钜锔窭裾趄醵踽龃雎瞿鞫","鄄狷涓桊蠲锩镌隽","厥劂谲矍蕨噘噱崛獗孓珏桷橛爝镢蹶觖","捃皲筠麇","佧咔胩","剀凵垲蒈忾恺铠锎锴","侃莰阚戡龛瞰",
            "伉闶钪","尻栲犒铐","嗑岢恪溘骒缂珂轲氪瞌钶锞稞疴窠颏蚵蝌髁","裉","铿","倥崆箜","芤蔻叩眍筘","刳堀喾绔骷","侉","蒯郐哙狯脍","髋","诓诳邝圹夼哐囗纩贶","馗匮夔隗蒉揆喹喟悝愦逵暌睽聩蝰篑跬","悃阃琨锟醌鲲髡","蛞","剌邋旯砬瘌",
            "崃徕涞濑赉赍睐铼癞籁","岚漤榄斓罱镧褴","莨蒗啷阆锒稂螂","唠崂栳铑铹痨耢醪","仂叻泐鳓","羸诔嘞嫘缧檑耒酹","塄愣","俪俚郦坜苈莅蓠藜呖唳喱猁溧澧逦娌嫠骊缡枥栎轹戾砺詈罹锂鹂疠疬蛎蜊蠡笠篥粝醴跞雳鲡鳢黧","","蔹奁潋濂琏楝殓臁裢裣蠊鲢","墚椋踉魉","蓼尥嘹獠寮缭钌鹩","冽埒捩咧洌趔躐鬣","蔺啉嶙廪懔遴檩辚膦瞵粼躏麟","酃苓呤囹泠绫柃棂瓴聆蛉翎鲮","浏遛骝绺旒熘锍镏鹨鎏",
            "垅茏泷珑栊胧砻癃","偻蒌喽嵝镂瘘耧蝼髅","垆撸噜泸渌漉逯璐栌橹轳辂辘氇胪镥鸬鹭簏舻鲈","捋闾榈膂稆褛","脔娈栾鸾銮","锊","囵","倮蠃荦摞猡泺漯珞椤脶镙瘰雒","呒","犸杩蟆","劢荬唛霾","墁幔缦熳镘颟螨鳗鞔","邙漭硭蟒","袤茆峁泖瑁昴牦耄旄懋瞀蝥蟊髦","麽","莓嵋猸浼湄楣镅鹛袂魅",
            "扪焖懑钔","勐甍瞢懵朦礞虻蜢蠓艋艨","芈冖谧蘼廾咪嘧猕汨宓弭脒祢敉糸縻麋","沔渑湎宀腼眄黾","喵邈缈缪杪淼眇鹋","乜咩蠛篾","苠岷闵泯缗玟珉愍鳘","冥茗溟暝瞑酩","","谟茉蓦馍嫫嬷殁镆秣瘼耱貊貘","侔哞眸蛑鍪","仫坶苜沐毪钼","嗯","捺肭镎衲","鼐艿萘柰","喃囡楠腩蝻赧",
            "攮囔馕曩","孬垴呶猱瑙硇铙蛲","讷疒","","恁","","伲坭猊怩昵旎睨铌鲵","廿埝辇黏鲇鲶","","茑嬲脲袅","陧蘖嗫颞臬蹑","","佞咛甯聍","狃忸妞","侬哝","耨",
            "弩胬孥驽","恧钕衄","","","傩搦喏锘","噢","讴怄瓯耦","葩杷筢","俳蒎哌","拚丬爿泮袢襻蟠蹒","滂逄螃","匏狍庖脬疱","辔帔旆锫醅霈","湓","堋嘭怦蟛","丕仳陴邳郫圮埤鼙芘擗噼庀淠媲纰枇甓睥罴铍癖蚍蜱貔",
            "谝骈犏胼翩蹁","剽嘌嫖缥殍瞟螵","丿苤氕","姘嫔榀牝颦虍","俜娉枰鲆","叵鄱珀攴钋钷皤笸","裒掊","匍噗溥濮璞氆攵镤镨蹼","亓俟圻芑芪萁萋葺蕲嘁屺岐汔淇骐绮琪琦杞桤槭耆欹祺憩碛颀蛴蜞綦綮蹊鳍麒","葜髂","倩佥阡芊芡茜荨掮岍悭慊骞搴褰缱椠肷愆钤虔箝鬈","戕嫱樯戗炝锖锵镪襁蜣羟跄","劁诮谯荞愀憔缲樵硗跷鞒","郄惬妾挈锲箧","芩揿吣嗪噙溱檎锓覃螓衾","苘圊檠磬蜻罄箐謦鲭黥",
            "邛茕穹蛩筇跫銎","俅巯犰逑遒楸赇虬蚯蝤裘糗鳅鼽","诎劬蕖蘧岖衢阒璩觑氍朐祛磲鸲癯蛐蠼麴黢","诠荃犭悛绻辁畎铨蜷筌","阕阙悫","逡","苒蚺髯","禳穰","荛娆桡","","亻仞荏葚饪轫稔衽","","","嵘狨榕肜蝾","糅蹂鞣","蓐薷嚅洳溽濡缛铷襦颥",
            "朊","芮蕤枘睿蚋","","偌箬","卅仨挲脎飒","噻","馓毵糁","搡磉颡","埽缫臊瘙鳋","啬铯穑","","","唼歃铩痧裟霎鲨","酾","讪鄯埏芟彡潸姗嬗骟膻钐疝蟮舢跚鳝","垧绱殇熵觞",
            "劭苕潲蛸筲艄","厍佘猞滠歙畲麝","诜谂莘哂渖椹胂矧蜃","嵊晟眚笙","谥埘莳蓍弑饣轼贳炻礻铈螫舐筮豕鲥鲺","扌狩绶艏","丨倏塾菽摅沭澍姝纾毹腧殳秫","唰","蟀","闩涮","孀","氵","","蒴搠妁槊铄","厮兕厶咝汜泗澌姒驷纟缌祀锶鸶耜蛳笥","凇菘崧嵩忪悚淞竦",
            "叟薮嗖嗾馊溲飕瞍锼螋","夙谡蔌嗉愫涑簌觫稣","狻","谇荽濉邃燧眭睢","荪狲飧榫隼","唢嗦嗍娑桫睃羧","闼溻遢榻铊趿鳎","邰薹肽炱钛跆鲐","郯昙忐钽锬","傥帑饧溏瑭樘铴镗耥螗螳羰醣","鼗啕洮韬饕","忒忑慝铽","滕","倜荑悌逖绨缇鹈裼醍","掭忝阗殄畋","佻祧窕蜩笤粜龆鲦髫",
            "萜餮","莛葶婷梃蜓霆","佟仝茼嗵恸潼砼","亠钭骰","堍荼菟钍酴","抟彖疃","煺","氽饨暾豚","乇佗坨庹沱柝橐砣箨酡跎鼍","佤娲腽","崴","剜芄菀纨绾琬脘畹蜿","罔惘辋魍","偎诿隈圩葳薇帏帷嵬猥猬闱沩洧涠逶娓玮韪軎炜煨痿艉鲔","刎阌汶璺雯","蓊蕹",
            "倭莴喔幄渥肟硪龌","兀仵阢邬圬芴唔庑怃忤浯寤迕妩婺骛杌牾於焐鹉鹜痦蜈鋈鼯","僖兮隰郗菥葸蓰奚唏徙饩阋浠淅屣嬉玺樨曦觋欷熹禊禧皙穸蜥螅蟋舄舾羲粞翕醯鼷","呷狎遐瑕柙硖罅黠","冼苋莶藓岘猃暹娴氙燹祆鹇痫蚬筅籼酰跣跹霰","芗葙饷庠骧缃蟓鲞飨","哓崤潇逍骁绡枭枵筱箫魈","偕亵勰燮薤撷獬廨渫瀣邂绁缬榭榍躞","囟馨忄昕歆鑫","陉荇荥擤悻硎","芎","咻岫馐庥溴鸺貅髹","诩勖蓿洫溆顼栩煦盱胥糈醑","儇谖萱揎泫渲漩璇楦暄炫煊碹铉镟痃","谑泶踅鳕","巽埙荀蕈薰峋徇獯恂洵浔曛窨醺鲟",
            "伢垭揠岈迓娅琊桠氩砑睚痖疋","厣赝剡俨偃兖讠谳郾鄢芫菸崦恹闫湮滟妍嫣琰檐晏胭焱罨筵酽魇餍鼹","徉怏泱炀烊恙蛘鞅","夭爻吆崾徭幺珧杳轺曜肴鹞窈繇鳐","靥谒邺揶晔烨铘","刈劓佚佾诒圯埸懿苡薏弈奕挹弋呓咦咿噫峄嶷猗饴怿怡悒漪迤驿缢殪轶贻旖熠钇镒镱痍瘗癔翊衤蜴舣羿翳酏黟","胤鄞廴垠堙茚吲喑狺夤洇氤铟瘾蚓霪龈","嬴郢茔莺萦蓥撄嘤膺滢潆瀛瑛璎楹媵鹦瘿颍罂","唷","俑壅墉喁慵邕镛甬鳙饔","卣攸侑莠莜莸尢呦囿宥柚猷牖铕疣蚰蚴蝣鱿黝鼬","禺毓伛俣谀谕萸蓣揄圄圉嵛狳饫馀庾阈鬻妪妤纡瑜昱觎腴欤煜熨燠肀聿钰鹆鹬瘐瘀窬窳蜮蝓竽臾舁雩龉","垸塬掾沅媛瑗橼爰眢鸢螈箢鼋","龠瀹樾刖钺","郓芸狁恽愠纭韫殒昀氲","拶咂",
            "崽甾","瓒昝簪糌趱錾","奘驵臧","唣","仄赜啧帻迮昃笮箦舴","","谮","缯甑罾锃","揸吒咤哳砟痄蚱齄","砦瘵","谵搌旃","仉鄣幛嶂獐嫜璋蟑","诏啁棹钊笊","谪摺柘辄磔鹧褶蜇赭","圳蓁浈缜桢榛轸赈胗朕祯畛稹鸩箴","诤峥徵钲铮筝",
            "卮陟郅埴芷摭帙夂忮彘咫骘栉枳栀桎轵轾贽胝膣祉祗黹雉鸷痣蛭絷酯跖踬踯豸觯","冢锺螽舯踵","荮妯纣绉胄碡籀酎","伫侏邾苎茱洙渚潴杼槠橥炷铢疰瘃竺箸舳翥躅麈","","","啭馔颛","僮","惴骓缒","肫窀","倬诼擢浞涿濯禚斫镯","谘嵫姊孳缁梓辎赀恣眦锱秭耔笫粢趑觜訾龇鲻髭","偬腙粽","诹陬鄹驺鲰","俎菹镞","攥缵躜",
            "蕞","撙樽鳟","阼唑怍胙祚"
        };
        #endregion
        /// <summary>
        /// GB2312汉字转拼音和首字母
        /// </summary>
        /// <param name="hanzi">待转换的字符串</param>
        /// <param name="ascii">是否保留字符串的ASCII字符</param>
        /// <param name="format">拼音和首字母输出格式, "[拼音]"替换成转换后的拼音, "[字母]"替换成转换后的首字母</param>
        /// <returns></returns>
        public static string Convert(string hanzi, bool ascii, string format)
        {
            //汉字两个字节机内码
            byte[] array;
            //拼音,字母
            string spell = "";
            string letter = "";
            //汉字机内码
            int charAscii = 0;
            //汉字字符数组
            char[] hanziChar = hanzi.ToCharArray();

            //遍历字符数组
            for (int i = 0; i < hanziChar.Length; i++)
            {
                //获取机内码
                array = Encoding.Default.GetBytes(hanziChar[i].ToString());
                //汉字机内码两个字节
                if (2 == array.Length)
                {
                    //一级汉字
                    //15<区码<56, 0xAF<机内码<0xD8 0<位码<95, 0xA0<机内码<0xFE
                    if (array[0] > 0xAF && array[0] < 0xD8 && array[1] > 0xA0)
                    {
                        //计算机内码
                        charAscii = array[0] * 0x100 + array[1];
                        //一级汉字检索
                        for (int j = 402; j >= 0; j--)
                        {
                            if (firstValue[j] <= charAscii)
                            {
                                spell += pyName[j];
                                letter += pyName[j].Substring(0, 1);
                                break;
                            }
                        }
                    }//二级汉字
                     //55<区码<88, 0xD7<机内码<0xF8 0<位码<95, 0xA0<机内码<0xFE
                    else if (array[0] > 0xD7 && array[0] < 0xF8 && array[1] > 0xA0)
                    {
                        //二级汉字检索
                        for (int j = 0; j < 403; j++)
                        {
                            if (secondValue[j].Contains(hanziChar[i].ToString()))
                            {
                                spell += pyName[j];
                                letter += pyName[j].Substring(0, 1);
                                break;
                            }
                        }
                    }
                    else//GBK 0x80<=机内区码<=0xFE 0x40<=机内位码<=0xFE 
                    {
                        //修正生僻字
                        switch (hanziChar[i])
                        {
                            case '汣':
                                spell += "Jiu";
                                letter += 'J';
                                break;
                            case '璟':
                                spell += "Jing";
                                letter += 'J';
                                break;
                            case '昇':
                            case '晟':
                                spell += "Sheng";
                                letter += 'S';
                                break;
                            case '鋆':
                                spell += "Jun";
                                letter += 'J';
                                break;
                            default:
                                spell += hanziChar[i];
                                letter += hanziChar[i];
                                break;
                        }
                    }
                }//单字节ASCII码是否显示
                else if (ascii)
                {
                    spell += hanziChar[i];
                    letter += hanziChar[i];
                }
            }
            return format.Replace("[字母]", letter).Replace("[拼音]", spell);
        }
    }
}
