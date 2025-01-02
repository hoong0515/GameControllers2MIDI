# Note
This program is currently tested with Nintendo Switch Pro Controller, USB Mode only.
If there is any problem on other environments, please notice me.

# GameControllers2MIDI

## Overview
Controllers2MIDI is a Windows application that enables game controllers (e.g., Xbox, Nintendo Switch Pro, PlayStation) to be mapped to MIDI signals. It is designed for musicians, developers, and creators looking to integrate controller inputs into their digital audio workstations (DAWs) or other MIDI-enabled software.

## Features
- **Controller Input Recognition**:
  - Supports Xbox, PlayStation, and Nintendo Switch Pro controllers.
- **Custom MIDI Mapping**:
  - Map controller buttons and axes to MIDI Notes, Control Changes (CC), or Pitch Bend.
  - Flexible options for mapping velocity, key, octave, and more.
- **Dynamic MIDI Channel Selection**:
  - Easily change the MIDI channel (1-16) via the user interface.
- **Profile Management**:
  - Save and load mapping profiles in JSON format.
- **Real-Time Feedback**:
  - Displays live input data from connected controllers.
- **Device Management**:
  - Detects connected controllers and MIDI devices dynamically.
  - Provides warnings for disconnected devices.

## Installation
1. Download the latest release from the [Releases](https://github.com/yourusername/Controllers2MIDI/releases) page.
2. Extract the files and run `Controllers2MIDI.exe`.

## Requirements
- Windows 10 or higher
- .NET Framework 4.7.2 or later
- A compatible game controller (e.g., Xbox, Nintendo Switch Pro, DualShock 4, or DualSense)
- MIDI-enabled software or hardware to receive the output
- (optional) Virtual MIDI driver e.g. [loopMIDI](https://www.tobias-erichsen.de/software/loopmidi.html), [LoopBe1](https://www.nerds.de/en/loopbe1.html) (to send MIDI messages to DAW)

## Usage
1. **Connect Your Devices**:
   - Connect a compatible controller via USB or Bluetooth.
   - Ensure your MIDI device is connected and recognized by the system.

2. **Launch the Application**:
   - Run the `GameControllers2MIDI.exe`.

3. **Configure Settings**:
   - Select a controller and a MIDI device from the dropdown menus.
   - Adjust the MIDI channel as needed.
   - Use the DataGridView to customize button and axis mappings.

4. **Save or Load Profiles**:
   - Save your current mapping as a JSON file for future use.
   - Load an existing mapping profile to quickly reconfigure.

5. **Start Playing**:
   - Input from your controller will now be converted into MIDI signals.

## Known Issues
- MIDI device list may not dynamically update in certain rare cases.
- Limited support for third-party or non-standard controllers.

## Contribution
Feel free to submit issues or contribute to the project via pull requests on [GitHub](https://github.com/yourusername/Controllers2MIDI).

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

## Contact
For support or inquiries, contact [hoong0515@naver.com] or create an issue on the GitHub repository.

