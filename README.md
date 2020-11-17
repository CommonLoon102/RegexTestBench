# RegexTestBench
Cross-platform regex testing tool in .NET 5.

**Protip**: use the [RegexFileSearcher](https://github.com/CommonLoon102/RegexFileSearcher) to search by regexes for files on your filesystem.

![](https://github.com/CommonLoon102/RegexTestBench/blob/master/image/screenshot.png?raw=true)

Running
-------
### Windows
 - Start the app with `RegexTestBench.Wpf.exe`
### Linux
 - Start the app from the terminal with `./RegexTestBench.Gtk`

Compiling
---------
### Windows
 - Download and open the repo in Visual Studio 2019
 - Publish `RegexTestBench.Wpf`
### Linux
- Install the .NET 5 SDK
- You need to have GTK3 installed too
- Git clone
- Publish with `dotnet publish RegexTestBench/RegexTestBench.Gtk/RegexTestBench.Gtk.csproj --configuration Release --output publish --self-contained true -p:PublishSingleFile=true --runtime linux-x64 --framework net5.0`

Debugging
---------
### Windows
 - With `Visual Studio 2019 Community`
   - Set `RegexTestBench.Wpf` as startup project
 - Or with `Visual Studio Code`
### Linux
 - With `Visual Studio Code`
 
 `launch.jon`:
 ```json
 {
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
        "name": "Debug on Linux",
        "type": "coreclr",
        "request": "launch",
        "preLaunchTask": "build",
        "program": "dotnet",
        "args": ["${workspaceFolder}/RegexTestBench/RegexTestBench.Gtk/bin/Debug/net5.0/RegexTestBench.Gtk.dll"],
        "cwd": "${workspaceFolder}",
        "stopAtEntry": false,
        "console": "internalConsole"
    }]
}
 ```
 `tasks.json`
 ```json
 {
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/RegexTestBench/RegexTestBench.Gtk/RegexTestBench.Gtk.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "publish",
            "command": "dotnet",
            "type": "process",
            "args": [
                "publish",
                "${workspaceFolder}/RegexTestBench/RegexTestBench.Gtk/RegexTestBench.Gtk.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "watch",
            "command": "dotnet",
            "type": "process",
            "args": [
                "watch",
                "run",
                "${workspaceFolder}/RegexTestBench/RegexTestBench.Gtk/RegexTestBench.Gtk.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
 ```
