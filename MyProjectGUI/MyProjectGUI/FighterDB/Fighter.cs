/*Program Name: FinalProjectLibary
 Date: 4/10/2022
Programmer: Marko Miserda
Class: CSCI

 Program Description: This program connects the program to the SQLITE database Fighter. 
It also contains the methods used in FighterConsole and FighterGUI to interact with the 
SQLITE database.

 Input: Fighter.db
Output: None

 Givens:none

 Testing Data:see FighterConsole and GUI*/
using System;
using System.Data.SQLite;
using System.Diagnostics;

public class Fighter
{
    private SQLiteConnection connection = null; // null -> not open
    private SQLiteCommand sqlCmd;
    private SQLiteDataReader reader;
    // The following parallel arrays and names drive the solution
    // and allow the code to be used for different databases with
    // minimal change.

    // For the display
    static public string[] FieldHeads = { "ID", "Rank", "First Name", "Last Name", "Age", "Height", "Weight", "Reach", "Wins", "Losses", "Draws", "Organization" };
    public int[] FieldLens = { 5, 5, 15, 15, 5, 10, 10, 10, 5, 5, 5, 15 };

    // For SQL UPDATE statement
    static string[] FieldNames = { "ID", "Rank", "First", "Last", "Age", "Height", "Weight", "Reach", "Wins", "Loss", "Draws", "OrgID"};

