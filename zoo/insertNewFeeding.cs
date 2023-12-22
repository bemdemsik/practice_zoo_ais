using System;
namespace Зоопарк
{
    class insertNewFeeding
    {
        public static void Check()
        {
            string day = "";
            int daay = (int)DateTime.Now.DayOfWeek;
            switch (daay)
            {
                case 1: day = "пн"; break;
                case 2: day = "вт"; break;
                case 3: day = "ср"; break;
                case 4: day = "чт"; break;
                case 5: day = "пт"; break;
                case 6: day = "сб"; break;
                case 7: day = "вс"; break;
            }
            string[] animals = Completion.Item("select idanimals from feeding_schedule where idanimals not in (select idanimal from animal_feeding where date_feeding=CURDATE()) and idanimals in (select idanimals from diet where days LIKE '%" + day + "%') and idanimals in (select idanimal from staff)", 0, 0);
            for (int i = 0; i < animals.Length; i++)
            {
                string feed;
                string[] nick;
                string idstaff;
                string insert = "";
                try
                {
                    feed = Completion.Item("select idfeed from diet where idanimals=" + animals[i] + " and days LIKE '%" + day + "%'", 0, 0)[0];
                }
                catch
                {
                    continue;
                }
                if ((nick = Completion.Item("select nickname from animalsnick where idanimals=" + animals[i], 0, 0)).Length == 0 || feed == "")
                    continue;

                for (int j = 0; j < nick.Length; j++)
                {
                    if (j > 0)
                    {
                        insert += ",";
                    }
                    double weight = Convert.ToDouble(Completion.Item("select weight from animalsnick where nickname='" + nick[j] + "'", 0, 0)[0]);
                    idstaff = Completion.Item("select id from staff where idanimal = " + animals[i], 0, 0)[0];
                    insert += "(" + idstaff + "," + animals[i] + ",'" + nick[j] + "'," + feed + ",'" + Math.Round((weight * 0.01), 2).ToString().Replace(",", ".") + "','Утреннее',CURDATE(),'Ожидается'),(" + idstaff + "," + animals[i] + ",'" + nick[j] + "'," + feed + ",'" + Math.Round((weight * 0.01), 2).ToString().Replace(",", ".") + "','Вечернее',CURDATE(),'Ожидается')";
                }
                if (insert == "")
                    continue;
                Table.Request("INSERT INTO animal_feeding (idstaff,idanimal,nickname,idfeed,feed_quantity,time,date_feeding,status) VALUES " + insert);
            }
        }
    }
}
