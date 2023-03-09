using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoNodeWorker.Lib
{
    /// <summary>
    /// Событие прозошедшее в логе
    /// </summary>
    public class EventLog : EventArgs
    {
        /// <summary>
        /// Сообщение события
        /// </summary>
        public String Message { get; private set; }

        /// <summary>
        /// Источник где произошло событие
        /// </summary>
        public String Source { get; private set; }

        /// <summary>
        /// Тип события
        /// </summary>
        public EventEn Evn { get; private set; }

        /// <summary>
        /// Записать событие в лог или нет. True значит записать в лог
        /// </summary>
        public bool isLog;

        /// <summary>
        /// Отобразить пользователю или нет. True значит отобразить окошко с сообщением пользователю
        /// </summary>
        public bool Show;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник где произошло событие</param>
        /// <param name="evn">Тип события</param>
        /// <param name="isLog">Записать событие в лог или нет. True значит записать в лог</param>
        /// <param name="Show">Отобразить пользователю или нет. True значит отобразить окошко с сообщением пользователю</param>
        public EventLog(String Message, String Source, EventEn evn, bool isLog, bool Show)
        {
            try
            {
                this.Message = Message;
                this.Source = Source;
                this.Evn = evn;
                this.isLog = isLog;
                this.Show = Show;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }
    }
}
