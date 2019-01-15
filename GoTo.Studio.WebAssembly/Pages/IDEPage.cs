using GoTo.Interpreter;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAssembly;
using Xamarin.Forms;

namespace GoTo.Studio.Pages
{
    public partial class IDEPage
    {
        const string GoToVersion = "1.1.0.0";
        const string CopyXProgram = 
            "; X must be > 0\n" +
            "[A] X = X - 1\n" +
            "Y = Y + 1\n" +
            "IF X != 0 GOTO A";
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
        const string ProgramQueryStringParam = "p=";

        Uri _currentURI;

        public IDEPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var windowLocation = Runtime.InvokeJS("window.location");
            _currentURI = new Uri(windowLocation);

            base.OnAppearing();

            Initialize();
        }

        protected override void OnDisappearing()
        {
            _runButton.Clicked -= RunButton_Clicked;
            _helpButton.Clicked -= HelpButton_Clicked;

            base.OnDisappearing();
        }

        void Alert(string message)
        {
            _outputEditor.Text = string.Empty;
            _outputEditor.TextColor = Color.Black;
            _outputEditor.Text = message;
        }

        void BlockUI(bool isBlocked = true)
        {
            _runButton.IsEnabled = !isBlocked;
            _helpButton.IsEnabled = !isBlocked;
        }

        void DebugReleaseSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                _debugReleaseLabel.Text = "Release";
            }
            else
            {
                _debugReleaseLabel.Text = "Debug";
            }
        }

        void Help()
        {
            var message = Welcome +
                "\n\n" +
                $"GoTo Studio (GoTo {GoToVersion})";
            Alert(message);
        }

        void HelpButton_Clicked(object sender, EventArgs e)
        {
            // This' currently not implemented
            //Device.OpenUri(new Uri("https://github.com/MarcosCobena/GoTo/wiki"));

            Help();
        }

        void Initialize()
        {
            _debugReleaseSwitch.Toggled += DebugReleaseSwitch_Toggled;
            _shareButton.Clicked += ShareButton_Clicked;
            _helpButton.Clicked += HelpButton_Clicked;
            _runButton.Clicked += RunButton_Clicked;

            _debugReleaseSwitch.IsToggled = true;

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

                _textEditor.Text = Uri.UnescapeDataString(unescapedProgram);
            }
            else
            {
                _textEditor.Text = CopyXProgram;
                Help();
            }
        }

        void Log(string message, bool isSuccess = false)
        {
            _outputEditor.TextColor = isSuccess ?
                Color.Green :
                Color.Red;
            _outputEditor.Text = message;
        }

        void Log(IEnumerable<Message> messages)
        {
            var errorMessages = messages
                .Select(message =>
                    $"{message.Severity} at line {message.Line}, column {message.Column}: {message.Description}")
                .Aggregate((current, next) => $"{current}\r\n{next}");
            Log(errorMessages);
        }

        void LogPrepend(string message)
        {
            _outputEditor.Text = message + _outputEditor.Text;
        }

        void Run()
        {
            var x1 = int.TryParse(_x1Entry.Text, out int parsedX1) ? parsedX1 : 0;
            var x2 = int.TryParse(_x2Entry.Text, out int parsedX2) ? parsedX2 : 0;
            var x3 = int.TryParse(_x3Entry.Text, out int parsedX3) ? parsedX3 : 0;
            var x4 = int.TryParse(_x4Entry.Text, out int parsedX4) ? parsedX4 : 0;
            var x5 = int.TryParse(_x5Entry.Text, out int parsedX5) ? parsedX5 : 0;
            var x6 = int.TryParse(_x6Entry.Text, out int parsedX6) ? parsedX6 : 0;
            var x7 = int.TryParse(_x7Entry.Text, out int parsedX7) ? parsedX7 : 0;
            var x8 = int.TryParse(_x8Entry.Text, out int parsedX8) ? parsedX8 : 0;

            BlockUI();
            _runButton.Text = "Running...";

            var isSucceeded = Framework.TryAnalyze(
                _textEditor.Text,
                out ProgramNode program,
                out IEnumerable<Message> messages);

            if (!isSucceeded)
            {
                Log(messages);

                _runButton.Text = "Run";
                BlockUI(false);

                return;
            }

            Log(string.Empty, isSuccess: true);

            var step = 0;
            Func<Locals, bool> stepDebugAndContinueFunc;

            if (_debugReleaseSwitch.IsToggled)
            {
                stepDebugAndContinueFunc = _ => true;
            }
            else
            {
                stepDebugAndContinueFunc = new Func<Locals, bool>(locals =>
                {
                    step++;

                    var message =
                        $"Step #{step}:\n" +
                        $"{locals}\n\n";
                    // FIXME out of memory in infinite loops
                    LogPrepend(message);

                    return true;
                });
            }

            int result = -1;

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

            _runButton.Text = "Run";
            BlockUI(false);

            if (isSucceeded)
            {
                if (_debugReleaseSwitch.IsToggled)
                {
                    Log($"Y = {result}", isSuccess: true);
                }
                else
                {
                    // Intentionally empty
                }
            }
            else
            {
                const string MaxStepsExceededMessage = 
                    "The execution exceeded max steps, it's likely the program contains an infinite loop.";

                if (_debugReleaseSwitch.IsToggled)
                {
                    Log(MaxStepsExceededMessage);
                }
                else
                {
                    LogPrepend(MaxStepsExceededMessage);
                }
            }
        }

        void RunButton_Clicked(object sender, EventArgs e)
        {
            Run();
        }

        void ShareButton_Clicked(object sender, EventArgs e)
        {
            var escapedProgram = Uri.EscapeDataString(_textEditor.Text);
            var newURI = $"{_currentURI.Scheme}://{_currentURI.Host}{_currentURI.AbsolutePath}?p={escapedProgram}";
            var message =
                "Copy this link to share the current program:\n" +
                "\n" +
                newURI;
            Alert(message);
        }
    }
}
