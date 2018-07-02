using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Agg_Serv;
using Agg_Serv.Class;

namespace AggregateTool
{
    public partial class Form_Login : Form
    {
        //入力文字列文字列を確認する際の固定の正規表現
        private Regex regex = new Regex("^[0-9a-zA-Z]+$");

        public User UserRet = new User();

        public Form_Login()
        {
            InitializeComponent();
        }

        //ここから================================================イベント=========================================================

        /// <summary>
        /// ログインボタン
        /// </summary>
        private void Btn_Login_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txtbox_LoginId.Text))
            {
                MessageBox.Show("ログインIDを入力してください。");
                Txtbox_LoginId.Focus();
                return;
            }
            if (String.IsNullOrEmpty(Txtbox_Password.Text))
            {
                MessageBox.Show("パスワードを入力してください。");
                Txtbox_Password.Focus();
                return;
            }

            IF_Service service = new IF_Service();
            User user = new User();

            user = service.Login(Txtbox_LoginId.Text, Txtbox_Password.Text);

            if(user == null || String.IsNullOrEmpty(user.Cd))
            {
                MessageBox.Show("ID、パスワードを確認してください。");
                Txtbox_Password.Select();
                return;
            }

            UserRet = user;

            Form_Menu fm = new Form_Menu();
            fm.form_login = this;
            this.Visible = false;
            ClearTextBoxText();
            fm.Show();
        }

        private void Form_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!GetTextBoxValue())
            {
                if (!GetResultClosing("アプリケーションを終了してよろしいですか？"))
                    e.Cancel = true;
            }
        }


        //ここから------------------------------------------------入力制限-------------------------------------------------------------
        /// <summary>
        /// エンターキーの入力を取得してフォーカスの移動を行う
        /// </summary>
        private void Form_LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.SelectNextControl(this.ActiveControl, !e.Shift, true, true, true);
            }
        }

        private void Txtbox_LoginId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(regex.IsMatch(e.KeyChar.ToString())) && !(e.KeyChar == '\b'))
                e.Handled = true;
        }

        private void Txtbox_Password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(regex.IsMatch(e.KeyChar.ToString())) && !(e.KeyChar == '\b'))
                e.Handled = true;
        }

        private void Txtbox_LoginId_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.Delete) && !(e.KeyCode == Keys.ShiftKey) && 
                !(e.KeyCode == Keys.Right) && !(e.KeyCode == Keys.Left))
                e.Handled = true;
        }

        private void Txtbox_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.Delete) && !(e.KeyCode == Keys.ShiftKey) && 
                !(e.KeyCode == Keys.Right) && !(e.KeyCode == Keys.Left))
                e.Handled = true;
        }
        //ここまで------------------------------------------------入力制限-------------------------------------------------------------

        //ここまで================================================イベント=============================================================

        //ここから================================================内部メソッド=========================================================
        
        /// <summary>
        /// テキストボックスの入力値を消す
        /// </summary>
        private void ClearTextBoxText()
        {
            foreach (Control control in this.Controls)
            {
                if (control is TextBoxBase)
                    control.Text = string.Empty;
            }
        }
        private bool GetTextBoxValue()
        {
            bool result = true;
            foreach(Control control in this.Controls)
            {
                if(control is TextBoxBase)
                {
                    if (!string.IsNullOrEmpty(((TextBoxBase)control).Text))
                        result = false;
                }
            }
            return result;
        }

        private bool GetResultClosing(string msg)
        {
            bool decision = false;
            DialogResult result = MessageBox.Show(msg, "注意", MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                decision = true;
            }
            return decision;
        }
        //ここから================================================内部メソッド=========================================================
    }
}
