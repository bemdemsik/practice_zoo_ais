using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Зоопарк
{
    class Completion
    {
        public static string[] Item(string queryString, int K, int N)
        {
            string[] Item1 = new string[0];
            try
            {
                MySqlConnection con = new MySqlConnection(Connection.constr);
                con.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, con);
                MySqlDataReader rdr = cmd.ExecuteReader();              
                while (rdr.Read())
                {
                    if (K == 0)
                    {
                        Array.Resize(ref Item1, Item1.Length + 1);
                        Item1[Item1.Length - 1] = rdr.GetString(N);
                    }
                    else
                    {
                        try
                        {
                            for (int i = 0; ; i++)
                            {
                                Array.Resize(ref Item1, Item1.Length + 1);
                                Item1[i] = rdr.GetString(N++);
                            }
                        }
                        catch
                        {
                            return Item1;
                        }
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Item1;
        }
    }
}
