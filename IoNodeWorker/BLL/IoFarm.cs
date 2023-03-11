using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.Lib;
using System.Reflection;
using IoNodeWorker.Lib;


namespace IoNodeWorker.BLL
{
    /// <summary>
    /// Ферма с нашим плагином
    /// </summary>
    public class IoFarm
    {
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
    }
}
