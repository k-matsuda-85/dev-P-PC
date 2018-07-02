namespace AggregateTool
{
    partial class Form_Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_OpenAggForm = new System.Windows.Forms.Button();
            this.Btn_OpenSettingForm = new System.Windows.Forms.Button();
            this.Btn_CloseMenu = new System.Windows.Forms.Button();
            this.Btn_OpenEditForm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_OpenAggForm
            // 
            this.Btn_OpenAggForm.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_OpenAggForm.Location = new System.Drawing.Point(26, 166);
            this.Btn_OpenAggForm.Name = "Btn_OpenAggForm";
            this.Btn_OpenAggForm.Size = new System.Drawing.Size(233, 44);
            this.Btn_OpenAggForm.TabIndex = 1;
            this.Btn_OpenAggForm.Text = "集計出力";
            this.Btn_OpenAggForm.UseVisualStyleBackColor = true;
            this.Btn_OpenAggForm.Click += new System.EventHandler(this.Btn_OpenAggForm_Click);
            // 
            // Btn_OpenSettingForm
            // 
            this.Btn_OpenSettingForm.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_OpenSettingForm.Location = new System.Drawing.Point(26, 236);
            this.Btn_OpenSettingForm.Name = "Btn_OpenSettingForm";
            this.Btn_OpenSettingForm.Size = new System.Drawing.Size(233, 44);
            this.Btn_OpenSettingForm.TabIndex = 2;
            this.Btn_OpenSettingForm.Text = "設定";
            this.Btn_OpenSettingForm.UseVisualStyleBackColor = true;
            this.Btn_OpenSettingForm.Click += new System.EventHandler(this.Btn_OpenSettingForm_Click);
            // 
            // Btn_CloseMenu
            // 
            this.Btn_CloseMenu.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_CloseMenu.Location = new System.Drawing.Point(26, 312);
            this.Btn_CloseMenu.Name = "Btn_CloseMenu";
            this.Btn_CloseMenu.Size = new System.Drawing.Size(233, 44);
            this.Btn_CloseMenu.TabIndex = 3;
            this.Btn_CloseMenu.Text = "ログアウト";
            this.Btn_CloseMenu.UseVisualStyleBackColor = true;
            this.Btn_CloseMenu.Click += new System.EventHandler(this.Btn_CloseMenu_Click);
            // 
            // Btn_OpenEditForm
            // 
            this.Btn_OpenEditForm.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_OpenEditForm.Location = new System.Drawing.Point(26, 97);
            this.Btn_OpenEditForm.Name = "Btn_OpenEditForm";
            this.Btn_OpenEditForm.Size = new System.Drawing.Size(233, 44);
            this.Btn_OpenEditForm.TabIndex = 0;
            this.Btn_OpenEditForm.Text = "レコード編集";
            this.Btn_OpenEditForm.UseVisualStyleBackColor = true;
            this.Btn_OpenEditForm.Click += new System.EventHandler(this.Btn_OpenEditForm_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(26, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(233, 44);
            this.button1.TabIndex = 4;
            this.button1.Text = "データ取得";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form_Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 375);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Btn_OpenEditForm);
            this.Controls.Add(this.Btn_CloseMenu);
            this.Controls.Add(this.Btn_OpenSettingForm);
            this.Controls.Add(this.Btn_OpenAggForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form_Menu";
            this.Text = "メニュー画面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Menu_FormClosing);
            this.Shown += new System.EventHandler(this.Form_Menu_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_OpenAggForm;
        private System.Windows.Forms.Button Btn_OpenSettingForm;
        private System.Windows.Forms.Button Btn_CloseMenu;
        private System.Windows.Forms.Button Btn_OpenEditForm;
        private System.Windows.Forms.Button button1;
    }
}