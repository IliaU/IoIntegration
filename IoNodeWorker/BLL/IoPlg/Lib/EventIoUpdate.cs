using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Класс для обработки события изменения инстанса
    /// </summary>
    public class EventIoUpdate : EventArgs
    {
        /// <summary>
        /// Добавляемый объект
        /// </summary>
        public Io nIo { get; private set; }

        /// <summary>
        /// Удаляемый объект объект
        /// </summary>
        public Io dIo { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nIo">Добавляемый объект</param>
        /// <param name="dIo">Удаляемый объект</param>
        public EventIoUpdate(Io nIo, Io dIo)
        {
            try
            {
                this.nIo = nIo;
                this.dIo = dIo;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }
    }
}
