using GoTo.Interpreter;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WebAssembly;
using Xamarin.Forms;

namespace GoTo.Studio
{
    internal class IDEViewModel : INotifyPropertyChanged
    {
        const string CopyXProgram =
            "; X must be > 0\n" +
            "[A] X = X - 1\n" +
            "Y = Y + 1\n" +
            "IF X != 0 GOTO A";
        const string GoToVersion = "1.1.0.0";
        const string MaxStepsExceededMessage =
            "The execution exceeded max steps, it's likely the program contains an infinite loop.";
        const string ProgramQueryStringParam = "p=";
        const string Welcome =
            "Welcome to GoTo Studio!\n" +
            "\n" +
            "I'm the output, here you'll see the result of running those programs you write at my left " +
            "—the editor.\n" +
            "\n" +
            "Do you see that column of entries at center? They're the inputs, waiting for you to type integers.\n" +
            "\n" +
            "Why don't you just start by typing 42 at X1 and click Run? Please, be patient as sometimes it takes " +
            "some time to show up.\n" +
            "\n" +
            "Oh, if you may encounter any issue, please report it at\n" +
            "\n" +
            "    https://github.com/MarcosCobena/GoTo/issues\n" +
            "\n" +
            "Thanks in advance.";

        internal const string LogMessage = nameof(LogMessage);

        readonly Uri _currentURI;

        string _currentProgram;
        bool _isReleaseEnabled;
        string _x1, _x2, _x3, _x4, _x5, _x6, _x7, _x8, _y;

        public IDEViewModel()
        {
            HelpCommand = new Command(Help);
            RunCommand = new Command(Run);
            ShareCommand = new Command(Share);

            var windowLocation = Runtime.InvokeJS("window.location");
            _currentURI = new Uri(windowLocation);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentProgram
        {
            get => _currentProgram;
            set => SetAndRaisePropertyChanged(ref _currentProgram, value);
        }

        public bool IsReleaseEnabled
        {
            get => _isReleaseEnabled;
            set => SetAndRaisePropertyChanged(ref _isReleaseEnabled, value);
        }

        public string X1
        {
            get => _x1;
            set => SetAndRaisePropertyChanged(ref _x1, value);
        }

        public string X2
        {
            get => _x2;
            set => SetAndRaisePropertyChanged(ref _x2, value);
        }

        public string X3
        {
            get => _x3;
            set => SetAndRaisePropertyChanged(ref _x3, value);
        }

        public string X4
        {
            get => _x4;
            set => SetAndRaisePropertyChanged(ref _x4, value);
        }

        public string X5
        {
            get => _x5;
            set => SetAndRaisePropertyChanged(ref _x5, value);
        }

        public string X6
        {
            get => _x6;
            set => SetAndRaisePropertyChanged(ref _x6, value);
        }

        public string X7
        {
            get => _x7;
            set => SetAndRaisePropertyChanged(ref _x7, value);
        }

        public string X8
        {
            get => _x8;
            set => SetAndRaisePropertyChanged(ref _x8, value);
        }

        public string Y
        {
            get => _y;
            set => SetAndRaisePropertyChanged(ref _y, value);
        }

        public ICommand HelpCommand { get; }

        public ICommand RunCommand { get; }

        public ICommand ShareCommand { get; }

        internal void Initialize()
        {
            IsReleaseEnabled = true;

            LoadStartUpProgram();
        }

        void Help()
        {
            var message = Welcome +
                "\n\n" +
                $"GoTo Studio (GoTo {GoToVersion})";
            Log(string.Empty);
            Log(message);
        }

        void LoadStartUpProgram()
        {
            var query = _currentURI.Query;
            var index = query.IndexOf(ProgramQueryStringParam);

            if (index >= 0)
            {
                var unescapedProgram = query.Substring(index + ProgramQueryStringParam.Length);
                var anyOtherParamIndex = unescapedProgram.IndexOf('&');

                if (anyOtherParamIndex >= 0)
                {
                    unescapedProgram = unescapedProgram.Substring(0, anyOtherParamIndex);
                }

                CurrentProgram = Uri.UnescapeDataString(unescapedProgram);
            }
            else
            {
                CurrentProgram = CopyXProgram;
                Help();
            }
        }

        void Log(string message) => MessagingCenter.Instance.Send(this, LogMessage, message);

        void Log(IEnumerable<Message> messages)
        {
            var errorMessages = messages
                .Select(message =>
                    $"{message.Severity} at line {message.Line}, column {message.Column}: {message.Description}")
                .Aggregate((current, next) => $"{current}\r\n{next}");
            Log(errorMessages);
        }

        void Run()
        {
            Log(string.Empty);
            
            // TODO converter
            var x1 = int.TryParse(_x1, out int parsedX1) ? parsedX1 : 0;
            var x2 = int.TryParse(_x2, out int parsedX2) ? parsedX2 : 0;
            var x3 = int.TryParse(_x3, out int parsedX3) ? parsedX3 : 0;
            var x4 = int.TryParse(_x4, out int parsedX4) ? parsedX4 : 0;
            var x5 = int.TryParse(_x5, out int parsedX5) ? parsedX5 : 0;
            var x6 = int.TryParse(_x6, out int parsedX6) ? parsedX6 : 0;
            var x7 = int.TryParse(_x7, out int parsedX7) ? parsedX7 : 0;
            var x8 = int.TryParse(_x8, out int parsedX8) ? parsedX8 : 0;

            var isSucceeded = Framework.TryAnalyze(
                _currentProgram,
                out ProgramNode program,
                out IEnumerable<Message> messages);

            if (!isSucceeded)
            {
                Log(messages);
                return;
            }

            Log("Program analyzed without errors.");

            var step = 0;
            Func<Locals, bool> stepDebugAndContinueFunc;

            if (_isReleaseEnabled)
            {
                stepDebugAndContinueFunc = _ => true;
            }
            else
            {
                Log("Running...");
                stepDebugAndContinueFunc = new Func<Locals, bool>(locals => StepDebugAndContinue(locals, step++));
            }

            var result = -1;

            try
            {
                Framework.RunInterpreted(
                    program,
                    out int localResult,
                    x1,
                    x2,
                    x3,
                    x4,
                    x5,
                    x6,
                    x7,
                    x8,
                    stepDebugAndContinueFunc);
                result = localResult;
            }
            catch (MaxStepsExceededException)
            {
                isSucceeded = false;
            }

            if (isSucceeded)
            {
                Log("Program run successfully.");
                Y = result.ToString();
            }
            else
            {
                Log(MaxStepsExceededMessage);
                Y = ":-(";
            }
        }

        void SetAndRaisePropertyChanged<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void Share()
        {
            var escapedProgram = Uri.EscapeDataString(CurrentProgram);
            var newURI = $"{_currentURI.Scheme}://{_currentURI.Host}{_currentURI.AbsolutePath}?p={escapedProgram}";
            var message =
                "Copy this link to share the current program:\n" +
                "\n" +
                newURI;
            Log(string.Empty);
            Log(message);
        }

        bool StepDebugAndContinue(Locals locals, int step)
        {
            // FIXME out of memory in infinite loops
            Log(
                $"Step #{step}:\n" +
                $"{locals}");

            return true;
        }
    }
}