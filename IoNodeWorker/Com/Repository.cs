using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.Lib;
using IoNodeWorker.Com.RepositoryPlg.Lib;

namespace IoNodeWorker.Com
{
    /// <summary>
    /// Репозиторий с помощью которого мы работаем с источником данных
    /// </summary>
    public class Repository : RepositoryBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustomClass">Тип репозитория</param>
        /// <param name="ConnectionString">Строка подключения к репозиторию</param>
        public Repository(string CustomClassTyp, string ConnectionString) : base(CustomClassTyp, ConnectionString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomClassTyp)) throw new ApplicationException("Данный клас является затычкой вам необходимо создать свой класс организовав наследование :Repository, RepositoryI");
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка в конструкторе класса Repository:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }
    }
}
