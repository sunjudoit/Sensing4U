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
        private readonly DataProcessor dataProcessor;
        private List<SensorData> currentDataset;
        private readonly List<string> loadedFileNames;

        // Constructor: Executed when the form is created.
        public SensorWindow()
        {
            InitializeComponent();
            fileManager = new FileManager();
            dataProcessor = DataProcessor.Instance;
            currentDataset = null;
            loadedFileNames = new List<string>();
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
                fileDialog.Filter = "Binary files|*.bin";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {

                    //Check for duplicate files
                    string selectedFile = fileDialog.FileName;
                    if(loadedFileNames.Contains(selectedFile))
                    {
                        ShowError("This file is already loaded.");
                        return; 
                    }

                    try 
                    {
                        // Pass the file path and read the file using the LoadFile method of FileManager to get the SensorData list.
                        var loadedData = fileManager.LoadFile(fileDialog.FileName);

                        dataProcessor.AddLoadedDataset(loadedData);
                        currentDataset = dataProcessor.GetCurrent();
                        ShowGrid(currentDataset);

                        // Clear user input and result displays after showing new dataset
                        ClearPreviousUI();


                        loadedFileNames.Add(fileDialog.FileName); // for checking duplicate
                        listLoadedFiles.Items.Add(System.IO.Path.GetFileName(fileDialog.FileName)); // for the listLoadedFiles(UI)


                        ShowInfo("File loaded successfully.");
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
            currentDataset = dataProcessor.GetCurrent();

            // Stop the save operation if the current dataset is null or empty.
            if (currentDataset == null || currentDataset.Count == 0)
            {
                ShowError("No data available to save.");
                return;
            }

            // Use SaveFileDialog to allow the user to choose a save location.
            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "Binary files|*.bin";

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
        /// Displays the given dataset in the DataGridView as Label / Value columns.
        /// </summary>
        /// <param name="dataset">The list of SensorData objects to display.</param>
        private void ShowGrid(List<SensorData> dataset)
        {
            if (dataset == null)
            {
                // If the dataset is null unbind and exit.
                dataGridView.DataSource = null;
                return;
            }
            // Clear the existing data source reference.
            dataGridView.DataSource = null;
            // Automatically generate columns based on object properties (labels, values).
            dataGridView.AutoGenerateColumns = true;
            // Bind the new list to the DataGridView for display.
            dataGridView.DataSource = dataset;

        }
        /// <summary>
        /// Event handler for the Previous button Click
        /// Click the Previous button to view previously loaded files.
        /// </summary>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // Request the previous dataset from the DataProcessor.
            var prevDataset = dataProcessor.Prev();
            if (prevDataset != null)
            {
                currentDataset = prevDataset;
                ShowGrid(currentDataset);

                // Clear user input and result displays after showing new dataset
                ClearPreviousUI();
            }
            //Select the dataset currently displayed in the loaded file list
            //if (listLoadedFiles.Items.Count > 0 && listLoadedFiles.SelectedIndex > 0) 
            //{
            //    listLoadedFiles.SelectedIndex--;
            //}


        }
        /// <summary>
        /// Event handler for the  Next button Click
        /// Click the next button to view next loaded files.
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            
            // Request the next dataset from the DataProcessor.
            var nextDataset = dataProcessor.Next();

            if (nextDataset != null)
            {
                currentDataset = nextDataset;
                ShowGrid(currentDataset);

                // Clear user input and result displays after showing new dataset
                ClearPreviousUI();
            }
            //Select the dataset currently displayed in the loaded file list
            //if (listLoadedFiles.Items.Count > 0 && listLoadedFiles.SelectedIndex < listLoadedFiles.Items.Count - 1)
            //{
             //   listLoadedFiles.SelectedIndex++;
            //}


        }


        /// <summary>
        /// Event handler for the Data Indicator button click.
        /// Reads user-defined upper and lower bounds, validates input values,
        /// receives a 2D array of colors,
        /// and applies those colors to the background of each DataGridView row.
        /// </summary>
        private void btnColorize_Click(object sender, EventArgs e)
        {
            currentDataset = dataProcessor.GetCurrent();
            if (currentDataset == null || currentDataset.Count == 0)
            {
                ShowError("No dataset loaded.");
                return;
            }

            // Validate user input values          
            if (!double.TryParse(txtLowerBound.Text, out double lower)) // If conversion of the input in the txtLowerBound text box to a double fails.
            {
                ShowError("Please enter a valid lower value.");
                return;
            }
            if (!double.TryParse(txtUpperBound.Text, out double upper))
            {
                ShowError("Please enter a valid upper value.");
                return;
            }

            if (lower > upper)
            {
                ShowError("The lower bound must be less than the upper bound.");
                return;
            }
            // Pass the bounds to DataProcessor to calculate the colors and get a 2D color map.
            var color = dataProcessor.ApplyColor(lower, upper);

            // Apply background colors to the DataGridView
            for (int i = 0; i < currentDataset.Count; i++)
            {
                // Set the background color .
                dataGridView.Rows[i].DefaultCellStyle.BackColor = color[i,0];
            }

            ShowInfo("User bound applied as color successfully.");
        }

        /// <summary>
        /// Event handler for the Search button click.
        /// Validates user input, sorts the current dataset, performs a binary search for the target value,
        /// and highlights the search results in the DataGridView.
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtSearchValue.Text, out double target)) 
            {
                ShowError("Please enter a valid value to search.");
                return;
            }
            dataProcessor.SortData(); //sort before searching
            

            int targetPoint = dataProcessor.BinarySearch(target);

            if (targetPoint != -1)
            {
                dataGridView.ClearSelection();
                dataGridView.Rows[targetPoint].Cells[1].Selected = true; // Highlight cells with searched values
                dataGridView.FirstDisplayedScrollingRowIndex = targetPoint; 

                ShowInfo("Search successful.");
            }
            else 
            {
                ShowInfo("No matching value was found.");
            }
                
        }


        /// <summary>
        /// Event handler for the Average button click.
        /// It calls the DataProcessor to calculate the average of the current dataset.
        /// displays the result, formatted to three decimal places, on the UI label.
        /// </summary>
        private void btnAverage_Click(object sender, EventArgs e)
        {
            //ClearPreviousUI();
            double average = dataProcessor.AverageData();

            if (double.IsNaN(average))
            {
                ShowError("No dataset loaded.");
                lblAverageResult.Text = "";
                return;
            }

            lblAverageResult.Text = $"{average:F3}"; // displays the result 
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

        /// <summary>
        /// Event handler that triggers the UI cleanup when the user clicks or tabs into the search value textbox.
        /// </summary>
        private void txtSearchValue_Enter(object sender, EventArgs e)
        {
           ClearPreviousUI();
        }
        /// <summary>
        /// Event handler that triggers the UI cleanup when the user clicks or tabs into the lower bound textbox.
        /// </summary>
        private void txtLowerBound_Enter(object sender, EventArgs e)
        {
            ClearPreviousUI();
        }

        /// <summary>
        /// Resets the DataGridView's color state and clears all displayed results from the UI controls.
        /// </summary>
        private void ClearPreviousUI()
        {
            // Grid color initialization
            dataGridView.ClearSelection();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.DefaultCellStyle.BackColor = SystemColors.Window; 
            }

            // Clear all related input fields and label
            lblAverageResult.Text = "";
            txtSearchValue.Clear();
            txtLowerBound.Clear();
            txtUpperBound.Clear();
        }

       
    }
}
