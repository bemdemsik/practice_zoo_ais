using System;
using System.Drawing;
using System.Windows.Forms;

namespace Зоопарк
{
    public partial class statusFeeding : Form
    {
        string status = "";
        string filt = "";
        string ono = " group by idstaff,idanimal,idfeed,date_feeding,time,status order by date_feeding desc,time,(select name from animals where id = idanimal)";
        string request = @"SELECT (select fio from users where id = (select iduser from staff where id=idstaff)) as 'Сотрудник',(select name from animals where id = idanimal) as 'Животное', (select name_food from feed where id = idfeed) as 'Корм',time as 'Кормление',
                        case time
                        when 'Утреннее' then(select morning_feeding from feeding_schedule where idanimals = idanimal)
                        when 'Вечернее' then(select party_feeding from feeding_schedule where idanimals = idanimal)
                        end  as 'Время кормления',date_feeding as 'Дата', status as 'Статус' FROM mydb.animal_feeding";
        string role = "";
        string fio = "";
        public statusFeeding(string role1, string fio1)
        {
            role = role1;
            fio = fio1;
            InitializeComponent();
        }
        private void load()
        {
            try
            {
                dGV1.DataSource = Table.Load(request + filt + ono);
                label5.Text = dGV1.Rows.Count.ToString();
            }
            catch
            {
                return;
            }
            Colors();
        }
        private void Colors()
        {
            int i = 0;
            foreach (DataGridViewRow row in dGV1.Rows)
            {
                if (row.Cells["Статус"].Value.ToString() == "Покормлен")
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Green;
                else if (row.Cells["Статус"].Value.ToString() == "Ожидается")
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Yellow;
                else
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Red;
                i++;
            }
            try
            {
                dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
            }
            catch
            {
                return;
            }
        }
        int rI = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            status = "Покормлен";
            if (!checkFeed.Check(dGV1.Rows[rI].Cells["Корм"].Value.ToString()))
            {
                var otv = MessageBox.Show("Корм '" + dGV1.Rows[rI].Cells["Корм"].Value.ToString() + "' закончился, чтобы покормить животных, необходимо его заказать\nХотите заказать его прямо сейчас?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (otv == DialogResult.Yes)
                {
                    this.Hide();
                    order q = new order(role, fio);
                    q.ShowDialog();
                }
                else
                    return;
            }
            updateStatus();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            status = "Не покормлен";
            updateStatus();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
        }

        private void statusFeeding_Load(object sender, EventArgs e)
        {
            load();
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker1.MaxDate = DateTime.Now;
            button5.Visible = false;
            groupBox2.Visible = false;
            groupBox1.Visible = false;
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dGV1.Columns[0].Width = 200;
            dGV1.Columns["Статус"].Width = 150;
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
        }
        private void updateStatus()
        {
            string znak = "";
            if (Table.Request("update animal_feeding set status='" + status + "' where idanimal=(select id from animals where name='" + dGV1.Rows[rI].Cells["Животное"].Value.ToString() + "') and date_feeding='" + dGV1.Rows[rI].Cells["Дата"].Value.ToString() + "' and time='" + dGV1.Rows[rI].Cells["Кормление"].Value.ToString() + "'"))
            {
                if (status == "Покормлен")
                {
                    dGV1.Rows[rI].Cells["Статус"].Style.BackColor = Color.Green;
                    znak = "-";
                }
                else
                {
                    dGV1.Rows[rI].Cells["Статус"].Style.BackColor = Color.Red;
                    if (dGV1.Rows[rI].Cells["Статус"].Value.ToString() == "Покормлен")
                        znak = "+";
                }
                dGV1.Rows[rI].Cells["Статус"].Value = status;
                if (znak != "")
                    if(!Table.Request("update feed set remainder=remainder" + znak + "(select sum(feed_quantity) from animal_feeding where idanimal = (select id from animals where name='" + dGV1.Rows[rI].Cells["Животное"].Value.ToString() + "') and date_feeding='" + dGV1.Rows[rI].Cells["Дата"].Value.ToString() + "' and time='" + dGV1.Rows[rI].Cells["Кормление"].Value.ToString() + "') where id=(select min(idfeed) from animal_feeding where date_feeding='" + dGV1.Rows[rI].Cells["Дата"].Value.ToString() + "' and idanimal = (select id from animals where name='" + dGV1.Rows[rI].Cells["Животное"].Value.ToString() + "'))"))
                        MessageBox.Show("Не удалось уследить за остатком корма", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Не удалось изменить статус, попробуйте позже", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            groupBox2.Visible = false;
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
            if (status == "Покормлен")
                if (!checkFeed.Check())
                {
                    var otv = MessageBox.Show("Кончается корм, необходимо его заказать.\nНажмите 'Да' если хотите перейти на форму заказа прямо сейчас", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (otv == DialogResult.Yes)
                    {
                        this.Hide();
                        order q = new order(role, fio);
                        q.ShowDialog();
                    }
                    else
                        return;
                }
        }

        private void dGV1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if(groupBox2.Visible || groupBox1.Visible)
            {
                groupBox2.Visible = false;
                groupBox1.Visible = false;
                button5.Visible = false;
                return;
            }
            rI = e.RowIndex;
            string[] time;
            try
            {
                time = dGV1.Rows[rI].Cells["Время кормления"].Value.ToString().Split(':');
            }
            catch
            {
                return;
            }
            int hourNow = Convert.ToInt32(DateTime.Now.ToString("HH"));
            int minuteNow = Convert.ToInt32(DateTime.Now.ToString("mm"));
            if (((Convert.ToInt32(time[0]) > hourNow) || (Convert.ToInt32(time[0]) == hourNow && Convert.ToInt32(time[1]) > minuteNow)) && DateTime.Now.ToString("yyyy-MM-dd") == dGV1.Rows[rI].Cells["Дата"].Value.ToString())
            {
                MessageBox.Show("Вы не можете изменить статус, еще рано", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            groupBox2.Visible = true;
            if (dGV1.Rows[rI].Cells["Статус"].Value.ToString() == "Покормлен")
            {
                button7.Enabled = false;
                button8.Enabled = true;
            }
            else if (dGV1.Rows[rI].Cells["Статус"].Value.ToString() == "Ожидается")
            {
                button8.Enabled = true;
                button7.Enabled = true;
            }
            else
            {
                button8.Enabled = false;
                button7.Enabled = true;
            }
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
        }

        private void statusFeeding_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            groupBox1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            filt = " where date_feeding='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' ";
            load();
            if (dGV1.Rows.Count == 0)
            {
                MessageBox.Show("За этот день нет даннных", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                groupBox1.Visible = true;
            }
            else
                groupBox1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            button5.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Visible = false;
            filt = "";
            load();
        }
    }
}
