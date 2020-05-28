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

namespace stnd_72_v2
{
    /// <summary>
    /// Логика взаимодействия для Panel_INFO.xaml
    /// </summary>
    public partial class Panel_INFO : Window
    {
        string Name_this = "";
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

        private void btn_ch1_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            MainWindow main = this.Owner as MainWindow;

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
    }
}
