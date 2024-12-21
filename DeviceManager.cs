using SDL2;
using System;
using System.Collections.Generic;

namespace Controllers2MIDI

{public class DeviceManager
    {
        private Dictionary<int, IntPtr> connectedControllers = new Dictionary<int, IntPtr>();
        private IntPtr activeController = IntPtr.Zero;

        public DeviceManager()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER) < 0)
            {
                throw new Exception($"SDL Initialization failed: {SDL.SDL_GetError()}");
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