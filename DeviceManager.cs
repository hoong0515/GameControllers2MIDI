using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GameControllers2MIDI

{public class DeviceManager
    {
        private Dictionary<int, IntPtr> connectedControllers = new Dictionary<int, IntPtr>();
        private IntPtr activeController = IntPtr.Zero;
        private int? activeControllerIndex = null;
        private string activeControllerName; // 활성화된 컨트롤러의 이름



        public event Action<SDL.SDL_GameControllerButton> ButtonPressed;
        public event Action<SDL.SDL_GameControllerButton> ButtonReleased;
        public event Action<SDL.SDL_GameControllerAxis, short> AxisMoved;
        public event Action<List<string>> ControllerListUpdated;
        public event Action<string> ControllerDisconnected;

        public DeviceManager()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER) < 0)
            {
                throw new Exception($"SDL Initialization failed: {SDL.SDL_GetError()}");
            }
        }

        public void StartUpdating()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    UpdateControllerState();
                    ProcessSDLEvents();
                    UpdateControllerList();
                    //Thread.Sleep(50); // CPU 사용량 조절
                }
            });
        }

        private void ProcessSDLEvents()
        {
            SDL.SDL_Event sdlEvent;
            while (SDL.SDL_PollEvent(out sdlEvent) == 1)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        UpdateControllerList();
                        break;
                }
            }
        }


        public void ScanDevices()
        {
            connectedControllers.Clear();

            int joystickCount = SDL.SDL_NumJoysticks();
            Console.WriteLine($"Detected {joystickCount} joystick(s).");

            for (int i = 0; i < joystickCount; i++)
            {
                if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE)
                {
                    IntPtr controller = SDL.SDL_GameControllerOpen(i);
                    if (controller != IntPtr.Zero)
                    {
                        connectedControllers[i] = controller;
                        Console.WriteLine($"Controller {i}: {SDL.SDL_GameControllerName(controller)} connected.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to open controller {i}: {SDL.SDL_GetError()}");
                    }
                }
                else
                {
                    Console.WriteLine($"Joystick {i} is not a game controller.");
                }
            }
        }
        public void UpdateControllerState()
        {
            if (activeController == IntPtr.Zero)
                return;

            SDL.SDL_GameControllerUpdate();

            // 버튼 입력 감지
            foreach (SDL.SDL_GameControllerButton button in Enum.GetValues(typeof(SDL.SDL_GameControllerButton)))
            {
                bool isPressed = SDL.SDL_GameControllerGetButton(activeController, button) == 1;
                if (isPressed)
                {
                    ButtonPressed?.Invoke(button); // 유효한 버튼 이벤트만 발생
                }
                else
                {
                    ButtonReleased?.Invoke(button);
                }

            }

            // 축 입력 감지
            foreach (SDL.SDL_GameControllerAxis axis in Enum.GetValues(typeof(SDL.SDL_GameControllerAxis)))
            {
                short axisValue = SDL.SDL_GameControllerGetAxis(activeController, axis);
                AxisMoved?.Invoke(axis, axisValue);
                
            }
        }


        public void UpdateControllerList()
        {
            var currentControllers = new Dictionary<int, IntPtr>();
            var currentNames = new List<string>();

            for (int i = 0; i < SDL.SDL_NumJoysticks(); i++)
            {
                if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE)
                {
                    if (!connectedControllers.ContainsKey(i))
                    {
                        IntPtr controllerHandle = SDL.SDL_GameControllerOpen(i);
                        if (controllerHandle != IntPtr.Zero)
                        {
                            currentControllers[i] = controllerHandle;
                            currentNames.Add(SDL.SDL_GameControllerName(controllerHandle));
                        }
                    }
                    else
                    {
                        currentControllers[i] = connectedControllers[i];
                        currentNames.Add(SDL.SDL_GameControllerName(connectedControllers[i]));
                    }
                }
            }

            // 새로 추가되거나 제거된 컨트롤러 확인
            if (!currentControllers.Keys.SequenceEqual(connectedControllers.Keys))
            {
                // 기존 컨트롤러 닫기
                foreach (var controller in connectedControllers.Values)
                {
                    if (!currentControllers.Values.Contains(controller))
                    {
                        SDL.SDL_GameControllerClose(controller);
                    }
                }

                // 새로운 상태로 갱신
                connectedControllers = currentControllers;

                // 이벤트 트리거
                ControllerListUpdated?.Invoke(currentNames);
            }
        }


        public void CheckActiveController()
        {
            if (activeControllerIndex.HasValue && !connectedControllers.ContainsKey(activeControllerIndex.Value))
            {
                // 제거된 컨트롤러의 이름 출력
                string disconnectedControllerName = activeControllerName ?? "Unknown Controller";

                // 활성화 상태 초기화
                activeControllerIndex = null;
                activeControllerName = null;

                ControllerDisconnected?.Invoke(disconnectedControllerName);

            }
        }



        public List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();

            foreach (var entry in connectedControllers)
            {
                string name = SDL.SDL_GameControllerName(entry.Value);
                if (!string.IsNullOrEmpty(name))
                {
                    controllerNames.Add(name);
                }
            }

            return controllerNames;
        }

        public IEnumerable<string> GetControllerInputs()
        {
            var inputs = new List<string>();

            // 버튼 입력
            foreach (SDL.SDL_GameControllerButton button in Enum.GetValues(typeof(SDL.SDL_GameControllerButton)))
            {
                inputs.Add(button.ToString());
            }

            // 축 입력
            foreach (SDL.SDL_GameControllerAxis axis in Enum.GetValues(typeof(SDL.SDL_GameControllerAxis)))
            {
                inputs.Add(axis.ToString());
            }

            return inputs;
        }



        public void SetActiveController(int index)
        {
            if (index >= 0 && index < connectedControllers.Count)
            {
                activeController = connectedControllers[index];
                activeControllerIndex = index;
                activeControllerName = SDL.SDL_GameControllerName(activeController);
                Console.WriteLine($"Active Controller Set: {SDL.SDL_GameControllerName(activeController)}");

            }
            else
            {
                Console.WriteLine("Invalid controller index.");
            }
        }

        public IntPtr GetActiveController()
        {
            return activeController;
        }


        public void CloseDevices()
        {
            foreach (var controller in connectedControllers.Values)
            {
                SDL.SDL_GameControllerClose(controller);
            }
            connectedControllers.Clear();
            Console.WriteLine("All controllers disconnected.");
        }

        ~DeviceManager()
        {
            CloseDevices();
            SDL.SDL_Quit();
        }
    }
}