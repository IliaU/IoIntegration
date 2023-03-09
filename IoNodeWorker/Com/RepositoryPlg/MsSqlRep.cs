using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using IoNodeWorker.Com.RepositoryPlg.Lib;
using IoNodeWorker.Lib;

namespace IoNodeWorker.Com.RepositoryPlg
{
    /// <summary>
    /// Класс для организации репозитория типа MsSql
    /// </summary>
    public sealed class MsSqlRep : Repository, RepositoryI
    {
        #region Параметры Private

        #endregion

        #region Параметры Public

        public SqlConnectionStringBuilder Scsb;

        #endregion

        #region Методы Public

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ConnectionString">Строка подключения к репозиторию</param>
        public MsSqlRep(string ConnectionString) : base("MsSqlRep", (string.IsNullOrWhiteSpace(ConnectionString) ? null : ConnectionString))
        {
            try
            {

            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка в конструкторе класса:""{0}""", ex.Message), "MsSqlRep", EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Строим строку подключения по параметрам
        /// </summary>
        /// <param name="Server">Сервер</param>
        /// <param name="ADAud">тип авторизации</param>
        /// <param name="Login">логин</param>
        /// <param name="Password">пароль</param>
        /// <param name="BD">база данных</param>
        /// <param name="VisibleError">Требуется выводить полдьзователю ошибку или нет</param>
        /// <param name="WriteLog">Логировать в системном логе или нет</param>
        /// <param name="InstalConnect">Сохранить в билдере строки подключения или нет</param>
        /// <returns></returns>
        public string InstalRepository(string Server, bool ADAud, string Login, string Password, string BD, bool VisibleError, bool WriteLog, bool InstalConnect)
        {
            SqlConnectionStringBuilder ScsbTmp = new SqlConnectionStringBuilder();
            ScsbTmp.DataSource = Server;
            ScsbTmp.IntegratedSecurity = ADAud;
            if (!ADAud)
            {
                ScsbTmp.UserID = Login;
                ScsbTmp.Password = Password;
            }
            if (BD != null && BD != string.Empty) ScsbTmp.InitialCatalog = BD;

            try
            {
                if (this.TestConnect(ScsbTmp.ConnectionString, VisibleError))
                {
                    if (InstalConnect) this.Scsb = ScsbTmp;
                    return ScsbTmp.ConnectionString;
                }
                else return null;
            }
            catch (Exception)
            {
                if (WriteLog) Log.EventSave("Не удалось создать подключение: " + Server, this.ToString(), EventEn.Error);
                throw;
            }
        }

        /// <summary>
        /// Получение списка доступных баз данных
        /// </summary>
        /// <param name="ConnectionString">Строка подключеиня которыю используем для получения списка</param>
        /// <returns>Список доступных баз данных</returns>
        public List<string> GetBdList(string ConnectionString)
        {
            List<string> rez = new List<string>();
            string SQL = "Select name, compatibility_level, is_read_only from sys.databases Where state_desc='ONLINE' and name not in ('master','tempdb','model','msdb') Order by name";

            try
            {
                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".GetBdList", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (SqlDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    rez.Add(dr.GetValue(0).ToString());
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (SqlException ex)
            {
                base.EventSave(string.Format("Произошла ошибка при получении списка доступных баз даных. {0}", ex.Message), GetType().Name + ".GetBdList", EventEn.Error, true, false);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".GetBdList", EventEn.Dump, true, false);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произошла ошибка при получении списка доступных баз даных. {0}", ex.Message), GetType().Name + ".GetBdList", EventEn.Error, true, false);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".GetBdList", EventEn.Dump, true, false);
                throw;
            }
            finally
            {
            }
        }

        #endregion

        #region Методы Public Override

        /// <summary>
        /// Проверка строки подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения для проверки</param>
        /// <param name="VisibleError">True если при проверке подключения надо выводить сообщения пользователю</param>
        /// <returns>True - Если база доступна | False - Если база не доступна</returns>
        public override bool TestConnect(string ConnectionString, bool VisibleError)
        {
            try
            {
                // Проверка подключения
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    base.VersionDB = con.ServerVersion; // Если не упали, значит подключение создано верно, тогда сохраняем переданные параметры
                    con.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (VisibleError || Com.Config.Trace) base.EventSave(string.Format(@"Ошибка при проверке подключения:""{0}""", ex.Message), "TestConnect", EventEn.Error, true, true);

