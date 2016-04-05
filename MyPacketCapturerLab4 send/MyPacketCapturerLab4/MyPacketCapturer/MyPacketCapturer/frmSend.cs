using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPacketCapturer
{
    public partial class frmSend : Form
    {
        public static int instantiations = 0;

        string[] packetArray = new string[] { "AC", "E0", "10", "7F", "8F", "B3", "00", "21", "86", "ED", "07", "35", "08", "00", "45", "00", "00", "34", "03", "3B", "00", "00", "80", "06", "00", "00", "8D", "A5", "D0", "31", "C0", "A8", "2B", "01", "ED", "30", "00", "50", "00", "00", "00", "02", "00", "00", "00", "00", "80", "02", "20", "00", "00", "00", "00", "00", "02", "04", "05", "B4", "01", "03", "03", "02", "01", "01", "04", "02" };
        string[] TCPTEST = new string[] { "AC", "E0", "10", "7F", "8F", "B3", "00", "21", "86", "ED", "07", "35", "08", "00", "45", "00", "00", "34", "03", "3B", "00", "00", "80", "06", "00", "00", "8D", "A5", "1c", "0a", "C0", "A8", "2B", "01", "ED", "30", "00", "50", "00", "00", "00", "02", "00", "00", "00", "00", "80", "02", "20", "00", "00", "00", "00", "00", "02", "04", "05", "B4", "01", "03", "03", "02", "01", "01", "04", "02" };
       
        //string arraycombine;
       // string[] ipheader = new string[] { "45", "00", "00", "34", "03", "3B", "00", "00", "80", "06", "00", "00", "8D", "A5", "D0", "31", "C0", "A8", "2B", "01"};
        


        //14-32
      //  string tcpPacket = "AC E0 10 7F 8F B3 00 21 86 ED 07 35 08 00 45 00 00 34 03 3B 00 00 80 06 EE 08 8D A5 D0 31 C0 A8 2B 01 ED 30 00 50 00 00 00 02 00 00 00 00 80 02 20 00 18 13 00 00 02 04 05 B4 01 03 03 02 01 01 04 02";
        //**********Default constructor
        string tcpPacket = "";
        public frmSend()
        {
            InitializeComponent();
            instantiations++;
            //arraycombine += charArray[0];
            //arraycombine += charArray[1];
            //Debug.WriteLine(arraycombine);
         
            /*
            string ipheaderchecksum="";
            foreach (string a in ipheader)
            {
                ipheaderchecksum += a + " ";
            }
            Debug.WriteLine(ipheaderchecksum);
            string[] sBytes3 = ipheaderchecksum.Split(new string[] { "\n", "\r\n", " ", "\t" },
           StringSplitOptions.RemoveEmptyEntries);


            byte[] ipchecksum = new byte[sBytes3.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes3) { ipchecksum[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }
            Debug.WriteLine("IP CHECKSUM: " + ComputeHeaderIpChecksum(ipchecksum, 0, ipchecksum.Length));
       */
        
        }  //End frmSend


        //**********Openning the file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if (openFileDialog1.FileName != "")
            {
                txtPacket.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        } //End openToolStripMenuItem_Click

        //**********Saving the file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            //Check to see if a filename was given
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtPacket.Text);
            }
        } //End saveToolStripMenuItem_Click


        //**********Sending out a packet (or packets)
        private void btnSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";
            //Get the hex values from the file
            foreach (string s in txtPacket.Lines){

                //Taking out the comments
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;
            }  //End for

        //Extract the hex values into a string array
        string[] sBytes = stringBytes.Split(new string[] {"\n","\r\n"," ","\t"},
            StringSplitOptions.RemoveEmptyEntries);

        //Change the strings into bytes
        byte[] packet = new byte[sBytes.Length];
        try
        {
            int i = 0;
            foreach (string s in sBytes) { packet[i] = Convert.ToByte(s, 16); i++; }
        }

        catch (Exception exp) { MessageBox.Show("Hex values not correct - cannot convert to bytes"); return; }
        string stringPackets="";
        string stingPackets2 = "";
            foreach (string ss in sBytes)
        {
            stingPackets2 += ss;
        }
            Debug.WriteLine(stingPackets2);
        foreach (byte b in packet)
        {
            stringPackets += b.ToString("X2") + " ";
        }
        Debug.WriteLine(stringPackets);
        //Sending out the packet
        try
        {
            frmCapture.device.SendPacket(packet);
        }
        catch (Exception exp) { MessageBox.Show("Cannot properly send the byte values"); return; }


        }  //End btnSend


        //**********Form is closing - decrement the number of instatiations
        private void frmSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instantiations--;
        } //End frmSend_FormClosed


        //**********Clear the screen
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtPacket.Clear();
        }







        /*------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * ------------------------------CONNECTION-----------------------------------
         * */
        private void button1_Click(object sender, EventArgs e)
        {

           getipchecksum();
            gettcpchecksum();


            Debug.WriteLine(tcpPacket);
            string[] sBytes2 = tcpPacket.Split(new string[] { "\n", "\r\n", " ", "\t" },
           StringSplitOptions.RemoveEmptyEntries);


            byte[] sendPacket = new byte[sBytes2.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes2) { sendPacket[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }
   
           // Debug.WriteLine("IP CHECKSUM: "+ ComputeHeaderIpChecksum(sendPacket, 14, sendPacket.Length));
            frmCapture.device.SendPacket(sendPacket);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string[] words = textBox1.Text.Split(' ');
                int i = 0;
                foreach (string word in words)
                {
                    packetArray[i] = word;
                    i++;
                }
              //  Debug.Write(packetArray[0] + packetArray[1] + packetArray[2]);
            }

            if (textBox2.Text != "")
            {
                string[] words = textBox2.Text.Split(' ');
                int i = 6;
                foreach (string word in words)
                {
                    packetArray[i] = word;
                    i++;
                }
                Debug.Write(packetArray[5] + packetArray[6] + packetArray[7]);
            }
            if (textBox3.Text != "")
            {
                string[] words = textBox3.Text.Split(' ','.');
                int i = 26;
                int value;
                string hexOutput;
                foreach (string word in words)
                {
                    value = Convert.ToInt32(word);
                    hexOutput = value.ToString("X2");
                    packetArray[i] = hexOutput;
                    i++;
                }
                Debug.Write(packetArray[26] + packetArray[27] + packetArray[28] + packetArray[29]);
            }
            if (textBox4.Text != "")
            {
                string[] words = textBox4.Text.Split(' ','.');
                int i = 30;
                int value;
                string hexOutput;
                foreach (string word in words)
                {
                    value = Convert.ToInt32(word);
                    hexOutput = value.ToString("X2");
                    packetArray[i] = hexOutput; i++;
                }
                      Debug.Write("IP DEST: "+packetArray[30] + packetArray[31] + packetArray[32] + packetArray[33]);
            }

        } //End clearToolStripMenuItem_Click


        ushort GetTCPChecksum(byte[] IPHeader, byte[] TCPHeader)
        {
            uint sum = 0;
            // TCP Header
            for (int x = 0; x < TCPHeader.Length; x += 2)
            {
                sum += ntoh(BitConverter.ToUInt16(TCPHeader, x));
            }
            // Pseudo header - Source Address
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 12));
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 14));

            // Pseudo header - Dest Address
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 16));
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 18));

            // Pseudo header - Protocol
            sum += ntoh(BitConverter.ToUInt16(new byte[] { 0, IPHeader[9] }, 0));

            // Pseudo header - TCP Header length
            sum += (UInt16)TCPHeader.Length;

            // 16 bit 1's compliment
            while ((sum >> 16) != 0) { sum = ((sum & 0xFFFF) + (sum >> 16)); }
            sum = ~sum;

            return (ushort)ntoh((UInt16)sum);
        }

        private ushort ntoh(UInt16 In)
        {
            int x = System.Net.IPAddress.NetworkToHostOrder(In);
            return (ushort)(x >> 16);
        }


        public void gettcpchecksum()
        {


            tcpPacket = "";
            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            //ipchecksum
            string ipheaderchecksum = "";

            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            for (int i = 14; i <= 33; i++)
            {
                ipheaderchecksum += packetArray[i] + " ";
            }
            Debug.WriteLine("IP CHECKSUM FOR TCP PART:" + ipheaderchecksum);

            string[] sBytes3 = ipheaderchecksum.Split(new string[] { "\n", "\r\n", " ", "\t" },
           StringSplitOptions.RemoveEmptyEntries);


            byte[] ipchecksum = new byte[sBytes3.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes3) { ipchecksum[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }







            //tcpchecksum
            string tcpchecksum = "";

            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            for (int i = 34; i <= packetArray.Length-1; i++)
            {
                tcpchecksum += packetArray[i] + " ";
            }
            Debug.WriteLine("TCP TCP TCP CHECKSUM FOR TCP PART: " + tcpchecksum);

            string[] sBytes4 = tcpchecksum.Split(new string[] { "\n", "\r\n", " ", "\t" },
           StringSplitOptions.RemoveEmptyEntries);


            byte[] tcpchecksumarray = new byte[sBytes4.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes4) { tcpchecksumarray[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }

            //string faketcpheader =  "8d a5", "d0", "31", "c0", "a8", "2b", "01","00","06","00","20"};
            string faketcpheader = tcpchecksum+ "8d a5 d0 31 c0 a8 2b 01 00 06 00 20";

            string[] sBytes5 = faketcpheader.Split(new string[] { "\n", "\r\n", " ", "\t" },
          StringSplitOptions.RemoveEmptyEntries);


            byte[] fakebyte = new byte[sBytes5.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes5) { fakebyte[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }


            string getTCPChecksum = GetTCPChecksum(ipchecksum, fakebyte).ToString("X2");
            Debug.WriteLine("TCP CHECKSUM: " + getTCPChecksum);

            /*
            if (getChecksum != "00")
            {
                packetArray[24] = "";
                packetArray[25] = "";
                int jjj = 0;
                foreach (char c in getChecksum)
                {

                    if (jjj < 2)
                    {
                        packetArray[24] += c;
                    }
                    else
                    {
                        packetArray[25] += c;
                    }
                    jjj++;
                }

            }
            */
           
            tcpPacket = "";
            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            //Debug.WriteLine("IP CHECKSUM: " + ComputeHeaderIpChecksum(ipchecksum, 0, ipchecksum.Length).ToString("X2"));


        }










        public static ushort ComputeHeaderIpChecksum(byte[] header, int start, int length)
        {

            ushort word16=0;
            long sum = 0;
            for (int i = start; i < (length + start); i += 2)
            {
                word16 = (ushort)(((header[i] << 8) & 0xFF00)
                + (header[i + 1] & 0xFF));
                sum += (long)word16;
            }

            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }

            sum = ~sum;

            return (ushort)sum;


        }

        public void getipchecksum() {


            tcpPacket = "";
            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            //ipchecksum
            string ipheaderchecksum = "";

            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            for (int i = 14; i <= 33; i++)
            {
                ipheaderchecksum += packetArray[i] + " ";
            }
            Debug.WriteLine("IP CHECKSUM " + ipheaderchecksum);

            string[] sBytes3 = ipheaderchecksum.Split(new string[] { "\n", "\r\n", " ", "\t" },
           StringSplitOptions.RemoveEmptyEntries);


            byte[] ipchecksum = new byte[sBytes3.Length];
            try
            {
                int i = 0;
                foreach (string ss in sBytes3) { ipchecksum[i] = Convert.ToByte(ss, 16); i++; }
            }
            catch { }
            string getChecksum = ComputeHeaderIpChecksum(ipchecksum, 0, ipchecksum.Length).ToString("X2");
            Debug.WriteLine("IP CHECKSUM: " + getChecksum);


            if (getChecksum != "00")
            {
                packetArray[24] = "";
                packetArray[25] = "";
                int jjj = 0;
                foreach (char c in getChecksum)
                {

                    if (jjj < 2)
                    {
                        packetArray[24] += c;
                    }
                    else
                    {
                        packetArray[25] += c;
                    }
                    jjj++;
                }
               
            }
            
            //   string[] getCheckSumArray = getChecksum.Split(' ');

            // packetArray[24]=getCheckSumArray[0];
            // packetArray[25] = getCheckSumArray[1];
            tcpPacket = "";
            foreach (string a in packetArray)
            {
                tcpPacket += a + " ";
            }

            //Debug.WriteLine("IP CHECKSUM: " + ComputeHeaderIpChecksum(ipchecksum, 0, ipchecksum.Length).ToString("X2"));
       

        }
    }
}
