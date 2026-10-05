# AGENTS.md

GoTo is a compiler and interpreter for the GOTO language written in C#.

## Layout

- `GoTo/`: the library (parser generated with ANTLR in `GoTo/Parser`, semantic analysis, interpreter, IL emitter, codifier).
- `Tests/`: xUnit tests for the library.
- `GoTo.CLI/`: command-line compiler (`gotool`).
- `GoToStudio/`: browser IDE built on Ooui/Xamarin.Forms (legacy).
- `Samples/`: sample `.goto` programs.

## Environment setup

The solution needs the .NET 10 SDK. If `dotnet --list-sdks` does not show a `10.0.x` SDK, install it:

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir "$HOME/.dotnet"
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
```

## Build and test

```bash
dotnet restore GoTo.sln
dotnet build GoTo.sln --no-restore
dotnet test Tests/Tests.csproj --no-build
```

There is no separate lint step; treat compiler warnings introduced by your change as issues to fix.

Projects targeting .NET Framework (`net48`, `net461`) build on Linux through reference assemblies, but their tests cannot run there. Report that limitation instead of working around it.

## Conventions

- Keep `using` directives in alphabetical order.
- Do not edit the ANTLR-generated files in `GoTo/Parser` (`GoToLexer.cs`, `GoToParser.cs`, `GoTo*Listener.cs`, `GoTo*Visitor.cs`, `*.interp`, `*.tokens`) by hand; regenerate them from `GoTo.g4` with `GoTo/Parser/antlr-4.7.1-complete.jar`, which matches `Antlr4.Runtime.Standard`.
- Do not add comments that restate what the code does.
