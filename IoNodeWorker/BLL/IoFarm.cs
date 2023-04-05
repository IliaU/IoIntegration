using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Threading;
using IoNodeWorker.Com.RepositoryPlg.Lib;
using IoNodeWorker.BLL.IoPlg.Lib;
using IoNodeWorker.Lib;


namespace IoNodeWorker.BLL
{
    /// <summary>
    /// Ферма с нашим плагином
    /// </summary>
    public class IoFarm
    {
        /// <summary>
        /// Поток для мониторинга состояния ноды и её здооровбя целиком
        /// </summary>
        private static Thread ThrCreateCurentPulList;

        /// <summary>
        /// Состояние мониторинга по всей ноде
        /// </summary>
        private static bool IsRunThrCreateCurentPulList = false;

        /// <summary>
        /// Список крос классов для управления базовым классом на уровне нашего пула
        /// </summary>
        private static List<IoBase.IoListBase.CrossLink> CurentPulCrossLink = null;

        /// <summary>
        /// Список доступных пулов со своими классами
        /// </summary>
        public static List<IoList> CurentPulList = null;

        /// <summary>
        /// Список доступных плагинов
        /// </summary>
        private static List<string> _IoName = null;

        /// <summary>
        /// Список доступных плагинов
        /// </summary>
        public static List<string> ListIoName
        {
            get
            {
                if (_IoName == null) _IoName = GetListIoName();
                return _IoName;
            }
            private set { }
        }

        /// <summary>
        /// Список доступных пулов плагинов
        /// </summary>
        private static List<string> _IoPulName = null;

        /// <summary>
        /// Список доступных пулов плагинов
        /// </summary>
        public static List<string> ListIoPulName
        {
            get
            {
                if (_IoPulName == null) _IoPulName = GetListIoPulName();
                return _IoPulName;
            }
            private set { }
        }

