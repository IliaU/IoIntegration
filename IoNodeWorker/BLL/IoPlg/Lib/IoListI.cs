using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Интерфейс который должны реализовать все плагины которые мы будем видеть как пулы плагинов в интерфейсе
    /// </summary>
    interface IoListI
    {
        /// <summary>
        /// Событие установки статуса пула для отслеживания зависаний и качества соединения с разными источниками относительно нод
        /// </summary>
        /// <returns>Возвращаем статус проверки</returns>
        EventEn SetStatusPul();

    }
}
