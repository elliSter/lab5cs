using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.OleDb;
using System.IO;

namespace AccessingData
{
    public partial class WorkWithData : Form
    {
        public WorkWithData()
        {
            InitializeComponent();
            ConboBoxEmployees();


        }

        private void PrepareDemo(bool ShowGrid)
        {
            PrepareDemo(ShowGrid, pgeListBox);
        }

        private void PrepareDemo(bool ShowGrid, TabPage SelectedPage)
        {
            if (demoList.DataSource == null)
            {
                demoList.Items.Clear();
            }
            else
            {
                demoList.DataSource = null;
                demoList.DisplayMember = "";
            }

            demoGrid.Visible = ShowGrid;
            tabDemo.SelectedTab = SelectedPage;
        }


        private void sqlDataReaderButton_Click(System.Object sender, System.EventArgs e)
        {
            PrepareDemo(false);
            DataReaderFromOleDB();
        }
        private void DataReaderFromOleDB()
        {
            string strSQL = "SELECT * FROM Customers";

            try
            {

                using (OleDbConnection cnn = new OleDbConnection(Properties.Settings.Default.OleDbConnectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(strSQL, cnn))
                    {
                        cnn.Open();

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            // Loop through all the rows, retrieving the 
                            // columns you need. Also look into the GetString
                            // method (and other Get... methods) for a faster 
                            // way to retrieve individual columns.
                           
                            while (dr.Read())
                            {
                                demoList.Items.Add(string.Format("{0} {1}: {2}", dr["CompanyName"], dr["ContactName"], dr["ContactTitle"]));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void oleDbDataSetButton_Click(System.Object sender, System.EventArgs e)
        {
            PrepareDemo(true);
            DataSetFromOleDb();
        }
        private void DataSetFromOleDb()
        {
            string strSQL = "SELECT * FROM Products WHERE CategoryID=1";

            try
            {
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, Properties.Settings.Default.OleDbConnectionString))
                {

                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "ProductInfo");


                    demoList.DataSource = ds.Tables["ProductInfo"];
                    demoList.DisplayMember = "ProductID";

                    // Εναλλάκτικά θα μπορούσαμε να γεμίσουμε το Listbox
                    // παίρνοντας μία μία τις γραμμές του πίνακα και προσθέτοντάς τες

                    //For Each dr As DataRow In _
                    // ds.Tables("ProductInfo").Rows
                    //    demoList.Items.Add(dr("ProductName").ToString)
                    //Next dr

                    // Want to bind a grid? It's this easy:
                    demoGrid.DataSource = ds.Tables["ProductInfo"];
                    demoGrid.Columns[0].Visible = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void sqlDataSetButton_Click(System.Object sender, System.EventArgs e)
        {
            PrepareDemo(true);
            DataTableFromSQL();
        }
        private void DataTableFromSQL()
        {
            string strSQL = "SELECT * FROM Employees WHERE City='London' ";

            try
            {
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, Properties.Settings.Default.OleDbConnectionString))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    foreach (DataColumn dc in dt.Columns)
                    {
                        demoList.Items.Add(string.Format("{0} ({1})", dc.ColumnName, dc.DataType));
                    }

                    demoGrid.DataSource = dt;
                    /*foreach (DataGridViewColumn col in demoGrid.Columns)
                    {
                        if (col.Name == "London")
                        {
                            demoGrid.Columns[8].Visible = true;
                        }
                    } */

                    string data = string.Empty;

                   /* foreach (DataGridViewRow row in demoGrid.Rows)
                    {
                        data = Convert.ToString(row.Cells[8].Value);
                        if (data == "London")
                        {
                            demoGrid.Columns[8].Visible = true;
                        }
                    } */
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void dataTableButton_Click(System.Object sender, System.EventArgs e)
        {
            PrepareDemo(true, pgeGrid);
            CreateDataTable();
        }
        private void CreateDataTable()
        {
            // Create a DataTable filled with information
            // about files in the current folder.
            // 
            // Note the use of the FileInfo and 
            // DirectoryInfo objects, provided by the 
            // .NET framework, in the System.IO namespace.

            DataTable dt = new DataTable();
            dt.Columns.Add("FileName", typeof(System.String));
            dt.Columns.Add("Size", typeof(System.Int64));
            dt.Columns.Add("ReadOnly", typeof(System.Boolean));

            DataRow dr = default(DataRow);
            DirectoryInfo dir = new DirectoryInfo("C:\\");
            foreach (FileInfo fi in dir.GetFiles())
            {
                dr = dt.NewRow();
                dr[0] = fi.Name;
                dr[1] = fi.Length;
                dr[2] = fi.IsReadOnly;
                dt.Rows.Add(dr);
            }

            // Bind the DataGridView to this DataTable.
            demoGrid.DataSource = dt;
        }

        private void ConboBoxEmployees()
        {

            string strSQL = "SELECT * FROM Employees ";

            try
            {

                using (OleDbConnection cnn = new OleDbConnection(Properties.Settings.Default.OleDbConnectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(strSQL, cnn))
                    {
                        cnn.Open();

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            
                            while (dr.Read())
                            {
                                comboBox1.Items.Add(string.Format("{0} ", dr["FirstName"]));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string strSQL = "SELECT * FROM Employees where FirstName='"+comboBox1.Text+"';";
            string strSQL2 = "SELECT * FROM Orders where EmployeeID='" + comboBox1.Text + "';";
            int id,orders=0;

            try
            {

                using (OleDbConnection cnn = new OleDbConnection(Properties.Settings.Default.OleDbConnectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(strSQL, cnn))
                    {
                        cnn.Open();

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {

                            while (dr.Read())
                            {
                                id = Convert.ToInt32(dr["EmployeeID"]);
                                label2.Text = ""+id;
                                label2.Show();
                                orders++;

                                //orders = Convert.ToInt32(dr["EmployeeID"]);
                                label4.Text = "" + orders;
                                label4.Show();


                            }
                        }
                    } //cnn.Close();
                    /*using (OleDbCommand cmd = new OleDbCommand(strSQL2, cnn))
                    {
                        cnn.Open();

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {

                            while (dr.Read())
                            {
                                orders = Convert.ToInt32(dr["EmployeeID"]);
                                label4.Text = "" + orders;
                                label4.Show();

                            }
                        }
                    } */


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }
    }
} 
