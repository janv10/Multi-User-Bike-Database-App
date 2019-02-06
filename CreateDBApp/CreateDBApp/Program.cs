
//
// Console app to create a database, e.g. BikeHike.
//
// Jahnvi Patel (jpate201)
// U. of Illinois, Chicago
// CS480, Summer 2018
// Project #1
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateDBApp
{
  class Program
  {

    static void Main(string[] args)
    {
      Console.WriteLine();
      Console.WriteLine("** Create Database Console App **");
      Console.WriteLine();

      string baseDatabaseName = "BikeHike";
      string sql;

      try
      {
        //
        // 1. Make a copy of empty MDF file to get us started:
        //
        Console.WriteLine("Copying empty database to {0}.mdf and {0}_log.ldf...", baseDatabaseName);

        CopyEmptyFile("__EmptyDB", baseDatabaseName);

        Console.WriteLine();

        //
        // 2. Now let's make sure we can connect to SQL Server on local machine:
        //
        DataAccessTier.Data data = new DataAccessTier.Data(baseDatabaseName + ".mdf");

        Console.Write("Testing access to database: ");

        if (data.TestConnection())
          Console.WriteLine("success");
        else
          Console.WriteLine("failure?!");

        Console.WriteLine();

        //
        // 3. Create tables by reading from .sql file and executing DDL queries:
        //
        Console.WriteLine("Creating tables by executing {0}.sql file...", baseDatabaseName);

        string[] lines = System.IO.File.ReadAllLines(baseDatabaseName + ".sql");

        sql = "";

        for (int i = 0; i < lines.Length; ++i)
        {
          string next = lines[i];

          if (next.Trim() == "")  // empty line, ignore...
          {
          }
          else if (next.Contains(";"))  // we have found the end of the query:
          {
            sql = sql + next + System.Environment.NewLine;

            Console.WriteLine("** Executing '{0}'...", sql.Substring(0, 32));

            data.ExecuteActionQuery(sql);

            sql = "";  // reset:
          }
          else  // add to existing query:
          {
              sql = sql + next + System.Environment.NewLine;
          }
        }

        Console.WriteLine();

        //
        // 4. Insert data by parsing data from .csv files:
        //
        Console.WriteLine("Inserting data...");

        //
        // DONE
        //
        
        Console.WriteLine("Inserting Bike Types");
        using (var file = new System.IO.StreamReader("BikeTypes.csv"))
                {
                    while (!file.EndOfStream)
                    {
                        //Parse each line from the given file 
                        string line = file.ReadLine();
                        string[] values = line.Split(',');

                        int typeid = Convert.ToInt32(values[0]);
                        string description = values[1];
                        double priceperhour = Convert.ToDouble(values[2]);

                        //Build SQL Query for BikeTypes
                        string bikeTypeSQL = string.Format(@"
                                        set identity_insert Bike_Type ON
                                        Insert into Bike_Type(Bike_Type_ID, Description, Price_hour) 
                                        values({0}, '{1}', {2});
                                        ",
                                        typeid, description, priceperhour);

                        //Execute the SQL query 
                        data.ExecuteActionQuery(bikeTypeSQL);

                        //Check to see format
                        Console.WriteLine(bikeTypeSQL);
                    }
                }

        
        Console.WriteLine("Inserting Bikes");
        using (var file = new System.IO.StreamReader("Bikes.csv"))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        string[] values = line.Split(',');

                        int typeid = Convert.ToInt32(values[0]);
                        int bikeTypeID = Convert.ToInt32(values[1]);
                        int year_service = Convert.ToInt32(values[2]);

                        //Build SQL Query for inserting bikes 
                        string BikesSQL = string.Format(@"
                                        set identity_insert Bike ON
                                        Insert into Bike(BID, Bike_Type_ID, Year_In_Service)
                                        values({0}, {1}, {2});
                                        ",
                                        typeid, bikeTypeID, year_service);

                        //Execute the SQL Query for inserting bikes 
                        data.ExecuteActionQuery(BikesSQL);

                        //Check to see if correct format added 
                        Console.WriteLine(BikesSQL); 
              

                    }
                }

                Console.WriteLine("Inserting Customers"); 
                using(var file = new System.IO.StreamReader("Customers.csv"))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        string[] values = line.Split(',');

                        long typeid = Convert.ToInt64(values[0]);
                        string firstName = values[1];
                        string lastName = values[2];
                        string email = values[3];

                        string customerSQL = string.Format(@"
                                             set identity_insert Customer ON
                                             insert into Customer( CID, First_name, Last_name, email_address)
                                             values({0}, '{1}', '{2}', '{3}'); 
                                             ", typeid, firstName, lastName, email);

                        //Execute the SQL Query 
                        data.ExecuteActionQuery(customerSQL);

                        //Write to Screen to check formatting
                        Console.WriteLine(customerSQL);

                            
                    }
                }

        Console.WriteLine();

        //
        // Done
        //
      }
      catch (Exception ex)
      {
        Console.WriteLine("**Exception: '{0}'", ex.Message);
      }

      Console.WriteLine();
      Console.WriteLine("** Project 1 Complete! Now exiting... **");
      Console.WriteLine();
    }//Main


    /// <summary>
    /// Makes a copy of an existing Microsoft SQL Server database file 
    /// and log file.  Throws an exception if an error occurs, otherwise
    /// returns normally upon successful copying.  Assumes files are in
    /// sub-folder bin\Debug or bin\Release --- i.e. same folder as .exe.
    /// </summary>
    /// <param name="basenameFrom">base file name to copy from</param>
    /// <param name="basenameTo">base file name to copy to</param>
    static void CopyEmptyFile(string basenameFrom, string basenameTo)
    {
      string from_file, to_file;

      //
      // copy .mdf:
      //
      from_file = basenameFrom + ".mdf";
      to_file = basenameTo + ".mdf";

      if (System.IO.File.Exists(to_file))
      {
        System.IO.File.Delete(to_file);
      }

      System.IO.File.Copy(from_file, to_file);

      // 
      // now copy .ldf:
      //
      from_file = basenameFrom + "_log.ldf";
      to_file = basenameTo + "_log.ldf";

      if (System.IO.File.Exists(to_file))
      {
        System.IO.File.Delete(to_file);
      }

      System.IO.File.Copy(from_file, to_file);
    }

  }//class
}//namespace

