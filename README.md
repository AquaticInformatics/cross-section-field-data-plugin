# cross-section-field-data-plugin

[![Build status](https://ci.appveyor.com/api/projects/status/rplg2foqo77g2kih/branch/master?svg=true)](https://ci.appveyor.com/project/SystemsAdministrator/cross-section-field-data-plugin/branch/master)

An AQTS field data plugin to import cross section survey measurements.

![Cross Section Survey](images/CrossSectionDiagram.png)

## Want to install this plugin?

- Download the latest release of the plugin [here](../../releases/latest)
- Install it using the [FieldVisitPluginTool](https://github.com/AquaticInformatics/aquarius-field-data-framework/tree/master/src/FieldDataPluginTool)

## Supported CSV format

The supported CSV file has the following text rules:
- UTF-8 encoding is assumed. The UTF-8 byte-order-mark of (`0xEF, 0xBB, 0xBF`) at the start of the file is completely optional.
- Lines starting with a `#` as the first non-whitespace character are ignored as comment-lines.
- Completely blank lines are ignored.
- The header line (line 14 in the [example file](#example-file) below) can be repeated any number of times, and will always be ignored. The field names in the header line must match the English field names listed in the table below.
- Leading/trailing whitespace is allowed between fields.
- Double-quotes are only required if a field value contains a comma, or if the value must have leading/trailing whitespace.
- Multi-line text values are not supported. (ie. the **Comment** field must be a single line)

### Starting line

The first non-comment, non-blank line must be `AQUARIUS Cross-Section CSV v1.0`. Leading/trailing whitespace is ignored.

If the first line does not match the expected pattern, the plugin will not attempt to parse the file.

### Field lines

Lines that follow the `<FieldName> : <FieldValue>` pattern will not be parsed as CSV data rows, but will set some field values which apply to the entire cross-section measurement.

- Leading/trailing whitespace is ignored
- The whitespace separating `<FieldName>`, the colon `:`, and `<FieldValue>` is ignored.
- The `<FieldName>`s are case-insensitive.

| Field name | Data type | Required? | Description |
| --- | --- | --- | --- |
| **Location** | string | Y | The location identifier for this measurement. |
| **StartDate** | [Timestamp](#timestamps) | Y | The starting time of the survey. |
| **EndDate** | [Timestamp](#timestamps) | Y | The ending time of the survey. |
| **Party** | string | N | The party performing the survey. |
| **Channel** | string | N | The name of the channel at the location. If omitted, a value of `Main` will be used. |
| **RelativeLocation** | string | N | A description of where the measurement was taken. If omitted, a value of `At the control` will be used. |
| **Stage** | numeric | Y | The stage height at the time of the survey.  |
| **Unit** | string | Y | The stage height units. |
| **StartBank** | string | Y | The starting bank of the survey, The value must start with `Left`, `LEW`, `Right`, or `REW`. |
| **Comment** | string | N | An optional comment. |

### CSV Header lines

A header line does not need to appear at all in the CSV file.
But if it does exist, it must exactly match line 14 in the [example file](#example-file) below.
The header line must be the 3 column names, listed in the order below, separated by commas. Whitespace between column names is ignored.

### Column definitions for CSV data rows

| ColumnName | Data type | Required? | Description |
| --- | --- | --- | --- |
| **Distance** | numeric | Y | The distance from the start bank. |
| **Elevation** | numeric | Y | The elevation relative to the stage. Positive values are deeper (below the water line). Negative values are above the water line. |
| **Comment** | string | N | An optional comment |

### Timestamps

Timestamps are specified in ISO 8601 format. Specifically the `"O"` (roundtrip) format for .NET DateTimeOffset values is used.

`yyyy-MM-ddTHH:mm:ss.fffffffzzz`

- The `T` character must separate the date and time portions.
- All seven fractional seconds digits are optional. This 100 nanosecond precision is the full resolution supported by AQUARIUS Time-Series.
- The time-zone (`zzz`) portion can either be an explicit offset in hours/minutes (`+04:00` or `-00:30`) or it can be the UTC indicator of the letter `Z`.
- These constraints ensure that the timestamps contain no ambiguity.

See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip for more details.

### Example file

```
AQUARIUS Cross-Section CSV v1.0
 
Location: Server.Plugins.FieldVisit.CrossSection.Tests.CompleteCrossSection
StartDate: 2001-05-08T14:32:15+07:00
EndDate: 2001-05-08T17:12:45+07:00
Party: Cross-Section Party
Channel: Right overflow
RelativeLocation: At the Gage
Stage: 12.2
Unit: ft
StartBank: Left bank
Comment: Cross-section survey comments
 
Distance, Elevation, Comment
0, 7.467,
19.1, 6.909, "some comment"
44.8, 6.3, "yet, another, comment"
70.1, 5.356, another comment
82.4, 5.287,
```

## Requirements for building the plugin from source

- Requires Visual Studio 2017 (Community Edition is fine)
- .NET 4.7 runtime

## Building the plugin

- Load the `src\CrossSectionPlugin.sln` file in Visual Studio and build the `Release` configuration.
- The `src\CrossSectionPlugin\deploy\Release\CrossSectionPlugin.plugin` file can then be installed on your AQTS app server.

## Testing the plugin within Visual Studio

Use the included `PluginTester.exe` tool from the `Aquarius.FieldDataFramework` package to test your plugin logic on the sample files.

1. Open the EhsnPlugin project's **Properties** page
2. Select the **Debug** tab
3. Select **Start external program:** as the start action and browse to `"src\packages\Aquarius.FieldDataFramework.17.4.1\tools\PluginTester.exe`
4. Enter the **Command line arguments:** to launch your plugin

```
/Plugin=CrossSectionPlugin.dll /Json=AppendedResults.json /Data=..\..\..\..\data\CrossSection.csv
```

The `/Plugin=` argument can be the filename of your plugin assembly, without any folder. The default working directory for a start action is the bin folder containing your plugin.

5. Set a breakpoint in the plugin's `ParseFile()` methods.
6. Select your plugin project in Solution Explorer and select **"Debug | Start new instance"**
7. Now you're debugging your plugin!

See the [PluginTester](https://github.com/AquaticInformatics/aquarius-field-data-framework/tree/master/src/PluginTester) documentation for more details.

## Installation of the plugin

Use the [FieldDataPluginTool](https://github.com/AquaticInformatics/aquarius-field-data-framework/tree/master/src/FieldDataPluginTool) to install the plugin on your AQTS app server.