        /// <summary>
        /// Создание объекта определённого типа
        /// </summary>
        /// <param name="CustomClass">Тип плагина</param>
        /// <returns>Универсальный объект</returns>
        public static Io CreateIo(string CustomClassTyp)
        {
            try
            {
                Io rez = null;

                // Проверка параметров
                if (CustomClassTyp == null || CustomClassTyp.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать репозиторий указанного типа: ({0})", CustomClassTyp == null ? "" : CustomClassTyp.Trim()));

                // Проверяем наличие класса в доступных по списку если найден то создаём его
                if (ListIoName.Where(a => a == CustomClassTyp).ToArray().Length > 0)
                {
                    // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
                    //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
                    Type myType = Type.GetType("IoNodeWorker.BLL.IoPlg." + CustomClassTyp.Trim(), false, true);

                    // Создаём экземпляр объекта  
                    object[] targ=null;// = { (string.IsNullOrWhiteSpace(ConnectionString) ? (object)null : ConnectionString) };
                    object obj = Activator.CreateInstance(myType, targ);
                    rez = (Io)obj;

                    // Линкуем в базовый класс специальный скрытый интерфейс для того чтобы базовый класс мог что-то специфическое вызывать в дочернем объекте
                    IoBase.CrossLink CrLink = new IoBase.CrossLink(rez);
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.CreateIo", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Создание объекта пула определённого типа
        /// </summary>
        /// <param name="CustomClass">Тип пула плагина</param>
        /// <returns>Универсальный объект</returns>
        public static IoList CreatePulIo(string CustomClassTyp)
        {
            try
            {
                IoList rez = null;

                // Проверка параметров
                if (CustomClassTyp == null || CustomClassTyp.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать репозиторий указанного типа: ({0})", CustomClassTyp == null ? "" : CustomClassTyp.Trim()));

                // Проверяем наличие класса в доступных по списку если найден то создаём его
                if (ListIoPulName.Where(a => a == CustomClassTyp).ToArray().Length > 0)
                {
                    // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
                    //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
                    Type myType = Type.GetType("IoNodeWorker.BLL.IoPlg." + CustomClassTyp.Trim(), false, true);

                    // Создаём экземпляр объекта  
                    object[] targ = null;// = { (string.IsNullOrWhiteSpace(ConnectionString) ? (object)null : ConnectionString) };
                    object obj = Activator.CreateInstance(myType, targ);
                    rez = (IoList)obj;

                    // Линкуем в базовый класс специальный скрытый интерфейс для того чтобы базовый класс мог что-то специфическое вызывать в дочернем объекте
                    IoBase.IoListBase.CrossLink CrLink = new IoBase.IoListBase.CrossLink(rez);
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.CreateIo", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Получаем список доступных плагинов
        /// </summary>
        /// <returns>Список имён доступных плагинов</returns>
        public static List<string> GetListIoName()
        {
            List<string> IoName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "IoNodeWorker.BLL.IoPlg").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подгрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "IoNodeWorker.BLL.IoPlg.Lib.IoI")
                    {
                        flagI = true;
                        break;
                    }
                }
                if (!flagI) continue;

                // Проверяем что наш клас наследует IoBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "IoNodeWorker.BLL.IoPlg.Lib.IoBase")
                    {
                        flagB = true;
                        break;
                    }
                }
                if (!flagB) continue;

                // Проверяем конструктор нашего класса  
                bool flag = false;
                string nameConstructor;
                foreach (ConstructorInfo ctor in item.GetConstructors())
                {
                    nameConstructor = item.Name;

                    // получаем параметры конструктора  
                    ParameterInfo[] parameters = ctor.GetParameters();

                    // если в этом конструктаре 0 параметров то проверяем тип и имя параметра  
                    if (parameters.Length == 0)
                    {
                        flag = true;
                        continue;
                        //if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;
                    }
                }
                if (!flag) continue;

                IoName.Add(item.Name);
            }

            return IoName;
        }

        /// <summary>
        /// Получаем список доступных пулов плагинов
        /// </summary>
        /// <returns>Список имён доступных пулов плагинов</returns>
        public static List<string> GetListIoPulName()
        {
            List<string> IoPulName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "IoNodeWorker.BLL.IoPlg").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подгрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "IoNodeWorker.BLL.IoPlg.Lib.IoListI")
                    {
                        flagI = true;
                        break;
                    }
                }
                if (!flagI) continue;

                // Проверяем что наш клас наследует IoBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "IoNodeWorker.BLL.IoPlg.Lib.IoBase+IoListBase")
                    {
                        flagB = true;
                        break;
                    }
                }
                if (!flagB) continue;

                // Проверяем конструктор нашего класса  
                bool flag = false;
                string nameConstructor;
                foreach (ConstructorInfo ctor in item.GetConstructors())
                {
                    nameConstructor = item.Name;

                    // получаем параметры конструктора  
                    ParameterInfo[] parameters = ctor.GetParameters();

                    // если в этом конструктаре 0 параметров то проверяем тип и имя параметра  
                    if (parameters.Length == 0)
                    {
                        flag = true;
                        continue;
                        //if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;
                    }
                }
                if (!flag) continue;

                IoPulName.Add(item.Name);
            }

