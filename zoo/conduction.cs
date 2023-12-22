using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
namespace Зоопарк
{
    public partial class conduction : Form
    {
        int t = 0, pop = 0;
        int q = 0;
        int countPage = 0;
        int countRows = 0;
        int numberPage = 1;
        string[] Item;
        string role = "";
        string fio = "";
        string request = "";
        string delete = "";
        string insert = "";
        string change = "";
        string sort = "";
        string filt = "";
        string check = "";
        string tablee = "";
        public conduction(string role1, string fio1)
        {
            role = role1;
            fio = fio1;
            InitializeComponent();
        }

        private void животныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t = 1;
            tabControl1.SelectedIndex = 0;
            button5.Visible = true;
            Delay();
        }

        private void видыЖивотныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t = 2;
            tabControl1.SelectedIndex = 1;
            Delay();
        }

        private void вольерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t = 3;
            tabControl1.SelectedIndex = 2;
            Delay();
        }

        private void кормToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t = 4;
            tabControl1.SelectedIndex = 3;
            Delay();
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

        private void поставщикиКормаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t = 5;
            tabControl1.SelectedIndex = 4;
            Delay();
        }

        private void животныеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            t = 6;
            tabControl1.SelectedIndex = 5;
            button11.Visible = true;
            Delay();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            q = 0;
            if (!Check())
                return;
            Query();
            if (Table.Request(insert))
                MessageBox.Show("Добавлен 1 элемент", "", MessageBoxButtons.OK);
            load();
            Clear();
            Add();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            var kn = MessageBox.Show("Вы уверены что хотите удалить эту запись?", "", MessageBoxButtons.YesNo);
            if (kn == DialogResult.No)
                return;
            Query();
            if (Table.Request(delete))
                MessageBox.Show("Удален 1 элемент", "", MessageBoxButtons.OK);
            load();
            Clear();
            Add();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            q = 1;
            if (!Check())
                return;
            Query();
            if (Table.Request(change))
                MessageBox.Show("Изменен 1 элемент", "", MessageBoxButtons.OK);
            load();
            Clear();
            Add();
        }

        int pK = 0;
        int rI = 0;
        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nado = rI;
            rI = e.RowIndex;
            try
            {
                pK = Convert.ToInt32(dGV1.Rows[rI].Cells["id"].Value.ToString());
                if(nado != 0)
                switch (t)
                {
                    case 1:
                        comboBox2.Items.Remove(dGV1.Rows[nado].Cells["Номер вольера"].Value.ToString());
                        break;
                }
            }
            catch
            {
                return;
            }
            pop = 1;
            Fields();
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void Delay()
        {
            if (t != 1 && t != 6)
                Hidee();
            if (t == 1)
            {
                groupBox3.Visible = false;
                button11.Visible = false;
            }
            if (t == 6)
            {
                groupBox2.Visible = false;
                button5.Visible = false;
            }
            numberPage = 1;
            sort = "";
            filt = "";
            Clear();
            Add();
            Query();
            load();
        }
        private void Query()
        {
            switch (t)
            {
                case 1:
                    {
                        request = "SELECT id, name as 'Название рода', (SELECT view FROM views_animals where id = idview) as 'Вид', (select number from aviary where id=idaviary) as 'Номер вольера', (select count(*) from animalsnick where idanimals = animals.id) as 'Количество животных этого рода' from animals";
                        insert = "insert into animals(name,idview,idaviary) values('" + texta1.Text + "',(select id from views_animals where view='" + comboBox1.Text + "'),(select id from aviary where number=" + comboBox2.Text + "))";
                        change = "update animals set name='" + texta1.Text + "',idview=(select id from views_animals where view='" + comboBox1.Text + "'),idaviary=(select id from aviary where number=" + comboBox2.Text + ")" + " where id=" + pK;
                        delete = "delete from animals where id=" + pK;
                        tablee = "animals";
                        break;
                    }
                case 2:
                    {
                        request = "SELECT views_animals.id, view as 'Вид животного', (select count(*) from animals where idview = views_animals.id) as 'Количество животных этого вида' from views_animals";
                        insert = "insert into views_animals(view) values('" + textv1.Text + "')";
                        change = "update views_animals set view='" + textv1.Text + "' where id =" + pK;
                        delete = "delete from views_animals where id=" + pK;
                        tablee = "views_animals";
                        break;
                    }
                case 3:
                    {
                        request = "SELECT id,number as 'Номер вольера' from aviary";
                        insert = "insert into aviary(number) values(" + textBox1.Text + ")";
                        change = "update aviary set number=" + textBox1.Text + " where id =" + pK;
                        delete = "delete from aviary where id=" + pK;
                        tablee = "aviary";
                        break;
                    }
                case 4:
                    {
                        request = "SELECT id,name_food as 'Название корма', remainder as 'Остаток(кг)' from feed";
                        insert = "insert into feed(name_food,remainder) values('" + textf1.Text + "','" + textf2.Text + "')";
                        change = "update feed set name_food='" + textf1.Text + "',remainder=" + textf2.Text + " where id =" + pK;
                        delete = "delete from feed where id=" + pK;
                        tablee = "feed";
                        break;
                    }
                case 5:
                    {
                        request = "SELECT id,organization_name as 'Поставщик', (select name_food from feed where id=idfeed) as 'Поставляемый корм', price as 'Цена за кг' from feed_supplier";
                        insert = "insert into feed_supplier(organization_name,idfeed,price) values('" + texto1.Text + "',(select id from feed where name_food='" + comboBox4.Text + "')," + texto2.Text + ")";
                        change = "update feed_supplier set organization_name='" + texto1.Text + "',idfeed=(select id from feed where name_food='" + comboBox4.Text + "'),price=" + texto2.Text + " where id =" + pK;
                        delete = "delete from feed_supplier where id=" + pK;
                        tablee = "feed_supplier";
                        break;
                    }
                case 6:
                    {
                        request = "SELECT id, (select name from animals where id=idanimals) as 'Род животного', nickname as 'Кличка', weight as 'Вес', receipt_date as 'Дата прибытия' from animalsnick";
                        insert = "insert into animalsnick(idanimals,nickname,weight,receipt_date) values((select id from animals where name = '" + comboBox5.Text + "'),'" + textBox2.Text + "','" + textBox3.Text + "',CURDATE())";
                        change = "update animalsnick set idanimals=(select id from animals where name = '" + comboBox5.Text + "'),nickname='" + textBox2.Text + "',weight='" + textBox3.Text + "' where id =" + pK;
                        delete = "delete from animalsnick where id=" + pK;
                        tablee = "animalsnick";
                        break;
                    }
                default: break;
            }
        }

        private void load()
        {
            pop = 0;
            try
            {
                countRows = Convert.ToInt32(Completion.Item("select count(*) from " + tablee, 0, 0)[0]);
                if (sort == "" && t == 6)
                    dGV1.DataSource = Table.Load(request + " order by (select name from animals where id=idanimals) limit " + ((numberPage - 1) * 10) + ",10");
                else if (t == 4)
                {
                    dGV1.DataSource = Table.Load(request + " order by remainder limit " + ((numberPage - 1) * 10) + ",10");
                    Colorr();
                }
                else
                    dGV1.DataSource = Table.Load(request + filt + sort + " limit " + ((numberPage - 1) * 10) + ",10");
                dGV1.Columns["id"].Visible = false;
                countPage = Convert.ToInt32(Math.Ceiling(countRows / 10.0));
                label5.Text = countRows.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK);
                return;
            }
            if (dGV1.Rows.Count == 0)
            {
                nextLabel.Enabled = false;
                backLabel.Enabled = false;
                return;
            }
            UpdateUI();
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            button3.Enabled = false;
            button4.Enabled = false;
        }
        private void Fields()
        {
            switch (t)
            {
                case 1:
                    {
                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            texta1.Text = Item[0];
                            comboBox1.Text = Item[1];
                            if (!comboBox2.Items.Contains(Item[2]))
                                comboBox2.Items.Add(Item[2]);
                            comboBox2.Text = Item[2];
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                case 2:
                    {
                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            textv1.Text = Item[0];
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                case 3:
                    {

                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            textBox1.Text = Item[0];
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                case 4:
                    {
                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            textf1.Text = Item[0];
                            textf2.Text = Item[1].Replace(",", ".");
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                case 5:
                    {
                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            texto1.Text = Item[0];
                            comboBox4.Text = Item[1];
                            texto2.Text = Item[2].Replace(",", ".");
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                case 6:
                    {
                        Item = Completion.Item(request + " where id=" + pK, 1, 1);
                        try
                        {
                            comboBox5.Text = Item[0];
                            textBox2.Text = Item[1];
                            textBox3.Text = Item[2].Replace(",", ".");
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    }
                default: break;
            }
        }

        private void Add()
        {
            switch (t)
            {
                case 1:
                    {
                        comboBox8.Items.Clear();
                        Item = Completion.Item("select view from views_animals", 0, 0);
                        comboBox8.Items.Add("");
                        for (int i = 0; i < Item.Length; i++)
                        {
                            comboBox1.Items.Add(Item[i]);
                            comboBox8.Items.Add(Item[i]);
                        }
                        Item = Completion.Item("select number from aviary where id not in (select idaviary from animals)", 0, 0);
                        for (int i = 0; i < Item.Length; i++)
                            comboBox2.Items.Add(Item[i]);
                        break;
                    }
                case 5:
                    {
                        Item = Completion.Item("select name_food from feed", 0, 0);
                        for (int i = 0; i < Item.Length; i++)
                            comboBox4.Items.Add(Item[i]);
                        break;
                    }
                case 6:
                    {
                        Item = Completion.Item("select name from animals", 0, 0);
                        for (int i = 0; i < Item.Length; i++)
                            comboBox5.Items.Add(Item[i]);
                        break;
                    }
                case 7:
                    {
                        
                        break;
                    }
                default: break;
            }
        }

        private bool Check()
        {
            bool p = true;
            switch (t)
            {
                case 1:
                    {
                        check = "SELECT * FROM animals where name ='" + texta1.Text + "'";
                        if (q == 1)
                            check += " and id != " + pK;
                        if (Completion.Item(check, 0, 2).Length > 0)
                        {
                            MessageBox.Show("Такой род уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            texta1.Clear();
                            return false;
                        }
                        if (texta1.Text == "" || comboBox1.Text == "" || comboBox2.Text == "")
                            p = false;
                        break;
                    }
                case 2:
                    {
                        check = "SELECT * FROM views_animals where view ='" + textv1.Text + "'";
                        if (q == 1)
                            check += " and id != " + pK;
                        if (Completion.Item(check, 0, 2).Length > 0)
                        {
                            MessageBox.Show("Такой вид уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textv1.Clear();
                            return false;
                        }
                        if (textv1.Text == "")
                            p = false;
                        break;
                    }
                case 3:
                    {
                        check = "SELECT * FROM aviary where number ='" + textBox1.Text + "'";
                        if (q == 1)
                            check += " and id != " + pK;
                        if (Completion.Item(check, 0, 2).Length > 0)
                        {
                            MessageBox.Show("Такой номер вольера уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox1.Clear();
                            return false;
                        }
                        if (textBox1.Text == "")
                            p = false;
                        break;
                    }
                case 4:
                    {
                        check = "SELECT * FROM feed where name_food ='" + textf1.Text + "'";
                        if (q == 1)
                            check += " and id != " + pK;
                        if (Completion.Item(check, 0, 2).Length > 0)
                        {
                            MessageBox.Show("Такой корм уже существует, вы можете его заказать на форме заказ корма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textf1.Clear();
                            return false;
                        }
                        if (textf1.Text == "" || textf2.Text == "")
                            p = false;
                        break;
                    }
                case 5:
                    {
                        if (texto1.Text == "" || texto2.Text == "" || comboBox4.Text == "")
                            p = false;
                        break;
                    }
                case 6:
                    {
                        check = "SELECT * FROM animalsnick where nickname ='" + textBox2.Text + "' and idanimals = (select id from animals where name = '" + comboBox5.Text + "')";
                        if (q == 1)
                            check += " and id != " + pK;
                        if (Completion.Item(check, 0, 2).Length > 0)
                        {
                            MessageBox.Show("Такая кличка у рода '" + comboBox5.Text + "' уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox2.Clear();
                            return false;
                        }
                        if (textBox2.Text == "" || textBox3.Text == "" || comboBox5.Text == "")
                            p = false;
                        break;
                    }
                default: break;
            }
            if (!p)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
        private void Hidee()
        {
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            button5.Visible = false;
            button11.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (comboBox8.Text == "")
                filt = "";
            else
                filt = " where (select view from views_animals where id=idview)='" + comboBox8.Text + "' ";
            load();
            groupBox2.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
        }

        private void Clear()
        {
            foreach (TextBox t in tabControl1.SelectedTab.Controls.OfType<TextBox>())
            {
                t.Text = "";
            }
            foreach (ComboBox t in tabControl1.SelectedTab.Controls.OfType<ComboBox>())
            {
                t.Text = "";
                t.Items.Clear();
            }
        }

        private void texta1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[ё]|[\b]|[^\S\r\n]"))
                e.Handled = true;

        }

        private void doubleKeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (Regex.IsMatch(Symbol, @"[0-9]|[\b]"))
            {
                e.Handled = false;
                return;
            }
            if (Symbol == ".")
            {
                if (textBox3.Text.Contains(".") || textBox3.Text == "" && t == 1)
                    e.Handled = true;
                else if (textf2.Text.Contains(".") || textf2.Text == "" && t == 4)
                    e.Handled = true;
                else if (texto2.Text.Contains(".") || texto2.Text == "" && t == 5)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void fioKeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (Symbol == "." && texto1.Text == "" && t == 5)
            {
                e.Handled = true;
                return;
            }
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[\b]|[^\S\r\n]|[.]"))
                e.Handled = true;
        }

        private void numberKeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[0-9]|[\b]"))
                e.Handled = true;
        }

        private void textss1_TextChanged(object sender, EventArgs e)
        {
            TextBox q = (TextBox)sender;
            if (q.Text.Length == 1)
            {
                q.Text = q.Text.ToUpper();
                q.SelectionStart = 1;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
                if (t == 6)
                    sort = " order by (select name from animals where id=idanimals)";
                else
                    sort = "";
            else
            {
                if (comboBox3.Text == "возрастанию")
                    sort = " order by receipt_date";
                else
                    sort = " order by receipt_date desc";
            }
            UpdateUI();
            load();
            groupBox3.Visible = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = true;
        }
        private void Colorr()
        {
            int i = 0;
            foreach (DataGridViewRow row in dGV1.Rows)
            {
                if ("0" != Completion.Item("select count(*) from animalsnick where (select sum(weight) from animalsnick where idanimals in (select idanimals from diet where idfeed = " + row.Cells[0].Value.ToString() + "))*0.03 > (select remainder from feed where id = " + row.Cells[0].Value.ToString() + ")", 0, 0)[0])
                    dGV1.Rows[i].Cells[2].Style.BackColor = Color.Red;
                else
                    dGV1.Rows[i].Cells[2].Style.BackColor = Color.White;
                i++;
            }
        }

        private void conduction_Load(object sender, EventArgs e)
        {
            t = 1;
            Hidee();
            Delay();
            AddFS();
            button5.Visible = true;
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
        }

        private void nextLabel_Click(object sender, EventArgs e)
        {
            numberPage++;
            UpdateUI();
            load();
        }

        private void backLabel_Click(object sender, EventArgs e)
        {
            numberPage--;
            UpdateUI();
            load();
        }
        private void UpdateUI()
        {
            nextLabel.Enabled = true;
            backLabel.Enabled = true;
            if (numberPage == 1)
                backLabel.Enabled = false;
            if (numberPage == countPage)
                nextLabel.Enabled = false;
            label18.Text = numberPage + "/" + countPage;
        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count == 0)
                MessageBox.Show("Нет свободного вольера, но вы можете его добавить на форме Вольеры", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void AddFS()
        {
            comboBox3.Items.Add("");
            comboBox3.Items.Add("возрастанию");
            comboBox3.Items.Add("убыванию");
        }
    }
}
