using System.Collections.Generic;
using MySqlConnector;
using System.Configuration;
using System.Data;

namespace LR8
{
    class DAL
    {
        private MySqlConnection _connection = null;
        private readonly string _connectionString;

        public DAL(string connString)
        {
            _connectionString = connString;
        }
        public DAL() : this(ConfigurationManager.ConnectionStrings["exams"].ConnectionString)
        {}

        public void OpenConnection()
        {
            _connection = new MySqlConnection(this._connectionString); 
            _connection.Open();
            
        }

        public void CloseConnection()
        {
            if(_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            string command = "select * from student";
            this.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(command, _connection);
            using (MySqlDataReader curRead = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (curRead.Read())
                {
                    students.Add(new Student
                    {
                        _id = (int)curRead["id"],
                        name = (string)curRead["name"],
                        spec = (string)curRead["spec"],
                        form = (string)curRead["form"],
                        value = (float)curRead["value"],
                        salary = (float)curRead["salary"],
                        course = (sbyte)curRead["course"]
                    });
                }
            }
            return students;
        }
        public void updateStudent(Student stud)
        {
            string cmd = $"update student set salary = {stud.salary + 20} where id={stud._id}";
            OpenConnection();
            var updCommand = new MySqlCommand(cmd, _connection);
            int res = updCommand.ExecuteNonQuery();
            CloseConnection();
        }
        public DataTable showExams()
        {
            OpenConnection();
            var cmd = new MySqlCommand("select " +
                "student.name, " +
                "subject.name," +
                "exam.date, " +
                "exam.mark, " +
                "subject.teacher " +
                "from student, exam, subject " +
                "where " +
                "student.id = exam.student_id " +
                "and " +
                "subject.id = exam.subject_id", _connection);
            var resTable = new DataTable();
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                resTable.Load(dr);
            }
            CloseConnection();
            return resTable;
        }
    }

    public class Student
    {
        public int _id {get; set;}
        public string name {get; set;}
        public string spec {get; set;}
        public string form {get; set;}
        public float value {get; set;}
        public float salary { get; set;}
        public sbyte course {get; set;}
    }

    public class Subject
    {
        public int _id {get; set;}
        public string name {get; set;}
        public string teacher {get; set;}
        public int hours {get; set;}
    }

    public class Exam
    {
        public int _id {get; set;}
        public string date {get; set;}
        public int subject_id {get; set;}
        public int student_id {get; set;}
        public int mark {get; set;}
    }

    
}
