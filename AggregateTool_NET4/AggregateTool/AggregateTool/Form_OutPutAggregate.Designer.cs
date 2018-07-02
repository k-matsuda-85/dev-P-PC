namespace AggregateTool
{
    partial class Form_OutPutAggregate
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
            this.Dtp_AggregateMonth = new System.Windows.Forms.DateTimePicker();
            this.Lbl_DateTime = new System.Windows.Forms.Label();
            this.Btn_OutPutAggregate = new System.Windows.Forms.Button();
            this.Btn_Accounting = new System.Windows.Forms.Button();
            this.Btn_ReturnMenu = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Dtp_AggregateMonth
            // 
            this.Dtp_AggregateMonth.CustomFormat = "yyyy年MM月";
            this.Dtp_AggregateMonth.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Dtp_AggregateMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Dtp_AggregateMonth.Location = new System.Drawing.Point(27, 14);
            this.Dtp_AggregateMonth.Name = "Dtp_AggregateMonth";
            this.Dtp_AggregateMonth.Size = new System.Drawing.Size(187, 43);
            this.Dtp_AggregateMonth.TabIndex = 0;
            // 
            // Lbl_DateTime
            // 
            this.Lbl_DateTime.AutoSize = true;
            this.Lbl_DateTime.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_DateTime.Location = new System.Drawing.Point(220, 20);
            this.Lbl_DateTime.Name = "Lbl_DateTime";
            this.Lbl_DateTime.Size = new System.Drawing.Size(39, 36);
            this.Lbl_DateTime.TabIndex = 1;
            this.Lbl_DateTime.Text = "分";
            // 
            // Btn_OutPutAggregate
            // 
            this.Btn_OutPutAggregate.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_OutPutAggregate.Location = new System.Drawing.Point(27, 98);
            this.Btn_OutPutAggregate.Name = "Btn_OutPutAggregate";
            this.Btn_OutPutAggregate.Size = new System.Drawing.Size(235, 47);
            this.Btn_OutPutAggregate.TabIndex = 2;
            this.Btn_OutPutAggregate.Text = "全集計出力";
            this.Btn_OutPutAggregate.UseVisualStyleBackColor = true;
            this.Btn_OutPutAggregate.Click += new System.EventHandler(this.Btn_OutPutAggregate_Click);
            // 
            // Btn_Accounting
            // 
            this.Btn_Accounting.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_Accounting.Location = new System.Drawing.Point(27, 447);
            this.Btn_Accounting.Name = "Btn_Accounting";
            this.Btn_Accounting.Size = new System.Drawing.Size(235, 50);
            this.Btn_Accounting.TabIndex = 3;
            this.Btn_Accounting.Text = "弥生販売連携";
            this.Btn_Accounting.UseVisualStyleBackColor = true;
            this.Btn_Accounting.Click += new System.EventHandler(this.Btn_Accounting_Click);
            // 
            // Btn_ReturnMenu
            // 
            this.Btn_ReturnMenu.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_ReturnMenu.Location = new System.Drawing.Point(27, 533);
            this.Btn_ReturnMenu.Name = "Btn_ReturnMenu";
            this.Btn_ReturnMenu.Size = new System.Drawing.Size(235, 49);
            this.Btn_ReturnMenu.TabIndex = 4;
            this.Btn_ReturnMenu.Text = "戻る";
            this.Btn_ReturnMenu.UseVisualStyleBackColor = true;
            this.Btn_ReturnMenu.Click += new System.EventHandler(this.Btn_ReturnMenu_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(51, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "月次データ出力";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(51, 217);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(187, 40);
            this.button2.TabIndex = 2;
            this.button2.Text = "依頼推移表出力";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button3.Location = new System.Drawing.Point(51, 273);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(187, 40);
            this.button3.TabIndex = 2;
            this.button3.Text = "日々の集計出力";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button4.Location = new System.Drawing.Point(51, 331);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(187, 40);
            this.button4.TabIndex = 5;
            this.button4.Text = "読影医集計出力";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button5.Location = new System.Drawing.Point(51, 390);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(187, 40);
            this.button5.TabIndex = 5;
            this.button5.Text = "平均出力";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form_OutPutAggregate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 614);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.Btn_ReturnMenu);
            this.Controls.Add(this.Btn_Accounting);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Btn_OutPutAggregate);
            this.Controls.Add(this.Lbl_DateTime);
            this.Controls.Add(this.Dtp_AggregateMonth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form_OutPutAggregate";
            this.Text = "集計結果出力";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_OutPutAggregate_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker Dtp_AggregateMonth;
        private System.Windows.Forms.Label Lbl_DateTime;
        private System.Windows.Forms.Button Btn_OutPutAggregate;
        private System.Windows.Forms.Button Btn_Accounting;
        private System.Windows.Forms.Button Btn_ReturnMenu;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}