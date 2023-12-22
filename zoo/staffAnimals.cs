using System;
using System.Drawing;
using System.Windows.Forms;

namespace Зоопарк
{
    public partial class staffAnimals : Form
    {
        string request = "SELECT id, (select fio from users where id=iduser) as 'Сотрудник', (select name from animals where id=idanimal) as 'Назначенный род' from staff";
        string delete = "";
        string insert = "";
        string change = "";
        string role = "";
        string fio = "";
        string[] Item;
        int pK = 0;
        public staffAnimals(string role1, string fio1)
        {
            role = role1;
            fio = fio1;
            InitializeComponent();
        }

        private void load()
        {
            dGV1.DataSource = Table.Load(request);
            label5.Text = dGV1.Rows.Count.ToString();
            dGV1.Columns["id"].Visible = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Visible = false;
            button2.Enabled = true;
        }
        private void Add()
        {
            Item = Completion.Item("select fio from users where role != 'Администратор' and (select count(*) from staff where iduser=users.id) < 2", 0, 0);
            for (int i = 0; i < Item.Length; i++)
                comboBox1.Items.Add(Item[i]);
            Item = Completion.Item("select name from animals where id not in (select idanimal from staff)", 0, 0);
            for (int i = 0; i < Item.Length; i++)
                comboBox2.Items.Add(Item[i]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Check();
            insert = "insert into staff(iduser,idanimal) values((select id from users where fio='" + comboBox1.Text + "'),(select id from animals where name='" + comboBox2.Text + "'))";
            if (Table.Request(insert))
            {
                MessageBox.Show("Вы назначили сотруднику " + comboBox2.Text + " род '" + comboBox2.Text + "'", "Круто", MessageBoxButtons.OK);
                Clear();
                load();
                Add();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Check();
            change = "update staff set iduser=(select id from users where fio='" + comboBox1.Text + "'),idanimal=(select id from animals where name='" + comboBox2.Text + "')" + " where id =" + pK;
            if (Table.Request(change))
            {
                MessageBox.Show("Вы изменили запись", "Круто", MessageBoxButtons.OK);
                load();
                Clear();
                Add();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var kn = MessageBox.Show("Вы уверены что хотите удалить эту запись?", "", MessageBoxButtons.YesNo);
            if (kn == DialogResult.Yes)
            {
                delete = "DELETE FROM staff where id =" + pK;
                if (Table.Request(delete))
                {
                    MessageBox.Show("Вы удалили запись", "Круто", MessageBoxButtons.OK);
                    load();
                    Clear();
                    Add();
                }
            }
        }
        int rI = 0;
        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rI = e.RowIndex;
            try
            {
                pK = Convert.ToInt32(dGV1.Rows[rI].Cells["id"].Value.ToString());
            }
            catch
            {
                return;
            }
            Clear();
            Add();
            if (!comboBox1.Items.Contains(dGV1.Rows[rI].Cells["Назначенный род"].Value.ToString()))
            {
                comboBox1.Items.Add(dGV1.Rows[rI].Cells["Сотрудник"].Value.ToString());
                comboBox1.Text = dGV1.Rows[rI].Cells["Сотрудник"].Value.ToString();
            }
            if (!comboBox2.Items.Contains(dGV1.Rows[rI].Cells["Назначенный род"].Value.ToString()))
            {
                comboBox2.Items.Add(dGV1.Rows[rI].Cells["Назначенный род"].Value.ToString());
                comboBox2.Text = dGV1.Rows[rI].Cells["Назначенный род"].Value.ToString();
            }
            button5.Visible = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
        }
        private void Check()
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Вы не выбрали сотрудника", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            if (comboBox2.Text == "")
            {
                MessageBox.Show("Вы не выбрали род животного", "Ошибка", MessageBoxButtons.OK);
                return;
            }
        }
        private void Clear()
        {
            comboBox1.Items.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("Все сотрудники уже заняты, ищите новых", "Внимание", MessageBoxButtons.OK);
            }
        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count == 0)
            {
                MessageBox.Show("Для каждого рода уже назначен сотрудник", "Внимание", MessageBoxButtons.OK);
            }
        }
        private void staffAnimals_Load(object sender, EventArgs e)
        {
            load();
            Add();
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            button5.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clear();
            Add();
            button5.Visible = false;
            button2.Enabled = true;
        }
    }
}
