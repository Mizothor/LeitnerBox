using System;
using System.Windows;
using System.IO;

//LeitnerBox is a spaced learning method that consists of 7 levels.
//Each level consists of cards on which are written questions and answers.
//Each day, you add new cards representing things you want to learn.
//Each day, you start going through the cards by levels from the highest level in that days available levels(by specific pattern, viz GetLevels).
//If a card is guessed correctly, it moves up in level, if not, it moves back to level 1. If guessed correctly in level 7, it gets deleted.
//The number of days before the card appears again circa doubles.
//At the end of the day, no card should be left in level 1, i.e. repeat all cards you guessed wrong until they move up a level.


namespace LeitnerBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Setup();
            InitializeComponent();
        }

        /// <summary>Checks if directory, config and level files exist, if not, creates them.</summary>
        private static void Setup()
        {
            if (!Directory.Exists(@".\Levels"))
            {
                Directory.CreateDirectory(@".\Levels");
            }

            for (int i = 1; i < 8; i++)
            {
                if (!File.Exists($@".\Levels\Level{i}.txt"))
                {
                    File.Create($@".\Levels\Level{i}.txt");
                }
            }

            if (!File.Exists(@".\Levels\Config.txt"))
            {
                var date = DateTime.Today.AddDays(-1).ToString("d");
                using (var writer = new StreamWriter(@".\Levels\Config.txt", false))
                {
                    writer.WriteLine(date);
                    writer.WriteLine("64");
                }
            }

            if (!File.Exists(@".\Levels\Temp.txt"))
            {
                File.Create($@".\Levels\Temp.txt");
            }
        }


        /// <summary>Checks if player added any new cards or if they started playing already today, in which case it doesn't allow for replaying.</summary>
        static bool CanYouPlay()
        {
            var today = DateTime.Today;
            string[] lastPlayed = File.ReadAllLines(@".\Levels\Config.txt");

            if (new FileInfo(@".\Levels\Level1.txt").Length == 0) //Can't play without any new cards
            {
                MessageBox.Show("You didn't add any new cards today.\nGo add some before playing.");
                return false;
            }
            else if (DateTime.Parse(lastPlayed[0]) < today)
            {
                using (StreamWriter sw = new StreamWriter(@".\Levels\Config.txt"))
                {
                    sw.WriteLine(today.ToString("d"));
                    if (lastPlayed[1] == "64")  //Resets pattern counter back to 1
                    {
                        sw.WriteLine(1);
                    }
                    else
                    {
                        sw.WriteLine(int.Parse(lastPlayed[1]) + 1);
                    }
                }
                return true;
            }
            else //If player clicked Read Cards button and closed app or clicked on Add Cards, it won't allow him to start again and finish, need to fix later
            {
                MessageBox.Show("You already finished today,\ncome back tomorrow.");
                return false;
            }
        }

        private void AddCardsClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new AddCardsPage();
        }

        private void ReadCardsClick(object sender, RoutedEventArgs e)
        {
            if (CanYouPlay())
            {
                Main.Content = new ReadCardsPage();
            }
        }
    }
}