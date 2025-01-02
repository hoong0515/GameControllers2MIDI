using NAudio.Midi;
using SDL2;
namespace GameControllers2MIDI
{
    public class MidiManager
    {
        private MidiOut midiOut;
        private int midiChannel = 1; // 기본 MIDI 채널
        private bool isProcessing = false;
        private DeviceManager deviceManager;
        private MappingManager mappingManager;

        // 상태 추적
        private Dictionary<int, bool> noteStates = new Dictionary<int, bool>(); // Note On/Off 상태
        private Dictionary<int, int> ccStates = new Dictionary<int, int>(); // CC 값
        private Dictionary<SDL.SDL_GameControllerButton, bool> buttonStates = new Dictionary<SDL.SDL_GameControllerButton, bool>(); // 버튼 상태
        private int lastPitchBendValue = 0; // Pitch Bend 값 (초기값은 중앙)

        private Dictionary<int, string> connectedMidiDevices = new Dictionary<int, string>();
        private int? activeMidiDeviceIndex; // 활성 MIDI 장치의 인덱스



        public MidiManager()
        {
        }

        public MidiManager(DeviceManager deviceManager, MappingManager mappingManager)
        {
            this.deviceManager = deviceManager;
            // DeviceManager 이벤트 구독
            deviceManager.ButtonPressed += HandleButtonInput;
            deviceManager.ButtonReleased += HandleButtonInput;
            deviceManager.AxisMoved += HandleAxisInput;
            this.mappingManager = mappingManager;

            foreach (SDL.SDL_GameControllerButton button in Enum.GetValues(typeof(SDL.SDL_GameControllerButton)))
            {
                buttonStates[button] = false;
            }
        }






        public void StartProcessing(IntPtr activeController, MappingManager mappingManager)
        {
            if (isProcessing)
            {
                Console.WriteLine("MIDI Processing is already running.");
                return;
            }

            isProcessing = true;

        }


        public void StopProcessing()
        {
            if (!isProcessing)
            {
                Console.WriteLine("MIDI Processing is not running.");
                return;
            }

            isProcessing = false;
        }

        private void HandleButtonInput(SDL.SDL_GameControllerButton button)
        {
            if (!isProcessing)
            {
                return;
            }

            // 현재 버튼 상태 가져오기
            bool isPressed = SDL.SDL_GameControllerGetButton(deviceManager.GetActiveController(), button) == 1;

            // 버튼 상태가 변경된 경우만 처리
            if (isPressed && (!buttonStates.ContainsKey(button) || !buttonStates[button]))
            {
                buttonStates[button] = true; // 상태 업데이트
                List<Mapping> buttonMappings = mappingManager.GetButtonMappings(button);

                foreach (Mapping mapping in buttonMappings)
                {
                    if (!mapping.IsInverted)
                    {
                        SendNoteOn(mapping.Value, mapping.Velocity);
                    }
                    else
                    {
                        SendNoteOff(mapping.Value);
                    }
                }
            }
            else if (!isPressed && buttonStates.ContainsKey(button) && buttonStates[button])
            {
                // Note Off 처리
                buttonStates[button] = false; // 상태 업데이트
                List<Mapping> buttonMappings = mappingManager.GetButtonMappings(button);

                foreach (Mapping mapping in buttonMappings)
                {
                    if (!mapping.IsInverted)
                    {
                        SendNoteOff(mapping.Value);
                    }
                    else
                    {
                        SendNoteOn(mapping.Value, mapping.Velocity);
                    }
                }
            }

        }

        private void HandleAxisInput(SDL.SDL_GameControllerAxis axis, short value)
        {
            if (!isProcessing)
            {
                return;
            }
            Console.WriteLine($"MIDI: Axis {axis} moved with value {value}");
            // MIDI 메시지 전송 로직
            short axisValue = SDL.SDL_GameControllerGetAxis(deviceManager.GetActiveController(), axis);
            List<Mapping> axisMappings = mappingManager.GetAxisMappings(axis);

            foreach (Mapping mapping in axisMappings)
            {
                switch (mapping.Map)
                {
                    case Map.CC:
                        int ccNumber = mapping.Value;
                        int ccValue = MapToMIDIValue(axisValue, mapping.IsInverted, mapping.IsUsingAbs);
                        SendControlChange(ccNumber, ccValue);
                        break;
                    case Map.Pitchbend:
                        int pitchBendValue = MapToPitchBend(axisValue, mapping.IsInverted);
                        SendPitchBend(pitchBendValue);
                        break;
                }
            }
        }



