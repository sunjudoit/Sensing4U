using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Sensing4UApp
{
    public partial class SensorWindow : Form
    {
        private readonly FileManager fileManager;
        // The dataset currently displayed in the DataGridView.
        private List<SensorData> currentDataset;

        // Constructor: Executed when the form is created.
        public SensorWindow()
        {
            InitializeComponent();
            fileManager = new FileManager();
            currentDataset = null;
        }
        /// <summary>
        /// This is the event handler for the Open File button click.
        /// After the user selects a .bin file and loads it using the FileManager,
        /// this displays the results in a DataGridView.
        /// </summary>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Use OpenFileDialog to allow file selection.
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                // Set the file filter (bin file only)
                fileDialog.Filter = "Binary files (*.bin)|*.bin";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try 
                    {
                        // Pass the file path and read the file using the LoadFile method of FileManager to get the SensorData list.
                        currentDataset = fileManager.LoadFile(fileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        // Handles all exceptions
                        ShowError(ex.Message);
                    }

                }

            }
        }

        /// <summary>
        /// Event handler for the Save File button click.
        /// Use FileManager to save the currently loaded dataset to a new .bin file.
        /// </summary>
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            // Stop the save operation if the current dataset is null or empty.
            if (currentDataset == null || currentDataset.Count == 0)
            {
                ShowError("No data available to save.");
                return;
            }

            // Use SaveFileDialog to allow the user to choose a save location.
            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "Binary files (*.bin)|*.bin";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Call the FileManager's SaveFile method and receive the success status.
                        bool success = fileManager.SaveFile(fileDialog.FileName, currentDataset);
                        if (success == true)
                        {
                            ShowInfo("File saved successfully.");
                        }
                        else
                        {
                            ShowError("Failed to save file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Shows an success information message to the user.
        /// </summary>
        /// <param name="message">The message string to display.</param>
        private void ShowInfo(string message)
        {
            
            MessageBox.Show(message,"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">The message string to display.</param>
        private void ShowError(string message)
        {
            MessageBox.Show(message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }
}
