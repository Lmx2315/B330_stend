using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;

namespace stnd_72_v2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
 
    public struct CHANNEL
    {
        public int OFF; //определяет включён или выключен соответствующий канал по цепи 12 Вольт
        public int PWR;
        public int I;
        public int U;
        public int P;
        public int T;
        public string ID;
    }

    public struct MAC
    {
        public string Name;
        public bool PWRDN;
        public bool init;
        public bool RST;
    }

    public partial class MainWindow : Window
    {
        public byte ISPRAV_AC;//переменная состояния светодиода "ИСПРАВ АЦ"
        public byte PROGR;
        public byte TEMP;
        public byte OTKL_AC;
        public byte ISPR_J330;
        public byte LS;
        public byte SINHR;
        public byte OFCH;
        public int VKL_12V=0;

        public CHANNEL CH1;
        public CHANNEL CH2;
        public CHANNEL CH3;
        public CHANNEL CH4;
        public CHANNEL CH5;
        public CHANNEL CH6;
        public CHANNEL CH7;
        public CHANNEL CH8;

        int MSG_CMD_OK = 3;//квитанция о том что команда выполненна
        int MSG_ID_CH1 = 101;
        int MSG_ID_CH2 = 102;
        int MSG_ID_CH3 = 103;
        int MSG_ID_CH4 = 104;
        int MSG_ID_CH5 = 105;
        int MSG_ID_CH6 = 106;
        int MSG_ID_CH7 = 107;
        int MSG_ID_CH8 = 108;

        const uint MSG_TEMP_CH1 = 111;
        const uint MSG_TEMP_CH2 = 112;
        const uint MSG_TEMP_CH3 = 113;
        const uint MSG_TEMP_CH4 = 114;
        const uint MSG_TEMP_CH5 = 115;
        const uint MSG_TEMP_CH6 = 116;
        const uint MSG_TEMP_CH7 = 117;
        const uint MSG_TEMP_CH8 = 118;

        const uint MSG_I_CH1 = 131;
        const uint MSG_I_CH2 = 132;
        const uint MSG_I_CH3 = 133;
        const uint MSG_I_CH4 = 134;
        const uint MSG_I_CH5 = 135;
        const uint MSG_I_CH6 = 136;
        const uint MSG_I_CH7 = 137;
        const uint MSG_I_CH8 = 138;

        const uint MSG_P_CH1 = 141;
        const uint MSG_P_CH2 = 142;
        const uint MSG_P_CH3 = 143;
        const uint MSG_P_CH4 = 144;
        const uint MSG_P_CH5 = 145;
        const uint MSG_P_CH6 = 146;
        const uint MSG_P_CH7 = 147;
        const uint MSG_P_CH8 = 148;

        const uint MSG_U_CH1 = 121;
        const uint MSG_U_CH2 = 122;
        const uint MSG_U_CH3 = 123;
        const uint MSG_U_CH4 = 124;
        const uint MSG_U_CH5 = 125;
        const uint MSG_U_CH6 = 126;
        const uint MSG_U_CH7 = 127;
        const uint MSG_U_CH8 = 128;

        const uint MSG_PWR_CHANNEL = 150;
       public uint FLAG_NEW_DATA = 0; //флаг что пришли новые данные о состоянии блока

        int N_STATE_MAX = 6+1;//максимальное число состояний стейт машины тестирования
        int FLAG_SYS_INIT;

        public MAC MAC0_072;//переменная состояния блока МАС ethernet в 072
        public MAC MAC1_072;//переменная состояния блока МАС ethernet в 072

        public SerialPort serialPort1 = new SerialPort();        
        static bool _continue;
        Byte[] RCV = new byte[64000];
        int sch_packet = 0;

        form_consol1 newForm;

        //----------ETH------------
        UdpClient _server = null;
        IPEndPoint _client = null;
        Thread _listenThread = null;
        private bool _isServerStarted = false;
        //-------------------eth-------------------------
        private void Start()
        {
            IPAddress my_ip;
            UInt16 my_port;

            my_ip = IPAddress.Parse(textBox_my_ip.Text);
            my_port = UInt16.Parse(textBox_my_port.Text);

            //Create the server.
            IPEndPoint serverEnd = new IPEndPoint(my_ip, my_port);
   
            _server = new UdpClient(serverEnd);
            _server.Client.ReceiveBufferSize = 8192 * 200;//увеличиваем размер приёмного буфера!!!
            Debug.WriteLine("Waiting for a client...");
            //Create the client end.
            //_client = new IPEndPoint(IPAddress.Any, 0);

            //Start listening.
            Thread listenThread = new Thread(new ThreadStart(Listening));
            listenThread.Start();
            listenThread.IsBackground = true;//делает поток фоновым который завершается по закрытию основного приложения
            //Change state to indicate the server starts.
            _isServerStarted = true;

            ////-----шлём приветствие программсе на си--------------
            //UdpClient client = new UdpClient();
            //client.Connect("127.0.0.1", 666);
            //string msg = "Hello bro!!!\n";
            //byte[] data = Encoding.UTF8.GetBytes(msg);
            //int number_bytes = client.Send(data, data.Length);
            //client.Close();
        }

        private void Stop()
        {
            try
            {
                //Stop listening.
                _listenThread.Join();
                Debug.WriteLine("Server stops.");
                _server.Close();
                //Changet state to indicate the server stops.
                _isServerStarted = false;
            }
            catch (Exception excp)
            { }
        }

        int FLAG_UDP_RCV = 0;
        int RCV_size=0;
        private void Listening()
        {
            byte[] data;
            //Listening loop.
            while (true)
            {
                //receieve a message form a client.
                data = _server.Receive(ref _client);
          //    Debug.WriteLine("*");
                if (FLAG_UDP_RCV == 0)
                {
             //     receivedMsg = Encoding.ASCII.GetString(data, 0, data.Length);
                    Array.Copy(data, RCV, data.Length);//копируем массив отсчётов в форму обработки    
                    RCV_size = data.Length;
             //     FLAG_UDP_RCV = 1;
                    UDP_BUF_DESCRIPT();
                }
                sch_packet++;
            }
        }

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer Timer2 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer Timer3 = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            byte[] a = new byte[4];
            InitializeComponent();
          
            Timer2.Tick += new EventHandler(Timer2_Tick);
            Timer2.Interval = new TimeSpan(0, 0, 0, 0, 250);
       
            Timer3.Tick += new EventHandler(Timer3_Tick);
            Timer3.Interval = new TimeSpan(0, 0, 0, 0,2000);
            newForm = new form_consol1("console1");         
            
        }

        private void button_comport_send_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);
 
            try
            {
                if (serialPort1.IsOpen == false)
                {
                    serialPort1.Open();
                }
                button_comport_send.Background = Brushes.Green;
                serialPort1.Write(textBox_comport_message.Text);
                //  serialPort1.Write(command2);
                // здесь может быть код еще...
            }
            catch (Exception ex)
            {
                // что-то пошло не так и упало исключение... Выведем сообщение исключения
                Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", serialPort1.PortName, ex.Message));
                button_comport_send.Background = myBrush;              
            }
        }
     

        private void button_comport_open_Click(object sender, RoutedEventArgs e)
        {
 
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);

            if (serialPort1.IsOpen==false)
            {
                // Allow the user to set the appropriate properties.
                serialPort1.PortName = textBox_comport.Text;
                serialPort1.BaudRate = 115200;
                serialPort1.DataReceived += OnDataReceived;

                // Set the read/write timeouts
                serialPort1.ReadTimeout = 500;
                serialPort1.WriteTimeout = 500;

                try
                {
                    if (serialPort1.IsOpen == false)
                    {
                        serialPort1.Open();
                    }
                    Debug.WriteLine("open");
                    button_comport_open.Content = "close";
                    button_comport_open.Background = Brushes.Green;
                    button_comport_send.Background = Brushes.Green;
                    // здесь может быть код еще...
                }
                catch (Exception ex)
                {
                    // что-то пошло не так и упало исключение... Выведем сообщение исключения
                    Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", serialPort1.PortName, ex.Message));
                    button_comport_open.Background = myBrush;
                }
            } else
            {
                serialPort1.Close();
                button_comport_open.Content = "open";
                button_comport_open.Background = Brushes.Coral;
            }
           
        }

 public   string console_text = "";
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var buffer = new byte[serialDevice.BytesToRead];
            serialDevice.Read(buffer, 0, buffer.Length);
            int i = 0;

            string z = Encoding.GetEncoding(1251).GetString(buffer);//чтобы видеть русский шрифт!!!

           // for (i = 0; i < buffer.Length; i++) z = z + Convert.ToString(buffer[i]);

            // process data on the GUI thread
            Application.Current.Dispatcher.Invoke(
                new Action(() => 
                {
                    //   Debug.WriteLine("чё-то принято!");
                    console_text = console_text + z;                   
                //    Debug.WriteLine(":"+ z);
                    /*
                ... do something here ...
                */
                }));
        }
        string config = "";
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string filename;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                config = File.ReadAllText(filename);
            }                
        }

        struct Command
        {
           public UInt32 Cmd_size;
           public UInt32 Cmd_type;
           public UInt64 Cmd_id;
           public UInt64 Cmd_time;
           public string Cmd_data;
           public byte [] A ;
        }

        struct Message
        {
            public UInt32 Msg_size;
            public UInt32 Msg_type;
            public UInt64 Num_cmd_in_msg;
            public Command CMD;
        }

        struct Frame
        {
            public UInt16 Frame_size;
            public UInt16 Frame_number;
            public UInt16 Stop_bit;
            public UInt32 Msg_uniq_id;
            public UInt64 Sender_id;
            public UInt64 Receiver_id;
            public Message MSG;
        }

        Frame FRAME;
        private void button_udp_init_072_Click(object sender, RoutedEventArgs e)
        {
            byte[] UDP_packet = new byte[1440];
            int DATA_lenght = 0;
            int i=0;
            string MSG = "";
            UInt64 sch_cmd = 0;
            MSG = config;

         // Timer1.Tick += new EventHandler(Timer1_Tick);
         // Timer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
         // Timer1.Start();//запускаю таймер проверяющий приём по UDP

            try 
            {
                FRAME.Frame_size         = Convert.ToUInt16(DATA(MSG, "Frame_size"  ));
                FRAME.Frame_number       = Convert.ToUInt16(DATA(MSG, "Frame_number"));
                FRAME.Stop_bit           = Convert.ToUInt16(DATA(MSG, "Stop_bit"    ));
                FRAME.Msg_uniq_id        = Convert.ToUInt32(DATA(MSG, "Msg_uniq_id" ));
                FRAME.Sender_id          = DATA(MSG, "Sender_id");
                FRAME.Receiver_id        = DATA(MSG, "Receiver_id");
                FRAME.MSG.Msg_size       = Convert.ToUInt32(DATA(MSG, "Msg_size"    ));
                FRAME.MSG.Msg_type       = Convert.ToUInt32(DATA(MSG, "Msg_type"    ));
                FRAME.MSG.Num_cmd_in_msg = DATA(MSG, "Num_cmd_in_msg");
                sch_cmd = FRAME.MSG.Num_cmd_in_msg;//считаем число команд в файле

                //-------------------фреймовая часть пакета-----------------------
                UDP_packet[0] = Convert.ToByte((FRAME.Frame_size >> 8) & 0xff);
                UDP_packet[1] = Convert.ToByte((FRAME.Frame_size >> 0) & 0xff);

                UDP_packet[2] = Convert.ToByte((FRAME.Frame_number >> 8) & 0xff);
                UDP_packet[3] = Convert.ToByte((FRAME.Frame_number >> 0) & 0xff);//

                UDP_packet[4]  = Convert.ToByte((FRAME.Msg_uniq_id >> 24) & 0xff);
                UDP_packet[5]  = Convert.ToByte((FRAME.Msg_uniq_id >> 16) & 0xff);
                UDP_packet[6] = Convert.ToByte((FRAME.Msg_uniq_id >> 8)  & 0xff);
                UDP_packet[7] = Convert.ToByte((FRAME.Msg_uniq_id >> 0)  & 0xff);

                UDP_packet[8]  = Convert.ToByte((FRAME.Sender_id >> 56) & 0xff);
                UDP_packet[9]  = Convert.ToByte((FRAME.Sender_id >> 48) & 0xff);
                UDP_packet[10] = Convert.ToByte((FRAME.Sender_id >> 40) & 0xff);
                UDP_packet[11] = Convert.ToByte((FRAME.Sender_id >> 32) & 0xff);
                UDP_packet[12] = Convert.ToByte((FRAME.Sender_id >> 24) & 0xff);
                UDP_packet[13] = Convert.ToByte((FRAME.Sender_id >> 16) & 0xff);
                UDP_packet[14] = Convert.ToByte((FRAME.Sender_id >> 8)  & 0xff);
                UDP_packet[15] = Convert.ToByte((FRAME.Sender_id >> 0)  & 0xff);

                UDP_packet[16] = Convert.ToByte((FRAME.Receiver_id >> 56) & 0xff);
                UDP_packet[17] = Convert.ToByte((FRAME.Receiver_id >> 48) & 0xff);
                UDP_packet[18] = Convert.ToByte((FRAME.Receiver_id >> 40) & 0xff);
                UDP_packet[19] = Convert.ToByte((FRAME.Receiver_id >> 32) & 0xff);
                UDP_packet[20] = Convert.ToByte((FRAME.Receiver_id >> 24) & 0xff);
                UDP_packet[21] = Convert.ToByte((FRAME.Receiver_id >> 16) & 0xff);
                UDP_packet[22] = Convert.ToByte((FRAME.Receiver_id >> 8) & 0xff);
                UDP_packet[23] = Convert.ToByte((FRAME.Receiver_id >> 0) & 0xff);

                UDP_packet[24] = Convert.ToByte((FRAME.MSG.Msg_size >> 24) & 0xff);
                UDP_packet[25] = Convert.ToByte((FRAME.MSG.Msg_size >> 16) & 0xff);
                UDP_packet[26] = Convert.ToByte((FRAME.MSG.Msg_size >> 8) & 0xff);
                UDP_packet[27] = Convert.ToByte((FRAME.MSG.Msg_size >> 0) & 0xff);

                UDP_packet[28] = Convert.ToByte((FRAME.MSG.Msg_type >> 24) & 0xff);
                UDP_packet[29] = Convert.ToByte((FRAME.MSG.Msg_type >> 16) & 0xff);
                UDP_packet[30] = Convert.ToByte((FRAME.MSG.Msg_type >> 8) & 0xff);
                UDP_packet[31] = Convert.ToByte((FRAME.MSG.Msg_type >> 0) & 0xff);

                UDP_packet[32] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 56) & 0xff);
                UDP_packet[33] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 48) & 0xff);
                UDP_packet[34] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 40) & 0xff);
                UDP_packet[35] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 32) & 0xff);
                UDP_packet[36] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 24) & 0xff);
                UDP_packet[37] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 16) & 0xff);
                UDP_packet[38] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 8)  & 0xff);
                UDP_packet[39] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 0)  & 0xff);
                //-----------------------------------------------------------------------------------------
                int j = 0;
                int pos1 = 0;
                DATA_lenght = 40;//это число байт из упаковки выше 39+1
                while (sch_cmd>0)
                {
                    FRAME.MSG.CMD.Cmd_size = Convert.ToUInt32(DATA(MSG, "Cmd_size"));
                    FRAME.MSG.CMD.Cmd_type = Convert.ToUInt32(DATA(MSG, "Cmd_type"));
                    FRAME.MSG.CMD.Cmd_id   = Convert.ToUInt64(DATA(MSG, "Cmd_id"  ));
                    FRAME.MSG.CMD.Cmd_time = Convert.ToUInt64(DATA(MSG, "Cmd_time"));

                    var value = data_finder(MSG, "Cmd_time", 0);//координаты разделителя для слова указанного в поиске 
                    FRAME.MSG.CMD.Cmd_data = MSG.Substring(value.Item2 + 3, Convert.ToInt32(FRAME.MSG.CMD.Cmd_size));
   //               Debug.WriteLine("Cmd_data:" + FRAME.MSG.CMD.Cmd_data);
                    pos1 = Convert.ToInt32(value.Item2 + 3 + FRAME.MSG.CMD.Cmd_size);

                    UDP_packet[40+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_size >> 24) & 0xff);
                    UDP_packet[41+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_size >> 16) & 0xff);
                    UDP_packet[42+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_size >> 8)  & 0xff);
                    UDP_packet[43+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_size >> 0)  & 0xff);

                    UDP_packet[44+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_type >> 24) & 0xff);
                    UDP_packet[45+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_type >> 16) & 0xff);
                    UDP_packet[46+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_type >> 8)  & 0xff);
                    UDP_packet[47+j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_type >> 0)  & 0xff);

                    UDP_packet[48 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 56) & 0xff);
                    UDP_packet[49 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 48) & 0xff);
                    UDP_packet[50 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 40) & 0xff);
                    UDP_packet[51 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 32) & 0xff);
                    UDP_packet[52 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 24) & 0xff);
                    UDP_packet[53 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 16) & 0xff);
                    UDP_packet[54 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 8)  & 0xff);
                    UDP_packet[55 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 0)  & 0xff);

                    UDP_packet[56 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 56) & 0xff);
                    UDP_packet[57 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 48) & 0xff);
                    UDP_packet[58 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 40) & 0xff);
                    UDP_packet[59 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 32) & 0xff);
                    UDP_packet[60 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 24) & 0xff);
                    UDP_packet[61 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 16) & 0xff);
                    UDP_packet[62 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 8)  & 0xff);
                    UDP_packet[63 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_time >> 0)  & 0xff);

                    for (i = 0; i < FRAME.MSG.CMD.Cmd_data.Length; i++) UDP_packet[64 + i + j] = Convert.ToByte(Convert.ToInt32(FRAME.MSG.CMD.Cmd_data[i])-0x30);

                    DATA_lenght = DATA_lenght + FRAME.MSG.CMD.Cmd_data.Length + 24;
                //  Debug.WriteLine("DATA_lenght:" + DATA_lenght);
                    MSG=MSG.Remove(0, pos1);//удаляем ранее обработанные данные из строки

                    // Debug.WriteLine("MSG:\r\n" + MSG);
    //                Debug.WriteLine("sch_cmd:" + sch_cmd);
                    sch_cmd--;
                    j=j+ FRAME.MSG.CMD.Cmd_data.Length + 24;
                }

                //-----шлём данные по UDP--------------
                string ip_dest = textBox_dest_ip.Text;
                int port_dest = Convert.ToInt32(textBox_dest_port.Text);

                UdpClient client = new UdpClient();
                                   client.Connect(ip_dest, port_dest);
                int number_bytes = client.Send(UDP_packet, DATA_lenght);
                //           Debug.WriteLine("DATA_lenght                  :" + DATA_lenght);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data.Length:" + FRAME.MSG.CMD.Cmd_data.Length);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data       :" + FRAME.MSG.CMD.Cmd_data);
                client.Close();
            }
            catch 
            {

            }
            
        }

        //---------------поиск строк в стринге-----------------
        public UInt64 DATA(string a,string b)
        {
            var value = data_finder(a,b, 0);
    //      Debug.WriteLine(b + " " + value.Item1);
            return value.Item1;
        }

        public Tuple<UInt64, int> data_finder(string MSG,string CMD, int pos)
        {
            int i = 0;
            int a1, a2;
            UInt64 data = 0;
            int index;
            i = pos;
            while ((MSG.Substring(i, CMD.Length) != CMD) && (i < (MSG.Length - 1)))   //ищем команду
            {
                i = i + 1;
            }
            
            i=i + CMD.Length;

            while (((MSG.Substring(i, 1) == " ") || (MSG.Substring(i, 1) == "\t"))&&(i < (MSG.Length - 1)))   //ищем ДАННЫЕ
            {
                i = i + 1;
            }
            a1 = i; 
            while ((MSG.Substring(i, 1) != ";") && (i < (MSG.Length - 1)))  //ищем первый разделиетель
            {
                i = i + 1;
            }
             a2 = i - a1;
      //    Debug.WriteLine("a1:" + a1);
      //    Debug.WriteLine("a2:" + a2);
            index = i;
      //    Debug.WriteLine(">" + MSG.Substring(a1, a2));
               
            try
            {
                data = Convert.ToUInt64 (MSG.Substring(a1, a2));
            }
            catch
            {
                MessageBox.Show("Введите правильные данные");
                index = 999999;
            }
            return Tuple.Create(data, index);
        }
        //-----------------------------------------------------

        public bool[] Panel_330_form = new bool[2];
    
        private void button_adc0_Click(object sender, RoutedEventArgs e)
        {
            if (Panel_330_form[0]==false)
            {
                Panel_330 newForm = new Panel_330("Панель 330",this);
                Panel_330_form[0] = true;
                newForm.Show();
                newForm.Owner = this;
            }            
        }

        public bool[] Console_form = new bool[1];

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
         //   Debug.WriteLine(":" + Console_form[0]);
            if (Console_form[0] == false)
            {
           //   newForm = new form_consol1("console1");
                newForm.Top = this.Top;
                newForm.Left = this.Left;
                newForm.Owner = this;            
                Console_form[0] = true;
                newForm.Show(); 
            }
        }

        public bool[] Panel_info_form = new bool[2];
        private void button_info_Click(object sender, RoutedEventArgs e)
        {
            if (Panel_info_form[0] == false)
            {
                Panel_INFO newForm = new Panel_INFO("Info", this);
                Panel_info_form[0] = true;
                newForm.Show();
                newForm.Owner = this;
            }
        }

        private void button_12V_vkl_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];

  
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);

            if (Convert.ToString(button_12V_vkl.Content)== "выкл +12V")
            {
                button_12V_vkl.Content = "вкл +12V";
                a[3] = 1;
                Timer2.Start();//запускаю таймер проверяющий приём по UDP
            } else
            {
                button_12V_vkl.Content = "выкл +12V";
                a[3] = 0;
 //              Timer2.Stop();//останавливаю таймер проверяющий приём по UDP
            }

            UDP_SEND(
                3,  //команда 3 - CMD_12V
                a,  //данные
                4,  //число данных в байтах
                0   //время исполнения 0 - значит сразу как сможешь
                );

            string command1 = " ~0 start:"+Convert.ToString(a[3])+"; ";

            try
            {
                if (serialPort1.IsOpen == false)
                {
                    serialPort1.Open();
                }
            //    Debug.WriteLine("шлём:" + command1);
            //  button_12V_vkl.Background = Brushes.Green;
                serialPort1.Write(command1);
                //  serialPort1.Write(command2);
                // здесь может быть код еще...
            }
            catch (Exception ex)
            {

                // что-то пошло не так и упало исключение... Выведем сообщение исключения
                Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", serialPort1.PortName, ex.Message));
            //    button_12V_vkl.Background = myBrush;
            //    button_comport_open.Background = myBrush;
            }
        }

        int FLAG_TIMER_1 = 0;
        private void Timer1_Tick(object sender, EventArgs e)
        {            
       
        }


        Frame MSG1 = new Frame();
     
 
        void UDP_BUF_DESCRIPT ()
        {
            int i = 0;
            int j = 0;
            int offset = 0;
            byte[] a = new byte[4];

            FLAG_NEW_DATA = 1;

            MSG1.MSG.CMD.A = new byte[4];
            //    Debug.WriteLine("------------------");
            MSG1.Frame_size         = Convert.ToUInt16((RCV[0] << 8) + RCV[1]);
            MSG1.Frame_number       = Convert.ToUInt16((RCV[2] << 8) + RCV[3]);
            MSG1.Stop_bit           = Convert.ToUInt16(1);
            MSG1.Msg_uniq_id        = Convert.ToUInt32((RCV[ 4] << 24) + (RCV[ 5] << 16) + (RCV[ 6] <<  8) + (RCV[7]  << 0));
            MSG1.Sender_id          = Convert.ToUInt64((RCV[ 8] << 56) + (RCV[ 9] << 48) + (RCV[10] << 40) + (RCV[11] << 32) + (RCV[12] << 24) + (RCV[13] << 16) + (RCV[14] << 8) + (RCV[15] << 0));
            MSG1.Receiver_id        = Convert.ToUInt64((RCV[16] << 56) + (RCV[17] << 48) + (RCV[18] << 40) + (RCV[19] << 32) + (RCV[20] << 24) + (RCV[21] << 16) + (RCV[22] << 8) + (RCV[23] << 0));
            MSG1.MSG.Msg_size       = Convert.ToUInt32((RCV[24] << 24) + (RCV[25] << 16) + (RCV[26] <<  8) + (RCV[27] << 0));
            MSG1.MSG.Msg_type       = Convert.ToUInt32((RCV[28] << 24) + (RCV[29] << 16) + (RCV[30] <<  8) + (RCV[31] << 0));
            MSG1.MSG.Num_cmd_in_msg = Convert.ToUInt64((RCV[32] << 56) + (RCV[33] << 48) + (RCV[34] << 40) + (RCV[35] << 32) + (RCV[36] << 24) + (RCV[37] << 16) + (RCV[38] << 8) + (RCV[39] << 0));
           
            /*
            Debug.WriteLine("    Frame_size:" + MSG1.Frame_size);
            Debug.WriteLine("  Frame_number:" + MSG1.Frame_number);
            Debug.WriteLine("   Msg_uniq_id:" + MSG1.Msg_uniq_id);
            Debug.WriteLine("     Sender_id:" + MSG1.Sender_id);
            Debug.WriteLine("   Receiver_id:" + MSG1.Receiver_id);
            Debug.WriteLine("      Msg_size:" + MSG1.MSG.Msg_size);
            Debug.WriteLine("      Msg_type:" + MSG1.MSG.Msg_type);
            Debug.WriteLine("Num_cmd_in_msg:" + MSG1.MSG.Num_cmd_in_msg);
            */

            offset = 40;

            for (i=0;i<Convert.ToInt32(MSG1.MSG.Num_cmd_in_msg);i++)
            {
                MSG1.MSG.CMD.Cmd_size = Convert.ToUInt32((RCV[offset + 0] << 24) + (RCV[offset + 1] << 16) + (RCV[offset +  2] <<  8) + (RCV[offset +  3] << 0));
                MSG1.MSG.CMD.Cmd_type = Convert.ToUInt32((RCV[offset + 4] << 24) + (RCV[offset + 5] << 16) + (RCV[offset +  6] <<  8) + (RCV[offset +  7] << 0));
                MSG1.MSG.CMD.Cmd_id   = Convert.ToUInt64((RCV[offset + 8] << 56) + (RCV[offset + 9] << 48) + (RCV[offset + 10] << 40) + (RCV[offset + 11] << 32) + (RCV[offset + 12] << 24) + (RCV[offset + 13] << 16) + (RCV[offset + 14] << 8) + (RCV[offset + 15] << 0));
                MSG1.MSG.CMD.Cmd_time = Convert.ToUInt64((RCV[offset +16] << 56) + (RCV[offset +17] << 48) + (RCV[offset + 18] << 40) + (RCV[offset + 19] << 32) + (RCV[offset + 20] << 24) + (RCV[offset + 21] << 16) + (RCV[offset + 22] << 8) + (RCV[offset + 23] << 0));

                if (MSG1.MSG.CMD.Cmd_type== MSG_CMD_OK)//если есть квитанция MSG_CMD_OK - "команда выполненна успешно"
                {
                    FLAG_TIMER_1 = 0;//сбрасываем счётчик таймера ожидания обратной связи с блоком по сети эзернет
                    FLAG_STATUS  = false;
                }

                //Debug.WriteLine("Cmd_size:" + MSG1.MSG.CMD.Cmd_size);
                //Debug.WriteLine("Cmd_type:" + MSG1.MSG.CMD.Cmd_type);
                //Debug.WriteLine("Cmd_id  :" + MSG1.MSG.CMD.Cmd_id);
                //Debug.WriteLine("Cmd_time:" + MSG1.MSG.CMD.Cmd_time);                
                //---------------------------------------------------------------------------------
                ID_channel(offset);
                //----------------------------------------------------------------------------------
 
                    for (j=0;j<Convert.ToInt32(MSG1.MSG.CMD.Cmd_size);j++)
                    {
                        MSG1.MSG.CMD.Cmd_data = Convert.ToString(RCV[offset + 24 + j]);
                        MSG1.MSG.CMD.A[j] = RCV[offset + 24 + j];
                        a[3-j] = RCV[offset + 24 + j];
                    //if (MSG1.MSG.CMD.Cmd_type == MSG_TEMP_CH1)  Debug.WriteLine("A[j]:" + MSG1.MSG.CMD.A[j]);
                }

                switch (MSG1.MSG.CMD.Cmd_type)
                {                  

                    case MSG_TEMP_CH1: CH1.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH2: CH2.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH3: CH3.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH4: CH4.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH5: CH5.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH6: CH6.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH7: CH7.T = BitConverter.ToInt32(a, 0); break;
                    case MSG_TEMP_CH8: CH8.T = BitConverter.ToInt32(a, 0); break;
              
                    case MSG_I_CH1: CH1.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH2: CH2.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH3: CH3.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH4: CH4.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH5: CH5.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH6: CH6.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH7: CH7.I = BitConverter.ToInt32(a, 0); break;
                    case MSG_I_CH8: CH8.I = BitConverter.ToInt32(a, 0); break;
              
                    case MSG_P_CH1: CH1.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH2: CH2.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH3: CH3.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH4: CH4.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH5: CH5.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH6: CH6.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH7: CH7.P = BitConverter.ToInt32(a, 0); break;
                    case MSG_P_CH8: CH8.P = BitConverter.ToInt32(a, 0); break;
              
                    case MSG_U_CH1: CH1.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH2: CH2.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH3: CH3.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH4: CH4.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH5: CH5.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH6: CH6.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH7: CH7.U = BitConverter.ToInt32(a, 0); break;
                    case MSG_U_CH8: CH8.U = BitConverter.ToInt32(a, 0); break;

                    case MSG_PWR_CHANNEL: 
                        CH8.PWR = ((BitConverter.ToInt32(a, 0)) >> 0) & 1;
                        CH7.PWR = ((BitConverter.ToInt32(a, 0)) >> 1) & 1;
                        CH6.PWR = ((BitConverter.ToInt32(a, 0)) >> 2) & 1;
                        CH5.PWR = ((BitConverter.ToInt32(a, 0)) >> 3) & 1;
                        CH4.PWR = ((BitConverter.ToInt32(a, 0)) >> 4) & 1;
                        CH3.PWR = ((BitConverter.ToInt32(a, 0)) >> 5) & 1;
                        CH2.PWR = ((BitConverter.ToInt32(a, 0)) >> 6) & 1;
                        CH1.PWR = ((BitConverter.ToInt32(a, 0)) >> 7) & 1;
                        VKL_12V = ((BitConverter.ToInt32(a, 0)) >> 8) & 1;

          //              Debug.WriteLine("PWR    :" + BitConverter.ToInt32(a, 0));
          //              Debug.WriteLine("CH8.PWR:" + CH8.PWR);
          //              Debug.WriteLine("CH7.PWR:" + CH7.PWR);
           //             Debug.WriteLine("CH6.PWR:" + CH6.PWR);
          //              Debug.WriteLine("CH5.PWR:" + CH5.PWR);
          //              Debug.WriteLine("CH4.PWR:" + CH4.PWR);
          //              Debug.WriteLine("CH3.PWR:" + CH3.PWR);
          //              Debug.WriteLine("CH2.PWR:" + CH2.PWR);
          //              Debug.WriteLine("CH1.PWR:" + CH1.PWR);
                        break;                       
                }
          
                // TEMP_channel(MSG1.MSG.CMD.A);

                offset = offset + 24 + j;
            }           
        }


        void ID_channel (int offset)
        {
            int j = 0;
            byte[] a = new byte[4];

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH1)
            {
                CH1.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH1.ID = CH1.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH2)
            {
                CH2.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH2.ID = CH2.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH3)
            {
                CH3.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH3.ID = CH3.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH4)
            {
                CH4.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH4.ID = CH4.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH5)
            {
                CH5.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH5.ID = CH5.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH6)
            {
                CH6.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH6.ID = CH6.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH7)
            {
                CH7.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH7.ID = CH7.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }

            if (MSG1.MSG.CMD.Cmd_type == MSG_ID_CH8)
            {
                CH8.ID = "";

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)
                {
                    a[0] = RCV[offset + 24 + j];
                    CH8.ID = CH8.ID + " " + a[0].ToString("X");
                }
                //Debug.WriteLine("CH1.ID:" + CH1.ID);
            }
        }
        UInt32 CMD_ID = 0;
   public   void UDP_SEND (uint CMD_type,byte [] CMD_data, uint CMD_size,ulong CMD_time)
        {
            byte[] UDP_packet = new byte[1440];
            int DATA_lenght = 0;
            int i = 0;

            UInt64 sch_cmd = 0;
            try
            {
                FRAME.Frame_size         = 0;
                FRAME.Frame_number       = 0;
                FRAME.Stop_bit           = 1;
                FRAME.Msg_uniq_id        = 1;
                FRAME.Sender_id          = 1;
                FRAME.Receiver_id        = 2;
                FRAME.MSG.Msg_size       = 10;
                FRAME.MSG.Msg_type       = 1;
                FRAME.MSG.Num_cmd_in_msg = 1;
                sch_cmd = 1;//считаем число команд в файле

                //-------------------фреймовая часть пакета-----------------------
                UDP_packet[0] = Convert.ToByte((FRAME.Frame_size >> 8) & 0xff);
                UDP_packet[1] = Convert.ToByte((FRAME.Frame_size >> 0) & 0xff);

                UDP_packet[2] = Convert.ToByte((FRAME.Frame_number >> 8) & 0xff);
                UDP_packet[3] = Convert.ToByte((FRAME.Frame_number >> 0) & 0xff);//

                UDP_packet[4] = Convert.ToByte((FRAME.Msg_uniq_id >> 24) & 0xff);
                UDP_packet[5] = Convert.ToByte((FRAME.Msg_uniq_id >> 16) & 0xff);
                UDP_packet[6] = Convert.ToByte((FRAME.Msg_uniq_id >> 8) & 0xff);
                UDP_packet[7] = Convert.ToByte((FRAME.Msg_uniq_id >> 0) & 0xff);

                UDP_packet[8] = Convert.ToByte((FRAME.Sender_id >> 56) & 0xff);
                UDP_packet[9] = Convert.ToByte((FRAME.Sender_id >> 48) & 0xff);
                UDP_packet[10] = Convert.ToByte((FRAME.Sender_id >> 40) & 0xff);
                UDP_packet[11] = Convert.ToByte((FRAME.Sender_id >> 32) & 0xff);
                UDP_packet[12] = Convert.ToByte((FRAME.Sender_id >> 24) & 0xff);
                UDP_packet[13] = Convert.ToByte((FRAME.Sender_id >> 16) & 0xff);
                UDP_packet[14] = Convert.ToByte((FRAME.Sender_id >> 8) & 0xff);
                UDP_packet[15] = Convert.ToByte((FRAME.Sender_id >> 0) & 0xff);

                UDP_packet[16] = Convert.ToByte((FRAME.Receiver_id >> 56) & 0xff);
                UDP_packet[17] = Convert.ToByte((FRAME.Receiver_id >> 48) & 0xff);
                UDP_packet[18] = Convert.ToByte((FRAME.Receiver_id >> 40) & 0xff);
                UDP_packet[19] = Convert.ToByte((FRAME.Receiver_id >> 32) & 0xff);
                UDP_packet[20] = Convert.ToByte((FRAME.Receiver_id >> 24) & 0xff);
                UDP_packet[21] = Convert.ToByte((FRAME.Receiver_id >> 16) & 0xff);
                UDP_packet[22] = Convert.ToByte((FRAME.Receiver_id >> 8) & 0xff);
                UDP_packet[23] = Convert.ToByte((FRAME.Receiver_id >> 0) & 0xff);

                UDP_packet[24] = Convert.ToByte((FRAME.MSG.Msg_size >> 24) & 0xff);
                UDP_packet[25] = Convert.ToByte((FRAME.MSG.Msg_size >> 16) & 0xff);
                UDP_packet[26] = Convert.ToByte((FRAME.MSG.Msg_size >> 8) & 0xff);
                UDP_packet[27] = Convert.ToByte((FRAME.MSG.Msg_size >> 0) & 0xff);

                UDP_packet[28] = Convert.ToByte((FRAME.MSG.Msg_type >> 24) & 0xff);
                UDP_packet[29] = Convert.ToByte((FRAME.MSG.Msg_type >> 16) & 0xff);
                UDP_packet[30] = Convert.ToByte((FRAME.MSG.Msg_type >> 8) & 0xff);
                UDP_packet[31] = Convert.ToByte((FRAME.MSG.Msg_type >> 0) & 0xff);

                UDP_packet[32] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 56) & 0xff);
                UDP_packet[33] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 48) & 0xff);
                UDP_packet[34] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 40) & 0xff);
                UDP_packet[35] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 32) & 0xff);
                UDP_packet[36] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 24) & 0xff);
                UDP_packet[37] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 16) & 0xff);
                UDP_packet[38] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 8) & 0xff);
                UDP_packet[39] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 0) & 0xff);
                //-----------------------------------------------------------------------------------------
                int j = 0;
                DATA_lenght = 40;//это число байт из упаковки выше 39+1
                while (sch_cmd > 0)
                {
                    FRAME.MSG.CMD.Cmd_size = Convert.ToUInt16(1 + CMD_data.Length);
                    FRAME.MSG.CMD.Cmd_id   = CMD_ID++;
 
                    UDP_packet[40 + j] = Convert.ToByte((CMD_size >> 24) & 0xff);
                    UDP_packet[41 + j] = Convert.ToByte((CMD_size >> 16) & 0xff);
                    UDP_packet[42 + j] = Convert.ToByte((CMD_size >> 8) & 0xff);
                    UDP_packet[43 + j] = Convert.ToByte((CMD_size >> 0) & 0xff);

                    UDP_packet[44 + j] = Convert.ToByte((CMD_type >> 24) & 0xff);
                    UDP_packet[45 + j] = Convert.ToByte((CMD_type >> 16) & 0xff);
                    UDP_packet[46 + j] = Convert.ToByte((CMD_type >> 8) & 0xff);
                    UDP_packet[47 + j] = Convert.ToByte((CMD_type >> 0) & 0xff);

                    UDP_packet[48 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 56) & 0xff);
                    UDP_packet[49 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 48) & 0xff);
                    UDP_packet[50 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 40) & 0xff);
                    UDP_packet[51 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 32) & 0xff);
                    UDP_packet[52 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 24) & 0xff);
                    UDP_packet[53 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 16) & 0xff);
                    UDP_packet[54 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 8) & 0xff);
                    UDP_packet[55 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 0) & 0xff);

                    UDP_packet[56 + j] = Convert.ToByte((CMD_time >> 56) & 0xff);
                    UDP_packet[57 + j] = Convert.ToByte((CMD_time >> 48) & 0xff);
                    UDP_packet[58 + j] = Convert.ToByte((CMD_time >> 40) & 0xff);
                    UDP_packet[59 + j] = Convert.ToByte((CMD_time >> 32) & 0xff);
                    UDP_packet[60 + j] = Convert.ToByte((CMD_time >> 24) & 0xff);
                    UDP_packet[61 + j] = Convert.ToByte((CMD_time >> 16) & 0xff);
                    UDP_packet[62 + j] = Convert.ToByte((CMD_time >> 8) & 0xff);
                    UDP_packet[63 + j] = Convert.ToByte((CMD_time >> 0) & 0xff);

                    for (i = 0; i < CMD_size; i++) UDP_packet[64 + i + j] = CMD_data[i];

                    DATA_lenght = DATA_lenght + Convert.ToInt32(CMD_size) + 24;

                    sch_cmd--;
                    j = j + Convert.ToInt32(CMD_size) + 24;
                }

                FRAME.Frame_size = Convert.ToUInt16(DATA_lenght);
                UDP_packet[0] = Convert.ToByte((FRAME.Frame_size >> 8) & 0xff);
                UDP_packet[1] = Convert.ToByte((FRAME.Frame_size >> 0) & 0xff);


                //-----шлём данные по UDP--------------
                string ip_dest = textBox_dest_ip.Text;
                int port_dest = Convert.ToInt32(textBox_dest_port.Text);

                UdpClient client = new UdpClient();
                client.Connect(ip_dest, port_dest);
                int number_bytes = client.Send(UDP_packet, DATA_lenght);
                //           Debug.WriteLine("DATA_lenght                  :" + DATA_lenght);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data.Length:" + FRAME.MSG.CMD.Cmd_data.Length);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data       :" + FRAME.MSG.CMD.Cmd_data);
                client.Close();
       
            }
            catch
            {

            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {//-------------Тут по таймеру шлём запрос состояния блока по UDP---------------------

            if (FLAG_TIMER_1 > 2)
            {
                button_udp_init_072.Background = new SolidColorBrush(Colors.Red);
                FLAG_SYS_INIT = 0;//нет связи с блоком 
            }
            else
            {
                if (button_udp_init_072.Background!= Brushes.Green) button_udp_init_072.Background = Brushes.Green;
                if (VKL_12V==1) button_12V_vkl.Content="вкл +12V";
                else            button_12V_vkl.Content="выкл +12V";
                FLAG_SYS_INIT = 1;//есть связь с блоком , можно запрашивать состояние
            }
            FLAG_TIMER_1++;

            byte[] a = new byte[1];
         
            a[0] = Convert.ToByte(100);    
            UDP_SEND(
                100,//команда 100 - CMD_STATUS
                a,  //данные
                1,  //число данных в байтах
                0   //время исполнения 0 - значит сразу как сможешь
                );
              
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {

        }

        bool TEST;
        private void button_START_click(object sender, RoutedEventArgs e)
        {
            byte[] ARRAY_data = new byte[4];
            byte CMD = 0;//код команды
            byte LENGTH_DATA = 0;//количество данных
            byte TIME_CMD = 0;//время начала исполнения 0 - кактолько так сразу!
            ERROR_SCH = 0;   //счётчик ошибок
            label_INFO.Content = "";
            string curTimeLong = DateTime.Now.ToLongTimeString();

            if (_isServerStarted==false)
            {
                Start();//запускаю сервер UDP
                UDP_SEND(100, ARRAY_data, 4, 0);
            }

            if (Convert.ToString(button_START.Content)=="START")
            {
     //           label_state.Visibility = Visibility.Visible;
     //           label_state.Content = "";
                TEST = true;
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 1700); //это надо чтобы успел позеленеть индикатор температуры, а то сбрасываются все светодиоды при тесте!
                newForm.Clear_data();
                button_START.Content = "STOP";
                console_text = "";
                Timer2.Start();//запускаю таймер проверяющий приём по UDP
                Timer3.Start();//запускаем таймер для отправки последовательности команд в блок
                STATE_PROCESS = 1;
                FLAG_STATUS = true;
                FLAG_TRX = true;
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду: ВКЛ +12Вольт !\r";
                CMD = 3;//команда 3 - CMD_12V
                LENGTH_DATA = 4;//число данных в байтах
                ARRAY_data[3] = 1;//данные
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
               
            } else
            {
    //            label_state.Visibility = Visibility.Hidden;
                TEST = false;
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 500);
                Timer3.Start();//запускаем таймер для отправки последовательности команд в блок
                STATE_PROCESS = N_STATE_MAX;//так как не надо запускать стейт машину, а надо только проверить что пришло подтверждение на команду
                button_START.Content = "START";
                FLAG_STATUS = true;
                FLAG_TRX = true;
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду: ВЫКЛ +12Вольт !\r";
                CMD = 3;//команда 3 - CMD_12V
                LENGTH_DATA = 4;//число данных в байтах
                ARRAY_data[3] = 0;//данные
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            }

        }

        byte [] tca_convert() //переводим значения переменныйх в вид удобный для программирования микросхемы i2c - TCA
        {
            byte[] a = new byte[4];
            int z;
            z = ISPRAV_AC + ((PROGR & 0x3) << 4) + ((OFCH    & 0x3) << 21) + ((SINHR     & 0x3) << 18) +
                                                   ((LS      & 0x3) << 16) + ((ISPR_J330 & 0x3) << 14) +
                                                   ((OTKL_AC & 0x1) << 12) + ((TEMP      & 0x3) << 8);

            a[0] = Convert.ToByte((z >> 24) & 0xff);
            a[1] = Convert.ToByte((z >> 16) & 0xff);
            a[2] = Convert.ToByte((z >> 8)  & 0xff);
            a[3] = Convert.ToByte((z >> 0)  & 0xff);

            return a;
        }

        int FLAG_TIMER_3  = 0;
        int STATE_PROCESS = 0;
        bool FLAG_STATUS;
        bool FLAG_TRX;
        int ERROR_SCH = 0;


        void STATE_MASHINE (int STATE)
        {
            int z = 0;
            byte[] ARRAY_data = new byte[4];
            byte CMD         = 0;//код команды
            byte LENGTH_DATA = 0;//количество данных
            byte TIME_CMD    = 0;//время начала исполнения 0 - кактолько так сразу!
            string curTimeLong = DateTime.Now.ToLongTimeString();

            FLAG_STATUS = true;//поднимаем флаг запроса! Квитанция должны его скинуть
            FLAG_TRX    = true;
         //   label_state.Content = STATE_PROCESS.ToString();
            if (STATE == 1)//зажечь все светодиоды на панели
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:включить красные светодиоды !\r";                

                ISPRAV_AC   = 2;
                PROGR       = 2;
                OFCH        = 2;
                LS          = 2;
                OTKL_AC     = 2;
                SINHR       = 2;
                ISPR_J330   = 2;
                TEMP        = 2;
                Timer3.Interval = new TimeSpan(0,0,0,0,2000);
                ARRAY_data = tca_convert();
                CMD = 200;        //команда 200 - CMD_LED
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            } else
                if (STATE == 2)//зажечь все светодиоды на панели
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:включить зелёные светодиоды.\r";

                ISPRAV_AC   = 1;
                PROGR       = 1;
                OFCH        = 1;
                LS          = 1;
                OTKL_AC     = 1;
                SINHR       = 1;
                ISPR_J330   = 1;
                TEMP        = 1;
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                ARRAY_data = tca_convert();
                CMD = 200;        //команда 200 - CMD_LED
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            } else
                if (STATE == 3)//Проверить напряжение в каналах
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:проверить напряжение в каналах.\r";
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 500);
                CMD = 100;        //команда 100 - CMD_STATUS
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            } else
            if (STATE == 4)//Проверить ID контроллеров LM
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:проверить ID контроллеров LM.\r";
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 500);
                CMD = 100;        //команда 100 - CMD_STATUS
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            } else
            if (STATE == 5)//Проверить управление каналами питания - выключить каналы питания
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:выключить каналы питания.\r";
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 1500);
                ARRAY_data[3] = 0xff;
                CMD = 4;        //команда 4 - CMD_CH_UP
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            }
            else
            if (STATE == 6)//Проверить управление каналами питания - включить каналы питания
            {
                console_text = console_text + $"[{curTimeLong}] " + "Отправлем команду:включить каналы питания.\r";
                Timer3.Interval = new TimeSpan(0, 0, 0, 0, 500);
                ARRAY_data[3] = 0x00;
                CMD = 4;        //команда 4 - CMD_CH_UP
                LENGTH_DATA = 4;  //число данных в байтах
                TIME_CMD = 0;
                UDP_SEND(CMD, ARRAY_data, LENGTH_DATA, TIME_CMD);
            }


        }

        private void Timer3_Tick(object sender, EventArgs e)
        {//-------------Тут по таймеру шлём запрос состояния блока по UDP---------------------
            string curTimeLong = DateTime.Now.ToLongTimeString();
           
            if (FLAG_STATUS==true)
            {               
                console_text = console_text +$"[{curTimeLong}] "+"ОШИБКА ОБМЕНА!\r";
                ERROR_SCH++;
                FLAG_STATUS = false;
                FLAG_TRX    = false;
            } else
            {
                if (FLAG_TRX == true)
                {
                    console_text = console_text + $"[{curTimeLong}] " + "OK.\r";
                    FLAG_TRX = false;
                }
            }
            STATE_MASHINE(STATE_PROCESS);
            if (STATE_PROCESS!= N_STATE_MAX) STATE_PROCESS++; //делаем на одно состояние стейт машины больше чтобы получить все ответы!!
            else
            {
                //MessageBox.Show("Блок отлично работает!");
                if (TEST==true)
                {
                    
                    if (ERROR_SCH == 0) label_INFO.Content = "ТЕСТЫ ПРОЙДЕНЫ УСПЕШНО";
                    else                label_INFO.Content = "     ТЕСТЫ НЕ ПРОЙДЕНЫ!";
                }                
                Timer3.Stop();
            }
        }
    }
}
