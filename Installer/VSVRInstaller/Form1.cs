using System;
using System.IO;
using System.IO.Compression; 
using System.Reflection;
using System.Windows.Forms;

namespace VSVRInstaller
{
    public partial class Form1 : Form
    {
        public Form1(String defaultPath)
        {
            InitializeComponent(defaultPath);
        }

        private Label lblInstructions;
        private TextBox txtGamePath;
        private Label lblInstructions2;
        private Button btnBrowse;
        private DataGridView dgvExeInfo;
        private Button btnInstall;
        private Button btnUninstall;
        private ProgressBar progressBar1;

        private void InitializeComponent(string defaultPath)
        {
            this.lblInstructions = new Label();
            this.txtGamePath = new TextBox { Text = defaultPath };
            this.btnBrowse = new Button();
            this.btnInstall = new Button();

            this.SuspendLayout();

            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(12, 9);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(223, 15);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "Select the location of the game’s exe:";

            // 
            // txtGamePath
            // 
            this.txtGamePath.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.txtGamePath.Location = new System.Drawing.Point(12, 27);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(400, 23);
            this.txtGamePath.TabIndex = 1;

            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(418, 27);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 3;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(505, 100);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.lblInstructions);
            this.Name = "Form1";
            this.Text = "VSVRMod2 Installer";

            this.btnInstall.Location = new System.Drawing.Point(
                (this.ClientSize.Width - this.btnInstall.Width) / 2,  // Center horizontally
                65 // Vertical position (adjust as needed)
            );
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select the Game Executable";
                ofd.Filter = "Virtual Succubus|Virtual Succubus.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtGamePath.Text = ofd.FileName;
                }
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            string exePath = txtGamePath.Text;
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                MessageBox.Show("Please select a valid game executable before installing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ExtractModFiles(Path.GetDirectoryName(exePath));
                MessageBox.Show("Installation completed successfully!", "Install", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while attempting to install the mod: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExtractModFiles(string extractPath)
        {
            Directory.CreateDirectory(extractPath);

            Assembly assembly = Assembly.GetExecutingAssembly();

            string resourceName = "VSVRInstaller.VSVRMOD.zip";

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    throw new Exception("Embedded resource not found.");

                string zipPath = Path.Combine(extractPath, "VSVRMOD.zip");

                using (FileStream fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }

                string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempFolder);

                try
                {
                    ZipFile.ExtractToDirectory(zipPath, tempFolder);
                    CopyAndOverride(tempFolder, extractPath);
                }
                finally
                {
                    File.Delete(zipPath);
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        static void CopyAndOverride(string sourceFolder, string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);

            foreach (string file in Directory.GetFiles(sourceFolder))
            {
                string targetFilePath = Path.Combine(targetFolder, Path.GetFileName(file));
                File.Copy(file, targetFilePath, overwrite: true);
            }

            foreach (string directory in Directory.GetDirectories(sourceFolder))
            {
                string targetSubFolder = Path.Combine(targetFolder, Path.GetFileName(directory));
                CopyAndOverride(directory, targetSubFolder);
            }
        }
    }
}
