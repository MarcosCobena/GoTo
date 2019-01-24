using Xamarin.Forms;

namespace GoTo.Studio
{
    public partial class IDEPage : ContentPage
    {
        Switch _debugReleaseSwitch;
        Label _debugReleaseLabel;
        Button _shareButton, _helpButton, _runButton;
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
                ColumnSpacing = 8,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                RowSpacing = 8
            };

            var topLeftStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            topLeftStackLayout.Children.Add(
                new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), Text = "GoTo Studio" });
            topLeftStackLayout.Children.Add(
                _debugReleaseSwitch = new Switch { VerticalOptions = LayoutOptions.Center });
            _debugReleaseSwitch.SetBinding(Switch.IsToggledProperty, nameof(ViewModel.IsReleaseEnabled));
            topLeftStackLayout.Children.Add(
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
            grid.Children.Add(topLeftStackLayout);

            var topRightStackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal
            };
            topRightStackLayout.Children.Add(
                _shareButton = new Button { Text = "Share" });
            _shareButton.SetBinding(Button.CommandProperty, nameof(ViewModel.ShareCommand));
            topRightStackLayout.Children.Add(
                _helpButton = new Button { Text = "Help" });
            _helpButton.SetBinding(Button.CommandProperty, nameof(ViewModel.HelpCommand));
            grid.Children.Add(topRightStackLayout, 2, 0);

            grid.Children.Add(_textEditor = new Editor { FontFamily = "monospace" }, 0, 1);
            _textEditor.SetBinding(Editor.TextProperty, nameof(ViewModel.CurrentProgram));

            var inputsStackLayout = new StackLayout();
            inputsStackLayout.Children.Add(_yEntry = new Entry { IsEnabled = false, Placeholder = "Y" });
            _yEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.Y));
            inputsStackLayout.Children.Add(_runButton = new Button { Text = "Run" });
            _runButton.SetBinding(Button.CommandProperty, nameof(ViewModel.RunCommand));
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
            grid.Children.Add(inputsStackLayout, 1, 1);

            grid.Children.Add(_outputEditor = new Editor { FontFamily = "monospace" }, 2, 1);

            Content = grid;
            Padding = 16;
        }
    }
}
