using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Класс для обработки события добавления инстанса
    /// </summary>
    public class EventIoAdd : EventArgs
    {
        /// <summary>
        /// Добавляемый объект
        /// </summary>
        public Io nIo { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nIo">Добавляемый объект</param>
        public EventIoAdd(Io nIo)
        {
            try
            {
                this.nIo = nIo;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }
    }
}
