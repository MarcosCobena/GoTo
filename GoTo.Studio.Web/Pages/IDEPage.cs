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
            var x1 = int.TryParse(_x1Entry.Text, out int parsedX1) ? parsedX1 : 0;
            var x2 = int.TryParse(_x2Entry.Text, out int parsedX2) ? parsedX2 : 0;
            var x3 = int.TryParse(_x3Entry.Text, out int parsedX3) ? parsedX3 : 0;
            var x4 = int.TryParse(_x4Entry.Text, out int parsedX4) ? parsedX4 : 0;
            var x5 = int.TryParse(_x5Entry.Text, out int parsedX5) ? parsedX5 : 0;
            var x6 = int.TryParse(_x6Entry.Text, out int parsedX6) ? parsedX6 : 0;
            var x7 = int.TryParse(_x7Entry.Text, out int parsedX7) ? parsedX7 : 0;
            var x8 = int.TryParse(_x8Entry.Text, out int parsedX8) ? parsedX8 : 0;

            Log("Running...", isSuccess: true);

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
