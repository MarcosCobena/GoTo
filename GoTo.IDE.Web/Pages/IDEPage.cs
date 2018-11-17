using System;
using System.Linq;
using Xamarin.Forms;

namespace GoTo.Studio.Web.Pages
{
    public class IDEPage : ContentPage
    {
        Button _runButton;
        Entry _x1Entry;
        Editor _textEditor;
        Editor _outputLabel;

        public IDEPage()
        {
            InitializeComponent();

            _runButton.Clicked += (_, __) => Run();

#if DEBUG
            Run();
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _x1Entry.Text = "0";
            _textEditor.Text = "Y = Y + 1";
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
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 8
            };
            var toolbarStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            toolbarStackLayout.Children.Add(_runButton = new Button { Text = "Run" });
            grid.Children.Add(toolbarStackLayout, 0, 0);
            _x1Entry = new Entry { Placeholder = "X1" };
            toolbarStackLayout.Children.Add(_x1Entry);
            _textEditor = new Editor();
            grid.Children.Add(_textEditor, 0, 1);
            _outputLabel = new Editor { IsEnabled = false };
            grid.Children.Add(_outputLabel, 1, 1);

            Content = grid;
            Padding = 16;
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

            var output = Language.Run(_textEditor.Text, x1);
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
