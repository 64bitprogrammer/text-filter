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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Text_Filter
{
    public partial class Form1 : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private string selectedFilePath = "";
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

            // Read the file content and add each line as a new item to the TableLayoutPanel
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // Create a new Label for each line in the file
                Label label = new Label();
                label.Text = line;
                label.AutoSize = true;  // Adjust size based on content
                label.Padding = new Padding(5);
                //label.BorderStyle = BorderStyle.FixedSingle;  // Optional: add border to label for visual feedback

                // Add the Label to the TableLayoutPanel (one item per row)
                tableLayoutPanel.RowCount++;
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // Allow dynamic row sizing
                tableLayoutPanel.Controls.Add(label, 0, tableLayoutPanel.RowCount - 1);  // Add to column 0 and next available row

                // Attach the double-click event handler
                label.DoubleClick += Label_DoubleClick;
            }
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
    }
}
