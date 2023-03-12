using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoNodeWorker.Lib
{
    /// <summary>
    /// Список событий которые встерчаются в программе
    /// </summary>
    public enum EventEn
    {
        /// <summary>
        /// Пусто
        /// </summary>
        Empty,
        /// <summary>
        /// Успех
        /// </summary>
        Success,
        /// <summary>
        /// Дамп
        /// </summary>
        Dump,
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        Message,
        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning,
        /// <summary>
        /// Ошибка без педения программы
        /// </summary>
        Error,
        /// <summary>
        /// Фатальная ошибка вызывающая падение программы
        /// </summary>
        FatalError,
        /// <summary>
        /// Флаг тарсировки
        /// </summary>
        Trace
    }
}
