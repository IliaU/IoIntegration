using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.Lib;
using IoNodeWorker.Lib;


namespace IoNodeWorker.BLL.IoPlg
{
    /// <summary>
    /// Кастомный список пулов по объектам типа событие например шедуллер (namespace в базе данных Eventer)
    /// </summary>
    public class EventerPul : IoList, IoListI
    {
        #region Параметры Private

        /// <summary>
        /// Кастомный объект для понимания какой объект перед нами
        /// </summary>
        private string _CustomClassTyp;

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
        /// <param name="CustomClass">Тип плагина</param>
        public EventerPul() : base("EventerPul")
        {
            try
            {
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Событие установки статуса пула для отслеживания зависаний и качества соединения с разными источниками относительно нод
        /// </summary>
        /// <returns>Возвращаем статус проверки</returns>
        public EventEn SetStatusPul()
        {
            EventEn rez = EventEn.Empty;
            try
            {


                rez = EventEn.Success;
            }
            catch (Exception ex)
            {
                rez = EventEn.Error;
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), string.Format("{0}.SetStatusPul", this.GetType().FullName), EventEn.Error, true, false);
                throw ex;
            }
            return rez;
        }

        #endregion

        #region Method (public override)

        /// <summary>
        /// Добавление элемента в список
        /// </summary>
        /// <param name="nIo">Элемент который мы добавили в список</param>
        public override void Add(Io nIo)
        {
            try
            {
                // Если всё успешно то добавляем в список
                base.Add(nIo);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), string.Format("{0}.Add", this.GetType().FullName), EventEn.Error, true, false);
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
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), string.Format("{0}.EventSave", this.GetType().FullName), EventEn.Error, true, false);
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

    }
}
