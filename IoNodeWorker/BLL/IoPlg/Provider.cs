using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.Lib;
using IoNodeWorker.BLL.IoPlg.ProviderPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg
{
    /// <summary>
    /// Класс которые реализует наш провайдер на нодах
    /// </summary>
    public class Provider : Io, IoI
    {
        #region Параметры Private

        /// <summary>
        /// Интерфейс для базового класса чтобы он мог дёргать скрытыем методы
        /// </summary>
        private ProviderI PrvI = null;

        /// <summary>
        /// Кастомный объект для ссылочной целостности
        /// </summary>
        private string _CustomClassTyp;

        /// <summary>
        /// Строка подключения
        /// </summary>
        private string _ConnectionString;

        #endregion

        #region Параметры Public

        /// <summary>
        /// Наличие подключения к источнику данных
        /// </summary>
        public bool HashConnect { get; protected set; } = false;

        #endregion

        #region Param (public get; protected set;)

        /// <summary>
        /// Кастомный объект для ссылочной целостности
        /// </summary>
        public new string CustomClassTyp
        {
            get
            {
                return string.Format("{0}.{1}", base.CustomClassTyp, this._CustomClassTyp);
            }
            protected set
            {
                this._CustomClassTyp = value;
            }
        }

        /// <summary>
        /// Информация о версии базы данных
        /// </summary>
        public string VersionDB { get; protected set; }

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

        #endregion

        #region Методы Public

        /// <summary>
        /// Конструктор
        /// </summary>
        public Provider() : base("Provider")
        {
            try
            {
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
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
        /// <param name="Prv">Провайдер который мы хотим править</param>
        /// <returns>True если пользователь решил сохранить репозиторй | False если пользователь не хочет сохранять</returns>
        public virtual bool SetupConnectDB(ref Provider Prv)
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
        protected new void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
        {
            try
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "EventSave", Message), string.Format("{0}.{1}", CustomClassTyp, Source), EventEn.Error, true, true);
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
        public new class CrossLink
        {
            /// <summary>
            /// Линкуеминтерфейс ProviderI скрытый для пользователя
            /// </summary>
            /// <param name="CustPrv">Кастомный обьект для линковки</param>
            public CrossLink(Provider CustPrv)
            {
                CustPrv.PrvI = (ProviderI)CustPrv;
            }
        }

        #endregion
    }
}
