using SDL2;

namespace Controllers2MIDI
{
    public partial class UIManager : Form
    {

        private const short Threshold = 8000;



        private MappingManager mappingManager;
        private DeviceManager deviceManager;
        private MidiManager midiManager;
        private bool isProcessing = false;
        private bool isInputCaptureActive = false;
        private int currentRowIndex;


        public UIManager(MappingManager mappingManager, DeviceManager deviceManager, MidiManager midiManager)
        {
            this.mappingManager = mappingManager;
            this.deviceManager = deviceManager;
            this.midiManager = midiManager;
            InitializeComponent();
            InitializeGrid();
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

            dataGridView.Enabled = !isProcessing;
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
                LoadMappingsIntoGrid();
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

            // MIDI 채널 드롭다운 초기화
            midiChannelDropdown.Items.AddRange(Enumerable.Range(1, 16).Select(i => i.ToString()).ToArray());
            midiChannelDropdown.SelectedIndex = midiManager.GetMidiChannel() - 1; // 기본 선택
        }

        private void LoadMappingsIntoGrid()
        {
            if (dataGridView.InvokeRequired)
            {
                dataGridView.Invoke(new Action(LoadMappingsIntoGrid));
                return;
            }

            dataGridView.Rows.Clear();
            foreach (var mapping in mappingManager.GetAllMappings())
            {
                var dictionary = mapping.ToDictionary(getEnumName: true);

                // DataGridView에 데이터 추가
                dataGridView.Rows.Add(
                    dictionary["input"].Substring(15),
                    dictionary["map"].ToString(),
                    dictionary["value"],
                    dictionary["isInverted"],
                    dictionary["key"].ToString(),
                    dictionary["oct"].ToString(),
                    dictionary["velocity"]
                );
            }
        }



        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex < 0 || rowIndex >= mappingManager.GetAllMappings().Count) return;

            var row = dataGridView.Rows[rowIndex];
            var mapping = mappingManager.GetAllMappings()[rowIndex];

            string input = "SDL_CONTROLLER_" + row.Cells["Input"].Value.ToString();

            if (mapping.InputType == InputType.Axis)
            {
                if (row.Cells["Map"].Value.ToString() != "Note")
                {
                    mapping.Map = Enum.Parse<Map>(row.Cells["Map"].Value.ToString());
                }
            }


            mapping.ModifyNoteProperty(int.Parse(row.Cells["Value"].Value.ToString()), Enum.Parse<Key>(row.Cells["Key"].Value.ToString()), int.Parse(row.Cells["Octave"].Value.ToString()));


            
            mapping.IsInverted = (bool)row.Cells["isInverted"].Value;
            mapping.Velocity = int.Parse(row.Cells["Velocity"].Value.ToString());



