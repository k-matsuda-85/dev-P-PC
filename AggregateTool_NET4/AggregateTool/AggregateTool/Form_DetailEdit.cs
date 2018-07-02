using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AggregateTool
{
    public partial class Form_DetailEdit : Form
    {
        public Form_Edit form_edit = new Form_Edit();
        public Dictionary<string, string> rowsValue = new Dictionary<string, string>();
        private Regex regex = new Regex("^[0-9]+$");

        public int rowIndex;

        public Form_DetailEdit()
        {
            InitializeComponent();
            TestDoctor();
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            if (GetResultClosing("更新を行ってよろしいですか？"))
            {
                form_edit.SetNewRowsValue(rowIndex, GetControlValues());
                form_edit.confirmationExit = false;
                rowsValue = GetControlValues();
                this.Close();
            }
        }

        private void Btn_ReSet_Click(object sender, EventArgs e)
        {
            if (!CheckChangedValue(GetControlValues()))
            {
                if (GetResultClosing("編集内容をリセットしてよろしいですか？"))
                    SetControlValue(rowsValue);
            }
        }

        private void Btn_Edit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 入力制限
        /// </summary>
        private void NumberOfImager_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(regex.IsMatch(e.KeyChar.ToString())) && !(e.KeyChar == '\b'))
                e.Handled = true;
        }

        /// <summary>
        /// フォーム終了イベント
        /// </summary>
        private void Form_DetailEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckChangedValue(GetControlValues()))
            {
                if (!GetResultClosing("編集画面を閉じて宜しいですか？"))
                    e.Cancel = true;
                else
                    form_edit.Show();
            }
            else
                form_edit.Show();
        }

        public void SetControlValue(Dictionary<string,string> value)
        {
            foreach(Control control in this.Controls)
            {
                if(control is RadioButton || control is Label || control is Button)
                    continue;
                else if (!value.Keys.Contains(control.Name))
                {
                    control.Enabled = false;
                    continue;
                }
                if (control is TextBoxBase && value.Keys.Contains(control.Name))
                    control.Text = value[control.Name];
                else if (control is GroupBox && value.Keys.Contains(control.Name))
                    SetRadioValue(control, value);
                else if (control is ComboBox && value.Keys.Contains(control.Name))
                    SetComboIndex(control,value);
                else if (control is DateTimePicker && value.Keys.Contains(control.Name))
                    SetDateTime(control, value);
            }
        }

        private void SetComboIndex(Control control, Dictionary<string, string> value)
        {
            int index = ((ComboBox)control).FindStringExact(value[control.Name]);
            ((ComboBox)control).SelectedIndex = index;
        }

        private void SetDateTime(Control control, Dictionary<string, string> value)
        {
            string date = value[control.Name];
            ((DateTimePicker)control).Value = DateTime.Parse(date);
        }

        private void SetRadioValue(Control control, Dictionary<string, string> value)
        {
            if (value[control.Name] == "1")
            {
                foreach (Control radio in ((GroupBox)control).Controls)
                {
                    if (radio.Name.Contains("True"))
                        ((RadioButton)radio).Checked = true;
                }
            }
            else
            {
                foreach (Control radio in ((GroupBox)control).Controls)
                {
                    if (radio.Name.Contains("False"))
                        ((RadioButton)radio).Checked = true;
                }
            }
        }

        private Dictionary<string,string> GetControlValues()
        {
            Dictionary<string, string> newValue = new Dictionary<string, string>();
            foreach(Control control in this.Controls)
            {
                if (control is RadioButton)
                    continue;
                if (control is Label)
                    continue;
                if (control is TextBoxBase)
                    newValue.Add(control.Name,control.Text);
                else if (control is GroupBox)
                    newValue.Add(control.Name, SetRadioValue(control));
                else if (control is ComboBox)
                    newValue.Add(control.Name,((ComboBox)control).SelectedItem.ToString());
                else if (control is DateTimePicker)
                    newValue.Add(control.Name,((DateTimePicker)control).Value.ToShortDateString());
            }
            return newValue;
        }

        /// <summary>
        /// 変更があったか確認を行なう
        /// </summary>
        private bool CheckChangedValue(Dictionary<string, string> newValue)
        {
            bool result = true;
            foreach(string key in newValue.Keys)
            {
                if (!rowsValue.Keys.Contains(key))
                    continue;
                if (!(newValue[key] == rowsValue[key]))
                    return false;
            }
            return result;
        }

        private string SetRadioValue(Control control)
        {
            string result = "0";
            foreach (Control radio in ((GroupBox)control).Controls)
            {
                if (radio.Name.Contains("True") && ((RadioButton)radio).Checked)
                    result = "1";
            }
            return result;
        }

        private void TestDoctor()
        {
            Modality.Items.Add("CT");
            Modality.Items.Add("CR");
            Modality.Items.Add("MR");
            Modality.Items.Add("RI");
            Modality.Items.Add("MG");
            Modality.Items.Add("US");
            Modality.Items.Add("OT");
            WorkDoctor.Items.Add("テスト医師A");
            WorkDoctor.Items.Add("テスト医師B");
            WorkDoctor.Items.Add("テスト医師C");
        }

        private void Form_DetailEdit_Shown(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private bool GetResultClosing(string msg)
        {
            bool decision = false;
            DialogResult result = MessageBox.Show(msg, "注意", MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
                decision = true;
            else if (result == DialogResult.No)
                decision = false;
            return decision;
        }
    }
}
