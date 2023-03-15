using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.Lib;
using IoNodeWorker.BLL.IoPlg.EventerPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg
{
    /// <summary>
    /// Класс которые реализует объект типа событие например шедуллер (namespace в базе данных Eventer)
    /// </summary>
    public class Eventer : Io, IoI
    {
        #region Параметры Private

        /// <summary>
        /// Интерфейс для базового класса чтобы он мог дёргать скрытыем методы
        /// </summary>
        private EventerI EventerInterface = null;

        /// <summary>
        /// Кастомный объект для ссылочной целостности
        /// </summary>
        private string _CustomClassTyp;

        #endregion

        #region Параметры Public


        #endregion

        #region Param (public get; protected set;)

        /// <summary>
        /// Кастомный объект для понимания какой объект перед нами
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

        #endregion

        #region Методы Public

        /// <summary>
        /// Конструктор
        /// </summary>
        public Eventer() : base("Eventer")
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

        #endregion

        #region Method (public virtual)


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

        #endregion

        #region CrossClass

        /// <summary>
        /// Внутренний класс для линковки интерфейсов састомного класса скрытых для пользователя
        /// </summary>
        public new class CrossLink
        {
            /// <summary>
            /// Линкуем интерфейс OsI скрытый для пользователя
            /// </summary>
            /// <param name="CustEventer">Кастомный обьект для линковки</param>
            public CrossLink(Eventer CustEventer)
            {
                CustEventer.EventerInterface = (EventerI)CustEventer;
            }
        }

        #endregion
    }
}
