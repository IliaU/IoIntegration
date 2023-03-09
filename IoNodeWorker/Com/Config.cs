using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using IoNodeWorker.Lib;

namespace IoNodeWorker.Com
{
    /// <summary>
    /// Класс для работы с конфиг файлом
    /// </summary>
    public class Config
    {
        #region Private Param
        private static Config obj = null;

        /// <summary>
        /// Версия XML файла
        /// </summary>
        private static int _Version = 1;

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        private static bool _Trace = false;

        /// <summary>
        /// Объект XML файла
        /// </summary>
        private static XmlDocument Document = new XmlDocument();

        /// <summary>
        /// Коренвой элемент нашего документа
        /// </summary>
        private static XmlElement xmlRoot;

        /// <summary>
        /// Количество поппыток повтороного соединения
        /// </summary>
        private static int _CountRefresh = 3;

        /// <summary>
        /// Тайм аут в секундах между попытками повторения подключений
        /// </summary>
        private static int _SecondTimeRefresh = 4;
        #endregion

        #region Public Param
        /// <summary>
        /// Файл в который будем сохранять лог
        /// </summary>
        public static string FileXml { get; private set; }

        /// <summary>
        /// Версия XML файла
        /// </summary>
        public static int Version { get { return _Version; } private set { } }

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        public static bool Trace
        {
            get
            {
                return _Trace;
            }
            set
            {
                xmlRoot.SetAttribute("Trace", value.ToString());
                Save();
                _Trace = value;
            }
        }

        /// <summary>
        /// Количество попыток повтороного соединения
        /// </summary>
        public static int CountRefresh
        {
            get { return _CountRefresh; }
            private set { }
        }

        /// <summary>
        /// Тайм аут в секундах между попытками повторения подключений
        /// </summary>
        public static int SecondTimeRefresh
        {
            get { return _SecondTimeRefresh; }
            private set { }
        }
        #endregion

