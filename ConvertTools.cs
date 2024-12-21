namespace Controllers2MIDI
{

    // Note와, Key, Octave 간 변환을 위한 클래스
    public class ConvertTools
    {

        public static int CalculateNoteFromKeyAndOctave(Key key, int octave)
        {
            Dictionary<Key, int> keyMap = new Dictionary<Key, int>
            {
                { Key.C, 0 }, { Key.Db, 1 }, { Key.D, 2 }, { Key.Eb, 3 }, { Key.E, 4 },
                { Key.F, 5 }, { Key.Gb, 6 }, { Key.G, 7 }, { Key.Ab, 8 }, { Key.A, 9 },
                { Key.Bb, 10 }, { Key.B, 11 }


            };

            if (!keyMap.ContainsKey(key))
            {
                throw new ArgumentException($"Invalid key: {key}");
            }

            // MIDI 노트 번호 계산
            return (octave + 1) * 12 + keyMap[key];
        }

        public static (Key key, int octave) CalculateKeyAndOctaveFromNote(int note)
        {
            int octave = (note / 12) - 1;
            Key key = (Key)(note % 12);
            return (key, octave);
        }


    }
}