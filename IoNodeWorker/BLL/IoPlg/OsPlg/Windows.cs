using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.OsPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.OsPlg
{
    /// <summary>
    /// Класс который реализует объект типа операционная система (namespace в базе данных Os)
    /// </summary>
    public class Windows : Os, OsI
    {
        #region Параметры Private


        #endregion

        #region Параметры Public


        #endregion

        #region Param (public get; private set;)

        #endregion

        #region Методы Public

        /// <summary>
        /// Конструктор
        /// </summary>
        public Windows() : base()
        {
            try
            {
                base.CustomClassTyp = "Windows";
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

        #endregion
    }
}