            return IoPulName;
        }

        /// <summary>
        /// Процесс создания и запуска процессов в наших пулах с плагинами
        /// </summary>
        public static void CreateCurentPulList()
        {
            try
            {
                // Если список пулов ещё не создавали то создаём его
                if (CurentPulList == null)
                {
                    // Асинхронный запуск процесса
                    IsRunThrCreateCurentPulList = true;
                    ThrCreateCurentPulList = new Thread(ACreateCurentPulList);
                    ThrCreateCurentPulList.Name = "AThrCreateCurentPulList";
                    ThrCreateCurentPulList.IsBackground = true;
                    ThrCreateCurentPulList.Start();


                    CurentPulList = new List<IoList>();
                    CurentPulCrossLink = new List<IoBase.IoListBase.CrossLink>();

                    // Пробегаем по всем доступным объектам
                    foreach (string itemPul in ListIoPulName)
                    {
                        // Создаём нужный нам пул для того чтобы его добавить в список существующих в работе пулов
                        IoList nPul = CreatePulIo(itemPul);
                        IoBase.IoListBase.CrossLink nPulCrossLink = new IoBase.IoListBase.CrossLink(nPul);

                        // Добавляем наш пул
                        CurentPulCrossLink.Add(nPulCrossLink);          // Для управления пулом из нашего воркера
                        CurentPulList.Add(nPul);                        // Добавление пула для доступности пользователям
                        
                        // Запускаем процесс на нашем пуле в базовом классе
                        nPulCrossLink.StartCompileListing();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.CreateCurentPuulList", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Асинхронный статус для фиксации состояния всей ноды целиком чтобы видеть работает она сейчас или нет на стороне базы
        /// </summary>
        private static void ACreateCurentPulList()
        {
            try
            {
                // Устанавливаем тайм из конфига
                int CountWhile = Com.Config.SecondPulRefreshStatus;

                while (IsRunThrCreateCurentPulList)
                {
                    if (CountWhile == 0)
                    {
                        // Устанавливаем тайм аут из конфига
                        CountWhile = Com.Config.SecondPulRefreshStatus;
                                                
                        // Если появилось подключение к базе данных и ещё небыло успешной регистрации нашего пула то делаем её в системе для того чтобы сервис знал о том что сервис такой существует 
                        if (Com.RepositoryFarm.CurentRep != null && Com.RepositoryFarm.CurentRep.HashConnect)
                        {
                            // Фиксируем версию нашего приложения и его статус
                            Version Ver = Assembly.GetExecutingAssembly().GetName().Version;
                            ((RepositoryI)Com.RepositoryFarm.CurentRep).NodeSetStatus(Environment.MachineName, DateTime.Now, Ver.ToString(), EventEn.Runned.ToString());
                        }
                    }

                    Thread.Sleep(1000);     // Тайм аут между проверками статуса
                    CountWhile--;
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.CreateCurentPuulList", EventEn.Error, true, true);
                throw ex;
            }
        }


        /// <summary>
        /// Остановка аснхронных процессов перед выключением всех потоков
        /// </summary>
        public static void Stop()
        {
            try
            {
                // Если список пулов ещё создавали
                if (CurentPulList != null)
                {
                    IsRunThrCreateCurentPulList = false;

                    // Пробегаем по всем доступным объектам
                    foreach (IoBase.IoListBase.CrossLink itemPul in CurentPulCrossLink)
                    {
                        itemPul.StopCompileListing();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.Stop", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Остановка аснхронных процессов перед выключением всех потоков
        /// </summary>
        /// <param name="Aborting">True если с прерывением всех процессов жёстное отклучение всех процессов</param>
        public static void Join(bool Aborting)
        {
            try
            {
                // Если список пулов ещё не создавали то создаём его
                if (CurentPulList != null)
                {
                    // Пробегаем по всем доступным объектам
                    Stop();

                    if (Com.RepositoryFarm.CurentRep != null && Com.RepositoryFarm.CurentRep.HashConnect)
                    {
                        // Фиксируем версию нашего приложения и его статус
                        Version Ver = Assembly.GetExecutingAssembly().GetName().Version;
                        ((RepositoryI)Com.RepositoryFarm.CurentRep).NodeSetStatus(Environment.MachineName, DateTime.Now, Ver.ToString(), EventEn.Stoping.ToString());
                    }

                    // Пробегаем по всем доступным объектам
                    foreach (IoBase.IoListBase.CrossLink itemPul in CurentPulCrossLink)
                    {
                        itemPul.Join(Aborting);
                    }

                    if (ThrCreateCurentPulList!=null) ThrCreateCurentPulList.Join();
                    if (Com.RepositoryFarm.CurentRep != null && Com.RepositoryFarm.CurentRep.HashConnect)
                    {
                        // Фиксируем версию нашего приложения и его статус
                        Version Ver = Assembly.GetExecutingAssembly().GetName().Version;
                        ((RepositoryI)Com.RepositoryFarm.CurentRep).NodeSetStatus(Environment.MachineName, DateTime.Now, Ver.ToString(), EventEn.Stop.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "IoFarm.Join", EventEn.Error, true, true);
                throw ex;
            }
        }

    }
}
