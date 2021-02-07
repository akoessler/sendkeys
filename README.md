# sendkeys
Tool to send keystrokes to other processes or windows.


## usage
```
sendkeys [options] <keys-to-send>

Arguments:
  keys-to-send  Keys to send

Options:
  -?|-h|--help  Show help information.
  -v|--verbose  Enable verbose logging
  -p|--id       Process id
  -n|--name     Process name
  -t|--title    Window title
```

## help text
Searches for a window of another process and sends keystrokes to that window.

* If only process (id or name) is provided, keys are sent to the main window of that process.
* If only a window title is provided, a window with that title is searched on the desktop.
* If both process and window title is provided, a window within that process is searched.

At least one of process or title must be provided to find a target window.

All other arguments, including unknown options, are considered as keys to be sent to the target window.
* Keys arguments can be enclosed in quotes, e.g. to include spaces.
* Keys are sent in the order they are provided.

Printable keys can be provided as they are. Control keys enclosed with braces, e.g. "{ENTER}".

Full key code reference: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send
