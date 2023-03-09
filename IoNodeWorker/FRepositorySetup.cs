using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using IoNodeWorker.Com;

namespace IoNodeWorker
{
    public partial class FRepositorySetup : Form
    {
        // Список типов репозиториев
        private List<string> cmbBoxRepTypList = new List<string>();

        // Текущий репозиторий
        private Repository CurentRepTmp = RepositoryFarm.CurentRep;

        // Конструктор
        public FRepositorySetup()
        {
            InitializeComponent();

            // Подгружаем список возможных провайдеров
            this.cmbBoxRepTyp.Items.Clear();
            cmbBoxRepTypList = RepositoryFarm.ListRepositoryName;
            foreach (string item in cmbBoxRepTypList)
            {
                this.cmbBoxRepTyp.Items.Add(item);
            }

            // Если всего один тип провайдеров существует то устанавливаем по умолчанию этот тип
            if (this.cmbBoxRepTyp.Items.Count == 1) this.cmbBoxRepTyp.SelectedIndex = 0;
        }

        // Чтение формы
        private void FRepositorySetup_Load(object sender, EventArgs e)
        {
            // Получаем текущий репозиторий
            this.CurentRepTmp = RepositoryFarm.CurentRep;

            // Если текущий репозиторий есть и он не выбран то нужно указать его тип
            if (this.CurentRepTmp != null)
            {
                for (int i = 0; i < this.cmbBoxRepTyp.Items.Count; i++)
                {
                    if (this.cmbBoxRepTyp.Items[i].ToString() == this.CurentRepTmp.CustomClassTyp) this.cmbBoxRepTyp.SelectedIndex = i;
                }
            }

            //  Если текущий репозиторий не установлен то на выход
            if (this.CurentRepTmp == null) return;
            this.txtBoxConnectionString.Text = this.CurentRepTmp.PrintConnectionString();
        }

        // Пользователь решил изменить 
        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (this.cmbBoxRepTyp.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали тип репозитория который вы будите использовать.");
                return;
            }

            // Создаём ссылку на подключение которое будем править
            Repository RepTmp = null;
            //
            // Если текущий провайдер не установлен то иницилизируем его новый экземпляр или создаём его на основе уже существующего провайдера
            if (this.CurentRepTmp == null || (this.CurentRepTmp != null && this.CurentRepTmp.CustomClassTyp != this.cmbBoxRepTyp.Items[this.cmbBoxRepTyp.SelectedIndex].ToString()))
            {
                RepTmp = RepositoryFarm.CreateRepository(this.cmbBoxRepTyp.Items[this.cmbBoxRepTyp.SelectedIndex].ToString());
            }
            else RepTmp = RepositoryFarm.CreateRepository(this.CurentRepTmp.CustomClassTyp, this.CurentRepTmp.ConnectionString);
            // 
            // Запускаем правку нового подключения
            bool HashSaveRepository = RepTmp.SetupConnectDB(ref RepTmp);

            // Пользователь сохраняет данный репозиорий в качестве текущего
            if (HashSaveRepository)
            {
                Com.RepositoryFarm.Setup(RepTmp);
            }

            // Перечитываем текущую форму
            FRepositorySetup_Load(null, null);  
        }
    }
}
