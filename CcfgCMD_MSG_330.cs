using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stnd_72_v2
{
   public class CcfgCMD_MSG_330
    {
        public uint CMD_TIME_SETUP;
        public uint CMD_HELP;
        public uint CMD_TIME;
        public uint CMD_12V;
        public uint CMD_STATUS;
        public uint CMD_LED;
        public uint CMD_xxx;
        public uint CMD_CH_UP;
        public uint CMD_SETUP_IP0;
        public uint CMD_SETUP_IP1;
        public uint CMD_SETUP_DEST_IP0;
        public uint CMD_SETUP_DEST_IP1;
        public uint CMD_REQ_VERSION;
        public uint CMD_REQ_NUM_SLAVE;
        public uint CMD_TEST_485;
        public uint CMD_TEST_SPI;
        public uint CMD_TEST_JTAG;
        public uint CMD_NUMBER_READ;//команда запрашивает номер блока
        public uint CMD_NUMBER_WRITE;//команда записывает номер блока
        public uint CMD_REALTIME;    //
        public uint CMD_Corr_I;   //
        public uint CMD_Corr_U;   //
        public uint CMD_Corr_REQ;

        public uint MSG_REPLY;
        public uint MSG_ERROR;
        public uint MSG_ERROR_CMD_BUF;//были затёртые команды в буфере
        public uint MSG_CMD_OK;       //команда выполненна успешно
        public uint MSG_STATUS_OK;    //Квитация на статус

        public uint MSG_PWR_CHANNEL;
        public uint MSG_REQ_VERSION;
        public uint MSG_REQ_NUMBER;//квитанция сообщает номер блока
        public uint MSG_REQ_NUM_SLAVE;
        public uint MSG_REQ_TEST_485;
        public uint MSG_REQ_TEST_SPI;
        public uint MSG_REQ_TEST_JTAG;
        public uint MSG_Corr_REQ;

        public uint MSG_ID_CH1;// 
        public uint MSG_ID_CH2;// 
        public uint MSG_ID_CH3;
        public uint MSG_ID_CH4;
        public uint MSG_ID_CH5;
        public uint MSG_ID_CH6;
        public uint MSG_ID_CH7;
        public uint MSG_ID_CH8;

        public uint MSG_TEMP_CH1;
        public uint MSG_TEMP_CH2;
        public uint MSG_TEMP_CH3;
        public uint MSG_TEMP_CH4;
        public uint MSG_TEMP_CH5;
        public uint MSG_TEMP_CH6;
        public uint MSG_TEMP_CH7;
        public uint MSG_TEMP_CH8;

        public uint MSG_I_CH1;
        public uint MSG_I_CH2;
        public uint MSG_I_CH3;
        public uint MSG_I_CH4;
        public uint MSG_I_CH5;
        public uint MSG_I_CH6;
        public uint MSG_I_CH7;
        public uint MSG_I_CH8;

        public uint MSG_P_CH1;
        public uint MSG_P_CH2;
        public uint MSG_P_CH3;
        public uint MSG_P_CH4;
        public uint MSG_P_CH5;
        public uint MSG_P_CH6;
        public uint MSG_P_CH7;
        public uint MSG_P_CH8;

        public uint MSG_U_CH1;
        public uint MSG_U_CH2;
        public uint MSG_U_CH3;
        public uint MSG_U_CH4;
        public uint MSG_U_CH5;
        public uint MSG_U_CH6;
        public uint MSG_U_CH7;
        public uint MSG_U_CH8;
    }
}
