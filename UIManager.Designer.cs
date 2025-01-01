namespace GameControllers2MIDI
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
            components = new System.ComponentModel.Container();
            controllerDropdown = new ComboBox();
            midiDeviceDropdown = new ComboBox();
            labelControllerComboBox = new Label();
            labelComboBoxMIDIDevice = new Label();
            toggleProcessingButton = new Button();
            loadMappingButton = new Button();
            saveMappingButton = new Button();
            dataGridView = new DataGridView();
            inputDataGridViewTextBoxColumn = new DataGridViewButtonColumn();
            mapDataGridViewTextBoxColumn = new DataGridViewComboBoxColumn();
            valueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            isInvertedDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            keyDataGridViewTextBoxColumn = new DataGridViewComboBoxColumn();
            octDataGridViewTextBoxColumn = new DataGridViewComboBoxColumn();
            velocityDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            isUsingAbsDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            mappingBindingSource = new BindingSource(components);
            deleteButton = new Button();
            addButton = new Button();
            midiChannelDropdown = new ComboBox();
            midiChanelDropdownLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mappingBindingSource).BeginInit();
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
            toggleProcessingButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            toggleProcessingButton.Location = new Point(1002, 520);
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
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { inputDataGridViewTextBoxColumn, mapDataGridViewTextBoxColumn, valueDataGridViewTextBoxColumn, isInvertedDataGridViewCheckBoxColumn, keyDataGridViewTextBoxColumn, octDataGridViewTextBoxColumn, velocityDataGridViewTextBoxColumn, isUsingAbsDataGridViewCheckBoxColumn });
            dataGridView.DataSource = mappingBindingSource;
            dataGridView.Location = new Point(338, 65);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(854, 449);
            dataGridView.TabIndex = 7;
            dataGridView.CellContentClick += dataGridView_CellContentClick;
            dataGridView.RowPostPaint += DataGridView_RowPostPaint;
            // 
            // inputDataGridViewTextBoxColumn
            // 
            inputDataGridViewTextBoxColumn.DataPropertyName = "InputName";
            inputDataGridViewTextBoxColumn.HeaderText = "Input";
            inputDataGridViewTextBoxColumn.Name = "inputDataGridViewTextBoxColumn";
            inputDataGridViewTextBoxColumn.ReadOnly = true;
            inputDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            inputDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // mapDataGridViewTextBoxColumn
            // 
            mapDataGridViewTextBoxColumn.DataPropertyName = "MapName";
            mapDataGridViewTextBoxColumn.HeaderText = "Map";
            mapDataGridViewTextBoxColumn.Items.AddRange(new object[] { "Note", "CC", "Pitchbend" });
            mapDataGridViewTextBoxColumn.Name = "mapDataGridViewTextBoxColumn";
            mapDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            mapDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // valueDataGridViewTextBoxColumn
            // 
            valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            valueDataGridViewTextBoxColumn.HeaderText = "Value";
            valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            // 
            // isInvertedDataGridViewCheckBoxColumn
            // 
            isInvertedDataGridViewCheckBoxColumn.DataPropertyName = "IsInverted";
            isInvertedDataGridViewCheckBoxColumn.HeaderText = "IsInverted";
            isInvertedDataGridViewCheckBoxColumn.Name = "isInvertedDataGridViewCheckBoxColumn";
            // 
            // keyDataGridViewTextBoxColumn
            // 
            keyDataGridViewTextBoxColumn.DataPropertyName = "KeyName";
            keyDataGridViewTextBoxColumn.HeaderText = "Key";
            keyDataGridViewTextBoxColumn.Items.AddRange(new object[] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" });
            keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            keyDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            keyDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // octDataGridViewTextBoxColumn
            // 
            octDataGridViewTextBoxColumn.DataPropertyName = "OctName";
            octDataGridViewTextBoxColumn.HeaderText = "Oct";
            octDataGridViewTextBoxColumn.Items.AddRange(new object[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            octDataGridViewTextBoxColumn.Name = "octDataGridViewTextBoxColumn";
            octDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            octDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // velocityDataGridViewTextBoxColumn
            // 
            velocityDataGridViewTextBoxColumn.DataPropertyName = "Velocity";
            velocityDataGridViewTextBoxColumn.HeaderText = "Velocity";
            velocityDataGridViewTextBoxColumn.Name = "velocityDataGridViewTextBoxColumn";
            // 
            // isUsingAbsDataGridViewCheckBoxColumn
            // 
            isUsingAbsDataGridViewCheckBoxColumn.DataPropertyName = "IsUsingAbs";
            isUsingAbsDataGridViewCheckBoxColumn.HeaderText = "IsUsingAbs";
            isUsingAbsDataGridViewCheckBoxColumn.Name = "isUsingAbsDataGridViewCheckBoxColumn";
            // 
            // mappingBindingSource
            // 
            mappingBindingSource.DataSource = typeof(Mapping);
            mappingBindingSource.CurrentChanged += mappingBindingSource_CurrentChanged;
            // 
            // deleteButton
            // 
            deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            deleteButton.Location = new Point(369, 520);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(25, 25);
            deleteButton.TabIndex = 8;
            deleteButton.Text = "-";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += DeleteSelectedMapping;
            // 
            // addButton
            // 
            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addButton.Location = new Point(338, 520);
            addButton.Name = "addButton";
            addButton.Size = new Size(25, 25);
            addButton.TabIndex = 9;
            addButton.Text = "+";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddNewMapping;
            // 
            // midiChannelDropdown
            // 
            midiChannelDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            midiChannelDropdown.FormattingEnabled = true;
            midiChannelDropdown.Location = new Point(175, 133);
            midiChannelDropdown.Name = "midiChannelDropdown";
            midiChannelDropdown.Size = new Size(73, 23);
            midiChannelDropdown.TabIndex = 10;
            midiChannelDropdown.SelectedIndexChanged += MidiChannelDropdown_SelectedIndexChanged;
            // 
            // midiChanelDropdownLabel
            // 
            midiChanelDropdownLabel.AutoSize = true;
            midiChanelDropdownLabel.Location = new Point(175, 115);
            midiChanelDropdownLabel.Name = "midiChanelDropdownLabel";
            midiChanelDropdownLabel.Size = new Size(81, 15);
            midiChanelDropdownLabel.TabIndex = 11;
            midiChanelDropdownLabel.Text = "MIDI Channel";
            // 
            // UIManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1204, 581);
            Controls.Add(midiChanelDropdownLabel);
            Controls.Add(midiChannelDropdown);
            Controls.Add(addButton);
            Controls.Add(deleteButton);
            Controls.Add(dataGridView);
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
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)mappingBindingSource).EndInit();
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
        private DataGridView dataGridView;
        private Button loadMappingButton;
        private Button deleteButton;
        private Button addButton;
        private ComboBox midiChannelDropdown;
        private Label midiChanelDropdownLabel;
        private BindingSource mappingBindingSource;
        private DataGridViewButtonColumn inputDataGridViewTextBoxColumn;
        private DataGridViewComboBoxColumn mapDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn isInvertedDataGridViewCheckBoxColumn;
        private DataGridViewComboBoxColumn keyDataGridViewTextBoxColumn;
        private DataGridViewComboBoxColumn octDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn velocityDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn isUsingAbsDataGridViewCheckBoxColumn;
    }
}