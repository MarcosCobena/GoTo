using Avalonia.Controls;
using System.ComponentModel;

namespace GoToStudio
{
    public partial class MainView : UserControl
    {
        readonly IDEViewModel _viewModel;
        bool _isSyncing;

        public MainView()
        {
            InitializeComponent();

            _viewModel = new IDEViewModel();
            DataContext = _viewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            ProgramEditor.TextChanged += OnEditorTextChanged;
            _viewModel.Initialize();
            SyncEditorFromViewModel();
        }

        void OnEditorTextChanged(object sender, System.EventArgs e)
        {
            if (_isSyncing)
            {
                return;
            }

            _isSyncing = true;
            _viewModel.CurrentProgram = ProgramEditor.Text;
            _isSyncing = false;
        }

        void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDEViewModel.CurrentProgram))
            {
                SyncEditorFromViewModel();
            }
        }

        void SyncEditorFromViewModel()
        {
            if (_isSyncing || ProgramEditor.Text == _viewModel.CurrentProgram)
            {
                return;
            }

            _isSyncing = true;
            ProgramEditor.Text = _viewModel.CurrentProgram ?? string.Empty;
            _isSyncing = false;
        }
    }
}
