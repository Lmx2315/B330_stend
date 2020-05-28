using System;
using System.Windows;
using System.ComponentModel; // CancelEventArgs
using System.Diagnostics;

namespace stnd_72_v2
{
    /// <summary>
    /// Логика взаимодействия для ADC_form.xaml
    /// </summary>
    public partial class Panel_330 : Window
    {

        public Panel_330(string a, MainWindow main)//конструктор формы
        {
            //     MainWindow main = this.Owner as MainWindow;
            InitializeComponent();
            this.Name_this = a;
            this.Title = a;

            if (main != null)
            {
                Debug.WriteLine("Name_this:" + Name_this);
                if (Name_this == "Панель 330")
                {
                    Debug.WriteLine("main.ISPRAV_AC:" + main.ISPRAV_AC);
                    if ((main.ISPRAV_AC & 0x01) != 0) checkBox_isprav_ac_green.IsChecked = true; else checkBox_isprav_ac_green.IsChecked = false;
                    if ((main.ISPRAV_AC & 0x02) != 0) checkBox_isprav_ac_red.IsChecked = true; else checkBox_isprav_ac_red.IsChecked = false;
                    if ((main.PROGR & 0x01) != 0) checkBox_progr_green.IsChecked = true; else checkBox_progr_green.IsChecked = false;
                    if ((main.PROGR & 0x02) != 0) checkBox_progr_red.IsChecked = true; else checkBox_progr_red.IsChecked = false;

                    if ((main.TEMP & 0x02) != 0) checkBox_temp_red.IsChecked = true; else checkBox_temp_red.IsChecked = false;
                    if ((main.TEMP & 0x01) != 0) checkBox_temp_green.IsChecked = true; else checkBox_temp_green.IsChecked = false;

                    if ((main.OTKL_AC & 0x01) != 0) checkBox_otkl_ac_red.IsChecked = true; else checkBox_otkl_ac_red.IsChecked = false;

                    if ((main.ISPR_J330 & 0x02) != 0) checkBox_ispr_j330_green.IsChecked = true; else checkBox_ispr_j330_green.IsChecked = false;
                    if ((main.ISPR_J330 & 0x01) != 0) checkBox_ispr_j330_red.IsChecked = true; else checkBox_ispr_j330_red.IsChecked = false;

                    if ((main.LS & 0x02) != 0) checkBox_ls_red.IsChecked = true; else checkBox_ls_red.IsChecked = false;
                    if ((main.LS & 0x01) != 0) checkBox_ls_green.IsChecked = true; else checkBox_ls_green.IsChecked = false;

                    if ((main.SINHR & 0x02) != 0) checkBox_sinhr_green.IsChecked = true; else checkBox_sinhr_green.IsChecked = false;
                    if ((main.SINHR & 0x01) != 0) checkBox_sinhr_red.IsChecked = true; else checkBox_sinhr_red.IsChecked = false;

                    if ((main.OFCH & 0x02) != 0) checkBox_ofch_red.IsChecked = true; else checkBox_ofch_red.IsChecked = false;
                    if ((main.OFCH & 0x01) != 0) checkBox_ofch_green.IsChecked = true; else checkBox_ofch_green.IsChecked = false;


                }
            }

        }

        string Name_this = "";
        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            // MessageBox.Show("Closing called");
            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (Name_this == "Панель 330") main.Panel_330_form[0] = false;
            }
        }


        int tca_convert() //переводим значения переменныйх в вид удобный для программирования микросхемы i2c - TCA
        {
            MainWindow main = this.Owner as MainWindow;
            int z;
            z = main.ISPRAV_AC + ((main.PROGR & 0x3) << 4) + ((main.OFCH & 0x3) << 21) + ((main.SINHR & 0x3) << 18) +
                                             ((main.LS & 0x3) << 16) + ((main.ISPR_J330 & 0x3) << 14) +
                                             ((main.OTKL_AC & 0x1) << 12) + ((main.TEMP & 0x3) << 8);
            return z;
        }

        byte[] a = new byte[4];

        private void checkBox_isprav_ac_red_Click(object sender, RoutedEventArgs e)
        {
            int z = 0;
            string s1 = "";

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_isprav_ac_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPRAV_AC = (byte)(main.ISPRAV_AC | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);//12-02-2020 200->203

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPRAV_AC = (byte)(main.ISPRAV_AC & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_isprav_ac_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_isprav_ac_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPRAV_AC = (byte)(main.ISPRAV_AC | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPRAV_AC = (byte)(main.ISPRAV_AC & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_progr_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_progr_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.PROGR = (byte)(main.PROGR | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.PROGR = (byte)(main.PROGR & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }

        }

        private void checkBox_progr_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_progr_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.PROGR = (byte)(main.PROGR | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.PROGR = (byte)(main.PROGR & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_temp_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_temp_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.TEMP = (byte)(main.TEMP | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.TEMP = (byte)(main.TEMP & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_temp_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_temp_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.TEMP = (byte)(main.TEMP | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.TEMP = (byte)(main.TEMP & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_otkl_ac_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_otkl_ac_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OTKL_AC = (byte)(main.OTKL_AC | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OTKL_AC = (byte)(main.OTKL_AC & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ispr_j330_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ispr_j330_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPR_J330 = (byte)(main.ISPR_J330 | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPR_J330 = (byte)(main.ISPR_J330 & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ispr_j330_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ispr_j330_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPR_J330 = (byte)(main.ISPR_J330 | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.ISPR_J330 = (byte)(main.ISPR_J330 & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ls_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ls_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.LS = (byte)(main.LS | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.LS = (byte)(main.LS & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ls_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ls_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.LS = (byte)(main.LS | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.LS = (byte)(main.LS & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_sinhr_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_sinhr_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.SINHR = (byte)(main.SINHR | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.SINHR = (byte)(main.SINHR & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_sinhr_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_sinhr_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.SINHR = (byte)(main.SINHR | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.SINHR = (byte)(main.SINHR & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ofch_red_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ofch_red.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OFCH = (byte)(main.OFCH | 2);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OFCH = (byte)(main.OFCH & (~2));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }

        private void checkBox_ofch_green_Click(object sender, RoutedEventArgs e)
        {
            string s1 = "";
            int z = 0;

            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                if (checkBox_ofch_green.IsChecked == true)
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OFCH = (byte)(main.OFCH | 1);

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";
                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);

                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
                else
                {
                    if (Name_this == "Панель 330")
                    {
                        main.OFCH = (byte)(main.OFCH & (~1));

                        z = tca_convert();

                        a[0] = Convert.ToByte((z >> 24) & 0xff);
                        a[1] = Convert.ToByte((z >> 16) & 0xff);
                        a[2] = Convert.ToByte((z >> 8) & 0xff);
                        a[3] = Convert.ToByte((z >> 0) & 0xff);

                        main.UDP_SEND(200, a, 4, 0);

                        s1 = " ~0 tca_w:" + Convert.ToString(z) + "; ";

                        try
                        {
                            if (main.serialPort1.IsOpen == false)
                            {
                                main.serialPort1.Open();
                            }
                            Debug.WriteLine("шлём:" + s1);
                            main.serialPort1.Write(s1);
                        }
                        catch (Exception ex)
                        {
                            // что-то пошло не так и упало исключение... Выведем сообщение исключения
                            Console.WriteLine(string.Format("Port:'{0}' Error:'{1}'", main.serialPort1.PortName, ex.Message));
                        }
                    }
                }
            }
        }
    }
}
