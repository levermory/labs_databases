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
            MySqlCommand query = new MySqlCommand("select name as name1, name as nam2 from `match`, `user` where `user`.id=`match`.id1 and `user`.id=`match`.id2", connection);

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
        public void UpdateTable(string id, string col, string new_data)
        {
            MySqlCommand query = new MySqlCommand($"update `user` set {col}='{new_data}' where id={id}", connection);
            query.ExecuteNonQuery();
        }
        public void CreateUser(string id)
        {
            MySqlCommand query = new MySqlCommand($"insert into `user` (id) value ({id})", connection);
            query.ExecuteNonQuery();
        }
        public void CreateUser(User newUser)
        {
            MySqlCommand query = new MySqlCommand($"insert into `user` () values ('{newUser._id}','{newUser._name}','{newUser._age}','{newUser._sex}')", connection);
            query.ExecuteNonQuery();
        }

    }
    class User
    {
        public int _id { get; set; }
        public string _name { get; set; }
        public int _age { get; set; }
        public string _sex { get; set; }
        public User(int id, string name, int age, string sex)
        {
            _id = id;
            _name = name;
            _age = age;
            _sex = sex;
        }
        public User(string[] row)
        {
            _id = int.Parse(row[0]);
            _name = row[1];
            _age = int.Parse(row[2]);
            _sex = row[3];
        }

    }
    class Match
    {
        int _id { get; set; }
        int _us1_id { get; set; }
        int _us2_id { get; set; }
        public Match(int id, int us1_id, int us2_id)
        {
            _id = id;
            _us1_id = us1_id;
            _us2_id = us2_id;
        }
    }
}