        private int MapToMIDIValue(short axisValue, bool isInverted, bool isUsingAbs)
        {
            int midiValue = isUsingAbs ? Math.Abs(axisValue * 127 / 32767) : (axisValue + 32768) * 127 / 65535;
            return isInverted ? 127 - midiValue : midiValue;
        }
        private int MapToPitchBend(short axisValue, bool invertDirection)
        {
            if (invertDirection)
            {
                axisValue = (short)-axisValue;
            }

            return axisValue < 0
                ? ((axisValue * 8192 / 32768) + 8192) * -1
                : ((axisValue * 8191 / 32767) - 8191) * -1;
        }

        public void SendNoteOn(int note, int velocity)
        {
            if (noteStates.ContainsKey(note) && noteStates[note])
            {
                // 이미 Note On 상태라면 메시지를 보내지 않음
                return;
            }

            noteStates[note] = true; // Note On 상태로 변경
            midiOut.Send(MidiMessage.StartNote(note, velocity, midiChannel).RawData);
            Console.WriteLine($"Note On: Note={note}, Velocity={velocity}, Channel={midiChannel}");
        }

        // Note Off 메시지 전송
        public void SendNoteOff(int note)
        {
            if (!noteStates.ContainsKey(note) || !noteStates[note])
            {
                // 이미 Note Off 상태라면 메시지를 보내지 않음
                return;
            }

            noteStates[note] = false; // Note Off 상태로 변경
            midiOut.Send(MidiMessage.StopNote(note, 0, midiChannel).RawData);
            Console.WriteLine($"Note Off: Note={note}, Channel={midiChannel}");
        }

        // Control Change 메시지 전송
        public void SendControlChange(int controller, int value)
        {


            if (!ccStates.TryGetValue(controller, out var current) || current != value)
            {
                ccStates[controller] = value;
                midiOut.Send(MidiMessage.ChangeControl(controller, value, midiChannel).RawData);
                Console.WriteLine($"Control Change: Controller={controller}, Value={value}, Channel={midiChannel}");
            }

        }

        // Pitch Bend 메시지 전송
        public void SendPitchBend(int value)
        {
            if (value == lastPitchBendValue)
            {
                // 이전 값과 동일하면 메시지를 보내지 않음
                return;
            }

            lastPitchBendValue = value; // Pitch Bend 값 업데이트

            // LSB와 MSB로 분리
            int lsb = value & 0x7F;          // 하위 7비트
            int msb = (value >> 7) & 0x7F;   // 상위 7비트



            midiOut.Send(0xE0 | (midiChannel - 1) | (lsb << 8) | (msb << 16));
            Console.WriteLine($"Pitch Bend: Value={value}, LSB={lsb}, MSB={msb}");
        }
        public List<string> GetMidiDeviceNames()
        {
            List<string> deviceNames = new List<string>();
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                deviceNames.Add(MidiOut.DeviceInfo(i).ProductName);
            }
            return deviceNames;
        }

        public int GetMidiChannel()
        {
            return midiChannel;
        }



        public void SetActiveMidiDevice(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex >= MidiOut.NumberOfDevices)
            {
                throw new ArgumentException("Invalid MIDI device index.");
            }

            // 기존 MIDI 장치 닫기
            midiOut?.Close();

            // 새로운 MIDI 장치 열기
            midiOut = new MidiOut(deviceIndex);
            activeMidiDeviceIndex = deviceIndex;
            Console.WriteLine($"Connected to MIDI Device: {MidiOut.DeviceInfo(deviceIndex).ProductName}");
        }


        public void SetMidiChannel(int channel)
        {
            if (channel < 1 || channel > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(channel), "MIDI channel must be between 1 and 16.");
            }

            midiChannel = channel;
        }


        public void Close()
        {
            midiOut.Close();
            Console.WriteLine("MIDI Device disconnected.");
        }

        ~MidiManager()
        {
            Close();
        }
    }
}