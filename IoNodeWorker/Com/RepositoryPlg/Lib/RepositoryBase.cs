using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.Com.RepositoryPlg.Lib
{
    /// <summary>
    /// Базовый класс нашего репозитория
    /// </summary>
    public abstract class RepositoryBase
    {
        #region Param (private)

        /// <summary>
        /// Интерфейс для базового класса чтобы он мог дёргать скрытыем методы
        /// </summary>
        private RepositoryI RepI = null;

        /// <summary>
        /// Строка подключения
        /// </summary>
        private string _ConnectionString;

        #endregion

        #region Param (public get; protected set;)

        /// <summary>
        /// Информация о версии базы данных
        /// </summary>
        public string VersionDB { get; protected set; }

        /// <summary>
        /// Наличие подключения к источнику данных
        /// </summary>
        public bool HashConnect { get; protected set; } = false;

        /// <summary>
        /// Кастомный объект для ссылочной целостности
        /// </summary>
        public string CustomClassTyp { get; protected set; } = null;

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this._ConnectionString;
            }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this._ConnectionString = null;
                    this.HashConnect = false;
                }
                else
                {
                    this._ConnectionString = value.ToString();
                    this.HashConnect = this.TestConnect();
                }
            }
        }

        /// <summary>
        /// Доп объекты для нашего класса
        /// </summary>
        public object Tag { get; protected set; }
        #endregion

        #region Param (protected)

        #endregion

        #region Method (public)

        /// <summary>
        /// Конструктор передаём в него кстомный класс для ссылочной целостности
        /// </summary>
        /// <param name="CustomClass">Тип репозитория</param>
        /// <param name="ConnectionString">Строка подключения к репозиторию</param>
        public RepositoryBase(string CustomClassTyp, string ConnectionString)
        {
            try
            {
                this.CustomClassTyp = CustomClassTyp;
                this.ConnectionString = ConnectionString;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Проверка строки подключения
        /// </summary>
        /// <returns>True - Если база доступна | False - Если база не доступна</returns>
        public bool TestConnect()
        {
            try
            {
                return TestConnect(this.ConnectionString, false);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе проверки подключения:""{0}""", ex.Message), string.Format("{0}.TestConnect", this.GetType().FullName), EventEn.Error, true, false);
                throw ex;
            }
        }

        #endregion

        #region Method (public virtual)

        /// <summary>
        /// Проверка строки подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения для проверки</param>
        /// <param name="VisibleError">True если при проверке подключения надо выводить сообщения пользователю</param>
        /// <returns>True - Если база доступна | False - Если база не доступна</returns>
        public virtual bool TestConnect(string ConnectionString, bool VisibleError)
        {
            try
            {
                throw new ApplicationException("Необходимо перезаписать метод TestConnect в наследуемом классе чтобы обработать данный метод.");
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе проверки подключения:""{0}""", ex.Message), string.Format("{0}.TestConnect", this.GetType().FullName), EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Вывод строки подключения в лог или интерфейс пользователя с затиранием пароля
        /// </summary>
        /// <returns>Строка подключения которую мы можем безопасно передавать пользователю так как пароль должен быть затёрт</returns>
        public virtual string PrintConnectionString()
        {
            try
            {
                throw new ApplicationException("Необходимо перезаписать метод PrintConnectionString в наследуемом классе чтобы обработать данный метод.");
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе печати строки подключения:""{0}""", ex.Message), string.Format("{0}.PrintConnectionString", this.GetType().FullName), EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Вывод строки подключения в лог или интерфейс пользователя с затиранием пароля
        /// </summary>
        /// <param name="Rep">Репозиторий который мы хотим править</param>
        /// <returns>True если пользователь решил сохранить репозиторй | False если пользователь не хочет сохранять</returns>
        public virtual bool SetupConnectDB(ref Repository Rep)
        {
            try
            {
                throw new ApplicationException("Необходимо перезаписать метод SetupConnectDB в наследуемом классе чтобы обработать данный метод.");
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе печати строки подключения:""{0}""", ex.Message), string.Format("{0}.SetupConnectDB", this.GetType().FullName), EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Запись лога в базу данных
        /// </summary>
        /// <param name="Message">Сообщение которое пишем в базу данных</param>
        /// <param name="Source">Источник где оно возникло</param>
        /// <param name="evn">Событие системное которое фиксируем</param>
        public virtual void EventSaveDb(string Message, string Source, EventEn evn)
        {
            try
            {
                throw new ApplicationException("Необходимо перезаписать метод EventSave(string Message, string Source, EventEn evn) в наследуемом классе чтобы была возмоность писать в лог.");
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе печати строки подключения:""{0}""", ex.Message), string.Format("{0}.SetupConnectDB", this.GetType().FullName), EventEn.Error, true, true);
                throw ex;
            }
        }

        /*
        /// <summary>
        /// Получение списка инстансов из разы репозитория
        /// </summary>
        /// <returns>Возвращаем списокинстансов</returns>
        public virtual List<TInstance> GetTInstanceList()
        {
            try
            {
                throw new ApplicationException("Необходимо перезаписать метод List<TInstance> GetTInstanceList() в наследуемом классе чтобы метод работал корректно.");
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе GetTInstanceList:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }
        */
        #endregion

        #region Method (protected)

        /// <summary>
        /// Логирование для наследуемых классов
        /// </summary>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="isLog">Писать в лог или нет</param>
        /// <param name="Show">Отобразить сообщение пользователю или нет</param>
        protected void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
        {
            try
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "EventSave", Message), string.Format("Repository.{0}.{1}", CustomClassTyp, Source), EventEn.Error, true, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        protected string test()
        {
            return RepI.getStr();
        }
        */

        #endregion

        #region CrossClass

        /// <summary>
        /// Внутренний класс для линковки интерфейсов састомного класса скрытых для пользователя
        /// </summary>
        public class CrossLink
        {
            /// <summary>
            /// Линкуеминтерфейс RepositoryI скрытый для пользователя
            /// </summary>
            /// <param name="CustRep">Кастомный обьект для линковки</param>
            public CrossLink(RepositoryBase CustRep)
            {
                CustRep.RepI = (RepositoryI)CustRep;
            }
        }

        #endregion
    }
}
