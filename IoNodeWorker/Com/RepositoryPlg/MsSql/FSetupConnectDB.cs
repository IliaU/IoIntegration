using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using IoNodeWorker.Lib;

namespace IoNodeWorker.Com.RepositoryPlg.MsSql
{
    public partial class FSetupConnectDB : Form
    {
        private MsSqlRep Rep;
        private Boolean isEdit = false;
        
        private string OldConnectionString;
        public string NewConnectionString;
        
        /// <summary>
        /// Конструктор по настройке нативного подключения к базе данных
        /// </summary>
        /// <param name="Rep">Репозиторий который мы настраиваем</param>
        public FSetupConnectDB(MsSqlRep Rep)
        {
            try
            {
                this.Rep = Rep;
                this.NewConnectionString = Rep.ConnectionString;
                InitializeComponent();

                // Если мы редактируем элемент, то подгружаем имя провайдера
                if (Rep.ConnectionString != null && Rep.ConnectionString != string.Empty)
                {
                    this.Rep.Scsb = new SqlConnectionStringBuilder(Rep.ConnectionString);
                    this.txtBox_Server_MSSQL.Text = this.Rep.Scsb.DataSource;
                    this.cBox_Audent.Checked = this.Rep.Scsb.IntegratedSecurity;
                    if (this.cBox_Audent.Checked)
                    {
                        this.txtBox_Login_MSSQL.Text = this.Rep.Scsb.UserID;
                        this.txtBox_Passvord_MSSQL.Text = this.Rep.Scsb.Password;
                    }
                    this.OldConnectionString = this.Rep.ConnectionString;
                    this.isEdit = true;
                    BD_MSSQL_Clear_MSSQL();
                }
                this.isEdit = false;
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в конструкторе FSetupConnectDB:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Чтение формы
        private void FSetupConnectDB_Load(object sender, EventArgs e)
        {
            try
            {

                if (this.cBox_Audent.Checked) this.panel_MSSQL_Login.Visible = false;
                else this.panel_MSSQL_Login.Visible = true;
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе FSetupConnectDB_Load:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь решил протестировать соединение
        private void btnTestODBC_Click(object sender, EventArgs e)
        {
            try
            {
                this.NewConnectionString = this.Rep.InstalRepository(this.txtBox_Server_MSSQL.Text, this.cBox_Audent.Checked, (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Login_MSSQL.Text) ? null : this.txtBox_Login_MSSQL.Text), (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Passvord_MSSQL.Text) ? null : this.txtBox_Passvord_MSSQL.Text), this.cBox_BD_MSSQL.SelectedText, true, false, false);
                if (!string.IsNullOrWhiteSpace(this.NewConnectionString)) MessageBox.Show("Тестирование подключения завершилось успешно");
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе btnTestODBC_Click:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь сохраняет подключение
        private void btnSaveODBC_Click(object sender, EventArgs e)
        {
            try
            {
                this.NewConnectionString = this.Rep.InstalRepository(this.txtBox_Server_MSSQL.Text, this.cBox_Audent.Checked, (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Login_MSSQL.Text) ? null : this.txtBox_Login_MSSQL.Text), (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Passvord_MSSQL.Text) ? null : this.txtBox_Passvord_MSSQL.Text), this.cBox_BD_MSSQL.Text, true, false, false);
                if (!string.IsNullOrWhiteSpace(this.NewConnectionString)) this.DialogResult = DialogResult.Yes;
                else this.DialogResult = DialogResult.No;

                this.Close();
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе btnSaveODBC_Click:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Появление мышки над выбором базы данных
        private bool MouseEnterFlag = false; // Флаг, по которому срабатывает перестроение списка баз данных
        private void cBox_BD_MSSQL_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                // проверяем флаг, возможно перечитывать список баз не нужно
                if (this.MouseEnterFlag)
                {
                    string ConnectionStringTmp = null;
                    try
                    {
                        ConnectionStringTmp = this.Rep.InstalRepository(this.txtBox_Server_MSSQL.Text, this.cBox_Audent.Checked, (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Login_MSSQL.Text) ? null : this.txtBox_Login_MSSQL.Text), (this.cBox_Audent.Checked && !string.IsNullOrWhiteSpace(this.txtBox_Passvord_MSSQL.Text) ? null : this.txtBox_Passvord_MSSQL.Text), this.cBox_BD_MSSQL.SelectedText, false, false, false);
                    }
                    catch (Exception) { }

                    // проверяем подключение и получаем список доступных баз
                    if (!string.IsNullOrWhiteSpace(ConnectionStringTmp))
                    {
                        foreach (string item in this.Rep.GetBdList(ConnectionStringTmp))
                        {
                            this.cBox_BD_MSSQL.Items.Add(item);
                        }
                    }

                }
                this.MouseEnterFlag = false;
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе cBox_BD_MSSQL_MouseEnter:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }
        // Чистка списка доступных баз данных
        private void BD_MSSQL_Clear_MSSQL()
        {
            try
            {
                this.cBox_BD_MSSQL.Items.Clear();

                MouseEnterFlag = true;

                // Делаем если только мы не на редактировании
                if (!string.IsNullOrWhiteSpace(this.OldConnectionString))
                {
                    if (this.isEdit)
                    {
                        this.txtBox_Login_MSSQL.Text = this.Rep.Scsb.UserID;
                        this.txtBox_Passvord_MSSQL.Text = this.Rep.Scsb.Password;
                    }

                    // Подгружаем список доступных баз
                    try
                    {
                        cBox_BD_MSSQL_MouseEnter(null, null);
                        if (this.cBox_BD_MSSQL.Items.Count == 0) this.cBox_BD_MSSQL.Text = "";
                        else this.cBox_BD_MSSQL.Text = this.Rep.Scsb.InitialCatalog;
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе BD_MSSQL_Clear_MSSQL:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь изменил тип аудентификации при подключении к MS SQL
        private void cBox_Audent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FSetupConnectDB_Load(null, null);
                BD_MSSQL_Clear_MSSQL();
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе cBox_Audent_CheckedChanged:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь изменил имя сервера
        private void txtBox_Server_MSSQL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.NewConnectionString))
                {
                    this.NewConnectionString = null;
                    BD_MSSQL_Clear_MSSQL();
                }
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе txtBox_Server_MSSQL_TextChanged:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь изменил Login
        private void txtBox_Login_MSSQL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.NewConnectionString))
                {
                    this.NewConnectionString = null;
                    BD_MSSQL_Clear_MSSQL();
                }
                this.MouseEnterFlag = true;
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе txtBox_Login_MSSQL_TextChanged:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }

        // Пользователь изменил Password
        private void txtBox_Passvord_MSSQL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.NewConnectionString))
                {
                    this.NewConnectionString = null;
                    BD_MSSQL_Clear_MSSQL();
                }
                this.MouseEnterFlag = true;
            }
            catch (Exception ex)
            {
                Log.EventSave(string.Format(@"Ошибка в методе txtBox_Passvord_MSSQL_TextChanged:""{0}""", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
                throw ex;
            }
        }
    }
}
