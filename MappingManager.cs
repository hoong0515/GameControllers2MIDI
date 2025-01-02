using System;
using System.Collections.Generic;
using SDL2;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;

namespace GameControllers2MIDI
{
    public class MappingManager
    {
        // 매핑 데이터 저장
        // Mapping 클래스 사용
        private BindingList<Mapping> mappings = new BindingList<Mapping>();


        // 생성자
        // 기본 버튼, Axis 매핑 추가
        public MappingManager()
        {
            AddButtonMapping(SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A, note: 60, velocity: 100); // Middle C
            AddButtonMapping(SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B, note: 62, velocity: 100); // D Note
            AddAxisMapping(SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX, isInverted: true, isPitchbend: true); // 오른쪽 X축을 Pitch Bend에 매핑
            AddAxisMapping(SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY, cc: 11, isUsingAbs: true); // Expression
            AddAxisMapping(SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, cc: 1, isUsingAbs: true); // Modulation

        }

        // 버튼 매핑 반환
        public List<Mapping> GetButtonMappings(SDL.SDL_GameControllerButton button)
        {
            List<Mapping> buttonMappings = new List<Mapping>();
            foreach (Mapping mapping in mappings)
            {
                if (mapping.Input is SDL.SDL_GameControllerButton && (SDL.SDL_GameControllerButton)mapping.Input == button)
                {
                    buttonMappings.Add(mapping);
                }
            }
            return buttonMappings;
        }

        // Axis 매핑 반환
        public List<Mapping> GetAxisMappings(SDL.SDL_GameControllerAxis axis)
        {
            List<Mapping>? axisMappings = new List<Mapping>();
            foreach (Mapping mapping in mappings)
            {
                if (mapping.Input is SDL.SDL_GameControllerAxis && (SDL.SDL_GameControllerAxis)mapping.Input == axis)
                {
                    axisMappings.Add(mapping);
                }
            }
            
            return axisMappings;
        }

        public BindingList<Mapping> GetAllMappings()
        {
            return mappings;
        }

        // 버튼 매핑 추가
        // 버튼, note, velocity(임의)를 받아서 매핑 추가
        // 또는 버튼, key, oct, velocity를 받아서 매핑 추가

        public void AddButtonMapping(SDL.SDL_GameControllerButton button, int note = 60, int velocity = 100)
        {
            if (note < 0 || note > 127 || velocity < 0 || velocity > 127)
            {
                throw new ArgumentOutOfRangeException("Note and velocity must be between 0 and 127.");
            }
            //buttonToNoteMap[button] = (note, velocity);
            mappings.Add(new Mapping(button, Map.Note, note, velocity: 100));
        }

        public void AddButtonMapping(SDL.SDL_GameControllerButton button, Key key = Key.C, int oct = 4, int velocity = 100)
        {
            int note = ConvertTools.CalculateNoteFromKeyAndOctave(key, oct);
            if (note < 0 || note > 127 || velocity < 0 || velocity > 127)
            {
                throw new ArgumentOutOfRangeException("Note and velocity must be between 0 and 127.");
            }
            //buttonToNoteMap[button] = (note, velocity);
            mappings.Add(new Mapping(button, Map.Note, key: Key.C, oct: 4, velocity: 100));
        }


        // Axis 매핑 추가
        public void AddAxisMapping(SDL.SDL_GameControllerAxis axis, int cc = 1, bool isInverted = false, bool isPitchbend = false, bool isUsingAbs = false)
        {
            //axisToCCMap[axis] = (cc, isInverted);
            if (isPitchbend)
            {
                mappings.Add(new Mapping(axis, Map.Pitchbend, isInverted: isInverted));
            }
            else
            {
                mappings.Add(new Mapping(axis, Map.CC, value: cc, isInverted: isInverted, isUsingAbs));
            }
        }

        // 매핑 추가
        public void AddMapping(Mapping mapping)
        {
            mappings.Add(mapping);
        }


        // 매핑 삭제
        // mapping 인스턴스를 받아서 삭제
        // 또는 input과 value를 받아서 삭제
        public void RemoveMapping(Mapping mapping)
        {
            //Mapping.usingInputs[mapping.Input]--;
            //if (Mapping.usingInputs[mapping.Input] < 0)
            //{
            //    Mapping.usingInputs.Remove(mapping.Input);
            //}
            mappings.Remove(mapping);
        }

        public void RemoveMapping(int index)
        {
            mappings.RemoveAt(index);
        }


        // 매핑을 JSON 파일로 저장
        public void SaveMappingToJson(string filePath)
        {

            List<Dictionary<string, dynamic>> mappingData = new List<Dictionary<string, dynamic>>();
            foreach (var mapping in mappings)
            {
                Dictionary<string, dynamic> mappingDict = mapping.ToDictionary();
                mappingData.Add(mappingDict);
            }



            string json = JsonConvert.SerializeObject(mappingData, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Mapping saved to {filePath}");
        }


        // JSON 파일로부터 매핑 로드
        public void LoadMappingFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                mappings.Clear();
                string json = File.ReadAllText(filePath);
                var mappingData = JsonConvert.DeserializeObject<dynamic>(json);
                foreach (var mappingDict in mappingData)
                {
                    if (mappingDict["inputType"] == 0)
                    {
                        AddButtonMapping((SDL.SDL_GameControllerButton)mappingDict["input"], note: (int)mappingDict["value"], velocity: (int)mappingDict["velocity"]);
                    }
                    else if (mappingDict["inputType"] == 1)
                    {
                        if (mappingDict["map"] == 1)
                        {
                            AddAxisMapping((SDL.SDL_GameControllerAxis)mappingDict["input"], cc: (int)mappingDict["value"], isInverted: (bool)mappingDict["isInverted"], isUsingAbs: (bool)mappingDict["isUsingAbs"]);
                        }
                        else if (mappingDict["map"] == 2)
                        {
                            AddAxisMapping((SDL.SDL_GameControllerAxis)mappingDict["input"], isInverted: (bool)mappingDict["isInverted"], isPitchbend: true);
                        }
                    }
                }

                Console.WriteLine($"Mapping loaded successfully from {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mapping: {ex.Message}");
            }
        }




    }

}