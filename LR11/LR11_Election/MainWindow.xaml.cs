using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LR11_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float Rating { get; set; }
        public ICollection<Confident> Confidents { get; set; }
        public ICollection<Promise> Promises { get; set; }
        public Candidate()
        {
            Confidents = new List<Confident>();
            Promises = new List<Promise>();
        }
        public void RatingChange(float rate)
        {
            if (rate <= 100 && rate >= 0)
            {
                this.Rating = rate;
            }
            else
            {
                MessageBox.Show("Incorect rate value!");
            }
        }
    }

    class Confident
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PoliticalPreferences { get; set; }
        public int Age { get; set; }
        public int? CandidateId { get; set; }
        public Candidate Candidate { get; set; }
    }

    class Promise
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<Candidate> Candidates { get; set; }
        public Promise()
        {
            Candidates = new List<Candidate>();
        }
    }

    class CandidateProfile
    {
        [Key]
        [ForeignKey("Candidate")]
        public int Id { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public virtual Candidate Candidate { get; set; }
    }
    class ElectionContext : DbContext
    {
        public ElectionContext() : base("DBConnection")
        {
        }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Promise> Promises { get; set; }
        public DbSet<Confident> Confidents { get; set; }
    }
    public partial class MainWindow : Window
    {
        private ElectionContext curCont;
        public MainWindow()
        {
            using (ElectionContext db = new ElectionContext())
            {
                Candidate c1 = new Candidate { Name = "Alex", Surname = "Lukashenko", Rating = 35 };
                Candidate c2 = new Candidate { Name = "Zianon", Surname = "Pozniak", Rating = 65 };
                Candidate c3 = new Candidate { Name = "Maria", Surname = "Vasilevish", Rating = 0 };
                db.Candidates.AddRange(new List<Candidate>() { c1, c2, c3 });
                Confident conf1 = new Confident { FullName = "Alex Matskevich", Age = 40, PoliticalPreferences = "Neutral", Candidate = c1 };
                Confident conf2 = new Confident { FullName = "Karyna Kluchnik", Age = 18, PoliticalPreferences = "Monarch", Candidate = c2 };
                Confident conf3 = new Confident { FullName = "Tanya Verbovich", Age = 25, PoliticalPreferences = "Liberal", Candidate = c2 };
                db.Confidents.AddRange(new List<Confident>() { conf1, conf2, conf3 });
                Promise prom1 = new Promise { Text = "Stop War" };
                Promise prom2 = new Promise { Text = "Increase revenue" };
                prom1.Candidates.Add(c1);
                prom2.Candidates.Add(c1);
                prom2.Candidates.Add(c2);
                db.Promises.AddRange(new List<Promise> { prom1, prom2 });
                db.SaveChanges();
                CandidateProfile prof1 = new CandidateProfile { Id = c1.Id, Age = 50, Description = "Current president" };
                CandidateProfile prof2 = new CandidateProfile { Id = c2.Id, Age = 40, Description = "Legend" };
                db.CandidateProfiles.AddRange(new List<CandidateProfile> { prof1, prof2 });
                db.SaveChanges();
            }
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            curCont = new ElectionContext();
            curCont.Candidates.Load();
            curCont.Confidents.Load();
            infoGrid.ItemsSource = curCont.Candidates.Local.ToBindingList();
            secGrid.ItemsSource = curCont.Confidents.Local.ToBindingList();
            this.Closing += Window_Closing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            curCont.Dispose();
        }

        private void infoGrid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            curCont.SaveChanges();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (infoGrid.SelectedItems != null)
                {
                    for (int i = 0; i < infoGrid.SelectedItems.Count; i++)
                    {
                        Candidate curCand = infoGrid.SelectedItems[i] as Candidate; if (curCand != null)
                        {
                            curCont.Candidates.Remove(curCand);
                        }
                    }
                }
                curCont.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getConfs_Click(object sender, RoutedEventArgs e)
        {
            var curCand = curCont.Candidates.Include(p => p.Confidents).ToList();
            string result = ""; 
            foreach (var c in curCand)
            {
                result += c.Name + " " + c.Surname + " \n" + "Confidents: "; foreach (var conf in c.Confidents)
                {
                    result += conf.FullName + " ";
                }
                result += "\n";
            }
            Console.WriteLine(result);
        }

        private void updRating_Click(object sender, RoutedEventArgs e)
        {
            using (ElectionContext db = new ElectionContext())
            {
                
                var result2 = db.Candidates.Join(db.Confidents, p => p.Id,
                c => c.CandidateId, (p, c) => new
                {
                    Name = c.FullName,
                    CandidateSurname = p.Surname
                }
                ).ToList();
                infoGrid.ItemsSource = result2;
                var result3 = db.Confidents.GroupBy(c => c.CandidateId).Select(g =>
              new { ID = g.FirstOrDefault().Id, AvgAge = g.Average(c => c.Age) }).Join(db.Candidates,
                a => a.ID,
                b => b.Id, (a, b) => new {
                    Name = b.Name,
                    Surname = b.Surname,
                    Age = a.AvgAge
                }
                ).ToList(); secGrid.ItemsSource = result3;
            }
        }
    }
}
