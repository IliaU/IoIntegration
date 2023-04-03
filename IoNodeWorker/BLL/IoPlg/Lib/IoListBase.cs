using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Collections;
using System.Threading;
using IoNodeWorker.Com.RepositoryPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.BLL.IoPlg.Lib
{
    /// <summary>
    /// Базовый класс наших плагинов
    /// </summary>
    public abstract partial class IoBase
    {
        /// <summary>
        /// Базовый класс реализующий список
        /// </summary>
        public class IoListBase : IEnumerable
        {
            #region Param (private)

            /// <summary>
            /// Интерфейс для базового класса пула чтобы он мог дёргать скрытыем методы
            /// </summary>
            private IoListI IoListI = null;

            /// <summary>
            /// Внутренний список
            /// </summary>
            private List<Io> IoL = new List<Io>();

            /// <summary>
            /// Поток с асинхронной обработкой
            /// </summary>
            Thread ThrAStartCompileListing;

            #endregion

            #region Param (public get; private set;)

            /// <summary>
            /// Получить текущие количество элементов в списке
            /// </summary>
            public int Count
            {
                get
                {
                    try
                    {
                        int rez = 0;

                        lock (this.IoL)
                        {
                            rez = this.IoL.Count;
                        }

                        return rez;
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в методе Count:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                        throw ex;
                    }
                }
                private set { }
            }

            /// <summary>
            /// Флаг для аснхронного процесса который отображает текущий статус процесса
            /// </summary>
            public bool IsRunThrAStartCompileListing { get; private set; } = false;

            #endregion

            #region Param (public get; protected set;)

            /// <summary>
            /// Кастомный объект для понимания какой объект перед нами
            /// </summary>
            public string CustomClassTyp { get; protected set; } = null;

            #endregion

            #region События

            /// <summary>
            /// Событие добавления объекта во внутренний список
            /// </summary>
            public event EventHandler<EventIoAdd> onIoAdd;

            /// <summary>
            /// Событие обновления объекта во внутренний список
            /// </summary>
            public event EventHandler<EventIoUpdate> onIoUpdate;

            /// <summary>
            /// Событие удаления объекта из внутреннего списока
            /// </summary>
            public event EventHandler<EventIoDelete> onIoDelete;

            #endregion

            #region Индексаторы

            /// <summary>
            /// Индексатор для поиска объекта по его индексу
            /// </summary>
            /// <param name="index">Индекс по которому мы ищем объект</param>
            /// <returns>Возвращаем найденый объект</returns>
            public Io this[int index]
            {
                get
                {
                    Io rez = null;
                    try
                    {
                        lock (this.IoL)
                        {
                            rez = this.IoL[index];
                        }
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в индексаторе Io this[int index]:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                        throw ex;
                    }
                    return rez;
                }
                private set { }
            }

            #endregion

            #region Method (public)

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="CustomClassTyp">Тип репозитория</param>
            public IoListBase(string CustomClassTyp)
            {
                try
                {
                    this.CustomClassTyp = CustomClassTyp;
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в конструкторе:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// IEnumerator для успешной работы конструкции foreach
            /// </summary>
            /// <returns>IEnumerator</returns>
            public IEnumerator GetEnumerator()
            {
                try
                {
                    lock (this.IoL)
                    {
                        //return this.IoL.GetEnumerator();
                        return new IoListBaseEnumerator(ref this.IoL);
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе Add:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            #endregion

            #region Method (public virtual)

            /// <summary>
            /// Добавление элемента в список
            /// </summary>
            /// <param name="nIo">Элемент который мы добавили в список</param>
            public virtual void Add(Io nIo)
            {
                try
                {
                    lock (this.IoL)
                    {
                        nIo.index = this.Count;
                        this.IoL.Add(nIo);
                    }

                    if (onIoAdd != null)
                    {
                        EventIoAdd MyArg = new EventIoAdd(nIo);
                        onIoAdd.Invoke(this, MyArg);
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе Add:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Обновление элемента в списоке
            /// </summary>
            /// <param name="nIo">Элемент знаяение которого мы применим</param>
            /// <param name="dIo">Элемент который необходимо обновить</param>
            public virtual void Update(Io nIo, Io dIo)
            {
                try
                {
                    lock (this.IoL)
                    {
                        int index = dIo.index;
                        nIo.index = index;
                        IoL[index] = nIo;
                    }

                    if (onIoUpdate != null)
                    {
                        EventIoUpdate MyArg = new EventIoUpdate(nIo, dIo);
                        onIoUpdate.Invoke(this, MyArg);
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе Update:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Удаление элемента из списка
            /// </summary>
            /// <param name="nIo">Элемент который необходимо обновить</param>
            public virtual void Delete(Io dIo)
            {
                try
                {
                    if (dIo.index == -1) throw new ApplicationException("У элемента не задан индекс скорее всего он не является членом списка.");

                    lock (this.IoL)
                    {
                        int index = dIo.index;

                        this.IoL.RemoveAt(index);

                        // Настраиваем индексы так как при считывании с базы их нет
                        for (int i = index; i < this.IoL.Count; i++)
                        {
                            this.SetIndex(this.IoL[i], i);
                        }
                    }

                    if (onIoDelete != null)
                    {
                        EventIoDelete MyArg = new EventIoDelete(dIo);
                        onIoDelete.Invoke(this, MyArg);
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе Update:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            #endregion

            #region Method (protected)

            /// <summary>
            /// Установка индекса в классе
            /// </summary>
            /// <param name="nIo">Объект в котором нужно установить</param>
            /// <param name="index">Индекс который надо установить</param>
            protected void SetIndex(Io nIo, int index)
            {
                try
                {
                    nIo.index = index;
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе SetIndex:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Логирование для наследуемых классов
            /// </summary>
            /// <param name="Message">Сообщение события</param>
            /// <param name="Source">Источник</param>
            /// <param name="evn">Тип события</param>
            /// <param name="isLog">Писать в лог или нет</param>
            /// <param name="Show">Отобразить сообщение пользователю или нет</param>
            protected void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
            {
                try
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "EventSave", Message), string.Format("Io.{0}.{1}", CustomClassTyp, Source), EventEn.Error, true, true);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            #endregion

            #region Method (private)

            /// <summary>
            /// Запуск асинхронного процесса обслуживающенго наш пул с точки зрения общих вещей на уровне базового класса
            /// </summary>
            private void StartCompileListing()
            {
                try
                {
                    if (IsRunThrAStartCompileListing) throw new ApplicationException("Процесс уже запущен. Надо сначало остановить текущий процесс и дождаться окончания его работы.");
                    this.IsRunThrAStartCompileListing = true;

                    // Асинхронный запуск процесса
                    this.ThrAStartCompileListing = new Thread(AStartCompileListing);
                    this.ThrAStartCompileListing.Name = "AStartCompileListing";
                    this.ThrAStartCompileListing.IsBackground = true;
                    this.ThrAStartCompileListing.Start();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.StartCompileListing", this.GetType().FullName), EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Запуск асинхронного процесса обслуживающенго наш пул с точки зрения общих вещей на уровне базового класса
            /// </summary>
            private void AStartCompileListing()
            {
                try
                {
                    // Устанавливаем тайм из конфига
                    int CountWhile = Com.Config.SecondPulRefreshStatus;

                    while (this.IsRunThrAStartCompileListing)
                    {
                        if (CountWhile == 0)
                        {
                            // Устанавливаем тайм аут из конфига
                            CountWhile = Com.Config.SecondPulRefreshStatus;

                            // Передаём управление нашему кастомному пулу чтобы он мог сохранить свою часть статусов в своей ему известной логике
                            // И получаем результат получилось ли сохранить или возникла ошибка
                            EventEn LastStatus = IoListI.SetStatusPul();

                            // Если появилось подключение к базе данных и ещё небыло успешной регистрации нашего пула то делаем её в системе для того чтобы сервис знал о том что сервис такой существует 
                            if (Com.RepositoryFarm.CurentRep != null && Com.RepositoryFarm.CurentRep.HashConnect)
                            {
                                // Фиксируем версию нашего приложения и его статус
                                Version Ver = Assembly.GetExecutingAssembly().GetName().Version;
                                ((RepositoryI)Com.RepositoryFarm.CurentRep).PulBasicSetStatus(Environment.MachineName, this.CustomClassTyp, DateTime.Now, Ver.ToString(), LastStatus.ToString());
                            }
                        }

                        Thread.Sleep(1000);     // Тайм аут между проверками статуса
                        CountWhile--;
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.StartCompileListing", this.GetType().FullName), EventEn.Error, true, false);
                    throw ex;
                }
                finally
                {
                    this.IsRunThrAStartCompileListing = false;
                }
            }

            /// <summary>
            /// Остановка асинхронного процесса обслуживающенго наш пул с точки зрения общих вещей на уровне базового класса без ожидания завершения процесса
            /// </summary>
            private void StopCompileListing()
            {
                try
                {
                    this.IsRunThrAStartCompileListing = false;
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.StopCompileListing", this.GetType().FullName), EventEn.Error, true, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Остановка аснхронных процессов перед выключением всех потоков
            /// </summary>
            /// <param name="Aborting">True если с прерывением всех процессов жёстное отклучение всех процессов</param>
            public void Join(bool Aborting)
            {
                try
                {
                    if (Aborting)
                    { 
                        this.ThrAStartCompileListing.Abort();
                    }
                    else
                    {
                        this.IsRunThrAStartCompileListing = false;
                        this.ThrAStartCompileListing.Join();
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.Join", this.GetType().FullName), EventEn.Error, true, false);
                    throw ex;
                }
            }

            #endregion

            #region CrossClass

            /// <summary>
            /// Внутренний класс для линковки интерфейсов састомного класса скрытых для пользователя
            /// </summary>
            public class CrossLink
            {
                // Кастомный объект пула
                private IoListBase CustIoList;

                /// <summary>
                /// Линкуеминтерфейс IoI скрытый для пользователя
                /// </summary>
                /// <param name="CustIo">Кастомный обьект для линковки</param>
                public CrossLink(IoListBase CustIoList)
                {
                    try
                    {
                        this.CustIoList = CustIoList;
                        CustIoList.IoListI = (IoListI)CustIoList;
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, false);
                        throw ex;
                    }
                }

                /// <summary>
                /// Запуск асинхронного процесса обслуживающенго наш пул с точки зрения общих вещей на уровне базового класса
                /// </summary>
                public void StartCompileListing()
                {
                    try
                    {
                        this.CustIoList.StartCompileListing();
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.StartCompileListing", this.GetType().FullName), EventEn.Error, true, false);
                        throw ex;
                    }
                }

                /// <summary>
                /// Остановка асинхронного процесса обслуживающенго наш пул с точки зрения общих вещей на уровне базового класса без ожидания завершения процесса
                /// </summary>
                public void StopCompileListing()
                {
                    try
                    {
                        this.CustIoList.StopCompileListing();
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.StopCompileListing", this.GetType().FullName), EventEn.Error, true, false);
                        throw ex;
                    }
                }

                /// <summary>
                /// Остановка аснхронных процессов перед выключением всех потоков
                /// </summary>
                /// <param name="Aborting">True если с прерывением всех процессов жёстное отклучение всех процессов</param>
                public void Join(bool Aborting)
                {
                    try
                    {
                        this.CustIoList.Join(Aborting);
                    }
                    catch (Exception ex)
                    {
                        Com.Log.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), String.Format("{0}.Join", this.GetType().FullName), EventEn.Error, true, false);
                        throw ex;
                    }
                }
            }

            #endregion
        }
    }
}
