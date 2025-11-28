# DropShell — A Quake-Style Drop-Down Console for Windows

DropShell is a C# WPF overlay inspired by Quake’s drop-down console.
Press a global hotkey to instantly open a floating command window where you can launch apps, open folders, run groups, and more.

## Features (v1)

- Global hotkey (Ctrl+Alt+Shift+P by default)
- Slide-down overlay window
- Command execution with history
- Launch executables
- Open folders/files
- Change working directory
- Clear console
- Group execution (launch dev)
- Startup commands
- JSON config file (no database)

## Config File: `%APPDATA%/DropShell/config.json`

Example:
```json
{
  "hotkey": "Ctrl+Alt+Shift+P",
  "window": {
    "height": 300,
    "opacity": 0.9,
    "background": "#1e1e1e",
    "textColor": "#ffffff",
    "fontSize": 14
  },
  "behavior": {
    "showOnStartup": false
  },
  "groups": {
    "dev": [
      "launch \"C:/Program Files/Visual Studio/VS.exe\"",
      "launch \"C:/Users/me/AppData/Local/Programs/Microsoft VS Code/Code.exe\""
    ]
  },
  "startup": [
    "echo Welcome to DropShell!",
    "echo Type 'help' for commands."
  ]
}
```

## Command List (v1)
General
```
help            Show all commands
clear           Clear the console
exit / hide     Hide DropShell
```

Navigation
```
pwd             Show current directory
cd <path>       Change working directory
```

Opening / Launching
```
launch <path>   Launch an application
open <path>     Open file or folder with default program
launch <group>  Execute a configured group of commands
```

Startup Commands
All commands inside `startup` run after DropShell opens.

## Group Execution

Groups are defined in config:
```json
"groups": {
  "dev": [
    "launch \"C:/Program Files/VS/VS.exe\"",
    "open C:/Projects"
  ]
}
```

Execute group:
```bash
launch dev
```

Group execution requires `launch` keyword by design.

## Tech Stack
- C# .NET 8
- WPF
- System.Text.Json
- Win32 hotkey API