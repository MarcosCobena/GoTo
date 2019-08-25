using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace GoToStudio
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

        void Log(IDEViewModel _, string message) => _outputEditor.Text = message;
    }
}
