/*Program Name: FinalProjectConsole
 Date: 4/10/2022
Programmer: Marko Miserda
Class: CSCI

 Program Description: This program allows you to interact with the Fighter database. From creating new records, veiweing the records,
and deleting the records in Fighter.

 Input: ID, Rank, First Name, Last Name, Age, Height, Weight, Reach, Wins, Losses, Draws, OrgID
Output: ID, Rank, First Name, Last Name, Age, Height, Weight, Reach, Wins, Losses, Draws, OrgID

 Givens: Organizations

 Testing Data:
Fighter Records                 Sunday, April 10, 2022


ID    Rank  First Name      Last Name       Age   Height     Weight     Reach      Wins  Losses Draws Organization
===== ===== =============== =============== ===== ========== ========== ========== ===== ===== ===== ===============
01    20    Marko           Miserda         22    6ft 4in    205 lbs    78in       0     0     0     ONE Championship
02    C     Jon             Jones           34    6ft 4in    205 lbs    84in       26    1     0     Ultimate Fighting Championship
03    2     Stipe           Miocic          39    6ft 4in    234 lbs    80in       20    4     0     Ultimate Fighting Championship
04    C     Israel          Adesanya        32    6ft 3in    185 lbs    80in       22    1     0     Ultimate Fighting Championship
05    1     Max             Holloway        30    5ft 9in    146 lbs    69in       23    6     0     Ultimate Fighting Championship
06    4     Benson          Henderson       38    5ft 8in    155 lbs    71in       29    11    0     Bellator
07    3     Adam            Borics          29    5ft 10in   145 lbs    71in       18    1     0     Bellator
08    9     Conor           Mcgregor        33    5ft 7in    156 lbs    74in       22    6     0     Ultimate Fighting Championship
09    C     Patricky        Pitbull         36    5ft 6in    155 lbs    71in       24    10    0     Bellator
10    5     Jenelyn         Olsim           24    5ft 3in    115 lbs    60in       5     3     0     ONE Championship

Press any key to return to main menu:

 see Console for more.*/
using System;

public class FighterConsole
{
    static Fighter fighter; // Declare the Drivers object, Created in Main
    static string[] Menuitems = { "New Record", "Open Record", "Display All Records", "Undelete", "Purge",  "Exit" };
    static string[] Record; // Holds individual Driver records
    static string[][] theFighter; // Holds the Drivers (Master) table data
    static string[][] Organization; // Holds the Manufacturers (support) table data
    static string[][] theCopy; //Holds the Copy table data
    static int LongestFieldLabel; // for display formatting
    static string FighterID;    // Master table primary key

    /*** Non Database display methods ***/

    /***********************************************************************/
    /* Method to display the program screen heading.                       */
    /***********************************************************************/
    static void heading()
    {
        Console.Clear();
        Console.Write("Fighter Records                 ");
        Console.WriteLine(DateTime.Now.ToLongDateString());
        Console.WriteLine();
    }

    /***********************************************************************/
    /* Method to display the program menu.                                 */
    /***********************************************************************/
    static void DisplayMenu()
    {
        int i;

        for (i = 0; i < Menuitems.Length; i++)
            Console.WriteLine($"{i + 1}. {Menuitems[i]}\n");
    }

    /*** Database Support Table methods ***/

