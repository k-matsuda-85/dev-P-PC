using Agg_Serv;
using Agg_Serv.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AggregateTool
{
    public partial class Form_Menu : Form
    {
        //ログインフォームを再度呼び出す為、保持しておく変数
        public Form_Login form_login= null;
        //閉じるボタンから閉じられたかを判定するbool
        private bool isClose = true;

        public List<Config> SystemConfig = new List<Config>();

        public Form_Menu()
        {
            InitializeComponent();

            LoadMaster();
        }

        public void LoadMaster()
        {
            IF_Service service = new IF_Service();

            SystemConfig = service.GetSystemConfig().ToList();
        }

        //ここから================================================イベント=========================================================

        /// <summary>
        /// 編集画面表示ボタン
        /// </summary>
        private void Btn_OpenEditForm_Click(object sender, EventArgs e)
        {
            //編集画面
            Form_Edit fe = new Form_Edit();
            //戻る際Show等の処理を行う為にMenuを渡す
            fe.form_menu = this;
            //メニューを隠す
            this.Visible = false;
            //編集を開く
            fe.Show();
        }

        /// <summary>
        /// 集計結果出力画面表示ボタン
        /// </summary>
        private void Btn_OpenAggForm_Click(object sender, EventArgs e)
        {
            Form_OutPutAggregate form_outputaggregate =
            new Form_OutPutAggregate();
            form_outputaggregate.form_menu = this;
            this.Visible = false;
            form_outputaggregate.Show();
        }

        /// <summary>
        /// ログアウトボタンを押した際、
        /// </summary>
        private void Btn_CloseMenu_Click(object sender, EventArgs e)
        {
            //閉じるボタンフラグをfalseにする。
            isClose = false;
            //閉じる
            this.Close();
        }
        /// <summary>
        /// メニュー画面を閉じた際の処理の分岐
        /// </summary>
        private void Form_Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //閉じるボタンがtrueのままの場合
            if(isClose)
            {
                if (GetResultClosing("アプリケーションを終了してよろしいですか？"))
                    //ログインフォームもそのまま閉じる
                    form_login.Close();
                else
                    e.Cancel = true;
            }
            else
            {
                if(GetResultClosing("ログアウトしてよろしいですか？"))
                    //表示する
                    form_login.Visible = true;
                else
                    e.Cancel = true;
            }
        }
        /// <summary>
        /// フォーカスを消す
        /// </summary>
        private void Form_Menu_Shown(object sender, EventArgs e)
        {
            //フォーカスを消す
            ActiveControl = null;
        }

        //ここまで================================================イベント=============================================================


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

        private void Btn_OpenSettingForm_Click(object sender, EventArgs e)
        {
            Form_Setting form_setting = new Form_Setting();
            form_setting.form_menu = this;
            form_setting.UserRet = form_login.UserRet;
            this.Visible = false;
            form_setting.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form_Data form_data = new Form_Data();
            form_data.form_menu = this;
            this.Visible = false;
            form_data.Show();
        }
    }
}
