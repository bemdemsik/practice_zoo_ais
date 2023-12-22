using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace Зоопарк
{
    public partial class order : Form
    {
        int pK = 0;
        int check = 0;
        double itog = 0;
        int countPage = 0;
        int countRows = 0;
        int numberPage = 1;
        string[] otchet = new string[0];
        string role = "";
        string fio = "";
        string insert = "";
        string insertSupplier = "";
        string requestFeed = "select id, name_food as 'Название корма', remainder as 'Остаток в кг' from feed";
        string requestSupplier = "select id, organization_name as 'Поставщик', (select name_food from feed where id=idfeed) as 'Поставляемый корм',price as 'Цена за кг' from feed_supplier";
        string request = "select id,(select organization_name from feed_supplier where id=idfeeding_supplier) as 'Поставщик', (select name_food from feed where id=idfeed) as 'Корм', feed_quantity_kg as 'Количество в кг',price as 'Цена', order_date as 'Дата заказа' from feed_order";
        public order(string role1, string fio1)
        {
            fio = fio1;
            role = role1;
            InitializeComponent();
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            button4.Visible = false;
            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker2.MaxDate = DateTime.Now;
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            load(request + " order by order_date desc");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
            numberPage = 1;
            UpdateUI();
            if (button1.Text == "Назад")
            {
                if (button2.Text == "Добавить")
                {
                    if (comboBox1.Text == "")
                        if (MessageBox.Show("Вы так и не добавили поставщика для корма " + textBox1.Text + ", хотите вернуться и заказать другой корм?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                        else
                            textBox1.Clear();
                    button2.Text = "Заказать";
                    load(requestFeed + " order by remainder");
                    groupBox2.Visible = false;
                    groupBox1.Visible = true;
                }
                else
                {
                    button1.Text = "В меню";
                    button2.Text = "Заказать корм";
                    button3.Visible = true;
                    load(request + " order by order_date desc");
                    groupBox1.Visible = false;
                    groupBox2.Visible = false;
                }
                button2.Enabled = true;
            }
            else
            {
                if (otchet.Length > 0)
                    if (MessageBox.Show("Хотите записать отчет заказа в документ?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        report();
                        MessageBox.Show("Отчет записан в документ 'Заказ корма за " + DateTime.Now.ToString("yyyy-MM-dd") + "'", "Внимание", MessageBoxButtons.OK);
                    }
                this.Hide();
                Main q = new Main(role, fio);
                q.ShowDialog();
            }
        }
        private void load(string query)
        {
            try
            {
                if (button2.Text == "Заказать корм")
                    countRows = Convert.ToInt32(Completion.Item("select count(*) from feed_order", 0, 0)[0]);
                else if (button2.Text == "Заказать")
                    countRows = Convert.ToInt32(Completion.Item("select count(*) from feed", 0, 0)[0]);
                else
                    countRows = Convert.ToInt32(Completion.Item("select count(*) from feed_supplier", 0, 0)[0]);
                if (button2.Text == "Заказать")
                {
                    dGV1.DataSource = Table.Load(query + " limit " + ((numberPage - 1) * 10) + ",10");
                    countPage = Convert.ToInt32(Math.Ceiling(countRows / 10.0));
                }
                else
                {
                    dGV1.DataSource = Table.Load(query + " limit " + ((numberPage - 1) * 14) + ",14");
                    countPage = Convert.ToInt32(Math.Ceiling(countRows / 14.0));
                }
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
            UpdateUI();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
            if (!Check())
            {
                MessageBox.Show("Вы указали не все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (button2.Text == "Заказать корм")
            {
                numberPage = 1;
                UpdateUI();
                button2.Text = "Заказать";
                button1.Text = "Назад";
                button2.Enabled = false;
                button3.Visible = false;
                groupBox1.Visible = true;
                load(requestFeed + " order by remainder");
                Colorr();
                MessageBox.Show("Клините по корму, который хотите заказать или введите название сами", "", MessageBoxButtons.OK);
            }
            else if (button2.Text == "Заказать")
            {
                insert = "insert into feed_order (idfeeding_supplier,idfeed,feed_quantity_kg,price,order_date) values((select id from feed_supplier where organization_name='" + comboBox1.Text + "'),(select id from feed where name_food='" + textBox1.Text + "'),'" + textBox2.Text + "','" + label9.Text + "',CURDATE())";
                if (Table.Request(insert))
                {
                    MessageBox.Show("Вы заказали " + textBox1.Text + " у " + comboBox1.Text, "Круто", MessageBoxButtons.OK);
                    Table.Request("update feed set remainder=remainder+" + textBox2.Text + " where id=" + Completion.Item("select id from feed where name_food='" + textBox1.Text + "'", 0, 0)[0]);
                    Array.Resize(ref otchet, otchet.Length + 1);
                    otchet[otchet.Length - 1] = comboBox1.Text + "," + textBox1.Text + "," + textBox2.Text + "," + label9.Text + ",";
                    itog += Convert.ToDouble(label9.Text.Replace(".", ","));
                    textBox1.Clear();
                    textBox2.Clear();
                    label9.Text = "";
                    textBox3.Clear();
                    comboBox1.Items.Clear();
                    comboBox1.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Не удалось заказать корм, попробуйте позже", "Ошибка", MessageBoxButtons.OK);
                    return;
                }
                load(requestFeed + " order by remainder");
                Colorr();
            }
            else
            {
                if (check == 1)
                    if (!Table.Request("insert into feed(name_food,remainder) values('" + textBox4.Text + "',0)"))
                    {
                        MessageBox.Show("При добавлении поставщика возникла ошибка, попробуйте позже", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                insertSupplier = "insert into feed_supplier(organization_name,idfeed,price) values('" + texto1.Text + "',(select id from feed where name_food='" + textBox4.Text + "')," + texto2.Text + ")";
                if (Table.Request(insertSupplier))
                {
                    MessageBox.Show("Вы добавили поставщика для корма " + textBox4.Text + "\nТеперь можете вернуться и заказать этот корм", "Круто", MessageBoxButtons.OK);
                    comboBox1.Items.Add(texto1.Text);
                    comboBox1.Text = texto1.Text;
                    texto1.Clear();
                    texto2.Clear();
                }
                else
                    MessageBox.Show("Не удалось добавить поставщика, попробуйте позже", "Ошибка", MessageBoxButtons.OK);
                load(requestSupplier);
            }
            button4.Visible = false;
            textBox1.Enabled = true;
        }

        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (button2.Text != "Заказать")
                return;
            int rI = e.RowIndex;
            try
            {
                pK = Convert.ToInt32(dGV1.Rows[rI].Cells["id"].Value.ToString());
            }
            catch
            {
                return;
            }
            textBox1.Text = Completion.Item(requestFeed + " where id=" + pK, 1, 1)[0];
            button2.Enabled = true;
            textBox1.Enabled = false;
            button4.Visible = true;
        }
        private bool Check()
        {
            bool b = true;
            if (groupBox1.Visible)
                if (textBox1.Text == "" || comboBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                    b = false;
            if (groupBox2.Visible)
                if (texto1.Text == "" || texto2.Text == "" || textBox4.Text == "")
                    b = false;
            return b;
        }

        private void report()
        {
            Word.Application application = new Word.Application();
            Word.Document document = application.Documents.Add();
            document.PageSetup.TopMargin = application.InchesToPoints(0.4f);
            document.PageSetup.LeftMargin = application.InchesToPoints(0.4f);
            document.PageSetup.RightMargin = application.InchesToPoints(0.4f);
            document.PageSetup.BottomMargin = application.InchesToPoints(0.4f);

            Word.Paragraph Par = document.Paragraphs.Add();
            Par.Range.Font.Size = 18;
            Par.Range.Text = "Заказ корма";
            Par.Range.Bold = 1;
            Par.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            Par.Range.InsertParagraphAfter();
            Word.Paragraph Dat = document.Paragraphs.Add();
            Dat.Range.Font.Size = 14;
            Dat.Range.Text = "Дата: " + DateTime.Now.ToString("yyyy-MM-dd");
            Dat.Range.Bold = 0;
            Par.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            Dat.Range.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table paymentsTable = document.Tables.Add(tableRange, 1, 4);
            paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = paymentsTable.Cell(1, 1).Range;
            cellRange.Text = "Поставщик";
            cellRange = paymentsTable.Cell(1, 2).Range;
            cellRange.Text = "Корм";
            cellRange = paymentsTable.Cell(1, 3).Range;
            cellRange.Text = "Количество в кг";
            cellRange = paymentsTable.Cell(1, 4).Range;
            cellRange.Text = "Цена";
            paymentsTable.Rows[1].Range.Bold = 1;
            paymentsTable.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int y = 2;
            for (int i = 0; i < otchet.Length; i++)
            {
                paymentsTable.Rows.Add();
                paymentsTable.Rows[y].Range.Bold = 0;
                cellRange = paymentsTable.Cell(y, 1).Range;
                cellRange.Text = otchet[i].Split(',')[0];
                cellRange = paymentsTable.Cell(y, 2).Range;
                cellRange.Text = otchet[i].Split(',')[1];
                cellRange = paymentsTable.Cell(y, 3).Range;
                cellRange.Text = otchet[i].Split(',')[2].Replace(",", ".");
                cellRange = paymentsTable.Cell(y, 4).Range;
                cellRange.Text = otchet[i].Split(',')[3].Replace(",", ".");
                y++;
                if (i == otchet.Length - 1)
                {
                    paymentsTable.Rows.Add();
                    paymentsTable.Rows[y].Range.Bold = 1;
                    cellRange = paymentsTable.Cell(y, 3).Range;
                    cellRange.Text = "Итого:";
                    cellRange = paymentsTable.Cell(y, 4).Range;
                    cellRange.Text = itog.ToString().Replace(",", ".");
                }
            }
            application.Visible = true;
            document.SaveAs(Directory.GetCurrentDirectory() + @"\Заказ корма за " + DateTime.Now.ToString("yyyy-MM-dd") + ".docx");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataTable feed_order = new DataTable();
            feed_order = Table.Load(request + " where order_date >= '" + string.Join("-", dateTimePicker1.Value.ToString().Split(' ')[0].Split('.').Reverse()) + "' and order_date <= '" + string.Join("-", dateTimePicker2.Value.ToString().Split(' ')[0].Split('.').Reverse()) + "' order by order_date desc");

            Word.Application application = new Word.Application();
            Word.Document document = application.Documents.Add();
            document.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;
            document.PageSetup.TopMargin = application.InchesToPoints(0.4f);
            document.PageSetup.LeftMargin = application.InchesToPoints(0.4f);
            document.PageSetup.RightMargin = application.InchesToPoints(0.4f);
            document.PageSetup.BottomMargin = application.InchesToPoints(0.4f);

            Word.Paragraph Par = document.Paragraphs.Add();
            Par.Range.Font.Size = 18;
            Par.Range.Text = "Заказ корма";
            Par.Range.Bold = 1;
            Par.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            Par.Range.InsertParagraphAfter();
            Word.Paragraph Dat = document.Paragraphs.Add();
            Dat.Range.Font.Size = 14;
            Dat.Range.Text = "Дата: c " + string.Join("-", dateTimePicker1.Value.ToString().Split(' ')[0].Split('.').Reverse()) +
                " по " + string.Join("-", dateTimePicker2.Value.ToString().Split(' ')[0].Split('.').Reverse());
            Dat.Range.Bold = 0;
            Par.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            Dat.Range.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table paymentsTable = document.Tables.Add(tableRange, 1, 4);
            paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = paymentsTable.Cell(1, 1).Range;
            cellRange.Text = "Поставщик";
            cellRange = paymentsTable.Cell(1, 2).Range;
            cellRange.Text = "Корм";
            cellRange = paymentsTable.Cell(1, 3).Range;
            cellRange.Text = "Количество в кг";
            cellRange = paymentsTable.Cell(1, 4).Range;
            cellRange.Text = "Цена";
            paymentsTable.Rows[1].Range.Bold = 1;
            paymentsTable.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int y = 2;
            foreach (DataRow row in feed_order.Rows)
            {
                paymentsTable.Rows.Add();
                paymentsTable.Rows[y].Range.Bold = 0;
                cellRange = paymentsTable.Cell(y, 1).Range;
                cellRange.Text = row[1].ToString();
                cellRange = paymentsTable.Cell(y, 2).Range;
                cellRange.Text = row[2].ToString();
                cellRange = paymentsTable.Cell(y, 3).Range;
                cellRange.Text = row[3].ToString();
                cellRange = paymentsTable.Cell(y, 4).Range;
                cellRange.Text = row[4].ToString();
                itog += Convert.ToDouble(row[4].ToString().Replace(".", ","));
                y++;
                if (y == feed_order.Rows.Count + 2)
                {
                    paymentsTable.Rows.Add();
                    paymentsTable.Rows[y].Range.Bold = 1;
                    cellRange = paymentsTable.Cell(y, 3).Range;
                    cellRange.Text = "Итого:";
                    cellRange = paymentsTable.Cell(y, 4).Range;
                    cellRange.Text = itog.ToString().Replace(",", ".");
                }
            }
            application.Visible = true;
            document.SaveAs(Directory.GetCurrentDirectory() + @"\Заказы за период " + string.Join("-", dateTimePicker1.Value.ToString().Split(' ')[0].Split('.').Reverse()) + " по " + string.Join("-", dateTimePicker2.Value.ToString().Split(' ')[0].Split('.').Reverse()) + ".docx");
            groupBox1.Visible = false;
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (Regex.IsMatch(Symbol, @"[0-9]|[\b]"))
            {
                e.Handled = false;
                return;
            }
            if (Symbol == ".")
            {
                if (textBox2.Text.Contains(".") || textBox2.Text == "" && groupBox1.Visible)
                    e.Handled = true;
                else if (texto2.Text.Contains(".") || texto2.Text == "" && groupBox2.Visible)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox3.Text = Completion.Item("select price from feed_supplier where organization_name='" + comboBox1.Text + "'", 0, 0)[0].Replace(",", ".");
            }
            catch
            {
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text[textBox2.Text.Length - 1] == '.')
                    return;
                label9.Text = (Convert.ToDouble(textBox2.Text.Replace(".", ",")) * Convert.ToDouble(textBox3.Text.Replace(".", ","))).ToString().Replace(",", ".");
            }
            catch
            {
                return;
            }
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Enabled == true)
                return;
            string[] Item = Completion.Item("select organization_name from feed_supplier where idfeed=(select id from feed where name_food='" + textBox1.Text + "')", 0, 0);
            if (Item.Length == 0 && textBox1.Text != "")
            {
                var t = MessageBox.Show("Поставщиков этого корма нет\nНажмите 'Ок', чтобы перейти на форму поставщиков", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (t == DialogResult.OK)
                {
                    load(requestSupplier);
                    textBox4.Text = textBox1.Text;
                    groupBox1.Visible = false;
                    groupBox2.Visible = true;
                    button2.Text = "Добавить";
                    button1.Text = "Назад";
                }
            }
            else
            {
                comboBox1.Items.Clear();
                for (int i = 0; i < Item.Length; i++)
                    comboBox1.Items.Add(Item[i]);
            }
        }

        private void order_Load(object sender, EventArgs e)
        {
            dGV1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGV1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(192, 192, 255);
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void nextLabel_Click(object sender, EventArgs e)
        {
            numberPage++;
            UpdateUI();
            if (button2.Text == "Заказать корм")
                load(request + " order by order_date desc");
            else if (button2.Text == "Заказать")
                load(requestFeed + " order by remainder");
            else
                load(requestSupplier);
        }

        private void backLabel_Click(object sender, EventArgs e)
        {
            numberPage--;
            UpdateUI();
            if (button2.Text == "Заказать корм")
                load(request + " order by order_date desc");
            else if (button2.Text == "Заказать")
                load(requestFeed + " order by remainder");
            else
                load(requestSupplier);
        }
        private void UpdateUI()
        {
            nextLabel.Enabled = true;
            backLabel.Enabled = true;
            if (numberPage == 1)
                backLabel.Enabled = false;
            if (numberPage == countPage)
                nextLabel.Enabled = false;
            label13.Text = numberPage + "/" + countPage;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            if (dateTimePicker1.Value > dateTimePicker2.Value)
                dateTimePicker2.Value = dateTimePicker1.Value;
        }

        private void texto1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (Symbol == "." && (texto1.Text == ""))
            {
                e.Handled = true;
                return;
            }
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[\b]|[^\S\r\n]|[.]"))
                e.Handled = true;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (Completion.Item("select * from feed where name_food='" + textBox1.Text + "'", 0, 0).Length == 0)
                check = 1;
            else
                check = 0;
            string[] Item = Completion.Item("select organization_name from feed_supplier where idfeed=(select id from feed where name_food='" + textBox1.Text + "')", 0, 0);
            if (Item.Length == 0 && textBox1.Text != "")
            {
                var t = MessageBox.Show("Поставщиков этого корма нет\nНажмите 'Ок', чтобы перейти на форму поставщиков", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (t == DialogResult.OK)
                {
                    load(requestSupplier);
                    textBox4.Text = textBox1.Text;
                    groupBox1.Visible = false;
                    groupBox2.Visible = true;
                    button2.Text = "Добавить";
                    button1.Text = "Назад";
                }
            }
            else
            {
                comboBox1.Items.Clear();
                for (int i = 0; i < Item.Length; i++)
                    comboBox1.Items.Add(Item[i]);
            }
            button2.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            check = 0;
            textBox1.Enabled = true;
            button4.Visible = false;
            textBox1.Clear();
            textBox2.Clear();
            label9.Text = "";
            textBox3.Clear();
            comboBox1.Items.Clear();
            comboBox1.SelectedIndex = -1;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[\b]|[^\S\r\n]"))
                e.Handled = true;
        }
    }
}
