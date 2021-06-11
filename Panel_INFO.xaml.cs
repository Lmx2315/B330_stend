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
using System.Windows.Shapes;
using System.ComponentModel; // CancelEventArgs
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace stnd_72_v2
{
    /// <summary>
    /// Логика взаимодействия для Panel_INFO.xaml
    /// </summary>
    public partial class Panel_INFO : Window
    {
        string Name_this = "";
        public Config_B330 cfg = new Config_B330();//тут храним конфигурацию , будем брать её из файла
        public CcfgCMD_MSG_330 cfg_CMD_MSG = new CcfgCMD_MSG_330();//тут храним конфигурацию команд протокола управления блоком питания
        public Panel_INFO(string a, MainWindow main)
        {
            InitializeComponent();
            this.Name_this = a;
            this.Title = a;

            if (main != null)
            {
    //            Debug.WriteLine("Name_this:" + Name_this);
                if (Name_this == "Info")
                {
    //                Debug.WriteLine(":" + main.ISPRAV_AC);                    
                }
            }

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer1.Start();//запускаю таймер проверяющий приём по UDP

            CFG_load();
        }

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();

        private void Timer1_Tick(object sender, EventArgs e)
        {//-------------Тут по таймеру шлём запрос состояния блока по UDP---------------------
            MainWindow main = this.Owner as MainWindow;
            if ((main != null)&&(main.FLAG_NEW_DATA == 1))
            {
                txt_ID_ch1.Text = main.CH1.ID;
                txt_ID_ch2.Text = main.CH2.ID;
                txt_ID_ch3.Text = main.CH3.ID;
                txt_ID_ch4.Text = main.CH4.ID;
                txt_ID_ch5.Text = main.CH5.ID;
                txt_ID_ch6.Text = main.CH6.ID;
                txt_ID_ch7.Text = main.CH7.ID;
                txt_ID_ch8.Text = main.CH8.ID;

                txt_TEMP1.Text = Convert.ToString(Convert.ToDouble(main.CH1.T) / 100) + " °С";
                txt_TEMP2.Text = Convert.ToString(Convert.ToDouble(main.CH2.T) / 100) + " °С";
                txt_TEMP3.Text = Convert.ToString(Convert.ToDouble(main.CH3.T) / 100) + " °С";
                txt_TEMP4.Text = Convert.ToString(Convert.ToDouble(main.CH4.T) / 100) + " °С";
                txt_TEMP5.Text = Convert.ToString(Convert.ToDouble(main.CH5.T) / 100) + " °С";
                txt_TEMP6.Text = Convert.ToString(Convert.ToDouble(main.CH6.T) / 100) + " °С";
                txt_TEMP7.Text = Convert.ToString(Convert.ToDouble(main.CH7.T) / 100) + " °С";
                txt_TEMP8.Text = Convert.ToString(Convert.ToDouble(main.CH8.T) / 100) + " °С";

                txt_I_ch1.Text  = Convert.ToString(Convert.ToDouble(main.CH1.I)/ 100) + " А";
                txt_I_ch2.Text = Convert.ToString(Convert.ToDouble(main.CH2.I) / 100) + " А";
                txt_I_ch3.Text = Convert.ToString(Convert.ToDouble(main.CH3.I) / 100) + " А";
                txt_I_ch4.Text = Convert.ToString(Convert.ToDouble(main.CH4.I) / 100) + " А";
                txt_I_ch5.Text = Convert.ToString(Convert.ToDouble(main.CH5.I) / 100) + " А";
                txt_I_ch6.Text = Convert.ToString(Convert.ToDouble(main.CH6.I) / 100) + " А";
                txt_I_ch7.Text = Convert.ToString(Convert.ToDouble(main.CH7.I) / 100) + " А";
                txt_I_ch8.Text = Convert.ToString(Convert.ToDouble(main.CH8.I) / 100) + " А";

                txt_P_ch1.Text = Convert.ToString(Convert.ToDouble(main.CH1.P) / 100) + " Вт";
                txt_P_ch2.Text = Convert.ToString(Convert.ToDouble(main.CH2.P) / 100) + " Вт";
                txt_P_ch3.Text = Convert.ToString(Convert.ToDouble(main.CH3.P) / 100) + " Вт";
                txt_P_ch4.Text = Convert.ToString(Convert.ToDouble(main.CH4.P) / 100) + " Вт";
                txt_P_ch5.Text = Convert.ToString(Convert.ToDouble(main.CH5.P) / 100) + " Вт";
                txt_P_ch6.Text = Convert.ToString(Convert.ToDouble(main.CH6.P) / 100) + " Вт";
                txt_P_ch7.Text = Convert.ToString(Convert.ToDouble(main.CH7.P) / 100) + " Вт";
                txt_P_ch8.Text = Convert.ToString(Convert.ToDouble(main.CH8.P) / 100) + " Вт";

                txt_U_ch1.Text = Convert.ToString(Convert.ToDouble(main.CH1.U) / 100) + " В";
                txt_U_ch2.Text = Convert.ToString(Convert.ToDouble(main.CH2.U) / 100) + " В";
                txt_U_ch3.Text = Convert.ToString(Convert.ToDouble(main.CH3.U) / 100) + " В";
                txt_U_ch4.Text = Convert.ToString(Convert.ToDouble(main.CH4.U) / 100) + " В";
                txt_U_ch5.Text = Convert.ToString(Convert.ToDouble(main.CH5.U) / 100) + " В";
                txt_U_ch6.Text = Convert.ToString(Convert.ToDouble(main.CH6.U) / 100) + " В";
                txt_U_ch7.Text = Convert.ToString(Convert.ToDouble(main.CH7.U) / 100) + " В";
                txt_U_ch8.Text = Convert.ToString(Convert.ToDouble(main.CH8.U) / 100) + " В";

              
                if (main.CH1.PWR == 1) btn_ch1.Content = "выкл"; else btn_ch1.Content = "вкл";
                if (main.CH2.PWR == 1) btn_ch2.Content = "выкл"; else btn_ch2.Content = "вкл";
                if (main.CH3.PWR == 1) btn_ch3.Content = "выкл"; else btn_ch3.Content = "вкл";
                if (main.CH4.PWR == 1) btn_ch4.Content = "выкл"; else btn_ch4.Content = "вкл";
                if (main.CH5.PWR == 1) btn_ch5.Content = "выкл"; else btn_ch5.Content = "вкл";
                if (main.CH6.PWR == 1) btn_ch6.Content = "выкл"; else btn_ch6.Content = "вкл";
                if (main.CH7.PWR == 1) btn_ch7.Content = "выкл"; else btn_ch7.Content = "вкл";
                if (main.CH8.PWR == 1) btn_ch8.Content = "выкл"; else btn_ch8.Content = "вкл";

                if (main.K_corr_I!=null)
                {
                    txt_I_ch1_corr.Text = main.K_corr_I[0].ToString();
                    txt_I_ch2_corr.Text = main.K_corr_I[1].ToString();
                    txt_I_ch3_corr.Text = main.K_corr_I[2].ToString();
                    txt_I_ch4_corr.Text = main.K_corr_I[3].ToString();
                    txt_I_ch5_corr.Text = main.K_corr_I[4].ToString();
                    txt_I_ch6_corr.Text = main.K_corr_I[5].ToString();
                    txt_I_ch7_corr.Text = main.K_corr_I[6].ToString();
                    txt_I_ch8_corr.Text = main.K_corr_I[7].ToString();
                    main.K_corr_I = null;
                }

                if (main.K_corr_U!=null)
                {
                    txt_U_ch1_corr.Text = main.K_corr_U[0].ToString();
                    txt_U_ch2_corr.Text = main.K_corr_U[1].ToString();
                    txt_U_ch3_corr.Text = main.K_corr_U[2].ToString();
                    txt_U_ch4_corr.Text = main.K_corr_U[3].ToString();
                    txt_U_ch5_corr.Text = main.K_corr_U[4].ToString();
                    txt_U_ch6_corr.Text = main.K_corr_U[5].ToString();
                    txt_U_ch7_corr.Text = main.K_corr_U[6].ToString();
                    txt_U_ch8_corr.Text = main.K_corr_U[7].ToString();
                    main.K_corr_U = null;
                }

                main.FLAG_NEW_DATA = 0;
            }
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            // MessageBox.Show("Closing called");
            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (Name_this == "Info") main.Panel_info_form[0] = false;
            }
        }

        void ZXC ()
        {
            MainWindow main = this.Owner as MainWindow;

            main.CH1.OFF = main.CH1.PWR;
            main.CH2.OFF = main.CH2.PWR;
            main.CH3.OFF = main.CH3.PWR;
            main.CH4.OFF = main.CH4.PWR;
            main.CH5.OFF = main.CH5.PWR;
            main.CH6.OFF = main.CH6.PWR;
            main.CH7.OFF = main.CH7.PWR;
            main.CH8.OFF = main.CH8.PWR;
        }
        private void btn_ch1_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//

            if (Convert.ToString(btn_ch1.Content)=="вкл")
            {
                btn_ch1.Content = "выкл";
                if (main != null)
                {
                    main.CH1.OFF = 1;
                 }
            } else
            {
                btn_ch1.Content = "вкл";
                if (main != null)
                {
                    main.CH1.OFF = 0;               
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF<<7)|(main.CH2.OFF << 6)|(main.CH3.OFF << 5)|(main.CH4.OFF << 4)| (main.CH5.OFF << 3)| (main.CH6.OFF << 2)|(main.CH7.OFF << 1)| (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn1:" + a[3]);
        }       

        private void btn_ch2_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch2.Content) == "вкл")
            {
                btn_ch2.Content = "выкл";
                if (main != null)
                {
                    main.CH2.OFF = 1;
                }
            }
            else
            {
                btn_ch2.Content = "вкл";
                if (main != null)
                {
                    main.CH2.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn2:" + a[3]);
        }

        private void btn_ch3_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch3.Content) == "вкл")
            {
                btn_ch3.Content = "выкл";
                if (main != null)
                {
                    main.CH3.OFF = 1;
                }
            }
            else
            {
                btn_ch3.Content = "вкл";
                if (main != null)
                {
                    main.CH3.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn3:" + a[3]);
        }

        private void btn_ch4_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch4.Content) == "вкл")
            {
                btn_ch4.Content = "выкл";
                if (main != null)
                {
                    main.CH4.OFF = 1;
                }
            }
            else
            {
                btn_ch4.Content = "вкл";
                if (main != null)
                {
                    main.CH4.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn4:" + a[3]);
        }

        private void btn_ch5_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch5.Content) == "вкл")
            {
                btn_ch5.Content = "выкл";
                if (main != null)
                {
                    main.CH5.OFF = 1;
                }
            }
            else
            {
                btn_ch5.Content = "вкл";
                if (main != null)
                {
                    main.CH5.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
           Debug.WriteLine("btn5:"+ a[3]);
        }

        private void btn_ch6_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch6.Content) == "вкл")
            {
                btn_ch6.Content = "выкл";
                if (main != null)
                {
                    main.CH6.OFF = 1;
                }
            }
            else
            {
                btn_ch6.Content = "вкл";
                if (main != null)
                {
                    main.CH6.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn6:" + a[3]);
        }

        private void btn_ch7_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch7.Content) == "вкл")
            {
                btn_ch7.Content = "выкл";
                if (main != null)
                {
                    main.CH7.OFF = 1;
                }
            }
            else
            {
                btn_ch7.Content = "вкл";
                if (main != null)
                {
                    main.CH7.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn7:" + a[3]);
        }

        private void btn_ch8_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;
            ZXC();//
            if (Convert.ToString(btn_ch8.Content) == "вкл")
            {
                btn_ch8.Content = "выкл";
                if (main != null)
                {
                    main.CH8.OFF = 1;
                }
            }
            else
            {
                btn_ch8.Content = "вкл";
                if (main != null)
                {
                    main.CH8.OFF = 0;
                }
            }

            a[3] = Convert.ToByte((main.CH1.OFF << 7) | (main.CH2.OFF << 6) | (main.CH3.OFF << 5) | (main.CH4.OFF << 4) | (main.CH5.OFF << 3) | (main.CH6.OFF << 2) | (main.CH7.OFF << 1) | (main.CH8.OFF << 0));
            main.UDP_SEND(4, a, 4, 0);
            Debug.WriteLine("btn8:" + a[3]);
        }

        private void btn_ch1_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i =1;
            int k_u =1;
            int error = 0;

            try
            {
                 k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch1_corr.Text) * 1000);
                 k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch1_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }            

            if (error==0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(0);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(0);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch2_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch2_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch2_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(1);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(1);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch3_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch3_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch3_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(2);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(2);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch4_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch4_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch4_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(3);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(3);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch5_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch5_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch5_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(4);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(4);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch6_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch6_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch6_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(5);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(5);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch7_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch7_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch7_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(6);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(6);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private void btn_ch8_corr_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[5];
            int k_i = 1;
            int k_u = 1;
            int error = 0;

            try
            {
                k_i = Convert.ToInt32(Convert.ToDouble(txt_I_ch8_corr.Text) * 1000);
                k_u = Convert.ToInt32(Convert.ToDouble(txt_U_ch8_corr.Text) * 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поставь запятую, а не точку!");
                error = 1;
            }

            if (error == 0)
            {
                a[4] = (byte)(k_i >> 0);
                a[3] = (byte)(k_i >> 8);
                a[2] = (byte)(k_i >> 16);
                a[1] = (byte)(k_i >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(7);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_I,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );

                a[4] = (byte)(k_u >> 0);
                a[3] = (byte)(k_u >> 8);
                a[2] = (byte)(k_u >> 16);
                a[1] = (byte)(k_u >> 25); //в младшем адресе находятся старшие байты числа!!!
                a[0] = (byte)(7);//номер канала

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_U,//
                    a,  //данные
                    5,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
            }

            Debug.WriteLine("k_i:" + k_i);
            Debug.WriteLine("k_u:" + k_u);
        }

        private bool CFG_load()
        {
            bool error = false;
            string path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = System.IO.Path.GetDirectoryName(path);

            // получаем выбранный файл
            string filename = "cfg_CMD_MSG_330.dat";//тут храним данные команд и сообщений протокола управления блоком питания
            try
            {
                XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CcfgCMD_MSG_330));
                FileStream fr = new FileStream(filename, FileMode.Open);
                cfg_CMD_MSG = (CcfgCMD_MSG_330)xmlSerialaizer.Deserialize(fr);
                fr.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error! B330 cfg_CMD_MSG_330!");
                Debug.WriteLine("Исключение:" + ex);
                MessageBox.Show("Проблема с конфигурационными файлами Б330!");
            }


            // получаем выбранный файл
            filename = "cfg_B330.dat";//тут храним конфигурационные данные блока питания : IP адреса блока и его сервера
            try
            {
                XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(Config_B330));
                FileStream fr = new FileStream(filename, FileMode.Open);
                cfg = (Config_B330)xmlSerialaizer.Deserialize(fr);
                fr.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("B330 Config_B330!");
                Debug.WriteLine("Исключение:" + ex);
                MessageBox.Show("Проблема с конфигурационными файлами Б330!");
            }

            return error;
        }

        private void btn_corr_req_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            byte[] a = new byte[1];          

                a[0] = (byte)(0);//

                main.UDP_SEND
                    (
                    cfg_CMD_MSG.CMD_Corr_REQ,//
                    a,  //данные
                    1,  //число данных в байтах
                    0   //время исполнения 0 - значит сразу как сможешь
                    );
        }
    }
}
