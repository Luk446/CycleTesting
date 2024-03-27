using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace CycleTesting
{
    // Define variables for storing data
    public partial class Form1 : Form  
    {
        private string csvFilePath; // define string for csv upload 
        // declare the vars for lists for pv and tv values (not init yet)
        private List<int> pvValues;
        private List<int> tvValues;
        private List<int> avValues;

        // create the timers 
        private Timer timer; // 'simple' timer
        private System.Windows.Forms.Timer progressTimer = new System.Windows.Forms.Timer();
        // in second case, a different type of timer is used that is more suitable for constant updating 


        // create the form execute certain preliminary blocks + functions
        public Form1()
        {
            InitializeComponent(); // create
            // now they are instantiated using 'new' which allocates memory and opens empty list
            pvValues = new List<int>();
            tvValues = new List<int>();
            avValues = new List<int>();

            InitializeTimer(); // init the primary timer 
            progressTimer.Tick += new EventHandler(progressTimer_Tick); // attach event handler to tick for bar timer
        }

        // Timer for the bar tick event 
        // includes cond. state for stopping when finished
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            // Increment progress bar
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
            }
            else
            {
                // Stop when max val
                progressTimer.Stop();
            }
        }

        // Initialise the serial port and define the params 
        private void InitializeSerialPort()
        {
            serialPort = new SerialPort(); // create 
            serialPort.PortName = "COM3"; // set the port ( fix later to rid hard code!!)
            serialPort.BaudRate = 9600; // set speed of transmission ( the same as set in Arduino IDE)
            serialPort.DataReceived += SerialPort_DataReceived; // subsribe event to handler that processes
            serialPort.Open(); // begin data transmission
        }

        // Create object for starting timer (primary timer)
        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Tick += Timer_Tick;
            // note - interval value defined elsewhere for updown val
        }

        // data reception and processing block
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string incomingData = serialPort.ReadLine(); // read each line

            // Due to complex data parse for data with approriate values
            if (incomingData.StartsWith("PV: ")) // check for "PV: "
            {
                string dataValue = incomingData.Substring(4); // we need to ignore first 4 points so that the identifier isnt counted
                // here we use int.TryParse to negate crashing, if for any reason the value was not an integer (string etc)
                // then it could cause the application to freeze/crash so this way the data collection can continue and ignore error
                if (int.TryParse(dataValue, out int potValue)) 
                {
                    pvValues.Add(potValue); // add to the predetermined string the integer value
                }
            }
            // secondary case for the other value present in monitor
            else if (incomingData.StartsWith("TV: "))
            {
                string dataValue = incomingData.Substring(4); 
                if (int.TryParse(dataValue, out int tvValue))
                {
                    tvValues.Add(tvValue);
                }
            }
            // check for third val
            else if (incomingData.StartsWith("AV: "))
            {
                string dataValue = incomingData.Substring(4);
                if (int.TryParse(dataValue, out int avValue))
                {
                    avValues.Add(avValue);
                }
            }
        }

        // Once the 'tick' event is completed this is called which stops the data collection
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop(); // stop timer
            serialPort.Close(); // close port 
            SaveDataToCSV(); // execute data save
            MessageBox.Show("Data collection completed and saved."); // inform user
        }

        // create a method for writing the data to csv
        // for the magnitude of data collected (depending on what string) execute a number of loops of line writing 
        private void SaveDataToCSV()
        {
            // before saving data check:
            // ensure the file path is legitimate ( not empty)
            // ensure thay data has actually been recorded ( likely result of serial port wire issue )
            // these checks like others before prevent crashes and the storing of wrong data
            if (!string.IsNullOrEmpty(csvFilePath) && (pvValues.Count > 0 || tvValues.Count > 0))
            {
                using (StreamWriter sw = new StreamWriter(csvFilePath)) // uses .NET library function
                {
                    // Define headers ( this is to simplify python data analysis in future )
                    sw.WriteLine("PV,TV,AV");
                    // check how many items are both strings and then loop for max of that
                    for (int i = 0; i < Math.Max(Math.Max(pvValues.Count, tvValues.Count), avValues.Count); i++)
                    {
                        // define a variable for the values but also:
                        // like most seemingly over complicated stuff its here to avoid error
                        // this ensures proper aligment of data in the csv, basically if the value is bounded in the i range
                        // it will attach to var, if not, the string will be empty 
                        // sequence var = is i smaller than the amount ? then if so attach value to the var
                        // TLDR: stops csv aligment screwing up
                        var pvValue = i < pvValues.Count ? pvValues[i].ToString() : "";
                        var tvValue = i < tvValues.Count ? tvValues[i].ToString() : "";
                        var avValue = i < avValues.Count ? avValues[i].ToString() : "";

                        // print line 
                        sw.WriteLine($"{pvValue},{tvValue},{avValue}");
                    }
                }
            }
        }

        // Handles what happens when the Record button is pressed
        private void button1_Click(object sender, EventArgs e)
        {
            // using .NET library for prompt
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv"; // user can only save as csv not anything else
                saveFileDialog.Title = "Save the CSV File"; // Set window title
                saveFileDialog.ShowDialog(); // display configured window


                if (!string.IsNullOrEmpty(saveFileDialog.FileName)) // checks if user has selected the file path
                {
                    csvFilePath = saveFileDialog.FileName; // instantiate file path

                    // cleanse the inputs
                    pvValues.Clear();
                    tvValues.Clear();
                    avValues.Clear();

                    // begin data collection!
                    InitializeSerialPort();

                    // Set the timer interval based on the numericUpDown1 value
                    timer.Interval = (int)numericUpDown1.Value * 1000; // C# reads time in ms so convert to s
                    timer.Start(); // Start the timer
                    MessageBox.Show("Recording Begun"); // inform user of starting

                    // Now for the bar timer

                    // Start the progress bar from min value
                    progressBar1.Value = progressBar1.Minimum; // set the progress bar initial value
                    int totalDuration = (int)numericUpDown1.Value * 1000; // Convert to ms
                    int interval = totalDuration / (progressBar1.Maximum - progressBar1.Minimum); // do some maths to find the tick range/rate
                    progressTimer.Interval = interval; // define the length of timer based on above maths
                    progressTimer.Start(); // start timer with above params 
                }
                else // secondary case incase of imporoper user action
                {
                    MessageBox.Show("Invalid File Path");
                }
            }
        }
        // these are autogenerated objects and serve no function

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
