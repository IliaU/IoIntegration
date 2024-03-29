﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Базовый класс наших плагинов
    /// </summary>
    public abstract partial class IoBase
    {
        #region Param (private)

        /// <summary>
        /// Интерфейс для базового класса чтобы он мог дёргать скрытыем методы
        /// </summary>
        private IoI InterfIo = null;

        #endregion

        #region Param (public get; protected set;)

        /// <summary>
        /// Индекс в списке IoList
        /// </summary>
        public int index { get; protected set; } = -1;

        /// <summary>
        /// Кастомный объект для понимания какой объект перед нами
        /// </summary>
        public string CustomClassTyp { get; protected set; } = null;

        #endregion

        #region Method (public)

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustomClassTyp">Тип репозитория</param>
        public IoBase(string CustomClassTyp)
        {
            try
            {
                this.CustomClassTyp = CustomClassTyp;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
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
        protected void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
        {
            try
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "EventSave", Message), string.Format("Io.{0}.{1}", CustomClassTyp, Source), EventEn.Error, true, true);
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
            /// Линкуеминтерфейс IoI скрытый для пользователя
            /// </summary>
            /// <param name="CustIo">Кастомный обьект для линковки</param>
            public CrossLink(IoBase CustIo)
            {
                CustIo.InterfIo = (IoI)CustIo;
            }
        }

        #endregion
    }
}
