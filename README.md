# retropath
RetroPath 2.0 algorithm as a standalone application.

## Installation instruction for users

### Currently supported operating systems
- Windows 10 x64 and Windows 11 x64
- macOS Big Sur or newer on the Apple Silicon architecture (M1/M2 Macs)
- Linux x64 (tested on Ubuntu)

### Installation steps

Because the build is statically compiled on the C++ side and self-contained on the .NET side there should be no extra dependencies that need to be downloaded.  

1. Download the distribution for your operating system from [GitHub Releases](https://github.com/TraceLD/retropath/releases).

2. Unzip the downloaded file to where you want RetroPath to be. To unzip on macOS simply double click the zip file. On Windows right click on the file and select "Extract All" or use a third-party software such as 7zip. On Linux open the terminal and type `unzip file_name.zip`.

### Running the example

Bundled with the program is a simple pinocembrin example data to verify the installation works.

#### Windows

1. Enter the unzipped folder and open the terminal.

2. Run the following command via the terminal: `./RetroPath.Cli.exe "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`

#### Linux and macOS

1. Enter the unzipped folder and open the terminal.

2. Run the following command via the terminal: `./RetroPath.Cli "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`

**Linux-only caution:** In the event that some system package is missing a human readable exception will be thrown and you should read it to see what package is missing and then install the missing package from your distro's package maneger (e.g. `apt`/`yum`/etc.).


## Information for developers

TODO