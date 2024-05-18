using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyProjectGUI
{
    public partial class NewForm : Form
    {
        const int KeyLength = 2;
        string FighterKey;
        public static bool OK = false;
        public NewForm()
        {
            InitializeComponent();
        }

        private void NewForm_Load(object sender, EventArgs e)
        {
            SetupFighterListBox();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /***********************************************************************/
        /* Method to determine if a string is an integer.                      */
        /*                                                                     */
        /* Input:	A string (s)                                               */
        /*                                                                     */
        /* Returns: True or False.                                             */
        /***********************************************************************/
        private bool IsInteger(string s)
        {
            int i;

            for (i = 0; i < s.Length; i++)
                if (s[i] < '0' || s[i] > '9') return false;

            return true;
        }

        /***********************************************************************/
        /* Method to Setup the DriverListBox.                                  */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Drivers object (drivers)                            */
        /*    Output:	The Driver ListBox (DriverListBox)                      */
        /***********************************************************************/
        void SetupFighterListBox()
        {
            int i, n;
            string[][] Fighter;
            string item;

            try
            {
                Fighter = FighterGUI.fighter.GetCurrentFighter();
                for (i = 0; i < Fighter.Length; i++)
                {
                    item = String.Empty;

                    for (n = 0; n < Fighter[i].Length; n++)
                    {
                        item += Fighter[i][n];
                        if (n < Fighter[i].Length - 1) item += ' ';
                    }
                    listBox1.Items.Add(item);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " SetupFighterListBox");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsInteger(textBox1.Text))
            {
                MessageBox.Show("Fighter ID must be an integer");
                textBox1.Focus();
                textBox1.SelectAll();
            }
            else if (textBox1.Text.Length > KeyLength)
            {
                MessageBox.Show("Fighter ID cannot be longer than " + KeyLength + " digits");
                textBox1.Focus();
                textBox1.SelectAll();
            }
            else
            {
                FighterKey = textBox1.Text.PadLeft(KeyLength);
                if (FighterGUI.fighter.RecordExists(FighterGUI.fighter.tableName, FighterGUI.fighter.keyName, FighterKey))
                {
                    MessageBox.Show("Fighter ID  " + FighterKey + " already exists");
                    textBox1.Focus();
                    textBox1.SelectAll();
                }
                else
                {
                    FighterGUI.KeyText.Text = FighterKey;
                    OK = true;
                    Close();
                }
            }
        }
    }
}
