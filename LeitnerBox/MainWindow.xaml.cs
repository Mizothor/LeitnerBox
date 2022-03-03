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

namespace LeitnerBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}


/*
    public class Program
    {
        //LeitnerBox is a spaced learning method that consists of 7 levels.
        //Each level consists of cards on which are written questions and on the opposite side answers.
        //Each day, you add new cards representing things you want to learn.
        //Each day, you start going through the cards in levels from the highest level in that days available levels(by specific algorithm, viz GetLevels).
        //If card is guessed correctly, it moves up a level, if not, it moves back at start to level 1. If guessed correctly in level 7, it gets deleted.
        //The main idea is if you guess correctly, the time before repeating the question will double
        //At the end of the day, no card should be left in level 1, aka repeat cards you got wrong until they move up a level.
        static string day1 = @".\Days\Day1.txt";
        static string tempDay1 = @".\Days\TempDay1.txt";

        static void Main(string[] args)
        {
            AddOrRead();
            Console.WriteLine("Congrats on finishing today");

        }

        static void AddOrRead()
        {
            Console.WriteLine("Do you want to Add new card or Read cards today? Add/Read/Exit");
            string playerInput = Console.ReadLine();
            if (string.Equals(playerInput, "add", StringComparison.OrdinalIgnoreCase))
            {
                bool addAnother = true;
                while (addAnother)
                {
                    AddNewCard();
                    Console.WriteLine("Do you want to add another card? y/n");
                    string another = Console.ReadLine();
                    if (another == "y")
                    {
                        continue;
                    }
                    else
                    {
                        addAnother = false;
                    }
                }
            }
            else if (string.Equals(playerInput, "read", StringComparison.OrdinalIgnoreCase))
            {
                if (CanYouPlay())
                {
                    ReadCards();
                }
                else 
                Environment.Exit(Environment.ExitCode);

            }
            else
            {
                Environment.Exit(Environment.ExitCode);
            }
        }

        static void AddNewCard()
        {
            //Adds new card to Level 1
            (string question, string answer) = GetInput();
            using (StreamWriter writer = new StreamWriter(day1, true))
            {
                writer.WriteLine(question+";"+answer);
            }
        }

        static void ReadCards()
        {
            List<string> levels = GetLevels();
            Random rand = new Random();
            foreach (string level in levels)
            {
                int intLevel = int.Parse(level);
                Console.WriteLine($"\nQuestions from Level {intLevel}\n");
                List<string> lines = new List<String>(File.ReadLines(@$".\Days\Day{intLevel}.txt"));

                while (lines.Count > 0)
                {
                    int line = rand.Next(0, lines.Count - 1);
                    bool result = AskQuestion(lines[line]);
                    WriteInFile(result, intLevel, lines[line]);
                    lines.RemoveAt(line);
                }
                ClearFile(@$".\Days\Day{intLevel}.txt");
            }
            Day1Loop();
        }

        static bool AskQuestion(string line)
        {
            string[] qa = line.Split(";");
            Console.Write($"{qa[0]}\nYour answer: ");
            string input = Console.ReadLine();
            Console.WriteLine($"\nThe answer is: \n{qa[1]}\n");
            Console.WriteLine("Was your answer correct? Yes/No");
            string correct = Console.ReadLine();
            Console.WriteLine("--------------------------------------------");
            if (string.Equals(correct, "yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        static (string, string) GetInput()
        {
            Console.WriteLine("Enter Question: ");
            string question = Console.ReadLine();
            Console.WriteLine("Enter Answer: ");
            string answer = Console.ReadLine();
            return (question, answer);
        }

        static List<string> GetLevels()
        {
            //Gets counter number of today
            int today = int.Parse(File.ReadAllLines(@".\Days\Today.txt")[1]);

            //Creates a list of levels we will go through today
            List<string> levels = new List<string>();
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
            return levels;
        }

        static void Day1Loop()
        {
            while (new FileInfo(tempDay1).Length > 6)
            {
                File.Copy(tempDay1, day1, true);
                ClearFile(tempDay1);
                string[] day1Lines = File.ReadAllLines(day1);
                foreach (string day1Line in day1Lines)
                {
                    bool result = AskQuestion(day1Line);
                    WriteInFile(result, 1, day1Line);

                }
            }
            ClearFile(day1);
        }

        static void ClearFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(@$"{path}"))
            {
                writer.Write("");
            }
        }

        static void WriteInFile(bool result, int level, string line)
        {
            if (result && (level != 7))
            {
                using (StreamWriter writerNext = new StreamWriter($@".\Days\Day{level + 1}.txt", true))
                {
                    writerNext.WriteLine(line);
                }
            }
            else if (!result && (level == 1))
            {
                using (StreamWriter writerTemp = new StreamWriter(tempDay1, true))
                {
                    writerTemp.WriteLine(line);
                }
            }
            else if (!result)
            {
                using (StreamWriter writerStart = new StreamWriter(day1, true))
                {
                    writerStart.WriteLine(line);
                }
            }
        }

        static bool CanYouPlay()
        {
            DateTime today = DateTime.Today;
            string[] lastPlayed = File.ReadAllLines(@".\Days\Today.txt");

            if (DateTime.Parse(lastPlayed[0]) < today)
            {
                using (StreamWriter sw = new StreamWriter(@".\Days\Today.txt"))
                {
                    sw.WriteLine(today.ToString("d"));
                    if (lastPlayed[1] == "64")
                    {
                        sw.Write(1);
                    }
                    else
                    { 
                        sw.Write(int.Parse(lastPlayed[1]) + 1);
                    }
                }
                Console.WriteLine("Okay, Lets play");
                return true;
            }
            else
            {
                Console.WriteLine("You already played today, come back tomorrow. ");
                return false;
            }
        }
    }
*/