using System.Linq;
using Xamarin.Forms;

namespace GoTo.IDE.Web.Pages
{
    public class IDEPage : ContentPage
    {
        Entry _x1Entry;
        Editor _textEditor;
        Editor _outputLabel;

        public IDEPage()
        {
            InitializeComponent();

            _x1Entry.Text = "1";
            _textEditor.Text = "Y = Y + 1";

            Run();
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
            toolbarStackLayout.Children.Add(new Button { Text = "Run" });
            grid.Children.Add(toolbarStackLayout, 0, 0);
            _x1Entry = new Entry { Placeholder = "X1", WidthRequest = 50 };
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
            var output = Language.Run(_textEditor.Text, int.Parse(_x1Entry.Text));
            var isFailed = output.messages.Any(message => message.Severity == SeverityEnum.Error);

            if (isFailed)
            {
                _outputLabel.TextColor = Color.Red;
                _outputLabel.Text = output.messages
                    .Select(message =>
                        $"{message.Severity} at line {message.Line}, column {message.Column}: {message.Description}")
                    .Aggregate((current, next) => $"{current}\r\n{next}");
            }
            else
            {
                _outputLabel.TextColor = Color.Green;
                _outputLabel.Text = $"Y = {output.result}";
            }
        }
    }
}
