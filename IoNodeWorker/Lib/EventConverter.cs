using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoNodeWorker.Lib
{
    /// <summary>
    /// Класс для конвертации моих собственных событий
    /// </summary>
    public static class EventConverter
    {
        /// <summary>
        /// Конвертация в объект MyEvent
        /// </summary>
        /// <param name="Event">Текстовый вариант нашего события</param>
        /// <returns>Возврашаем костомизированное сопытие из перечисления</returns>
        public static EventEn Convert(string Event) //:this ()
        {
            if (Event != null && Event.Trim() != string.Empty)
            {
                foreach (EventEn item in Enum.GetValues(typeof(EventEn)))
                {
                    if (item.ToString() == Event.Trim()) return item;
                }
            }
            throw new ApplicationException("Не смогли преобразовать: " + Event);
        }
    }
}
