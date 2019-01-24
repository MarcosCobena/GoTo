using Xamarin.Forms;

namespace GoTo.Studio
{
    public partial class IDEPage
    {
        public IDEPage()
        {
            InitializeComponent();

            BindingContext = new IDEViewModel();
        }

        IDEViewModel ViewModel => BindingContext as IDEViewModel;

        protected override void OnAppearing()
        {
            MessagingCenter.Instance.Subscribe<IDEViewModel, string>(this, IDEViewModel.LogMessage, Log);

            base.OnAppearing();

            ViewModel.Initialize();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Instance.Unsubscribe<IDEViewModel, string>(this, IDEViewModel.LogMessage);
        }

        void Log(IDEViewModel _, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _outputEditor.Text = string.Empty;
                return;
            }

            if (string.IsNullOrWhiteSpace(_outputEditor.Text))
            {
                _outputEditor.Text = message;
                return;
            }

            // TODO scroll to bottom
            _outputEditor.Text += $"\n{message}";
        }
    }
}
