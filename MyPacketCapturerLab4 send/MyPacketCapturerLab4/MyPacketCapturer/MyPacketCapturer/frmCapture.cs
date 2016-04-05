using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;
using System.Diagnostics;

namespace MyPacketCapturer
{
    public partial class frmCapture : Form
    {

        CaptureDeviceList devices;   //List of devices for this computer
        public static ICaptureDevice device; //The device we will be using
        public static string stringPackets = ""; //Data that is captured
        static int numPackets = 0;
        frmSend fSend;  //This will be our send form
       static Boolean  ackConnection;

        //**********Default constructor
        public frmCapture()
        {
            InitializeComponent();
            ackConnection = false ;
            //Get the list of devices
            devices = CaptureDeviceList.Instance;

            //Make sure that there is at least one device
            if (devices.Count < 1)
            {
                MessageBox.Show("No Capture Devices Found!!!");
                Application.Exit();
            }

            //Add the devices to the combo box
            foreach (ICaptureDevice dev in devices)
            {
                cmbDevices.Items.Add(dev.Description);
            }

            //Get the second device and display in combo box
            device = devices[5];
            cmbDevices.Text = device.Description;

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

        } //End frmCapture


        //********** Event handler when a packet arrives
        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            //Increment the number of packets captured
            numPackets++;

            //Put the packet number in the capture window
            //   stringPackets += "Packet Number: " + Convert.ToString(numPackets);
            //stringPackets += Environment.NewLine;

            //Array to store our data
            byte[] data = packet.Packet.Data;

            //Keep track of the number of bytes displayed per line
            int byteCounter = 0;


