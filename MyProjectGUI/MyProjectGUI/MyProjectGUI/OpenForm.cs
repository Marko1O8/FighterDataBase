using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MyProjectGUI
{
    public partial class OpenForm : Form
    {
        const int KeyLength = 2;
        string FighterKey;
        public OpenForm()
        {
            InitializeComponent();
        }

        private void OpenForm_Load(object sender, EventArgs e)
        {
            SetupFighterDropDown();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FighterGUI.OpenFighterAccepted = false;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int KeyLen;

            if (comboBox1.Text.Length < KeyLength)
                KeyLen = comboBox1.Text.Length;
            else
                KeyLen = KeyLength;

            FighterGUI.OpenFighterAccepted = true;

            Debug.WriteLine("FighterNumComboBox.Text = " + comboBox1.Text);

            FighterKey = comboBox1.Text.Substring(0, KeyLen);
            Debug.WriteLine("FighterKey = " + FighterKey);
            Debug.WriteLine("FighterKey.Length = " + FighterKey.Length);
            FighterKey = FighterKey.Trim().PadLeft(KeyLength);
            Debug.WriteLine("FighterKey.Length = " + FighterKey.Length);
            Debug.WriteLine("FighterKey = " + FighterKey);

            FighterGUI.KeyText.Text = FighterKey;

            FighterGUI.OpenRecord(FighterKey);

            Close();
        }

        /***********************************************************************/
        /* Method to Setup the Driver # ComboBox.                              */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:  The Drivers object (drivers)                             */
        /*            The Driver # ComboBox (DriverNumComboBox)                */
        /***********************************************************************/
        void SetupFighterDropDown()
        {
            int i, n;
            string[][] Fighter;
            string item;

            try
            {
                Fighter = FighterGUI.fighter.GetFighter();
                for (i = 0; i < Fighter.Length; i++)
                {
                    item = String.Empty;
                    Debug.WriteLine("Fighter[{0}].Length= {1}", i, Fighter[i].Length);

                    for (n = 0; n < Fighter[i].Length; n++)
                    {
                        item += Fighter[i][n];
                        if (n < Fighter[i].Length - 1) item += ' ';
                    }
                    comboBox1.Items.Add(item);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
