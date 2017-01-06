This is an example of using the [.NET Compiler
Platform](https://github.com/dotnet/roslyn) (a.k.a. Project Roslyn) in F#. This
code consists of an F# port of the C#
[code](https://github.com/unosviluppatore/getting-started-roslyn) written by
@unosviluppatore (Gabriele Tomassetti) to accompany his helpful
[article](https://tomassetti.me/getting-started-roslyn/).

# Running
I tested this on OSX using Mono (the precise version info, including those of
package dependencies, is included in the metadata files).

Running the following commands from the console should refresh any packages you
require and build the system for you:
```
$ nuget restore
$ xbuild getting_started_roslyn_fsharp.sln
```
Then run:
```
./getting_started_roslyn_fsharp/bin/Debug/getting_started_roslyn_fsharp.exe
```

# Setup
Put the `.cs` files from the [original code](https://github.com/unosviluppatore/getting-started-roslyn)
in the folder holding `getting_started_roslyn_fsharp.exe` -- in my case it's
`./getting_started_roslyn_fsharp/bin/Debug/`. When you run the binary you should
find a folder called `new_src` in the containing directory
(`./getting_started_roslyn_fsharp/bin` in my case) containing the modified `.cs`
files.
