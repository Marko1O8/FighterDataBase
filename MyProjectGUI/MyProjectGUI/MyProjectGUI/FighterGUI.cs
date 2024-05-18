/*Program Name: GUIPROJECT
 Date: 3/23/2022
Programmer: Marko Miserda
Class: CSCI

 Program Description: The form consists of a menu strip, labels, and text boxes. The only functional part of 
the GUI is the exit button. The labels show the fighter id, name, age, height, weight, reach and record.

 Input: None
Output: None

 Givens: none

 Testing Data: Check the GUI*/
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
    public partial class FighterGUI : Form
    {
        public static Fighter fighter; // Declare the Drivers object, Created in Constructor
        static int FontWidth;
        static int LeftMargin = 6;
        static int TopMargin = 35;
        static int ExtraWidth = 120;
        int FieldTop;
        int LongestFieldLabel; // for display formatting
        static int VerticalKeySep = 10;
        static int VerticalFieldSep = 10;
        static int VerticalFieldPad = 6;
        Graphics g;
        Label KeyLabel = new Label();
        public static Label KeyText = new Label();
        public static bool OpenFighterAccepted;
        static string[] Record;  // Holds individual Driver records
        static string[] CleanRecord; // Holds Clean individual Driver records (After a Save)
        static string[][] Organization;  // Holds the Manufacturers (support) table data
        static int NumCombo = 1; // Number of support foreign key fields
        static int NumText; // Number of non foreign key fields
        public static TextBox[] FieldText; // Array of TextBoxes for non foreign key fields
        public static ComboBox[] FieldCombo; // Array of ComboBoxes for foreign key fields
        Label[] FieldLabels; // Array of Labels for Label fields
        int[] FieldSize = { 5, 5, 15, 15, 5, 10, 10, 10, 5, 5, 5, 15 }; // Array of non key field sizes
        static bool RecordLoaded = false;
        public FighterGUI()
        {

            int i = 1;

            Debug.Print("Before InitializeComponent");
            InitializeComponent();
            Debug.Print("after InitializeComponent");
            try
            {
                fighter = new Fighter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }


            NumText = Fighter.FieldHeads.Length - NumCombo;

            // This section creates the GUI controls dynamically
            // to improve the reusability of the code
            // You may create them statically with drag and drop
            // if you prefer
            FieldLabels = new Label[Fighter.FieldHeads.Length];
            FieldText = new TextBox[NumText];
            FieldCombo = new ComboBox[NumCombo];
            LongestFieldLabel = Fighter.GetLongestFieldLabel();

            SuspendLayout();

            g = CreateGraphics();
            FontHeight = (int)(Font.SizeInPoints / 72 * g.DpiX);
            FontWidth = (FontHeight * 8) / 10;

            FieldTop = TopMargin + FontHeight + VerticalFieldPad + VerticalFieldSep + VerticalKeySep;

            KeyLabel = new Label();
            KeyLabel.AutoSize = false;
            KeyLabel.Location = new Point(LeftMargin, TopMargin);
            KeyLabel.Name = "KeyLabel";
            KeyLabel.Text = "Fighter ID";
            KeyLabel.Size = new Size(FontWidth * KeyLabel.Text.Length + 2, FontHeight + VerticalFieldPad);
            Controls.Add(KeyLabel);

            KeyText = new Label();
            KeyText.AutoSize = false;
            KeyText.Location = new Point(LeftMargin + KeyLabel.Size.Width + 2, TopMargin);
            KeyText.Name = "KeyText";
            KeyText.Text = String.Empty;
            KeyText.Size = new Size(FontWidth * KeyLabel.Text.Length + 2, FontHeight + VerticalFieldPad);
            KeyText.BorderStyle = BorderStyle.Fixed3D;
            KeyText.BackColor = Color.White;
            Controls.Add(KeyText);

            for (i = 1; i < Fighter.FieldHeads.Length; i++)
            {

                FieldLabels[i] = new Label();
                FieldLabels[i].AutoSize = false;
                FieldLabels[i].Location = new Point(LeftMargin, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));
                FieldLabels[i].Name = "FieldLabel" + i;
                FieldLabels[i].Text = Fighter.FieldHeads[i];
                FieldLabels[i].Size = new Size(FontWidth * LongestFieldLabel + 2, FontHeight + VerticalFieldPad);
                FieldLabels[i].TextAlign = ContentAlignment.MiddleRight;
                Controls.Add(FieldLabels[i]);

                if (i < NumText)
                {
                    FieldText[i] = new TextBox();
                    FieldText[i].AutoSize = false;
                    FieldText[i].MinimumSize = new System.Drawing.Size(0, 0);
                    FieldText[i].Location = new Point(FieldLabels[i].Location.X + FieldLabels[i].Size.Width + 5, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));
                    FieldText[i].Name = "FieldText" + i;
                    FieldText[i].Text = String.Empty;
                    FieldText[i].Size = new Size(FontWidth * FieldSize[i] + ExtraWidth, FontHeight + VerticalFieldPad);
                    Controls.Add(FieldText[i]);
                }
                else
                {
                    FieldCombo[i - NumText] = new ComboBox();
                    FieldCombo[i - NumText].AutoSize = false;
                    FieldCombo[i - NumText].MinimumSize = new System.Drawing.Size(0, 0);
                    FieldCombo[i - NumText].Location = new Point(FieldLabels[i].Location.X + FieldLabels[i].Size.Width + 5, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));


                    FieldCombo[i - NumText].Name = "FieldCombo" + i;
                    FieldCombo[i - NumText].Text = String.Empty;
                    FieldCombo[i - NumText].Size = new Size(FontWidth * FieldSize[i - NumText] + ExtraWidth, FontHeight + VerticalFieldPad);
                    FieldCombo[i - NumText].DropDownStyle = ComboBoxStyle.DropDownList;
                    Controls.Add(FieldCombo[i - NumText]);
                }
            }

            ResumeLayout(false);

            SetupOrganizationDropDown();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fighter Database\nAuthor Marko Miserda\n2022");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveRecord();
        }

        /***********************************************************************/
        /* Method to Save (Copy) the contents of the record into the           */
        /* CleanRecord after a Save                                            */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Record, array of strings (Record)                   */
        /*    Output:	The Clean Record, array of strings (CleanRecord)        */
        /***********************************************************************/
        private static void Copy2CleanRecord()
        {
            int i;

            CleanRecord = new string[Record.Length];
            for (i = 0; i < Record.Length; i++)
                CleanRecord[i] = Record[i];
        }

        /***********************************************************************/
        /* Method to fill the Drop Down list for the Manufacturers             */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Manufacturers View (Manufacturers)                  */
        /*             The Drivers object (drivers)                            */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /***********************************************************************/
        void SetupOrganizationDropDown()
        {
            int i;

            try
            {
                Organization = fighter.GetOrganization();

                for (i = 0; i < Organization.Length; i++)
                    FieldCombo[0].Items.Add(Organization[i][1]);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " SetupOrganizationDropDown");
            }
        }

        /***********************************************************************/
        /* Method to Clear the Data on the screen                              */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Number of non foreign key fields (NumText)              */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /*             Array of TextBoxes for non foreign key fields(FieldText)*/
        /*             An array of Field Headings (FieldHeads)                 */
        /***********************************************************************/
        private static void ClearDataEntry()
        {
            int i;

            // We are starting a 1 instead of 0 because the
            // first control is done separately with KeyLabel and KeyText
            for (i = 1; i < Fighter.FieldHeads.Length; i++)
            {
                if (i < NumText)
                    FieldText[i].Text = String.Empty;
                else
                    FieldCombo[i - NumText].SelectedIndex = -1; //No selection
            }
        }

        /***********************************************************************/
        /* Method to check if the contents of the record are dirty             */
        /* i.e. has any thing changed since the last save                      */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Record, array of strings (Record)                   */
        /*             The Clean Record, array of strings (CleanRecord)        */
        /***********************************************************************/
        private static bool RecordChanged()
        {
            int i;

            for (i = 0; i < Record.Length; i++)
                if (CleanRecord[i] != Record[i]) return true;

            return false;
        }

        /***********************************************************************/
        /* Method to Open a record.                                            */
        /* Called from OpenForm                                                */
        /*                                                                     */
        /* Input:	A Driver# (key)                                            */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Record, array of strings (Record)                   */
        /*             The Drivers object (drivers)                            */
        /*             Number of non foreign key fields (NumText)              */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /*             Array of TextBoxes for non foreign key fields(FieldText)*/
        /*    Output:	Boolean to track when a record is loaded (RecordLoaded) */
        /***********************************************************************/
        public static void OpenRecord(string key)
        {
            int i;

            try
            {
                Debug.Print("RecordLoaded = " + RecordLoaded);
                Record = fighter.GetRecord(key);
                for (i = 0; i < Record.Length; i++)
                   Debug.Print("Record[" + i + "] = " + Record[i]);
                if (Record[0] == "no record found") // The DLL set this if record not found
                {
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                    RecordLoaded = false;
                    MessageBox.Show("Record for Fighter " + key + " not found");
                }
                else
                {
                    // We are starting a 1 instead of 0 because the
                    // first control is done separately with KeyLabel and KeyText
                    for (i = 1; i < Fighter.FieldHeads.Length; i++)
                    {
                        // We are assuming the foreign key Fields are 
                        // at the end of the record
                        if (i < NumText)  // Text Fields
                            FieldText[i].Text = Record[i];
                        else              // foreign key ComboBox Fields
                        {
                            Debug.Print("i = " + i);
                            Debug.Print("NumText = " + NumText);
                            Debug.Print("FieldCombo.Length = " + FieldCombo.Length);
                            Debug.Print("Record.Length = " + Record.Length);
                            FieldCombo[i - NumText].Text = Fighter.FindValue(Record[i], Organization, 1);
                            Debug.Print("Record[" + i + "] = " + Record[i]);
                            Debug.Print("FieldCombo[" + (i - NumText) + "].Text = " + FieldCombo[i - NumText].Text);
                        }
                    }
                    RecordLoaded = true;
                    Copy2CleanRecord();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " OpenRecord");
            }
        }

        /***********************************************************************/
        /* Method to Copy the contents of the GUI fields to the Record in      */
        /* memory so they can be saved to the database                         */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Number of non foreign key fields (NumText)              */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /*             Array of TextBoxes for non foreign key fields(FieldText)*/
        /*    Output:	The Record, array of strings (Record)                   */
        /***********************************************************************/
        private static void ResetRecord()
        {
            int i;
            int index;

            Debug.Print("NumText = " + NumText);

            // We are starting a 1 instead of 0 because the
            // first control is done separately with KeyLabel and KeyText
            for (i = 1; i < Fighter.FieldHeads.Length; i++)
            {
                if (i < NumText)
                    Record[i] = FieldText[i].Text;
                else
                {
                    try
                    {
                        index = FieldCombo[i - NumText].SelectedIndex;
                        Record[i] = Organization[index][0];
                        Debug.Print("ResetRecord Record[" + i + "] = " + Record[i]);
                    }

                    catch (Exception ex)
                    {
                        Debug.Print("ResetRecord Exception");
                        Debug.Print(ex.Message);
                        Debug.Print("i = " + i);
                        Debug.Print("fighter.FieldHeads.Length = " + Fighter.FieldHeads.Length);
                        Debug.Print("Record.Length = " + Record.Length);

                        Record[i] = String.Empty;
                    }
                }
            }
        }

        /***********************************************************************/
        /* Method to save a loaded record to the database                      */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Boolean to track when a record is loaded (RecordLoaded) */
        /*             The Drivers object (drivers)                            */
        /*    Output:	The Record, array of strings (Record)                   */
        /***********************************************************************/
        private static void SaveRecord()

        {
            if (RecordLoaded)
            {
                ResetRecord();
                Record[0] = KeyText.Text;

                if (RecordChanged())
                {
                    try
                    {
                        fighter.SaveRecord(Record);
                        Copy2CleanRecord();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "SaveRecord");
                    }
                }
                else
                    Copy2CleanRecord();
            }
            else
                MessageBox.Show("No record loaded to save");
        }

        /***********************************************************************/
        /* Method to check if the contents of the record are dirty             */
        /* i.e. has any changed since the last save.                           */
        /* If it is dirty alert the user and give them an opportunity to       */
        /* save it.                                                            */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Boolean to track when a record is loaded (RecordLoaded) */
        /***********************************************************************/
        private static void CheckRecordChanged()
        {
            DialogResult result;

            if (RecordLoaded && RecordChanged())
            {
                result = MessageBox.Show("Record changed. do you want to save the changes?",
                   "Nascar Maintenance", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm objOpenForm = new OpenForm();

            if (RecordLoaded) ResetRecord();
            CheckRecordChanged();

            objOpenForm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string key;

            if (RecordLoaded)
            {
                key = Record[0];
                try
                {
                    fighter.SaveCopy(Record); //Yoooooooooo
                    fighter.DeleteRecord(key);
                    RecordLoaded = false;
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " deleteToolStripMenuItem_Click");
                }
            }
            else
                MessageBox.Show("No record loaded to delete");
        }

        private void displayAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayForm objDisplayForm = new DisplayForm();

            objDisplayForm.ShowDialog();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewForm objNewForm = new NewForm();

            if (RecordLoaded) ResetRecord();
            CheckRecordChanged();

            objNewForm.ShowDialog();

            if (NewForm.OK)
            {
                Record = new string[Fighter.FieldHeads.Length];
                Record[0] = KeyText.Text;
                ClearDataEntry();
                ResetRecord();
                RecordLoaded = true;
                Copy2CleanRecord();
            }
        }

        private void undeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndeleteForm objNewForm = new UndeleteForm();

            objNewForm.ShowDialog();

            string key;

            if (RecordLoaded)
            {
                key = Record[0];
                try
                {
                    fighter.SaveRecord(Record); //Yoooooooooo
                    fighter.DeleteCopy(key);
                    RecordLoaded = false;
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " deleteToolStripMenuItem_Click");
                }
            }
            else
                MessageBox.Show("No record loaded to delete");
        }

        public static void OpenCopy(string key)
        {
            int i;

            try
            {
                Debug.Print("RecordLoaded = " + RecordLoaded);
                Record = fighter.GetCopy(key);
                for (i = 0; i < Record.Length; i++)
                    Debug.Print("Record[" + i + "] = " + Record[i]);
                if (Record[0] == "no record found") // The DLL set this if record not found
                {
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                    RecordLoaded = false;
                    MessageBox.Show("Record for Fighter " + key + " not found");
                }
                else
                {
                    // We are starting a 1 instead of 0 because the
                    // first control is done separately with KeyLabel and KeyText
                    for (i = 1; i < Fighter.FieldHeads.Length; i++)
                    {
                        // We are assuming the foreign key Fields are 
                        // at the end of the record
                        if (i < NumText)  // Text Fields
                            FieldText[i].Text = Record[i];
                        else              // foreign key ComboBox Fields
                        {
                            Debug.Print("i = " + i);
                            Debug.Print("NumText = " + NumText);
                            Debug.Print("FieldCombo.Length = " + FieldCombo.Length);
                            Debug.Print("Record.Length = " + Record.Length);
                            FieldCombo[i - NumText].Text = Fighter.FindValue(Record[i], Organization, 1);
                            Debug.Print("Record[" + i + "] = " + Record[i]);
                            Debug.Print("FieldCombo[" + (i - NumText) + "].Text = " + FieldCombo[i - NumText].Text);
                        }
                    }
                    RecordLoaded = true;
                    Copy2CleanRecord();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " OpenRecord");
            } 
        }

        private void purgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurgeForm objNewForm = new PurgeForm();

            objNewForm.ShowDialog();
        }

        private void FighterGUI_Load(object sender, EventArgs e)
        {

        }
    }
}
