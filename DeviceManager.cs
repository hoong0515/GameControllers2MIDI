using SDL2;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Controllers2MIDI

{public class DeviceManager
    {
        private Dictionary<int, IntPtr> connectedControllers = new Dictionary<int, IntPtr>();
        private IntPtr activeController = IntPtr.Zero;
        private bool isProcessing = false;
        

        public event Action<SDL.SDL_GameControllerButton> ButtonPressed;
        public event Action<SDL.SDL_GameControllerButton> ButtonReleased;
        public event Action<SDL.SDL_GameControllerAxis, short> AxisMoved;

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
                    //Thread.Sleep(50); // CPU 사용량 조절
                }
            });
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

        public void SetActiveController(IntPtr controller)
        {
            activeController = controller;
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