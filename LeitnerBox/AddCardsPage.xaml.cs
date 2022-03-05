using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace LeitnerBox
{
    /// <summary>
    /// Interaction logic for AddCardsPage.xaml
    /// </summary>
    public partial class AddCardsPage : Page
    {
        public AddCardsPage()
        {
            InitializeComponent();
        }

        ///<summary>Adds new flash card to Level 1</summary>
        private void AddCardClick(object sender, RoutedEventArgs e)
        {
            (string question, string answer) = (NewQuestionInputField.Text, NewAnswerInputField.Text);
            using (StreamWriter writer = File.AppendText(@".\Levels\Level1.txt"))
            {
                if (question != "" && answer != "")
                {
                    writer.WriteLine($"{question};{answer}");
                    MessageBox.Show("Flash card added successfully!");
                    NewQuestionInputField.Clear();
                    NewAnswerInputField.Clear();
                }
                else 
                {
                    MessageBox.Show("Question or answer field was left empty. \nCard not saved.");
                }
            }
        }
    }
}
