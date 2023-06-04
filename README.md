# IGRT_Couch_Shifter

This is a script for inserting and automatically shifting Varian IGRT couch structure on ECLIPSE treatment planning system. Additionally, the UI will display if there is gantry clearance for coplanar beams in the loaded plan. The script is developed using the Eclipse Scripting API for research purposes.


## Requirements

- Visual Studio with C# support
- Eclipse Treatment Planning System (TPS)

## Installation

1. Clone or download the repository.
2. Open the solution in Visual Studio.
3. Build the solution to generate the script executable.
4. In Eclipse, go to the Script Approvals to approve the script executable.

## Usage

1. Launch the script from Eclipse.
2. The script will open a graphical user interface (UI).
3. The UI will display buttons and information based on the couch insertion and position.
4. Follow the instructions on the UI to interact with the script and perform couch-related operations.

## Features

- Insert thin, medium and thick Varian IGRT couch structures.
- Calculate the distance to move the couch to the Varian couch top on CT scan (a bit slow at the moment).
- Detect collisions between the couch and coplanar beams in the treatment plan.
- Provide warnings and errors for possible collisions.
- Update the UI dynamically based on the couch position and operations.


## Disclaimer

- This script is provided as-is without any warranty or support.
- Use this script at your own risk.
- The script has been tested on Eclipse TPS v16.

## Author

- Rohit Inippully(mailto:rohitinippully@gmail.com)