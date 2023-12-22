using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Зоопарк
{
    public partial class authorization : Form
    {
        string request = "";
        string psw = "";
        string role = "";
        string fio = "";
        public authorization()
        {
            InitializeComponent();
            string[] Item = Completion.Item("select * from users", 0, 1);
            for (int i = 0; i < Item.Length; i++)
                comboBox1.Items.Add(Item[i]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            entrance();
        }
        private void entrance()
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Вы не выбрали логин", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("Вы не ввели пароль", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            request = "select passwd,role from users where fio = '" + comboBox1.Text + "'";
            string[] Item = Completion.Item(request, 1, 0);
            psw = Item[0];
            role = Item[1];
            fio = comboBox1.Text;

            string pswd = textBox1.Text;
            byte[] psw1 = HashPassword(pswd);
            byte[] psw2 = pas(psw);

            if (psw1.SequenceEqual(psw2))
            {
                if (role == "Администратор")
                {
                    this.Hide();
                    Main qw = new Main(role, fio);
                    qw.ShowDialog();
                }
                else
                {
                    this.Hide();
                    mainStaff qw = new mainStaff(role, fio);
                    qw.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Вы неверно ввели пароль", "Ошибка", MessageBoxButtons.OK);
                textBox1.Clear();
                return;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Отобразить пароль")
            {
                textBox1.UseSystemPasswordChar = false;
                button3.Text = "Скрыть пароль";
            }
            else
            {
                textBox1.UseSystemPasswordChar = true;
                button3.Text = "Отобразить пароль";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        static byte[] HashPassword(string password)
        {
            var sha2 = SHA256.Create();
            var hbyte = sha2.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] data = new UTF8Encoding().GetBytes(BitConverter.ToString(hbyte).Replace("-", "").ToLower());
            byte[] result;
            SHA256 shaM = new SHA256Managed();
            result = shaM.ComputeHash(data);
            return result;
        }
        static byte[] pas(string psw)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(psw);
            byte[] result;
            SHA256 shaM = new SHA256Managed();
            result = shaM.ComputeHash(data);
            return result;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
                entrance();
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[a-zA-Z]|[0-9]|[\b]"))
                e.Handled = true;
        }
    }
}
