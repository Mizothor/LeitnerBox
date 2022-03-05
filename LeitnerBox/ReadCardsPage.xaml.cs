using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace LeitnerBox
{
    /// <summary>
    /// Interaction logic for ReadCardsPage.xaml
    /// </summary>
    public partial class ReadCardsPage : Page
    {
        static private List<string> levels;
        static private List<string> cards;
        static private string[] currentCard;

        public ReadCardsPage()
        {
            InitializeComponent();
            GetLevels();
            RemoveEmptyLevels();
            ReadCards();
        }

        /// <summary>Checks if there are any levels to go through left. If yes, fetches their cards. If not, ends todays reading of cards.</summary>
        private void ReadCards()
        {
            if (levels.Count > 0)
            {
                cards = new List<String>(File.ReadLines($@".\Levels\Level{levels[0]}.txt"));
                DayBlock.Text = ($"Questions from Level {levels[0]}");
                AskQuestion();
            }
            else if (new FileInfo($@".\Levels\Temp.txt").Length != 0) //Checks if there are any day 1 wrongly guessed cards in temporary holder file
            {
                Day1Loop();

            }
            else
            {
                MessageBox.Show("Congratulations, you finished today!");
                QuestionBlock.Text = "";
                SubmitButton.IsEnabled = false;
            }
        }

        /// <summary>Picks a random question from list of questions and displays it.</summary>
        private void AskQuestion()
        {
            var rand = new Random();
            int randomCard = rand.Next(0, cards.Count - 1);
            currentCard = cards[randomCard].Split(';');
            QuestionBlock.Text = currentCard[0];
        }

        /// <summary>Adds descending levels to go through today to list of levels.</summary>
        private void GetLevels()
        {
            //Gets pattern number of today
            int today = int.Parse(File.ReadAllLines(@".\Levels\Config.txt")[1]);

            //The used pattern should allow each card circa the same time to go through all 7 levels
            levels = new List<string>();
            if (today == 56)
            {
                levels.Add("7");
            }
            if (today == 24 || today == 59)
            {
                levels.Add("6");
            }
            if (new[] { 12, 28, 44, 60 }.Contains(today))
            {
                levels.Add("5");
            }
            if (new[] { 4, 13, 20, 29, 36, 45, 52, 61 }.Contains(today))
            {
                levels.Add("4");
            }
            if (today % 4 == 2)
            {
                levels.Add("3");
            }
            if (today % 2 == 1)
            {
                levels.Add("2");
            }
            levels.Add("1");
        }

        /// <summary>Checks if any of the levels is empty, if it is, it gets removed from list of levels.</summary>
        private void RemoveEmptyLevels()
        {
            for (int i = levels.Count - 1; i >= 0; i--)
            {
                if (new FileInfo($@".\Levels\Level{levels[i]}.txt").Length == 0)
                {
                    levels.Remove(levels[i].ToString());
                }
            }
        }

        /// <summary>Clears the fields in window, removes asked card from list of cards to ask and if there are any left, asks next card.</summary>
        private void NextCard()
        {
            CorrectAnswerBlock.Text = "";
            AnswerInputField.Clear();
            GuessedCorrectly.Visibility = Visibility.Hidden;
            GuessYesNoPanel.Visibility = Visibility.Hidden;
            cards.Remove($"{currentCard[0]};{currentCard[1]}");
            if (cards.Count != 0)
            {
                AskQuestion();
            }
            else //If there are no more cards to ask, clears the file and removes the level from list of remaining levels
            {
                File.WriteAllText($@".\Levels\Level{levels[0]}.txt", String.Empty);
                levels.RemoveAt(0);
                ReadCards();
            }
        }

        /// <summary>Loop for solving guessing question in day 1 wrong.</summary>
        private void Day1Loop()
        {
            File.Copy($@".\Levels\Temp.txt", $@".\Levels\Level1.txt", true);
            File.WriteAllText($@".\Levels\Temp.txt", String.Empty);
            levels.Add("1");
            ReadCards();
        }


        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            CorrectAnswerBlock.Text = currentCard[1];
            GuessedCorrectly.Visibility = Visibility.Visible;
            GuessYesNoPanel.Visibility = Visibility.Visible;
        }


        private void GuessYesClick(object sender, RoutedEventArgs e)
        {
            if (!levels[0].Equals("7")) //Guessing correctly below level 7 moves the card to next level
            {
                using (StreamWriter writer = File.AppendText($@".\Levels\Level{int.Parse(levels[0]) + 1}.txt"))
                {
                    writer.WriteLine($"{currentCard[0]};{currentCard[1]}");
                }
            }
            NextCard();
        }


        private void GuessNoClick(object sender, RoutedEventArgs e)
        {
            if (levels[0].Equals("1"))
            {
                using (StreamWriter writer = File.AppendText(@".\Levels\Temp.txt")) //If question is in Level 1 and user guessed wrong, the question gets moved to temp file, to avoid deletion
                {
                    writer.WriteLine($"{currentCard[0]};{currentCard[1]}");
                }
            }
            else
            {
                using (StreamWriter writer = File.AppendText(@".\Levels\Level1.txt")) //Guessing wrong moves the card back to Level 1
                {
                    writer.WriteLine($"{currentCard[0]};{currentCard[1]}");
                }
            }
            NextCard();
        }
    }
}