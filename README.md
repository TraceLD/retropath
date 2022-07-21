# retropath (WIP)
RetroPath2.0 algorithm as a standalone application aimed to be easy to use and install.

The algorithm builds a reaction network from a set of source compounds to a set of sink compounds using a set of reaction rules. In retrosynthesis the source is (are) the target compound(s) and sink are the native metabolites. After generating the chemical reactions via retrosynthesis a subset may effectively link a source to a subset of sink compounds. This sub-network is a scope and is output in a dedicated file. (Delépine et al., 2018)

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

1. Enter the unzipped folder and open the terminal

2. Run the following command via the terminal: `./RetroPath.Cli.exe "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`

#### Linux and macOS

1. Enter the unzipped folder and open the terminal inside it

2. Give RetroPath the permission to execute by running `chmod +x ./RetroPath.Cli`

3. Run the following command via the terminal: `./RetroPath.Cli "./example_data/rules.csv" "./example_data/source.csv" "./example_data/sink.csv" 4`

**Linux-only caution:** In the unlikely event that some system package is missing a human readable exception will be thrown and you should read it to see what package is missing and then install the missing package from your distro's package maneger (e.g. `apt`/`yum`/etc.).

### Viewing the results

To view the results see the `results` folder that has been generated after running the example.

### Full usage

To see help how to use the program with all the possible options and parameters run `./RetroPath.Cli` on Linux/macOS and `./RetroPath.Cli.exe` in the terminal.

The basic usage is `./RetroPath.Cli <rules-file> <source-file> <sink-file> <pathway-length>` (on Windows it needs to be `./RetroPath.Cli.exe` instead).

## Information for developers

TODO

## References

Delépine, B., Duigou, T., Carbonell, P. and Faulon, J.-L. (2018). RetroPath2.0: A retrosynthesis workflow for metabolic engineers. *Metabolic Engineering*, 45, pp.158–170. doi:10.1016/j.ymben.2017.12.002.
‌
