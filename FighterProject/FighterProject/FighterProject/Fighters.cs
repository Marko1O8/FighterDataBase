using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FighterProject
{
    class Fighters
    {
        static string connectString;
        static string commandString;
        static SQLiteConnection connection = null;
        static SQLiteCommand sqlCmd;
        static bool done = false;
        static string[] FieldHeads = { "ID", "Rank", "First Name", "Last Name", "Age", "Height", "Weight", "Reach", "Wins", "Loss", "Draws" };
        static int[] FieldLens = { 5, 5, 15, 15, 7, 10, 10, 7, 7, 7, 7 };
        static int[] DataTypes = { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        // 0 -> No quotes, 1 -> Quotes needed

        /*************************************/
        /* Method to Connect to the database */
        /* Global input: connectString       */
        /* Global output: connection         */
        /*************************************/
        static void DBConnect()
        {
            try
            {
                connection = new SQLiteConnection(connectString);
                connection.Open();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        /*************************************/
        /* Method to Close the database      */
        /* Global input: connection          */
        /*************************************/
        static void DBClose()
        {
            if (connection != null) connection.Close();
        }

        /*****************************************************/
        /* Method to Insert a Record into the database       */
        /*                                                   */
        /* Input:   array of the field data in the Record    */
        /* Globals: DataTypes, sqlCmd                        */
        /*****************************************************/
        static void InsertRecord(string[] Record)
        {
            string commandString;
            int i;

            commandString = "insert into Fighter values (";
            for (i = 0; i < Record.Length; i++)
            {
                if (DataTypes[i] == 1) commandString += "'";
                commandString += Record[i];
                if (DataTypes[i] == 1) commandString += "'";
                if (i < Record.Length - 1) commandString += ",";
            }
            commandString += ")";

            try
            {
                sqlCmd = new SQLiteCommand();
                sqlCmd.CommandText = commandString;
                sqlCmd.Connection = connection;

                sqlCmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Write("Press <Ener> to contine: ");
                Console.ReadLine();
            }
        }

        /*****************************************************/
        /* Method to Delete a Record from the database       */
        /*                                                   */
        /* Input:   key field data                           */
        /* Globals: sqlCmd                                   */
        /*****************************************************/
        static void DeleteDBRecord(string key)
        {
            string commandString;

            commandString = "delete from Fighter where ID = ";
            commandString += key;

            try
            {
                sqlCmd = new SQLiteCommand();
                sqlCmd.CommandText = commandString;
                sqlCmd.Connection = connection;

                sqlCmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Write("Press <Ener> to contine: ");
                Console.ReadLine();
            }
        }

        /*********************************************************/
        /* Method to Display the records in the database         */
        /*                                                       */
        /* Globals: commandString, sqlCmd, FieldLens, FieldHeads */
        /*********************************************************/
        static void Display()
        {
            SQLiteDataReader reader = null;

            Console.Clear();

            commandString = "SELECT * FROM Fighter ";
            commandString += "ORDER BY ID";

            try
            {
                int i, k;

                sqlCmd = new SQLiteCommand();
                sqlCmd.CommandText = commandString;
                sqlCmd.Connection = connection;

                for (i = 0; i < FieldHeads.Length; i++)
                    Console.Write("{0,-" + FieldLens[i] + "} ", FieldHeads[i]);
                Console.WriteLine();

                for (i = 0; i < FieldHeads.Length; i++)
                {
                    for (k = 0; k < FieldLens[i]; k++)
                        Console.Write("=");
                    Console.Write(" ");
                }
                Console.WriteLine();

                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    for (i = 0; i < reader.FieldCount; i++)
                        Console.Write("{0,-" + FieldLens[i] + "} ", reader.GetValue(i).ToString());
                    Console.WriteLine();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Write("Press <Ener> to contine: ");
                Console.ReadLine();
            }

            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /*****************************************************/
        /* Method to Add a record to the database            */
        /* Gets the field data for the Record from the user  */
        /*                                                   */
        /* Globals: FieldHeads                               */
        /*****************************************************/
        static void AddRecord()
        {
            int i;
            string[] Record = new string[FieldHeads.Length];


            for (i = 0; i < FieldHeads.Length; i++)
            {
                Console.Write("Enter {0}: ", FieldHeads[i]);
                Record[i] = Console.ReadLine();
            }

            InsertRecord(Record);
        }

        /*****************************************************/
        /* Method to Delete a Record from the database       */
        /* Gets the primary key for the Record from the user */
        /*****************************************************/
        static void DeleteRecord()
        {
            string key;

            Console.Write("Enter ID of record to delete: ");
            key = Console.ReadLine();

            DeleteDBRecord(key);
        }

        /*****************************************************/
        /* Method to Display the Options for the user        */
        /* and call the proper Method                        */
        /*                                                   */
        /* Globals: done                                     */
        /*****************************************************/
        static void DisplayOptions()
        {
            int choice;

            Console.WriteLine();
            Console.WriteLine("1 = Add record    2 = Delete record   3 = Exit program");
            Console.WriteLine();

            do
            {
                Console.Write("Enter you choice: ");
                choice = Convert.ToInt32(Console.ReadLine());


                if (choice == 1)
                {
                    AddRecord();
                }
                else if (choice == 2)
                {
                    DeleteRecord();
                }
                else if (choice == 3)
                {
                    done = true;
                }
            }
            while (choice < 1 || choice > 3);
        }

        /*****************************************/
        /* Method to Maintain the database       */
        /*****************************************/
        static void Maintain()
        {
            Display();

            DisplayOptions();
        }

        /*************************************/
        /* Main Method                       */
        /*                                   */
        /* Globals: connectString, done      */
        /*************************************/
        static void Main(string[] args)
        {
            // The baseball database must be in the Current Directory
            connectString = "Data Source=Fighter.db";

            DBConnect();

            do
            {
                Maintain();
            }
            while (!done);

            DBClose();
        }
    }
}
