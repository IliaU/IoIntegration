using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using IoNodeWorker.Lib;

namespace IoNodeWorker.Com
{
    /// <summary>
    /// Ферма с нашим репозиторием
    /// </summary>
    public static class RepositoryFarm
    {
        /// <summary>
        /// Список доступных плагинов
        /// </summary>
        private static List<string> _RepositoryName = null;

        /// <summary>
        /// Список доступных плагинов
        /// </summary>
        public static List<string> ListRepositoryName
        {
            get
            {
                if (_RepositoryName == null) _RepositoryName = GetListRepositoryName();
                return _RepositoryName;
            }
            private set { }
        }

        /// <summary>
        /// Текущий репозиторий
        /// </summary>
        public static Repository CurentRep { get; private set; }

        /// <summary>
        /// Событие изменения текущего универсального репозитория
        /// </summary>
        public static event EventHandler<EventRepositoryFarm> onEventSetup;

        /// <summary>
        /// Доступно ли подключение или нет
        /// </summary>
        /// <returns>true Если смогли подключиться к базе данных</returns>
        public static bool HashConnect()
        {
            try
            {
                bool rez = false;

                // проверяем наличие подключения в системе
                if (CurentRep != null && CurentRep.VersionDB != null && CurentRep.VersionDB.Trim() != string.Empty)
                {
                    rez = CurentRep.HashConnect;
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе проверки подключения:""{0}""", ex.Message), "HashConnect", EventEn.Error, true, true);
                return false;
            }
        }

        /// <summary>
        /// Создание репозитория определённого типа
        /// </summary>
        /// <param name="CustomClass">Тип репозитория</param>
        /// <param name="ConnectionString">Строка подключения к репозиторию</param>
        /// <returns>Универсальный репозиторий</returns>
        public static Repository CreateRepository(string CustomClassTyp, string ConnectionString)
        {
            try
            {
                Repository rez = null;

                // Проверка параметров
                if (CustomClassTyp == null || CustomClassTyp.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать репозиторий указанного типа: ({0})", CustomClassTyp == null ? "" : CustomClassTyp.Trim()));

                // Проверяем наличие класса в доступных по списку если найден то создаём его
                if (ListRepositoryName.Where(a => a == CustomClassTyp).ToArray().Length > 0)
                {
                    // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
                    //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
                    Type myType = Type.GetType("IoNodeWorker.Com.RepositoryPlg." + CustomClassTyp.Trim(), false, true);

                    // Создаём экземпляр объекта  
                    object[] targ = { (string.IsNullOrWhiteSpace(ConnectionString) ? (object)null : ConnectionString) };
                    object obj = Activator.CreateInstance(myType, targ);
                    rez = (Repository)obj;

                    // Линкуем в базовый класс специальный скрытый интерфейс для того чтобы базовый класс мог что-то специфическое вызывать в дочернем объекте
                    RepositoryPlg.Lib.RepositoryBase.CrossLink CrLink = new RepositoryPlg.Lib.RepositoryBase.CrossLink(rez);
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса репозитория:""{0}""", ex.Message), "RepositoryFarm.CreateRepository", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Создание репозитория определённого типа
        /// </summary>
        /// <param name="CustomClass">Тип репозитория</param>
        /// <returns>Универсальный репозиторий</returns>
        public static Repository CreateRepository(string CustomClassTyp)
        {
            return CreateRepository(CustomClassTyp, null);
        }

        /// <summary>
        /// Получаем список доступных репозиториев
        /// </summary>
        /// <returns>Список имён доступных репозиториев</returns>
        public static List<string> GetListRepositoryName()
        {
            List<string> RepositoryName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "IoNodeWorker.Com.RepositoryPlg").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "IoNodeWorker.Com.RepositoryPlg.Lib.RepositoryI") flagI = true;
                }
                if (!flagI) continue;

                // Проверяем что наш клас наследует PlugInBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "IoNodeWorker.Com.RepositoryPlg.Lib.RepositoryBase") flagB = true;
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

                    // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                    if (parameters.Length == 1)
                    {

                        if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;

                    }
                }
                if (!flag) continue;

                RepositoryName.Add(item.Name);
            }

            return RepositoryName;
        }

        /// <summary>
        /// Установка нового репозитория
        /// </summary>
        /// <param name="Rep">Универсальный репозиторий который собираемся установить в качестве текущего репозитория</param>
        /// <param name="WriteLog">Запись в лог</param>
        public static void Setup(Repository Rep, bool WriteLog)
        {
            try
            {
                CurentRep = Rep;

                // Собственно обработка события
                EventRepositoryFarm myArg = new EventRepositoryFarm(Rep);
                if (onEventSetup != null)
                {
                    onEventSetup.Invoke(Rep, myArg);
                }

                // Логируем изменение подключения
                if (WriteLog) Log.EventSave(string.Format("Пользователь настроил новый репозиторий: {0} ({1})", Rep.PrintConnectionString(), Rep.CustomClassTyp), "RepositoryFarm", EventEn.Message);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "Setup (UProvider Uprov, bool WriteLog)", ex.Message), "RepositoryFarm", EventEn.Error, true, true);
            }
        }
        //
        /// <summary>
        /// Установка нового репозитория
        /// </summary>
        /// <param name="Rep">Универсальный репозиторий который собираемся установить в качестве текущего репозитория</param>
        public static void Setup(Repository Rep)
        {
            try
            {
                Setup(Rep, true);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "Setup(Repository Rep)", ex.Message), "RepositoryFarm", EventEn.Error, true, true);
            }
        }

    }
}
