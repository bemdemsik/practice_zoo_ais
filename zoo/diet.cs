using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Зоопарк
{
    public partial class diet : Form
    {
        string role = "";
        string fio = "";
        int countPage = 0;
        int countRows = 0;
        int numberPage = 1;
        int q = 0;
        int pK = 0;
        string request = "SELECT id, (SELECT name FROM animals where id = idanimals) as 'Род животного', (SELECT name_food FROM feed where id = idfeed) as 'Корм', days as 'График по дням' FROM diet";
        string delete = "";
        string insert = "";
        string change = "";
        public diet(string role1, string fio1)
        {
            role = role1;
            fio = fio1;
            InitializeComponent();
            button5.Visible = false;
            LoadData();
            UpdateUI();
            string[] Item = Completion.Item("SELECT name FROM animals", 0, 0);
            for (int i = 0; i < Item.Length; i++)
                comboBox1.Items.Add(Item[i]);
            Item = Completion.Item("SELECT name_food FROM feed", 0, 0);
            for (int i = 0; i < Item.Length; i++)
                comboBox2.Items.Add(Item[i]);
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var kn = MessageBox.Show("Вы уверены что хотите удалить эту запись?", "", MessageBoxButtons.YesNo);
            if (kn == DialogResult.No)
                return;
            delete = "DELETE FROM diet where id =" + pK;
            if (Table.Request(delete))
                MessageBox.Show("Вы удалили рацион для рода " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            Clear();
            q = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Check())
                return;
            change = "update diet set idanimals=(select id from animals where name='" + comboBox1.Text + "'),idfeed=(select id from feed where name_food='" + comboBox2.Text + "'),days='" + Days() + "' where id=" + pK;
            if (Table.Request(change))
                MessageBox.Show("Вы изменили рацион для рода " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            Clear();
            q = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Check())
                return;
            insert = "INSERT INTO diet(idanimals,idfeed,days) VALUES((select id from animals where name='" + comboBox1.Text + "'),(select id from feed where name_food='" + comboBox2.Text + "'),'" + Days() + "')";
            if (Table.Request(insert))
                MessageBox.Show("Вы добавили рацион для рода " + comboBox1.Text, "", MessageBoxButtons.OK);
            LoadData();
            Clear();
        }
        private void LoadData()
        {
            try
            {
                countRows = Convert.ToInt32(Completion.Item("select count(*) from diet", 0, 0)[0]);
                dGV1.DataSource = Table.Load(request + " order by (SELECT name FROM animals where id = idanimals) limit " + ((numberPage - 1) * 12) + ",12");
                countPage = Convert.ToInt32(Math.Ceiling(countRows / 12.0));
                label5.Text = countRows.ToString();
                dGV1.Columns["id"].Visible = false;
            }
            catch
            {
                return;
            }
            if (dGV1.Rows.Count == 0)
            {
                nextLabel.Enabled = false;
                backLabel.Enabled = false;
                return;
            }
            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = true;
            button5.Visible = false;
            UpdateUI();
        }
        int y = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!button5.Visible)
            {
                CBox();
                if (!CheckBoxChecked())
                {
                    MessageBox.Show("Для рода '" + comboBox1.Text + "' рацион составлен на все дни", "", MessageBoxButtons.OK);
                    button2.Enabled = false;
                }
                else
                    button2.Enabled = true;
                comboBox1.Enabled = true;
            }
        }

        private bool CheckBoxChecked()
        {
            bool b = true;
            int countDay = 0;
            string daysFeeding = "select days from diet where idanimals = (select id from animals where name = '" + comboBox1.Text + "')";
            if (q == 1)
                daysFeeding += " and id !=" + pK;
            string[] Item = Completion.Item(daysFeeding, 0, 0);
            for (int i = 0; i < Item.Length; i++)
                foreach (CheckBox cb in this.Controls.OfType<CheckBox>())
                    if (Item[i].Contains(cb.Text))
                    {
                        cb.Enabled = false;
                        countDay++;
                    }
            if (q == 1)
            {
                Item = Completion.Item("select days from diet where id=" + pK, 0, 0);
                for (int i = 0; i < Item.Length; i++)
                    foreach (CheckBox cb in this.Controls.OfType<CheckBox>())
                        if (Item[i].Contains(cb.Text))
                        {
                            cb.Checked = true;
                            countDay++;
                        }
            }
            if (countDay >= 7)
                return false;
            return b;
        }

        private bool Check()
        {
            bool b = true;
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Вы указали не все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked && !checkBox5.Checked && !checkBox6.Checked && !checkBox7.Checked && q == 0)
            {
                MessageBox.Show("Вы не указали дни кормления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return b;
        }
        private string Days()
        {
            string cheeck = "";
            foreach (CheckBox cb in this.Controls.OfType<CheckBox>())
            {
                if (cb.Checked)
                {
                    if (cheeck != "")
                        cheeck += ",";
                    cheeck += cb.Text;
                }
            }
            return cheeck;
        }
        private void Clear()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            CBox();
        }
        private void CBox()
        {
            foreach (CheckBox cb in this.Controls.OfType<CheckBox>())
            {
                cb.Checked = false;
                cb.Enabled = true;
            }
        }

        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int u = pK;
            q = 1;
            int rI = e.RowIndex;
            try
            {
                pK = Convert.ToInt32(dGV1.Rows[rI].Cells["id"].Value.ToString());
            }
            catch
            {
                return;
            }
            button5.Visible = true;
            string[] Item = Completion.Item(request + " where id=" + pK, 1, 1);
            comboBox1.Text = Item[0];
            comboBox2.Text = Item[1];
            comboBox1.Enabled = false;
            CBox();
            CheckBoxChecked();
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            q = 0;
            Clear();
            comboBox1.Enabled = true;
            button5.Visible = false;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void diet_Load(object sender, EventArgs e)
        {
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
        }

        private void nextLabel_Click(object sender, EventArgs e)
        {
            numberPage++;
            UpdateUI();
            LoadData();
        }

        private void backLabel_Click(object sender, EventArgs e)
        {
            numberPage--;
            UpdateUI();
            LoadData();
        }
        private void UpdateUI()
        {
            nextLabel.Enabled = true;
            backLabel.Enabled = true;
            if (numberPage == 1)
                backLabel.Enabled = false;
            if (numberPage == countPage)
                nextLabel.Enabled = false;
            label4.Text = numberPage + "/" + countPage;
        }
    }
}
