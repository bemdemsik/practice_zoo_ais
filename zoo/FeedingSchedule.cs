using System;
using System.Drawing;
using System.Windows.Forms;

namespace Зоопарк
{
    public partial class FeedingSchedule : Form
    {
        string role = "";
        string fio = "";
        int pK = 0;
        string request = "SELECT id, (SELECT name FROM animals where id = idanimals) as 'Животное', morning_feeding as 'Утренние кормление', party_feeding as 'Вечерние кормление' FROM feeding_schedule";
        string delete = "";
        string insert = "";
        string change = "";
        int check = 0;
        public FeedingSchedule(string role1, string fio1)
        {
            InitializeComponent();
            fio = fio1;
            role = role1;
            LoadData();
            dTP1.CustomFormat = "HH:mm";
            dTP2.CustomFormat = "HH:mm";
            button5.Visible = false;
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var kn = MessageBox.Show("Вы уверены что хотите удалить эту запись?", "", MessageBoxButtons.YesNo);
            if (kn == DialogResult.No)
                return;
            delete = "DELETE FROM feeding_schedule where id =" + pK;
            if (Table.Request(delete))
                MessageBox.Show("Вы удалили график для вида " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            check = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dTP1.Value >= dTP2.Value)
            {
                MessageBox.Show("Утренние кормление не может быть позже или такое же как и вечерние", "Ошибка", MessageBoxButtons.YesNo);
                return;
            }
            change = "update feeding_schedule set idanimals=(select id from animals where name='" + comboBox1.Text + "')," + "morning_feeding='" + dTP1.Text + "'," + "party_feeding='" + dTP2.Text + "' where id=" + pK;
            if (Table.Request(change))
                MessageBox.Show("Вы изменили график для вида " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            check = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "")
            {
                MessageBox.Show("Вы не выбрали род животного", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            if (dTP1.Value >= dTP2.Value)
            {
                MessageBox.Show("Утренние кормление не может быть позже или такое же как и вечерние", "Ошибка", MessageBoxButtons.YesNo);
                return;
            }
            insert = "INSERT INTO feeding_schedule(idanimals,morning_feeding,party_feeding) VALUES((select id from animals where name='" + comboBox1.Text + "'),'" + dTP1.Text + "','" + dTP2.Text + "')";
            if (Table.Request(insert))
                MessageBox.Show("Вы добавили график для рода " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            check = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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

            string click = "SELECT id, (SELECT name FROM animals where id = idanimals) as 'Вид животного', morning_feeding as 'Утренние кормление', party_feeding as 'Вечерние кормление' FROM feeding_schedule where id=" + pK;
            comboBox1.Items.Add(Completion.Item("SELECT name FROM animals where id = (select idanimals from feeding_schedule where id = "+pK+")", 0, 0)[0]);
            comboBox1.Enabled = false;
            button5.Visible = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button2.Enabled = false;
            string[] Item = Completion.Item(click, 1, 1);
            comboBox1.Text = Item[0];
            dTP1.Text = Item[1];
            dTP2.Text = Item[2];
            check = 1;
        }

        private void FeedingSchedule_Load(object sender, System.EventArgs e)
        {
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
        }
        private void LoadData()
        {
            dGV1.DataSource = Table.Load(request);
            dGV1.Columns["id"].Visible = false;
            label5.Text = dGV1.Rows.Count.ToString();
            Add();
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Visible = false;
        }
        private void Add()
        {
            comboBox1.Items.Clear();
            comboBox1.SelectedIndex = -1;
            string[] Item = Completion.Item("select name from animals where id not in (select idanimals from feeding_schedule)", 0, 0);
            for (int i = 0; i < Item.Length; i++)
                comboBox1.Items.Add(Item[i]);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Add();
            comboBox1.Enabled = true;
            button5.Visible = false;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
                MessageBox.Show("График кормления составлен для всех животных, вы можите его лишь изменить, нажав на нужную строчку","Внимание",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        private void dTP1_ValueChanged(object sender, EventArgs e)
        {
            if (dTP1.Value > dTP2.Value)
                dTP2.Value = dTP1.Value;
        }

        private void dTP2_ValueChanged(object sender, EventArgs e)
        {
            if (dTP2.Value < dTP1.Value)
                dTP1.Value = dTP2.Value;
        }
    }
}
