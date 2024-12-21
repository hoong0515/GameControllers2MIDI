using SDL2;

namespace Controllers2MIDI
{

    public enum inputType
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
        C = 0,        Db = 1,        D = 2,        Eb = 3,        E = 4,        F = 5,
        Gb = 6,        G = 7,        Ab = 8,        A = 9,        Bb = 10,        B = 11
    }


    // 매핑 클래스
    // 입력에 대한 매핑, 값, 반전 여부, 음정, 옥타브, Velocity를 저장
    public class Mapping
    {
        private inputType inputType;
        private dynamic input;
        private Map map = Map.Note;
        private int value = 1;
        private bool isInverted = false;
        private Key key = Key.C;
        private int oct = 4;
        private int velocity = 100;
        public Mapping(dynamic input, Map map, int value = 1, bool isInverted = false, Key key = Key.C, int oct = 4, int velocity = 100)
        {
            this.inputType = input.GetType() == typeof(SDL.SDL_GameControllerButton) ? inputType.Button : inputType.Axis;
            this.input = input;
            this.map = map;
            this.value = value;
            this.isInverted = isInverted;
            this.key = key;
            this.oct = oct;
            this.velocity = velocity;
        }

        public Mapping(SDL.SDL_GameControllerButton input, Map map, Key key = Key.C, int oct = 4, int velocity = 100)
        {
            this.inputType = inputType.Button;
            this.input = input;
            this.map = map;
            this.value = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
            this.key = key;
            this.oct = oct;
            this.velocity = velocity;
        }

        public Mapping(SDL.SDL_GameControllerButton input, Map map, int value, int velocity = 100)
        {
            this.inputType = inputType.Button;
            this.input = input;
            this.map = map;
            this.value = value;
            (this.key, this.oct) = ConvertTools.CalculateKeyAndOctaveFromNote(value);
            this.velocity = velocity;
        }

        public Mapping(SDL.SDL_GameControllerAxis input, Map map, int value = 1, bool isInverted = false)
        {
            this.inputType = inputType.Axis;
            this.input = input;
            this.map = map;
            this.value = value;
            this.isInverted = isInverted;
        }


        public dynamic Input
        {
            get { return input; } set { input = value; }
        }
        
        public Map Map
        {
            get { return map; } set { map = value; }
        }

        public int Value
        {
            get { return value; } set { this.value = value; }
        }

        public bool IsInverted
        {
            get { return isInverted; }  set { isInverted = value; }
        }

        public Key Key
        {
            get { return key; } set { key = value; }
        }

        public int Oct
        {
            get { return oct; } set { oct = value; }
        }

        public int Velocity
        {
            get { return velocity; }    set { velocity = value; }
        }

        public Dictionary<string, dynamic> ToDictionary()
        {
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>
            {
                { "inputType", inputType },
                { "input", input },
                { "map", map },
                { "value", value },
                { "isInverted", isInverted },
                { "key", key },
                { "oct", oct },
                { "velocity", velocity }
            };
            return dict;
        }
    }
}