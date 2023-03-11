using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Собственный нумератор
    /// </summary>
    public class IoListBaseEnumerator : IEnumerator
    {
        /// <summary>
        /// Внутренний список
        /// </summary>
        private List<Io> IoL;

        /// <summary>
        /// Позиция
        /// </summary>
        int position = -1;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="IoL">Список по которому строим перечеслитель</param>
        public IoListBaseEnumerator(ref List<Io> IoL)
        {
            try
            {
                this.IoL = IoL;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Получение текущего объекта
        /// </summary>
        public object Current
        {
            get
            {
                try
                {
                    lock (this.IoL)
                    {
                        if (position == -1 || position >= this.IoL.Count) throw new ArgumentException();
                        return this.IoL[position];
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в параметре Current:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Переключение перечислитиля на следующий элемент
        /// </summary>
        /// <returns>статус успешно илинет</returns>
        public bool MoveNext()
        {
            try
            {
                lock (this.IoL)
                {
                    if (position < this.IoL.Count - 1)
                    {
                        position++;
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе MoveNext:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Сброс перечислителя
        /// </summary>
        public void Reset()
        {
            try
            {
                lock (this.IoL)
                {
                    position = -1;
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе Reset:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                throw ex;
            }
        }
    }
}
