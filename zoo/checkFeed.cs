namespace Зоопарк
{
    class checkFeed
    {
        public static bool Check()
        {
            string[] idfeed = Completion.Item("select id from feed", 0, 0);
            for (int i = 0; i < idfeed.Length; i++)
            {
                if (Completion.Item("select count(*) from animalsnick where (select sum(weight) from animalsnick where idanimals in (select idanimals from diet where idfeed = " + idfeed[i] + "))*0.03 > (select remainder from feed where id = " + idfeed[i] + ")", 0, 0)[0] != "0")
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Check(string feed)
        {
            if (Completion.Item("select count(*) from animalsnick where (select sum(weight) from animalsnick where idanimals in (select idanimals from diet where idfeed = (select id from feed where name_food='" + feed + "')))*0.01 > (select remainder from feed where id = (select id from feed where name_food='" + feed + "'))", 0, 0)[0] != "0")
            {
                return false;
            }
            return true;
        }
    }
}
