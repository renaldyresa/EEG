using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;

using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace testprogram {
    public partial class Program : Form
    {
        static Connector connector = new Connector();
        static byte poorSig;
        private Label txtAttention;
        private Label valueAttention;
        private Label valueMeditation;
        private Label txtMeditation;
        private Label valueBlink;
        private Label txtBlink;
        private Label valueDelta;
        private Label txtDelta;
        private Label valueHighAlpha;
        private Label txtHighAlpha;
        private Label valueLowAlpha;
        private Label txtLowAlpha;
        private Label valueTheta;
        private Label txtTheta;
        private Label valueHighBeta;
        private Label txtHighBeta;
        private Label valueLowBeta;
        private Label txtLowBeta;
        private Button btnConnect;
        private Button btnStart;
        private Button btnStop;
        private Button btnDisconnect;
        private Thread thread;
        private delegate void updateAttention();
        private int sumAplha= 0, sumBeta=0 , sumTheta=0, fatigue=0;
        private int lowal = 0, highal = 0, lowbeta = 0, highbeta = 0, theta = 0;
        private Label txtFatigue;
        private Label valueFatigue;

        public Program()
        {
            InitializeComponent();
        }


        public void NeuroskySdk()
        {
            // Initialize a new Connector and add event handlers            
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);
            
        }

        // Called when a device is connected 

        public void OnDeviceConnected(object sender, EventArgs e)
        {

            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Console.WriteLine("Device found on: " + de.Device.PortName);
            de.Device.DataReceived += new EventHandler(OnDataReceived);

        }


        // Called when scanning fails

        public void OnDeviceFail(object sender, EventArgs e)
        {

            Console.WriteLine("No devices found! :(");

        }



        // Called when each port is being validated

        public void OnDeviceValidating(object sender, EventArgs e)
        {

            Console.WriteLine("Validating: ");

        }

        // Called when data is received from a device

        public void OnDataReceived(object sender, EventArgs e)
        {

            //Device d = (Device)sender;

            Device.DataEventArgs de = (Device.DataEventArgs)e;
            DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);



            /* Loops through the newly parsed data of the connected headset*/
            // The comments below indicate and can be used to print out the different data outputs. 
            

            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {

                if (tgParser.ParsedData[i].ContainsKey("Raw"))
                {

                    //Console.WriteLine("Raw Value:" + tgParser.ParsedData[i]["Raw"]);

                }


                if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
                {

                    //The following line prints the Time associated with the parsed data
                    //Console.WriteLine("Time:" + tgParser.ParsedData[i]["Time"]);

                    //A Poor Signal value of 0 indicates that your headset is fitting properly
                    //Console.WriteLine("Poor Signal:" + tgParser.ParsedData[i]["PoorSignal"]);

                    poorSig = (byte)tgParser.ParsedData[i]["PoorSignal"];
                }


                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {
                    try
                    {
                        add("Attention", "" + tgParser.ParsedData[i]["Attention"]);
                    }
                    catch (Exception ex) { }                  
                }


                if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                {
                    try
                    {
                        add("Meditation", "" + tgParser.ParsedData[i]["Meditation"]);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);

                }

                if (tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                {

                    try
                    {
                        add("Blink", "" + tgParser.ParsedData[i]["BlinkStrength"]);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Eyeblink " + tgParser.ParsedData[i]["BlinkStrength"]);

                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha1"))
                {
                    int aa = (int)tgParser.ParsedData[i]["EegPowerAlpha1"];
                    try
                    {
                        this.sumAplha += aa;
                        this.lowal += 1;
                        add("LowAlpha", "" + aa);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerAlpha2"))
                {
                    int aa = (int)tgParser.ParsedData[i]["EegPowerAlpha2"];
                    try
                    {
                        this.sumAplha += aa;
                        this.highal += 1;
                        add("HighAlpha", "" + aa);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);

                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta1"))
                {
                    int aa = (int)tgParser.ParsedData[i]["EegPowerBeta1"];
                    try
                    {
                        this.sumBeta += aa;
                        this.lowbeta += 1;
                        add("LowBeta", "" + aa);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);
                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta2"))
                {
                    int aa = (int)tgParser.ParsedData[i]["EegPowerBeta2"];
                    try
                    {
                        this.sumBeta += aa;
                        this.highbeta += 1;
                        add("HighBeta", "" + aa);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Med Value:" + tgParser.ParsedData[i]["Meditation"]);

                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerDelta"))
                {
                    try
                    {
                        add("Delta", "" + tgParser.ParsedData[i]["EegPowerDelta"]);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("Delta: " + tgParser.ParsedData[i]["EegPowerDelta"]);

                }

                if (tgParser.ParsedData[i].ContainsKey("EegPowerTheta"))
                {
                    int aa = (int)tgParser.ParsedData[i]["EegPowerTheta"];
                    try
                    {
                        this.sumTheta += aa;
                        this.theta += 1;
                        add("Theta", "" + aa);
                    }
                    catch (Exception ex) { }
                    //Console.WriteLine("EegPowerTheta Value:" + tgParser.ParsedData[i]["EegPowerTheta"]);

                }                      
            }

        }

        public void add(string type, string data)
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action<string, string>(add), new object[] { type, data });
                return;
            }

            switch (type)
            {
                case "Attention":
                    valueAttention.Text = data;
                    break;
                case "Meditation":
                    valueMeditation.Text = data;
                    break;
                case "Blink":
                    valueBlink.Text = data;
                    break;
                case "LowAlpha":
                    valueLowAlpha.Text = data;
                    break;
                case "HighAlpha":
                    valueHighAlpha.Text = data;
                    break;
                case "LowBeta":
                    valueLowBeta.Text = data;
                    break;
                case "HighBeta":
                    valueHighBeta.Text = data;
                    break;
                case "Delta":
                    valueDelta.Text = data;
                    break;
                case "Theta":
                    valueTheta.Text = data;
                    break;
            }



        }

        public static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }
      

        private void InitializeComponent()
        {
            this.txtAttention = new System.Windows.Forms.Label();
            this.valueAttention = new System.Windows.Forms.Label();
            this.valueMeditation = new System.Windows.Forms.Label();
            this.txtMeditation = new System.Windows.Forms.Label();
            this.valueBlink = new System.Windows.Forms.Label();
            this.txtBlink = new System.Windows.Forms.Label();
            this.valueDelta = new System.Windows.Forms.Label();
            this.txtDelta = new System.Windows.Forms.Label();
            this.valueHighAlpha = new System.Windows.Forms.Label();
            this.txtHighAlpha = new System.Windows.Forms.Label();
            this.valueLowAlpha = new System.Windows.Forms.Label();
            this.txtLowAlpha = new System.Windows.Forms.Label();
            this.valueTheta = new System.Windows.Forms.Label();
            this.txtTheta = new System.Windows.Forms.Label();
            this.valueHighBeta = new System.Windows.Forms.Label();
            this.txtHighBeta = new System.Windows.Forms.Label();
            this.valueLowBeta = new System.Windows.Forms.Label();
            this.txtLowBeta = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.valueFatigue = new System.Windows.Forms.Label();
            this.txtFatigue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAttention
            // 
            this.txtAttention.AccessibleDescription = "Attention";
            this.txtAttention.AccessibleName = "Attention";
            this.txtAttention.AutoSize = true;
            this.txtAttention.Location = new System.Drawing.Point(219, 32);
            this.txtAttention.Name = "txtAttention";
            this.txtAttention.Size = new System.Drawing.Size(49, 13);
            this.txtAttention.TabIndex = 0;
            this.txtAttention.Text = "Attention";
            // 
            // valueAttention
            // 
            this.valueAttention.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueAttention.Location = new System.Drawing.Point(284, 28);
            this.valueAttention.Name = "valueAttention";
            this.valueAttention.Size = new System.Drawing.Size(98, 22);
            this.valueAttention.TabIndex = 1;
            // 
            // valueMeditation
            // 
            this.valueMeditation.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueMeditation.Location = new System.Drawing.Point(284, 59);
            this.valueMeditation.Name = "valueMeditation";
            this.valueMeditation.Size = new System.Drawing.Size(98, 22);
            this.valueMeditation.TabIndex = 3;
            // 
            // txtMeditation
            // 
            this.txtMeditation.AccessibleDescription = "Attention";
            this.txtMeditation.AccessibleName = "Attention";
            this.txtMeditation.AutoEllipsis = true;
            this.txtMeditation.AutoSize = true;
            this.txtMeditation.Location = new System.Drawing.Point(219, 63);
            this.txtMeditation.Name = "txtMeditation";
            this.txtMeditation.Size = new System.Drawing.Size(56, 13);
            this.txtMeditation.TabIndex = 2;
            this.txtMeditation.Text = "Meditation";
            // 
            // valueBlink
            // 
            this.valueBlink.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueBlink.Location = new System.Drawing.Point(284, 90);
            this.valueBlink.Name = "valueBlink";
            this.valueBlink.Size = new System.Drawing.Size(98, 22);
            this.valueBlink.TabIndex = 5;
            // 
            // txtBlink
            // 
            this.txtBlink.AccessibleDescription = "Attention";
            this.txtBlink.AccessibleName = "Attention";
            this.txtBlink.AutoSize = true;
            this.txtBlink.Location = new System.Drawing.Point(219, 94);
            this.txtBlink.Name = "txtBlink";
            this.txtBlink.Size = new System.Drawing.Size(30, 13);
            this.txtBlink.TabIndex = 4;
            this.txtBlink.Text = "Blink";
            // 
            // valueDelta
            // 
            this.valueDelta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueDelta.Location = new System.Drawing.Point(284, 250);
            this.valueDelta.Name = "valueDelta";
            this.valueDelta.Size = new System.Drawing.Size(98, 22);
            this.valueDelta.TabIndex = 11;
            // 
            // txtDelta
            // 
            this.txtDelta.AccessibleDescription = "Attention";
            this.txtDelta.AccessibleName = "Attention";
            this.txtDelta.AutoSize = true;
            this.txtDelta.Location = new System.Drawing.Point(219, 254);
            this.txtDelta.Name = "txtDelta";
            this.txtDelta.Size = new System.Drawing.Size(32, 13);
            this.txtDelta.TabIndex = 10;
            this.txtDelta.Text = "Delta";
            // 
            // valueHighAlpha
            // 
            this.valueHighAlpha.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueHighAlpha.Location = new System.Drawing.Point(284, 152);
            this.valueHighAlpha.Name = "valueHighAlpha";
            this.valueHighAlpha.Size = new System.Drawing.Size(98, 22);
            this.valueHighAlpha.TabIndex = 9;
            // 
            // txtHighAlpha
            // 
            this.txtHighAlpha.AccessibleDescription = "Attention";
            this.txtHighAlpha.AccessibleName = "Attention";
            this.txtHighAlpha.AutoEllipsis = true;
            this.txtHighAlpha.AutoSize = true;
            this.txtHighAlpha.Location = new System.Drawing.Point(219, 156);
            this.txtHighAlpha.Name = "txtHighAlpha";
            this.txtHighAlpha.Size = new System.Drawing.Size(56, 13);
            this.txtHighAlpha.TabIndex = 8;
            this.txtHighAlpha.Text = "HighAlpha";
            // 
            // valueLowAlpha
            // 
            this.valueLowAlpha.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueLowAlpha.Location = new System.Drawing.Point(284, 121);
            this.valueLowAlpha.Name = "valueLowAlpha";
            this.valueLowAlpha.Size = new System.Drawing.Size(98, 22);
            this.valueLowAlpha.TabIndex = 7;
            // 
            // txtLowAlpha
            // 
            this.txtLowAlpha.AccessibleDescription = "Attention";
            this.txtLowAlpha.AccessibleName = "Attention";
            this.txtLowAlpha.AutoSize = true;
            this.txtLowAlpha.Location = new System.Drawing.Point(219, 125);
            this.txtLowAlpha.Name = "txtLowAlpha";
            this.txtLowAlpha.Size = new System.Drawing.Size(54, 13);
            this.txtLowAlpha.TabIndex = 6;
            this.txtLowAlpha.Text = "LowAlpha";
            // 
            // valueTheta
            // 
            this.valueTheta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueTheta.Location = new System.Drawing.Point(284, 286);
            this.valueTheta.Name = "valueTheta";
            this.valueTheta.Size = new System.Drawing.Size(98, 22);
            this.valueTheta.TabIndex = 17;
            // 
            // txtTheta
            // 
            this.txtTheta.AccessibleDescription = "Attention";
            this.txtTheta.AccessibleName = "Attention";
            this.txtTheta.AutoSize = true;
            this.txtTheta.Location = new System.Drawing.Point(219, 290);
            this.txtTheta.Name = "txtTheta";
            this.txtTheta.Size = new System.Drawing.Size(35, 13);
            this.txtTheta.TabIndex = 16;
            this.txtTheta.Text = "Theta";
            // 
            // valueHighBeta
            // 
            this.valueHighBeta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueHighBeta.Location = new System.Drawing.Point(284, 218);
            this.valueHighBeta.Name = "valueHighBeta";
            this.valueHighBeta.Size = new System.Drawing.Size(98, 22);
            this.valueHighBeta.TabIndex = 15;
            // 
            // txtHighBeta
            // 
            this.txtHighBeta.AccessibleDescription = "Attention";
            this.txtHighBeta.AccessibleName = "Attention";
            this.txtHighBeta.AutoEllipsis = true;
            this.txtHighBeta.AutoSize = true;
            this.txtHighBeta.Location = new System.Drawing.Point(219, 222);
            this.txtHighBeta.Name = "txtHighBeta";
            this.txtHighBeta.Size = new System.Drawing.Size(51, 13);
            this.txtHighBeta.TabIndex = 14;
            this.txtHighBeta.Text = "HighBeta";
            // 
            // valueLowBeta
            // 
            this.valueLowBeta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.valueLowBeta.Location = new System.Drawing.Point(284, 187);
            this.valueLowBeta.Name = "valueLowBeta";
            this.valueLowBeta.Size = new System.Drawing.Size(98, 22);
            this.valueLowBeta.TabIndex = 13;
            // 
            // txtLowBeta
            // 
            this.txtLowBeta.AccessibleDescription = "Attention";
            this.txtLowBeta.AccessibleName = "Attention";
            this.txtLowBeta.AutoSize = true;
            this.txtLowBeta.Location = new System.Drawing.Point(219, 191);
            this.txtLowBeta.Name = "txtLowBeta";
            this.txtLowBeta.Size = new System.Drawing.Size(49, 13);
            this.txtLowBeta.TabIndex = 12;
            this.txtLowBeta.Text = "LowBeta";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(16, 43);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(91, 33);
            this.btnConnect.TabIndex = 18;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 84);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(91, 33);
            this.btnStart.TabIndex = 19;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(113, 84);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(91, 33);
            this.btnStop.TabIndex = 20;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(113, 43);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(91, 33);
            this.btnDisconnect.TabIndex = 21;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // valueFatigue
            // 
            this.valueFatigue.BackColor = System.Drawing.SystemColors.Info;
            this.valueFatigue.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueFatigue.Location = new System.Drawing.Point(32, 182);
            this.valueFatigue.Margin = new System.Windows.Forms.Padding(3);
            this.valueFatigue.Name = "valueFatigue";
            this.valueFatigue.Size = new System.Drawing.Size(154, 72);
            this.valueFatigue.TabIndex = 22;
            this.valueFatigue.Text = "0";
            this.valueFatigue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtFatigue
            // 
            this.txtFatigue.AutoSize = true;
            this.txtFatigue.Location = new System.Drawing.Point(31, 160);
            this.txtFatigue.Name = "txtFatigue";
            this.txtFatigue.Size = new System.Drawing.Size(48, 13);
            this.txtFatigue.TabIndex = 23;
            this.txtFatigue.Text = "Fatigue :";
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(420, 333);
            this.Controls.Add(this.txtFatigue);
            this.Controls.Add(this.valueFatigue);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.valueTheta);
            this.Controls.Add(this.txtTheta);
            this.Controls.Add(this.valueHighBeta);
            this.Controls.Add(this.txtHighBeta);
            this.Controls.Add(this.valueLowBeta);
            this.Controls.Add(this.txtLowBeta);
            this.Controls.Add(this.valueDelta);
            this.Controls.Add(this.txtDelta);
            this.Controls.Add(this.valueHighAlpha);
            this.Controls.Add(this.txtHighAlpha);
            this.Controls.Add(this.valueLowAlpha);
            this.Controls.Add(this.txtLowAlpha);
            this.Controls.Add(this.valueBlink);
            this.Controls.Add(this.txtBlink);
            this.Controls.Add(this.valueMeditation);
            this.Controls.Add(this.txtMeditation);
            this.Controls.Add(this.valueAttention);
            this.Controls.Add(this.txtAttention);
            this.Name = "Program";
            this.Load += new System.EventHandler(this.Program_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Program_Load(object sender, EventArgs e)
        {
            
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(NeuroskySdk));
            thread.IsBackground = true;
            thread.Start();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Scan for devices across COM ports
            // The COM port named will be the first COM port that is checked.
            connector.ConnectScan("COM40");
            // Blink detection needs to be manually turned on
            connector.setBlinkDetectionEnabled(true);
        }

        

        private void BtnStop_Click(object sender, EventArgs e)
        {
            System.Console.WriteLine("Goodbye.");
            connector.Close();
            CalFatigue();
            
        }    

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            thread.Abort();
        }

        private void CalFatigue()
        {
            if(sumBeta != 0)
            fatigue = (sumAplha + sumTheta) / sumBeta;
            Console.WriteLine("Nilai Fatigue : " + fatigue);
            valueFatigue.Text = "" + fatigue;
            Console.WriteLine("Low Alpha : " + lowal);
            Console.WriteLine("High Alpha : " + highal);
            Console.WriteLine("Low Beta : " + lowbeta);
            Console.WriteLine("High Beta : " + highbeta);
            Console.WriteLine("Theta : " + theta);
        }
    }

}
