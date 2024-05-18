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
    public partial class DisplayForm : Form
    {
        static int[] ColWidths = { 25, 50, 50, 50, 30, 50, 50, 45, 40, 50, 50, 175 };
        static string[][] theFighter;
        static string[] TableData = new string[Fighter.FieldHeads.Length];

        public DisplayForm()
        {
            InitializeComponent();
        }

        private void DisplayForm_Load(object sender, EventArgs e)
        {
            int i, k;

            try
            {
                theFighter = FighterGUI.fighter.GetAllFighter();
                dataGridView1.ColumnCount = Fighter.FieldHeads.Length;
                for (i = 0; i < Fighter.FieldHeads.Length; i++)
                {
                    dataGridView1.Columns[i].Name = Fighter.FieldHeads[i];
                    dataGridView1.Columns[i].Width = ColWidths[i];
                }

                for (i = 0; i < theFighter.Length; i++)
                {
                    for (k = 0; k < theFighter[i].Length - 1; k++)
                        TableData[k] = theFighter[i][k];
                    TableData[k] = theFighter[i][k];

                    dataGridView1.Rows.Add(TableData);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
