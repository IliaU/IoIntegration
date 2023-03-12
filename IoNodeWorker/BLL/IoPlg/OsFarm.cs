using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg
{
    /// <summary>
    /// Ферма с нашим плагином которые реализует объект типа операционная система (namespace в базе данных Os)
    /// </summary>
    public class OsFarm
    {
        /// <summary>
        /// Список доступных плагинов типа операционная система (namespace в базе данных Os)
        /// </summary>
        private static List<string> _OsName = null;

        /// <summary>
        /// Список доступных плагинов типа операционная система (namespace в базе данных Os)
        /// </summary>
        public static List<string> ListOsName
        {
            get
            {
                if (_OsName == null) _OsName = GetListOsName();
                return _OsName;
            }
            private set { }
        }

        /// <summary>
        /// Создание провайдера типа операционная система (namespace в базе данных Os)
        /// </summary>
        /// <param name="CustomClass">Тип плагина</param>
        /// <returns>Универсальный плагин</returns>
        public static Os CreateOs(string CustomClassTyp)
        {
            try
            {
                Os rez = null;

                // Проверка параметров
                if (CustomClassTyp == null || CustomClassTyp.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать репозиторий указанного типа: ({0})", CustomClassTyp == null ? "" : CustomClassTyp.Trim()));

                // Проверяем наличие класса в доступных по списку если найден то создаём его
                if (ListOsName.Where(a => a == CustomClassTyp).ToArray().Length > 0)
                {
                    // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
                    //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
                    Type myType = Type.GetType("IoNodeWorker.BLL.IoPlg.OsPlg." + CustomClassTyp.Trim(), false, true);

                    // Создаём экземпляр объекта  
                    object[] targ = null;// { (string.IsNullOrWhiteSpace(ConnectionString) ? (object)null : ConnectionString) };
                    object obj = Activator.CreateInstance(myType, targ);
                    rez = (Os)obj;

                    // Линкуем в базовый класс специальный скрытый интерфейс для того чтобы базовый класс мог что-то специфическое вызывать в дочернем объекте
                    Os.CrossLink CrLink = new Os.CrossLink(rez);
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка при создании класса плагина:""{0}""", ex.Message), "OsFarm.CreateOs", EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Получаем список доступных плагинов
        /// </summary>
        /// <returns>Список имён доступных плагинов</returns>
        public static List<string> GetListOsName()
        {
            List<string> IoName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "IoNodeWorker.BLL.IoPlg.OsPlg").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подгрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "IoNodeWorker.BLL.IoPlg.OsPlg.Lib.OsI")
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
                    if (mi.DeclaringType.FullName == "IoNodeWorker.BLL.IoPlg.Os")
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
                        //if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString")
                        //{
                            flag = true;
                            continue;
                        //}
                    }
                }
                if (!flag) continue;

                IoName.Add(item.Name);
            }

            return IoName;
        }
    }
}
