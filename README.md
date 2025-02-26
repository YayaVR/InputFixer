# InputFixer - VR Input To VRCHAT OSC Movement Tool

**InputFixer** is a lightweight application designed assist when the left controller up/down tilt doesn't reach the intended full tilt value anymore by detecting controller tilt and when those reach a given threshold, sends movement inputs to VRChat using OSC.

---

## Features
- Detects positive and negative tilt on the left VR controller.
- Sends OSC messages to VRChat to trigger forward or backward movement.
- Adjustable threshold value to fine-tune controller sensitivity.
- Supports any SteamVR-compatible controllers (e.g., Vive, Index, Oculus).
- Easy configuration via a simple `config.txt` file.

---

## Requirements
- **SteamVR**: The application requires SteamVR to be installed and set up.
- **VRChat**: A running VRChat instance that listens for OSC messages.
- **.NET Runtime**: This application is built with .NET, so you’ll need the .NET runtime installed on your machine.

---

## Installation
1. Download the latest release from the GitHub repository.
2. Extract the contents of the archive to a folder.
3. Run the `InputFixer.exe` executable to start the application.
4. The `config.txt` file will be created automatically if it doesn't exist. You can edit this file to customize the threshold value.

---

## Configuration
The threshold value that determines the controller tilt sensitivity can be modified in the `config.txt` file. By default, the threshold is set to `0.75`. To change it, simply open the `config.txt` file and modify the value.

---


## Usage
1. Ensure SteamVR is running and your VR controller is connected.
2. Run the `InputFixer.exe` application.
3. The application will automatically connect to VRChat via OSC (default: `127.0.0.1:9000`).
4. Tilt the left controller to trigger forward or backward movement in VRChat.

---

## Credits
- **Valve** for **OpenVR**, which provides the interface for VR hardware and tracking.  
  [OpenVR GitHub Repository](https://github.com/ValveSoftware/openvr)
  
- **VolcanicArts** for **FastOSC**, which is used for sending OSC messages in this application.  
  [FastOSC GitHub Repository](https://github.com/VolcanicArts/FastOSC)

---

## License
This project is open source and available under the MIT License.

---

## Troubleshooting
- If the application doesn't detect the controller, ensure that SteamVR is correctly set up and the controller is paired and tracked.
- If the OSC messages are not being sent to VRChat, verify that VRChat's OSC settings are configured to listen on the correct port (`9000` by default).
