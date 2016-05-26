using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Test_Invoice
{
    class Other
    {
        /// <summary>
        /// 把数字转换成人民币大写
        /// </summary>
        /// <param name="Money">要转换的数字</param>
        /// <returns></returns>
        public static string MoneyToCN(decimal Money)
        {
            Money = Math.Round(Money, 2);//四舍五入
            string cnNumber = "零壹贰叁肆伍陆柒捌玖";
            string cnUnit = "圆拾佰仟万拾佰仟亿拾佰仟万拾佰仟";
            string[] MoneyString = Money.ToString().Split('.');
            string intString = MoneyString[0];//整数部分
            int moneyLength = intString.Length - 1;//以0起始的先减去1
            string r = "⊗";//返回值
            int bitMoney = 0;//存放数字的每一位
            bool zero = false;//记录数字中间的零位
            for (int i = moneyLength; i >= 0; i--)
            {
                bitMoney = intString[moneyLength - i] - 48;//0的ASCII码是48
                if (0 == bitMoney)//出现零的情况
                {
                    if (0 == moneyLength)
                    {
                        r += '零';//在整数部分intString为零时,0.X为零圆X角,零在圆前面已经有了标记为无零位
                    }
                    else
                    {
                        zero = true;//其他情况标记有零位
                    }
                    //在遇到圆，万，亿的情况下要添加单位
                    if (i == 0 || i == 4 || i == 8 || i == 12)
                    {
                        r += cnUnit[i];
                    }
                }
                else
                {
                    //有零位标记先填充零，再标记无零位
                    if (zero)
                    {
                        r += '零';
                        zero = false;
                    }
                    try
                    {
                        //1～9，填充大写数字位和单位
                        r = r + cnNumber[bitMoney] + cnUnit[i];
                    }
                    catch (System.Exception)
                    {
                        //如果应数字过大过小或其他原因返回错误
                        return "错误";
                    }

                }

            }
            //有小数位处理
            if (1 < MoneyString.Length)
            {
                //角这位肯定有值的
                string decString = MoneyString[1];
                bitMoney = decString[0] - 48;//0的ASCII码是48
                if (zero)
                {
                    //整数位有余留下来的零位也要填零 X0.X为X拾圆零X角
                    r += '零';
                }
                if (0 != bitMoney)
                {
                    //1～9，填充大写数字位和单位
                    r = r + cnNumber[bitMoney] + '角';
                }
                else if (!zero)
                {
                    //角位为零并且没有整数位余留下来的零位,要填充零,角位为零,分位必定有值
                    r += '零';
                }
                if (1 < decString.Length)
                {
                    bitMoney = decString[1] - 48;//0的ASCII码是48
                    r = r + cnNumber[bitMoney] + '分';
                }
                else
                {
                    //无分位时填充
                    r += '整';
                }
            }
            else
            {
                //无小数时填充
                r += '整';
            }
            return r;
        }
        /// <summary>
        /// 公司简称
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string ShortCompanyName(string CompanyName)
        {
            return CompanyName.Replace("有限公司", "").Replace("市", "").Replace("县", "").Replace("进出口", "").Replace("有限责任公司", "");
        }
        public static string CompanyNameToSpell(string CompanyName)
        {
            return Chinese2Spell.Convert(ShortCompanyName(CompanyName), false, "[字母] [拼音]").ToLower();
        }
        /// <summary>
        /// 取得一个文本文件流的编码方式.
        /// </summary>
        /// <param name="fileStream">文本文件流</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream fileStream)
        {
            Encoding r = Encoding.Default;
            if (fileStream != null && fileStream.Length >= 2)
            {
                //保存文件流的前4个字节
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;

                //保存当前Seek位置
                long origPos = fileStream.Seek(0, SeekOrigin.Begin);
                byte1 = Convert.ToByte(fileStream.ReadByte());
                byte2 = Convert.ToByte(fileStream.ReadByte());
                if (fileStream.Length >= 3)
                {
                    byte3 = Convert.ToByte(fileStream.ReadByte());
                }
                if (fileStream.Length >= 4)
                {
                    byte4 = Convert.ToByte(fileStream.ReadByte());
                }

                //根据文件流的前4个字节判断Encoding
                //Unicode = {0xFF, 0xFE};
                //BE-Unicode = {0xFE, 0xFF};
                //UTF8 = {0xEF, 0xBB, 0xBF};
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
                {
                    r = Encoding.BigEndianUnicode;
                }

                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
                {
                    r = Encoding.Unicode;
                }

                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
                {
                    r = Encoding.UTF8;
                }

                //恢复Seek位置
                fileStream.Seek(origPos, SeekOrigin.Begin);
            }
            return r;
        }
        /// <summary>
        /// 把字符串中的全角字符转换为半角字符
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="SBC"></param>
        /// <returns></returns>
        public static string SBC2DBC(string SBC)
        {
            char[] r = SBC.ToCharArray();
            for (int i = 0; i < r.Length; i++)
            {
                //全角字符
                if (65280 < r[i] && 65375 > r[i])
                    r[i] = (char)(r[i] - 65248);
                //中文标点
                if (8215 < r[i] && 12306 > r[i])
                    switch (r[i])
                    {
                        case '　':
                            r[i] = ' ';
                            break;
                        case '、':
                            r[i] = '\\';
                            break;
                        case '。':
                            r[i] = '.';
                            break;
                        case '“':
                        case '”':
                            r[i] = '"';
                            break;
                        case '‘':
                        case '’':
                            r[i] = '\'';
                            break;
                        case '『':
                        case '【':
                            r[i] = '[';
                            break;
                        case '』':
                        case '】':
                            r[i] = ']';
                            break;
                        default:
                            break;
                    }
            }
            return new string(r);
        }
        /// <summary>
        /// 替换标签用户信息
        /// </summary>
        public static string ReplaceLabel(string LabelString)
        {
            return LabelString
                .Replace("[我方公司]", Global.User.公司名称)
                .Replace("[我方联系人]", Global.User.联系人)
                .Replace("[我方地址]", Global.User.地址)
                .Replace("[我方电话]", Global.User.电话)
                .Replace("[我方传真]", Global.User.传真)
                .Replace("[我方手机]", Global.User.手机)
                .Replace("[当前时间]", DateTime.Now.ToString("yyyy年MM月dd日HH时mm分"));
        }
        /// <summary>
        /// 图片载入内存
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns></returns>
        public static BitmapImage GetImage(Guid id)
        {
            BitmapImage r = new BitmapImage();
            MemoryStream imgStream = new MemoryStream(File.ReadAllBytes(Global.ImagePath.Replace("[图片]", id.ToString())));
            r = new BitmapImage();
            r.BeginInit();
            r.StreamSource = imgStream;
            r.EndInit();
            return r;
        }

    }
}
