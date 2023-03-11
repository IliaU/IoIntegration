using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL
{
    /// <summary>
    /// Класс реализующий логику работы с листом плагинов
    /// </summary>
    public class IoList: IoPlg.Lib.IoBase.IoListBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustomClass">Тип плагина</param>
        public IoList(string CustomClassTyp) : base(CustomClassTyp)
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
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }
                
    }
}
