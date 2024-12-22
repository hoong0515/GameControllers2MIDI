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


            // WinForms 초기화
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DeviceManager deviceManager = new DeviceManager();
            deviceManager.StartUpdating();
            MappingManager mappingManager = new MappingManager();
            MidiManager midiManager = new MidiManager(deviceManager, mappingManager);
            UIManager uiManager = new UIManager(mappingManager, deviceManager, midiManager);

            deviceManager.ScanDevices();

            // 타이머 설정 및 이벤트 연결
            var timer = new System.Timers.Timer(300); // 300ms 간격으로 상태 확인
            timer.Elapsed += (sender, e) =>
            {
                deviceManager.UpdateControllerList();
                deviceManager.CheckActiveController();


                midiManager.UpdateMidiDeviceList();
                midiManager.CheckActiveMidiDevice();
            };

            deviceManager.ControllerListUpdated += uiManager.UpdateControllerDropdown;
            deviceManager.ControllerDisconnected += uiManager.ShowControllerDisconnectedWarning;

            midiManager.MidiDeviceListUpdated += uiManager.UpdateMidiDropdown;
            midiManager.MidiDeviceDisconnected += uiManager.ShowMidiDeviceDisconnectedWarning;

            timer.Start();

            // UI 실행
            Application.Run(uiManager);
        }
    }
}