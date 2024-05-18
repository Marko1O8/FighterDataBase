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
    public partial class UndeleteForm : Form
    {
        const int KeyLength = 2;
        string FighterKey;
        public UndeleteForm()
        {
            InitializeComponent();
        }

        private void UndeleteForm_Load(object sender, EventArgs e)
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

            FighterGUI.OpenCopy(FighterKey);

            Close();
        }
        void SetupFighterDropDown()
        {
            int i, n;
            string[][] Copy;
            string item;

            try
            {
                Copy = FighterGUI.fighter.GetFighterCopy();
                for (i = 0; i < Copy.Length; i++)
                {
                    item = String.Empty;
                    Debug.WriteLine("Copy[{0}].Length= {1}", i, Copy[i].Length);

                    for (n = 0; n < Copy[i].Length; n++)
                    {
                        item += Copy[i][n];
                        if (n < Copy[i].Length - 1) item += ' ';
                    }
                    comboBox1.Items.Add(item);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
