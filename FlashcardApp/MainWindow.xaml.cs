using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace FlashcardApp
{
    public partial class MainWindow : Window
    {
        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Flashcards");
        string[] txtFiles;
        string CurrentQuote;
        string BlurredQuote;
        private List<string> BlurredWords;
        int count = 0;
        int correctCount = 0;
        bool Checked=false;
        string currentFileName = "";
        string currentFile = "";

        public MainWindow()
        {
            InitializeComponent();
            NextButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public void NextQuote()
        {
            txtFiles = Directory.GetFiles(folderPath, "*.txt");
            Random random = new Random();
            currentFile = txtFiles[random.Next(txtFiles.Length)];
            currentFileName = Path.GetFileNameWithoutExtension(Path.GetFileName(currentFile));
            string[] randomFileContent = File.ReadAllLines(currentFile);
            string randomLine = randomFileContent[random.Next(randomFileContent.Length)];
            CurrentQuote = randomLine;

            // Split the quote into words
            string[] words = CurrentQuote.Split(' ');

            int numberOfWords = words.Length;

            // Pick a random number from 2 to numberOfWords / 2
            int numberOfBlurredWords;
            if (numberOfWords > 2)
            {
                // Pick a random number from 2 to numberOfWords / 3 + 1
                numberOfBlurredWords = random.Next(2, numberOfWords / 3 + 1);
            }
            else
            {
                numberOfBlurredWords = 1;
            }

            // Initialize the list to store blurred words
            BlurredWords = new List<string>();

            // Pick random words and replace them with underscores
            List<int> indexesToBlur = new List<int>();
            for (int i = 0; i < numberOfBlurredWords; i++)
            {
                int randomWordIndex = random.Next(0, numberOfWords);
                indexesToBlur.Add(randomWordIndex);
            }

            StringBuilder blurredQuote = new StringBuilder();
            for (int i = 0; i < numberOfWords; i++)
            {
                if (indexesToBlur.Contains(i))
                {
                    BlurredWords.Add(words[i]); // Add the blurred word to the list
                    blurredQuote.Append(new string('_', words[i].Length));
                }
                else
                {
                    blurredQuote.Append(words[i]);
                }

                // Add space between words
                blurredQuote.Append(" ");
            }

            // Trim the trailing space
            blurredQuote.Length--;

            BlurredQuote = blurredQuote.ToString();
            string splitQuote = SplitTextIntoLines(BlurredQuote, 40);
            Flashcard.Content = splitQuote;
        }

        public void Check()
        {
            if (Checked == false)
            {
                count = count + 1;
            }

            if (BlurredWords != null)
            {
                bool temp = true;
                foreach (string i in BlurredWords)
                {
                    string cleanedString = new string(i.Where(char.IsLetterOrDigit).ToArray());
                    if (!Guessbox.Text.ToLower().Contains(cleanedString.ToLower()))
                    {
                        temp = false;
                    }
                }

                if (temp == true)
                {
                    correctCount = correctCount + 1;
                }
            }
        }

        private string SplitTextIntoLines(string text, int charactersPerLine)
        {
            StringBuilder result = new StringBuilder();
            string[] words = text?.Split(' ') ?? Array.Empty<string>();

            int currentLineLength = 0;

            foreach (string word in words)
            {
                if (currentLineLength + word.Length > charactersPerLine)
                {
                    result.AppendLine();
                    currentLineLength = 0;
                }

                result.Append(word + ' ');
                currentLineLength += word.Length + 1;
            }

            return result.ToString().Trim();
        }

        private void Guessbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Checked == false)
                {
                    Flashcard.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                else {
                    NextButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button clickedButton)
            {
                if (clickedButton.Name == "NextButton")
                {
                    Check();
                    Checked = false;
                    NextQuote();
                    filename.Text = currentFileName;
                    ScoreBoard.Text = correctCount.ToString() + "/" + count.ToString();
                    Guessbox.Text = "";
                }
                else if (clickedButton.Name == "Flashcard")
                {
                    Check();
                    Checked = true;
                    ScoreBoard.Text = correctCount.ToString() + "/" + count.ToString();
                    string splitQuote = SplitTextIntoLines(CurrentQuote, 40);
                    Flashcard.Content = splitQuote;
                    Guessbox.Text = "";
                }
            }
        }
    }
}
