using Ooui.Forms;
using Xamarin.Forms;

namespace GoTo.Studio
{
    public partial class IDEPage : ContentPage
    {
        private const int DefaultSpacing = 8;

        Switch _debugReleaseSwitch;
        Label _debugReleaseLabel;
        Button _shareButton, _runButton;
        Entry _x1Entry, _x2Entry, _x3Entry, _x4Entry, _x5Entry, _x6Entry, _x7Entry, _x8Entry, _yEntry;
        Editor _textEditor, _outputEditor;

        void InitializeComponent()
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(100) },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                ColumnSpacing = DefaultSpacing,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                RowSpacing = DefaultSpacing
            };

            var topLeftStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            topLeftStackLayout.Children.Add(
                new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), Text = "GoTo Studio" });
            grid.Children.Add(topLeftStackLayout);

            var leftMenuStackLayout = new StackLayout 
            { 
                Orientation = StackOrientation.Horizontal,
                Spacing = DefaultSpacing
            };
            leftMenuStackLayout.Children.Add(
                _debugReleaseSwitch = new Switch { VerticalOptions = LayoutOptions.Center });
            _debugReleaseSwitch.SetBinding(Switch.IsToggledProperty, nameof(ViewModel.IsReleaseEnabled));
            leftMenuStackLayout.Children.Add(
                _debugReleaseLabel = new Label { VerticalOptions = LayoutOptions.Center });
            var debugTrigger = new DataTrigger(typeof(Label))
            {
                Binding = new Binding(nameof(_debugReleaseSwitch.IsToggled), source: _debugReleaseSwitch),
                Value = false
            };
            debugTrigger.Setters.Add(new Setter { Property = Label.TextProperty, Value = "Debug" });
            _debugReleaseLabel.Triggers.Add(debugTrigger);
            var releaseTrigger = new DataTrigger(typeof(Label))
            {
                Binding = new Binding(nameof(_debugReleaseSwitch.IsToggled), source: _debugReleaseSwitch),
                Value = true
            };
            releaseTrigger.Setters.Add(new Setter { Property = Label.TextProperty, Value = "Release" });
            _debugReleaseLabel.Triggers.Add(releaseTrigger);
            leftMenuStackLayout.Children.Add(_runButton = new Button { Text = "Run" });
            _runButton.SetBinding(Button.CommandProperty, nameof(ViewModel.RunCommand));
            leftMenuStackLayout.Children.Add(
                _shareButton = new Button { Text = "Share" });
            _shareButton.SetBinding(Button.CommandProperty, nameof(ViewModel.ShareCommand));
            grid.Children.Add(leftMenuStackLayout, 0, 1);

            var rightMenuStackLayout = new StackLayout 
            { 
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal 
            };
            rightMenuStackLayout.Children.Add(
                new LinkLabel 
                { 
                    HRef = "https://github.com/MarcosCobena/GoTo/wiki/Language", 
                    Target = "_blank",
                    Text = "Language", 
                    VerticalOptions = LayoutOptions.Center
                });
            rightMenuStackLayout.Children.Add(
                new LinkLabel 
                { 
                    HRef = "https://github.com/MarcosCobena/GoTo/issues/new", 
                    Target = "_blank",
                    Text = "Report issue", 
                    VerticalOptions = LayoutOptions.Center
                });
            grid.Children.Add(rightMenuStackLayout, 2, 1);

            grid.Children.Add(_textEditor = new Editor { FontFamily = "monospace" }, 0, 2);
            _textEditor.SetBinding(Editor.TextProperty, nameof(ViewModel.CurrentProgram));

            var inputsStackLayout = new StackLayout();
            inputsStackLayout.Children.Add(_yEntry = new Entry { IsEnabled = false, Placeholder = "Y" });
            _yEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.Y));
            inputsStackLayout.Children.Add(_x1Entry = new Entry { Placeholder = "X1" });
            _x1Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X1));
            inputsStackLayout.Children.Add(_x2Entry = new Entry { Placeholder = "X2" });
            _x2Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X2));
            inputsStackLayout.Children.Add(_x3Entry = new Entry { Placeholder = "X3" });
            _x3Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X3));
            inputsStackLayout.Children.Add(_x4Entry = new Entry { Placeholder = "X4" });
            _x4Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X4));
            inputsStackLayout.Children.Add(_x5Entry = new Entry { Placeholder = "X5" });
            _x5Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X5));
            inputsStackLayout.Children.Add(_x6Entry = new Entry { Placeholder = "X6" });
            _x6Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X6));
            inputsStackLayout.Children.Add(_x7Entry = new Entry { Placeholder = "X7" });
            _x7Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X7));
            inputsStackLayout.Children.Add(_x8Entry = new Entry { Placeholder = "X8" });
            _x8Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.X8));
            grid.Children.Add(inputsStackLayout, 1, 2);

            grid.Children.Add(_outputEditor = new Editor { FontFamily = "monospace" }, 2, 2);

            Content = grid;
            Padding = DefaultSpacing;
        }
    }
}
