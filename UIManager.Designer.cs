namespace Controllers2MIDI
{
    partial class UIManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            controllerDropdown = new ComboBox();
            midiDeviceDropdown = new ComboBox();
            labelControllerComboBox = new Label();
            labelComboBoxMIDIDevice = new Label();
            toggleProcessingButton = new Button();
            loadMappingButton = new Button();
            saveMappingButton = new Button();
            dataGridView1 = new DataGridView();
            Input = new DataGridViewTextBoxColumn();
            Map = new DataGridViewComboBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            isInverted = new DataGridViewCheckBoxColumn();
            Key = new DataGridViewComboBoxColumn();
            Octave = new DataGridViewComboBoxColumn();
            Velocity = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // controllerDropdown
            // 
            controllerDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            controllerDropdown.FormattingEnabled = true;
            controllerDropdown.Location = new Point(12, 65);
            controllerDropdown.Name = "controllerDropdown";
            controllerDropdown.Size = new Size(157, 23);
            controllerDropdown.TabIndex = 0;
            // 
            // midiDeviceDropdown
            // 
            midiDeviceDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            midiDeviceDropdown.FormattingEnabled = true;
            midiDeviceDropdown.Location = new Point(175, 65);
            midiDeviceDropdown.Name = "midiDeviceDropdown";
            midiDeviceDropdown.Size = new Size(157, 23);
            midiDeviceDropdown.TabIndex = 1;
            // 
            // labelControllerComboBox
            // 
            labelControllerComboBox.AutoSize = true;
            labelControllerComboBox.Location = new Point(12, 47);
            labelControllerComboBox.Name = "labelControllerComboBox";
            labelControllerComboBox.Size = new Size(60, 15);
            labelControllerComboBox.TabIndex = 2;
            labelControllerComboBox.Text = "Controller";
            // 
            // labelComboBoxMIDIDevice
            // 
            labelComboBoxMIDIDevice.AutoSize = true;
            labelComboBoxMIDIDevice.Location = new Point(175, 47);
            labelComboBoxMIDIDevice.Name = "labelComboBoxMIDIDevice";
            labelComboBoxMIDIDevice.Size = new Size(73, 15);
            labelComboBoxMIDIDevice.TabIndex = 3;
            labelComboBoxMIDIDevice.Text = "MIDI Device";
            // 
            // toggleProcessingButton
            // 
            toggleProcessingButton.Location = new Point(598, 389);
            toggleProcessingButton.Name = "toggleProcessingButton";
            toggleProcessingButton.Size = new Size(190, 49);
            toggleProcessingButton.TabIndex = 4;
            toggleProcessingButton.Text = "Start Processing";
            toggleProcessingButton.UseVisualStyleBackColor = true;
            toggleProcessingButton.Click += ToggleProcessingButton_Click;
            // 
            // loadMappingButton
            // 
            loadMappingButton.Location = new Point(12, 133);
            loadMappingButton.Name = "loadMappingButton";
            loadMappingButton.Size = new Size(157, 23);
            loadMappingButton.TabIndex = 5;
            loadMappingButton.Text = "Load Mapping Data (JSON)";
            loadMappingButton.UseVisualStyleBackColor = true;
            loadMappingButton.Click += LoadMappingButton_Click;
            // 
            // saveMappingButton
            // 
            saveMappingButton.Location = new Point(12, 162);
            saveMappingButton.Name = "saveMappingButton";
            saveMappingButton.Size = new Size(157, 23);
            saveMappingButton.TabIndex = 6;
            saveMappingButton.Text = "Save Mapping Data (Data)";
            saveMappingButton.UseVisualStyleBackColor = true;
            saveMappingButton.Click += SaveMappingButton_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Input, Map, Value, isInverted, Key, Octave, Velocity });
            dataGridView1.Location = new Point(338, 65);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(450, 318);
            dataGridView1.TabIndex = 7;
            // 
            // Input
            // 
            Input.HeaderText = "Input";
            Input.Name = "Input";
            Input.ReadOnly = true;
            // 
            // Map
            // 
            Map.HeaderText = "Map";
            Map.Items.AddRange(new object[] { "None", "Note", "CC", "Pitchbend" });
            Map.Name = "Map";
            // 
            // Value
            // 
            Value.HeaderText = "Value";
            Value.Name = "Value";
            // 
            // isInverted
            // 
            isInverted.HeaderText = "isInverted";
            isInverted.Name = "isInverted";
            // 
            // Key
            // 
            Key.HeaderText = "Key";
            Key.Items.AddRange(new object[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" });
            Key.Name = "Key";
            // 
            // Octave
            // 
            Octave.HeaderText = "Octave";
            Octave.Items.AddRange(new object[] { "-2", "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8" });
            Octave.Name = "Octave";
            // 
            // Velocity
            // 
            Velocity.HeaderText = "Velocity";
            Velocity.Name = "Velocity";
            // 
            // UIManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dataGridView1);
            Controls.Add(saveMappingButton);
            Controls.Add(loadMappingButton);
            Controls.Add(toggleProcessingButton);
            Controls.Add(labelComboBoxMIDIDevice);
            Controls.Add(labelControllerComboBox);
            Controls.Add(midiDeviceDropdown);
            Controls.Add(controllerDropdown);
            Name = "UIManager";
            Text = "Controller2MIDI";
            Load += InitializeDropdowns;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox controllerDropdown;
        private ComboBox midiDeviceDropdown;
        private Label labelControllerComboBox;
        private Label labelComboBoxMIDIDevice;
        private Button toggleProcessingButton;
        private Button saveMappingButton;
        private DataGridView dataGridView1;
        private Button loadMappingButton;
        private DataGridViewTextBoxColumn Input;
        private DataGridViewComboBoxColumn Map;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewCheckBoxColumn isInverted;
        private DataGridViewComboBoxColumn Key;
        private DataGridViewComboBoxColumn Octave;
        private DataGridViewTextBoxColumn Velocity;
    }
}