    /***********************************************************************/
    /* Method to get a database View (an array of records where each       */
    /* record is an array of strings) of the support table.                */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*    Output:	The Manufacturers View (Manufacturers)                  */
    /***********************************************************************/
    static void GetSupportData()
    {
        try
        {
            Organization = fighter.GetOrganization();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /*** Database New & Open Record methods ***/

    /***********************************************************************/
    /* Method to display a record.                                         */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*             The length of the longest Field Heading                 */
    /*                (LongestFieldLabel)                                  */
    /*             The Manufacturers View (Manufacturers)                  */
    /*             The record (Record)                                     */
    /***********************************************************************/
    static void DisplayRecord()
    {
        int i;

        for (i = 0; i < Fighter.FieldHeads.Length; i++)
        {
            // We use the older numbered formatting because the newer $ formatting
            // requires a constant for field display width
            Console.Write("{0}. {1," + LongestFieldLabel + "}: ", i + 1, Fighter.FieldHeads[i]);
            Console.Write(Record[i]);
            if (i == 11) // foreign key Field
            {
                Console.Write("   (");
                Console.Write(Fighter.FindValue(Record[i], Organization, 1));
                Console.Write(")");
            }
            Console.WriteLine();
        }
    }

    /***********************************************************************/
    /* Method to Open a record.                                            */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*    Output:	The Record, array of strings (Record)                   */
    /*             A Driver# (DriverNo)                                    */
    /***********************************************************************/
    static void OpenRecord()
    {
        heading();  // clear the screen

        Console.WriteLine();

        Console.Write("Enter a fighter ID: ");
        FighterID = Console.ReadLine();
        FighterID = FighterID.PadLeft(2); // Right justify the Driver#

        try
        {
            Record = fighter.GetRecord(FighterID);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }

        Console.WriteLine();
        if (Record[0] == "no record found") // The DLL set this if record not found
        {
            Console.WriteLine("Record for Fighter " + FighterID + " not found");
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
        else
        {
            DisplayRecord();
            RecordOptions(); // Display the Record Options
        }
    }

    /***********************************************************************/
    /* Method to Save a record.                                            */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*           	The Record, array of strings (Record)                   */
    /***********************************************************************/
    static void SaveRecord()
    {
        try
        {
            fighter.SaveRecord(Record);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /***********************************************************************/
    /* Method to Delete a record.                                          */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /***********************************************************************/
    static void DeleteRecord()
    {
        try
        {
            fighter.SaveCopy(Record);
            fighter.DeleteRecord(FighterID);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /***********************************************************************/
    /* Method to get a New record.                                         */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*    Output:	The Record, array of strings (Record)                   */
    /***********************************************************************/
    static void NewRecord()
    {
        int i;
        string value;
        bool OK;
        int number;

        heading();    // clear the screen

        Record = new string[Fighter.FieldHeads.Length];

        Console.WriteLine();

        // ask the uesr to enter a value for each field
        for (i = 0; i < Fighter.FieldHeads.Length; i++)
        {
            Console.Write($"Enter value for {Fighter.FieldHeads[i]}: ");
            value = Console.ReadLine();
            if (i == 0)
            {
                value = value.PadLeft(2);  // Right justify the Driver#
                if (fighter.RecordExists(fighter.tableName, fighter.keyName, value))
                {
                    Console.WriteLine("Fighter ID  " + value + " already exists");
                    Console.WriteLine("Press any key to return to main menu: ");
                    Console.ReadLine();
                    return;
                }
            }
            if (i == Fighter.FieldHeads.Length - 1) // Manufacturers foreign key
            {
                OK = false;
                do
                {
                    OK = int.TryParse(value, out number);
                    if (!OK)
                    {
                        Console.Write($"Enter value for {Fighter.FieldHeads[i]} (Must be an integer): ");
                        value = Console.ReadLine();
                    }
                }
                while (!OK);
                value = $"{number:d2}"; //Zero Fill to 2 digits
            }
            Record[i] = value;
        }
        Console.WriteLine();

        FighterID = Record[0];

        SaveRecord();
        DisplayRecord();
        RecordOptions();  // Display the Record Options
    }

    /***********************************************************************/
    /* Method to display and process the record options.                   */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /***********************************************************************/
    static void RecordOptions()
    {
        int choice;
        string value;
        bool OK;
        int number;

        Console.WriteLine();
        Console.WriteLine("1 = Modify record    2 = Delete record   3 = Return to main menu");
        Console.WriteLine();
        Console.Write("Enter you choice: ");
        value = Console.ReadLine();
        OK = false;
        do
        {
            OK = int.TryParse(value, out number);
            if (!OK)
            {
                Console.Write("Enter you choice (Must be an integer): ");
                value = Console.ReadLine();
            }
        }
        while (!OK);
        choice = number;

        if (choice == 1)  // Modify record
        {
            Console.WriteLine();
            Console.Write("Which field do you want to modify? ");
            value = Console.ReadLine();
            OK = false;
            do
            {
                OK = int.TryParse(value, out number);
                if (!OK)
                {
                    Console.Write("Which field do you want to modify? (Must be an integer)");
                    value = Console.ReadLine();
                }
            }
            while (!OK);
            choice = number;
            if (choice >= 1 && choice <= Fighter.FieldHeads.Length)
            {
                Console.Write($"Enter new value for field {choice}: ");
                value = Console.ReadLine();
                // Manufacturers foreign key
                if (choice == 6)
                {
                    OK = false;
                    do
                    {
                        OK = int.TryParse(value, out number);
                        if (!OK)
                        {
                            Console.Write($"Enter new value for field {choice} (Must be an integer): ");
                            value = Console.ReadLine();
                        }
                    }
                    while (!OK);
                    value = $"{number:d2}"; //Zero Fill to 2 digits
                }
                Record[choice - 1] = value;
                SaveRecord();
                DisplayRecord();
            }
        }
        else if (choice == 2) // Delete record
        {
            DeleteRecord();
        }

        // Nothing is needed for choice 3 because we return to the main loop
    }

    /*** Database Display All Records methods ***/

    /***********************************************************************/
    /* Method to display all master table records with the supporting data */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*             An array of Field Lengths (FieldLens)                   */
    /*             An array of Field Headings (FieldHeads)                 */
    /*             The Drivers View, an array of records where each record */
    /*             is an array of strings (theDrivers)                     */
    /***********************************************************************/
    static void DisplayAllRecords()
    {
        int i, k;

        heading();

        Console.WriteLine();

        // We use the older numbered formatting because the newer $ formatting
        // requires a constant for field display width
        for (i = 0; i < Fighter.FieldHeads.Length; i++)
            Console.Write("{0,-" + fighter.FieldLens[i] + "} ", Fighter.FieldHeads[i]);
        Console.WriteLine();

        for (i = 0; i < Fighter.FieldHeads.Length; i++)
            Console.Write($"{new string('=', fighter.FieldLens[i])} ");
        Console.WriteLine();

        try
        {
            theFighter = fighter.GetAllFighter();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }

        for (i = 0; i < theFighter.Length; i++)
        {
            // We use the older numbered formatting because the newer $ formatting
            // requires a constant for field display width
            for (k = 0; k < theFighter[i].Length; k++)
                Console.Write("{0,-" + fighter.FieldLens[k] + "} ", theFighter[i][k]);
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.WriteLine("Press any key to return to main menu: ");
        Console.ReadLine();
    }

    //Shows all deleted
    static void DisplayAllCopy()
    {
        int i, k;
        int choice;
        string value;
        bool OK;
        int number;

        heading();

        Console.WriteLine();

        // We use the older numbered formatting because the newer $ formatting
        // requires a constant for field display width
        for (i = 0; i < Fighter.FieldHeads.Length; i++)
            Console.Write("{0,-" + fighter.FieldLens[i] + "} ", Fighter.FieldHeads[i]);
        Console.WriteLine();

        for (i = 0; i < Fighter.FieldHeads.Length; i++)
            Console.Write($"{new string('=', fighter.FieldLens[i])} ");
        Console.WriteLine();

        try
        {
            theCopy = fighter.GetAllCopy();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }

        for (i = 0; i < theCopy.Length; i++)
        {
            // We use the older numbered formatting because the newer $ formatting
            // requires a constant for field display width
            for (k = 0; k < theCopy[i].Length; k++)
                Console.Write("{0,-" + fighter.FieldLens[k] + "} ", theCopy[i][k]);
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.WriteLine("1 = Undelete    2 = Return to Menu ");
        Console.WriteLine();
        Console.Write("Enter you choice: ");
        value = Console.ReadLine();
        OK = false;
        do
        {
            OK = int.TryParse(value, out number);
            if (!OK)
            {
                Console.Write("Enter you choice (Must be an integer): ");
                value = Console.ReadLine();
            }
        }
        while (!OK);
        choice = number;

        if(choice == 1)
        {
            Console.WriteLine();
            Console.Write("Which field do you want to Undelete? ");
            value = Console.ReadLine();
            OK = false;
            do
            {
                OK = int.TryParse(value, out number);
                if (!OK)
                {
                    Console.Write("Which field do you want to modify? (Must be an integer)");
                    value = Console.ReadLine();
                }
            }
            while (!OK);
            choice = number;
            if (choice >= 1 && choice <= Fighter.FieldHeads.Length)
            {
               Record = fighter.GetCopy(value);
               fighter.SaveRecord(Record);
               fighter.DeleteCopy(value);
            }
        }
    }

    //Deletes all deleted programs
    static void Purge()
    {
        heading();

        Console.WriteLine("Are you sure you want to purge all deleted records? (Y/N)");
        string x = Console.ReadLine();

        if(x == "y" || x == "Y")
        {
            fighter.DeleteAllCopy(true);
        }
        
       
    }

    /*** Main method ***/

    static void Main(string[] args)
    {
        int choice;
        bool OK;
        int number;
        string value;

        try
        {
            fighter = new Fighter();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        LongestFieldLabel = Fighter.GetLongestFieldLabel();
        GetSupportData();

        do
        {
            heading();
            DisplayMenu();

            Console.Write("Enter your choice: ");
            value = Console.ReadLine();
            OK = false;
            do
            {
                OK = int.TryParse(value, out number);
                if (!OK)
                {
                    Console.WriteLine("Enter your choice (Must be an integer): ");
                    value = Console.ReadLine();
                }
            }
            while (!OK);
            choice = number;

            switch (choice)
            {
                case 1:
                    NewRecord();
                    break;
                case 2:
                    OpenRecord();
                    break;
                case 3:
                    DisplayAllRecords();
                    break;
                case 4:
                    DisplayAllCopy();
                    break;
                case 5:
                    Purge();
                    break;
            }
        }
        while (choice != Menuitems.Length);  // We set the last menu item to exit
    }
}