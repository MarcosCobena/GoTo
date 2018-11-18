using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace GoTo.Studio.Web.Pages
{
    public class IDEPage : ContentPage
    {
        private readonly string _goToVersion;

        Button _runButton;
        Entry _x1Entry;
        Button _helpButton;
        Editor _textEditor;
        Editor _outputLabel;

        public IDEPage()
        {
            InitializeComponent();

#if DEBUG
            Initialize();
            Run();
#endif

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

        void InitializeComponent()
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
                },
                ColumnSpacing = 8,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                RowSpacing = 8
            };
            var toolbarStackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal
            };
            toolbarStackLayout.Children.Add(_runButton = new Button { Text = "Run" });
            toolbarStackLayout.Children.Add(_x1Entry = new Entry { Placeholder = "X1" });
            grid.Children.Add(toolbarStackLayout, 0, 0);

            grid.Children.Add(_helpButton = new Button { HorizontalOptions = LayoutOptions.End, Text = "Help" }, 1, 0);

            grid.Children.Add(_textEditor = new Editor(), 0, 1);

            grid.Children.Add(_outputLabel = new Editor { IsEnabled = false }, 1, 1);

            Content = grid;
            Padding = 16;
        }

        private void Initialize()
        {
            _runButton.Clicked += RunButton_Clicked;
            _helpButton.Clicked += HelpButton_Clicked;

            _x1Entry.Text = "0";
            _textEditor.Text = "Y = Y + 1";
        }

        void HelpButton_Clicked(object sender, EventArgs e)
        {
            // This' currently not implemented
            //Device.OpenUri(new Uri("https://github.com/MarcosCobena/GoTo/wiki"));

            DisplayAlert("Help", $"GoTo Studio (GoTo v. {_goToVersion})", "Thanks");
        }

        void RunButton_Clicked(object sender, EventArgs e)
        {
            Run();
        }

        void Run()
        {
            var isSuccess = true;

            isSuccess = int.TryParse(_x1Entry.Text, out int x1);

            if (!isSuccess)
            {
                Log($"{_x1Entry.Placeholder} must be an {x1.GetType()}");
                return;
            }

            (int result, IEnumerable<Message> messages) output;
            var isRunAborted = false;

            try
            {
                output = Language.Run(_textEditor.Text, x1);
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

        void Log(string message, bool isSuccess = false)
        {
            _outputLabel.TextColor = isSuccess ? 
                Color.Green : 
                Color.Red;
            _outputLabel.Text = message;
        }
    }
}
