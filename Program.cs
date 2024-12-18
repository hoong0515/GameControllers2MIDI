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

            MappingManager mappingManager = new MappingManager();
            DeviceManager deviceManager = new DeviceManager();
            MidiManager midiManager = new MidiManager();

            deviceManager.ScanDevices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UIManager(mappingManager, deviceManager, midiManager));
        }
    }
}