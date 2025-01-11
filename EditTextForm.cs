using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Text_Filter
{
    public partial class EditTextForm : Form
    {
        public string UpdatedText { get; private set; }
        public EditTextForm()
        {
            InitializeComponent();
        }

        public EditTextForm(string currentText)
        {
            InitializeComponent();
            textBox1.Text = currentText;  // Set the current text in the TextBox
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;  // Close the form with Cancel result
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdatedText = textBox1.Text;  // Get the updated text from the TextBox
            DialogResult = DialogResult.OK;  // Close the form with OK result
        }
    }
}
