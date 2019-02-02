using GoTo.Interpreter;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        const string MaxStepsExceededMessage =
            "The execution exceeded max steps, it's likely the program contains an infinite loop.";
        const string ProgramQueryStringParam = "p=";
        const string Welcome =
            "Welcome to GoTo Studio!\n" +
            "\n" +
            "I'm the output, here you'll see the info while running those programs you write at my left " +
            "—the editor.\n" +
            "\n" +
            "Do you see that column of entries at center? They're the inputs, waiting for you to type integers.\n" +
            "\n" +
            "Why don't you just start by typing 42 at X1 and click Run?\n" +
            "\n" +
            "Oh, if you may encounter any issue, please send it to us through above Report issue link. Thanks in advance.";

        internal const string LogMessage = nameof(LogMessage);

        readonly Uri _currentURI;

        string _currentProgram;
        bool _isReleaseEnabled;
        string _x1, _x2, _x3, _x4, _x5, _x6, _x7, _x8, _y;

        public IDEViewModel()
        {
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

        public ICommand RunCommand { get; }

        public ICommand ShareCommand { get; }

        internal void Initialize()
        {
            IsReleaseEnabled = true;

            LoadStartUpProgram();
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
                Log(Welcome);
            }
        }

        void Log(string message) => MessagingCenter.Instance.Send(this, LogMessage, message);

        void Run()
        {
            // TODO converter
            var x1 = int.TryParse(_x1, out int parsedX1) ? parsedX1 : 0;
            var x2 = int.TryParse(_x2, out int parsedX2) ? parsedX2 : 0;
            var x3 = int.TryParse(_x3, out int parsedX3) ? parsedX3 : 0;
            var x4 = int.TryParse(_x4, out int parsedX4) ? parsedX4 : 0;
            var x5 = int.TryParse(_x5, out int parsedX5) ? parsedX5 : 0;
            var x6 = int.TryParse(_x6, out int parsedX6) ? parsedX6 : 0;
            var x7 = int.TryParse(_x7, out int parsedX7) ? parsedX7 : 0;
            var x8 = int.TryParse(_x8, out int parsedX8) ? parsedX8 : 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var isSucceeded = Framework.TryAnalyze(
                _currentProgram,
                out ProgramNode program,
                out IEnumerable<Message> messages);
            stopwatch.Stop();

            if (!isSucceeded)
            {
                var errors = messages
                    .Select(item =>
                        $"{item.Severity} at line {item.Line}, column {item.Column}: {item.Description}")
                    .Aggregate((current, next) => $"{current}\r\n{next}");
                Log(errors);
                return;
            }

            var message = new StringBuilder();
            message.AppendLine($"Program analyzed without errors ({stopwatch.ElapsedMilliseconds} ms)");

            var step = 0;
            Func<Locals, bool> stepDebugAndContinueFunc;

            if (_isReleaseEnabled)
            {
                stepDebugAndContinueFunc = _ => true;
            }
            else
            {
                message.AppendLine("Running...");
                stepDebugAndContinueFunc = new Func<Locals, bool>(
                    locals => StepDebugAndContinue(locals, ++step, message));
            }

            var result = 0;
            stopwatch.Restart();

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
            finally
            {
                stopwatch.Stop();
            }

            if (isSucceeded)
            {
                message.AppendLine($"Program run successfully ({stopwatch.ElapsedMilliseconds} ms)");
                Y = result.ToString();
            }
            else
            {
                message.AppendLine(MaxStepsExceededMessage);
                Y = string.Empty;
            }

            Log(message.ToString());
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
            Log(message);
        }

        bool StepDebugAndContinue(Locals locals, int step, StringBuilder message)
        {
            message.AppendLine(
                $"Step #{step}:\n" +
                $"{locals}");

            return true;
        }
    }
}