using System;
using System.Drawing;
using System.Windows.Forms;

namespace Зоопарк
{
    public partial class mainStaff : Form
    {
        int rI = 0;
        string status = "";
        string request = "";
        public mainStaff(string role1, string fio1)
        {
            InitializeComponent();
            label4.Text = role1;
            label5.Text = fio1;
            request = @"select (select name from animals where id=idanimal) as 'Род животного', (select name_food from feed where id=idfeed) as 'Корм', time as 'Кормление',
                        case time 
                        when 'Утреннее' then(select morning_feeding from feeding_schedule where idanimals = idanimal)
                        when 'Вечернее' then(select party_feeding from feeding_schedule where idanimals = idanimal)
                        end  as 'Время кормления', status as 'Статус'
                        from animal_feeding
                        where date_feeding = CURDATE() and idstaff in (select id from staff where iduser=(select id from users where fio='"+fio1+"')) group by idanimal,idfeed,time,status";
            insertNewFeeding.Check();
            Table.Request("update animal_feeding set status='Не покормлен' where date_feeding<CURDATE() and status='Ожидается'");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            authorization q = new authorization();
            q.ShowDialog();
        }
        private void load()
        {
            try
            {
                dGV1.DataSource = Table.Load(request);
                if(dGV1.Rows.Count == 0)
                {
                    dGV1.Visible = false;
                    label1.Visible = true;
                    return;
                } 
            }
            catch
            {
                return;
            }
            Colors();
        }
        private void mainStaff_Load(object sender, EventArgs e)
        {
            load();
            groupBox2.Visible = false;
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dGV1.Columns["Статус"].Width = 150;
            dGV1.Columns["Корм"].Width = 150;
        }
        private void Colors()
        {
            int i = 0;
            int check = 0;
            foreach (DataGridViewRow row in dGV1.Rows)
            {
                if (row.Cells["Статус"].Value.ToString() == "Покормлен")
                {
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Green;
                    check++;
                }
                else if (row.Cells["Статус"].Value.ToString() == "Ожидается")
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Yellow;
                else
                    dGV1.Rows[i].Cells["Статус"].Style.BackColor = Color.Red;
                i++;
            }
            if(check == i)
            {
                dGV1.Visible = false;
                label1.Visible = true;
                label1.Text = "На сегодня все, вы покормили всех назначенных Вам животных. Можете идти домой!";
                return;
            }
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[0].Cells["Статус"].Style.BackColor;    
        }

        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {        
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
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
            if ((Convert.ToInt32(time[0]) > hourNow) || (Convert.ToInt32(time[0]) == hourNow && Convert.ToInt32(time[1]) > minuteNow))
            {
                MessageBox.Show("Вы не можете изменить статус, еще рано", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                groupBox2.Visible = false;
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
        }
        private void updateStatus()
        {
            string znak = "";
            if (Table.Request("update animal_feeding set status='" + status + "' where idanimal=(select id from animals where name='" + dGV1.Rows[rI].Cells["Род животного"].Value.ToString() + "') and date_feeding=CURDATE() and time=('" + dGV1.Rows[rI].Cells["Кормление"].Value.ToString() + "')"))
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
                if(znak != "")
                if (!Table.Request("update feed set remainder=remainder"+znak+"(select sum(feed_quantity) from animal_feeding where idanimal = (select id from animals where name='" + dGV1.Rows[rI].Cells["Род животного"].Value.ToString() + "') and date_feeding=CURDATE() and time='" + dGV1.Rows[rI].Cells["Кормление"].Value.ToString() + "') where id=(select min(idfeed) from animal_feeding where date_feeding=CURDATE() and idanimal = (select id from animals where name='" + dGV1.Rows[rI].Cells["Род животного"].Value.ToString() + "'))"))
                    MessageBox.Show("Не удалось уследить за остатком корма", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dGV1.Rows[rI].Cells["Статус"].Value = status;     
            }
            else
            {
                MessageBox.Show("Не удалось изменить статус, попробуйте позже", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            groupBox2.Visible = false;
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            status = "Покормлен";
            if (!checkFeed.Check(dGV1.Rows[rI].Cells["Корм"].Value.ToString()))
            {
                MessageBox.Show("Корм '" + dGV1.Rows[rI].Cells["Корм"].Value.ToString() + "' закончился, вам нечем кормить животных", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return;
            }
            updateStatus();
            load();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            status = "Не покормлен";
            updateStatus();
            load();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
        }

        private void mainStaff_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
        }
    }
}