    // For SQL statements
    int[] DataTypes = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 }; // 0 -> No quotes, 1 -> Quotes needed
    public string keyName = FieldNames[0]; // Master table
    public string tableName = "Fighter";   // Master table
    string DBName = "Fighter.db";

    /*** Constructors and Destructors ***/
    public Fighter()
    {
        DBConnect(DBName);
    }

    ~Fighter()
    {
        DBClose();
    }

    /*** Connection methods ***/

    /***********************************************************************/
    /* Void Method to open a connection to the database                    */
    /*                                                                     */
    /* Input:	the database name (DBName)                                 */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Output:	connection                                              */
    /***********************************************************************/
    void DBConnect(string DBName)
    {
        string connectString;

        // For this project we are using the MySQL clone MariaDB.
        // MariaDB takes considerable less space than MySQL but
        // is 100% compatible.
        // The advantage of MySQL/MariaDB is that it is connected to by
        // TCP/IP and can be used from any folder.
        // The disadvantage is it is rather large making it less portable
        // and only a single instance can be run at a time.
        // For this project the database is stored on the local machine
        // and localhost is used for the URL.
        connectString = "Data Source =" + DBName;

        try
        {
            connection = new SQLiteConnection(connectString);
            connection.Open();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in DBConnect");
        }
    }

    void DBClose()
    {
        if (connection != null) connection.Close();
    }

    /*** Database collection methods ***/

    /***********************************************************************/
    /* Method to get a database View (an array of records where each       */
    /* record is an array of strings). Used to get any View of the         */
    /* database depending on the SQL SELECT Command.                       */
    /*                                                                     */
    /* Input:	A SQL SELECT Command (commandString) to create the View    */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: The View (Array of array of strings)                       */
    /***********************************************************************/
    private string[][] GetData(string commandString)
    {
        string[][] data = null;
        int col;
        int row = 0;
        int records;

        Debug.Print("commandString = " + commandString);

        reader = null;

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            // Make one pass over the data to count the records
            // to know what size to make the array
            records = 0;
            reader = sqlCmd.ExecuteReader();
            while (reader.Read()) records++;

            reader.Close();

            // Allocate the array of records
            data = new string[records][];

            // Make another pass over the data to fill the arrays
            reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                // Allocate each record in the array of records
                data[row] = new string[reader.FieldCount];
                for (col = 0; col < reader.FieldCount; col++)
                    data[row][col] = reader.GetValue(col).ToString();
                row++;
            }
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in GetData");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return data;
    }

    /***********************************************************************/
    /* Method to get the All Drivers View (an array of records where each  */
    /* record is an array of strings). Uses a join of the Drivers and      */
    /* Manufacturers tables.                                               */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The All Drivers View for the Display All Records User      */
    /*          Inerface.                                                  */
    /***********************************************************************/
    public string[][] GetAllFighter()
    {
        string[][] data = null;
        string commandString;

        // Note: This is an inner join so drivers without matching
        //       Manufacturers will not be included. To include all
        //       drivers, even when they do not include a matching
        //       Manufacturer we would need to use a left outer join.
        commandString = "SELECT ID,Rank,First,Last,Age,Height,Weight,Reach,";
        commandString += "Wins,Loss,Draws, Org from Fighter,Organization";
        commandString += " WHERE Fighter.OrgID = Organization.OrgID";
        commandString += " ORDER BY ID";

        //Console.WriteLine(commandString);

        data = GetData(commandString);

        return data;
    }

    /***********************************************************************/
    /* Method to get the Manufacturers View (an array of records where each*/
    /* record is an array of strings).                                     */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The Manufacturers View                                     */
    /***********************************************************************/
    public string[][] GetOrganization()
    {
        string[][] data = null;
        string commandString;

        commandString = "SELECT * FROM Organization";

        data = GetData(commandString);

        return data;
    }

    /*** Database single record methods ***/

    /***********************************************************************/
    /* Method to get a single record (array of strings) from the database. */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The key's value (key)                                      */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: The record.                                                */
    /***********************************************************************/
    public string[] GetARecord(string tableName, string keyName, string key)
    {
        string commandString;
        string[] Record = null;
        int col;

        commandString = "SELECT * FROM " + tableName;
        commandString += " WHERE " + keyName + " = ";
        commandString += "'";
        commandString += key;
        commandString += "'";

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            reader = sqlCmd.ExecuteReader();
            if (reader.Read())
            {
                Record = new string[reader.FieldCount];
                for (col = 0; col < reader.FieldCount; col++)
                    Record[col] = reader.GetValue(col).ToString();
            }
            else
            {
                Record = new string[1];
                Record[0] = "no record found";
            }
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in GetARecord");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return Record;
    }

    /***********************************************************************/
    /* Method to get a single record (array of strings) from the database  */
    /* using global names.                                                 */
    /*                                                                     */
    /* Input:	The primary key's value (key)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: The record.                                                */
    /***********************************************************************/
    public string[] GetRecord(string key)
    {
        return GetARecord(tableName, keyName, key);
    }

    /***********************************************************************/
    /* Method to delete a single record from the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The  key's value (key)                                     */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void DeleteARecord(string tableName, string keyName, string key)
    {
        string commandString;

        commandString = "delete from " + tableName;
        commandString += " where " + keyName + " = ";
        commandString += "'";
        commandString += key;
        commandString += "'";

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in DeleteARecord");
        }
    }

    /***********************************************************************/
    /* Method to delete a single record from the database                  */
    /* using global names.                                                 */
    /*                                                                     */
    /* Input:	The primary key's value (key)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void DeleteRecord(string key)
    {
        DeleteARecord(tableName, keyName, key);
    }

    /***********************************************************************/
    /* Method to determine if a record exists in the database.             */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The key's value (key)                                      */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: True or False.                                             */
    /***********************************************************************/
    public bool RecordExists(string tableName, string keyName, string key)
    {
        string commandString;
        bool Exists = false;

        commandString = "SELECT * FROM " + tableName;
        commandString += " WHERE " + keyName + " = ";
        commandString += "'"; commandString += key;
        commandString += "'";

        Debug.Print("RecordExists commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            reader = sqlCmd.ExecuteReader();
            Exists = reader.Read();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in RecordExists");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return Exists;
    }

    /***********************************************************************/
    /* Method to insert a single record into the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The record (array of strings)                              */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*             An array of Data Types (DataTypes)                      */
    /*             0 -> No quotes, 1 -> Quotes needed                      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    void InsertRecord(string tableName, string[] Record)
    {
        string commandString;
        int i;

        commandString = "INSERT INTO " + tableName + " VALUES (";
        for (i = 0; i < Record.Length; i++)
        {
            if (DataTypes[i] == 1) commandString += "'";
            commandString += Record[i];
            if (DataTypes[i] == 1) commandString += "'";
            if (i < Record.Length - 1) commandString += ",";
        }
        commandString += ")";

        Debug.Print("InsertRecord commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in InsertRecord");
        }
    }

    /***********************************************************************/
    /* Method to update a single record into the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The record (array of strings)                              */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*             An array of Data Types (DataTypes)                      */
    /*             0 -> No quotes, 1 -> Quotes needed                      */
    /*             An array of Field Names (FieldNames)                    */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /*                                                                     */
    /* for simplicity we do not track which fields have changed but update */
    /* all fields                                                          */
    /***********************************************************************/
    public void UpdateRecord(string tableName, string keyName, string[] Record)
    {
        string commandString;
        string key;
        int i;

        key = Record[0];

        commandString = "UPDATE " + tableName + " SET ";
        for (i = 0; i < Record.Length; i++)
        {
            commandString += FieldNames[i];
            commandString += " = '";
            commandString += Record[i];
            commandString += "'";
            if (i < Record.Length - 1) commandString += ",";
        }
        commandString += " WHERE " + keyName + " = '";
        commandString += key;
        commandString += "'";

        Debug.Print("UpdateRecord commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in UpdateRecord");
        }
    }

    /***********************************************************************/
    /* Method to Save a single record to the database.                     */
    /* We assume the first field in the record is the primary key.         */
    /*                                                                     */
    /* Input:	The record (array of strings)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void SaveRecord(string[] Record)
    {
        string key;

        key = Record[0];
        if (RecordExists(tableName, keyName, key))
            UpdateRecord(tableName, keyName, Record);
        else
            InsertRecord(tableName, Record);
    }

    /***********************************************************************/
    /* Method to get a View of the Drivers #, First Name and Last Name     */
    /* (an array of records where each record is an array of strings)      */
    /* ordered by Driver #.                                                */
    /* Used by the GUI new record list box in the dialog to see existing   */
    /* driver #s.                                                          */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The View                                                   */
    /***********************************************************************/
    public string[][] GetCurrentFighter()
    {
        string[][] data = null;
        string commandString;

        commandString = "SELECT ID,Rank,First,Last,Age,Height,Weight,Reach, Wins,Loss,Draws from Fighter";   
        commandString += " ORDER BY ID";

        data = GetData(commandString);

        return data;
    }

    /***********************************************************************/
    /* Method to get a View of the Drivers #, First Name and Last Name     */
    /* (an array of records where each record is an array of strings)      */
    /* ordered by Last Name then First Name.                               */
    /* Used by the GUI open record drop down in the dialog box to pick a   */
    /* driver.                                                             */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The View                                                   */
    /***********************************************************************/
    public string[][] GetFighter()
    {
        string[][] data = null;
        string commandString;

        commandString = "SELECT ID,Rank,First,Last,Age,Height,Weight,Reach,Wins,Loss,Draws from Fighter";
        commandString += " ORDER BY Last, First";

        data = GetData(commandString);

        return data;
    }

    /*** Database Utility methods ***/

    /***********************************************************************/
    /* Method to find the Longest Field Label                              */
    /* Used by the console user interface for dispaly.                     */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*                                                                     */
    /* Returns: Longest Field Label Length                                 */
    /***********************************************************************/
    public static int GetLongestFieldLabel()
    {
        int i;
        int Longest = 0;

        for (i = 0; i < FieldHeads.Length; i++)
            if (FieldHeads[i].Length > Longest)
                Longest = FieldHeads[i].Length;

        return Longest;
    }

    /***********************************************************************/
    /* Method to Find a Value in the support table given the support table */
    /* primary key (from the Master table foreign key).                    */
    /*                                                                     */
    /* Input:	the support table primary key (TheKey)                     */
    /*          the support table data (values)                            */
    /*          the index (location) of the support table                  */
    /*              field (Value) we are looking for (index)               */
    /*                                                                     */
    /* Returns: The Value or "Not Found"                                   */
    /***********************************************************************/
    public static string FindValue(string TheKey, string[][] values, int index)
    {
        int i;
        string key;
        string value;

        for (i = 0; i < values.Length; i++)
        {
            key = values[i][0];
            value = values[i][index];
            if (TheKey == key) return value;
        }

        return "Not Found";
    }

    //Gets Copy table
    public string[][] GetAllCopy()
    {
        string[][] data = null;
        string commandString;

        // Note: This is an inner join so drivers without matching
        //       Manufacturers will not be included. To include all
        //       drivers, even when they do not include a matching
        //       Manufacturer we would need to use a left outer join.
        commandString = "SELECT ID,Rank,First,Last,Age,Height,Weight,Reach,";
        commandString += "Wins,Loss,Draws, Org from Copy,Organization";
        commandString += " WHERE Copy.OrgID = Organization.OrgID";
        commandString += " ORDER BY ID";

        //Console.WriteLine(commandString);

        data = GetData(commandString);

        return data;
    }
    //Save to Copy
    public void SaveCopy(string[] Record)
    {
        InsertRecord("Copy", Record);
    }

    //Gets a Copy Record
    public string[] GetCopy(string key)
    {
        return GetARecord("Copy", keyName, key);
    }

    //Deletes a Copy
    public void DeleteCopy(string key)
    {
        DeleteARecord("Copy", keyName, key);
    }

    //Delets All Copies
    public void DeleteAllCopy(bool delete)
    {
        string commandString;

        commandString = "delete from " + "Copy";

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in DeleteARecord");
        }
    }

    public string[][] GetFighterCopy()
    {
        string[][] data = null;
        string commandString;

        commandString = "SELECT ID,Rank,First,Last,Age,Height,Weight,Reach,Wins,Loss,Draws from Copy";
        commandString += " ORDER BY Last, First";

        data = GetData(commandString);

        return data;
    }
}

