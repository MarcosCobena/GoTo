﻿using Xamarin.Forms;

namespace GoTo.Studio.Web.Pages
{
    public partial class IDEPage : ContentPage
    {
        Button _runButton;
        Button _helpButton;
        Editor _textEditor;
        Entry _x1Entry;
        Entry _x2Entry;
        Entry _x3Entry;
        Entry _x4Entry;
        Entry _x5Entry;
        Entry _x6Entry;
        Entry _x7Entry;
        Entry _x8Entry;
        Editor _outputLabel;

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

            grid.Children.Add(
                new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), Text = "GoTo Studio" });

            grid.Children.Add(
                _helpButton = new Button { HorizontalOptions = LayoutOptions.End, Text = "Help" }, 2, 0);

            grid.Children.Add(_textEditor = new Editor(), 0, 1);

            var inputsStackLayout = new StackLayout();
            inputsStackLayout.Children.Add(_x1Entry = new Entry { Placeholder = "X1" });
            inputsStackLayout.Children.Add(_x2Entry = new Entry { Placeholder = "X2" });
            inputsStackLayout.Children.Add(_x3Entry = new Entry { Placeholder = "X3" });
            inputsStackLayout.Children.Add(_x4Entry = new Entry { Placeholder = "X4" });
            inputsStackLayout.Children.Add(_x5Entry = new Entry { Placeholder = "X5" });
            inputsStackLayout.Children.Add(_x6Entry = new Entry { Placeholder = "X6" });
            inputsStackLayout.Children.Add(_x7Entry = new Entry { Placeholder = "X7" });
            inputsStackLayout.Children.Add(_x8Entry = new Entry { Placeholder = "X8" });
            inputsStackLayout.Children.Add(_runButton = new Button { Text = "Run" });
            grid.Children.Add(inputsStackLayout, 1, 1);

            grid.Children.Add(_outputLabel = new Editor { IsEnabled = false }, 2, 1);

            Content = grid;
            Padding = 16;
        }
    }
}