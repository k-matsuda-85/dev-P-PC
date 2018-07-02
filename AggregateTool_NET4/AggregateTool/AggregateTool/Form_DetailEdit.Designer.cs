namespace AggregateTool
{
    partial class Form_DetailEdit
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
            this.Office = new System.Windows.Forms.TextBox();
            this.PatientId = new System.Windows.Forms.TextBox();
            this.PatientName = new System.Windows.Forms.TextBox();
            this.Parts = new System.Windows.Forms.TextBox();
            this.RequestDoctor = new System.Windows.Forms.TextBox();
            this.RequestContent = new System.Windows.Forms.TextBox();
            this.Acceptedonly = new System.Windows.Forms.TextBox();
            this.Contact = new System.Windows.Forms.TextBox();
            this.Memo = new System.Windows.Forms.TextBox();
            this.Btn_Update = new System.Windows.Forms.Button();
            this.Btn_ReSet = new System.Windows.Forms.Button();
            this.Btn_Edit = new System.Windows.Forms.Button();
            this.InspectionDate = new System.Windows.Forms.DateTimePicker();
            this.Modality = new System.Windows.Forms.ComboBox();
            this.WorkDoctor = new System.Windows.Forms.ComboBox();
            this.InterpretationDate = new System.Windows.Forms.DateTimePicker();
            this.AddParts = new System.Windows.Forms.GroupBox();
            this.AddPartsFalse = new System.Windows.Forms.RadioButton();
            this.AddPartsTrue = new System.Windows.Forms.RadioButton();
            this.FaxAndMail = new System.Windows.Forms.GroupBox();
            this.FaxAndMailFalse = new System.Windows.Forms.RadioButton();
            this.FaxAndMailTrue = new System.Windows.Forms.RadioButton();
            this.Claim = new System.Windows.Forms.GroupBox();
            this.ClaimFalse = new System.Windows.Forms.RadioButton();
            this.ClaimTrue = new System.Windows.Forms.RadioButton();
            this.Emergency = new System.Windows.Forms.GroupBox();
            this.EmegencyFalse = new System.Windows.Forms.RadioButton();
            this.EmegencyTrue = new System.Windows.Forms.RadioButton();
            this.AddMammaryGland = new System.Windows.Forms.GroupBox();
            this.AddMammaryGlandFalse = new System.Windows.Forms.RadioButton();
            this.AddMammaryGlandTrue = new System.Windows.Forms.RadioButton();
            this.AddPic = new System.Windows.Forms.GroupBox();
            this.AddPicFalse = new System.Windows.Forms.RadioButton();
            this.AddPicTrue = new System.Windows.Forms.RadioButton();
            this.Payment = new System.Windows.Forms.GroupBox();
            this.PaymentFalse = new System.Windows.Forms.RadioButton();
            this.PaymentTrue = new System.Windows.Forms.RadioButton();
            this.NumberOfImager = new System.Windows.Forms.TextBox();
            this.Lbl_Office = new System.Windows.Forms.Label();
            this.Lbl_PatientId = new System.Windows.Forms.Label();
            this.Lbl_PatientName = new System.Windows.Forms.Label();
            this.Lbl_InspectionDate = new System.Windows.Forms.Label();
            this.Lbl_Modality = new System.Windows.Forms.Label();
            this.Lbl_Parts = new System.Windows.Forms.Label();
            this.Lbl_RequestDoctor = new System.Windows.Forms.Label();
            this.Lbl_WorkDoctor = new System.Windows.Forms.Label();
            this.Lbl_InterpretationDate = new System.Windows.Forms.Label();
            this.Lbl_RequestContent = new System.Windows.Forms.Label();
            this.Lbl_Acceptedonly = new System.Windows.Forms.Label();
            this.Lbl_Contact = new System.Windows.Forms.Label();
            this.Lbl_NumberOfImager = new System.Windows.Forms.Label();
            this.Lbl_Memo = new System.Windows.Forms.Label();
            this.AddParts.SuspendLayout();
            this.FaxAndMail.SuspendLayout();
            this.Claim.SuspendLayout();
            this.Emergency.SuspendLayout();
            this.AddMammaryGland.SuspendLayout();
            this.AddPic.SuspendLayout();
            this.Payment.SuspendLayout();
            this.SuspendLayout();
            // 
            // Office
            // 
            this.Office.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Office.Location = new System.Drawing.Point(112, 34);
            this.Office.MaxLength = 20;
            this.Office.Name = "Office";
            this.Office.ReadOnly = true;
            this.Office.Size = new System.Drawing.Size(206, 31);
            this.Office.TabIndex = 0;
            // 
            // PatientId
            // 
            this.PatientId.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PatientId.Location = new System.Drawing.Point(112, 94);
            this.PatientId.MaxLength = 20;
            this.PatientId.Name = "PatientId";
            this.PatientId.Size = new System.Drawing.Size(282, 31);
            this.PatientId.TabIndex = 1;
            // 
            // PatientName
            // 
            this.PatientName.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PatientName.Location = new System.Drawing.Point(112, 154);
            this.PatientName.MaxLength = 20;
            this.PatientName.Name = "PatientName";
            this.PatientName.Size = new System.Drawing.Size(282, 31);
            this.PatientName.TabIndex = 2;
            // 
            // Parts
            // 
            this.Parts.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Parts.Location = new System.Drawing.Point(112, 274);
            this.Parts.MaxLength = 20;
            this.Parts.Name = "Parts";
            this.Parts.Size = new System.Drawing.Size(193, 31);
            this.Parts.TabIndex = 5;
            // 
            // RequestDoctor
            // 
            this.RequestDoctor.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RequestDoctor.Location = new System.Drawing.Point(557, 94);
            this.RequestDoctor.MaxLength = 20;
            this.RequestDoctor.Name = "RequestDoctor";
            this.RequestDoctor.Size = new System.Drawing.Size(152, 31);
            this.RequestDoctor.TabIndex = 6;
            // 
            // RequestContent
            // 
            this.RequestContent.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RequestContent.Location = new System.Drawing.Point(557, 334);
            this.RequestContent.MaxLength = 20;
            this.RequestContent.Name = "RequestContent";
            this.RequestContent.Size = new System.Drawing.Size(384, 31);
            this.RequestContent.TabIndex = 16;
            // 
            // Acceptedonly
            // 
            this.Acceptedonly.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Acceptedonly.Location = new System.Drawing.Point(557, 428);
            this.Acceptedonly.MaxLength = 20;
            this.Acceptedonly.Name = "Acceptedonly";
            this.Acceptedonly.Size = new System.Drawing.Size(388, 31);
            this.Acceptedonly.TabIndex = 17;
            // 
            // Contact
            // 
            this.Contact.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Contact.Location = new System.Drawing.Point(557, 374);
            this.Contact.MaxLength = 20;
            this.Contact.Name = "Contact";
            this.Contact.Size = new System.Drawing.Size(384, 31);
            this.Contact.TabIndex = 18;
            // 
            // Memo
            // 
            this.Memo.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Memo.Location = new System.Drawing.Point(557, 482);
            this.Memo.MaxLength = 500;
            this.Memo.Multiline = true;
            this.Memo.Name = "Memo";
            this.Memo.Size = new System.Drawing.Size(406, 126);
            this.Memo.TabIndex = 20;
            // 
            // Btn_Update
            // 
            this.Btn_Update.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_Update.Location = new System.Drawing.Point(652, 674);
            this.Btn_Update.Name = "Btn_Update";
            this.Btn_Update.Size = new System.Drawing.Size(104, 50);
            this.Btn_Update.TabIndex = 21;
            this.Btn_Update.Text = "更新";
            this.Btn_Update.UseVisualStyleBackColor = true;
            this.Btn_Update.Click += new System.EventHandler(this.Btn_Update_Click);
            // 
            // Btn_ReSet
            // 
            this.Btn_ReSet.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_ReSet.Location = new System.Drawing.Point(765, 674);
            this.Btn_ReSet.Name = "Btn_ReSet";
            this.Btn_ReSet.Size = new System.Drawing.Size(100, 50);
            this.Btn_ReSet.TabIndex = 22;
            this.Btn_ReSet.Text = "リセット";
            this.Btn_ReSet.UseVisualStyleBackColor = true;
            this.Btn_ReSet.Click += new System.EventHandler(this.Btn_ReSet_Click);
            // 
            // Btn_Edit
            // 
            this.Btn_Edit.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Btn_Edit.Location = new System.Drawing.Point(871, 674);
            this.Btn_Edit.Name = "Btn_Edit";
            this.Btn_Edit.Size = new System.Drawing.Size(100, 50);
            this.Btn_Edit.TabIndex = 23;
            this.Btn_Edit.Text = "戻る";
            this.Btn_Edit.UseVisualStyleBackColor = true;
            this.Btn_Edit.Click += new System.EventHandler(this.Btn_Edit_Click);
            // 
            // InspectionDate
            // 
            this.InspectionDate.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.InspectionDate.Location = new System.Drawing.Point(112, 334);
            this.InspectionDate.Name = "InspectionDate";
            this.InspectionDate.Size = new System.Drawing.Size(169, 31);
            this.InspectionDate.TabIndex = 30;
            // 
            // Modality
            // 
            this.Modality.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Modality.FormattingEnabled = true;
            this.Modality.Location = new System.Drawing.Point(112, 213);
            this.Modality.Name = "Modality";
            this.Modality.Size = new System.Drawing.Size(91, 32);
            this.Modality.TabIndex = 4;
            // 
            // WorkDoctor
            // 
            this.WorkDoctor.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WorkDoctor.FormattingEnabled = true;
            this.WorkDoctor.Location = new System.Drawing.Point(557, 153);
            this.WorkDoctor.Name = "WorkDoctor";
            this.WorkDoctor.Size = new System.Drawing.Size(152, 32);
            this.WorkDoctor.TabIndex = 7;
            // 
            // InterpretationDate
            // 
            this.InterpretationDate.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.InterpretationDate.Location = new System.Drawing.Point(557, 214);
            this.InterpretationDate.Name = "InterpretationDate";
            this.InterpretationDate.Size = new System.Drawing.Size(206, 31);
            this.InterpretationDate.TabIndex = 8;
            // 
            // AddParts
            // 
            this.AddParts.Controls.Add(this.AddPartsFalse);
            this.AddParts.Controls.Add(this.AddPartsTrue);
            this.AddParts.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddParts.Location = new System.Drawing.Point(52, 407);
            this.AddParts.Name = "AddParts";
            this.AddParts.Size = new System.Drawing.Size(153, 52);
            this.AddParts.TabIndex = 9;
            this.AddParts.TabStop = false;
            this.AddParts.Text = "部位追加";
            // 
            // AddPartsFalse
            // 
            this.AddPartsFalse.AutoSize = true;
            this.AddPartsFalse.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPartsFalse.Location = new System.Drawing.Point(91, 19);
            this.AddPartsFalse.Name = "AddPartsFalse";
            this.AddPartsFalse.Size = new System.Drawing.Size(38, 22);
            this.AddPartsFalse.TabIndex = 1;
            this.AddPartsFalse.TabStop = true;
            this.AddPartsFalse.Text = "無";
            this.AddPartsFalse.UseVisualStyleBackColor = true;
            // 
            // AddPartsTrue
            // 
            this.AddPartsTrue.AutoSize = true;
            this.AddPartsTrue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPartsTrue.Location = new System.Drawing.Point(22, 19);
            this.AddPartsTrue.Name = "AddPartsTrue";
            this.AddPartsTrue.Size = new System.Drawing.Size(38, 22);
            this.AddPartsTrue.TabIndex = 0;
            this.AddPartsTrue.TabStop = true;
            this.AddPartsTrue.Text = "有";
            this.AddPartsTrue.UseVisualStyleBackColor = true;
            // 
            // FaxAndMail
            // 
            this.FaxAndMail.Controls.Add(this.FaxAndMailFalse);
            this.FaxAndMail.Controls.Add(this.FaxAndMailTrue);
            this.FaxAndMail.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FaxAndMail.Location = new System.Drawing.Point(52, 567);
            this.FaxAndMail.Name = "FaxAndMail";
            this.FaxAndMail.Size = new System.Drawing.Size(153, 52);
            this.FaxAndMail.TabIndex = 10;
            this.FaxAndMail.TabStop = false;
            this.FaxAndMail.Text = "Fax･Mail";
            // 
            // FaxAndMailFalse
            // 
            this.FaxAndMailFalse.AutoSize = true;
            this.FaxAndMailFalse.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FaxAndMailFalse.Location = new System.Drawing.Point(91, 19);
            this.FaxAndMailFalse.Name = "FaxAndMailFalse";
            this.FaxAndMailFalse.Size = new System.Drawing.Size(38, 22);
            this.FaxAndMailFalse.TabIndex = 1;
            this.FaxAndMailFalse.TabStop = true;
            this.FaxAndMailFalse.Text = "無";
            this.FaxAndMailFalse.UseVisualStyleBackColor = true;
            // 
            // FaxAndMailTrue
            // 
            this.FaxAndMailTrue.AutoSize = true;
            this.FaxAndMailTrue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FaxAndMailTrue.Location = new System.Drawing.Point(22, 19);
            this.FaxAndMailTrue.Name = "FaxAndMailTrue";
            this.FaxAndMailTrue.Size = new System.Drawing.Size(38, 22);
            this.FaxAndMailTrue.TabIndex = 0;
            this.FaxAndMailTrue.TabStop = true;
            this.FaxAndMailTrue.Text = "有";
            this.FaxAndMailTrue.UseVisualStyleBackColor = true;
            // 
            // Claim
            // 
            this.Claim.Controls.Add(this.ClaimFalse);
            this.Claim.Controls.Add(this.ClaimTrue);
            this.Claim.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Claim.Location = new System.Drawing.Point(246, 674);
            this.Claim.Name = "Claim";
            this.Claim.Size = new System.Drawing.Size(183, 52);
            this.Claim.TabIndex = 11;
            this.Claim.TabStop = false;
            this.Claim.Text = "請求";
            // 
            // ClaimFalse
            // 
            this.ClaimFalse.AutoSize = true;
            this.ClaimFalse.Font = new System.Drawing.Font("メイリオ", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClaimFalse.Location = new System.Drawing.Point(103, 18);
            this.ClaimFalse.Name = "ClaimFalse";
            this.ClaimFalse.Size = new System.Drawing.Size(43, 27);
            this.ClaimFalse.TabIndex = 1;
            this.ClaimFalse.TabStop = true;
            this.ClaimFalse.Text = "無";
            this.ClaimFalse.UseVisualStyleBackColor = true;
            // 
            // ClaimTrue
            // 
            this.ClaimTrue.AutoSize = true;
            this.ClaimTrue.Font = new System.Drawing.Font("メイリオ", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClaimTrue.Location = new System.Drawing.Point(35, 18);
            this.ClaimTrue.Name = "ClaimTrue";
            this.ClaimTrue.Size = new System.Drawing.Size(43, 27);
            this.ClaimTrue.TabIndex = 0;
            this.ClaimTrue.TabStop = true;
            this.ClaimTrue.Text = "有";
            this.ClaimTrue.UseVisualStyleBackColor = true;
            // 
            // Emergency
            // 
            this.Emergency.Controls.Add(this.EmegencyFalse);
            this.Emergency.Controls.Add(this.EmegencyTrue);
            this.Emergency.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Emergency.Location = new System.Drawing.Point(52, 487);
            this.Emergency.Name = "Emergency";
            this.Emergency.Size = new System.Drawing.Size(153, 52);
            this.Emergency.TabIndex = 12;
            this.Emergency.TabStop = false;
            this.Emergency.Text = "緊急";
            // 
            // EmegencyFalse
            // 
            this.EmegencyFalse.AutoSize = true;
            this.EmegencyFalse.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.EmegencyFalse.Location = new System.Drawing.Point(91, 19);
            this.EmegencyFalse.Name = "EmegencyFalse";
            this.EmegencyFalse.Size = new System.Drawing.Size(50, 22);
            this.EmegencyFalse.TabIndex = 1;
            this.EmegencyFalse.TabStop = true;
            this.EmegencyFalse.Text = "通常";
            this.EmegencyFalse.UseVisualStyleBackColor = true;
            // 
            // EmegencyTrue
            // 
            this.EmegencyTrue.AutoSize = true;
            this.EmegencyTrue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.EmegencyTrue.Location = new System.Drawing.Point(22, 19);
            this.EmegencyTrue.Name = "EmegencyTrue";
            this.EmegencyTrue.Size = new System.Drawing.Size(50, 22);
            this.EmegencyTrue.TabIndex = 0;
            this.EmegencyTrue.TabStop = true;
            this.EmegencyTrue.Text = "緊急";
            this.EmegencyTrue.UseVisualStyleBackColor = true;
            // 
            // AddMammaryGland
            // 
            this.AddMammaryGland.Controls.Add(this.AddMammaryGlandFalse);
            this.AddMammaryGland.Controls.Add(this.AddMammaryGlandTrue);
            this.AddMammaryGland.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddMammaryGland.Location = new System.Drawing.Point(241, 487);
            this.AddMammaryGland.Name = "AddMammaryGland";
            this.AddMammaryGland.Size = new System.Drawing.Size(153, 52);
            this.AddMammaryGland.TabIndex = 13;
            this.AddMammaryGland.TabStop = false;
            this.AddMammaryGland.Text = "乳腺加算";
            // 
            // AddMammaryGlandFalse
            // 
            this.AddMammaryGlandFalse.AutoSize = true;
            this.AddMammaryGlandFalse.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddMammaryGlandFalse.Location = new System.Drawing.Point(91, 19);
            this.AddMammaryGlandFalse.Name = "AddMammaryGlandFalse";
            this.AddMammaryGlandFalse.Size = new System.Drawing.Size(38, 22);
            this.AddMammaryGlandFalse.TabIndex = 1;
            this.AddMammaryGlandFalse.TabStop = true;
            this.AddMammaryGlandFalse.Text = "無";
            this.AddMammaryGlandFalse.UseVisualStyleBackColor = true;
            // 
            // AddMammaryGlandTrue
            // 
            this.AddMammaryGlandTrue.AutoSize = true;
            this.AddMammaryGlandTrue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddMammaryGlandTrue.Location = new System.Drawing.Point(22, 19);
            this.AddMammaryGlandTrue.Name = "AddMammaryGlandTrue";
            this.AddMammaryGlandTrue.Size = new System.Drawing.Size(38, 22);
            this.AddMammaryGlandTrue.TabIndex = 0;
            this.AddMammaryGlandTrue.TabStop = true;
            this.AddMammaryGlandTrue.Text = "有";
            this.AddMammaryGlandTrue.UseVisualStyleBackColor = true;
            // 
            // AddPic
            // 
            this.AddPic.Controls.Add(this.AddPicFalse);
            this.AddPic.Controls.Add(this.AddPicTrue);
            this.AddPic.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPic.Location = new System.Drawing.Point(241, 407);
            this.AddPic.Name = "AddPic";
            this.AddPic.Size = new System.Drawing.Size(153, 52);
            this.AddPic.TabIndex = 15;
            this.AddPic.TabStop = false;
            this.AddPic.Text = "画像加算";
            // 
            // AddPicFalse
            // 
            this.AddPicFalse.AutoSize = true;
            this.AddPicFalse.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPicFalse.Location = new System.Drawing.Point(91, 19);
            this.AddPicFalse.Name = "AddPicFalse";
            this.AddPicFalse.Size = new System.Drawing.Size(38, 22);
            this.AddPicFalse.TabIndex = 1;
            this.AddPicFalse.TabStop = true;
            this.AddPicFalse.Text = "無";
            this.AddPicFalse.UseVisualStyleBackColor = true;
            // 
            // AddPicTrue
            // 
            this.AddPicTrue.AutoSize = true;
            this.AddPicTrue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPicTrue.Location = new System.Drawing.Point(22, 19);
            this.AddPicTrue.Name = "AddPicTrue";
            this.AddPicTrue.Size = new System.Drawing.Size(38, 22);
            this.AddPicTrue.TabIndex = 0;
            this.AddPicTrue.TabStop = true;
            this.AddPicTrue.Text = "有";
            this.AddPicTrue.UseVisualStyleBackColor = true;
            // 
            // Payment
            // 
            this.Payment.Controls.Add(this.PaymentFalse);
            this.Payment.Controls.Add(this.PaymentTrue);
            this.Payment.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Payment.Location = new System.Drawing.Point(435, 674);
            this.Payment.Name = "Payment";
            this.Payment.Size = new System.Drawing.Size(183, 52);
            this.Payment.TabIndex = 14;
            this.Payment.TabStop = false;
            this.Payment.Text = "支払い";
            // 
            // PaymentFalse
            // 
            this.PaymentFalse.AutoSize = true;
            this.PaymentFalse.Font = new System.Drawing.Font("メイリオ", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PaymentFalse.Location = new System.Drawing.Point(95, 18);
            this.PaymentFalse.Name = "PaymentFalse";
            this.PaymentFalse.Size = new System.Drawing.Size(43, 27);
            this.PaymentFalse.TabIndex = 1;
            this.PaymentFalse.TabStop = true;
            this.PaymentFalse.Text = "無";
            this.PaymentFalse.UseVisualStyleBackColor = true;
            // 
            // PaymentTrue
            // 
            this.PaymentTrue.AutoSize = true;
            this.PaymentTrue.Font = new System.Drawing.Font("メイリオ", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PaymentTrue.Location = new System.Drawing.Point(31, 18);
            this.PaymentTrue.Name = "PaymentTrue";
            this.PaymentTrue.Size = new System.Drawing.Size(43, 27);
            this.PaymentTrue.TabIndex = 0;
            this.PaymentTrue.TabStop = true;
            this.PaymentTrue.Text = "有";
            this.PaymentTrue.UseVisualStyleBackColor = true;
            // 
            // NumberOfImager
            // 
            this.NumberOfImager.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.NumberOfImager.Location = new System.Drawing.Point(557, 274);
            this.NumberOfImager.MaxLength = 20;
            this.NumberOfImager.Name = "NumberOfImager";
            this.NumberOfImager.Size = new System.Drawing.Size(106, 31);
            this.NumberOfImager.TabIndex = 19;
            this.NumberOfImager.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberOfImager_KeyPress);
            // 
            // Lbl_Office
            // 
            this.Lbl_Office.AutoSize = true;
            this.Lbl_Office.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Office.Location = new System.Drawing.Point(48, 37);
            this.Lbl_Office.Name = "Lbl_Office";
            this.Lbl_Office.Size = new System.Drawing.Size(58, 24);
            this.Lbl_Office.TabIndex = 31;
            this.Lbl_Office.Text = "事業所";
            // 
            // Lbl_PatientId
            // 
            this.Lbl_PatientId.AutoSize = true;
            this.Lbl_PatientId.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_PatientId.Location = new System.Drawing.Point(46, 97);
            this.Lbl_PatientId.Name = "Lbl_PatientId";
            this.Lbl_PatientId.Size = new System.Drawing.Size(60, 24);
            this.Lbl_PatientId.TabIndex = 32;
            this.Lbl_PatientId.Text = "患者ID";
            // 
            // Lbl_PatientName
            // 
            this.Lbl_PatientName.AutoSize = true;
            this.Lbl_PatientName.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_PatientName.Location = new System.Drawing.Point(48, 157);
            this.Lbl_PatientName.Name = "Lbl_PatientName";
            this.Lbl_PatientName.Size = new System.Drawing.Size(58, 24);
            this.Lbl_PatientName.TabIndex = 33;
            this.Lbl_PatientName.Text = "患者名";
            // 
            // Lbl_InspectionDate
            // 
            this.Lbl_InspectionDate.AutoSize = true;
            this.Lbl_InspectionDate.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_InspectionDate.Location = new System.Drawing.Point(48, 337);
            this.Lbl_InspectionDate.Name = "Lbl_InspectionDate";
            this.Lbl_InspectionDate.Size = new System.Drawing.Size(58, 24);
            this.Lbl_InspectionDate.TabIndex = 34;
            this.Lbl_InspectionDate.Text = "検査日";
            // 
            // Lbl_Modality
            // 
            this.Lbl_Modality.AutoSize = true;
            this.Lbl_Modality.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Modality.Location = new System.Drawing.Point(16, 217);
            this.Lbl_Modality.Name = "Lbl_Modality";
            this.Lbl_Modality.Size = new System.Drawing.Size(90, 24);
            this.Lbl_Modality.TabIndex = 35;
            this.Lbl_Modality.Text = "モダリティ";
            // 
            // Lbl_Parts
            // 
            this.Lbl_Parts.AutoSize = true;
            this.Lbl_Parts.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Parts.Location = new System.Drawing.Point(32, 277);
            this.Lbl_Parts.Name = "Lbl_Parts";
            this.Lbl_Parts.Size = new System.Drawing.Size(74, 24);
            this.Lbl_Parts.TabIndex = 36;
            this.Lbl_Parts.Text = "検査部位";
            // 
            // Lbl_RequestDoctor
            // 
            this.Lbl_RequestDoctor.AutoSize = true;
            this.Lbl_RequestDoctor.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_RequestDoctor.Location = new System.Drawing.Point(493, 97);
            this.Lbl_RequestDoctor.Name = "Lbl_RequestDoctor";
            this.Lbl_RequestDoctor.Size = new System.Drawing.Size(58, 24);
            this.Lbl_RequestDoctor.TabIndex = 37;
            this.Lbl_RequestDoctor.Text = "依頼医";
            // 
            // Lbl_WorkDoctor
            // 
            this.Lbl_WorkDoctor.AutoSize = true;
            this.Lbl_WorkDoctor.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_WorkDoctor.Location = new System.Drawing.Point(493, 157);
            this.Lbl_WorkDoctor.Name = "Lbl_WorkDoctor";
            this.Lbl_WorkDoctor.Size = new System.Drawing.Size(58, 24);
            this.Lbl_WorkDoctor.TabIndex = 38;
            this.Lbl_WorkDoctor.Text = "読影医";
            // 
            // Lbl_InterpretationDate
            // 
            this.Lbl_InterpretationDate.AutoSize = true;
            this.Lbl_InterpretationDate.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_InterpretationDate.Location = new System.Drawing.Point(493, 217);
            this.Lbl_InterpretationDate.Name = "Lbl_InterpretationDate";
            this.Lbl_InterpretationDate.Size = new System.Drawing.Size(58, 24);
            this.Lbl_InterpretationDate.TabIndex = 39;
            this.Lbl_InterpretationDate.Text = "読影日";
            // 
            // Lbl_RequestContent
            // 
            this.Lbl_RequestContent.AutoSize = true;
            this.Lbl_RequestContent.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_RequestContent.Location = new System.Drawing.Point(477, 337);
            this.Lbl_RequestContent.Name = "Lbl_RequestContent";
            this.Lbl_RequestContent.Size = new System.Drawing.Size(74, 24);
            this.Lbl_RequestContent.TabIndex = 40;
            this.Lbl_RequestContent.Text = "依頼内容";
            // 
            // Lbl_Acceptedonly
            // 
            this.Lbl_Acceptedonly.AutoSize = true;
            this.Lbl_Acceptedonly.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Acceptedonly.Location = new System.Drawing.Point(477, 435);
            this.Lbl_Acceptedonly.Name = "Lbl_Acceptedonly";
            this.Lbl_Acceptedonly.Size = new System.Drawing.Size(74, 24);
            this.Lbl_Acceptedonly.TabIndex = 41;
            this.Lbl_Acceptedonly.Text = "受付専用";
            // 
            // Lbl_Contact
            // 
            this.Lbl_Contact.AutoSize = true;
            this.Lbl_Contact.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Contact.Location = new System.Drawing.Point(477, 381);
            this.Lbl_Contact.Name = "Lbl_Contact";
            this.Lbl_Contact.Size = new System.Drawing.Size(74, 24);
            this.Lbl_Contact.TabIndex = 42;
            this.Lbl_Contact.Text = "連絡事項";
            // 
            // Lbl_NumberOfImager
            // 
            this.Lbl_NumberOfImager.AutoSize = true;
            this.Lbl_NumberOfImager.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_NumberOfImager.Location = new System.Drawing.Point(477, 277);
            this.Lbl_NumberOfImager.Name = "Lbl_NumberOfImager";
            this.Lbl_NumberOfImager.Size = new System.Drawing.Size(74, 24);
            this.Lbl_NumberOfImager.TabIndex = 43;
            this.Lbl_NumberOfImager.Text = "画像枚数";
            // 
            // Lbl_Memo
            // 
            this.Lbl_Memo.AutoSize = true;
            this.Lbl_Memo.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Lbl_Memo.Location = new System.Drawing.Point(509, 482);
            this.Lbl_Memo.Name = "Lbl_Memo";
            this.Lbl_Memo.Size = new System.Drawing.Size(42, 24);
            this.Lbl_Memo.TabIndex = 45;
            this.Lbl_Memo.Text = "メモ";
            // 
            // Form_DetailEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1066, 736);
            this.Controls.Add(this.Lbl_Memo);
            this.Controls.Add(this.Lbl_NumberOfImager);
            this.Controls.Add(this.Lbl_Contact);
            this.Controls.Add(this.Lbl_Acceptedonly);
            this.Controls.Add(this.Lbl_RequestContent);
            this.Controls.Add(this.Lbl_InterpretationDate);
            this.Controls.Add(this.Lbl_WorkDoctor);
            this.Controls.Add(this.Lbl_RequestDoctor);
            this.Controls.Add(this.Lbl_Parts);
            this.Controls.Add(this.Lbl_Modality);
            this.Controls.Add(this.Lbl_InspectionDate);
            this.Controls.Add(this.Lbl_PatientName);
            this.Controls.Add(this.Lbl_PatientId);
            this.Controls.Add(this.Lbl_Office);
            this.Controls.Add(this.NumberOfImager);
            this.Controls.Add(this.AddPic);
            this.Controls.Add(this.AddMammaryGland);
            this.Controls.Add(this.Payment);
            this.Controls.Add(this.Emergency);
            this.Controls.Add(this.Claim);
            this.Controls.Add(this.FaxAndMail);
            this.Controls.Add(this.InterpretationDate);
            this.Controls.Add(this.WorkDoctor);
            this.Controls.Add(this.Modality);
            this.Controls.Add(this.InspectionDate);
            this.Controls.Add(this.Btn_Edit);
            this.Controls.Add(this.Btn_ReSet);
            this.Controls.Add(this.Btn_Update);
            this.Controls.Add(this.Memo);
            this.Controls.Add(this.Contact);
            this.Controls.Add(this.Acceptedonly);
            this.Controls.Add(this.RequestContent);
            this.Controls.Add(this.RequestDoctor);
            this.Controls.Add(this.Parts);
            this.Controls.Add(this.PatientName);
            this.Controls.Add(this.PatientId);
            this.Controls.Add(this.Office);
            this.Controls.Add(this.AddParts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form_DetailEdit";
            this.Text = "編集画面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_DetailEdit_FormClosing);
            this.Shown += new System.EventHandler(this.Form_DetailEdit_Shown);
            this.AddParts.ResumeLayout(false);
            this.AddParts.PerformLayout();
            this.FaxAndMail.ResumeLayout(false);
            this.FaxAndMail.PerformLayout();
            this.Claim.ResumeLayout(false);
            this.Claim.PerformLayout();
            this.Emergency.ResumeLayout(false);
            this.Emergency.PerformLayout();
            this.AddMammaryGland.ResumeLayout(false);
            this.AddMammaryGland.PerformLayout();
            this.AddPic.ResumeLayout(false);
            this.AddPic.PerformLayout();
            this.Payment.ResumeLayout(false);
            this.Payment.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Office;
        private System.Windows.Forms.TextBox PatientId;
        private System.Windows.Forms.TextBox PatientName;
        private System.Windows.Forms.TextBox Parts;
        private System.Windows.Forms.TextBox RequestDoctor;
        private System.Windows.Forms.TextBox RequestContent;
        private System.Windows.Forms.TextBox Acceptedonly;
        private System.Windows.Forms.TextBox Contact;
        private System.Windows.Forms.TextBox Memo;
        private System.Windows.Forms.Button Btn_Update;
        private System.Windows.Forms.Button Btn_ReSet;
        private System.Windows.Forms.Button Btn_Edit;
        private System.Windows.Forms.DateTimePicker InspectionDate;
        private System.Windows.Forms.ComboBox Modality;
        private System.Windows.Forms.ComboBox WorkDoctor;
        private System.Windows.Forms.DateTimePicker InterpretationDate;
        private System.Windows.Forms.GroupBox AddParts;
        private System.Windows.Forms.GroupBox FaxAndMail;
        private System.Windows.Forms.GroupBox Claim;
        private System.Windows.Forms.GroupBox Emergency;
        private System.Windows.Forms.GroupBox AddMammaryGland;
        private System.Windows.Forms.GroupBox AddPic;
        private System.Windows.Forms.GroupBox Payment;
        private System.Windows.Forms.RadioButton AddPartsFalse;
        private System.Windows.Forms.RadioButton AddPartsTrue;
        private System.Windows.Forms.RadioButton FaxAndMailFalse;
        private System.Windows.Forms.RadioButton FaxAndMailTrue;
        private System.Windows.Forms.RadioButton ClaimFalse;
        private System.Windows.Forms.RadioButton ClaimTrue;
        private System.Windows.Forms.RadioButton EmegencyFalse;
        private System.Windows.Forms.RadioButton EmegencyTrue;
        private System.Windows.Forms.RadioButton AddMammaryGlandFalse;
        private System.Windows.Forms.RadioButton AddMammaryGlandTrue;
        private System.Windows.Forms.RadioButton AddPicFalse;
        private System.Windows.Forms.RadioButton AddPicTrue;
        private System.Windows.Forms.RadioButton PaymentFalse;
        private System.Windows.Forms.RadioButton PaymentTrue;
        private System.Windows.Forms.TextBox NumberOfImager;
        private System.Windows.Forms.Label Lbl_Office;
        private System.Windows.Forms.Label Lbl_PatientId;
        private System.Windows.Forms.Label Lbl_PatientName;
        private System.Windows.Forms.Label Lbl_InspectionDate;
        private System.Windows.Forms.Label Lbl_Modality;
        private System.Windows.Forms.Label Lbl_Parts;
        private System.Windows.Forms.Label Lbl_RequestDoctor;
        private System.Windows.Forms.Label Lbl_WorkDoctor;
        private System.Windows.Forms.Label Lbl_InterpretationDate;
        private System.Windows.Forms.Label Lbl_RequestContent;
        private System.Windows.Forms.Label Lbl_Acceptedonly;
        private System.Windows.Forms.Label Lbl_Contact;
        private System.Windows.Forms.Label Lbl_NumberOfImager;
        private System.Windows.Forms.Label Lbl_Memo;
    }
}