        #region Public metod
        /// <summary>
        /// Коонструктор
        /// </summary>
        /// <param name="FileConfig">Имя файла лога программы</param>
        public Config(string FileConfig)
        {
            try
            {
                // Если это первая загрузка класса то инициируем его
                if (obj == null)
                {
                    // Подгружаем данные из реестра
                    //this.ConfigReg();


                    if (FileConfig == null) FileXml = "Config.xml";
                    else FileXml = FileConfig;

                    obj = this;

                    // Логируем запуск программы
                    Log.EventSave("Загрузка чтения параметров.", GetType().Name, EventEn.Message);

                    // Читаем файл или создаём
                    if (File.Exists(Environment.CurrentDirectory + @"\" + FileXml)) { Load(); }
                    else { Create(); }

                    // Создаём костомизированный объект
                    GetDate();

                    // Подписываемся на события
                    Com.RepositoryFarm.onEventSetup += RepositoryFarm_onEventSetup;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при загрузке конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Коонструктор
        /// </summary>
        public Config()
            : this(null)
        { }

        #endregion

        #region Private metod

        /// <summary>
        /// Читаем файл конфигурации
        /// </summary>
        private static void Load()
        {
            try
            {
                lock (obj)
                {
                    Document.Load(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при чтении конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Load()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        private static void Save()
        {
            try
            {
                lock (obj)
                {
                    Document.Save(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при сохранении конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Save()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Создаём файл конфигурации
        /// </summary>
        private static void Create()
        {
            try
            {
                //Document = new XmlDocument();

                // Создаём строку инициализации
                XmlElement wbRoot = Document.DocumentElement;
                XmlDeclaration wbxmldecl = Document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                Document.InsertBefore(wbxmldecl, wbRoot);

                // Создаём начальное тело в кторое будем потом всё вставлять
                XmlElement xmlMain = Document.CreateElement("AlfaOlapMon");
                xmlMain.SetAttribute("Version", Version.ToString());
                xmlMain.SetAttribute("Trace", _Trace.ToString());
                xmlMain.SetAttribute("CountRefresh", _CountRefresh.ToString());
                xmlMain.SetAttribute("SecondTimeRefresh", _SecondTimeRefresh.ToString());
                Document.AppendChild(xmlMain);

                // Сохранение в файл
                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при создании конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Create()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Чтение данных
        /// </summary>
        private static void GetDate()
        {
            ApplicationException appM = new ApplicationException("Неправильный настроечный файл, скорее всего не от этой программы.");
            ApplicationException appV = new ApplicationException("Неправильная версия настроечного файла, требуется " + Version.ToString() + " версия.");
            try
            {
                xmlRoot = Document.DocumentElement;

                // Проверяем тип файла настройки по имени коренвого нода и версию
                if (xmlRoot.Name != "AlfaOlapMon") throw appM;
                if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) { throw appV; }
                if (Version > int.Parse(xmlRoot.GetAttribute("Version"))) UpdateVersionXml(xmlRoot, int.Parse(xmlRoot.GetAttribute("Version")));

                // Получаем значения из заголовка
                string RepositCustomClassTyp = null;
                string RepositConnectionString = null;
                for (int i = 0; i < xmlRoot.Attributes.Count; i++)
                {
                    if (xmlRoot.Attributes[i].Name == "RepositCustomClassTyp") RepositCustomClassTyp = xmlRoot.Attributes[i].Value.ToString();
                    try { if (xmlRoot.Attributes[i].Name == "RepositConnectionString") RepositConnectionString = xmlRoot.Attributes[i].Value.ToString(); } //Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }
                    //
                    if (xmlRoot.Attributes[i].Name == "CountRefresh")
                        try { _CountRefresh = int.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    //
                    if (xmlRoot.Attributes[i].Name == "SecondTimeRefresh")
                        try { _SecondTimeRefresh = int.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }

                    /*
                    if (xmlRoot.Attributes[i].Name == "TrashDir") _TrashDir = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "Trace")
                        try { _Trace = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "Mode")
                    {
                        foreach (string item in xmlRoot.Attributes[i].Value.ToString().Split('|'))
                        {
                            _Mode.Add(EventConverter.Convert(item, ModeEn.Normal));
                        }
                    }
                    if (xmlRoot.Attributes[i].Name == "VisibleLocal")
                        try { _VisibleLocal = int.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "LoockVisibleLocal")
                        try { _LoockVisibleLocal = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "EnableServiceCrypto")
                        try { _EnableServiceCrypto = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "EnableServiceFileQueue")
                        try { _EnableServiceFileQueue = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "EnableServiceProvider")
                        try { _EnableServiceProvider = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "EnableServiceWeb")
                        try { _EnableServiceWeb = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "TSMItemDocsCRPT")
                        try { _TSMItemDocsCRPT = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "WebSite") WebSite = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "WebSiteForIsmp") WebSiteForIsmp = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "OmsId") _OmsId = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "ClientToken") _ClientToken = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "ContractNumber") _ContractNumber = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "ContractDate")
                        try { _ContractDate = DateTime.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "CrptCert") _CrptCert = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "PrvFullName") PrvFullName = xmlRoot.Attributes[i].Value.ToString();
                    try { if (xmlRoot.Attributes[i].Name == "ConnectionString") ConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "RepositFullName") RepositFullName = xmlRoot.Attributes[i].Value.ToString();
                    try { if (xmlRoot.Attributes[i].Name == "RepositConnectionString") RepositConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }
                    */
                }

                // Подгружаем репозиторий
                try
                {
                    if (!string.IsNullOrWhiteSpace(RepositCustomClassTyp)) RepositoryFarm.Setup(RepositoryFarm.CreateRepository(RepositCustomClassTyp, RepositConnectionString), false);
                }
                catch (Exception) { }

                // Получаем список объектов
                foreach (XmlElement item in xmlRoot.ChildNodes)
                {
                    switch (item.Name)
                    {
                        /*
                        case "Users":
                            xmlUsers = item;
                            foreach (XmlElement xuser in item.ChildNodes)
                            {
                                string Logon = xuser.Name;
                                string Password = null;
                                string Description = null;
                                Lib.RoleEn Role = RoleEn.None;
                                foreach (XmlAttribute auser in xuser.Attributes)
                                {
                                    if (auser.Name == "Password") Password = Com.Lic.DeCode(xuser.GetAttribute(auser.Name));
                                    if (auser.Name == "Description") Description = xuser.GetAttribute(auser.Name);
                                    if (auser.Name == "Role") Role = Lib.EventConverter.Convert(xuser.GetAttribute(auser.Name), Role);
                                }

                                // Если пароль не указан, то пользователя всё равно нужно добавить, просто при запуске он должен будет придумать пароль
                                if (!string.IsNullOrWhiteSpace(Logon) && Role != RoleEn.None)
                                {
                                    try
                                    {
                                        UserFarm.List.Add(new Lib.User(Logon, Password, Description, Role), true, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.EventSave(string.Format("Не смогли добавить пользователя с именем {0} при чтении конфигурационного файла: {1}", Logon, ex.Message), obj.GetType().Name + ".GetDate()", EventEn.Error);
                                    }

                                }
                            }
                            break;
                        */
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при парсинге файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".GetDate()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Oбновляем до текущей версии
        /// </summary>
        /// <param name="root">Корневой элемент</param>
        /// <param name="oldVersion">Текущая версия элемента</param>
        private static void UpdateVersionXml(XmlElement root, int oldVersion)
        {
            /*
            try
            {
                if (oldVersion <= 2)
                {
                    string _SpecificProcessBonus = null;
                    for (int i = 0; i < root.Attributes.Count; i++)
                    {
                        if (root.Attributes[i].Name == "SpecificProcessBonus") _SpecificProcessBonus = root.Attributes[i].Value.ToString();
                    }

                    root.SetAttribute("SpecificProcessBonus", string.Empty);*/
            /*
                                // Устанавливаем строку подключения в объекте провайдера
                                if (OraTNS != null && OraTNS.Trim() != string.Empty && OraUser != null && OraUser.Trim() != string.Empty && OraPassword != null && OraPassword.Trim() != string.Empty)
                                {
                                    Com_Provider_Ora conOra = new Com_Provider_Ora(this._MyCom);
                                    try
                                    {
                                        if (!conOra.SaveConnectStr(OraTNS, OraUser, OraPassword) || conOra.ConnectString() == null || conOra.ConnectString().Trim() == string.Empty) new ApplicationException("Не можем обновить конфиг файл так как подключение к ораклу невалидно.");
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ApplicationException("Не можем обновить конфиг файл так как подключение к ораклу невалидно. (" + ex.Message + ")");
                                    }

                                    root.SetAttribute("ProviderTyp", Enum.GetName(typeof(Lib.Provider_En), Lib.Provider_En.Oracle));
                                    root.SetAttribute("ConnectionString", conOra.ConnectString());
                                    root.RemoveAttribute("OraTNS");
                                    root.RemoveAttribute("OraUser");
                                    root.RemoveAttribute("OraPassword");
                                }
                                */
            /*        }

                    root.SetAttribute("Version", _Version.ToString());
                    Save();
                }
                catch (Exception ex)
                {
                    Log.EventSave(ex.Source + @": " + ex.Message, "UpdateVersionXml", EventEn.Error);
                    throw ex;
                }
                */
        }

        // Обработка события изменения репозитория
        private void RepositoryFarm_onEventSetup(object sender, EventRepositoryFarm e)
        {
            try
            {
                XmlElement root = Document.DocumentElement;

                root.SetAttribute("RepositCustomClassTyp", e.Rep.CustomClassTyp);
                try { root.SetAttribute("RepositConnectionString", e.Rep.ConnectionString /* Com.Lic.InCode(e.Urep.ConnectionString)*/); }
                catch (Exception) { }

                // Получаем список объектов
                //foreach (XmlElement item in root.ChildNodes)
                //{
                //}

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при изменении файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".RepositoryFarm_onEventSetup()", EventEn.Error);
                throw ae;
            }
        }

        #endregion

        #region Вложенные классы

        /*
        /// <summary>
        /// Вложенный класс для доступа к элементам внутренним нашего конфигурационного класса
        /// </summary>
        public class AceessForDoc
        {
            /// <summary>
            /// Плучить новый экземпляр элемента
            /// </summary>
            /// <param name="ElementName">Задаём имя элемента который мы хотим создать</param>
            /// <returns></returns>
            public XmlElement getNewXmlElement(string ElementName)
            {
                return Com.Config.Document.CreateElement(ElementName);
            }

            /// <summary>
            /// Сохранение изменений в файл
            /// </summary>
            public void SaveDoc()
            {
                Com.Config.Save();
            }
        }
        */
        #endregion
    }
}
