using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoNodeWorker.BLL.IoPlg.ProviderPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.ProviderPlg
{
    /// <summary>
    /// Класс который реализует провайдера в виде нашего спецтального плагина
    /// </summary>
    public class MsSqlPrv:Provider, ProviderI
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
        /// <param name="ConnectionString">Строка подключения к репозиторию</param>
        public MsSqlPrv(string ConnectionString) : base()
        {
            try
            {
                base.CustomClassTyp = "MsSqlPrv";
                this.ConnectionString = ConnectionString;
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
