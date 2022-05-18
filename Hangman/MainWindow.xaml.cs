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
using System.IO;
using System.Collections;

namespace Hangman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Button> buttons;
        List<TextBlock> letterField;
        string word;
        int imageNumber = 0;
        string[] guessedWord;
        bool isWin = false;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        // function for images
        private void LoadImage(int imageNumber)
        {
            var hangmanIMG = img_hangman as Image;
            hangmanIMG.Source = new BitmapImage(new Uri(@"/resources/images/hangman" + imageNumber + ".png", UriKind.Relative));
        }

        //function play
        private void btn_show_Click(object sender, RoutedEventArgs e)
        {
            isWin = false;
            imageNumber = 0;
            word = SelectWord().ToLower();
            word = word.Replace("\r", ""); // each word after using method split has assign \r at the end, so this statment remove it
            guessedWord = new string[word.Length]; // initialize arrys's size accurated to the lenght of selected word
            CreateKeys();
            CreateWordSpace(word);
            LoadImage(imageNumber);
        }

        //function which choice world from text file and retun selected word to string
        private string SelectWord()
        {
            List<string> wordsList = new();
            Random rNumber = new();

            string words = System.IO.File.ReadAllText("words.txt");
            string[] word = words.Split("\n");

            for (int i = 0; i < word.Length; i++)
            {
                wordsList.Add(word[i]);
            }

            return wordsList[rNumber.Next(0, 20)];
        }

        //function which create keyboard for player
        private void CreateKeys()
        {
            //cleaning stack-panel for keyboard
            st_keysFirst.Children.Clear();
            st_keysSecond.Children.Clear();
            st_keysThird.Children.Clear();
            buttons = new(); //list with buttons

            //keyboard is creating by using ASCII numbers as content in buttons
            for (int charInt = 65; charInt < 91; charInt++)
            {
                Button keys = new()
                {
                    Content = ((char)charInt).ToString(),
                    Height = 50,
                    Width = 50,
                    FontSize = 12,
                    Margin = new Thickness(3)
                };
                keys.Click += btn_keys_Click;
                buttons.Add(keys);

                if (charInt % 65 < 8) st_keysFirst.Children.Add(keys);
                else if (charInt % 65 >= 8 && charInt % 65 < 16) st_keysSecond.Children.Add(keys);
                else st_keysThird.Children.Add(keys);
            }
        }

        //function which check if letter belongs to button is exist in word
        private void btn_keys_Click(object sender, RoutedEventArgs e)
        {
            var clickedBtn = e.Source as Button;
            string selectedLetter = clickedBtn.Content.ToString().ToLower();

            if (word.ToString().Contains(selectedLetter))
            {
                for (int counter = 0; counter < word.Length; counter++)
                {
                    if (selectedLetter == word[counter].ToString())
                    {
                        letterField[counter].Text = selectedLetter;
                        clickedBtn.IsEnabled = false;
                        check(counter, selectedLetter);   
                    }
                }
            }
            else
            {  
                clickedBtn.IsEnabled = false;
                imageNumber++;
                if (imageNumber < 6) LoadImage(imageNumber);
                else LostOrWin(isWin); LoadImage(imageNumber);

            } 
        }

        //function which create dash line instead letters
        private void CreateWordSpace(string word)
        {
            st_word.Children.Clear();
            letterField = new(); //list with textblocks

            for (int counter = 0; counter < word.Length; counter++)
            {
                TextBlock letters = new()
                {
                    Text = "_",
                    FontSize = 20,
                    Margin = new Thickness(10),
                    FontWeight = FontWeights.Bold
                };
                letterField.Add(letters);
                st_word.Children.Add(letters);
            }
        }

        //function which occure when user lost and display correct word
        private void LostOrWin(bool isWin)
        {
            st_keysFirst.Children.Clear();
            st_keysSecond.Children.Clear();
            st_keysThird.Children.Clear();
            st_word.Children.Clear();

            if (!isWin)
            {
                TextBlock lost = new()
                {
                    Text = "You LOST! Try again by clicking Play...",
                    FontSize = 28,
                    FontWeight = FontWeights.Bold
                };
                st_keysThird.Children.Add(lost);
            }
            else
            {
                TextBlock win = new()
                {
                    Text = "You WIN! Try again by clicking Play...",
                    FontSize = 28,
                    FontWeight = FontWeights.Bold
                };
                st_keysThird.Children.Add(win);
            }

            for (int counter = 0; counter < word.Length; counter++)
            {
                TextBlock letters = new()
                {
                    Text = word[counter].ToString(),
                    FontSize = 20,
                    Margin = new Thickness(10),
                    FontWeight = FontWeights.Bold
                };
                st_word.Children.Add(letters);
            }
        }

        //function which check if guessed word is the same with selected word
        private void check(int counter, string selectedLetter)
        {
            guessedWord[counter] += selectedLetter;
            if (String.Concat(guessedWord) == word)
            {
                isWin = true;
                LostOrWin(isWin);
            }
        }
    }
}
