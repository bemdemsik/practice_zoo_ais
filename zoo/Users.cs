using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace Зоопарк
{
    public partial class Users : Form
    {
        int pK = 0;
        string request = "SELECT id, fio as 'ФИО', login as 'Логин', passwd as 'Пароль', age as 'Возраст', role as 'Роль' FROM users";
        string query = "";
        string sort = "";
        string filt = "";
        string role = "";
        string fio = "";
        public Users(string role1, string fio1)
        {
            fio = fio1;
            role = role1;
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var kn = MessageBox.Show("Вы уверены что хотите удалить эту запись?", "", MessageBoxButtons.YesNo);
            if (kn == DialogResult.Yes)
            {
                query = "DELETE FROM users where id =" + pK;
                if (Table.Request(query))
                {
                    MessageBox.Show("Вы удалили пользователя", "Урааа!!!", MessageBoxButtons.OK);
                    load();
                    Clear();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Check())
                return;
            query = "update users set fio='" + textBox1.Text + "',login='" + textBox2.Text + "',passwd='" + getHash(textBox3.Text.ToString()) + "',age='"+textss2.Text+"',role='" + comboBox1.Text + "' where id=" + pK;
            if(Table.Request(query))
            {
                MessageBox.Show("Вы изменили пользователя", "Урааа!!!", MessageBoxButtons.OK);
                load();
                Clear();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Очистить")
            {
                button2.Text = "Добавить";
                button3.Enabled = false;
                button4.Enabled = false;
                Clear();
                return;
            }
            if (!Check())
                return;
            query = "INSERT INTO users(fio,login,passwd,age,role) VALUES('" + textBox1.Text + "','" + textBox2.Text + "','" + getHash(textBox3.Text.ToString()) + "','"+textss2.Text+"','" + comboBox1.Text + "')";
            if (Table.Request(query))
            {
                MessageBox.Show("Вы добавили пользователя", "Урааа!!!", MessageBoxButtons.OK);
                Clear();
                load();
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }

        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rI = e.RowIndex;
            try
            {
                pK = Convert.ToInt32(dGV1.Rows[rI].Cells["id"].Value.ToString());
            }
            catch
            {
                return;
            }
            textBox1.Text = dGV1.Rows[rI].Cells["ФИО"].Value.ToString();
            textBox2.Text = dGV1.Rows[rI].Cells["Логин"].Value.ToString();
            textss2.Text = dGV1.Rows[rI].Cells["Возраст"].Value.ToString();
            textBox3.Text = dGV1.Rows[rI].Cells["Пароль"].Value.ToString();
            comboBox1.Text = dGV1.Rows[rI].Cells["Роль"].Value.ToString();
            button2.Text = "Очистить";
            button3.Enabled = true;
            button4.Enabled = true;
        }
        private void load()
        {
            dGV1.DataSource = Table.Load(request + filt + sort);
            dGV1.Columns["id"].Visible = false;
            label7.Text = dGV1.Rows.Count.ToString();
            button2.Text = "Добавить";
            button3.Enabled = false;
            button4.Enabled = false;
        }
        private void Users_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            comboBox7.Items.Add("");
            comboBox7.Items.Add("по возрастанию");
            comboBox7.Items.Add("по убыванию");
            comboBox1.Items.Add("Администратор");
            comboBox1.Items.Add("Сотрудник");
            comboBox1.SelectedIndex = -1;
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            load();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string pwd = "qwertyuioplkjhgfdsavcbxnzm1234567890ASWQDTREFGHJUYNBVCZXMKIOLP";
            string[] qwe = pwd.Split();
            StringBuilder pswd = new StringBuilder();
            int i = 0;
            Random rdm = new Random();
            for (i = 0; i < 8; i++)
            {
                pswd.Append(pwd[rdm.Next(pwd.Length)]);
            }
            textBox3.Text = pswd.ToString();
        }
        string getHash(string str)
        {
            var sha2 = SHA256.Create();
            var hbyte = sha2.ComputeHash(Encoding.UTF8.GetBytes(str));

            return BitConverter.ToString(hbyte).Replace("-", "").ToLower();
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[a-zA-Z]|[0-9]|[\b]"))
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[a-zA-Z]|[0-9]|[\b]"))
                e.Handled = true;
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (Symbol == "." && textBox1.Text == "")
            {
                e.Handled = true;
                return;
            }
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[\b]|[^\S\r\n]|[.]"))
                e.Handled = true;
        }

        private void textss2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[0-9]|[\b]"))
                e.Handled = true;
        }
        private bool Check()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Вы не ввели ФИО", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (textss2.Text == "")
            {
                MessageBox.Show("Вы не указали возраст", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Вы не ввели логин", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            else
            {
                string condition = "";
                if (button3.Enabled)
                {
                    condition = " and id <> " + pK.ToString();
                }
                if (Completion.Item("SELECT * FROM users where login ='" + textBox2.Text.ToString() +"'"+ condition, 0, 2).Length > 0)
                {
                    MessageBox.Show("Такой логин уже существует", "Ошибка", MessageBoxButtons.OK);
                    textBox2.Clear();
                    return false;
                }
            }
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Вы не выбрали роль", "Ошибка", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
        private void Clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textss2.Text = "";
            comboBox1.SelectedIndex = -1;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (min.Text == "" && max.Text == "" && rav.Text == "")
                filt = "";
            else
            {
                if (rav.Text != "")
                    filt = " where age=" + rav.Text + " ";
                else if (min.Text != "" && max.Text != "")
                    filt = " where age<" + min.Text + " and age>" + max.Text + " ";
                else if (min.Text != "")
                    filt = " where age<" + min.Text + " ";
                else if (max.Text != "")
                    filt = " where age>" + max.Text + " ";
            }
            if (comboBox7.Text == "")
                sort = "";
            else
            {
                if (comboBox7.Text == "возрастанию")
                    sort = " order by fio";
                else
                    sort = " order by fio desc";
            }
            groupBox1.Visible = false;
            load();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
        }
    }
}
