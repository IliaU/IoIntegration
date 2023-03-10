using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using IoNodeWorker.Lib;

namespace IoNodeWorker
{
    public partial class FStart : Form
    {
        /// <summary>
        /// Для того чтобы статус в нижней части работал последлвательно в событиях
        /// </summary>
        private object LockEventLog = new object();

        /// <summary>
        /// Значение цвета панели со статусом по умолчанию
        /// </summary>
        private Color DefBaskCoclortSSLabel;

        /// <summary>
        /// Поток обновляющий текущий статус
        /// </summary>
        private Thread ThrAUpdateStatusCon;

        /// <summary>
        /// Флаг для аснхронного процесса который прерывает проверку подключений
        /// </summary>
        private bool IsRunAUpdateStatusCon;

        #region Системные методы и настройка из менюшки menuStrip1

        /// <summary>
        /// Конструктор
        /// </summary>
        public FStart()
        {
            try
            {
                // Логируем запуск программы
                Com.Log.EventSave("Запуск основных процессов в приложении.", GetType().Name, EventEn.Message);

                InitializeComponent();
                this.DefBaskCoclortSSLabel = this.tSSLabel.BackColor;

                // Получаем текущий статус программы
                Log_onEventLog(null, null);

                // Подписываемся на события 
                Com.Log.onEventLog += Log_onEventLog;

                // Монитор 
                this.ThrAUpdateStatusCon = new Thread(AUpdateStatusCon);
                this.ThrAUpdateStatusCon.Name = "AUpdateStatusCon";
                this.ThrAUpdateStatusCon.IsBackground = true;
                this.ThrAUpdateStatusCon.Start();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при загрузке конфигурации с ошибкой: " + ex.Message);
                Com.Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }

        }

        // Закрытие формы
        private void FStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Остановка обновления панели со статусом снизу
                this.IsRunAUpdateStatusCon = false;
                this.ThrAUpdateStatusCon.Join();

                // Пауза на всякий случай чтобы объект уничтожился
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе FStart_FormClosing:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        // Пользователь решил настроить репозиторий
        private void TsmItemConfigRepository_Click(object sender, EventArgs e)
        {
            try
            {
                using (FRepositorySetup Frm = new FRepositorySetup())
                {
                    Frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе TsmItemConfigRepository_Click:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }
        #endregion


        #region Обработка статусов в главном окне

        /// <summary>
        /// Асинхронная проверка статуса подключений
        /// </summary>
        private void AUpdateStatusCon()
        {
            try
            {
                this.IsRunAUpdateStatusCon = true;

                int CountWhile = 10;

                while (this.IsRunAUpdateStatusCon)
                {
                    if (CountWhile == 0)
                    {
                        CountWhile = 10;
                        this.Log_onEventLog(null, null);
                    }

                    Thread.Sleep(500);     // Тайм аут между проверками статуса
                }
            }
            catch (Exception) { }
        }

        // Произошло событие системное правим текущий статус
        delegate void delig_Log_onEventLog(object sender, Lib.EventLog e);
        private void Log_onEventLog(object sender, EventLog e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    lock (this.LockEventLog)
                    {
                        delig_Log_onEventLog dl = new delig_Log_onEventLog(Log_onEventLog);
                        this.Invoke(dl, new object[] { sender, e });
                    }
                }
                else
                {
                    bool HashConnectRep = Com.RepositoryFarm.HashConnect();
                    if (HashConnectRep)
                    {
                        this.PicStatRepOnline.Visible = true;
                        this.PicStatRepOfline.Visible = false;
                    }
                    else
                    {
                        this.PicStatRepOnline.Visible = false;
                        this.PicStatRepOfline.Visible = true;
                    }

                    if (e == null)
                    {
                        if (!HashConnectRep)
                        {
                            this.tSSLabel.BackColor = Color.Khaki;
                            this.tSSLabel.Text = "Подключения к репозиторию не установлено.";
                        }
                        else
                        {
                            this.tSSLabel.Text = string.Format("Подключение с базой данных версии {0} ({1}) установлено.", Com.RepositoryFarm.CurentRep.VersionDB, Com.RepositoryFarm.CurentRep.CustomClassTyp);
                        }
                    }

                    if (e != null)
                    {
                        string Message = null;
                        if (e.Message.Length < 200) Message = e.Message;
                        else
                        {
                            Message = e.Message.Substring(0, 200);
                        }

                        switch (e.Evn)
                        {
                            case Lib.EventEn.Empty:
                            case Lib.EventEn.Dump:
                                break;
                            case Lib.EventEn.Warning:
                                this.tSSLabel.BackColor = Color.Khaki;
                                this.tSSLabel.Text = Message;
                                break;
                            case Lib.EventEn.Error:
                            case Lib.EventEn.FatalError:
                                this.tSSLabel.BackColor = Color.Tomato;
                                this.tSSLabel.Text = Message;
                                break;
                            default:
                                this.tSSLabel.BackColor = this.DefBaskCoclortSSLabel;
                                this.tSSLabel.Text = Message;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "Log_onEventLog", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        #endregion

    }
}
