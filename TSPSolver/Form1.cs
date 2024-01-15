using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TSPSolver
{
    public partial class Form1 : Form
    {

        public class City
        {
            public string NodeName { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public City(string nodeName, int x, int y)
            {
                NodeName = nodeName;
                X = x;
                Y = y;
            }
        }

        private List<Point> cities = new List<Point>();

        public Form1()
        {
            InitializeComponent();

            // Set up DataGridView columns
            dataGridView1.Columns.Add("NodeName", "NodeName");
            dataGridView1.Columns.Add("X", "X");
            dataGridView1.Columns.Add("Y", "Y");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Parse cities from the TextBox input
            ParseCities();

            GenerateNetworkMap();

            // Display the cities in the DataGridView
            DisplayCities();

            // Run Nearest Neighbor Algorithm
            List<int> nnTour = NearestNeighborAlgorithm();
            textBoxResult.Text = "Nearest Neighbor Tour: " + string.Join(" -> ", nnTour);

            // Run Nearest Insertion Algorithm
            List<int> niTour = NearestInsertionAlgorithm();
            textBoxResult.Text += Environment.NewLine + "Nearest Insertion Tour: " + string.Join(" -> ", niTour);
        }

        private void ParseCities()
        {
            cities.Clear();
            string[] lines = textBox1.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                {
                    cities.Add(item: new Point(x, y));
                }
                else
                {
                    MessageBox.Show("Invalid input format. Please use the format 'NodeName,X,Y' for each city.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void GenerateNetworkMap()
        {
           
        }

        private void DisplayCities()
        {
            dataGridView1.Rows.Clear();
            foreach (Point city in cities)
            {
                dataGridView1.Rows.Add("Node" + (dataGridView1.Rows.Count + 1), city.X, city.Y);
            }
        }

        private List<int> NearestNeighborAlgorithm()
        {
            // Nearest Neighbor Algorithm implementation
            int numCities = cities.Count;
            HashSet<int> unvisited = new HashSet<int>(Enumerable.Range(1, numCities - 1));
            List<int> tour = new List<int> { 0 }; // Starting from city 0

            while (unvisited.Count > 0)
            {
                int currentCity = tour[tour.Count - 1];
                int nearestCity = unvisited.OrderBy(city => Distance(cities[currentCity], cities[city])).First();
                tour.Add(nearestCity);
                unvisited.Remove(nearestCity);
            }

            // Return to the starting city
            tour.Add(tour[0]);
            return tour;
        }

        private List<int> NearestInsertionAlgorithm()
        {
            // Nearest Insertion Algorithm implementation
            int numCities = cities.Count;
            List<int> tour = new List<int> { 0 }; // Starting from city 0

            while (tour.Count < numCities + 1)
            {
                double minInsertCost = double.PositiveInfinity;
                int bestInsertIndex = 0;

                for (int i = 0; i < tour.Count - 1; i++)
                {
                    foreach (int j in Enumerable.Range(1, numCities - 1).Where(city => !tour.Contains(city)))
                    {
                        double cost = Distance(cities[tour[i]], cities[j]) +
                                      Distance(cities[j], cities[tour[i + 1]]) -
                                      Distance(cities[tour[i]], cities[tour[i + 1]]);
                        if (cost < minInsertCost)
                        {
                            minInsertCost = cost;
                            bestInsertIndex = i + 1;
                        }
                    }
                }

                tour.Insert(bestInsertIndex, bestInsertIndex);
            }

            return tour;
        }

        private double Distance(Point city1, Point city2)
        {
            // Euclidean distance between two cities
            return Math.Sqrt(Math.Pow(city1.X - city2.X, 2) + Math.Pow(city1.Y - city2.Y, 2));
        }
    }
}
