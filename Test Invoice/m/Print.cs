using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace Test_Invoice
{
    /// <summary>
    /// 单据打印
    /// </summary>
    class BillPrint<T> : IDisposable where T : InvoiceListModel
    {
        #region  -----私有变量----
        PrintDocument pDocument;
        /// <summary>
        /// 清单模板
        /// </summary>
        BillModel billTemplate;
        /// <summary>
        /// 表数据
        /// </summary>
        List<T> invoiceListInfo;
        /// <summary>
        /// 打印宽度,单位:1/100英寸
        /// </summary>
        int pWidth;
        /// <summary>
        /// 当打印高度,单位:1/100英寸
        /// </summary>
        int pHeight;
        /// <summary>
        /// 表格打印高度,单位:1/100英寸
        /// </summary>
        int tHeight;
        /// <summary>
        /// 表行高(1.5倍行间距),单位:1/100英寸
        /// </summary>
        int tRowHeight;
        /// <summary>
        /// 表行总数
        /// </summary>
        int tRowCount;
        /// <summary>
        /// 表打印完成行数
        /// </summary>
        int tRowComplete;
        /// <summary>
        /// 当前打印字符串
        /// </summary>
        string pString;
        /// <summary>
        /// 当前绘制行起点,单位:1/100英寸
        /// </summary>
        Point pStartPoint;
        /// <summary>
        /// 当前绘制文本的格式,FontSize单位:磅
        /// </summary>
        Font pFont;
        /// <summary>
        /// 当前绘制文本的宽高,单位:1/100英寸
        /// </summary>
        SizeF pSizeF;
        /// <summary>
        /// 黑色画笔,绘制文字
        /// </summary>
        SolidBrush pBrush = new SolidBrush(Color.Black);
        /// <summary>
        /// 当前绘制文本的左上角点,单位:1/100英寸
        /// </summary>
        System.Windows.Point pPoint = new System.Windows.Point();
        /// <summary>
        /// 黑色线条笔,绘制表格
        /// </summary>
        Pen tPen = new Pen(Color.Black, 1);
        /// <summary>
        /// 水平边距,单位:1/100英寸
        /// </summary>
        int HorizontalMargin;
        /// <summary>
        /// 水平平移,单位:1/100英寸
        /// </summary>
        int HorizontalMove;
        /// <summary>
        /// 垂直边距,单位:1/100英寸
        /// </summary>
        int VerticalMargin;
        /// <summary>
        /// 垂直平移,单位:1/100英寸
        /// </summary>
        int VerticalMove;
        #endregion

        #region    -----构造函数,析构函数----
        public BillPrint(BillModel bt, List<T> invoiceList)
        {
            //初始化
            billTemplate = bt;
            invoiceListInfo = invoiceList;
            //打印事件
            pDocument = new PrintDocument();
            pDocument.PrintPage += pDocument_PrintPage;
            pDocument.DocumentName = "发货单";
            pDocument.DefaultPageSettings.PaperSize = billTemplate.纸张尺寸;

            //----页面信息----
            HorizontalMargin = (int)(billTemplate.水平边距 / 0.254f);
            HorizontalMove = (int)(billTemplate.水平平移 / 0.254f);
            VerticalMargin = (int)(billTemplate.垂直边距 / 0.254f);
            VerticalMove = (int)(billTemplate.垂直平移 / 0.254f);


            pWidth = billTemplate.纸张尺寸.Width - HorizontalMargin * 2;
            pHeight = billTemplate.纸张尺寸.Height - VerticalMargin * 2;
            tHeight = pHeight - billTemplate.段落.Sum(_func => _func.行高);
            tRowHeight = billTemplate.表字号 * 3 / 2;
            tRowCount = invoiceListInfo.Count;
            //计算正确表列宽
            //double _double = (double)pWidth / (double)billTemplate.表列.Sum(_func => _func.列宽);
            //foreach (var item in billTemplate.表列)
            //{
            //    item.列宽 = (int)(item.列宽 * _double);
            //}
            //下有自适应文字宽度
        }
        /// <summary>
        /// 手动释放有BillPrint使用的所有资源
        /// </summary>
        public void Dispose()
        {
            pDocument.Dispose();
            pBrush.Dispose();
            pFont.Dispose();
            tPen.Dispose();
            ////调用带参数的Dispose方法，释放托管和非托管资源
            //Dispose(true);
            ////手动调用了Dispose释放资源，那么析构函数就是不必要的了，这里阻止GC调用析构函数
            //System.GC.SuppressFinalize(this);
        }
        ///// <summary>
        ///// 传入bool值disposing以确定是否释放托管资源
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //TODO:在这里加入清理"托管资源"的代码，应该是xxx.Dispose();
        //    }
        //    //TODO:在这里加入清理"非托管资源"的代码
        //}
        ///// <summary>
        ///// 供GC调用的析构函数
        ///// </summary>
        //~BillPrint()
        //{
        //    //释放非托管资源
        //    Dispose(false);
        //}
        #endregion -----构造函数,析构函数----

        //绘制打印页面
        void pDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;//获得绘图对象
            if (tRowComplete == 0)
            {
                //自适应文字宽度
                pFont = new Font(billTemplate.表字体, billTemplate.表字号 * 0.75f, FontStyle.Regular);

                double minWidth = g.MeasureString("俩字", pFont).Width;
                double _Width;
                foreach (var item in billTemplate.表列)
                {
                    switch (item.绑定)
                    {
                        case "单价":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.单价.ToString(), pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "金额":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString((item2.数量 * item2.单价).ToString("0.00#"), pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "名称":
                            item.列宽 = minWidth * 2;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.名称, pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "单位":
                            item.列宽 = minWidth;
                            //foreach (var item2 in invoiceListInfo)
                            //{
                            //    _Width = g.MeasureString(item2.单位, pFont).Width;
                            //    if (item.列宽 < _Width)
                            //    {
                            //        item.列宽 = _Width;
                            //    }
                            //}
                            break;
                        case "数量":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.数量.ToString(), pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "编号":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.编号, pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "规格":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.规格, pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "颜色":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.颜色, pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                        case "备注":
                            item.列宽 = minWidth;
                            foreach (var item2 in invoiceListInfo)
                            {
                                _Width = g.MeasureString(item2.备注, pFont).Width;
                                if (item.列宽 < _Width)
                                {
                                    item.列宽 = _Width;
                                }
                            }
                            break;
                    }
                }
                _Width = (pWidth - billTemplate.表列.Sum(_func => _func.列宽)) / billTemplate.表列.Count;
                foreach (var item in billTemplate.表列)
                {
                    item.列宽 += _Width;
                }
            }




            //当前绘制行起点
            pStartPoint = new Point(HorizontalMargin + HorizontalMove, VerticalMargin + VerticalMove);

            foreach (var row in billTemplate.段落.OrderBy(_func => _func.行号))
            {
                //是绘制表的行->绘制表>>继续
                if (row.行号 == billTemplate.表行号)
                {
                    //本页打印行数
                    int tableRows;
                    //打印行高>可打印行高->分页打印>>继续
                    if ((tRowCount - tRowComplete + 2) * tRowHeight > tHeight)
                    {
                        //打印最多可打印行数
                        tableRows = tHeight / tRowHeight - 2;
                        //有下一页
                        e.HasMorePages = true;
                    }
                    else//其他->打印剩余行>>继续
                    {
                        //打印剩余行数
                        tableRows = tRowCount - tRowComplete;
                        //无下一页
                        e.HasMorePages = false;
                    }
                    //画框
                    g.DrawRectangle(tPen, pStartPoint.X, pStartPoint.Y, pWidth, (tableRows + 2) * tRowHeight);
                    //画表头和统计行横线
                    int _tempInt = pStartPoint.Y + tRowHeight;
                    g.DrawLine(tPen, pStartPoint.X, _tempInt, pStartPoint.X + pWidth, _tempInt);
                    _tempInt = (tableRows + 1) * tRowHeight + pStartPoint.Y;
                    g.DrawLine(tPen, pStartPoint.X, _tempInt, pStartPoint.X + pWidth, _tempInt);

                    //计算表头垂直居中点
                    pFont = new Font(billTemplate.表字体, billTemplate.表字号 * 0.75f, FontStyle.Bold);
                    pSizeF = g.MeasureString("头", pFont);
                    pPoint.Y = (tRowHeight - pSizeF.Height) * 0.5F + pStartPoint.Y + 2;
                    //绘制表头和竖线
                    float _tempX = pStartPoint.X;
                    bool _bool = false;//跳过首列竖线绘制
                    foreach (var item in billTemplate.表列)
                    {
                        //保存当前列起点X坐标
                        item.X = _tempX;
                        //计算水平居中位置绘制文字
                        pSizeF = g.MeasureString(item.表头, pFont);
                        pPoint.X = (item.列宽 - pSizeF.Width) / 2 + _tempX + 1;
                        g.DrawString(item.表头, pFont, pBrush, (float)pPoint.X, (float)pPoint.Y);
                        //跳过首列绘制竖线
                        if (_bool) { g.DrawLine(tPen, _tempX, pStartPoint.Y, _tempX, _tempInt); }
                        else _bool = true;
                        //下一列起点
                        _tempX += (float)item.列宽;
                    }
                    //表头打印完切入下一行
                    pStartPoint.Y += tRowHeight;

                    //绘制表内容
                    //计算第一行垂直居中点
                    pFont = new Font(billTemplate.表字体, billTemplate.表字号 * 0.75f, FontStyle.Regular);
                    pSizeF = g.MeasureString("行", pFont);
                    pPoint.Y = (tRowHeight - pSizeF.Height) * 0.5F + pStartPoint.Y + 2;
                    //小计金额
                    decimal moneySum = 0;
                    //tableRows现在为打印到第几行(行号为0开头判断时不需要<=)
                    tableRows += tRowComplete;
                    for (; tRowComplete < tableRows; tRowComplete++)
                    {
                        foreach (var item in billTemplate.表列)
                        {
                            switch (item.绑定)
                            {
                                case "单价":
                                    if (invoiceListInfo[tRowComplete].单价 != 0)
                                        pString = invoiceListInfo[tRowComplete].单价.ToString("0.00#");
                                    else
                                        pString = "";
                                    break;
                                case "金额":
                                    if (invoiceListInfo[tRowComplete].单价 != 0)
                                    {
                                        decimal 金额 = invoiceListInfo[tRowComplete].单价 * invoiceListInfo[tRowComplete].数量;
                                        moneySum += 金额;
                                        pString = 金额.ToString("0.00#");
                                    }
                                    else
                                        pString = "";
                                    break;
                                case "名称":
                                    pString = invoiceListInfo[tRowComplete].名称;
                                    break;
                                case "单位":
                                    pString = invoiceListInfo[tRowComplete].单位;
                                    break;
                                case "数量":
                                    pString = invoiceListInfo[tRowComplete].数量.ToString();
                                    break;
                                case "编号":
                                    pString = invoiceListInfo[tRowComplete].编号;
                                    break;
                                case "规格":
                                    pString = invoiceListInfo[tRowComplete].规格;
                                    break;
                                case "颜色":
                                    pString = invoiceListInfo[tRowComplete].颜色;
                                    break;
                                case "备注":
                                    pString = invoiceListInfo[tRowComplete].备注;
                                    break;
                            }

                            //打印字符串不为空->计算打印位置并绘制>>继续
                            if (pString != null && pString.Length != 0)
                            {
                                pSizeF = g.MeasureString(pString, pFont);
                                switch (item.水平对齐)
                                {
                                    case "Center":
                                        pPoint.X = (item.列宽 - pSizeF.Width) * 0.5F + item.X + 1;
                                        break;
                                    case "Right":
                                        pPoint.X = item.X + item.列宽 - pSizeF.Width;
                                        break;
                                    default://"Left"
                                        pPoint.X = item.X;
                                        break;
                                }

                                g.DrawString(pString, pFont, pBrush, (float)pPoint.X, (float)pPoint.Y);
                            }
                        }
                        //打印完一行切入下一行
                        pStartPoint.Y += tRowHeight;
                        //垂直中心点也切入下一行
                        pPoint.Y += tRowHeight;
                    }
                    //打印小计
                    pPoint.X = pStartPoint.X;
                    g.DrawString("小计:", pFont, pBrush, (float)pPoint.X, (float)pPoint.Y);
                    pString = moneySum.ToString("C");
                    //最后一列的开头减去字符串长度
                    pPoint.X = billTemplate.表列.LastOrDefault().X - g.MeasureString(pString, pFont).Width;
                    g.DrawString(pString, pFont, pBrush, (float)pPoint.X, (float)pPoint.Y);
                    //打印完一行切入下一行
                    pStartPoint.Y += tRowHeight;
                }
                else//其他->绘制标签>>继续
                {
                    foreach (var item in billTemplate.标签.Where(_func => _func.行号 == row.行号))
                    {
                        pString = item.文本;
                        pFont = new Font(item.字体, (float)item.字号 * 0.75f, FontStyle.Regular);
                        pSizeF = g.MeasureString(pString, pFont);

                        switch (item.水平对齐)
                        {
                            case "Center":
                                pPoint.X = (pWidth - pSizeF.Width) * 0.5F + pStartPoint.X;
                                break;
                            case "Right":
                                pPoint.X = pWidth - pSizeF.Width + pStartPoint.X - item.水平边距;
                                break;
                            default://"Left"
                                pPoint.X = pStartPoint.X + item.水平边距;
                                break;
                        }

                        switch (item.垂直对齐)
                        {
                            case "Center":
                                pPoint.Y = (row.行高 - pSizeF.Height) * 0.5F + pStartPoint.Y;
                                break;
                            case "Bottom":
                                pPoint.Y = row.行高 - pSizeF.Height + pStartPoint.Y - item.垂直边距;
                                break;
                            default://"Top"
                                pPoint.Y = pStartPoint.Y + item.垂直边距;
                                break;
                        }
                        g.DrawString(pString, pFont, pBrush, (float)pPoint.X, (float)pPoint.Y);
                    }
                    pStartPoint.Y += row.行高;
                }
            }
        }
        /// <summary>
        /// 执行送货单打印
        /// </summary>
        public void PrintPage()
        {
            //PrintDialog pDialog = new PrintDialog();
            //pDialog.Document = pDocument;
            //if (pDialog.ShowDialog() == DialogResult.OK)
            //{
            pDocument.Print();
            //}
        }
        public void PreviewPage()
        {
            //调整预览和打印边距差异
            HorizontalMove += 52;
            //打印预览
            PrintPreviewDialog pPreviewDialog = new PrintPreviewDialog();
            //隐藏打印预览中的打印按钮(在预览中打印无效,数据还未实际更新到数据库)
            ToolStrip toolStrip = pPreviewDialog.Controls["toolStrip1"] as ToolStrip;
            if (toolStrip != null)
                toolStrip.Items["printToolStripButton"].Visible = false;
            pPreviewDialog.Width = 800;
            pPreviewDialog.Height = 500;
            pPreviewDialog.PrintPreviewControl.Zoom = 1;
            pPreviewDialog.Document = pDocument;
            pPreviewDialog.ShowDialog();

        }
    }

    /// <summary>
    /// 标签打印
    /// </summary>
    class LabelPrint : IDisposable
    {
        #region  -----私有变量----
        PrintDocument pDocument;
        /// <summary>
        /// 标签信息
        /// </summary>
        LabelPrintModel labelInfo;
        #endregion  -----私有变量----

        #region  -----构造函数,析构函数----
        /// <summary>
        /// 初始化标签打印
        /// </summary>
        /// <param name="LabelPrintInfo">标签信息</param>
        public LabelPrint(LabelPrintModel LabelPrintInfo) : this(LabelPrintInfo, "打印标签") { }
        /// <summary>
        /// 初始化标签打印
        /// </summary>
        /// <param name="LabelPrintInfo">标签信息</param>
        /// <param name="name">显示在打印列表中的名称</param>
        public LabelPrint(LabelPrintModel LabelPrintInfo, string name)
        {
            labelInfo = LabelPrintInfo;
            pDocument = new PrintDocument();
            pDocument.DefaultPageSettings.PaperSize = labelInfo.纸张尺寸;
            pDocument.DocumentName = name;
            pDocument.PrintPage += Express_PrintPage;
        }
        /// <summary>
        /// 手动释放有LabelPrint使用的所有资源
        /// </summary>
        public void Dispose()
        {
            pDocument.Dispose();
            ////调用带参数的Dispose方法，释放托管和非托管资源
            //Dispose(true);
            ////手动调用了Dispose释放资源，那么析构函数就是不必要的了，这里阻止GC调用析构函数
            //System.GC.SuppressFinalize(this);
        }
        ///// <summary>
        ///// 传入bool值disposing以确定是否释放托管资源
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //TODO:在这里加入清理"托管资源"的代码，应该是xxx.Dispose();
        //        pDocument.Dispose();
        //    }
        //    //TODO:在这里加入清理"非托管资源"的代码
        //}
        ///// <summary>
        ///// 供GC调用的析构函数
        ///// </summary>
        //~LabelPrint()
        //{
        //    //释放非托管资源
        //    Dispose(false);
        //}
        #endregion  -----构造函数,析构函数----

        #region  -----方法----
        /// <summary>
        /// 打印预览
        /// </summary>
        public void Preview()
        {
            PrintPreviewDialog pPreviewDialog = new PrintPreviewDialog();
            pPreviewDialog.Document = pDocument;
            pPreviewDialog.ShowDialog();

        }
        /// <summary>
        /// 打印
        /// </summary>
        public void Print()
        {
            pDocument.Print();
        }
        /// <summary>
        /// 高级打印(选择打印机)
        /// </summary>
        public void AdvancedPrint()
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.Document = pDocument;
            pDialog.AllowCurrentPage = false;
            pDialog.AllowPrintToFile = false;
            pDialog.AllowSelection = false;
            pDialog.AllowSomePages = false;
            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                pDocument.Print();
            }
        }
        #endregion  -----方法----

        #region  -----事件方法----
        private void Express_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush pBrush = new SolidBrush(Color.Black);//黑色画笔
            RectangleF pRectangle = new RectangleF(0, 0, labelInfo.文本长度, 0);
            Font pFont;

            foreach (var item in labelInfo.标签)
            {
                //FontSize 像素转为磅
                pFont = new Font(item.字体, (float)item.字号 * 0.75f, FontStyle.Regular);
                pRectangle.X = (float)(labelInfo.起始点.X + item.水平边距 / 0.96);
                pRectangle.Y = (float)(labelInfo.起始点.Y + item.垂直边距 / 0.96);
                g.DrawString(item.文本, pFont, pBrush, pRectangle);
            }
        }
        #endregion  -----事件方法----
    }
}