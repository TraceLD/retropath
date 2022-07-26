# retropath (WIP)
RetroPath2.0 algorithm as a standalone application aimed to be easy to use and install.

The algorithm builds a reaction network from a set of source compounds to a set of sink compounds using a set of reaction rules. In retrosynthesis the source is (are) the target compound(s) and sink are the native metabolites. After generating the chemical reactions via retrosynthesis a subset may effectively link a source to a subset of sink compounds. This sub-network is a scope and is output in a dedicated file. (Delépine et al., 2018)

## Installation instruction for users

### Currently supported operating systems
- Windows 10 x64 and Windows 11 x64
- macOS Big Sur or newer on the Apple Silicon architecture (M1/M2 Macs); Intel Macs are not supported at present.
- Linux x64 (tested on Ubuntu)

### Installation steps

1. Open [GitHub Releases](https://github.com/TraceLD/retropath/releases) in a new tab (so you can easily go back to this document and reference further instructions) and download the distribution for your operating system by expanding the Assets list and downloading the file for your operating system.

### Windows

2. Unzip the downloaded file by right clicking on the file and selecting "Extract All" and following the extraction wizard. If you get an error due to permissions/security exit the wizard, right click on the file, click "Properties" and towards the bottom right select "Unblock". You can see a visual example how to do it [here](https://winbuzzer.com/wp-content/uploads/2021/12/01.3-Windows-11-File-Explorer-Right-click-on-File-Properties-General-Unblock-Accept.jpg).

### Linux

2. Open the terminal, `cd` into the folder where the downloaded zip file is and type `unzip file_name.zip` replacing `file_name` with the name of the downloaded file.

3. Give RetroPath the permission to execute by `cd`ing into the unzipped folder in the terminal and running `chmod +x ./RetroPath.Cli`.

### macOS (Apple Silicon Macs only)

2. Unzip the downloaded file by double clicking the zip file.

3. Because macOS requires all pre-compiled binaries to be signed using an Apple Developer account which costs £100/year, to avoid that charge the RetroPath binaries for macOS are distributed as framework-dependant as the .NET runtime is already signed by Microsoft. This means that you will have to install .NET 6 Runtime.

4. Go to the download page for .NET 6 by opening [this link](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) in a new tab.

5. On the right side find the position that says ".NET Runtime", then find macOS in the table in the Installers column and click on "Arm64" to download the installer. This will download the ".pkg" installer.

6. Open the downloaded installer and follow the instructions to complete the installation of .NET 6 Runtime.

### Running the example

Bundled with the program is a simple pinocembrin example data to verify the installation works.

#### Windows

1. Enter the unzipped folder and open the terminal (On Windows 11 you can open the terminal inside the folder by right clicking inside the folder and selecting "Open in terminal", otherwise open PowerShell and `cd` into the folder).

2. Run the following command via the terminal: `./RetroPath.Cli.exe "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`.

#### Linux

1. Enter the unzipped folder and open the terminal inside it (you can do this by opening the terminal and `cd`ing into the folder).

2. Run the following command via the terminal: `./RetroPath.Cli "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`

**Linux-only caution:** In the unlikely event that some system package is missing a human readable exception will be thrown and you should read it to see what package is missing and then install the missing package from your distro's package maneger (e.g. `apt`/`yum`/etc.).

#### macOS

1. Enter the unzipped folder and open the terminal inside it (you can do this by opening the terminal and `cd`ing into the folder).

2. Run the following command via the terminal: `dotnet RetroPath.Cli.dll "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`.

### Viewing the results

To view the results see the `results` folder that has been generated after running the example.

### Full usage

To see help how to use the program with all the possible options and parameters run `./RetroPath.Cli` on Linux/macOS and `./RetroPath.Cli.exe` in the terminal.

The basic usage is `./RetroPath.Cli <rules-file> <source-file> <sink-file> <pathway-length>`. On Windows `./RetroPath.Cli` needs to be replaced with `./RetroPath.Cli.exe` and on macOS with `dotnet RetroPath.Cli.dll`.

## Information for developers

TODO

## References

Delépine, B., Duigou, T., Carbonell, P. and Faulon, J.-L. (2018). RetroPath2.0: A retrosynthesis workflow for metabolic engineers. *Metabolic Engineering*, 45, pp.158–170. doi:10.1016/j.ymben.2017.12.002.
‌
