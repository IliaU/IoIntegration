using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Класс для обработки события удаления инстанса
    /// </summary>
    public class EventIoDelete : EventArgs
    {
        /// <summary>
        /// Удаляемый объект объект
        /// </summary>
        public Io dIo { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dIo">Удаляемый объект</param>
        public EventIoDelete(Io dIo)
        {
            try
            {
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
