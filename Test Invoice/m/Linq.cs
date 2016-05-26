using System.Linq;
using System.Xml.Linq;
using System.Drawing.Printing;
using System;
using System.Collections.Generic;

namespace Test_Invoice
{
    #region Xml读取数据
    class XmlLinq
    {
        /// <summary>
        /// 获取全部快递模板息
        /// </summary>
        public static List<T> SelectAllExpresses<T>() where T : ExpressModel, new()
        {
            List<T> r = new List<T>();
            var _XEList = XElement.Load(Global.ConfigPath).Elements("ExpTemplate");
            foreach (var item in _XEList)
            {
                r.Add(new T()
                {
                    ID = Guid.Parse(item.Attribute("ID").Value),
                    名称 = item.Attribute("Name").Value,
                    宽 = int.Parse(item.Attribute("Width").Value),
                    高 = int.Parse(item.Attribute("Height").Value),
                    水平平移 = int.Parse(item.Attribute("HorizontalMove").Value),
                    垂直平移 = int.Parse(item.Attribute("VerticalMove").Value),
                    标签 = item.Elements().Select(_func => new LabelModel()
                    {
                        文本 = _func.Value,
                        字体 = _func.Attribute("FontFamily").Value,
                        字号 = int.Parse(_func.Attribute("FontSize").Value),
                        水平边距 = int.Parse(_func.Attribute("HorizontalMargin").Value),
                        //水平对齐 =
                        垂直边距 = int.Parse(_func.Attribute("VerticalMargin").Value),
                        //垂直对齐 =
                        //行号 =
                        绝对定位 = bool.Parse(_func.Attribute("Absolute").Value)
                    }).ToList()
                });
            }
            //替换标签用户信息
            //foreach (var item in r)
            //{
            //    foreach (var item2 in item.标签)
            //    {
            //        item2.文本 = Other.ReplaceLabel(item2.文本);
            //    }
            //}
            return r;
        }
        /// <summary>
        /// 发货单模板
        /// </summary>
        public static BillModel GetInvoiceTemplate(InvoiceModelPrint invoicePrintInfo, bool payment)
        {
            BillModel r;
            string _nameString;
            if (payment)
                _nameString = "现金";
            else
                _nameString = "赊欠";
            //载入配置文件数据
            XElement _XE = XElement.Load(Global.ConfigPath);
            XElement TemplateXE = _XE.Elements("InvoiceTemplate").First(_func => _func.Attribute("Name").Value == _nameString);
            _nameString = TemplateXE.Attribute("PaperName").Value;
            XElement paperSizeXE = _XE.Elements("PaperSize").First(_func => _func.Attribute("Name").Value == _nameString);
            PaperSize paperSize = new PaperSize(_nameString, int.Parse(paperSizeXE.Attribute("Width").Value), int.Parse(paperSizeXE.Attribute("Height").Value));
            //初始化发货单模板
            r = new BillModel()
            {
                纸张尺寸 = paperSize,
                水平边距 = int.Parse(TemplateXE.Attribute("HorizontalMargin").Value),
                水平平移 = int.Parse(TemplateXE.Attribute("HorizontalMove").Value),
                垂直边距 = int.Parse(TemplateXE.Attribute("VerticalMargin").Value),
                垂直平移 = int.Parse(TemplateXE.Attribute("VerticalMove").Value),

                表字体 = TemplateXE.Element("Table").Attribute("FontFamily").Value,
                表字号 = int.Parse(TemplateXE.Element("Table").Attribute("FontSize").Value),
                表行号 = int.Parse(TemplateXE.Element("Table").Attribute("RowNumber").Value),

                段落 = TemplateXE.Elements("Row").Select(_func => new RowModel
                {
                    行号 = int.Parse(_func.Attribute("Number").Value),
                    行高 = int.Parse(_func.Attribute("Height").Value),
                }).ToList(),

                标签 = TemplateXE.Elements("Label").Select(_func => new LabelModel
                {
                    文本 = _func.Value,
                    字体 = _func.Attribute("FontFamily").Value,
                    字号 = int.Parse(_func.Attribute("FontSize").Value),
                    水平边距 = int.Parse(_func.Attribute("HorizontalMargin").Value),
                    水平对齐 = _func.Attribute("Horizontal").Value,
                    垂直边距 = int.Parse(_func.Attribute("VerticalMargin").Value),
                    垂直对齐 = _func.Attribute("Vertical").Value,
                    行号 = int.Parse(_func.Attribute("RowNumber").Value),
                    绝对定位 = bool.Parse(_func.Attribute("Absolute").Value)
                }).ToList(),

                表列 = TemplateXE.Element("Table").Elements("Column").Select(_func => new TableColumnModel
                {
                    绑定 = _func.Attribute("Binding").Value,
                    表头 = _func.Attribute("Hesder").Value,
                    列宽 = int.Parse(_func.Attribute("Width").Value),
                    水平对齐 = _func.Attribute("Horizontal").Value
                }).ToList()
            };


            //替换标签模板信息
            string pageNumber = string.Format("{0}\r\n{1}", invoicePrintInfo.分页码, invoicePrintInfo.总页码.ToString("'No.'00000000"));
            string totalAmount = (invoicePrintInfo.金额 == 0 ? "" : string.Format("{0}　{1}", Other.MoneyToCN(invoicePrintInfo.金额), invoicePrintInfo.金额.ToString("C")));
            foreach (var item in r.标签)
            {
                item.文本 = Other.ReplaceLabel(item.文本)
                    .Replace("[页码]", pageNumber)
                    .Replace("[总金额]", totalAmount)
                    .Replace("[公司名称]", invoicePrintInfo.公司名称)
                    .Replace("[赊欠金额]", invoicePrintInfo.赊欠金额.ToString("C"))
                    .Replace("[联系人]", invoicePrintInfo.联系人)
                    .Replace("[备注]", invoicePrintInfo.Comment)
                    .Replace("[打印日期]", invoicePrintInfo.打印时间.ToLongDateString());
            }
            return r;
        }
    }
    #endregion
}