                // Отображаем ошибку если это нужно
                if (VisibleError) MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Вывод строки подключения в лог или интерфейс пользователя с затиранием пароля
        /// </summary>
        /// <returns>Строка подключения которую мы можем безопасно передавать пользователю так как пароль должен быть затёрт</returns>
        public override string PrintConnectionString()
        {
            try
            {
                if (base.ConnectionString != null && base.ConnectionString.Trim() != string.Empty)
                {
                    this.Scsb = new SqlConnectionStringBuilder(base.ConnectionString);
                    string Pssword = Scsb.Password;

                    if (string.IsNullOrWhiteSpace(Pssword)) return base.ConnectionString;
                    else
                    {
                        Scsb.Password = "*****";
                        return Scsb.ConnectionString;
                    }
                }
            }
            catch (Exception ex) { base.EventSave(string.Format(@"Ошибка при печати строки подключения:""{0}""", ex.Message), "PrintConnectionString", EventEn.Error, true, true); }

            return null;
        }

        /// <summary>
        /// Вывод строки подключения в лог или интерфейс пользователя с затиранием пароля
        /// </summary>
        /// <param name="Rep">Репозиторий который мы хотим править</param>
        /// <returns>True если пользователь решил сохранить репозиторй | False если пользователь не хочет сохранять</returns>
        public override bool SetupConnectDB(ref Repository Rep)
        {
            try
            {
                bool HashSaveRepository = false;

                // Вызываем форм для проверки и настройки подключения
                using (MsSql.FSetupConnectDB Frm = new MsSql.FSetupConnectDB(this))
                {
                    // Проверяем результат того что сделал пользователь
                    DialogResult drez = Frm.ShowDialog();
                    if (drez == DialogResult.Yes)
                    {
                        base.ConnectionString = Frm.NewConnectionString;

                        HashSaveRepository = true;
                    }
                }

                return HashSaveRepository;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format(@"Ошибка при создании новой строки подключения к репозиторию SetupConnectDB:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        /// <summary>
        /// Запись лога в базу данных
        /// </summary>
        /// <param name="Message">Сообщение которое пишем в базу данных</param>
        /// <param name="Source">Источник где оно возникло</param>
        /// <param name="evn">Событие системное которое фиксируем</param>
        public override void EventSaveDb(string Message, string Source, EventEn evn)
        {
            //int rez = 0;
            string SQL = string.Format("insert into [BiMon].[MonOlap_Log_Process]([DateTime],[Message],[Source],[Status]) Values(GetDate(), '{0}', '{1}', '{2}')"
                , Message.Replace("'", "''"), Source.Replace("'", "''"), evn.ToString());

            try
            {
                if (!base.HashConnect) throw new ApplicationException("Нет подключение к репозиторию данных.");

                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".EventSaveDb", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {
                        // Запускаем процедуру
                        com.ExecuteNonQuery();
                    }
                }

                //return rez;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при получении списка доступных баз даных. {0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при получении списка доступных баз даных. {0}", ex.Message));
            }
            finally
            {
            }
        }

        /*
        /// <summary>
        /// Получение списка инстансов из разы репозитория
        /// </summary>
        /// <returns>Возвращаем списокинстансов</returns>
        public override List<TInstance> GetTInstanceList()
        {
            List<TInstance> rez = new List<TInstance>();
            string SQL = "Select [ID], [LastUpdate], [Server], [Server_alias], [Instance], [On_Off], [Tabul] From [BiMon].[MonOlap_Instance] Order by [Id]";

            try
            {
                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".GetTInstanceList", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (SqlDataReader dr = com.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                int? IDTmp;
                                DateTime? LastUpdate;
                                string Server;
                                string Server_alias;
                                string Instance;
                                bool? On_Off;
                                bool? Tabul;

                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    IDTmp = null;
                                    LastUpdate = null;
                                    Server = null;
                                    Server_alias = null;
                                    Instance = null;
                                    On_Off = null;
                                    Tabul = null;

                                    try { IDTmp = int.Parse(dr.GetValue(0).ToString()); }
                                    catch (Exception) { }
                                    LastUpdate = dr.GetDateTime(1);
                                    Server = dr.GetValue(2).ToString();
                                    Server_alias = dr.GetValue(3).ToString();
                                    Instance = dr.GetValue(4).ToString();
                                    On_Off = dr.GetBoolean(5);
                                    Tabul = dr.GetBoolean(6);

                                    // Проверяем результат и если он нормальный то добавляем его в список
                                    if (LastUpdate != null && On_Off != null && Tabul != null)
                                    {
                                        TInstance newTInstance = new TInstance(IDTmp, (DateTime)LastUpdate, Server, Server_alias, Instance, (bool)On_Off, (bool)Tabul);
                                        rez.Add(newTInstance);
                                    }
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (SqlException ex)
            {
                base.EventSave(string.Format("Произошла ошибка при получении списка доступных инстансов из таблицы [BiMon].[MonOlap_Instance]. {0}", ex.Message), GetType().Name + ".GetTInstanceList", EventEn.Error, true, false);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".GetTInstanceList", EventEn.Dump, true, false);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произошла ошибка при получении списка доступных баз даных. {0}", ex.Message), GetType().Name + ".GetTInstanceList", EventEn.Error, true, false);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".GetTInstanceList", EventEn.Dump, true, false);
                throw;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Обновление или вставка нового значения в инстансе
        /// </summary>
        /// <param name="nTInstance">Инстанс который нужно установить в базе данных</param>
        /// <returns></returns>
        public override int SetTInstance(TInstance nTInstance)
        {
            int rez = 0;
            string SQL = "[BiMon].[MonOlap_Instance_Set]";

            try
            {
                if (!base.HashConnect) throw new ApplicationException("Нет подключение к репозиторию данных.");

                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".SetTInstance", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {

                        com.CommandTimeout = 900;  // 15 минут
                        com.CommandType = CommandType.StoredProcedure;
                        //
                        SqlParameter PIdOut = new SqlParameter("@IdOut", SqlDbType.Int);
                        PIdOut.Direction = ParameterDirection.ReturnValue;
                        com.Parameters.Add(PIdOut);
                        //
                        SqlParameter PId = new SqlParameter("@Id", SqlDbType.Int);
                        PId.Direction = ParameterDirection.Input;
                        if (nTInstance.ID != null) PId.Value = (int)nTInstance.ID;
                        com.Parameters.Add(PId);
                        //
                        SqlParameter PEnterDate = new SqlParameter("@LastUpdate", SqlDbType.DateTime);
                        PEnterDate.Direction = ParameterDirection.Input;
                        PEnterDate.Value = nTInstance.LastUpdate;
                        com.Parameters.Add(PEnterDate);
                        //
                        SqlParameter PServer = new SqlParameter("@Server", SqlDbType.VarChar, 25);
                        PServer.Direction = ParameterDirection.Input;
                        PServer.Value = nTInstance.Server;
                        com.Parameters.Add(PServer);
                        //
                        SqlParameter PServerAlias = new SqlParameter("@Server_alias", SqlDbType.VarChar, 25);
                        PServerAlias.Direction = ParameterDirection.Input;
                        PServerAlias.Value = nTInstance.Server_alias;
                        com.Parameters.Add(PServerAlias);
                        //
                        SqlParameter PInstance = new SqlParameter("@Instance", SqlDbType.VarChar, 25);
                        PInstance.Direction = ParameterDirection.Input;
                        PInstance.Value = (String.IsNullOrWhiteSpace(nTInstance.Instance) ? "MSSQLSERVER" : nTInstance.Instance);
                        com.Parameters.Add(PInstance);
                        //
                        SqlParameter POnOff = new SqlParameter("@On_Off", SqlDbType.Bit);
                        POnOff.Direction = ParameterDirection.Input;
                        POnOff.Value = nTInstance.On_Off;
                        com.Parameters.Add(POnOff);
                        //
                        SqlParameter PTabul = new SqlParameter("@Tabul", SqlDbType.Bit);
                        PTabul.Direction = ParameterDirection.Input;
                        PTabul.Value = nTInstance.Tabul;
                        com.Parameters.Add(PTabul);

                        // Строим строку которую воткнём в дамп в случае падения
                        SQL = GetStringPrintPar(com);

                        // Запускаем процедуру
                        com.ExecuteNonQuery();

                        // Получаем идентификатор товара
                        rez = int.Parse(PIdOut.Value.ToString());
                    }
                }

                return rez;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при сохранении или изменении информации по провайдеру. {0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при сохранении или изменении информации по провайдеру. {0}", ex.Message));
            }
            finally
            {
            }
        }

        /// <summary>
        /// Удаление инстанса
        /// </summary>
        /// <param name="dTInstance">Инстанс который нужно удалить</param>
        public override void DelTInstance(TInstance dTInstance)
        {
            string SQL = "[BiMon].[MonOlap_Instance_Del]";

            try
            {
                if (!base.HashConnect) throw new ApplicationException("Нет подключение к репозиторию данных.");

                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".DelTInstance", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {

                        com.CommandTimeout = 900;  // 15 минут
                        com.CommandType = CommandType.StoredProcedure;
                        //
                        SqlParameter PId = new SqlParameter("@Id", SqlDbType.Int);
                        PId.Direction = ParameterDirection.Input;
                        PId.Value = (int)dTInstance.ID;
                        com.Parameters.Add(PId);

                        // Строим строку которую воткнём в дамп в случае падения
                        SQL = GetStringPrintPar(com);

                        // Запускаем процедуру
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при удалении информации по провайдеру. {0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при удалении информации по провайдеру. {0}", ex.Message));
            }
            finally
            {
            }
        }

        /// <summary>
        /// Установка статусных строк состояния в базу данных для истории и дальнейшей обработки
        /// </summary>
        /// <param name="nTInstance">Инстанс статус которого надо зафиксировать</param>
        public override void SetLastStatusTInstance(TInstance nTInstance)
        {
            string SQL = "[BiMon].[MonOlap_Instance_SetLastStatus]";

            try
            {
                if (!base.HashConnect) throw new ApplicationException("Нет подключение к репозиторию данных.");

                if (Config.Trace) base.EventSave(SQL, GetType().Name + ".SetLastStatusTInstance", EventEn.Dump, true, false);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand(SQL, con))
                    {

                        com.CommandTimeout = 900;  // 15 минут
                        com.CommandType = CommandType.StoredProcedure;
                        //
                        SqlParameter PId = new SqlParameter("@Id", SqlDbType.Int);
                        PId.Direction = ParameterDirection.Input;
                        if (nTInstance.ID != null) PId.Value = (int)nTInstance.ID;
                        com.Parameters.Add(PId);
                        //
                        SqlParameter PLastEvent = new SqlParameter("@LastEvent", SqlDbType.DateTime);
                        PLastEvent.Direction = ParameterDirection.Input;
                        if (nTInstance.LastEvent != null) PLastEvent.Value = (DateTime)nTInstance.LastEvent;
                        else PLastEvent.Value = nTInstance.LastEvent;
                        com.Parameters.Add(PLastEvent);
                        //
                        SqlParameter PVersionDB = new SqlParameter("@VersionDB", SqlDbType.VarChar, 25);
                        PVersionDB.Direction = ParameterDirection.Input;
                        PVersionDB.Value = nTInstance.LastVersionDB;
                        com.Parameters.Add(PVersionDB);
                        //
                        SqlParameter PBackupDir = new SqlParameter("@BackupDir", SqlDbType.VarChar, 200);
                        PBackupDir.Direction = ParameterDirection.Input;
                        PBackupDir.Value = nTInstance.LastBackupDir;
                        com.Parameters.Add(PBackupDir);
                        //
                        SqlParameter PDataDir = new SqlParameter("@DataDir", SqlDbType.VarChar, 200);
                        PDataDir.Direction = ParameterDirection.Input;
                        PDataDir.Value = nTInstance.LastDataDir;
                        com.Parameters.Add(PDataDir);
                        //
                        SqlParameter PLogDir = new SqlParameter("@LogDir", SqlDbType.VarChar, 200);
                        PLogDir.Direction = ParameterDirection.Input;
                        PLogDir.Value = nTInstance.LastLogDir;
                        com.Parameters.Add(PLogDir);
                        //
                        SqlParameter PTempDir = new SqlParameter("@TempDir", SqlDbType.VarChar, 200);
                        PTempDir.Direction = ParameterDirection.Input;
                        PTempDir.Value = nTInstance.LastTempDir;
                        com.Parameters.Add(PTempDir);
                        //
                        SqlParameter PErrorMsg = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 2000);
                        PErrorMsg.Direction = ParameterDirection.Input;
                        if (!string.IsNullOrWhiteSpace(nTInstance.LastErrorMsg)) PErrorMsg.Value = string.Format("{0}", nTInstance.LastErrorMsg);
                        com.Parameters.Add(PErrorMsg);
                        //
                        SqlParameter PPort = new SqlParameter("@Port", SqlDbType.Int);
                        PPort.Direction = ParameterDirection.Input;
                        if (nTInstance.LastPort != null) PPort.Value = (int)nTInstance.LastPort;
                        com.Parameters.Add(PPort);

                        // Строим строку которую воткнём в дамп в случае падения
                        SQL = GetStringPrintPar(com);

                        // Запускаем процедуру
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при сохранении статуса инстанса провайдера. {0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Произошла ошибка при сохранении статуса инстанса провайдера. {0}", ex.Message));
            }
            finally
            {
            }
        }
        */
        #endregion

        #region Методы Private Method

        /// <summary>
        /// Строим строку которую будем потом печатать во время выполнения команды к данному провайдеру
        /// </summary>
        /// <param name="com">Команда</param>
        /// <returns>SQL предложение на основе соманды</returns>
        private string GetStringPrintPar(SqlCommand com)
        {
            string rez = string.Format("exec {0}", com.CommandText);
            try
            {
                // Строим строку которую воткнём в дамп в случае падения
                bool isFirst = true;
                foreach (SqlParameter item in com.Parameters)
                {
                    if (item.Direction != ParameterDirection.ReturnValue)
                    {
                        if (isFirst)
                        {
                            rez += " ";
                            isFirst = false;
                        }
                        else rez += ", ";

                        switch (item.SqlDbType)
                        {
                            case SqlDbType.Int:
                                rez += string.Format("{0}={1}", item.ParameterName, (item.SqlValue == null ? "null" : item.SqlValue));
                                break;
                            case SqlDbType.DateTime:
                                string tmp = "null";
                                if (item.SqlValue != null)
                                {
                                    DateTime dt = (DateTime)item.Value;
                                    // DateTime dt = DateTime.Parse(item.SqlValue.ToString());
                                    tmp = string.Format("Declare @P{0} datetime = convert(datetime,convert(varchar, '{1}.{2:D3}', 21),21);", item.ParameterName.Replace(@"@", ""), dt.ToString("yyyy-MM-dd HH:mm:ss"), dt.Millisecond);

                                    rez = tmp + "\r\n" + rez + string.Format("{0}={1}", item.ParameterName, @"@P" + item.ParameterName.Replace(@"@", ""));
                                }
                                else rez += string.Format("{0}={1}", item.ParameterName, "null");
                                break;
                            case SqlDbType.Money:
                                rez += string.Format("{0}={1}", item.ParameterName, (item.SqlValue == null ? "null" : item.SqlValue.ToString().Replace(",", ".")));
                                break;
                            default:
                                rez += string.Format("{0}={1}", item.ParameterName, (item.SqlValue == null ? "null" : string.Format("'{0}'", item.Value)));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произошла ошибка при парсинге параметров. {0}", ex.Message), GetType().Name + ".GetStringPrintPar", EventEn.Error, true, false);
                throw;
            }
            return rez;
        }

        #endregion

        #region Интерфейс для базового класса чтобы он мог дёргать    

        /*
        public string getStr()
        {
            return base.ConnectionString;
        }
        */

        #endregion
    }
}
