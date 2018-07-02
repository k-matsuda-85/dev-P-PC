namespace AggregateTool
{
    partial class Form_Login
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.Txtbox_LoginId = new System.Windows.Forms.TextBox();
            this.Txtbox_Password = new System.Windows.Forms.TextBox();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Lbl_LoginTitle = new System.Windows.Forms.Label();
            this.Lbl_LoginId = new System.Windows.Forms.Label();
            this.Lbl_Password = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Txtbox_LoginId
            // 
            this.Txtbox_LoginId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txtbox_LoginId.Font = new System.Drawing.Font("MS UI Gothic", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Txtbox_LoginId.Location = new System.Drawing.Point(195, 196);
            this.Txtbox_LoginId.MaxLength = 10;
            this.Txtbox_LoginId.Name = "Txtbox_LoginId";
            this.Txtbox_LoginId.Size = new System.Drawing.Size(348, 44);
            this.Txtbox_LoginId.TabIndex = 0;
            this.Txtbox_LoginId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txtbox_LoginId_KeyDown);
            this.Txtbox_LoginId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txtbox_LoginId_KeyPress);
            // 
            // Txtbox_Password
            // 
            this.Txtbox_Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txtbox_Password.Font = new System.Drawing.Font("MS UI Gothic", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Txtbox_Password.Location = new System.Drawing.Point(195, 270);
            this.Txtbox_Password.MaxLength = 10;
            this.Txtbox_Password.Name = "Txtbox_Password";
            this.Txtbox_Password.PasswordChar = '*';
            this.Txtbox_Password.Size = new System.Drawing.Size(347, 44);
            this.Txtbox_Password.TabIndex = 1;
            this.Txtbox_Password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txtbox_Password_KeyDown);
            this.Txtbox_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txtbox_Password_KeyPress);
            // 
            // Btn_Login
            // 
            this.Btn_Login.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_Login.Location = new System.Drawing.Point(371, 363);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(171, 40);
            this.Btn_Login.TabIndex = 2;
            this.Btn_Login.Text = "ログイン";
            this.Btn_Login.UseVisualStyleBackColor = true;
            this.Btn_Login.Click += new System.EventHandler(this.Btn_Login_Click);
            // 
            // Lbl_LoginTitle
            // 
            this.Lbl_LoginTitle.AutoSize = true;
            this.Lbl_LoginTitle.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_LoginTitle.Location = new System.Drawing.Point(137, 64);
            this.Lbl_LoginTitle.Name = "Lbl_LoginTitle";
            this.Lbl_LoginTitle.Size = new System.Drawing.Size(307, 64);
            this.Lbl_LoginTitle.TabIndex = 100;
            this.Lbl_LoginTitle.Text = "集計ツール";
            // 
            // Lbl_LoginId
            // 
            this.Lbl_LoginId.AutoSize = true;
            this.Lbl_LoginId.Font = new System.Drawing.Font("MS UI Gothic", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_LoginId.Location = new System.Drawing.Point(29, 198);
            this.Lbl_LoginId.Name = "Lbl_LoginId";
            this.Lbl_LoginId.Size = new System.Drawing.Size(160, 37);
            this.Lbl_LoginId.TabIndex = 101;
            this.Lbl_LoginId.Text = "ログインID";
            // 
            // Lbl_Password
            // 
            this.Lbl_Password.AutoSize = true;
            this.Lbl_Password.Font = new System.Drawing.Font("MS UI Gothic", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Password.Location = new System.Drawing.Point(28, 272);
            this.Lbl_Password.Name = "Lbl_Password";
            this.Lbl_Password.Size = new System.Drawing.Size(161, 37);
            this.Lbl_Password.TabIndex = 102;
            this.Lbl_Password.Text = "パスワード";
            // 
            // Form_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 462);
            this.Controls.Add(this.Lbl_Password);
            this.Controls.Add(this.Lbl_LoginId);
            this.Controls.Add(this.Lbl_LoginTitle);
            this.Controls.Add(this.Btn_Login);
            this.Controls.Add(this.Txtbox_Password);
            this.Controls.Add(this.Txtbox_LoginId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form_Login";
            this.Text = "ログイン画面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Login_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_LoginForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Txtbox_LoginId;
        private System.Windows.Forms.TextBox Txtbox_Password;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.Label Lbl_LoginTitle;
        private System.Windows.Forms.Label Lbl_LoginId;
        private System.Windows.Forms.Label Lbl_Password;
    }
}

