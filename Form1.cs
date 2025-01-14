using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Text_Filter
{
    public partial class Form1 : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private string selectedFilePath = "";
        private string lastUsedFilePath = "";
        public Form1()
        {
            InitializeComponent();

            textBox1.TextChanged += textBox1_TextChanged;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFilePath = "";
            label2.Text = "";
            panel1.Controls.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRecentFilePath();
            CheckFilterListFileExists();
            // load fiters list
            LoadFiltersToComboBox();
        }

        private void CheckFilterListFileExists()
        {
            string filePath = "filters.json"; // The path to the file

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file does not exist, create it and optionally write some content
                string defaultContent = "{}"; // Default content for the JSON file (empty object)
                File.WriteAllText(filePath, defaultContent);
            }
        }

        private void LoadRecentFilePath()
        {
            // Check if last used file path is stored
            string configFilePath = "config.json";
            string currentDirectory = Directory.GetCurrentDirectory();
            string fullConfigFilePath = Path.Combine(currentDirectory, configFilePath);

            if (File.Exists(configFilePath))
            {
                try
                {
                    // Read the JSON file
                    string jsonContent = File.ReadAllText(configFilePath);

                    // Check if the file is empty
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        //MessageBox.Show("The configuration file is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        openLastFileToolStripMenuItem.Enabled = false;
                        return;
                    }

                    // Parse the JSON content
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        // Try to get the "last_file_path" key

                        if (doc.RootElement.TryGetProperty("last_file_path", out JsonElement lastFilePathElement))
                        {
                            string stored_filepath = lastFilePathElement.GetString().Trim();
                            // if value is non empty

                            //MessageBox.Show(string.IsNullOrEmpty(stored_filepath).ToString());
                            //MessageBox.Show(File.Exists(stored_filepath).ToString());
                            if (!string.IsNullOrEmpty(stored_filepath) && File.Exists(stored_filepath))
                            {
                                lastUsedFilePath = stored_filepath;
                                //MessageBox.Show(selectedFilePath);
                            }
                            else
                            {
                                openLastFileToolStripMenuItem.Enabled = false;
                            }
                        }
                        else
                        {
                            // property not found
                            openLastFileToolStripMenuItem.Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading config.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //MessageBox.Show(Directory.GetCurrentDirectory());
                CreateDefaultConfigFile(fullConfigFilePath);
                // Disable the "openLastFileToolStripMenuItem" menu item
                openLastFileToolStripMenuItem.Enabled = false;
            }
        }

        private void CreateDefaultConfigFile(string configFilePath)
        {
            // Create a default JSON object with the "last_file_path" key
            var defaultConfig = new
            {
                last_file_path = string.Empty // Empty string or default path
            };

            // Serialize the object to JSON
            string jsonContent = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });

            // Write the default JSON to the file
            File.WriteAllText(configFilePath, jsonContent);

            // Optionally, notify the user that the file has been created
            MessageBox.Show($"A default configuration file has been created at: {configFilePath}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set the filter to only show .txt and .org files
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|ORG Files (*.org)|*.org";

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                selectedFilePath = openFileDialog.FileName;
                label2.Text = selectedFilePath;
                //toolStripStatusLabel1.Text = selectedFilePath;
                textBox1.Focus();
                loadFileIntoTableLayoutPanel(selectedFilePath);
            }
        }

        private void loadFileIntoTableLayoutPanel(string filePath)
        {
            lastUsedFilePath = filePath; // Update last opened file path
            UpdateLastOpenedFilePath(filePath);

            // Create a new TableLayoutPanel and set its properties
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.Dock = DockStyle.Fill;  // Set to fill the Panel1
            tableLayoutPanel.ColumnCount = 1;  // Single column
            tableLayoutPanel.RowCount = 0;  // Start with no rows

            // Add TableLayoutPanel to Panel1
            panel1.Controls.Clear();  // Clear any previous controls (if needed)

            // Set AutoScroll to true on Panel1 to allow scrolling
            panel1.AutoScroll = true;  // Enable scrolling

            panel1.Controls.Add(tableLayoutPanel);  // Add the new TableLayoutPanel

            tableLayoutPanel.SuspendLayout();  // Suspend layout updates for performance
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Label label = new Label();
                        label.Text = line;
                        label.AutoSize = true;
                        label.Padding = new Padding(5);

                        tableLayoutPanel.RowCount++;
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        tableLayoutPanel.Controls.Add(label, 0, tableLayoutPanel.RowCount - 1);
                        label.DoubleClick += Label_DoubleClick;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
            finally
            {
                tableLayoutPanel.ResumeLayout();  // Resume layout updates
            }
            //lastUsedFilePath = filePath; // Update last opened file path
            //UpdateLastOpenedFilePath(filePath);

            //// Create a new TableLayoutPanel and set its properties
            //tableLayoutPanel = new TableLayoutPanel();
            //tableLayoutPanel.AutoScroll = true;
            //tableLayoutPanel.Dock = DockStyle.Fill;  // Set to fill the Panel1
            //tableLayoutPanel.ColumnCount = 1;  // Single column
            //tableLayoutPanel.RowCount = 0;  // Start with no rows

            //// Add TableLayoutPanel to Panel1
            //panel1.Controls.Clear();  // Clear any previous controls (if needed)

            //// Set AutoScroll to true on Panel1 to allow scrolling
            //panel1.AutoScroll = true;  // Enable scrolling

            //panel1.Controls.Add(tableLayoutPanel);  // Add the new TableLayoutPanel

            //// Read the file content and add each line as a new item to the TableLayoutPanel
            //string[] lines = File.ReadAllLines(filePath);
            //foreach (string line in lines)
            //{
            //    // Create a new Label for each line in the file
            //    Label label = new Label();
            //    label.Text = line;
            //    label.AutoSize = true;  // Adjust size based on content
            //    label.Padding = new Padding(5);
            //    //label.BorderStyle = BorderStyle.FixedSingle;  // Optional: add border to label for visual feedback

            //    // Add the Label to the TableLayoutPanel (one item per row)
            //    tableLayoutPanel.RowCount++;
            //    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // Allow dynamic row sizing
            //    tableLayoutPanel.Controls.Add(label, 0, tableLayoutPanel.RowCount - 1);  // Add to column 0 and next available row

            //    // Attach the double-click event handler
            //    label.DoubleClick += Label_DoubleClick;
            //}

        }

        private void Label_DoubleClick(object sender, EventArgs e)
        {
            // Get the label that was double-clicked
            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                // Show a popup or input form to edit the label's text
                ShowEditTextDialog(clickedLabel);
            }
        }

        private void ShowEditTextDialog(Label clickedLabel)
        {
            // Create and show the EditTextForm
            EditTextForm editForm = new EditTextForm(clickedLabel.Text);
            DialogResult result = editForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Update the label's text with the new text from the form
                clickedLabel.Text = editForm.UpdatedText;
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(selectedFilePath == "")
            {
                return;
            }
            //MessageBox.Show("hello");
            // Get the entered text from textbox1
            string searchText = textBox1.Text.Trim();

            // Split the search text into individual keywords (handles multiple spaces)
            string[] keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Call the method to filter labels based on the entered keywords
            FilterLabels(keywords);
        }

        private void FilterLabels(string[] keywords)
        {
            // Iterate through each control in the TableLayoutPanel
            foreach (Control control in tableLayoutPanel.Controls)
            {
                // Check if the control is a Label (since the tableLayoutPanel could have other controls as well)
                if (control is Label label)
                {
                    bool isVisible = true;

                    // Check if each keyword is contained in the label's text
                    foreach (string keyword in keywords)
                    {
                        // If the label's text doesn't contain the keyword, hide the label
                        if (!label.Text.ToLower().Contains(keyword.ToLower()))
                        {
                            isVisible = false;
                            break;  // No need to check further keywords if one is not matched
                        }
                    }

                    // Show or hide the label based on whether it matches all keywords
                    label.Visible = isVisible;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(selectedFilePath == "")
            {
                MessageBox.Show(" Please open a file first !");
                return;
            }

            string filePath = selectedFilePath;

            // Call the method to save the labels to the file
            SaveLabelsToFile(filePath);
        }

        private void SaveLabelsToFile(string filePath)
        {
            // Initialize a list to store the labels' text
            List<string> labelTexts = new List<string>();

            // Loop through each control in the TableLayoutPanel
            foreach (Control control in tableLayoutPanel.Controls)
            {
                // Check if the control is a Label
                if (control is Label label)
                {
                    // Add the text of the label to the list
                    labelTexts.Add(label.Text);
                }
            }

            // Write the label texts to the file, overwriting the existing contents
            try
            {
                File.WriteAllLines(filePath, labelTexts);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void openLastFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(" Clicked! ");
            selectedFilePath = lastUsedFilePath;
            loadFileIntoTableLayoutPanel(lastUsedFilePath);
        }

        // New function to update the 'last_file_path' in the JSON file
        public void UpdateLastOpenedFilePath(string filePath)
        {
            string configFilePath = "config.json";

            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory();
            string fullConfigFilePath = Path.Combine(currentDirectory, configFilePath);

            if (File.Exists(fullConfigFilePath))
            {
                try
                {
                    // Read the JSON file content
                    string jsonContent = File.ReadAllText(fullConfigFilePath);

                    // Parse the JSON content
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        // Check if the "last_file_path" key exists
                        if (doc.RootElement.TryGetProperty("last_file_path", out JsonElement lastFilePathElement))
                        {
                            // Key exists, update the value
                            var updatedConfig = new
                            {
                                last_file_path = filePath
                            };

                            // Serialize the updated object to JSON
                            string updatedJsonContent = JsonSerializer.Serialize(updatedConfig, new JsonSerializerOptions { WriteIndented = true });

                            // Write the updated JSON back to the file
                            File.WriteAllText(fullConfigFilePath, updatedJsonContent);

                            //MessageBox.Show("The last file path has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // If the key doesn't exist, create and set the key
                            var updatedConfig = new
                            {
                                last_file_path = filePath
                            };

                            // Serialize the updated object to JSON
                            string updatedJsonContent = JsonSerializer.Serialize(updatedConfig, new JsonSerializerOptions { WriteIndented = true });

                            // Write the updated JSON back to the file
                            File.WriteAllText(fullConfigFilePath, updatedJsonContent);

                            //MessageBox.Show("The 'last_file_path' key has been created and updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Handle invalid JSON (parsing error)
                    MessageBox.Show($"Error parsing config.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Handle other unexpected errors
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // If the config file doesn't exist, create a default one and set the last_file_path
                MessageBox.Show("Configuration file not found. A default configuration will be created.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CreateDefaultConfigFile(fullConfigFilePath);
                UpdateLastOpenedFilePath(filePath); // Now update the path in the newly created file
            }
        }

        // Function to read key-value pairs from "filters.json" and add them to comboBox1
        private void LoadFiltersToComboBox()
        {
            string filtersFilePath = "filters.json";

            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory();
            string fullFiltersFilePath = Path.Combine(currentDirectory, filtersFilePath);

            if (File.Exists(fullFiltersFilePath))
            {
                try
                {
                    // Read the JSON file content
                    string jsonContent = File.ReadAllText(fullFiltersFilePath);

                    // Parse the JSON content
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        // Iterate through the key-value pairs
                        foreach (var property in doc.RootElement.EnumerateObject())
                        {
                            // Create a string in the "key-value" format
                            string item = $"{property.Name}-{property.Value.ToString()}";

                            // Add the item to the ComboBox
                            comboBox1.Items.Add(item);
                        }
                    }

                    // Optionally, set a default selected item or handle empty ComboBox
                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.SelectedIndex = 0; // Set the first item as the selected item
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Error parsing filters.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // If the file does not exist, notify the user
                MessageBox.Show("The filters.json file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString();
            string[] splitValues = selectedItem.Split('-');

            textBox1.Text = splitValues[1];
            //MessageBox.Show($"You selected: {selectedItem}", "Selection Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


}
