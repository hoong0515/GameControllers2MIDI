using System;
using System.Windows.Forms;
using SDL2;


namespace Controllers2MIDI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);

            DeviceManager deviceManager = new DeviceManager();
            deviceManager.StartUpdating();
            MappingManager mappingManager = new MappingManager();
            MidiManager midiManager = new MidiManager(deviceManager, mappingManager);

            deviceManager.ScanDevices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UIManager(mappingManager, deviceManager, midiManager));
        }
    }
}