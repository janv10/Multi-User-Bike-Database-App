//
// Multi-user BikeHike Windows App, using transactions
//
// JAHNVI PATEL
// U. of Illinois, Chicago
// CS480, Summer 2018
// Project #3
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; 

namespace BikeHike
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /**
         * TEST CONNECTION 
         * Test Connection Button: Tests the connection to the database
         */ 
        private void button1_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db;

            //Specifiy a file name to be opened
            filename = "BikeHike.mdf";

            //Address of the where the filename can be found 
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

            db = new SqlConnection(connectionInfo);
            db.Open();

            //Print the status of the connection to the screen
            string msg = db.State.ToString();     
            MessageBox.Show("Filename: " + filename + "\n" + "Status: " + msg); 

            //Close the connection
            db.Close();

        }//end Test Connection Button


        /**
         * DISPLAY ALL CUSTOMERS
        * Show all customers
        * in alphabetical order by last name
        * No transaction needed as it only displays the customers 
        */
        private void button4_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db;

            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);
            db = new SqlConnection(connectionInfo);

            //Open a connection
            db.Open();

            string sql = string.Format(@"SELECT First_name, Last_name 
                                        FROM customer
                                        ORDER BY Last_name, First_name ASC;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);

            db.Close();

            this.listBox1.Items.Clear();

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string msg = string.Format("{0}, {1}",
                    Convert.ToString(row["First_name"]),
                    Convert.ToString(row["Last_name"]));

                this.listBox1.Items.Add(msg);
            }

        }//end display all customers

        /**
         * LOCATE CUSTOMER
         * Customer lookup: display CID, email address and out with a rental or not
         * if out with a rental: display the number of bikes and expected return date and time 
         */
        private void button3_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db;

            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

            db = new SqlConnection(connectionInfo);
            db.Open();

            //Convert the value read from the textbox to an int 
            int customer_id = Convert.ToInt32(this.textBox1.Text);

            //Retrieving Customer Name and Email 
            string sql = string.Format(@"SELECT CID, email_address FROM Customer 
                                        WHERE CID = {0};", customer_id);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);

            this.listBox1.Items.Clear();

            foreach (DataRow row in ds.Tables["TABLE"].Rows)   //Display to the screen
            {
                string msg = string.Format("Customer ID: {0}, Email: '{1}'",
                    Convert.ToInt32(row["CID"]),
                    Convert.ToString(row["email_address"])
                    );
                this.listBox1.Items.Add(msg);
            }
         
            //Retreiving num of bikes rented  
            string sql2 = string.Format(@"SELECT COUNT(*) as NumCount FROM Rent 
                                          WHERE CID = {0};", customer_id);

            //MessageBox.Show(sql2); //For debugging

            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = db;
            cmd2.CommandText = sql2;

            object result = cmd2.ExecuteScalar(); 
            
             if (result == null)
            {
                MessageBox.Show("No Rented bikes found");
            }
            if (result == DBNull.Value)
            {
                MessageBox.Show("Count is null");
            }
            else
            {
                string count = Convert.ToString(result);
                this.listBox1.Items.Add("Number of bikes rented: " + count);
            }


            //Retrieving expected return time 
            string sql3 = string.Format(@"--Indexed CID, reduced one scan 
SELECT Expected_Return_Time FROM rent with (index(rent_cid))
                                        where cid = {0};", customer_id);

            //MessageBox.Show(sql3); //For debugging

            SqlCommand cmd3 = new SqlCommand();
            cmd3.Connection = db;
            cmd3.CommandText = sql3;

            object result3 = cmd3.ExecuteScalar();

            if (result3 == null)
            {
                this.listBox1.Items.Add("Rented: No");
                this.listBox1.Items.Add("Expected Return: N/A");
               
            }
            if (result3 == DBNull.Value)
            {
                MessageBox.Show("Error: Time is null");
            }
            else
            {
                string time = Convert.ToString(result3);
                if (time == ""){}
                else
                {
                    this.listBox1.Items.Add("Rented: Yes");
                    this.listBox1.Items.Add("Expected Return: " + time);
                }
            }
            db.Close();


        }//End customer lookup


        /**
         * DIPLAY ALL BIKES
         * Display a list of all bikes
         */
        private void button2_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db;

            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

            db = new SqlConnection(connectionInfo);
            db.Open();

            //SQL Query to retrive all the bikes 
            string sql = string.Format(@"SELECT * FROM Bike ORDER by BID ASC;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds); 
            db.Close();

            this.listBox1.Items.Clear();

            //Display the results to the listBox1
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string msg = string.Format("BID: {0}, Type: {1}, Year in Service: {2}",
                    Convert.ToInt32(row["BID"]),
                    Convert.ToInt32(row["Bike_Type_ID"]),
                    Convert.ToInt32(row["Year_In_Service"]));
                this.listBox1.Items.Add(msg);

            }
        }//end display all bikes 


        /**
         * LOCATE BIKE
         * Find a bike from the list, display the bike's year and type (description)
         * and the rental price per hour
         */ 
        private void button5_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db;

            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

            db = new SqlConnection(connectionInfo);
            db.Open();

            int bikeID = Convert.ToInt32(this.textBox2.Text);       //Convert string to int 
           
            //SQL query to retrive the bike id, year in service, description and price per hour info
            string sql = string.Format(@"SELECT BID, Year_In_Service, Description, Price_hour 
                                         FROM bike
                                         INNER JOIN Bike_Type ON Bike.Bike_Type_ID = Bike_Type.Bike_Type_ID 
                                         WHERE BID = {0};", bikeID);

            //MessageBox.Show(sql); //for debugging
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);

            this.listBox1.Items.Clear();

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string bike = string.Format(@"BID: {0}", Convert.ToInt32(row["BID"]));
                string year = string.Format(@"Year in Service: {0} ", Convert.ToString(row["Year_In_Service"]));
                string description = string.Format(@"Description: '{0}'  ", Convert.ToString(row["Description"]));
                string rental_price = string.Format(@"Rental Price: {0}", Convert.ToDouble(row["Price_hour"]));
               
                this.listBox1.Items.Add(bike);
                this.listBox1.Items.Add(year);
                this.listBox1.Items.Add(description);
                this.listBox1.Items.Add(rental_price);
            }

            //SQL query for expected return time 
            string sql2 = string.Format(@"SELECT Expected_Return_Time FROM rent
										 with (index(rent_bid))
                                            where BID = {0};", bikeID);

            //MessageBox.Show(sql2); //For debugging

            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = db;
            cmd2.CommandText = sql2;

            object result = cmd2.ExecuteScalar();

            //Close the database
            db.Close();

            if (result == null)
            {
                this.listBox1.Items.Add("Rented: No");
                this.listBox1.Items.Add("Expected Return: N/A");
            }
            if (result == DBNull.Value)
            {
                MessageBox.Show("Error: Time is null");
            }
            else   //Bike is rented 
            {
                string count = Convert.ToString(result);
                if (count == "") { }
                else
                {
                    this.listBox1.Items.Add("Rented: Yes");
                    this.listBox1.Items.Add("Expected Return: " + count);
                }
            }
        }//End Bike lookup 


        /**
         * SHOW RENTABLE BIKES
         * Display a list of bikes avaialble for rent by type
         * same type order by newest bikes
         */
        private void button6_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db = null;
            DataSet ds = null;

            SqlTransaction tx = null;

            try
            {
                filename = "BikeHike.mdf";
                connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

                db = new SqlConnection(connectionInfo);
                db.Open();
                tx = db.BeginTransaction(); 

                string sql = string.Format(@"SELECT bike.Bike_Type_ID, Bike.Year_In_Service, Bike.BID from Bike
                                        LEFT JOIN Rent 
                                        ON Bike.BID = Rent.bid
                                        WHERE Rent.bid is NULL OR Rent.Bike_Status = 0
                                        ORDER BY Bike.Bike_Type_ID ASC, Bike.Year_In_Service DESC;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandText = sql;
                cmd.Transaction = tx;
                adapter.Fill(ds);

                tx.Commit(); 
            }//try 

            catch (Exception ex)
            {
                if (tx != null)
                {
                    tx.Rollback();
                    MessageBox.Show(ex.Message); 
                }
            }//catch

            finally
            {
                this.listBox1.Items.Clear();

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    string msg = string.Format(@"Type: {0}, Year in Service: {1}, BID: {2}",
                                  Convert.ToString(row["Bike_Type_ID"]),
                                  Convert.ToString(row["Year_In_Service"]),
                                  Convert.ToString(row["BID"])
                                  );
                    this.listBox1.Items.Add(msg);
                }

                db.Close(); 
            }//finally
           
        }//end display available to rent bikes 

        /**
         * RENTING A BIKE
         * rent button 
         * transaction 
         * DONE
         */
        private void button7_Click(object sender, EventArgs e)
        {
            RentalCart cartObj = new RentalCart(this.textBox3.Text, this.textBox4.Text, this.textBox5.Text); 
     
            string filename, connectionInfo;
            SqlConnection db = null;
            SqlTransaction tx = null;
            int rowsModified;
            int retry = 0;

            while (retry < 3)
            {
                try
                {
                    //Specifiy a file name to be opened
                    filename = "BikeHike.mdf";

                    //Address of the where the filename can be found 
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

                    //Establish a connection
                    db = new SqlConnection(connectionInfo);

                    //Open a connection
                    db.Open();

                    tx = db.BeginTransaction(IsolationLevel.Serializable);

                    string sqla = string.Format(@"SELECT * FROM customer
                                        LEFT JOIN rent 
                                        ON Customer.CID = Rent.CID
                                        WHERE Rent.RID is NULL AND Customer.CID = {0};
                                        ", cartObj.customerID);

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sqla;
                    cmd.Transaction = tx;

                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        //Check if the customer is out with a bike
                        string sql2 = string.Format(@"SELECT * from Customer
                                              RIGHT JOIN rent ON Customer.CID = Rent.CID
                                              WHERE Customer.CID = {0};", cartObj.customerID);

                        SqlCommand cmd2 = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql2;

                        object result2 = cmd.ExecuteScalar();


                        if (result2 == null)
                        {
                            MessageBox.Show("Error: Customer not found.");
                            return;
                        }
                    }

                    else if (result == DBNull.Value)
                    {
                        MessageBox.Show("Error: Customer ID is NULL");
                        return;
                    }


                    string sqlb = string.Format(@"SELECT * FROM Bike
                                        LEFT JOIN Rent 
                                        ON Bike.BID = Rent.BID
                                        WHERE Rent.RID is NULL AND Bike.BID = {0};
                                        ", cartObj.bikeID);


                    cmd.Connection = db;
                    cmd.CommandText = sqlb;

                    object resultb = cmd.ExecuteScalar();

                    if (resultb == null)
                    {
                        //SQL Query to check if the bike is out with a customer 
                        string sql2 = string.Format(@"SELECT * FROM Bike
                                            RIGHT JOIN Rent on Bike.BID = Rent.BID
                                            WHERE Bike.BID = {0};", cartObj.bikeID);

                        SqlCommand cmd2 = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql2;

                        object result2 = cmd.ExecuteScalar();


                        if (result2 == null)
                        {
                            MessageBox.Show("Error: Bike not found.");
                            tx.Rollback();
                            db.Close();
                            return;
                        }
                        else if (result2 == DBNull.Value)
                        {
                            MessageBox.Show("Error: Bike ID is NULL");
                            tx.Rollback();
                            db.Close();
                            return;
                        }
                        else
                        {
                            string bid = Convert.ToString(result2);
                            MessageBox.Show("Bike " + bid + " out with a Customer!");
                            tx.Rollback();
                            db.Close();
                            return;
                        }
                    }

                    else if (resultb == DBNull.Value)
                    {
                        MessageBox.Show("Error: Bike ID is NULL");
                        tx.Rollback();
                        db.Close();
                        return;
                    }


                    //SQL Query to insert the given data to the database
                    string sql = string.Format(@"INSERT INTO Rent (BID, CID, Borrowed_Time, Expected_Return_Time, Bike_Status)
Values({0}, {1}, GETDATE(), DATEADD(hour, {2}, GETDATE()), 1)", Convert.ToInt32(cartObj.bikeID), Convert.ToInt32(cartObj.customerID), Convert.ToInt32(cartObj.hours));


                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    rowsModified = cmd.ExecuteNonQuery();

                    int timeinMS = Convert.ToInt32(textBox7.Text);
                    System.Threading.Thread.Sleep(timeinMS);

                    tx.Commit();


                    if (rowsModified == 1)
                    {
                        MessageBox.Show("Success!");

                        db.Close();
                        return;
                    }

                    else
                    {
                        MessageBox.Show("Error: Update failed");
                        tx.Rollback();
                        db.Close();
                        return;
                    }
                }

                catch (SqlException ex)
                {
                    if (ex.Number == 1205) //Handle deadlock
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }

                    else
                    {
                        retry = 3;
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {

                        MessageBox.Show(ex.Message);
                        tx.Rollback();
                        db.Close();
                        return;
                    }
                }//catch 

                finally
                {
                    db.Close(); 
                }//finally 
            }//while

        }//end requirement 6

        /**
         * RETURNING A RENTAL
         * Return button
         * transaction added 
         */
        private void button10_Click(object sender, EventArgs e)
        {
            string filename, connectionInfo;
            SqlConnection db = null;
            SqlTransaction tx = null;
            int retry = 0;

            while (retry < 3)
            {
                try
                {
                    filename = "BikeHike.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);

                    db = new SqlConnection(connectionInfo);
                    db.Open();

                    tx = db.BeginTransaction(IsolationLevel.Serializable);

                    //SQL Query to update the database to say that the bike is returned 
                    string sql = string.Format(@"UPDATE rent 
	                                     SET Actual_Return_Time = GETDATE(),
	                                     Bike_status = 0
	                                     WHERE RID = {0} ", Convert.ToInt32(this.textBox6.Text));

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    string sql2 = string.Format(@"SELECT actual_return_time FROM rent WHERE RID = {0};", Convert.ToInt32(this.textBox6.Text));

                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.Connection = db;
                    cmd2.Transaction = tx;
                    cmd2.CommandText = sql2;

                    string sql3 = string.Format(@"SELECT borrowed_time FROM rent WHERE RID = {0}; ", Convert.ToInt32(this.textBox6.Text));

                    SqlCommand cmd3 = new SqlCommand();
                    cmd3.Connection = db;
                    cmd3.Transaction = tx;
                    cmd3.CommandText = sql3;

                    //Actual time 
                    object result2 = cmd2.ExecuteScalar();
                    DateTime timeA = Convert.ToDateTime(result2);

                    //Borrowed time
                    object result3 = cmd3.ExecuteScalar();
                    DateTime timeB = Convert.ToDateTime(result3);
                    TimeSpan span = timeA.Subtract(timeB);
                    double totHours = span.TotalHours;

                    //sql command to get price of the bike
                    string sql4 = string.Format(@"SELECT DISTINCT Price_hour FROM Rent 
                                        INNER JOIN Bike ON Rent.BID = Bike.BID
                                        INNER JOIN Bike_Type ON Bike.Bike_Type_ID = Bike_Type.Bike_Type_ID
                                        WHERE Rent.RID = {0};", this.textBox6.Text);

                    SqlCommand cmd4 = new SqlCommand();
                    cmd4.Connection = db;
                    cmd4.Transaction = tx;
                    cmd4.CommandText = sql4;

                    object result4 = cmd4.ExecuteScalar();
                    double price = Convert.ToDouble(result4);
                    double charge = totHours * price;
                    charge = System.Math.Round(charge, 2); 

                    if (rowsModified == 1)
                    {
                        MessageBox.Show("Success! Thank you for returning the Bike.");
                        MessageBox.Show("Your charge for Rental ID " + this.textBox6.Text + " is: $" + Convert.ToString(charge));
                    }

                    else
                    {
                        MessageBox.Show("Error: Update failed");
                    }
                    int timeinMS = Convert.ToInt32(textBox7.Text);
                    System.Threading.Thread.Sleep(timeinMS);
                    tx.Commit();
                    retry = 3;

                }//try 
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) //Handle deadlock
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }

                    else
                    {
                        retry = 3;
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                        MessageBox.Show(ex.Message);
                    }

                    retry = 3;
                }//catch 

                finally
                {
                    db.Close();
                }//finally
            }
   
}//End of Requirement 7

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }
 
        /**
         * RESET DATABASE
         */ 
        private void button11_Click(object sender, EventArgs e)
        {
            SqlTransaction tx = null;
            string filename, connectionInfo;
            SqlConnection db = null;
            int retry = 0;

            while (retry < 3) {
                try
                {
                    this.textBox3.Clear();
                    this.textBox4.Clear();
                    this.textBox5.Clear();
                    this.textBox6.Clear(); 
                    filename = "BikeHike.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;  AttachDbFilename=|DataDirectory|\{0};
                                           Integrated Security=True;", filename);
                    db = new SqlConnection(connectionInfo);

                    //Open a connection
                    db.Open();

                    tx = db.BeginTransaction(IsolationLevel.Serializable);

                    string sql = string.Format(@"Truncate table rent");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    adapter.Fill(ds);

                    int timeinMS = Convert.ToInt32(textBox7.Text);
                    System.Threading.Thread.Sleep(timeinMS); 
                
                    tx.Commit();
                    retry = 3; 
                }//try 

                catch (SqlException ex)
                {
                    if (ex.Number == 1205) //Handle deadlock
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++; 
                    }
                  
                    else
                    {
                        retry = 3;
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                        MessageBox.Show(ex.Message);
                    }

                    retry = 3;

                }//catch

                finally
                {
                    db.Close();
                    MessageBox.Show("Success! Database has been reset.");
                }//finally
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
