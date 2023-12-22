using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace Зоопарк
{
    public partial class Feeding : Form
    {
        DataTable feedingAnimals = new DataTable();
        string fio = "";
        string role = "";
        readonly string request = "SELECT id, (select fio from users where id = (select iduser from staff where id=idstaff)) as 'Сотрудник', (select name from animals where id = idanimal) as 'Животное', nickname as 'Кличка', (select name_food from feed where id = idfeed) as 'Корм',feed_quantity as 'Количество корма в кг',time as 'Кормление',date_feeding as 'Дата', status as 'Статус' FROM mydb.animal_feeding";
        string poisk = "";
        int countPage = 0;
        int countRows = 0;
        int numberPage = 1;
        string insert = "";
        public Feeding(string role1, string fio1)
        {
            fio = fio1;
            role = role1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main q = new Main(role, fio);
            q.ShowDialog();
        }
        private void load()
        {
            try
            {
                if (textBox1.Text == "")
                {
                    countRows = Convert.ToInt32(Completion.Item("select count(*) from animal_feeding", 0, 0)[0]);
                    dGV1.DataSource = Table.Load(request + " order by date_feeding desc,time,(select name from animals where id = idanimal) limit " + ((numberPage - 1) * 15) + ",15");
                }
                else
                {
                    countRows = Convert.ToInt32(Completion.Item("select count(*) from animal_feeding" + poisk, 0, 0)[0]);
                    dGV1.DataSource = Table.Load(request + poisk + " order by date_feeding desc,time,(select name from animals where id = idanimal) limit " + ((numberPage - 1) * 15) + ",15");
                }
                countPage = Convert.ToInt32(Math.Ceiling(countRows / 15.0));
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
            Colors();
            UpdateUI();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 1)
            {
                textBox1.Text = textBox1.Text.ToUpper();
                textBox1.SelectionStart = 1;
            }
            try
            {
                if (textBox1.Text == "")
                    poisk = "";
                else
                    poisk = " where (select name from animals where id = idanimal) LIKE '" + textBox1.Text + "%'";
                numberPage = 1;
                UpdateUI();
                load();
            }
            catch (Exception ee)
            {
                return;
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();
            if (!Regex.IsMatch(Symbol, @"[а-яА-Я]|[\b]|[^\S\r\n]"))
                e.Handled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы действительно хотите произвести экспорт в csv файл?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            if (dr == DialogResult.Yes)
            {
                DataTable dt = new DataTable();
                dt = Table.Load(request);
                string fileName = "Кормление.csv";
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate);
                }
                catch
                {
                    MessageBox.Show("Возникла ошибка при экспорте данных, попробуйте позже", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                StreamWriter writer = new StreamWriter(fs, Encoding.Unicode);
                for (int i = 0, len = dt.Columns.Count - 1; i <= len; ++i)
                {
                    writer.Write(dt.Columns[i].ColumnName);
                    if (i != len)
                        writer.Write(";");
                }
                writer.Write("\n");
                foreach (DataRow dataRow in dt.Rows)
                {
                    string r = String.Join(";", dataRow.ItemArray);
                    writer.WriteLine(r);
                }
                writer.Close();
                MessageBox.Show("Выгружено " + dt.Rows.Count + " строк", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы действительно хотите произвести импорт из csv файла?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            if (dr == DialogResult.Yes)
            {
                StreamReader FILE = null;

                try
                {
                    string read = "";
                    string[] array;
                    try
                    {
                        FILE = new StreamReader("Кормление.csv");
                    }
                    catch
                    {
                        MessageBox.Show("Файл не найден", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int row = 0;
                    insert = "";
                    while ((read = FILE.ReadLine()) != null)
                    {
                        if (row == 0)
                        {
                            row++;
                            continue;
                        }
                        if (row > 1)
                        {
                            insert += ',';
                        }
                        array = read.Split(';');
                        insert += "((select max(id) from staff where iduser = (select id from users where fio = '" + array[1] + "')),(select id from animals where name='" + array[2] + "'),'" + array[3] + "',(select id from feed where name_food='" + array[4] + "'),'" + array[5].Replace(",", ".") + "','" + array[6] + "','" + array[7] + "','" + array[8] + "')";
                        row++;
                    }

                    if (Table.Request("INSERT INTO animal_feeding (idstaff,idanimal,nickname,idfeed,feed_quantity,time,date_feeding,status) VALUES " + insert))
                    {
                        load();
                        MessageBox.Show("Импорт произведён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                FILE.Close();
            }
        }
        private void Colors()
        {
            int i = 0;
            foreach (DataGridViewRow row in dGV1.Rows)
            {
                if (row.Cells[8].Value.ToString() == "Покормлен")
                    dGV1.Rows[i].Cells[8].Style.BackColor = Color.Green;
                else if (row.Cells[8].Value.ToString() == "Ожидается")
                    dGV1.Rows[i].Cells[8].Style.BackColor = Color.Yellow;
                else
                    dGV1.Rows[i].Cells[8].Style.BackColor = Color.Red;
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
        private void button4_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void Feeding_Load(object sender, EventArgs e)
        {
            load();
            UpdateUI();
            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker2.MaxDate = DateTime.Now;
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            groupBox1.Visible = false;
            backLabel.Enabled = false;
            foreach (DataGridViewColumn dgvc in dGV1.Columns)
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string condition = " where date_feeding >= '" + string.Join("-", dateTimePicker1.Value.ToString().Split(' ')[0].Split('.').Reverse()) + "' and date_feeding <= '" + string.Join("-", dateTimePicker2.Value.ToString().Split(' ')[0].Split('.').Reverse()) + "'";
            DataTable animals = new DataTable();
            DataTable animalsFeeding = new DataTable();
            animals = Table.Load("SELECT (select fio from users where id = (select iduser from staff where id=idstaff)) as 'Сотрудник',(select name from animals where id = idanimal) as 'Животное' FROM mydb.animal_feeding " + condition + " group by idstaff,idanimal");
            animalsFeeding = Table.Load(request + condition + "  order by date_feeding desc,time,(select name from animals where id = idanimal)");
            if (animals.Rows.Count == 0)
                return;
            if (animalsFeeding.Rows.Count == 0)
            {
                MessageBox.Show("За этот период никого не кормили","Внимание",MessageBoxButtons.OK);
                return;
            }

            Word.Application application = new Word.Application();
            Word.Document document = application.Documents.Add();
            document.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;
            document.PageSetup.TopMargin = application.InchesToPoints(0.4f);
            document.PageSetup.LeftMargin = application.InchesToPoints(0.4f);
            document.PageSetup.RightMargin = application.InchesToPoints(0.4f);
            document.PageSetup.BottomMargin = application.InchesToPoints(0.4f);
            foreach (DataRow Row in animals.Rows)
            {
                Word.Paragraph animalsPar = document.Paragraphs.Add();
                Word.Range animalsRange = animalsPar.Range;
                animalsRange.Font.Size = 14;
                animalsRange.Text = "Сотрудник: "+Row["Сотрудник"].ToString()+", Животное: "+Row["Животное"].ToString();
                animalsPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                animalsRange.Bold = 1;
                animalsRange.InsertParagraphAfter();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table paymentsTable = document.Tables.Add(tableRange, 1, 6);
                paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tableParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Word.Range cellRange;
                cellRange = paymentsTable.Cell(1, 1).Range;
                cellRange.Text = "Кличка животного";
                cellRange = paymentsTable.Cell(1, 2).Range;
                cellRange.Text = "Корм";
                cellRange = paymentsTable.Cell(1, 3).Range;
                cellRange.Text = "Количество корма";
                cellRange = paymentsTable.Cell(1, 4).Range;
                cellRange.Text = "Время кормления";
                cellRange = paymentsTable.Cell(1, 5).Range;
                cellRange.Text = "Дата кормления";
                cellRange = paymentsTable.Cell(1, 6).Range;
                cellRange.Text = "Статус";
                paymentsTable.Rows[1].Range.Bold = 1;
                paymentsTable.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                int y = 2;
                for (int i = 0; i < animalsFeeding.Rows.Count; i++)
                {
                    DataRow row = animalsFeeding.Rows[i];
                    if (row[2].ToString() == Row[1].ToString())
                    {
                        paymentsTable.Rows.Add();
                        paymentsTable.Rows[y].Range.Bold = 0;
                        cellRange = paymentsTable.Cell(y, 1).Range;
                        cellRange.Text = row[3].ToString();
                        cellRange = paymentsTable.Cell(y, 2).Range;
                        cellRange.Text = row[4].ToString();
                        cellRange = paymentsTable.Cell(y, 3).Range;
                        cellRange.Text = row[5].ToString();
                        cellRange = paymentsTable.Cell(y, 4).Range;
                        cellRange.Text = row[6].ToString();
                        cellRange = paymentsTable.Cell(y, 5).Range;
                        cellRange.Text = row[7].ToString();
                        cellRange = paymentsTable.Cell(y, 6).Range;
                        cellRange.Text = row[8].ToString();
                        y++;
                    }
                }
                if (animals.Rows.Count - 1 != animals.Rows.IndexOf(Row))
                {
                    document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }
            }
            application.Visible = true;
            try
            {
                document.SaveAs(Directory.GetCurrentDirectory() + @"\Кормление за период с " + string.Join("-", dateTimePicker1.Value.ToString().Split(' ')[0].Split('.').Reverse()) + " по " + string.Join("-", dateTimePicker2.Value.ToString().Split(' ')[0].Split('.').Reverse()) + ".docx");
            }
            catch
            {
                MessageBox.Show("Документ с таким названием и с такими же данными уже сущестует, можете выбрать другой период","Внимание",MessageBoxButtons.OK);
            }
            groupBox1.Visible = false;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            numberPage--;
            UpdateUI();
            load();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            numberPage++;
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
            label4.Text = numberPage + "/" + countPage;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            if (dateTimePicker1.Value > dateTimePicker2.Value)
                dateTimePicker2.Value = dateTimePicker1.Value;
        }
        int pK = 0;
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
            groupBox1.Visible = false;
            dGV1.DefaultCellStyle.SelectionBackColor = dGV1.Rows[rI].Cells["Статус"].Style.BackColor;
        }
    }
}