            //   stringPackets += "Destination MAC Address: ";
            //Parsing the packets
            if (data[23] == 6)
            {
                if (data[26] == 10)
                {
                    if (data[27] == 30)
                    {
                        if (data[28] == 12)
                        {
                            if (data[29] == 26)
                            {
                                foreach (byte b in data)
                                {
                                    //Add the byte to our string (in hexidecimal)
                                    if (byteCounter <= 58) stringPackets += b.ToString("X2") + " ";
                                    byteCounter++;

                                    switch (byteCounter)
                                    {

                                        case 6: stringPackets += Environment.NewLine;
                                            stringPackets += "Source MAC Address: ";
                                            break;
                                        case 12: stringPackets += Environment.NewLine;
                                            stringPackets += "EtherType: ";
                                            break;
                                        case 14: if (data[12] == 8)
                                            {
                                                if (data[13] == 0) stringPackets += "(IP)";
                                                if (data[13] == 6) stringPackets += "(ARP)";

                                            }
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Header Length: ";
                                            break;
                                        case 15:

                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Differ: service field: \n";
                                            break;


                                        case 16:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Total Length: ";
                                            break;

                                        case 18:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Identifcation: ";
                                            break;

                                        case 20:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Flag and Offset: ";

                                            break;

                                        case 22:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Time To Live: ";
                                            break;

                                        case 23:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Protocol: ";
                                            break;


                                        case 24:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Header Check Sum: ";
                                            break;
                                        case 26:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Source IP: ";
                                            break;
                                        case 30:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Dest IP: ";
                                            break;
                                        case 34:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Source Port: ";
                                            break;
                                        case 36:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Dest Port: ";
                                            break;
                                        case 38:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Seq Number: ";
                                            break;
                                        case 42:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Ack Number: ";
                                            break;
                                        case 46:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Header Len: ";
                                            break;
                                        case 47:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Flag: ";
                                            break;
                                        case 48:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Header Len: ";
                                            break;
                                        case 50:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Checksum: ";
                                            break;
                                        case 52:
                                            stringPackets += Environment.NewLine;
                                            stringPackets += "Options: ";
                                            break;

                                    }
                                }


                                
                                if ((data[47] == 18) && (ackConnection==false))
                                {
                                   // Debug.WriteLine("SYN");
                                    ackConnection = true;
                                    Debug.WriteLine("ACK");
                                   
                                    //string tcpPacketAck = "AC E0 10 7F 8F B3 00 21 86 ED 07 35 08 00 45 00 00 34 03 3B 00 00 80 06 ee 08 8D A5 D0 31 C0 A8 2B 01 ED 30 00 50 00 00 00 03 00 00 00 03 80 10 20 00 18 01 00 00 02 04 05 B4 01 03 03 02 01 01 04 02";


                                    byte[] swapPacket = new byte[data.Length];
                                    //Swap the source and destination around.
                                  swapPacket=swapDestination(data);
                                  
                                    string sendAckConnection="";
                                    int j = 0;
                                    foreach (byte bb in swapPacket)
                                    {
                                        if (j == 47) 
                                        { sendAckConnection += "10" + " "; }
                                        sendAckConnection += bb.ToString("X2") + " ";
                                        j++;
                                    }
                                   string[] sBytes3 = sendAckConnection.Split(new string[] { "\n", "\r\n", " ", "\t" },
                                     StringSplitOptions.RemoveEmptyEntries);


                                    byte[] sendAckConnectionPacket = new byte[sBytes3.Length];
                                    try
                                    {
                                        int i = 0;
                                        foreach (string ss in sBytes3) { sendAckConnectionPacket[i] = Convert.ToByte(ss, 16); i++; }
                                    }
                                    catch { }

                                    frmCapture.device.SendPacket(sendAckConnectionPacket);
                               
                                     }

                                stringPackets += Environment.NewLine + Environment.NewLine;
                                byteCounter = 0;
                                stringPackets += "Raw Data" + Environment.NewLine;
                                //Process each byte in our captured packet
                                foreach (byte b in data)
                                {
                                    //Add the byte to our string (in hexidecimal)
                                    stringPackets += b.ToString("X2") + " ";
                                    byteCounter++;

                                    if (byteCounter == 16)
                                    {
                                        byteCounter = 0;
                                        stringPackets += Environment.NewLine;
                                    }

                                }
                                stringPackets += Environment.NewLine;
                                stringPackets += Environment.NewLine;
                            } //End device_OnPacketArrival
 
                        }
                                     
                    }
                               
                }
            }
        }      
            
    
        //**********Starting and stopping the packet capturing
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text == "Start")
                {
                    btnStartStop.Text = "Stop";
                    device.StartCapture();
                    timer1.Enabled = true;
                  }
                else
                {
                    btnStartStop.Text = "Start";
                    device.StopCapture();
                    timer1.Enabled = false;
                }
            }
            
            catch (Exception exp) { MessageBox.Show("Error starting and stopping capture"); }
        } //End btnStartStop
       
        public static byte[] swapDestination(byte[] d)
        {
            byte[] newdata = new byte[d.Length];
            int i = 0;
            foreach (byte a in d)
            {
                newdata[i] = a;
                i++;
            }
            //dest mac
            newdata[0] = d[6];
            newdata[1] = d[7];
            newdata[2] = d[8];
            newdata[3] = d[9];
            newdata[4] = d[10];
            newdata[5] = d[11];

            newdata[6] = d[0];
            newdata[7] = d[1];
            newdata[8] = d[2];
            newdata[9] = d[3];
            newdata[10] = d[4];
            newdata[11] = d[5];


            newdata[26] = d[30];
            newdata[27] = d[31];
            newdata[28] = d[32];
            newdata[29] = d[33];

            newdata[30] = d[26];
            newdata[31] = d[27];
            newdata[32] = d[28];
            newdata[33] = d[29];



            return newdata;
        }

        //**********Dumping the packet data from stringPackets to the text box
        private void timer1_Tick(object sender, EventArgs e)
        {
            txtCapturedData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);
        } //End timer1


        //**********Changing devices
        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (device != null) device.Close();
            device = devices[cmbDevices.SelectedIndex];
            cmbDevices.Text = device.Description;
            txtGUID.Text = device.Name;

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        } //End cmbDevices_SelectedIndexChanged

 
        //**********Saving the file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtCapturedData.Text);
            }
        } //End saveToolStripMenuItem_Click


        //**********Openning the file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if (openFileDialog1.FileName != "")
            {
                txtCapturedData.Text=System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        } //End openToolStripMenuItem_Click


        //**********Show the send window
        private void sendWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSend.instantiations == 0)
            {
                fSend = new frmSend();  //Creates a new frmSend
                fSend.Show();
            }
        }  //End sendWindowToolStripMenuItem_Click


        //**********Clear the main textbox
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCapturedData.Clear();
            numPackets = 0;
            txtNumPackets.Text = "0";
        }  //End learToolStripMenuItem_Click
    }
}
