using System;
using System.IO;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Зоопарк
{
    class Table
    {
        public static DataTable Load(string queryString)
        {
            MySqlConnection con = new MySqlConnection(Connection.constr);
            DataTable table = new DataTable();
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, con);
                MySqlDataAdapter da = new MySqlDataAdapter(queryString, Connection.constr);
                da.Fill(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
            return table;
        }
        public static bool Request(string queryString)
        {
            bool b = true;
            MySqlConnection con = new MySqlConnection(Connection.constr);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                b = false;
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
            return b;
        }
        public static void Backup()
        {
            string path = Directory.GetCurrentDirectory()+"\\zoo " + DateTime.Now.ToString().Replace(":", "-") + ".sql";
            MySqlConnection con = new MySqlConnection(Connection.constr);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                MySqlBackup back = new MySqlBackup(cmd);
                cmd.Connection = con;
                back.ExportToFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить резервное копирование\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }
        public static void Restore(string File)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + File;
            MySqlConnection con = new MySqlConnection(Connection.constr);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                MySqlBackup back = new MySqlBackup(cmd);
                cmd.Connection = con;
                back.ImportFromFile(path);
                MessageBox.Show("База данных была успешно восстановлена", "", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить восстановление БД\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
