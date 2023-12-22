using System;
using System.Windows.Forms;
using System.IO;

namespace Зоопарк
{
    public partial class Main : Form
    {
        string role = "";
        string fio = "";
        public Main(string role1, string fio1)
        {
            InitializeComponent();
            role = role1;
            fio = fio1;
            label4.Text = role;
            label5.Text = fio;
            insertNewFeeding.Check();
            Table.Request("update animal_feeding set status='Не покормлен' where date_feeding<CURDATE() and status='Ожидается'");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FeedingSchedule q = new FeedingSchedule(role, fio);
            q.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            Users q = new Users(role, fio);
            q.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            authorization q = new authorization();
            q.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Table.Backup();
            Application.Exit();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            conduction q = new conduction(role, fio);
            q.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Feeding q = new Feeding(role, fio);
            q.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            diet q = new diet(role, fio);
            q.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            order q = new order(role, fio);
            q.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string[] qwe = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sql");
            if(qwe.Length == 0)
            {
                MessageBox.Show("Вы не можите восстановить базу данных, так как не найдено ни одного зарезервированного скрипта для восстановления","",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            this.Hide();
            restore q = new restore(role, fio, qwe);
            q.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            statusFeeding q = new statusFeeding(role, fio);
            q.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Hide();
            staffAnimals q = new staffAnimals(role, fio);
            q.ShowDialog();
        }
    }
}
