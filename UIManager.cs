﻿using System.ComponentModel;
using SDL2;

namespace GameControllers2MIDI
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
        private BindingList<Mapping> mappings;


        public UIManager(MappingManager mappingManager, DeviceManager deviceManager, MidiManager midiManager)
        {
            this.mappingManager = mappingManager;
            this.deviceManager = deviceManager;
            this.midiManager = midiManager;

            InitializeComponent();
            mappings = mappingManager.GetAllMappings();
            dataGridView.DataSource = mappings;
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
                mappingBindingSource.RaiseListChangedEvents = false;
                mappingManager.LoadMappingFromJson(openFileDialog.FileName);
                mappingBindingSource.RaiseListChangedEvents = true;
                //LoadMappingsIntoGrid();
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

        private void AddNewMapping(object sender, EventArgs e)
        {
            var newMapping = new Mapping();
            mappingManager.AddMapping(newMapping);
        }

        private void DeleteSelectedMapping(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.Index < 0 || row.Index >= mappingManager.GetAllMappings().Count) continue;

                mappingManager.RemoveMapping(row.Index);
            }



        }


        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
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



        private void ApplyInputToMapping(dynamic input, int rowIndex)
        {
            //var row = dataGridView.Rows[rowIndex];
            var mapping = mappingManager.GetAllMappings()[rowIndex];
            mapping.Input = input;

            // 추가로 업데이트가 필요하다면 호출
            //LoadMappingsIntoGrid();
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



        private void MappingBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }


        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var mapping = mappings[e.RowIndex];
                DataGridViewComboBoxCell comboBoxCell;
                DataGridViewCheckBoxCell checkBoxCell;

                foreach (DataGridViewCell c in dataGridView.Rows[e.RowIndex].Cells)
                {
                    switch (c.OwningColumn.Name)
                    {
                        case "keyDataGridViewTextBoxColumn":
                        case "octDataGridViewTextBoxColumn":
                            if (mapping.InputType == InputType.Axis)
                            {
                                c.ReadOnly = true;
                                comboBoxCell = (DataGridViewComboBoxCell)c;
                                comboBoxCell.FlatStyle = FlatStyle.Flat;
                            }
                            break;
                        case "velocityDataGridViewTextBoxColumn":
                            if (mapping.InputType == InputType.Axis)
                            {
                                c.ReadOnly = true;
                                c.Style.BackColor = Color.LightGray;
                                c.Style.ForeColor = Color.DarkGray;
                            }
                            break;
                        case "isUsingAbsDataGridViewCheckBoxColumn":
                            if (mapping.InputType == InputType.Button)
                            {
                                c.ReadOnly = true;
                                checkBoxCell = (DataGridViewCheckBoxCell)c;
                                checkBoxCell.Style.BackColor = Color.LightGray;
                                checkBoxCell.Style.ForeColor = Color.DarkGray;
                                checkBoxCell.FlatStyle = FlatStyle.Flat;
                            }
                            break;
                        default:
                            c.ReadOnly = false;
                            c.Style.BackColor = Color.White;
                            c.Style.ForeColor = Color.Black;
                            switch (c.OwningColumn.Name)
                            {
                                case "keyDataGridViewTextBoxColumn":
                                case "octDataGridViewTextBoxColumn":
                                    comboBoxCell = (DataGridViewComboBoxCell)c;
                                    comboBoxCell.FlatStyle = FlatStyle.Standard;
                                    break;
                                case "isUsingAbsDataGridViewCheckBoxColumn":
                                    checkBoxCell = (DataGridViewCheckBoxCell)c;
                                    checkBoxCell.FlatStyle = FlatStyle.Standard;
                                    break;
                            }
                            break;
                    }
                }
            }

        }

        private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (dataGridView.CurrentCell.OwningColumn.Name == "mapDataGridViewTextBoxColumn" && e.Control is ComboBox comboBox)
            {
                int previousIndex = comboBox.SelectedIndex;
                // ComboBox 선택지 초기화
                comboBox.Items.Clear();

                // 현재 행의 Mapping 객체 가져오기
                var mapping = mappings[dataGridView.CurrentCell.RowIndex];

                // InputType에 따라 ComboBox 선택지 설정
                if (mapping.InputType == InputType.Button)
                {
                    comboBox.Items.AddRange(new object[] { "Note" });
                    comboBox.SelectedIndex = 0;
                }
                else if (mapping.InputType == InputType.Axis)
                {
                    comboBox.Items.AddRange(new object[] { "CC", "Pitchbend" });
                    comboBox.SelectedIndex = previousIndex - 1;
                }
            }

        }

        private void DataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.IsCurrentCellInEditMode)
            {
                dataGridView.EndEdit(); // 편집 종료
            }

        }

        private void DataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dataGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && !dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly)
                {
                    dataGridView.BeginEdit(true);
                    ((ComboBox)dataGridView.EditingControl).DroppedDown = true;
                }
            }
        }
    }
}
