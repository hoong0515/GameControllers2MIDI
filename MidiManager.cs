using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Midi;
using SDL2;

public class MidiManager
{
    private MidiOut midiOut;
    private int midiChannel = 1; // 기본 MIDI 채널
    private bool isProcessing = false;

    // 상태 추적
    private Dictionary<int, bool> noteStates = new Dictionary<int, bool>(); // Note On/Off 상태
    private Dictionary<int, int> ccStates = new Dictionary<int, int>(); // CC 값
    private int lastPitchBendValue = 0; // Pitch Bend 값 (초기값은 중앙)

    public MidiManager()
    {
    }


    public void StartProcessing(IntPtr activeController, MappingManager mappingManager)
    {
        if (isProcessing)
        {
            Console.WriteLine("MIDI Processing is already running.");
            return;
        }

        isProcessing = true;

        Task.Run(() =>
        {
            Console.WriteLine("MIDI Processing Started.");
            var buttonStates = new Dictionary<SDL.SDL_GameControllerButton, bool>();
            while (isProcessing)
            {
                SDL.SDL_GameControllerUpdate();
                foreach (SDL.SDL_GameControllerButton button in Enum.GetValues(typeof(SDL.SDL_GameControllerButton)))
                {
                    bool isPressed = SDL.SDL_GameControllerGetButton(activeController, button) == 1;
                    if (isPressed && (!buttonStates.ContainsKey(button) || !buttonStates[button]))
                    {
                        int? note = mappingManager.GetMappedNote(button);
                        if (note.HasValue)
                        {
                            SendNoteOn(note.Value, 127);
                        }
                    }
                    else if (!isPressed && buttonStates.ContainsKey(button) && buttonStates[button])
                    {
                        // Note Off 처리
                        int? note = mappingManager.GetMappedNote(button);
                        if (note.HasValue)
                        {
                            SendNoteOff(note.Value);
                        }
                    }

                    buttonStates[button] = isPressed;

                }

                foreach (SDL.SDL_GameControllerAxis axis in Enum.GetValues(typeof(SDL.SDL_GameControllerAxis)))
                {
                    short axisValue = SDL.SDL_GameControllerGetAxis(activeController, axis);

                    if (mappingManager.IsPitchBendAxis(axis, out bool inverDirection))
                    {
                        // Pitch Bend로 매핑된 축 처리
                        int pitchBendValue = MapToPitchBend(axisValue, inverDirection);
                        SendPitchBend(pitchBendValue);
                    }
                    else
                    {
                        var ccMapping = mappingManager.GetMappedCC(axis);
                        if (ccMapping.HasValue)
                        {
                            var (ccNumber, ccInvertDirection) = ccMapping.Value;
                            int ccValue = MapToMIDIValue(axisValue, ccInvertDirection);
                            SendControlChange(ccNumber.Value, ccValue);
                        }
                    }
                }


            }
        });
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

    private int MapToMIDIValue(short axisValue, bool invertDirection)
    {
        if (invertDirection)
        {
            axisValue = (short)-axisValue;
        }
        return (axisValue + 32768) * 127 / 65535;
    }
    private int MapToPitchBend(short axisValue, bool invertDirection)
    {
        if (invertDirection)
        {
            axisValue = (short)-axisValue;
        }

        return axisValue < 0
            ? (((axisValue * 8192) / 32768) + 8192) * -1
            : (((axisValue * 8191) / 32767) - 8191) * -1;
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
        if (ccStates.ContainsKey(controller) && ccStates[controller] == value)
        {
            // 이전 값과 동일하면 메시지를 보내지 않음
            return;
        }

        ccStates[controller] = value; // CC 값 업데이트
        midiOut.Send(MidiMessage.ChangeControl(controller, value, midiChannel).RawData);
        Console.WriteLine($"Control Change: Controller={controller}, Value={value}, Channel={midiChannel}");
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
        Console.WriteLine($"Connected to MIDI Device: {MidiOut.DeviceInfo(deviceIndex).ProductName}");
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
