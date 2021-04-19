using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Configuration;


namespace LR9_3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataSet World = new DataSet("world");
        private string connStr = string.Empty;

        private MySqlDataAdapter cityAdap;
        private MySqlDataAdapter countryAdap;
        private MySqlDataAdapter countryLangAdap;
        public MainWindow()
        {
            InitializeComponent();

            connStr = ConfigurationManager.ConnectionStrings["world"].ConnectionString;

            cityAdap = new MySqlDataAdapter("select * from city", connStr);
            countryAdap = new MySqlDataAdapter("select * from country", connStr);
            countryLangAdap = new MySqlDataAdapter("select * from countrylanguage", connStr);

            cityAdap.Fill(World, "city");
            countryAdap.Fill(World, "country");
            countryLangAdap.Fill(World, "countrylanguage");

            var cityToCountry = new DataRelation("city_ibfk_1", World.Tables["country"].Columns["Code"],
                World.Tables["city"].Columns["CountryCode"]);
            var langToCountry = new DataRelation("countryLanguage_ibfk_0", World.Tables["country"].Columns["Code"],
                World.Tables["countrylanguage"].Columns["CountryCode"]);

            World.Relations.Add(cityToCountry);
            World.Relations.Add(langToCountry);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            infoGrid.ItemsSource = World.Tables["country"].DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Найти страны в которых код и название страны начинаются и заканчиваются одинаково");

            var query = from country in World.Tables["country"].AsEnumerable()
                        select new
                        {
                            Name = (string)country["Name"],
                            Code = (string)country["Code"]
                        } into Country
                        where Country.Name[0] == Country.Code[0] && Country.Name[Country.Name.Length - 1] == Char.ToLower(Country.Code[2])
                        select new { Name = Country.Name, Code = Country.Code };

            infoGrid.ItemsSource = query;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Сколько стран мира имеют ВПН превосходящий суммарный ВПН РБ и Украины");

            var total = (from country in World.Tables["country"].AsEnumerable()
                        where (string)country["Name"] == "Belarus" || (string)country["Name"] == "Ukraine"
                        select country.Field<float>("GNP")).Sum();
            var query = from country in World.Tables["country"].AsEnumerable()
                        where (float)country["GNP"] > total
                        select new { Name = country.Field<string>("Name"), GNP = country.Field<float>("GNP") };
            infoGrid.ItemsSource = query;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вывести города занимающие по населению позиции с 200 по 2034");
            var query = from city in World.Tables["city"].AsEnumerable()
                        orderby city["Population"]
                        select new {ID = city["ID"], Name = city["Name"], Pop = city["Population"] };
            var res = query.Skip(199).Take(1835);

            infoGrid.ItemsSource = res;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Кол-во городов в каждом регионе Европы");

            var query = from city in World.Tables["city"].AsEnumerable()
                        join country in World.Tables["country"].AsEnumerable()
                        on city["CountryCode"] equals country["Code"]
                        where (string)country["Continent"] == "Europe"
                        group new { country, city } by country["Region"] into g
                        select new { Region = g.Key, Count = g.Count() };

            infoGrid.ItemsSource = query;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Процентное распределение населения по континентам");
            var query = from country in World.Tables["country"].AsEnumerable()
                        select new
                        {
                            Continent = (string)country["Continent"],
                            Population = Convert.ToInt64((int)country["Population"])
                        } into conts
                        group conts by conts.Continent into c
                        let total = World.Tables["country"].AsEnumerable().Sum(x => Convert.ToInt64(x.Field<int>("Population")))
                        select new { Cont = c.Key, Pop = 100.0 * c.Sum(x => x.Population) / total };

            infoGrid.ItemsSource = query;
        }
    }
}
