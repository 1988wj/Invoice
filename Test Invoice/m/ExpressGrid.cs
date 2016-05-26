using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace Test_Invoice
{
    public class ExpressGrid
    {
        #region    ------私有变量及控件------
        /// <summary>
        /// 是否绑定联系人数据源
        /// </summary>
        bool isBinding;
        /// <summary>
        /// 显示缩放系数
        /// </summary>
        double zoomCoefficient;
        /// <summary>
        /// 撕边边距
        /// </summary>
        double edge;
        /// <summary>
        /// 文本在网格内移动的最大坐标
        /// </summary>
        Point gridMaxPoint;
        /// <summary>
        /// 网格左上角与鼠标相对坐标
        /// </summary>
        Point mousePoint;
        /// <summary>
        /// 文本左上角与鼠标相对坐标
        /// </summary>
        Point textPoint;
        /// <summary>
        /// 预览快递信息的Grid
        /// </summary>
        Grid expressGrid;
        /// <summary>
        /// 快递图片
        /// </summary>
        Image expressImage;
        /// <summary>
        /// 右键点击的TextBlock
        /// </summary>
        TextBlock selectTB;
        /// <summary>
        /// TextBlock的编辑状态
        /// </summary>
        TextBox editTB;
        /// <summary>
        /// Grid快捷菜单
        /// </summary>
        ContextMenu gridCM;
        /// <summary>
        /// TextBlock快捷菜单
        /// </summary>
        ContextMenu textCM;
        #endregion ------私有变量及控件------

        #region    ------构造函数------
        /// <summary>
        /// 快递打印预览
        /// </summary>
        /// <param name="expGrid">预览快递打印格式的Grid</param>
        /// <param name="isBind">是否绑定联系人数据源</param>
        public ExpressGrid(Grid expGrid, bool isBind)
        {
            expressGrid = expGrid;
            isBinding = isBind;
            //TextBox初始化
            editTB = new TextBox()
            {
                //Background = Brushes.LightSteelBlue,
                //BorderBrush = Brushes.Black,
                //BorderThickness = new Thickness(2),
                MinWidth = 66,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Collapsed
            };
            editTB.PreviewKeyUp += TextBox_PreviewKeyUp;
            //TextBlock快捷菜单
            textCM = new ContextMenu();
            string[] textItems = new string[2] { "编辑", "删除" };
            foreach (var item in textItems)
            {
                MenuItem _MI = new MenuItem();
                _MI.Header = item;
                _MI.Click += TextBlock_MenuItem_Click;
                textCM.Items.Add(_MI);
            }
            //Grid快捷菜单
            gridCM = new ContextMenu();
            string[] gridItems = new string[13]
            {
            "[联系人]","[公司名称]","[地址]","[手机]","[电话]","[传真]","[日期]",
            "[我方公司]","[我方联系人]","[我方地址]","[我方电话]","[我方传真]","[我方手机]"
            };
            foreach (var item in gridItems)
            {
                MenuItem _MI = new MenuItem();
                _MI.Header = item;
                _MI.Click += Grid_MenuItem_Click;
                gridCM.Items.Add(_MI);
            }
            //expressGrid初始化
            expressGrid.ContextMenu = gridCM;
            expressGrid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;

        }
        #endregion ------构造函数------

        #region    ------方法------
        /// <summary>
        /// 显示快递模板
        /// </summary>
        /// <param name="expTempLate">快递模板</param>
        /// <param name="zoom">显示模板的缩放系数</param>
        public void ShowExpress(ExpressModel expTempLate, double zoom)
        {
            zoomCoefficient = zoom;
            edge = 44 * zoomCoefficient;
            //设置expressGrid
            expressGrid.Children.Clear();
            expressGrid.Width = expTempLate.宽 * 0.96 * zoomCoefficient;
            expressGrid.Height = expTempLate.高 * 0.96 * zoomCoefficient;
            //载入背景图片
            expressImage = new Image();
            expressImage.Width = expressGrid.Width;
            expressImage.Height = expressGrid.Height;
            expressImage.Source = Other.GetImage(expTempLate.ID);
            //添加背景图片和编辑文本框
            expressGrid.Children.Add(expressImage);
            expressGrid.Children.Add(editTB);
            //向expressGrid添加Text元素
            foreach (var item in expTempLate.标签)
            {
                AddTextBlock(item, true);
            }
        }
        /// <summary>
        /// 返回Grid中TextBlock列表
        /// LabelModel形式
        /// </summary>
        /// <returns></returns>
        public List<LabelModel> GetLabelList()
        {
            List<LabelModel> r = new List<LabelModel>();
            foreach (var item in expressGrid.Children)
            {
                TextBlock _TB = item as TextBlock;
                if (_TB != null && _TB.Text != null && _TB.Text.Length != 0)
                {
                    r.Add(new LabelModel(_TB, zoomCoefficient));
                }
            }
            return r;
        }
        /// <summary>
        /// 向expressGrid添加TextBlock
        /// </summary>
        /// <param name="labelInfo">要添加的标签信息</param>
        /// <param name="isZoom">是否执行缩放计算</param>
        private void AddTextBlock(LabelModel labelInfo, bool isZoom)
        {
            if (labelInfo.文本 == null) return;
            string textString = labelInfo.文本.Trim();
            if (textString.Length == 0) return;
            else
            {
                //初始化TextBlock,设置必要的相同属性
                TextBlock elementTB = new TextBlock()
                {
                    Cursor = Cursors.Hand,
                    FontFamily = new FontFamily(labelInfo.字体),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MaxWidth = 288 * zoomCoefficient,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = Brushes.Violet,
                    //Background = new SolidColorBrush(Color.FromArgb(0x50, 0xff, 0, 0xff)),
                    ContextMenu = textCM
                };
                if (isZoom)
                {
                    elementTB.FontSize = labelInfo.字号 * zoomCoefficient;
                    elementTB.Margin = new Thickness(labelInfo.水平边距 * zoomCoefficient, labelInfo.垂直边距 * zoomCoefficient, 0, 0);
                }
                else
                {
                    elementTB.FontSize = labelInfo.字号;
                    elementTB.Margin = new Thickness(labelInfo.水平边距, labelInfo.垂直边距, 0, 0);
                }
                elementTB.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
                elementTB.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
                elementTB.MouseRightButtonUp += TextBlock_MouseRightButtonUp;

                //绑定联系人数据源
                if (isBinding)
                {
                    //替换标签用户信息
                    textString = Other.ReplaceLabel(textString);
                    //需要绑定的联系人数据源
                    Binding binding;
                    switch (textString)
                    {
                        case "[联系人]":
                            binding = new Binding("名称");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[昵称]":
                            binding = new Binding("昵称");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[公司名称]":
                            binding = new Binding("显示公司");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[公司简称]":
                            binding = new Binding("公司简称");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[地址]":
                            binding = new Binding("地址");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[手机]":
                            binding = new Binding("手机优先");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[电话]":
                            binding = new Binding("电话优先");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[传真]":
                            binding = new Binding("传真优先");
                            BindingOperations.SetBinding(elementTB, TextBlock.TextProperty, binding);
                            break;
                        case "[日期]":
                            elementTB.Text = DateTime.Now.ToShortDateString();
                            break;
                        default:
                            elementTB.Text = textString;
                            break;
                    }
                }
                //向expressGrid添加此元素
                expressGrid.Children.Add(elementTB);
            }
        }
        /// <summary>
        /// 把TextBlock设置为编辑状态
        /// </summary>
        /// <param name="editTextBlock">要编辑的TextBlock</param>
        private void EditTextBlock(TextBlock editTextBlock)
        {
            //已在编辑(显示)状态->保存为TextBlock
            if (editTB.Visibility == Visibility.Visible)
            {
                AddTextBlock(new LabelModel(editTB), false);
            }

            editTB.FontFamily = editTextBlock.FontFamily;
            editTB.FontSize = editTextBlock.FontSize;
            //expressTB.Height = editTextBlock.ActualHeight;
            editTB.Margin = editTextBlock.Margin;
            editTB.Text = editTextBlock.Text;
            editTB.Visibility = Visibility.Visible;
            editTB.SelectionStart = editTB.Text.Length;
            editTB.Focus();
        }
        #endregion ------方法------

        #region    ------事件方法------
        /// <summary>
        /// 单击Grid快捷菜单项
        /// </summary>
        private void Grid_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Point MIPoint = gridCM.TranslatePoint(new Point(0, 0), expressGrid);
            LabelModel labelInfo = new LabelModel()
            {
                文本 = ((MenuItem)sender).Header.ToString(),
                字体 = "宋体",
                字号 = 16 * zoomCoefficient,
                水平边距 = MIPoint.X,
                垂直边距 = MIPoint.Y,
                绝对定位 = true
            };
            AddTextBlock(labelInfo, false);
        }
        /// <summary>
        /// 左键单击Grid空白处
        /// </summary>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //已在编辑(显示)状态->保存为TextBlock
            if (editTB.Visibility == Visibility.Visible)
            {
                AddTextBlock(new LabelModel(editTB), false);
                editTB.Visibility = Visibility.Collapsed;
            }
            else
            {
                Point marginPoint = e.GetPosition(expressGrid);
                editTB.FontFamily = new FontFamily("宋体");
                editTB.FontSize = 16 * zoomCoefficient;
                editTB.Margin = new Thickness(marginPoint.X, marginPoint.Y, 0, 0);
                editTB.Text = "";
                editTB.Visibility = Visibility.Visible;
                editTB.SelectAll();
                //editTB.SelectionStart = editTB.Text.Length;
                editTB.Focus();
            }
        }
        /// <summary>
        /// 单击TextBlock快捷菜单项
        /// </summary>
        private void TextBlock_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectTB != null)
            {
                expressGrid.Children.Remove(selectTB);
                if (((MenuItem)sender).Header.ToString() == "编辑")
                {
                    EditTextBlock(selectTB);
                }
            }
        }
        /// <summary>
        /// TextBlock拖拽功能->按下
        /// </summary>
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock _TB = (TextBlock)sender;
            _TB.CaptureMouse();
            _TB.Cursor = Cursors.None;
            _TB.MouseMove += TextBlock_MouseMove;
            //获取鼠标与文本的相对坐标
            textPoint = e.GetPosition(_TB);
            //计算文本在网格中可移动的最大坐标
            gridMaxPoint = new Point(expressGrid.ActualWidth - _TB.ActualWidth - edge, expressGrid.ActualHeight - _TB.ActualHeight);
            e.Handled = true;//防止事件传递到 Grid_MouseLeftButtonDown
        }
        /// <summary>
        /// TextBlock拖拽功能->松开
        /// </summary>
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock _TB = (TextBlock)sender;
            _TB.Cursor = Cursors.Hand;
            _TB.ReleaseMouseCapture();
            _TB.MouseMove -= TextBlock_MouseMove;
        }
        /// <summary>
        /// TextBlock拖拽功能->移动
        /// </summary>
        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            TextBlock _TB = (TextBlock)sender;
            //获取相对坐标
            mousePoint = e.GetPosition(expressGrid);
            //平移文本左上角与鼠标相对坐标
            mousePoint.Offset(-textPoint.X, -textPoint.Y);
            //水平限定
            if (edge > mousePoint.X)
                mousePoint.X = edge;
            else if (mousePoint.X > gridMaxPoint.X)
                mousePoint.X = gridMaxPoint.X;
            //垂直限定
            if (0 > mousePoint.Y)
                mousePoint.Y = 0;
            else if (mousePoint.Y > gridMaxPoint.Y)
                mousePoint.Y = gridMaxPoint.Y;
            //设置边距
            _TB.Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);
            //鼠标在限定范围外指针显示为No图标
            if (mousePoint.X == edge || mousePoint.Y == 0 || mousePoint.X == gridMaxPoint.X || mousePoint.Y == gridMaxPoint.Y)
                _TB.Cursor = Cursors.No;
            else
                _TB.Cursor = Cursors.None;

        }
        /// <summary>
        /// TextBlock弹起右键后选定元素便于操作
        /// </summary>
        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectTB = (TextBlock)sender;
        }
        /// <summary>
        /// TextBox编辑状态按回车保存为TextBlock
        /// </summary>
        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddTextBlock(new LabelModel(editTB), false);
                editTB.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }
        #endregion ------事件方法------

    }
}
