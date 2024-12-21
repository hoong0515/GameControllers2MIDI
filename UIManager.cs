namespace Controllers2MIDI
{
    public partial class UIManager : Form
    {
        private MappingManager mappingManager;
        private DeviceManager deviceManager;
        private MidiManager midiManager;
        private bool isProcessing = false;
        public UIManager(MappingManager mappingManager, DeviceManager deviceManager, MidiManager midiManager)
        {
            this.mappingManager = mappingManager;
            this.deviceManager = deviceManager;
            this.midiManager = midiManager;
            InitializeComponent();
        }


        private void ToggleProcessingButton_Click(object sender, EventArgs e)
        {
            if (!isProcessing)
            {
                // Start Processing
                var activeController = deviceManager.GetActiveController(); // 활성 컨트롤러 가져오기
                if (activeController == IntPtr.Zero)
                {
                    MessageBox.Show("No active controller selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                midiManager.StartProcessing(deviceManager.GetActiveController(), mappingManager);
                toggleProcessingButton.Text = "Stop Processing";
                isProcessing = true;
            }
            else
            {
                // Stop Processing
                midiManager.StopProcessing();
                toggleProcessingButton.Text = "Start Processing";
                isProcessing = false;
            }

        }


        private void LoadMappingButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Load Mapping Profile"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                mappingManager.LoadMappingFromJson(openFileDialog.FileName);
                MessageBox.Show("Mapping loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void SaveMappingButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Save Mapping Profile"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                mappingManager.SaveMappingToJson(saveFileDialog.FileName);
                MessageBox.Show("Mapping saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }



        private void ControllerDropdown_Selected(object sender, EventArgs e)
        {
            if (controllerDropdown.SelectedItem is KeyValuePair<int, string> selectedController)
            {
                int controllerIndex = selectedController.Key;
                deviceManager.SetActiveController(controllerIndex);
                IntPtr activeController = deviceManager.GetActiveController();

                if (activeController != IntPtr.Zero)
                {
                    Console.WriteLine($"Controller {selectedController.Value} is now active.");
                }
                else
                {
                    Console.WriteLine("Failed to activate the selected controller.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Console.WriteLine("No controller selected!");
            }
        }

        private void MidiDeviceDropdown_Selected(object sender, EventArgs e)
        {
            if (midiDeviceDropdown.SelectedItem is KeyValuePair<int, string> selectedDevice)
            {
                int deviceIndex = selectedDevice.Key;
                string deviceName = selectedDevice.Value;

                midiManager.SetActiveMidiDevice(deviceIndex);
                Console.WriteLine($"Active MIDI Device set to: {deviceName} (Index: {deviceIndex})");
            }
            else
            {
                Console.WriteLine("No MIDI device selected!");
            }
        }


        private void InitializeControllerDropdown()
        {
            controllerDropdown.SelectedIndexChanged += (sender, e) =>
            {
                try
                {
                    if (controllerDropdown.SelectedItem != null)
                    {
                        int selectedIndex = controllerDropdown.SelectedIndex;

                        // 컨트롤러 설정
                        deviceManager.SetActiveController(selectedIndex);
                        Console.WriteLine($"Controller changed to: {controllerDropdown.SelectedItem}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting controller: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void InitializeMIDIDeviceDropdown()
        {
            midiDeviceDropdown.SelectedIndexChanged += (sender, e) =>
            {
                try
                {
                    if (midiDeviceDropdown.SelectedItem != null)
                    {
                        int selectedIndex = midiDeviceDropdown.SelectedIndex;

                        // MIDI 장치 설정
                        midiManager.SetActiveMidiDevice(selectedIndex);
                        Console.WriteLine($"MIDI Device changed to: {midiDeviceDropdown.SelectedItem}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting MIDI device: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }


        private void InitializeDropdowns(object sender, EventArgs e)
        {
            InitializeControllerDropdown();
            InitializeMIDIDeviceDropdown();

            // 컨트롤러 드롭다운 초기화
            controllerDropdown.Items.AddRange(deviceManager.GetControllerNames().ToArray());
            if (controllerDropdown.Items.Count > 0)
            {
                controllerDropdown.SelectedIndex = 0; // 기본 선택
            }

            // MIDI 장치 드롭다운 초기화
            midiDeviceDropdown.Items.AddRange(midiManager.GetMidiDeviceNames().ToArray());
            if (midiDeviceDropdown.Items.Count > 0)
            {
                midiDeviceDropdown.SelectedIndex = 0; // 기본 선택
            }
        }


    }
}
