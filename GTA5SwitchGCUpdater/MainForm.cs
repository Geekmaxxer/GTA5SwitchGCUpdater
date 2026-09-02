using System;
using System.Drawing;
using System.Windows.Forms;

namespace GTAGameconfigUpdater
{
    public partial class MainForm : Form
    {
        private string? selectedGameconfigPath;
        private string? selectedUpdateRpfPath;

        private TextBox gameConfigTextBox;
        private TextBox rpfTextBox;
        private readonly ToolTip pathToolTip = new();

        public MainForm()
        {
            gameConfigTextBox = null!;
            rpfTextBox = null!;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "GTA Gameconfig Updater";
            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(480, 264);

            var mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            var descriptionLabel = new Label
            {
                Text = "Update gameconfig.xml inside a GTA V Switch update.rpf file.",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 16)
            };

            var (gameConfigGroup, gameConfigBox) = CreateFileSelectionGroup(
                "1. Select gameconfig.xml",
                (s, e) => SelectGameconfig());
            gameConfigTextBox = gameConfigBox;

            var (rpfGroup, rpfBox) = CreateFileSelectionGroup(
                "2. Select update.rpf file",
                (s, e) => SelectUpdateRpf());
            rpfTextBox = rpfBox;

            var updateButton = new Button
            {
                Text = "3. Update RPF",
                Size = new Size(448, 32),
                Margin = new Padding(0),
                UseVisualStyleBackColor = true
            };
            updateButton.Click += UpdateRpf_Click;

            AcceptButton = updateButton;

            mainPanel.Controls.Add(descriptionLabel);
            mainPanel.Controls.Add(gameConfigGroup);
            mainPanel.Controls.Add(rpfGroup);
            mainPanel.Controls.Add(updateButton);

            Controls.Add(mainPanel);
        }

        private (GroupBox Group, TextBox PathTextBox) CreateFileSelectionGroup(string title, EventHandler onBrowse)
        {
            var group = new GroupBox
            {
                Text = title,
                Size = new Size(448, 62),
                Margin = new Padding(0, 0, 0, 16)
            };

            var pathTextBox = new TextBox
            {
                Location = new Point(12, 24),
                Size = new Size(316, 23),
                ReadOnly = true,
                TabStop = false,
                Text = "No file selected"
            };

            var browseButton = new Button
            {
                Text = "Browse...",
                Location = new Point(340, 23),
                Size = new Size(96, 25),
                UseVisualStyleBackColor = true
            };
            browseButton.Click += onBrowse;

            group.Controls.Add(pathTextBox);
            group.Controls.Add(browseButton);

            return (group, pathTextBox);
        }

        private void SelectGameconfig()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select gameconfig.xml",
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                DefaultExt = ".xml"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                selectedGameconfigPath = ofd.FileName;
                gameConfigTextBox.Text = ofd.FileName;
                pathToolTip.SetToolTip(gameConfigTextBox, ofd.FileName);
            }
        }

        private void SelectUpdateRpf()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select update.rpf",
                Filter = "RPF Files (*.rpf)|*.rpf|All Files (*.*)|*.*",
                DefaultExt = ".rpf"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                selectedUpdateRpfPath = ofd.FileName;
                rpfTextBox.Text = ofd.FileName;
                pathToolTip.SetToolTip(rpfTextBox, ofd.FileName);
            }
        }

        private void UpdateRpf_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedGameconfigPath))
            {
                MessageBox.Show(this, "Please select a gameconfig.xml file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(selectedUpdateRpfPath))
            {
                MessageBox.Show(this, "Please select an update.rpf file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Save updated update.rpf as",
                Filter = "RPF Files (*.rpf)|*.rpf",
                DefaultExt = ".rpf",
                FileName = "update.rpf"
            };

            if (sfd.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;

                var updater = new GameconfigUpdater();
                updater.UpdateGameconfig(selectedUpdateRpfPath, selectedGameconfigPath, sfd.FileName);

                MessageBox.Show(this, "Successfully updated update.rpf!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
