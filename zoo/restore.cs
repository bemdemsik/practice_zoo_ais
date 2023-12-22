using System;
using System.Windows.Forms;
using System.IO;

namespace Зоопарк
{
    public partial class restore : Form
    {
        string fio = "";
        string role = "";
        public restore(string role1, string fio1, string[] qwe)
        {
            role = role1;
            fio = fio1;
            InitializeComponent();
            for (int i = 0; i < qwe.Length; i++ )
                comboBox1.Items.Add(Path.GetFileName(qwe[i]));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "")
            {
                MessageBox.Show("Вы не выбрали файл","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Table.Restore(comboBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
    }
}
