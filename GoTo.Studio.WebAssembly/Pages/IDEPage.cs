using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace GoTo.Studio.Web.Pages
{
    public partial class IDEPage
    {
        private const string GoToVersion = "GoTo 1.0.3.0";
        private const string CopyXProgram = 
            "[A] X = X - 1\n" +
            "Y = Y + 1\n" +
            "IF X != 0 GOTO A";
        private const string Welcome =
            "Welcome to GoTo Studio!\n" +
            "\n" +
            "I'm the output, here you'll see the result of running those programs you write at my left —the editor.\n" +
            "\n" +
            "Do you see that column of entries at center? They're the inputs, waiting for you to type integers.\n" +
            "\n" +
            "Why don't you just start by typing 42 at X1 and click Run? Please, be patient as sometimes it takes some time to show up.\n" +
            "\n" +
            "Oh, if you may encounter any issue, please report it at\n" +
            "\n" +
            "    https://github.com/MarcosCobena/GoTo/issues\n" +
            "\n" +
            "Thanks in advance.";

        public IDEPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();
        }

        protected override void OnDisappearing()
        {
            _runButton.Clicked -= RunButton_Clicked;
            _helpButton.Clicked -= HelpButton_Clicked;

            base.OnDisappearing();
        }

        void Initialize()
        {
            _runButton.Clicked += RunButton_Clicked;
            _helpButton.Clicked += HelpButton_Clicked;

            _textEditor.Text = CopyXProgram;

            Help();
        }

        void HelpButton_Clicked(object sender, EventArgs e)
        {
            // This' currently not implemented
            //Device.OpenUri(new Uri("https://github.com/MarcosCobena/GoTo/wiki"));

            Help();
        }

        void RunButton_Clicked(object sender, EventArgs e)
        {
            Run();
        }

        void Run()
        {
            var x1 = int.TryParse(_x1Entry.Text, out int parsedX1) ? parsedX1 : 0;
            var x2 = int.TryParse(_x2Entry.Text, out int parsedX2) ? parsedX2 : 0;
            var x3 = int.TryParse(_x3Entry.Text, out int parsedX3) ? parsedX3 : 0;
            var x4 = int.TryParse(_x4Entry.Text, out int parsedX4) ? parsedX4 : 0;
            var x5 = int.TryParse(_x5Entry.Text, out int parsedX5) ? parsedX5 : 0;
            var x6 = int.TryParse(_x6Entry.Text, out int parsedX6) ? parsedX6 : 0;
            var x7 = int.TryParse(_x7Entry.Text, out int parsedX7) ? parsedX7 : 0;
            var x8 = int.TryParse(_x8Entry.Text, out int parsedX8) ? parsedX8 : 0;

            BlockUI();
            _runButton.Text = "Running...";

            var isSucceeded = Framework.TryRun(
                _textEditor.Text, 
                out int result,
                out IEnumerable<Message> messages,
                x1, 
                x2, 
                x3, 
                x4, 
                x5, 
                x6, 
                x7, 
                x8, 
                isInterpreted: true);

            _runButton.Text = "Run";
            BlockUI(false);

            if (!isSucceeded)
            {
                var errorMessages = messages
                    .Select(message =>
                        $"{message.Severity} at line {message.Line}, column {message.Column}: {message.Description}")
                    .Aggregate((current, next) => $"{current}\r\n{next}");
                Log(errorMessages);
            }
            else
            {
                Log($"Y = {result}", isSuccess: true);
            }
        }

        void BlockUI(bool isBlocked = true)
        {
            _runButton.IsEnabled = !isBlocked;
            _helpButton.IsEnabled = !isBlocked;
        }

        void Help()
        {
            _outputEditor.Text = string.Empty;
            _outputEditor.TextColor = Color.Black;

            var text = Welcome +
                "\n\n" +
                $"GoTo Studio (GoTo {GoToVersion})";
            _outputEditor.Text = text;
        }

        void Log(string message, bool isSuccess = false)
        {
            _outputEditor.TextColor = isSuccess ? 
                Color.Green : 
                Color.Red;
            _outputEditor.Text = message;
        }
    }
}
