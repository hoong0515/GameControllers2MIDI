using SDL2;


namespace GameControllers2MIDI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);


            // Initialize WinForms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DeviceManager deviceManager = new DeviceManager();
            deviceManager.StartUpdating();
            MappingManager mappingManager = new MappingManager();
            MidiManager midiManager = new MidiManager(deviceManager, mappingManager);
            UIManager uiManager = new UIManager(mappingManager, deviceManager, midiManager);

            deviceManager.ScanDevices();

            // Set timer & connect to events
            var timer = new System.Timers.Timer(300); // Check device connecting status every 300ms 
            timer.Elapsed += (sender, e) =>
            {
                deviceManager.UpdateControllerList();
                deviceManager.CheckActiveController();

            };

            deviceManager.ControllerListUpdated += uiManager.UpdateControllerDropdown;
            deviceManager.ControllerDisconnected += uiManager.ShowControllerDisconnectedWarning;


            timer.Start();

            // Execute UI
            Application.Run(uiManager);
        }
    }
}