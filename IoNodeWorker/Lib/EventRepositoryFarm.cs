using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Com;

namespace IoNodeWorker.Lib
{
    /// <summary>
    /// Для событий обработки изменений репозитория в текущей ферме
    /// </summary>
    public class EventRepositoryFarm : EventArgs
    {
        /// <summary>
        /// Репозиторий
        /// </summary>
        public Repository Rep { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Rep">Кастомный репозиторий</param>
        public EventRepositoryFarm(Repository Rep)
        {
            this.Rep = Rep;
        }
    }
}
