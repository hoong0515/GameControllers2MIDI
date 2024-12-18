using System;
using System.Collections.Generic;
using SDL2;
using System.IO;
using Newtonsoft.Json;

namespace Controllers2MIDI
{
    public class MappingManager
    {
        // 매핑 데이터 저장
        private Dictionary<SDL.SDL_GameControllerButton, int> buttonToNoteMap = new Dictionary<SDL.SDL_GameControllerButton, int>();
        private Dictionary<SDL.SDL_GameControllerAxis, (int ccOrNull, bool invertDirection)> axisToCCMap =
            new Dictionary<SDL.SDL_GameControllerAxis, (int, bool)>();

        private Dictionary<SDL.SDL_GameControllerAxis, bool> axisToPitchbend =
            new Dictionary<SDL.SDL_GameControllerAxis, bool>();



        public MappingManager()
        {
            // 기본 매핑 설정
            buttonToNoteMap[SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A] = 60; // Middle C
            buttonToNoteMap[SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B] = 62; // D Note
            axisToPitchbend[SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX] = true; // 오른쪽 X축을 Pitch Bend에 매핑
            axisToCCMap[SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY] = (11, false); // Expression
            axisToCCMap[SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX] = (1, false); // Modulation
        }

        // 버튼을 MIDI Note로 매핑
        public int? GetMappedNote(SDL.SDL_GameControllerButton button)
        {
            if (buttonToNoteMap.TryGetValue(button, out int note))
            {
                return note;
            }
            return null; // 매핑되지 않은 버튼
        }

        // 축을 MIDI Control Change로 매핑
        public (int? ccNumber, bool invertDirection)? GetMappedCC(SDL.SDL_GameControllerAxis axis)
        {
            if (axisToCCMap.TryGetValue(axis, out var mapping))
            {
                return (mapping.ccOrNull, mapping.invertDirection);
            }
            return null;
        }



        // 매핑 동적 설정 (외부 파일 또는 사용자 설정)
        public void SetMapping(Dictionary<SDL.SDL_GameControllerButton, int> buttonMap, Dictionary<SDL.SDL_GameControllerAxis, (int, bool)> axisMap, Dictionary<SDL.SDL_GameControllerAxis, bool> pbMap)
        {
            buttonToNoteMap = buttonMap;
            axisToCCMap = axisMap;
            axisToPitchbend = pbMap;
        }

        // 매핑 데이터 출력
        public void PrintCurrentMapping()
        {
            Console.WriteLine("Button-to-Note Mapping:");
            foreach (var mapping in buttonToNoteMap)
            {
                Console.WriteLine($"Button {mapping.Key}: Note {mapping.Value}");
            }

            Console.WriteLine("Axis-to-CC Mapping:");
            foreach (var mapping in axisToCCMap)
            {
                Console.WriteLine($"Axis {mapping.Key}: CC {mapping.Value}");
            }

            Console.WriteLine("Pitch Bend Axes:");
            foreach (var axis in axisToPitchbend)
            {
                Console.WriteLine($"Axis {axis} is mapped to Pitch Bend");
            }
        }
        // 축이 Pitch Bend에 매핑되어 있는지 확인
        public bool IsPitchBendAxis(SDL.SDL_GameControllerAxis axis, out bool invertDirection)
        {
            if (axisToPitchbend.TryGetValue(axis, out invertDirection))
            {
                return true;
            }
            invertDirection = false;
            return false;
        }

        public class MappingData
        {
            public Dictionary<string, int> ButtonToNote { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> AxisToCC { get; set; } = new Dictionary<string, int>();
            public List<string> axisToPitchbend { get; set; } = new List<string>();
        }

        public void SaveMappingToJson(string filePath)
        {
            var mappingData = new
            {
                buttonToNoteMap = buttonToNoteMap.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
                axisToCCMap = axisToCCMap.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => new { ccNumber = kvp.Value.Item1, invertDirection = kvp.Value.Item2 }
                ),
                axisToPitchbend = axisToPitchbend.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => kvp.Value
                )
            };

            string json = JsonConvert.SerializeObject(mappingData, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Mapping saved to {filePath}");
        }

        public void LoadMappingFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var mappingData = JsonConvert.DeserializeObject<dynamic>(json);

                // Button-to-Note 매핑
                buttonToNoteMap = ((IDictionary<string, int>)mappingData.buttonToNoteMap)
                    .ToDictionary(
                        kvp => Enum.Parse<SDL.SDL_GameControllerButton>(kvp.Key),
                        kvp => kvp.Value
                    );

                // Axis-to-CC 매핑
                axisToCCMap = ((IDictionary<string, dynamic>)mappingData.axisToCCMap)
                    .ToDictionary(
                        kvp => Enum.Parse<SDL.SDL_GameControllerAxis>(kvp.Key),
                        kvp => ((int)kvp.Value.ccNumber, (bool)kvp.Value.invertDirection)
                    );

                // Axis-to-Pitchbend 매핑
                axisToPitchbend = ((IDictionary<string, bool>)mappingData.axisToPitchbend)
                    .ToDictionary(
                        kvp => Enum.Parse<SDL.SDL_GameControllerAxis>(kvp.Key),
                        kvp => kvp.Value
                    );

                Console.WriteLine($"Mapping loaded successfully from {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mapping: {ex.Message}");
            }
        }

        //buttonToNoteMap = ((IDictionary<string, int>)mappingData.ButtonToNote)
        //    .ToDictionary(kvp => Enum.Parse<SDL.SDL_GameControllerButton>(kvp.Key), kvp => kvp.Value);

        //axisToCCMap = ((IDictionary<string, dynamic>)mappingData.AxisToCCMap)
        //    .ToDictionary(
        //        kvp => Enum.Parse<SDL.SDL_GameControllerAxis>(kvp.Key),
        //        kvp => ((int)kvp.Value.ccNumber, (bool)kvp.Value.invertDirection)
        //    );

        //axisToPitchbend = ((IDictionary<string, bool>)mappingData.AxisToPitchbend)
        //    .ToDictionary(
        //        kvp => Enum.Parse<SDL.SDL_GameControllerAxis>(kvp.Key),
        //        kvp => kvp.Value
        //    );


    }

}