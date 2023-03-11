using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;
using IoNodeWorker.BLL.IoPlg.Lib;

namespace IoNodeWorker.BLL
{
    /// <summary>
    /// Плагин который используется по нашему назначению
    /// </summary>
    public class Io:IoBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustomClass">Тип плагина</param>
        public Io(string CustomClassTyp) : base(CustomClassTyp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomClassTyp)) throw new ApplicationException("Данный клас является затычкой вам необходимо создать свой класс организовав наследование :Io, IoI");
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка в конструкторе класса Io:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

    }
}
