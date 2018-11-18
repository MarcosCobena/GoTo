using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace GoTo.Studio.Web.Pages
{
    public partial class IDEPage
    {
        private readonly string _goToVersion;

        public IDEPage()
        {
            InitializeComponent();

            _goToVersion = typeof(Language)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                .Version;
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

            _textEditor.Text = LoadEmbeddedResource("CopyX.goto");

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
            var isX1Parsed = int.TryParse(_x1Entry.Text, out int x1);
            var isX2Parsed = int.TryParse(_x2Entry.Text, out int x2);
            var isX3Parsed = int.TryParse(_x3Entry.Text, out int x3);
            var isX4Parsed = int.TryParse(_x4Entry.Text, out int x4);
            var isX5Parsed = int.TryParse(_x5Entry.Text, out int x5);
            var isX6Parsed = int.TryParse(_x6Entry.Text, out int x6);
            var isX7Parsed = int.TryParse(_x7Entry.Text, out int x7);
            var isX8Parsed = int.TryParse(_x8Entry.Text, out int x8);

            if (!(isX1Parsed && isX2Parsed && isX3Parsed && isX4Parsed && 
                isX5Parsed && isX6Parsed && isX7Parsed && isX8Parsed))
            {
                Log($"Every X input must be an integer");
                return;
            }

            (int result, IEnumerable<Message> messages) output;
            var isRunAborted = false;

            try
            {
                output = Language.Run(_textEditor.Text, x1, x2, x3, x4, x5, x6, x7, x8);
            }
            catch (Exception exception)
            {
                Log(exception.ToString());

                isRunAborted = true;
                output = (-1, null);
            }

            if (isRunAborted)
            {
                return;
            }

            var isFailed = output.messages.Any(message => message.Severity == SeverityEnum.Error);

            if (isFailed)
            {
                var errorMessages = output.messages
                    .Select(message =>
                        $"{message.Severity} at line {message.Line}, column {message.Column}: {message.Description}")
                    .Aggregate((current, next) => $"{current}\r\n{next}");
                Log(errorMessages);
            }
            else
            {
                Log($"Y = {output.result}", isSuccess: true);
            }
        }

        void Help()
        {
            _outputLabel.Text = string.Empty;
            _outputLabel.TextColor = Color.Black;

            var text = LoadEmbeddedResource("Welcome.txt") +
                "\n\n" +
                $"GoTo Studio (GoTo {_goToVersion})";
            _outputLabel.Text = text;
        }

        void Log(string message, bool isSuccess = false)
        {
            _outputLabel.TextColor = isSuccess ? 
                Color.Green : 
                Color.Red;
            _outputLabel.Text = message;
        }

        string LoadEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"GoTo.Studio.Web.Pages.{fileName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
