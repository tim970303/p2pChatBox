﻿using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace chatBoxHome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket SckSs;   //定義socket
        string LocalIP; //定義本地IP
        int SPort = 20282;  //定義本地Port
        int RDataLen;
        string S;
        String HostName = Dns.GetHostName();
        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192"))
                {
                    LocalIP = ipaddress.ToString();
                }
            }
            textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Listen();
        }

        private void SckSWaitAccept()
        {
            Thread SckSAcceptTd = new Thread(SckSAcceptProc);
            SckSAcceptTd.Start();
            SckSAcceptTd.IsBackground = true;
        }

        private void SckSAcceptProc()
        {
            try
            {
                SckSs = SckSs.Accept();
                textBox1.AppendText("Connect!!\r\n");
                button1.Enabled = true;
                button2.Enabled = false;
                textBox2.Enabled = true;
                send(true,"");
                long IntAcceptData;

                while (true)

                {
                    byte[] clientData = new byte[3000];
                    IntAcceptData = SckSs.Receive(clientData);
                    S = Encoding.UTF8.GetString(clientData);
                    string check = textBox1.Text;
                    textBox1.AppendText(S);

                }
            }
            catch
            { 
            }

        }

        private void Listen()
        {
            SckSs = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SckSs.Bind(new IPEndPoint(IPAddress.Parse(LocalIP), SPort));
            SckSs.Listen(10);
            SckSWaitAccept();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send(false,"");
        }

        private void send(bool first,string s)
        {
            RDataLen = Encoding.UTF8.GetBytes(textBox2.Text + "\r\n").Length;
            if (first == true)
            {
                SckSs.Send(Encoding.UTF8.GetBytes("Connect successful\r\n"));
                return;
            }
            if (s == "disconnect")
            {
                try
                {
                    SckSs.Send(Encoding.UTF8.GetBytes("//Server disconnect//\r\n"));
                }
                catch { }
                return;
            }
            if (SckSs.Connected == true)

            {
                if (textBox2.Text != "")
                {
                    try
                    {
                        string SendS = "Server : " + textBox2.Text + "\r\n";
                        SckSs.Send(Encoding.UTF8.GetBytes(SendS));
                        textBox1.AppendText(textBox2.Text + "\r\n");
                        textBox2.Text = "";
                    }

                    catch
                    {
                    }
                }
            }
            else
            {

            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("//Client Disconnect//"))
            {
                MessageBox.Show("Client Disconnect\r\nPlease restart the application","Connect Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            send(false, "disconnect");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192") && button2.Text == "Private")
                {
                    LocalIP = ipaddress.ToString();
                    textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
                    button2.Text = "Public";
                    return;
                }
                else if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ipaddress.ToString().StartsWith("192") && button2.Text != "Private")
                {
                    LocalIP = ipaddress.ToString();
                    textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
                    button2.Text = "Private";
                    return;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text += LocalIP + "\r\n";
        }
    }
}