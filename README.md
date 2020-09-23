# Signal Creator for NI-RFmx

This tool simplifies the process of creating NI-RFmx signal configurations by creating them automatically from NI-RFmx Waveform Creator configuration files. For supported waveform configuration files, a NI-RFmx signal will be created that maps all available properties from the configuration file to the appropriate NI-RFmx settings.

<img src="/_img/transfer.png" width="400">



## Currently Supported Standards

The Signal Creator features plugin support to support the different standards implemented in NI-RFmx. The list below contains all currently available plugins:

- [5G NR](/Source/Plugins/NrPlugin)
- [WLAN](/Source/Plugins/WlanPlugin)

# Installation

The best way to install is by adding the release feed to NI Package Manager. This way, NI Package Manager will automatically notify you when updates are available. Alternatively, you can manually download the latest packages. In both cases, you can find up-to-date installation instructions and the latest verision from the [Releases section](../../releases/latest).

# Usage

1) Do one of the following to launch the program and show the help screen with the list of arguments and examples. 
    - Search for "Signal Creator for NI-RFmx" from the Windows Start Menu, or select it from National Instruments >> Signal Creator for NI-RFmx
    - From the command prompt, launch the program from *C:\Program Files (x86)\National Instruments\Shared\Signal Creator for NI-RFmx\SignalCreator.exe*. 

2) Pass the utility one or more supported waveform configuration files. For each file that can be parsed by a plugin, a TDMS file containing the NI-RFmx signal configuration will be created.

3) Using either the NI-RFmx Soft Front Panel or RFmx Instr API, load the signal configuration TDMS file and begin analyzing your waveform.



### Arguments

| Argument            | Description                                                  |
| ------------------- | ------------------------------------------------------------ |
| `<Paths> (pos. 0)`  | Required. Specifies one or more paths to load; paths can be a single waveform configuration file or a directory of configuration |
| `-o or --outputdir` | Alternate directory to output configuration files to; default is in the same directory. |
| `-v or --verbose`   | Enable verbose logging in the log file and optionally the console if -c is set. |
| `-c or --console`   | Sends full file log to console in addition to the log file.  |
| `--help`            | Display the help screen.                                     |
| `--version`         | Display version information.                                 |

### Examples

##### Process a single waveform configuration
`SignalCreator C:\waveform.rfws`
##### Process a directory containing multiple waveform configurations with a custom output directory
`SignalCreator -o "C:\RFmx Configurations" "C:\Waveform Configurations"`
##### Process multiple files and diretories containing multiple waveform configurations
`SignalCreator -o "C:\RFmx Configurations" waveform1.rfws waveform2.rfws Waveforms\MoreFiles`
##### Process a directory with verbose logging to the console
`SignalCreator -c -v "C:\Waveform Configurations"`
