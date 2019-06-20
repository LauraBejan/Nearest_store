using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Device.Location ;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace distance_app
{
    public partial class Form1 : Form
    {
        // public string store_address="vaslui";
        public int page_number = 0;
        public Form1()
        {
            InitializeComponent();
            label1.Hide();
            label2.Hide();
            listView1.Hide();
            button3.Hide();
            button3.Text = "Previous";
            button4.Hide();
            button4.Text = "Next";
        }

        public int getMaxIdClient(Connection connection)
        {

            if (connection._connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            var command = new Oracle.ManagedDataAccess.Client.OracleCommand("get_total_price1", connection._connection);

            string city = textBox1.Text;
            command.CommandText = "select max(clientID) from addressC";
            command.CommandType = CommandType.Text;
            OracleDataReader dr = command.ExecuteReader();
            int index = 0;
            if (dr.HasRows)
                while (dr.Read())
                    index = dr.GetInt32(0);
            int ok = index;
            connection.Close();
            return index;
        }

        public List<Address> getCityStore(Connection connection)
        {
            try
            {
                if (connection._connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();

                var command = new OracleCommand("getCities", connection._connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                List<Address> addressList = new List<Address>();
                OracleParameter output = command.Parameters.Add("l_cursor", OracleDbType.RefCursor);
                output.Direction = System.Data.ParameterDirection.ReturnValue;
                command.ExecuteNonQuery();

                Oracle.ManagedDataAccess.Client.OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Address address = new Address(
                        reader.GetInt32(0),
                        reader.GetString(1));
                    addressList.Add(address);
                }
                connection.Close();
                return addressList;
                //  return null;
            }
            catch (Exception e)
            {
                connection.Close();
                throw;

            }


        }

        public List<Address> getCityStore1(Connection connection, int page)
        {
            try
            {
                if (connection._connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();

                var command = new OracleCommand("getCities1", connection._connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                List<Address> addressList = new List<Address>();

                command.Parameters.Add("page", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = page;
                Oracle.ManagedDataAccess.Client.OracleParameter p_rc = command.Parameters.Add("l_cursor", OracleDbType.RefCursor,
                                       DBNull.Value,
                                       System.Data.ParameterDirection.Output);
                // Oracle.ManagedDataAccess.Client.OracleParameter output = command.Parameters.Add("rc", OracleDbType.RefCursor);
                // output.Direction = System.Data.ParameterDirection.ReturnValue;
                command.ExecuteNonQuery();

                Oracle.ManagedDataAccess.Client.OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Address address = new Address(
                        reader.GetInt32(0),
                        reader.GetString(1));
                    addressList.Add(address);
                }
                connection.Close();
                return addressList;
                //  return null;
            }
            catch (Exception e)
            {
                connection.Close();
                throw;

            }


        }
        public void insert_new_address_client(Connection connection,int id_client_max, string city)
        {
            if (connection._connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            var command = new Oracle.ManagedDataAccess.Client.OracleCommand("get_total_price1", connection._connection);

            command.CommandText = "insert into addressC VALUES(:id_c,:city)";
            command.Parameters.Add(new OracleParameter("id_c", id_client_max));
            command.Parameters.Add(new OracleParameter("city", city));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void insert_into_common_table(Connection connection ,int id_store, int id_client,int dist)
        {
            try
            {
                if (connection._connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                var command = new Oracle.ManagedDataAccess.Client.OracleCommand("insert_into_distance_table", connection._connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("store_id", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = id_store;
                command.Parameters.Add("client_id", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = id_client;
                command.Parameters.Add("distance", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = dist;

                command.ExecuteNonQuery();

                Oracle.ManagedDataAccess.Client.OracleDataReader reader = command.ExecuteReader();
                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                throw;

            }

        }

        //public void insert_distances(Connection connection,int city_id, int user_id, int distance)
        //{
        //    try
        //    {
        //        if (connection._connection.State == System.Data.ConnectionState.Closed)
        //            connection.Open();
        //        var command = new Oracle.ManagedDataAccess.Client.OracleCommand("insert_into_distance_user", connection._connection);
        //        command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.Parameters.Add("store_id", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = id_store;
        //        command.Parameters.Add("client_id", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = id_client;
        //        command.Parameters.Add("distance", OracleDbType.Int32, System.Data.ParameterDirection.Input).Value = dist;

        //        command.ExecuteNonQuery();

        //        Oracle.ManagedDataAccess.Client.OracleDataReader reader = command.ExecuteReader();
        //        connection.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        connection.Close();
        //        throw;

        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            int dist=get_distance(textBox1.Text);

             Connection connection = new Connection();

            //  int id_client_max = getMaxIdClient(connection) + 1;

            ////  insert_distances(connection,textBox1.Text);
            ////  int id_client_max = getmaxidclient(connection) + 1;
            //  insert_new_address_client(connection, id_client_max, textBox1.Text);


            //  List<Address> storeList = getCityStore(connection);
            //  for (int i = 0; i < storeList.Count(); i++)
            //  {
            //      int distance = get_distance(storeList[i].address);
            //      insert_distances(connection, storeList[i].id, id_client_max, distance);

            //  }





            int id_client_max = getMaxIdClient(connection) + 1;
            insert_new_address_client(connection, id_client_max, textBox1.Text);

            //GET MIN DISTANCE
            List<Address> storeList = getCityStore(connection);
            // int min = storeList[0].id;
            int index = 0;
            int min = -1;
            for (int i = 0; i < storeList.Count(); i++)
            {
                if (min == -1)
                {
                    min = get_distance(storeList[i].address);
                    index = i;
                }
                else
                {
                    int pos_min = get_distance(storeList[i].address);
                    if (pos_min < min)
                    {
                        min = pos_min;
                        index = i;
                    }
                }
            }
            label1.Show();
            label1.Text = "The nearest shop is in " + storeList[index].address + " at " + min + " away";
            // int min = get_distance("vaslui");
            insert_into_common_table(connection, storeList[index].id, id_client_max, dist);
            //insert_into_common_table(connection ,2, id_client_max, dist);
            //  insert_into_common_table(connection ,2, 26, 500);
        }
        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        public int get_distance(string store_address,string unit=" ")
        {
            string customer_address = textBox1.Text;
            // Google API key
            string apiKey = "AIzaSyDWZKU5p29xB5pRNph04bsufuzxuaGDMKg";

            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Headers.Add("key", apiKey);
            //string result = client.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?address=" + customer_address);
            string url1 = client.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?address="+customer_address+ store_address+"CA&key=" +apiKey);
            string url2 = client.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?address="+store_address+ customer_address+"CA&key=" +apiKey);
            var url3 = url1;

            string pattern_lng= "([0-9]+[.]+[0-9]+)";
            Regex rx = new Regex(pattern_lng);
            MatchCollection matches = rx.Matches(url1);

            double latTo=0;
            double lngTo =0;
            foreach (Match match in matches)
            {
                if (latTo == 0)
                    latTo = double.Parse(match.ToString());
                else
                {
                    lngTo = double.Parse(match.ToString());
                    break;
                }
            }

            double latFrom = 0;
            double lngFrom = 0;
             matches = rx.Matches(url2);
            foreach (Match match in matches)
            {
                if (latFrom == 0)
                    latFrom = double.Parse(match.ToString());
                else
                {
                    lngFrom = double.Parse(match.ToString());
                    break;
                }
            }

            var sCoord = new GeoCoordinate(latFrom, lngFrom);
            var eCoord = new GeoCoordinate(latTo, lngTo);

            double distance1 = sCoord.GetDistanceTo(eCoord)%1000;
            int distance = Convert.ToInt32(distance1);

            
            return distance;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connection connection = new Connection();
            List<Address> addressList = getCityStore1(connection,0);

            listView1.Show();
            listView1.Clear();
            button4.Show();

            listView1.Columns.Add("Store");
            foreach (var address in addressList)
            {

                //display new item in cart
                ListViewItem item = new ListViewItem(address.address);

                listView1.Items.Add(item);
                listView1.Refresh();

            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedtItem = listView1.SelectedItems[0].Text;

            int dist = get_distance(selectedtItem);
            label2.Show();
            label2.Text = "The shop is " + dist + " km away";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Connection connection = new Connection();
            page_number += 2;
            List<Address> addressList = getCityStore1(connection, page_number);

            listView1.Show();
            listView1.Clear();

            listView1.Columns.Add("Store");
            foreach (var address in addressList)
            {

                //display new item in cart
                ListViewItem item = new ListViewItem(address.address);

                listView1.Items.Add(item);
                listView1.Refresh();

            }
            button3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Connection connection = new Connection();
            page_number -= 2;
            List<Address> addressList = getCityStore1(connection, page_number);

            listView1.Show();
            listView1.Clear();

            listView1.Columns.Add("Store");
            foreach (var address in addressList)
            {

                //display new item in cart
                ListViewItem item = new ListViewItem(address.address);

                listView1.Items.Add(item);
                listView1.Refresh();

            }
            if (page_number == 0)
                button3.Hide();
        }
    }
}