            // 데이터 재로드
            LoadMappingsIntoGrid();
        }

        private void AddNewMapping(object sender, EventArgs e)
        {
            var newMapping = new Mapping();
            mappingManager.AddMapping(newMapping);
            LoadMappingsIntoGrid(); // 데이터 재로드
        }

        private void DeleteSelectedMapping(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.Index < 0 || row.Index >= mappingManager.GetAllMappings().Count) continue;

                var mapping = mappingManager.GetAllMappings()[row.Index];
                mappingManager.RemoveMapping(mapping);
            }
            LoadMappingsIntoGrid(); // 데이터 재로드
        }

        private void InitializeGrid()
        {
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            LoadMappingsIntoGrid(); // 매핑 데이터 로드
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView.Columns["Input"].Index && e.RowIndex >= 0)
            {
                if (isProcessing)
                {
                    MessageBox.Show("Cannot change input while processing is active!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int rowIndex = e.RowIndex;
                ShowInputCaptureDialog(rowIndex);
            }
        }
        private void ShowInputCaptureDialog(int rowIndex)
        {
            if (isInputCaptureActive) return;

            isInputCaptureActive = true;
            currentRowIndex = rowIndex;

            var dialog = new Form
            {
                Text = "Input Capture",
                Size = new Size(300, 150),
                StartPosition = FormStartPosition.CenterParent
            };

            // 메시지 라벨
            var messageLabel = new Label
            {
                Text = "Press a button or move an axis on your controller.",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 취소 버튼
            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Dock = DockStyle.Bottom
            };

            dialog.Controls.Add(messageLabel);
            dialog.Controls.Add(cancelButton);

            // 입력 이벤트 핸들러
            void OnButtonPressed(SDL.SDL_GameControllerButton button)
            {
                if (!isInputCaptureActive) return;

                Invoke(new Action(() =>
                {
                    Console.WriteLine($"Input captured: Button {button}");
                    ApplyInputToMapping(button, currentRowIndex);
                    dialog.Close(); // 입력 후 창 닫기
                }));
            }

            void OnAxisMoved(SDL.SDL_GameControllerAxis axis, short value)
            {
                if (!isInputCaptureActive) return;

                if (value == short.MinValue || Math.Abs(value) > Threshold)
                {
                    Invoke(new Action(() =>
                    {
                        Console.WriteLine($"Input captured: Axis {axis} with value {value}");
                        ApplyInputToMapping(axis, currentRowIndex);
                        dialog.Close(); // 입력 후 창 닫기
                    }));
                }
            }

            // DeviceManager 이벤트 연결
            deviceManager.ButtonPressed += OnButtonPressed;
            deviceManager.AxisMoved += OnAxisMoved;

            dialog.FormClosed += (s, e) =>
            {
                // 창 닫힘 처리
                isInputCaptureActive = false;
                deviceManager.ButtonPressed -= OnButtonPressed;
                deviceManager.AxisMoved -= OnAxisMoved;
            };

            dialog.ShowDialog(this);
        }



        private void ApplyInputToMapping(SDL.SDL_GameControllerButton button, int rowIndex)
        {
            var row = dataGridView.Rows[rowIndex];
            var mapping = mappingManager.GetAllMappings()[rowIndex];

            mappingManager.ModifyMapping(mapping, button);
            row.Cells["Input"].Value = button.ToString().Substring(15);

            // 추가로 업데이트가 필요하다면 호출
            LoadMappingsIntoGrid();
        }

        private void ApplyInputToMapping(SDL.SDL_GameControllerAxis axis, int rowIndex)
        {
            var row = dataGridView.Rows[rowIndex];
            var mapping = mappingManager.GetAllMappings()[rowIndex];

            mappingManager.ModifyMapping(mapping, axis);
            
            row.Cells["Input"].Value = axis.ToString().Substring(15);
            LoadMappingsIntoGrid();
        }

        public void UpdateControllerDropdown(List<string> controllers)
        {
            if (controllerDropdown.InvokeRequired)
            {
                controllerDropdown.Invoke(new Action(() => UpdateControllerDropdown(controllers)));
            }
            else
            {
                controllerDropdown.Items.Clear();
                controllerDropdown.Items.AddRange(controllers.ToArray());
            }
        }

        public void ShowControllerDisconnectedWarning(string controllerName)
        {
            // 경고 창 표시
            MessageBox.Show($"Controller '{controllerName}' has been disconnected.",
                            "Controller Disconnected",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }


        public void UpdateMidiDropdown(List<string> midiDevices)
        {
            if (midiDeviceDropdown.InvokeRequired)
            {
                midiDeviceDropdown.Invoke(new Action(() => UpdateMidiDropdown(midiDevices)));
            }
            else
            {
                midiDeviceDropdown.Items.Clear();
                midiDeviceDropdown.Items.AddRange(midiDevices.ToArray());
            }
        }


        private void MidiChannelDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(midiChannelDropdown.SelectedItem.ToString(), out int selectedChannel))
            {
                midiManager.SetMidiChannel(selectedChannel);
                Console.WriteLine($"MIDI channel set to {selectedChannel}");
            }
        }


        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox comboBox)
            {
                // 기존 이벤트 핸들러 제거 (중복 방지)
                comboBox.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;

                // 새 이벤트 핸들러 추가
                comboBox.SelectionChangeCommitted += ComboBox_SelectionChangeCommitted;
            }
        }

        private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellInEditMode)
            {
                dataGridView.EndEdit(); // 편집 종료
            }
        }



    }
}
