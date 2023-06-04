# Couch Mover

This is a script for moving and manipulating couch structures in a radiation therapy treatment planning system. The script is developed using the Eclipse Scripting API for research purposes.

## Requirements

- Visual Studio with C# support
- Eclipse Treatment Planning System (TPS)

## Installation

1. Clone or download the repository.
2. Open the solution in Visual Studio.
3. Build the solution to generate the script executable.
4. In Eclipse, go to the Scripting workspace and load the script executable.

## Usage

1. Launch the script from Eclipse.
2. The script will open a graphical user interface (UI) named "LotusMoon".
3. The UI will display buttons and information based on the couch insertion and position.
4. Follow the instructions on the UI to interact with the script and perform couch-related operations.

## Features

- Insert couch structures of different types.
- Calculate the coarse distance to move the couch.
- Calculate the fine distance to move the couch using an iterative optimization approach.
- Detect collisions between the couch and treatment plans.
- Provide warnings and errors for possible collisions.
- Update the UI dynamically based on the couch position and operations.

## Customization

- To replace the version attributes, create an `AssemblyInfo.cs` file in the project properties and specify the desired version information.
- To enable write access, uncomment the `[assembly: ESAPIScript(IsWriteable = true)]` attribute.

## Disclaimer

- This script is provided as-is without any warranty or support.
- Use this script at your own risk.
- The script has been tested on Eclipse TPS, but it may not be compatible with future versions or different treatment planning systems.

## Author

- Rohit Inippully(mailto:rohitinippully@gmail.com)