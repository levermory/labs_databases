using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;


namespace WpfApp1
{
    class DAL
    {
        private MySqlConnection connection = null;

        public void OpenConnection(string connString)
        {
            connection = new MySqlConnection(connString);
            connection.Open();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public DataTable GetUsers()
        {
            MySqlCommand query = new MySqlCommand("select * from `user`", connection);
           
            DataTable restable = new DataTable();
            using (MySqlDataReader dr = query.ExecuteReader())
            {
                restable.Load(dr);
            }
            return restable;
        }
        public DataTable GetMatches()
        {
            MySqlCommand query = new MySqlCommand("select name, name from `match`, `user` where `user`.id=`match`.us1_id and `user`.id=`match`.us2_id", connection);

            DataTable restable = new DataTable();
            using (MySqlDataReader dr = query.ExecuteReader())
            {
                restable.Load(dr);
            }
            return restable;
        }
        public void DeleteRow(string id)
        {
            MySqlCommand query = new MySqlCommand($"delete from `user` where id={id}", connection);
            
            query.ExecuteNonQuery();
            
        }
        
    }
    class User
    {
        public int _id;
        public string _name;
        public int _age;
        public string _sex;
        public User(int id, string name, int age, string sex)
        {
            _id = id;
            _name = name;
            _age = age;
            _sex = sex;
        }

    }
    class Match
    {
        int _id;
        int _us1_id;
        int _us2_id;
        public Match(int id, int us1_id, int us2_id)
        {
            _id = id;
            _us1_id = us1_id;
            _us2_id = us2_id;
        }
    }
}
