
namespace IoNodeWorker
{
    partial class FStart
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FStart));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlTopRight = new System.Windows.Forms.Panel();
            this.pnlTopFill = new System.Windows.Forms.Panel();
            this.pnlFill = new System.Windows.Forms.Panel();
            this.tSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.TsmItemConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmItemConfigRepository = new System.Windows.Forms.ToolStripMenuItem();
            this.PicStatRepOnline = new System.Windows.Forms.PictureBox();
            this.PicStatRepOfline = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlTopRight.SuspendLayout();
            this.pnlTopFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicStatRepOnline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicStatRepOfline)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmItemConfig});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(671, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSSLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlTopFill);
            this.pnlTop.Controls.Add(this.pnlTopRight);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 44);
            this.pnlTop.TabIndex = 2;
            // 
            // pnlTopRight
            // 
            this.pnlTopRight.Controls.Add(this.PicStatRepOfline);
            this.pnlTopRight.Controls.Add(this.PicStatRepOnline);
            this.pnlTopRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlTopRight.Location = new System.Drawing.Point(671, 0);
            this.pnlTopRight.Name = "pnlTopRight";
            this.pnlTopRight.Size = new System.Drawing.Size(129, 44);
            this.pnlTopRight.TabIndex = 0;
            // 
            // pnlTopFill
            // 
            this.pnlTopFill.Controls.Add(this.menuStrip1);
            this.pnlTopFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTopFill.Location = new System.Drawing.Point(0, 0);
            this.pnlTopFill.Name = "pnlTopFill";
            this.pnlTopFill.Size = new System.Drawing.Size(671, 44);
            this.pnlTopFill.TabIndex = 1;
            // 
            // pnlFill
            // 
            this.pnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFill.Location = new System.Drawing.Point(0, 44);
            this.pnlFill.Name = "pnlFill";
            this.pnlFill.Size = new System.Drawing.Size(800, 380);
            this.pnlFill.TabIndex = 3;
            // 
            // tSSLabel
            // 
            this.tSSLabel.Name = "tSSLabel";
            this.tSSLabel.Size = new System.Drawing.Size(121, 20);
            this.tSSLabel.Text = "Лог программы";
            // 
            // TsmItemConfig
            // 
            this.TsmItemConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmItemConfigRepository});
            this.TsmItemConfig.Name = "TsmItemConfig";
            this.TsmItemConfig.Size = new System.Drawing.Size(97, 26);
            this.TsmItemConfig.Text = "Настройка";
            // 
            // TsmItemConfigRepository
            // 
            this.TsmItemConfigRepository.Name = "TsmItemConfigRepository";
            this.TsmItemConfigRepository.Size = new System.Drawing.Size(262, 26);
            this.TsmItemConfigRepository.Text = "Настройка репозитория";
            this.TsmItemConfigRepository.Click += new System.EventHandler(this.TsmItemConfigRepository_Click);
            // 
            // PicStatRepOnline
            // 
            this.PicStatRepOnline.Dock = System.Windows.Forms.DockStyle.Left;
            this.PicStatRepOnline.Image = ((System.Drawing.Image)(resources.GetObject("PicStatRepOnline.Image")));
            this.PicStatRepOnline.Location = new System.Drawing.Point(0, 0);
            this.PicStatRepOnline.Name = "PicStatRepOnline";
            this.PicStatRepOnline.Size = new System.Drawing.Size(44, 44);
            this.PicStatRepOnline.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicStatRepOnline.TabIndex = 0;
            this.PicStatRepOnline.TabStop = false;
            this.PicStatRepOnline.Visible = false;
            // 
            // PicStatRepOfline
            // 
            this.PicStatRepOfline.Dock = System.Windows.Forms.DockStyle.Left;
            this.PicStatRepOfline.Image = ((System.Drawing.Image)(resources.GetObject("PicStatRepOfline.Image")));
            this.PicStatRepOfline.Location = new System.Drawing.Point(44, 0);
            this.PicStatRepOfline.Name = "PicStatRepOfline";
            this.PicStatRepOfline.Size = new System.Drawing.Size(44, 44);
            this.PicStatRepOfline.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicStatRepOfline.TabIndex = 0;
            this.PicStatRepOfline.TabStop = false;
            this.PicStatRepOfline.Visible = false;
            // 
            // FStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlFill);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FStart";
            this.Text = "Управление нодой на данном кластере";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FStart_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTopRight.ResumeLayout(false);
            this.pnlTopFill.ResumeLayout(false);
            this.pnlTopFill.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicStatRepOnline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicStatRepOfline)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlTopFill;
        private System.Windows.Forms.Panel pnlTopRight;
        private System.Windows.Forms.Panel pnlFill;
        private System.Windows.Forms.ToolStripStatusLabel tSSLabel;
        private System.Windows.Forms.ToolStripMenuItem TsmItemConfig;
        private System.Windows.Forms.ToolStripMenuItem TsmItemConfigRepository;
        private System.Windows.Forms.PictureBox PicStatRepOnline;
        private System.Windows.Forms.PictureBox PicStatRepOfline;
    }
}

