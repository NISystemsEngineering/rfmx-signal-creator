# NI-RFmx Signal Creator

This tool simplifies the process of creating NI-RFmx signal configurations by creating them automatically from NI-RFmx Waveform Creator configuration files. For supported waveform configuration files, a NI-RFmx signal will be created that maps all available properties from the configuration file to the appropriate NI-RFmx settings.

## Currently Supported Standards

The NI-RFmx Signal Creator features plugin support to support the different standards implemented in NI-RFmx. The list below contains all currently available plugins:

- [5G NR](/Source/Plugins/NrPlugin)

# Usage

1) Install the latest NI Package from the [Releases section](../../releases/latest).

2) From the command prompt, launch the program from *C:\Program Files (x86)\National Instruments\Shared\RFmxSignalCreator.exe*. This will show the help screen with the list of arguments and examples.

3) Pass the utility one or more supported waveform configuration files. For each file that can be parsed by a plugin, a TDMS file containing the NI-RFmx signal configuration will be created.

4) Using either the NI-RFmx Soft Front Panel or RFmx Instr API, load the signal configuration TDMS file and begin analyzing your waveform.



### Arguments

| Argument | Description |
| -------- | ----------- |
| `<Paths> (pos. 0)`  | Required. Specifies one or more paths to load; paths can be a single waveform configuration file or a directory of configuration files
| `-o or --outputdir` | Alternate directory to output configuration files to; default is in the same directory.
| `-v or --verbose`   | Enable verbose logging in the log file and optionally the console if -c is set.
| `c or --console`      | Sends full file log to console in addition to the log file.
| `--help`            | Display the help screen.
| `--version`         | Display version information.

### Examples

##### Process a single waveform configuration
`RFmxSignalCreator C:\waveform.rfws`
##### Process a directory containing multiple waveform configurations with a custom output directory
`RFmxSignalCreator -o "C:\RFmx Configurations" "C:\Waveform Configurations"`
##### Process multiple files and diretories containing multiple waveform configurations
`RFmxSignalCreator -o "C:\RFmx Configurations" waveform1.rfws waveform2.rfws Waveforms\MoreFiles`
##### Process a directory with verbose logging to the console
`RFmxSignalCreator -c -v "C:\Waveform Configurations"`
