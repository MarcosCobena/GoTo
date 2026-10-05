using Avalonia.Controls;

namespace GoToStudio
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

            var viewModel = new IDEViewModel();
            DataContext = viewModel;
            viewModel.Initialize();
        }
    }
}
