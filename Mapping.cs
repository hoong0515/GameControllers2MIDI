using System.ComponentModel;
using System.Runtime.CompilerServices;
using SDL2;

namespace GameControllers2MIDI
{

    public enum InputType
    {
        Button = 0,
        Axis = 1
    }
    public enum Map
    {
        Note = 0,
        CC = 1,
        Pitchbend = 2
    }

    public enum Key
    {
        C = 0, Db = 1, D = 2, Eb = 3, E = 4, F = 5,
        Gb = 6, G = 7, Ab = 8, A = 9, Bb = 10, B = 11
    }


    // 매핑 클래스
    // 입력에 대한 매핑, 값, 반전 여부, 음정, 옥타브, Velocity를 저장
    public class Mapping : INotifyPropertyChanged
    //public class Mapping
    {
        //public static Dictionary<dynamic, int> usingInputs = new Dictionary<dynamic, int>();
        private InputType inputType = InputType.Button;
        private dynamic input = SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A;

        private Map map = Map.Note;
        private int value = 60;
        private bool isInverted = false;
        private Key key = Key.C;
        private int oct = 4;
        private int velocity = 100;
        private bool isUsingAbs = false;


        private string inputName = "BUTTON_A";
        private string mapName = "Note";
        private string keyName = "C";
        private string octName = "4";

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public Mapping()
        {
            this.inputName = this.input.ToString().Substring(15);
            this.mapName = this.map.ToString();
            this.keyName = this.key.ToString();
            this.octName = this.oct.ToString();
            //if(!usingInputs.ContainsKey(this.input))
            //{
            //    usingInputs[this.input] = 0;
            //}
            //else
            //{
            //    usingInputs[this.input]++;
            //}

        }
        public Mapping(dynamic input, Map map, int value = 1, bool isInverted = false, Key key = Key.C, int oct = 4, int velocity = 100, bool isUsingAbs = false)
        {
            this.inputType = input.GetType() == typeof(SDL.SDL_GameControllerButton) ? InputType.Button : InputType.Axis;
            this.input = input;
            this.map = map;
            this.value = value;
            this.isInverted = isInverted;
            this.key = key;
            this.oct = oct;
            this.velocity = velocity;
            this.isUsingAbs = isUsingAbs;

            this.inputName = this.input.ToString().Substring(15);
            this.mapName = this.map.ToString();
            this.keyName = this.key.ToString();
            this.octName = this.oct.ToString();

            //if (!usingInputs.ContainsKey(this.input))
            //{
            //    usingInputs[this.input] = 0;
            //}
            //else
            //{
            //    usingInputs[this.input]++;
            //}
        }

        public Mapping(SDL.SDL_GameControllerButton input, Map map, Key key = Key.C, int oct = 4, int velocity = 100)
        {
            this.inputType = InputType.Button;
            this.input = input;
            this.map = map;
            this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
            this.key = key;
            this.oct = oct;
            this.velocity = velocity;

            this.inputName = this.input.ToString().Substring(15);
            this.mapName = this.map.ToString();
            this.keyName = this.key.ToString();
            this.octName = this.oct.ToString();
            //if (!usingInputs.ContainsKey(this.input))
            //{
            //    usingInputs[this.input] = 0;
            //}
            //else
            //{
            //    usingInputs[this.input]++;
            //}
        }

        public Mapping(SDL.SDL_GameControllerButton input, Map map, int value, int velocity = 100)
        {
            this.inputType = InputType.Button;
            this.input = input;
            this.map = map;
            this.value = value;
            (this.key, this.oct) = ConvertTools.CalculateKeyAndOctaveFromNote(value);
            this.velocity = velocity;

            this.inputName = this.input.ToString().Substring(15);
            this.mapName = this.map.ToString();
            this.keyName = this.key.ToString();
            this.octName = this.oct.ToString();
            //if (!usingInputs.ContainsKey(this.input))
            //{
            //    usingInputs[this.input] = 0;
            //}
            //else
            //{
            //    usingInputs[this.input]++;
            //}
        }

        public Mapping(SDL.SDL_GameControllerAxis input, Map map, int value = 1, bool isInverted = false, bool isUsingAbs = false)
        {
            this.inputType = InputType.Axis;
            this.input = input;
            this.map = map;
            this.value = value;
            this.isInverted = isInverted;
            this.isUsingAbs = isUsingAbs;

            this.inputName = this.input.ToString().Substring(15);
            this.mapName = this.map.ToString();
            this.keyName = this.key.ToString();
            this.octName = this.oct.ToString();
            //if (!usingInputs.ContainsKey(this.input))
            //{
            //    usingInputs[this.input] = 0;
            //}
            //else
            //{
            //    usingInputs[this.input]++;
            //}
        }



        public InputType InputType
        {
            get { return inputType; }
            set { inputType = value; }
        }

        public dynamic Input
        {
            get { return input; }
            set
            {
                input = value;
                InputType newInputType = typeof(SDL.SDL_GameControllerButton) == value.GetType() ? InputType.Button : InputType.Axis;
                if (inputType != newInputType)
                {
                    if (newInputType == InputType.Button)
                    {
                        SetButtonDefalut();
                    }
                    else
                    {
                        SetAxisDefalut();
                    }
                    inputType = newInputType;

                }
                this.InputName = value.ToString().Substring(15);
                NotifyPropertyChanged("Input");
            }
        }

        public Map Map
        {
            get { return map; }
            set { map = value; this.MapName = value.ToString(); NotifyPropertyChanged("Map"); }
        }

        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                (this.Key, this.Oct) = ConvertTools.CalculateKeyAndOctaveFromNote(value);
                NotifyPropertyChanged("Value");
            }
        }

        public bool IsInverted
        {
            get { return isInverted; }
            set { isInverted = value; NotifyPropertyChanged("IsInverted"); }
        }

        public Key Key
        {
            get { return key; }
            set
            {
                key = value;
                if (keyName != value.ToString())
                {
                    this.KeyName = value.ToString();
                }
                this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
                NotifyPropertyChanged("Key");
                NotifyPropertyChanged("Value");
            }
        }

        public int Oct
        {
            get { return oct; }
            set
            {
                oct = value;
                if (octName != value.ToString())
                {
                    this.OctName = value.ToString();
                }
                this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
                NotifyPropertyChanged("Oct");
                NotifyPropertyChanged("Value");
            }
        }

        public int Velocity
        {
            get { return velocity; }
            set { velocity = value; NotifyPropertyChanged("Velocity"); }
        }

        public bool IsUsingAbs
        {
            get { return isUsingAbs; }
            set { isUsingAbs = value; NotifyPropertyChanged("IsUsingAbs"); }
        }



        public string InputName
        {
            get { return inputName; }
            set { inputName = value; NotifyPropertyChanged("InputName"); }
        }

        public string MapName
        {
            get { return mapName; }
            set { mapName = value; Enum.TryParse<Map>(value, out map); NotifyPropertyChanged("MapName"); }
        }

        public string KeyName
        {
            get { return keyName; }
            set
            {
                keyName = value;
                Key newKey;
                Enum.TryParse<Key>(value, out newKey);
                this.Key = newKey;
                NotifyPropertyChanged("KeyName");
            }
        }

        public string OctName
        {
            get { return octName; }
            set
            {
                octName = value;
                this.Oct = int.Parse(value);
                NotifyPropertyChanged("OctName");
            }
        }



        public Dictionary<string, dynamic> ToDictionary(bool getEnumName = false)
        {
            Dictionary<string, dynamic> dict;
            if (!getEnumName)
            {
                dict = new Dictionary<string, dynamic>
                {
                    { "inputType", inputType },
                    { "input", input },
                    { "map", map },
                    { "value", value },
                    { "isInverted", isInverted },
                    { "key", key },
                    { "oct", oct },
                    { "velocity", velocity },
                    { "isUsingAbs", isUsingAbs }
                };
            }
            else
            {
                dict = new Dictionary<string, dynamic>
                {
                    { "input", input.ToString() },
                    { "map", map.ToString()},
                    { "value", value },
                    { "isInverted", isInverted },
                    { "key", key.ToString() },
                    { "oct", oct },
                    { "velocity", velocity },
                    { "isUsingAbs", isUsingAbs }
                };
            }

            return dict;
        }

        public void ModifyNoteProperty(int value, Key key, int oct)
        {
            if ((key, oct) != (this.key, this.oct))
            {
                this.key = key;
                this.oct = oct;
                this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
            }
            else if (value != this.value)
            {
                this.value = value;
                (this.key, this.oct) = ConvertTools.CalculateKeyAndOctaveFromNote(value);
            }
            NotifyPropertyChanged();
        }

        public void ModifyNoteProperty(int value)
        {
            if (value != this.value)
            {
                this.value = value;
                (this.key, this.oct) = ConvertTools.CalculateKeyAndOctaveFromNote(value);
            }
            NotifyPropertyChanged();
        }

        public void ModifyNoteProperty(Key key, int oct)
        {
            if ((key, oct) != (this.key, this.oct))
            {
                this.key = key;
                this.oct = oct;
                this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
            }
            NotifyPropertyChanged();
        }

        private void SetButtonDefalut()
        {
            this.Map = Map.Note;
            this.Value = 60;
            this.IsInverted = false;
            this.Key = Key.C;
            this.Oct = 4;
            this.Velocity = 100;
            this.IsUsingAbs = false;
        }

        private void SetAxisDefalut()
        {
            this.Map = Map.CC;
            this.Value = 1;
            this.IsInverted = false;
            this.IsUsingAbs = false;
        }

    }
}