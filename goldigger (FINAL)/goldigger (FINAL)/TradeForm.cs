using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace GD_TRADING_APP_beta_
{

    public partial class TradeForm : Form
    {


        private Series candlestickSeries;
        private int currentIndex = 0;
        private const int MaxCandlestickHeight = 100; // Adjust this value as needed
        private MySqlConnection connection;

        public TradeForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeChart();
            PopulateComboBox();

            comboBox1.SelectedIndex = 0;  // Set default quantity to the first item
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            decimal existingBalance = GetCurrentBalance();
            label100.Text = $"Existing Balance: {existingBalance.ToString("F2")} PHP";

        }
        int userID = UserManager.Instance.CurrentUserID;

        private void InitializeChart()
        {
            // Assuming you already have a Chart control named chart1 on your form
            chart1.Series.Clear(); // Clear existing series (if any)

            candlestickSeries = new Series("Candlestick");
            candlestickSeries.ChartType = SeriesChartType.Candlestick;
            candlestickSeries["OpenCloseStyle"] = "Triangle"; // Adjust style as needed
            candlestickSeries["ShowOpenClose"] = "Both"; // Show both open and close prices

            chart1.Series.Add(candlestickSeries);

            // Set the axis range to accommodate the candlesticks
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 5; // Adjust this value based on your data
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 150; // Adjust this value based on your data



            // Add the initial candlestick
            AddCandlestick();
        }
        private void InitializeDatabase()
        {
            // Establish a connection to the database
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
        }
        private void AddCandlestick()
        {
            double open = 0;
            double close = 0;
            double high = 0;
            double low = 0;

            DataPoint dataPoint = new DataPoint();
            dataPoint.XValue = currentIndex++;
            dataPoint.YValues = new double[] { high, low, open, close };

            candlestickSeries.Points.Add(dataPoint);
        }

        private void MoveCandlestickUp()
        {
            // Update the close value to simulate moving up
            double newClose = candlestickSeries.Points[currentIndex - 1].YValues[2] + 10; // Adjust the increment as needed
            candlestickSeries.Points[currentIndex - 1].YValues[2] = Math.Min(newClose, MaxCandlestickHeight);

            // Update the chart
            chart1.Invalidate();

            // Check if the current candlestick is full
            if (candlestickSeries.Points[currentIndex - 1].YValues[2] >= MaxCandlestickHeight)
            {
                // Move to the next candlestick if available
                if (currentIndex < chart1.Series[0].Points.Count)
                {
                    currentIndex++;
                }
                else
                {
                    // Add a new candlestick if the next index is within the maximum X-axis value
                    if (currentIndex < chart1.ChartAreas[0].AxisX.Maximum)
                    {
                        AddCandlestick();
                    }
                }
            }
        }

        private void MoveCandlestickDown()
        {
            // Update the close value to simulate moving down
            double newClose = candlestickSeries.Points[currentIndex - 1].YValues[2] - 10; // Adjust the decrement as needed
            candlestickSeries.Points[currentIndex - 1].YValues[2] = Math.Max(newClose, 0);

            // Update the chart
            chart1.Invalidate();

            // Check if the current candlestick is empty
            if (candlestickSeries.Points[currentIndex - 1].YValues[2] <= 0)
            {
                // Move to the previous candlestick if available
                if (currentIndex > 1)
                {
                    currentIndex--;
                }
            }
        }
        private decimal GetCurrentBalance()
        {

            decimal currentBalance = 0;

            try
            {
                connection.Open();

                // Fetch the user's current balance from the balance_table
                string balanceQuery = $"SELECT COALESCE(SUM(amount), 0) FROM balance_table WHERE user_id = {userID}";
                using (MySqlCommand balanceCmd = new MySqlCommand(balanceQuery, connection))
                {
                    currentBalance = Convert.ToDecimal(balanceCmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching balance: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }

            return currentBalance; // Ensure a return statement is present
        }
        private void BuyCoin(CryptocurrencyInfo selectedCrypto, decimal costPerCoin, int selectedQuantity, double totalPrice)
        {
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
            decimal currentBalance = GetCurrentBalance();

            // Check if the user has enough balance to buy the coin
            if (currentBalance >= costPerCoin)
            {
                // Calculate the new balance after the purchase
                decimal newBalance = currentBalance - costPerCoin;

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString)) // Use a new MySqlConnection
                    {
                        connection.Open();

                        // Update the user's balance in the balance_table
                        string updateBalanceQuery = $"UPDATE balance_table SET amount = {newBalance} WHERE user_id = {userID}";
                        using (MySqlCommand updateBalanceCommand = new MySqlCommand(updateBalanceQuery, connection))
                        {
                            updateBalanceCommand.ExecuteNonQuery();
                        }

                        // Check if the user already owns this coin
                        int coinId = GetCoinId(selectedCrypto.CoinSymbol);
                        string checkOwnershipQuery = $"SELECT COUNT(*) FROM user_coins WHERE user_id = {userID} AND crypto_id = {coinId}";
                        using (MySqlCommand checkOwnershipCommand = new MySqlCommand(checkOwnershipQuery, connection))
                        {
                            int count = Convert.ToInt32(checkOwnershipCommand.ExecuteScalar());

                            if (count > 0)
                            {
                                // If the user already owns the coin, update the amount
                                string updateOwnershipQuery = $"UPDATE user_coins SET amount = amount + 1 WHERE user_id = {userID} AND crypto_id = {coinId}";
                                using (MySqlCommand updateOwnershipCommand = new MySqlCommand(updateOwnershipQuery, connection))
                                {
                                    updateOwnershipCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // If the user doesn't own the coin, insert a new record
                                string insertOwnershipQuery = $"INSERT INTO user_coins (user_id, crypto_id, amount) VALUES ({userID}, {coinId}, 1)";
                                using (MySqlCommand insertOwnershipCommand = new MySqlCommand(insertOwnershipQuery, connection))
                                {
                                    insertOwnershipCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        // Insert a record into trade_history for the buy transaction
                        string insertTradeHistoryQuery = $"INSERT INTO trade_history (user_id, crypto_id, transaction_type, quantity, amount, coin_symbol, transaction_date) " +
                            $"VALUES ({userID}, {coinId}, 'Buy', {selectedQuantity}, {totalPrice}, '{selectedCrypto.CoinSymbol}', NOW())";

                        using (MySqlCommand insertTradeHistoryCommand = new MySqlCommand(insertTradeHistoryQuery, connection))
                        {
                            insertTradeHistoryCommand.ExecuteNonQuery();
                        }

                        // Display a success message or update the UI as needed
                        MessageBox.Show("Purchase successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                     
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Insufficient funds to buy the coin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void TradeForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[] sell = { 69.00, 68.50, 67.19, 67.18, 69.10, 68.35, 68.76, 68.33, 68 };
            double[] buy = { 69.19, 68.50, 67.19, 67.18, 69.10, 68.35, 68.76, 68.33, 68 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }



        private void button11_Click(object sender, EventArgs e)
        {
            MoveCandlestickUp();
            // Check if a cryptocurrency is selected in comboBox2
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a cryptocurrency.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the selected CryptocurrencyInfo object from the ComboBox
            CryptocurrencyInfo selectedCrypto = (CryptocurrencyInfo)comboBox2.SelectedItem;

            // Get the coin ID based on the selected coin symbol
            int coinId = GetCoinId(selectedCrypto.CoinSymbol);

            // Fetch the coin price from the database
            double coinPrice = GetCoinPrice(coinId);

            // Get the selected quantity from comboBox1
            if (!int.TryParse(comboBox1.SelectedItem.ToString(), out int selectedQuantity))
            {
                MessageBox.Show("Please select a valid quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calculate the total price based on quantity and coin price
            double totalPrice = selectedQuantity * coinPrice;

            // Check if the user has enough balance to buy the coin
            decimal currentBalance = GetCurrentBalance();
            if (currentBalance < (decimal)totalPrice)
            {
                MessageBox.Show("Insufficient funds to buy the coin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display a confirmation dialog with the details
            string confirmationMessage = $"Are you sure you want to buy {selectedQuantity} {selectedCrypto.CoinSymbol} for {totalPrice} PHP?";
            DialogResult result = MessageBox.Show(confirmationMessage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check if the user confirmed the purchase
            if (result == DialogResult.Yes)
            {

                // Execute the buy operation
                BuyCoin(selectedCrypto, (decimal)totalPrice, selectedQuantity, totalPrice);




                // Optionally, update the UI or perform any other necessary actions
                // ...

                // Update the balance label or any other UI element displaying the balance
                label100.Text = $"Balance: {GetCurrentBalance().ToString("F2")} PHP";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MoveCandlestickDown();
            // Check if a cryptocurrency is selected in comboBox2
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a cryptocurrency.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the selected CryptocurrencyInfo object from the ComboBox
            CryptocurrencyInfo selectedCrypto = (CryptocurrencyInfo)comboBox2.SelectedItem;

            // Get the coin ID based on the selected coin symbol
            int coinId = GetCoinId(selectedCrypto.CoinSymbol);

            // Check if the user owns the selected cryptocurrency
            if (!UserOwnsCoin(coinId))
            {
                MessageBox.Show("You do not own this cryptocurrency. Buy it first before selling.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Fetch the current coin price from the database
            double coinPrice = GetCoinPrice(coinId);

            // Get the selected quantity from comboBox1
            if (!int.TryParse(comboBox1.SelectedItem.ToString(), out int selectedQuantity))
            {
                MessageBox.Show("Please select a valid quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calculate the total earnings based on quantity and coin price
            double totalEarnings = selectedQuantity * coinPrice;

            // Perform the sell operation
            SellCoin((CryptocurrencyInfo)comboBox2.SelectedItem, selectedQuantity, totalEarnings);


            // Update the balance label or any other UI element displaying the balance
            label100.Text = $"Balance: {GetCurrentBalance().ToString("F2")} PHP";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double[] sell = { 22734.56, 22987.12, 22567.89, 22890.45, 22654.32, 22912.34, 22501.23, 22876.54, 22789.67, 22999.99 };
            double[] buy = { 22555.67, 22945.23, 22678.90, 22834.56, 22712.34, 22976.54, 22589.12, 22867.89, 22745.67, 22934.56 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double[] sell = { 82.34, 88.45, 91.27, 95.12, 98.76, 80.98, 85.43, 89.65, 92.54, 100.50 };
            double[] buy = { 86.20, 90.75, 94.89, 81.34, 87.56, 92.43, 96.10, 100.00, 80.67, 84.32 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double[] sell = { 170.45, 179.23, 174.56, 177.89, 172.34, 178.90, 171.12, 176.54, 173.67, 179.78 };
            double[] buy = { 175.63, 172.18, 177.45, 170.34, 178.91, 174.76, 179.67, 171.89, 176.28, 180.00 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double[] sell = { 0.25, 0.47, 0.62, 0.84, 0.33, 0.76, 0.92, 0.15, 0.71, 1.00 };
            double[] buy = { 0.12, 0.68, 0.45, 0.79, 0.23, 0.57, 0.91, 0.34, 0.65, 0.88 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double[] sell = { 0.37, 0.64, 0.21, 0.89, 0.45, 0.78, 0.53, 0.72, 0.96, 1.00 };
            double[] buy = { 0.55, 0.80, 0.42, 0.72, 0.19, 0.93, 0.67, 0.29, 0.58, 0.76 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double[] sell = { 61204, 61123, 61293, 61045, 61178, 61210, 61312, 61089, 61245, 61156 };
            double[] buy = { 61101, 61234, 61067, 61278, 61145, 61300, 61023, 61322, 61189, 61256 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            double[] sell = { 170.45, 179.23, 174.56, 177.89, 172.34, 178.90, 171.12, 176.54, 173.67, 179.78 };
            double[] buy = { 175.63, 172.18, 177.45, 170.34, 178.91, 174.76, 179.67, 171.89, 176.28, 180.00 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            double[] sell = { 2.23, 3.01, 2.45, 2.89, 2.12, 3.45, 2.67, 3.12, 2.98, 2.30 };
            double[] buy = { 3.21, 2.56, 2.89, 2.34, 3.00, 2.12, 2.78, 3.45, 2.67, 2.98 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            double[] sell = { 4.23, 5.01, 4.45, 4.89, 4.12, 5.45, 4.67, 5.12, 4.98, 4.30 };
            double[] buy = { 5.21, 4.56, 4.89, 4.34, 5.00, 4.12, 4.78, 5.45, 4.67, 4.98 };
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (double number in sell)
            {
                listBox1.Items.Add(number);
            }
            foreach (double number in buy)
            {
                listBox2.Items.Add(number);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }
        private void PopulateComboBox()
        {
            try
            {
                connection.Open();
               

                // Fetch the cryptocurrency data from the database
                string cryptoQuery = "SELECT coin_symbol, coin_rate FROM cryptocurrency";
                using (MySqlCommand cryptoCmd = new MySqlCommand(cryptoQuery, connection))
                {
                    using (MySqlDataReader reader = cryptoCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string coinSymbol = reader.GetString("coin_symbol");
                            double coinRate = reader.GetDouble("coin_rate");

                            // Populate the ComboBox with cryptocurrency names
                            comboBox2.Items.Add(new CryptocurrencyInfo { CoinSymbol = coinSymbol, CostPerCoin = coinRate });
                        }
                    }
                }
                comboBox2.DisplayMember = "CoinSymbol";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching cryptocurrency data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure that there is a selected item in the ComboBox
            if (comboBox2.SelectedItem != null)
            {
                // Get the selected CryptocurrencyInfo object from the ComboBox
                CryptocurrencyInfo selectedCrypto = (CryptocurrencyInfo)comboBox2.SelectedItem;

                // Get the coin ID based on the selected coin symbol
                int coinId = GetCoinId(selectedCrypto.CoinSymbol);

                // Fetch the coin price from the database
                double coinPrice = GetCoinPrice(coinId);

                // Format the coin price using a custom format to handle larger numbers
                string formattedPrice = FormatCoinPrice(coinPrice);

                // Display the formatted coin price in Label4
                label4.Text = $" {formattedPrice}";
            }
        }

        private string FormatCoinPrice(double coinPrice)
        {
            // Format the coin price with two decimal places
            return coinPrice.ToString("#,##0.00######## PHP");
        }



        private int GetCoinId(string coinSymbol)
        {
            try
            {
                connection.Open();

                // Fetch the coin ID based on the coin symbol
                string coinIdQuery = $"SELECT cryptocurrency_id FROM cryptocurrency WHERE coin_symbol = '{coinSymbol}'";
                using (MySqlCommand coinIdCmd = new MySqlCommand(coinIdQuery, connection))
                {
                    return Convert.ToInt32(coinIdCmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching coin ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1; // Return a default value or handle the error accordingly
            }
            finally
            {
                connection.Close();
            }
        }

        private double GetCoinPrice(int coinId)
        {
            try
            {
                connection.Open();

                // Fetch the coin price based on the coin ID
                string coinPriceQuery = $"SELECT coin_rate FROM cryptocurrency WHERE cryptocurrency_id = {coinId}";
                using (MySqlCommand coinPriceCmd = new MySqlCommand(coinPriceQuery, connection))
                {
                    return Convert.ToDouble(coinPriceCmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching coin price: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1; // Return a default value or handle the error accordingly
            }
            finally
            {
                connection.Close();
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Ensure that an item is selected in comboBox2
            if (comboBox2.SelectedItem != null)
            {
                // Get the selected quantity from comboBox1
                if (int.TryParse(comboBox1.SelectedItem.ToString(), out int selectedQuantity))
                {
                    // Get the coin price from the selected item in comboBox2
                    CryptocurrencyInfo selectedCrypto = (CryptocurrencyInfo)comboBox2.SelectedItem;
                    double coinPrice = (double)selectedCrypto.CostPerCoin;

                    // Calculate the total price based on quantity and coin price
                    double totalPrice = selectedQuantity * coinPrice;

                    // Update label5 with the total price
                    label5.Text = $" {totalPrice.ToString("#,##0.00")} PHP"; // Format as a decimal with two decimal places
                }
                else
                {
                    // Handle parsing error (e.g., display an error message)
                    label5.Text = "Invalid quantity selected.";
                }
            }
        }
        private bool UserOwnsCoin(int coinId)
        {
            try
            {
                connection.Open();

                // Check if the user owns the selected coin
                string checkOwnershipQuery = $"SELECT COUNT(*) FROM user_coins WHERE user_id = {userID} AND crypto_id = {coinId}";
                using (MySqlCommand checkOwnershipCommand = new MySqlCommand(checkOwnershipQuery, connection))
                {
                    int count = Convert.ToInt32(checkOwnershipCommand.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking ownership: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Handle the error accordingly
            }
            finally
            {
                connection.Close();
            }
        }

        private void SellCoin(CryptocurrencyInfo selectedCrypto, int quantity, double totalEarnings)
        {
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
            decimal currentBalance = GetCurrentBalance();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString)) // Use a new MySqlConnection
                {
                    connection.Open();

                    // Update the user's balance in the balance_table
                    decimal newBalance = currentBalance + (decimal)totalEarnings;
                    string updateBalanceQuery = $"UPDATE balance_table SET amount = {newBalance} WHERE user_id = {userID}";
                    using (MySqlCommand updateBalanceCommand = new MySqlCommand(updateBalanceQuery, connection))
                    {
                        updateBalanceCommand.ExecuteNonQuery();
                    }

                    // Get the coin ID based on the selected coin symbol
                    int coinId = GetCoinId(selectedCrypto.CoinSymbol);

                    // Update the user's coin balance in the user_coins table
                    string updateOwnershipQuery = $"UPDATE user_coins SET amount = amount - {quantity} WHERE user_id = {userID} AND crypto_id = {coinId}";
                    using (MySqlCommand updateOwnershipCommand = new MySqlCommand(updateOwnershipQuery, connection))
                    {
                        updateOwnershipCommand.ExecuteNonQuery();
                    }

                    // Insert a record into trade_history for the sell transaction
                    string insertTradeHistoryQuery = $"INSERT INTO trade_history (user_id, crypto_id, transaction_type, quantity, amount, coin_symbol, transaction_date) " +
                        $"VALUES ({userID}, {coinId}, 'Sell', {quantity}, {totalEarnings}, '{selectedCrypto.CoinSymbol}', NOW())";

                    using (MySqlCommand insertTradeHistoryCommand = new MySqlCommand(insertTradeHistoryQuery, connection))
                    {
                        insertTradeHistoryCommand.ExecuteNonQuery();
                    }

                    // Display a success message or update the UI as needed
                    MessageBox.Show($"Sold {quantity} {selectedCrypto.CoinSymbol} for {totalEarnings} PHP. New balance: {newBalance.ToString("F2")} PHP", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally, refresh the balance label or other UI elements
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }








}

