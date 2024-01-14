# SuperComic's Libraries
![GitHub](https://img.shields.io/github/license/ekfvoddl3536/0SuperComicLibs) 
![GitHub release (latest by date)](https://img.shields.io/github/v/release/ekfvoddl3536/0SuperComicLibs) 
  
# !!== Notification ==!!
`0SuperComicLibs` no longer supports `x86` or `Any CPU`.  
The last version that supports `x86` or `Any CPU` architecture is `v4.1 (104100)`.  
Updates will be made on the `x-amd64` branch for the time being.  
  
# Summary
Series of libraries that can be used universally for various development.  
Unless specified, all projects require `.NET Framework 4.7.1` or higher.  

| Name                                  | Description                                                                                                                   |
| :------------                         | :----------------------                                                                                                       |
| 0SuperComicLib.Core                   | Provides features that will be useful in most developments.                                                                   |
| 0SuperComicLib.NET5Core               | (`.NET 6.0+`) Provides experimental technology.                                                                               |
| 1SuperComicLib.Runtime                | Provides features such as low-level memory manipulation and array access without bounds checking.                             |
| 1SuperComicLib.Tensor                 | (`.NET 8.0+`) Provides tensor-structured memory excluding operations.                                                         |
| 1SuperComicLib.Text                   | Provides features related to string processing.                                                                               |
| 1SuperComicLib.Threading              | Provides features related to parallel processing or threads.                                                                  |
| 2SuperComicLib.Arithmetic             | Provides features related to mathematical operations.                                                                         |
| 2SuperComicLib.Runtime.Managed        | Similar to `1SuperComicLib.Runtime`, but provides more features by expanding `1SuperComicLib.Runtime`.                        |
| 3SuperComicLib.Collections            | Provides implementations of various data structures and provides useful extensions related to collections.                    |
| 3SuperComicLib.IO                     | Provides features related to file I/O.                                                                                        |
| 4SuperComicLib.Collection.Concurrent  | Extends `3SuperComicLib.Collections` to provide features for concurrency.                                                     |
| 4SuperComicLib.IO.Unsafe              | Provides an extended features that does not sequentially process file I/O.                                                    |
| 7SuperComicLib.Reflection             | Provides features related to run-time dynamic CIL code generation/reading.                                                    |
| 9SuperComicLib.DataObject             | Provides read/write features for special typed data files.                                                                    |
| 9SuperComicLib.XPatch                 | Provides features related to run-time dynamic method patching.                                                                |


## Recommended library
 - 0SuperComicLib.Core
 - 1SuperComicLib.Runtime
 - 2SuperComicLib.Runtime.Managed
 - 3SuperComicLib.Collections

# Get build artifacts
## Method 1 (_recommend_)
1. Visit the [Release page](https://github.com/ekfvoddl3536/0SuperComicLibs/releases).
2. Download the result attached to `Assets` of the latest version tag.

## Method 2
1. Download the remote repository to local machine.
2. Open the project directory want to build in Visual Studio, or add all the source code in the directory to an existing project.
3. Build and get output.
