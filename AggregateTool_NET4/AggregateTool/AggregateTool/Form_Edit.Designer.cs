namespace AggregateTool
{
    partial class Form_Edit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Cmb_Facility = new System.Windows.Forms.ComboBox();
            this.Dtp_SearchDate = new System.Windows.Forms.DateTimePicker();
            this.Btn_DgvUpdate = new System.Windows.Forms.Button();
            this.Btn_DgvClear = new System.Windows.Forms.Button();
            this.Btn_ReturnMenu = new System.Windows.Forms.Button();
            this.Dgv_InspectionResult = new System.Windows.Forms.DataGridView();
            this.Claim = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Payment = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CopyBtnColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.DeleteBtnColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.PatientId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InspectionDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Modality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Acceptedonly = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Parts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddMammaryGland = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.InterpretationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkDoctor = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AddParts = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Emergency = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NumberOfImager = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddPic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FaxAndMail = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Contact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequestContent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Memo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Office = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.RequestDoctor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDelete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditBtnColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lbl_Facility = new System.Windows.Forms.Label();
            this.Lbl_Date = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblCnt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchMod = new System.Windows.Forms.ComboBox();
            this.SearchDoc = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_InspectionResult)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_Search
            // 
            this.Btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Search.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_Search.Location = new System.Drawing.Point(1230, 15);
            this.Btn_Search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(77, 38);
            this.Btn_Search.TabIndex = 0;
            this.Btn_Search.Text = "検索";
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Cmb_Facility
            // 
            this.Cmb_Facility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Cmb_Facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Facility.Font = new System.Drawing.Font("メイリオ", 10F);
            this.Cmb_Facility.FormattingEnabled = true;
            this.Cmb_Facility.Location = new System.Drawing.Point(41, 14);
            this.Cmb_Facility.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Cmb_Facility.Name = "Cmb_Facility";
            this.Cmb_Facility.Size = new System.Drawing.Size(312, 28);
            this.Cmb_Facility.TabIndex = 1;
            // 
            // Dtp_SearchDate
            // 
            this.Dtp_SearchDate.CalendarFont = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Dtp_SearchDate.Font = new System.Drawing.Font("メイリオ", 10F);
            this.Dtp_SearchDate.Location = new System.Drawing.Point(419, 17);
            this.Dtp_SearchDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Dtp_SearchDate.Name = "Dtp_SearchDate";
            this.Dtp_SearchDate.Size = new System.Drawing.Size(142, 27);
            this.Dtp_SearchDate.TabIndex = 2;
            // 
            // Btn_DgvUpdate
            // 
            this.Btn_DgvUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_DgvUpdate.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_DgvUpdate.Location = new System.Drawing.Point(1202, 829);
            this.Btn_DgvUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Btn_DgvUpdate.Name = "Btn_DgvUpdate";
            this.Btn_DgvUpdate.Size = new System.Drawing.Size(77, 40);
            this.Btn_DgvUpdate.TabIndex = 3;
            this.Btn_DgvUpdate.Text = "更新";
            this.Btn_DgvUpdate.UseVisualStyleBackColor = true;
            this.Btn_DgvUpdate.Click += new System.EventHandler(this.Btn_DgvUpdate_Click);
            // 
            // Btn_DgvClear
            // 
            this.Btn_DgvClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_DgvClear.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_DgvClear.Location = new System.Drawing.Point(1286, 829);
            this.Btn_DgvClear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Btn_DgvClear.Name = "Btn_DgvClear";
            this.Btn_DgvClear.Size = new System.Drawing.Size(91, 40);
            this.Btn_DgvClear.TabIndex = 4;
            this.Btn_DgvClear.Text = "リセット";
            this.Btn_DgvClear.UseVisualStyleBackColor = true;
            this.Btn_DgvClear.Click += new System.EventHandler(this.Btn_DgvClear_Click);
            // 
            // Btn_ReturnMenu
            // 
            this.Btn_ReturnMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_ReturnMenu.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_ReturnMenu.Location = new System.Drawing.Point(1314, 14);
            this.Btn_ReturnMenu.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Btn_ReturnMenu.Name = "Btn_ReturnMenu";
            this.Btn_ReturnMenu.Size = new System.Drawing.Size(72, 38);
            this.Btn_ReturnMenu.TabIndex = 5;
            this.Btn_ReturnMenu.Text = "戻る";
            this.Btn_ReturnMenu.UseVisualStyleBackColor = true;
            this.Btn_ReturnMenu.Click += new System.EventHandler(this.Btn_ReturnMenu_Click);
            // 
            // Dgv_InspectionResult
            // 
            this.Dgv_InspectionResult.AllowUserToAddRows = false;
            this.Dgv_InspectionResult.AllowUserToDeleteRows = false;
            this.Dgv_InspectionResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Dgv_InspectionResult.ColumnHeadersHeight = 30;
            this.Dgv_InspectionResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Claim,
            this.Payment,
            this.CopyBtnColumn,
            this.DeleteBtnColumn,
            this.PatientId,
            this.PatientName,
            this.InspectionDate,
            this.Modality,
            this.Acceptedonly,
            this.Parts,
            this.AddMammaryGland,
            this.InterpretationDate,
            this.WorkDoctor,
            this.AddParts,
            this.Emergency,
            this.NumberOfImager,
            this.AddPic,
            this.FaxAndMail,
            this.Contact,
            this.RequestContent,
            this.Memo,
            this.order,
            this.Office,
            this.RequestDoctor,
            this.IsDelete,
            this.EditBtnColumn,
            this.dep});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Dgv_InspectionResult.DefaultCellStyle = dataGridViewCellStyle15;
            this.Dgv_InspectionResult.Location = new System.Drawing.Point(2, 63);
            this.Dgv_InspectionResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Dgv_InspectionResult.MultiSelect = false;
            this.Dgv_InspectionResult.Name = "Dgv_InspectionResult";
            this.Dgv_InspectionResult.RowHeadersVisible = false;
            this.Dgv_InspectionResult.RowTemplate.Height = 30;
            this.Dgv_InspectionResult.Size = new System.Drawing.Size(1384, 758);
            this.Dgv_InspectionResult.TabIndex = 6;
            this.Dgv_InspectionResult.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_InspectionResult_CellClick);
            this.Dgv_InspectionResult.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_InspectionResult_CellValueChanged);
            // 
            // Claim
            // 
            this.Claim.FalseValue = "0";
            this.Claim.HeaderText = "請求";
            this.Claim.MinimumWidth = 40;
            this.Claim.Name = "Claim";
            this.Claim.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Claim.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Claim.TrueValue = "1";
            this.Claim.Width = 40;
            // 
            // Payment
            // 
            this.Payment.FalseValue = "0";
            this.Payment.HeaderText = "支払";
            this.Payment.MinimumWidth = 40;
            this.Payment.Name = "Payment";
            this.Payment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Payment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Payment.TrueValue = "1";
            this.Payment.Width = 40;
            // 
            // CopyBtnColumn
            // 
            this.CopyBtnColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CopyBtnColumn.HeaderText = "";
            this.CopyBtnColumn.MinimumWidth = 40;
            this.CopyBtnColumn.Name = "CopyBtnColumn";
            this.CopyBtnColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CopyBtnColumn.Text = "複製";
            this.CopyBtnColumn.UseColumnTextForButtonValue = true;
            this.CopyBtnColumn.Width = 40;
            // 
            // DeleteBtnColumn
            // 
            this.DeleteBtnColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DeleteBtnColumn.HeaderText = "";
            this.DeleteBtnColumn.MinimumWidth = 40;
            this.DeleteBtnColumn.Name = "DeleteBtnColumn";
            this.DeleteBtnColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeleteBtnColumn.Text = "削除";
            this.DeleteBtnColumn.UseColumnTextForButtonValue = true;
            this.DeleteBtnColumn.Width = 40;
            // 
            // PatientId
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.PatientId.DefaultCellStyle = dataGridViewCellStyle1;
            this.PatientId.HeaderText = "患者ID";
            this.PatientId.Name = "PatientId";
            this.PatientId.Width = 120;
            // 
            // PatientName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.PatientName.DefaultCellStyle = dataGridViewCellStyle2;
            this.PatientName.HeaderText = "患者名";
            this.PatientName.Name = "PatientName";
            this.PatientName.Width = 140;
            // 
            // InspectionDate
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InspectionDate.DefaultCellStyle = dataGridViewCellStyle3;
            this.InspectionDate.HeaderText = "検査日";
            this.InspectionDate.Name = "InspectionDate";
            this.InspectionDate.Width = 69;
            // 
            // Modality
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.Modality.DefaultCellStyle = dataGridViewCellStyle4;
            this.Modality.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Modality.HeaderText = "モダリティ";
            this.Modality.Name = "Modality";
            this.Modality.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Modality.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Modality.Width = 80;
            // 
            // Acceptedonly
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Acceptedonly.DefaultCellStyle = dataGridViewCellStyle5;
            this.Acceptedonly.HeaderText = "受付専用";
            this.Acceptedonly.Name = "Acceptedonly";
            this.Acceptedonly.Width = 81;
            // 
            // Parts
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Parts.DefaultCellStyle = dataGridViewCellStyle6;
            this.Parts.HeaderText = "検査部位";
            this.Parts.Name = "Parts";
            this.Parts.Width = 240;
            // 
            // AddMammaryGland
            // 
            this.AddMammaryGland.FalseValue = "0";
            this.AddMammaryGland.HeaderText = "乳腺加算";
            this.AddMammaryGland.Name = "AddMammaryGland";
            this.AddMammaryGland.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AddMammaryGland.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AddMammaryGland.TrueValue = "1";
            this.AddMammaryGland.Width = 68;
            // 
            // InterpretationDate
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InterpretationDate.DefaultCellStyle = dataGridViewCellStyle7;
            this.InterpretationDate.HeaderText = "読影日";
            this.InterpretationDate.Name = "InterpretationDate";
            this.InterpretationDate.Width = 69;
            // 
            // WorkDoctor
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            this.WorkDoctor.DefaultCellStyle = dataGridViewCellStyle8;
            this.WorkDoctor.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.WorkDoctor.HeaderText = "読影医";
            this.WorkDoctor.Name = "WorkDoctor";
            this.WorkDoctor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // AddParts
            // 
            this.AddParts.FalseValue = "0";
            this.AddParts.HeaderText = "部位追加";
            this.AddParts.Name = "AddParts";
            this.AddParts.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AddParts.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AddParts.TrueValue = "1";
            this.AddParts.Width = 68;
            // 
            // Emergency
            // 
            this.Emergency.FalseValue = "0";
            this.Emergency.HeaderText = "緊急";
            this.Emergency.Name = "Emergency";
            this.Emergency.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Emergency.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Emergency.TrueValue = "1";
            this.Emergency.Width = 40;
            // 
            // NumberOfImager
            // 
            this.NumberOfImager.HeaderText = "画像枚数";
            this.NumberOfImager.Name = "NumberOfImager";
            this.NumberOfImager.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NumberOfImager.Width = 81;
            // 
            // AddPic
            // 
            dataGridViewCellStyle9.Format = "N0";
            dataGridViewCellStyle9.NullValue = null;
            this.AddPic.DefaultCellStyle = dataGridViewCellStyle9;
            this.AddPic.HeaderText = "画像加算";
            this.AddPic.Name = "AddPic";
            this.AddPic.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AddPic.Width = 81;
            // 
            // FaxAndMail
            // 
            this.FaxAndMail.FalseValue = "0";
            this.FaxAndMail.HeaderText = "FAX･ﾒｰﾙ";
            this.FaxAndMail.Name = "FaxAndMail";
            this.FaxAndMail.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FaxAndMail.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.FaxAndMail.TrueValue = "1";
            this.FaxAndMail.Width = 70;
            // 
            // Contact
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Contact.DefaultCellStyle = dataGridViewCellStyle10;
            this.Contact.HeaderText = "連絡事項";
            this.Contact.Name = "Contact";
            this.Contact.Width = 81;
            // 
            // RequestContent
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.RequestContent.DefaultCellStyle = dataGridViewCellStyle11;
            this.RequestContent.HeaderText = "依頼内容";
            this.RequestContent.Name = "RequestContent";
            this.RequestContent.Width = 81;
            // 
            // Memo
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Memo.DefaultCellStyle = dataGridViewCellStyle12;
            this.Memo.HeaderText = "メモ欄";
            this.Memo.MinimumWidth = 100;
            this.Memo.Name = "Memo";
            // 
            // order
            // 
            this.order.HeaderText = "オーダー";
            this.order.Name = "order";
            this.order.Visible = false;
            // 
            // Office
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Office.DefaultCellStyle = dataGridViewCellStyle13;
            this.Office.HeaderText = "事業所";
            this.Office.Name = "Office";
            this.Office.Visible = false;
            this.Office.Width = 120;
            // 
            // RequestDoctor
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.RequestDoctor.DefaultCellStyle = dataGridViewCellStyle14;
            this.RequestDoctor.HeaderText = "依頼医師名";
            this.RequestDoctor.Name = "RequestDoctor";
            this.RequestDoctor.Visible = false;
            // 
            // IsDelete
            // 
            this.IsDelete.HeaderText = "";
            this.IsDelete.Name = "IsDelete";
            this.IsDelete.Visible = false;
            // 
            // EditBtnColumn
            // 
            this.EditBtnColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.EditBtnColumn.HeaderText = "";
            this.EditBtnColumn.MinimumWidth = 55;
            this.EditBtnColumn.Name = "EditBtnColumn";
            this.EditBtnColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EditBtnColumn.Text = "レポート";
            this.EditBtnColumn.UseColumnTextForButtonValue = true;
            // 
            // dep
            // 
            this.dep.HeaderText = "依頼科";
            this.dep.Name = "dep";
            this.dep.Visible = false;
            // 
            // Lbl_Facility
            // 
            this.Lbl_Facility.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Lbl_Facility.AutoSize = true;
            this.Lbl_Facility.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Facility.Location = new System.Drawing.Point(0, 18);
            this.Lbl_Facility.Name = "Lbl_Facility";
            this.Lbl_Facility.Size = new System.Drawing.Size(35, 20);
            this.Lbl_Facility.TabIndex = 7;
            this.Lbl_Facility.Text = "施設";
            // 
            // Lbl_Date
            // 
            this.Lbl_Date.AutoSize = true;
            this.Lbl_Date.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Date.Location = new System.Drawing.Point(372, 21);
            this.Lbl_Date.Name = "Lbl_Date";
            this.Lbl_Date.Size = new System.Drawing.Size(35, 20);
            this.Lbl_Date.TabIndex = 8;
            this.Lbl_Date.Text = "日付";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker1.Font = new System.Drawing.Font("メイリオ", 10F);
            this.dateTimePicker1.Location = new System.Drawing.Point(567, 16);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(143, 27);
            this.dateTimePicker1.TabIndex = 9;
            // 
            // lblCnt
            // 
            this.lblCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCnt.Enabled = false;
            this.lblCnt.Font = new System.Drawing.Font("メイリオ", 15F);
            this.lblCnt.Location = new System.Drawing.Point(12, 829);
            this.lblCnt.Name = "lblCnt";
            this.lblCnt.Size = new System.Drawing.Size(100, 37);
            this.lblCnt.TabIndex = 10;
            this.lblCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(716, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "モダリティ";
            // 
            // SearchMod
            // 
            this.SearchMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SearchMod.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SearchMod.FormattingEnabled = true;
            this.SearchMod.Location = new System.Drawing.Point(789, 14);
            this.SearchMod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SearchMod.Name = "SearchMod";
            this.SearchMod.Size = new System.Drawing.Size(80, 28);
            this.SearchMod.TabIndex = 12;
            // 
            // SearchDoc
            // 
            this.SearchDoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SearchDoc.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SearchDoc.FormattingEnabled = true;
            this.SearchDoc.Location = new System.Drawing.Point(934, 13);
            this.SearchDoc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SearchDoc.Name = "SearchDoc";
            this.SearchDoc.Size = new System.Drawing.Size(139, 28);
            this.SearchDoc.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(880, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "読影医";
            // 
            // Form_Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1389, 874);
            this.Controls.Add(this.SearchDoc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchMod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCnt);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.Lbl_Date);
            this.Controls.Add(this.Lbl_Facility);
            this.Controls.Add(this.Dgv_InspectionResult);
            this.Controls.Add(this.Btn_ReturnMenu);
            this.Controls.Add(this.Btn_DgvClear);
            this.Controls.Add(this.Btn_DgvUpdate);
            this.Controls.Add(this.Dtp_SearchDate);
            this.Controls.Add(this.Cmb_Facility);
            this.Controls.Add(this.Btn_Search);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(581, 731);
            this.Name = "Form_Edit";
            this.Text = "レコード編集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Edit_FormClosing);
            this.Load += new System.EventHandler(this.Form_Edit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv_InspectionResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Cmb_Facility;
        private System.Windows.Forms.DateTimePicker Dtp_SearchDate;
        private System.Windows.Forms.Button Btn_DgvUpdate;
        private System.Windows.Forms.Button Btn_DgvClear;
        private System.Windows.Forms.Button Btn_ReturnMenu;
        private System.Windows.Forms.DataGridView Dgv_InspectionResult;
        private System.Windows.Forms.Label Lbl_Facility;
        private System.Windows.Forms.Label Lbl_Date;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox lblCnt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox SearchMod;
        private System.Windows.Forms.ComboBox SearchDoc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Claim;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Payment;
        private System.Windows.Forms.DataGridViewButtonColumn CopyBtnColumn;
        private System.Windows.Forms.DataGridViewButtonColumn DeleteBtnColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientId;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn InspectionDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn Modality;
        private System.Windows.Forms.DataGridViewTextBoxColumn Acceptedonly;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parts;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AddMammaryGland;
        private System.Windows.Forms.DataGridViewTextBoxColumn InterpretationDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn WorkDoctor;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AddParts;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Emergency;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberOfImager;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddPic;
        private System.Windows.Forms.DataGridViewCheckBoxColumn FaxAndMail;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contact;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestContent;
        private System.Windows.Forms.DataGridViewTextBoxColumn Memo;
        private System.Windows.Forms.DataGridViewTextBoxColumn order;
        private System.Windows.Forms.DataGridViewComboBoxColumn Office;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestDoctor;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsDelete;
        private System.Windows.Forms.DataGridViewButtonColumn EditBtnColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dep;
    }
}