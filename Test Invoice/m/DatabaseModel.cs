using System;

namespace Test_Invoice
{
    #region------数据库模型------
    /// <summary>
    /// 公司模型(数据库)17
    /// </summary>
    class CompanyModel
    {
        public Guid ID { get; set; }
        public Guid 用户ID { get; set; }
        public Guid? 图片ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 名称 { get; set; }
        /// <summary>
        /// 256字符
        /// </summary>
        public string 拼音 { get; set; }
        public int 发货单分页码 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 累计金额 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 赊欠金额 { get; set; }
        public bool 赊欠 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 税号 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string 地址 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 电话 { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 银行 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 账号 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 联系人模型(数据库)16
    /// </summary>
    class ContactModel
    {
        public Guid ID { get; set; }
        public Guid 用户ID { get; set; }
        public Guid 公司ID { get; set; }
        public Guid? 图片ID { get; set; }
        public Guid? 快递ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 名称 { get; set; }
        /// <summary>
        /// 256字符
        /// </summary>
        public string 拼音 { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 昵称 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string 地址 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 手机 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 电话 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 传真 { get; set; }
        public bool? 性别 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 发货单模型(数据库)16
    /// </summary>
    class InvoiceModel
    {
        public Guid ID { get; set; }
        public Guid 用户ID { get; set; }
        public Guid 公司ID { get; set; }
        public Guid? 对账单ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 联系人 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 金额 { get; set; }
        public int 总页码 { get; set; }
        public int 分页码 { get; set; }
        public bool 打印 { get; set; }
        public bool 对账 { get; set; }
        public bool 付款 { get; set; }
        public bool 作废 { get; set; }
        public DateTime 打印时间 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 发货清单模型(数据库)13
    /// </summary>
    class InvoiceListModel
    {
        public Guid ID { get; set; }
        public Guid 发货单ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 名称 { get; set; }
        /// <summary>
        /// 16字符
        /// </summary>
        public string 单位 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 数量 { get; set; }
        /// <summary>
        /// 16位长度,5位小数
        /// </summary>
        public decimal 单价 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 编号 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 规格 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 颜色 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 备注 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 对账单模型(数据库)9
    /// </summary>
    class StatementModel
    {
        public Guid ID { get; set; }
        public Guid 用户ID { get; set; }
        public Guid 公司ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 标题 { get; set; }
        public DateTime 起始日期 { get; set; }
        public DateTime 截止日期 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 用户模型(数据库)13
    /// </summary>
    class UserModel
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 公司名称 { get; set; }
        /// <summary>
        /// 64字符
        /// </summary>
        public string 联系人 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string 地址 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 电话 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 传真 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 手机 { get; set; }
        /// <summary>
        /// 32字符
        /// </summary>
        public string 版本 { get; set; }
        public bool 默认赊欠 { get; set; }
        public int 发货单总页码 { get; set; }
        public int 对账单总页码 { get; set; }
        /// <summary>
        /// 128字符
        /// </summary>
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    #endregion------数据库模型------
}
