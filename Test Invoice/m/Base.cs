using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Test_Invoice
{
    /// <summary>
    /// Command
    /// </summary>
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute) : this(execute, null) { }
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            //参数execute为空->抛出异常
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    /// <summary>
    /// Command支持泛型参数
    /// </summary>
    class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public DelegateCommand(Action<T> execute) : this(execute, null) { }
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            //参数execute为空->抛出异常
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            Execute((T)parameter);
        }
        public void Execute(T parameter)
        {
            _execute(parameter);
        }
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsVisible)
            {
                return (_canExecute == null);
            }
            return CanExecute((T)parameter);
        }
        public bool CanExecute(T parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    /// <summary>
    /// 消息
    /// ViewModel发送给View的消息
    /// </summary>
    class MessageModel
    {
        string _head, _body;
        /// <summary>
        /// 区分功能 或MessageBox显示的标题
        /// </summary>
        public string Head { get { return _head; } }
        /// <summary>
        /// 功能参数 或MessageBox显示的文本
        /// </summary>
        public string Body { get { return _body; } }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="head">--不能为空--,区分功能 或MessageBox显示的标题</param>
        /// <param name="body">功能参数 或MessageBox显示的文本</param>
        public MessageModel(string head, string body)
        {
            if (head == null)
                throw new ArgumentNullException("head", "消息头不能为空");
            else
                _head = head;

            if (body == null)
                _body = string.Empty;
            else
                _body = body;
        }
    }
    /// <summary>
    /// 侦听消息
    /// View消息响应功能
    /// </summary>
    class ListenMessage : DependencyObject// UIElement// FrameworkElement
    {
        /// <summary>
        /// 获取消息
        /// </summary>
        public MessageModel GetMessage
        {
            get
            {
                return (MessageModel)GetValue(MessageProperty);
            }
        }
        /// <summary>
        /// Message 依赖项属性改变时触发
        /// </summary>
        public event Action MessageChangedEvent;
        /// <summary>
        /// 标识 Message 依赖项属性
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(MessageModel), typeof(ListenMessage), new PropertyMetadata(null, new PropertyChangedCallback(OnMessagePropertyChanged)));
        /// <summary>
        /// Message 依赖项属性更改回调方法
        /// </summary>
        private static void OnMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ListenMessage lm = (ListenMessage)d;
                if (lm.MessageChangedEvent != null)
                    lm.MessageChangedEvent();
            }
        }
    }
    /// <summary>
    /// 实现INotifyPropertyChanged接口的抽象基类
    /// 实时通知页面绑定数据更新
    /// </summary>
    abstract class ViewModelBase : INotifyPropertyChanged
    {
        //------属性------
        /// <summary>
        /// 消息
        /// 绑定到View的ListenMessage.MessageProperty依赖属性
        /// </summary>
        public MessageModel Message { get; set; }
        //------命令属性------
        /// <summary>
        /// 退出
        /// 向View发送消息"Quit"
        /// </summary>
        public DelegateCommand QuitCMD { get; set; }
        //------事件------
        /// <summary>
        /// 实现INotifyPropertyChanged接口
        /// 通知View绑定值已更改的事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        //------方法------
        /// <summary>
        /// 构造函数
        /// 初始化QuitCMD
        /// </summary>
        public ViewModelBase()
        {
            QuitCMD = new DelegateCommand(Quit);
        }
        /// <summary>
        /// 通知View更新绑定属性数据的方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 发送消息
        /// ViewModel向View发送消息
        /// </summary>
        /// <param name="head">--不能为空--功能命令 或MessageBox显示的标题</param>
        /// <param name="body">功能参数 或MessageBox显示的文本</param>
        protected void SendMessage(string head, string body)
        {
            Message = new MessageModel(head, body);
            OnPropertyChanged("Message");
        }
        /// <summary>
        /// 向View发送消息"Quit"
        /// </summary>
        /// <param name="o"></param>
        private void Quit(object o)
        {
            SendMessage("Quit", null);
        }
    }
    /// <summary>
    /// 实现单机版本和网络版本共存
    /// 还未使用
    /// </summary>
    interface DBManipulate
    {
    }

    /// <summary>
    /// 全局信息
    /// </summary>
    class Global
    {
        private static UserModel _user;
        private static List<CompanyModel> _allCompanies;
        private static List<ContactModel> _allContacts;
        private static List<ExpressModel> _allExpresses;
        /// <summary>
        /// 图片存放路径
        /// </summary>
        public const string ImagePath = "Infos\\Images\\[图片].jpg";
        /// <summary>
        /// 模板及配置文件路径
        /// </summary>
        public const string ConfigPath = "Infos\\Config.xml";
        /// <summary>
        /// 智能信息路径
        /// </summary>
        public const string SmartPath = "Infos\\Smart.xml";

        public static MySQL SQL { get; set; }
        public static UserModel User { get { return _user; } }
        public static List<CompanyModel> AllCompanies { get { return _allCompanies; } }
        public static List<ContactModel> AllContacts { get { return _allContacts; } }
        public static List<ExpressModel> AllExpresses { get { return _allExpresses; } }


        /// <summary>
        /// 初始化数据源
        /// 请在程序运行时马上执行,并配合合理的异常处理
        /// </summary>
        public static void Initialize()
        {
            _user = SQL.IDSelectUser<UserModel>(Guid.Empty);
            LoadCompanies();
            LoadContacts();
            LoadExpresses();
        }
        public static void LoadCompanies() { _allCompanies = SQL.SelectAllCompanies<CompanyModel>(); }
        public static void LoadContacts() { _allContacts = SQL.SelectAllContacts<ContactModel>(); }
        public static void LoadExpresses() { _allExpresses = XmlLinq.SelectAllExpresses<ExpressModel>(); }
    }
}
