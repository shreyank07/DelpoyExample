using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PGYMiniCooper.DataModule.Model
{
    public static class HardwarePacketFactory
    {
        static Dictionary<eVoltage, int> voltageLookupTable1;
        static Dictionary<eVoltage, int> voltageLookupTable2;
        static HardwarePacketFactory()
        {
            voltageLookupTable1 = new Dictionary<eVoltage, int>();
            voltageLookupTable1.Add(eVoltage._1_2V, 0x258);
            voltageLookupTable1.Add(eVoltage._1_8V, 0x384);
            voltageLookupTable1.Add(eVoltage._2_5V, 0x4E2);
            voltageLookupTable1.Add(eVoltage._3_3V, 0x672);
            voltageLookupTable1.Add(eVoltage._5_0V, 0x9C4);

            voltageLookupTable2 = new Dictionary<eVoltage, int>();
            //voltageLookupTable2.Add(eVoltage._1_2V, 0x96);
            //voltageLookupTable2.Add(eVoltage._1_8V, 0xD2);
            //voltageLookupTable2.Add(eVoltage._2_5V, 0x118);
            //voltageLookupTable2.Add(eVoltage._3_3V, 0x168);
            //voltageLookupTable2.Add(eVoltage._5_0V, 0x212);

            //voltageLookupTable2.Add(eVoltage._1_2V, 0xC8);
            //voltageLookupTable2.Add(eVoltage._1_8V, 0x12C);
            //voltageLookupTable2.Add(eVoltage._2_5V, 0x1A1);
            //voltageLookupTable2.Add(eVoltage._3_3V, 0x226);
            //voltageLookupTable2.Add(eVoltage._5_0V, 0x33E);
            voltageLookupTable2.Add(eVoltage._1_0V, 0x64);
            voltageLookupTable2.Add(eVoltage._1_2V, 0x05B);
            voltageLookupTable2.Add(eVoltage._1_8V, 0x08D);
            voltageLookupTable2.Add(eVoltage._2_5V, 0x0C6);
            voltageLookupTable2.Add(eVoltage._3_3V, 0x0F3);
            voltageLookupTable2.Add(eVoltage._5_0V, 0x142);
        }

        public static byte[] GetHWResetbytes()
        {
            List<byte> header = new List<byte>();
            header.Add(5);//[42] - Packet Type
            header.Add(0); //[43] - LSB Length - need to be updated before add to main list
            header.Add(0); //[44] - MSB Length - need to be updated before add to main list
            header.Add(0); //[45]
            header.Add(0); //[46]
            header.Add(0); //[47]
            header.Add(0); //[48]
            header.Add(0); //[49]
            header.Add(0); //[50]
            header.Add(0); //[51]
            header.Add(0); //[52]
            header.Add(0); //[53]
            header.Add(0); //[54]
            header.Add(0); //[55]
            header.Add(0); //[56]
            header.Add(0); //[57]
            header.Add(0); //[58]
            header.Add(0); //[59]
            header.Add(0); //[60]
            header.Add(0); //[61]
            header.Add(0); //[62]
            header.Add(0); //[63]

            return header.ToArray();
        }

        public static byte[] GetConfigurationBytes(ConfigModel configuration)
        {
            var UARTConfig = configuration.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var canconfig=configuration.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            var qspiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            List<byte> byteList = new List<byte>();
            byteList.Add(7); //[42] Exerciser Node Packet - 7
            byteList.Add(0); //[43]
            byteList.Add(0); //[44]
            if ((configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both) && (UARTConfig != null ))
            {
                //configuration.DataWidth = ConfigModel.GetInstance().DataWidth;
                //configuration.DataWidth = eDataWidth._8_bit; // for now keeping it default
                configuration.IsParity = true; // for now keeping it default
                byteList.Add(Convert.ToByte((GetBaudrate((int)UARTConfig.BaudRate) << 4) | Convert.ToByte((GetDataWidth(UARTConfig.DataWidth) << 2) | (Convert.ToByte(configuration.IsParity) << 1) | 0x1)));
            }
            else
            {
                byteList.Add(0x1);
            }
            if ((configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both))
            {
                if(UARTConfig==null)
                {
                    byteList.Add(Convert.ToByte(GetSamplingEdge(configuration) | 0x1));

                }
                else
                byteList.Add(Convert.ToByte(GetSamplingEdge(configuration) |Convert.ToByte( GetMSBLASB(UARTConfig.BitOrder))));
            }
            else
            {
                byteList.Add(0x0);
            }
            byteList.AddRange(GetChannelBytes(configuration));
            byteList.Add(0); //Reserved
            var protocolList = GetProtocolChannelBytes(configuration);
            byteList.AddRange(protocolList.Take(4));//4 bytes
            byteList.AddRange(GetVoltageChannelBytes(configuration));//4 bytes

            byteList.Add(GetSamplingRateBytes(configuration));//sampling rate

         byteList.AddRange(protocolList.ToList().GetRange(4, 3));
            if (canconfig != null)
            {
                if (canconfig.CANType == eCANType.CAN)
                {
                    byteList.Add(Convert.ToByte(GetBaudRateCan(canconfig.BaudRate) << 5));
                }
                else
                {
                    byteList.Add(Convert.ToByte(GetBaudRateCan(canconfig.BaudRateCANFD) << 5));
                }

                byteList.Add(Convert.ToByte(GetCANtypes(configuration) << 6 | Convert.ToByte(GetBRS(canconfig.BRS) << 4)));//71
                if (canconfig != null && canconfig.BRS == true)
                {
                    byteList.Add(Convert.ToByte(GetBaudRateCanBRS(canconfig.BaudRateBRS) << 5)); //72    
                }
                else
                    byteList.Add(0);
             
            }
            else
            {
                byteList.Add(0);
                byteList.Add(0);
                byteList.Add(0);

            }
      

            byteList.Add(0);//73
            byteList.Add(0);//74

            if(qspiconfig!=null)
            {
                byteList.Add(Convert.ToByte(GetQSPIClockPolarityType(configuration) << 7 | Convert.ToByte(GetQSPIClockPhaseType(configuration) << 6 | Convert.ToByte(GetQSPIChipSelectType(configuration) << 5 | Convert.ToByte(GetQSPIAddressModeType(configuration) << 4 | Convert.ToByte(GetQSPIModeType(configuration) << 2 & 0xC0))))));////75



            }
            else
                byteList.Add(0);




            byteList.AddRange(protocolList.ToList().GetRange(7, 3));
            int packetLength = byteList.Count - 22;
            byteList[1] = Convert.ToByte(packetLength & 0xFF);
            byteList[2] = Convert.ToByte((packetLength >> 8) & 0xFF);
            return byteList.ToArray();
        }

        static byte GetBRS(bool BRSSelecetd)
        {

            if (BRSSelecetd == false)
                return 0;
            else if (BRSSelecetd == true)
                return 1;

            return 0;
        }

        private static int GetQSPIClockPolarityType(ConfigModel configuration)
        {
            var qsiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            if (qsiconfig.Polarity==eQSPIPolarity.Low)
                return 0;
            else
                return 1;
        }



        private static int GetQSPIClockPhaseType(ConfigModel configuration)
        {
            var qsiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            if (qsiconfig.Phase==eQSPIPhase.Low)
                return 0;
            else
                return 1;
        }


    

        private static int GetQSPIChipSelectType(ConfigModel configuration)
        {
            var qsiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            if (qsiconfig.ChipSelect == eQSPIChipSelect.Low)
                return 0;
            else
                return 1;
        }
        private static int GetQSPIAddressModeType(ConfigModel configuration)
        {
            var qsiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            if (qsiconfig.QSPIByteAddresssType == eQSPIAddress.ThreeByte)
                
                return 0;
            else
                return 1;
        }

        private static int GetQSPIModeType(ConfigModel configuration)
        {
            var qsiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            if (qsiconfig.Mode == eQSPIMode.Extended)
                return 0;
            else if (qsiconfig.Mode ==eQSPIMode.Dual)
                return 4;
            else if (qsiconfig.Mode==eQSPIMode.Quad)
                return 8;
            else
                return 0;
        }

        static byte GetBaudRateCanBRS(int baudRate)
        {
           
                if (baudRate == 2000)
                    return 1;
                if (baudRate == 5000)
                    return 2;
                if (baudRate == 8000)
                    return 3;
            
            return 0;
        }
        static byte GetMSBLASB(eBitOrder eBitOrder)
        {

        
                if (eBitOrder == eBitOrder.MSBFirst)
                    return 0;
                if (eBitOrder == eBitOrder.LSBFirst)
                    return 1;

            return 0;

        }

        static byte GetCANtypes(ConfigModel configuartion)
        {
            var canconfig = configuartion.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            if (canconfig!=null)
            {
                if (canconfig.CANType==eCANType.CAN)

                    return 1;
                if (canconfig.CANType==eCANType.CANFD)
                    return 2;
            }
        

            return 0;
        }

        static byte GetSamplingRateBytes(ConfigModel configuration)
        {
            if (configuration.ConfigurationMode == eConfigMode.PA_Mode)
                return 0x01;
            else if (configuration.ConfigurationMode == eConfigMode.LA_Mode)
            {
                if (configuration.GeneralPurposeMode == eGeneralPurpose.State)
                    return 0x01;
                else
                    return GetSampleRate(configuration.SampleRateLAPA);
            }
            else
                return GetSampleRate(configuration.SampleRateLAPA);
        }

   
        static byte[] GetVoltageChannelBytes(ConfigModel configuration)
        {
            List<byte> returnbytes = new List<byte>();
            //Voltage is same irrespective of mode
            int ch_A, ch_B, ch_C, ch_D, ch_E, ch_F, ch_G, ch_H;
            //if (configuration.ProbeType == eProbeType.Type1)
            //{
            //    if (voltageLookupTable1.ContainsKey(configuration.SignalAmpCh1_2))
            //        ch_A = voltageLookupTable1[configuration.SignalAmpCh1_2];
            //    else
            //        ch_A = 0;
            //    //if (configuration.VoltText.ToLower().Contains("0x"))
            //    //    configuration.VoltText = configuration.VoltText.Replace("0x", "");
            //    //ch_A = int.Parse(configuration.VoltText, NumberStyles.HexNumber, CultureInfo.CurrentCulture);
            //    if (voltageLookupTable1.ContainsKey(configuration.SignalAmpCh3_4))
            //        ch_B = voltageLookupTable1[configuration.SignalAmpCh3_4];
            //    else
            //        ch_B = 0;
            //    if (voltageLookupTable1.ContainsKey(configuration.SignalAmpCh5_6))
            //        ch_C = voltageLookupTable1[configuration.SignalAmpCh5_6];
            //    else
            //        ch_C = 0;
            //    if (voltageLookupTable1.ContainsKey(configuration.SignalAmpCh7_8_9_10))
            //        ch_D = voltageLookupTable1[configuration.SignalAmpCh7_8_9_10];
            //    else
            //        ch_D = 0;
            //}
            //else
            {
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh1_2))
                    ch_A = voltageLookupTable2[configuration.SignalAmpCh1_2];
                else
                    ch_A = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh3_4))
                    ch_B = voltageLookupTable2[configuration.SignalAmpCh3_4];
                else
                    ch_B = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh5_6))
                    ch_C = voltageLookupTable2[configuration.SignalAmpCh5_6];
                else
                    ch_C = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh7_8))
                    ch_D = voltageLookupTable2[configuration.SignalAmpCh7_8];
                else
                    ch_D = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh9_10))
                    ch_E = voltageLookupTable2[configuration.SignalAmpCh9_10];
                else
                    ch_E = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh11_12))
                    ch_F = voltageLookupTable2[configuration.SignalAmpCh11_12];
                else
                    ch_F = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh13_14))
                    ch_G = voltageLookupTable2[configuration.SignalAmpCh13_14];
                else
                    ch_G = 0;
                if (voltageLookupTable2.ContainsKey(configuration.SignalAmpCh15_16))
                    ch_H = voltageLookupTable2[configuration.SignalAmpCh15_16];
                else
                    ch_H = 0;
            }

            returnbytes.Add(Convert.ToByte((ch_A >> 2) & 0xFF));
            returnbytes.Add(Convert.ToByte((ch_A & 0x3) << 6 | ((ch_B >> 4) & 0x3F)));
            returnbytes.Add(Convert.ToByte((ch_B & 0xF) << 4 | ((ch_C >> 6) & 0xF)));
            returnbytes.Add(Convert.ToByte((ch_C & 0x1F) << 2 | ((ch_D >> 8) & 0x3)));
            returnbytes.Add(Convert.ToByte(ch_D & 0xFF));
            returnbytes.Add(Convert.ToByte((ch_E >> 2) & 0xFF));
            returnbytes.Add(Convert.ToByte((ch_E & 0x3) << 6 | ((ch_F >> 4) & 0x3F)));
            returnbytes.Add(Convert.ToByte((ch_F & 0xF) << 4 | ((ch_G >> 6) & 0xF)));
            returnbytes.Add(Convert.ToByte((ch_G & 0x1F) << 2 | ((ch_H >> 8) & 0x3)));
            returnbytes.Add(Convert.ToByte(ch_H & 0xFF));

            return returnbytes.ToArray();
        }

        static byte[] GetProtocolChannelBytes(ConfigModel configuration)
        {
            List<byte> protocolChannelBytes = new List<byte>();

            if (configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both)
            {
                // Byte offset 52 - 55
                // I2C SCL Channel info  | I2C SDA Channel info			
                // SPI CLK channel info  | SPI Chip select channel info
                // SPI MISO channel info | SPI MOSI channel info
                // UART_TX_channel info  | UART_RX_Channel_info
                // Byte Offset 67 - 69
                // I3C_SCL_Channel_Info  | I3C_SDA_Channel_Info			
                // SPMI_SCL_Channel_Info | SPMI_SCL_Channel_Info
                // RFFE_SCL_Channel_Info | RFFE_SDA_Channel_Info

                if (configuration.Trigger.ProtocolSelection is ConfigModel_I2C i2CConfig)
                {
                    int I2CchannelVal = 0;
                    if (i2CConfig.ChannelIndex_SDA != eChannles.None)
                        I2CchannelVal = (int)i2CConfig.ChannelIndex_SDA;

                    if (i2CConfig.ChannelIndex_SCL != eChannles.None)
                        I2CchannelVal = ((int)i2CConfig.ChannelIndex_SCL << 4) | I2CchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(I2CchannelVal & 0xFF));//I2C SCL/SDA
                }
                else
                {
                    protocolChannelBytes.Add(0);
                }

                if (configuration.Trigger.ProtocolSelection is ConfigModel_SPI spiConfig)
                {
                    int SPIchannelVal = 0;

                    if (spiConfig.ChannelIndex_CS != eChannles.None)
                        SPIchannelVal = ((int)spiConfig.ChannelIndex_CS);

                    if (spiConfig.ChannelIndex_CLK != eChannles.None)
                        SPIchannelVal = ((int)spiConfig.ChannelIndex_CLK << 4) | SPIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(SPIchannelVal & 0xFF));//I2C SCL/SDA

                    SPIchannelVal = 0;
                    if (spiConfig.ChannelIndex_MOSI != eChannles.None)
                        SPIchannelVal = (int)spiConfig.ChannelIndex_MOSI;

                    if (spiConfig.ChannelIndex_MISO != eChannles.None)
                        SPIchannelVal = ((int)spiConfig.ChannelIndex_MISO << 4) | SPIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(SPIchannelVal & 0xFF));
                }
                else
                {
                    protocolChannelBytes.Add(0);
                    protocolChannelBytes.Add(0);
                }

                if (configuration.Trigger.ProtocolSelection is ConfigModel_UART UARTConfig)
                {
                    int uARTchannelVal = 0;
                    if (UARTConfig.ChannelIndex_RX != eChannles.None)
                        uARTchannelVal = (int)UARTConfig.ChannelIndex_RX;

                    if (UARTConfig.ChannelIndex_TX != eChannles.None)
                        uARTchannelVal = ((int)UARTConfig.ChannelIndex_TX << 4) | uARTchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(uARTchannelVal & 0xFF));//I2C SCL/SDA
                }
                else
                {
                    protocolChannelBytes.Add(0);
                }

                if (configuration.Trigger.ProtocolSelection is ConfigModel_I3C i3cConfig)
                {
                    int I3CchannelVal = 0;
                    if (i3cConfig.ChannelIndex_SDA != eChannles.None)
                        I3CchannelVal = (int)i3cConfig.ChannelIndex_SDA;

                    if (i3cConfig.ChannelIndex_SCL != eChannles.None)
                        I3CchannelVal = ((int)i3cConfig.ChannelIndex_SCL << 4) | I3CchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(I3CchannelVal & 0xFF));//I3C SCL/SDA
                }
                else
                {
                    protocolChannelBytes.Add(0);
                }

                if (configuration.Trigger.ProtocolSelection is ConfigModel_SPMI spmiConfig)
                {
                    int SPMIchannelVal = 0;
                    if (spmiConfig.ChannelIndex_SDA != eChannles.None)
                        SPMIchannelVal = (int)spmiConfig.ChannelIndex_SDA;

                    if (spmiConfig.ChannelIndex_SCL != eChannles.None)
                        SPMIchannelVal = ((int)spmiConfig.ChannelIndex_SCL << 4) | SPMIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(SPMIchannelVal & 0xFF));//SPMI SCL/SDA
                }
                else
                {
                    protocolChannelBytes.Add(0);
                }

                if (configuration.Trigger.ProtocolSelection is ConfigModel_RFFE rffeconfig)
                {
                    int SPMIchannelVal = 0;
                    if (rffeconfig.ChannelIndex_SDA != eChannles.None)
                        SPMIchannelVal = (int)rffeconfig.ChannelIndex_SDA;

                    if (rffeconfig.ChannelIndex_SCL != eChannles.None)
                        SPMIchannelVal = ((int)rffeconfig.ChannelIndex_SCL << 4) | SPMIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(SPMIchannelVal & 0xFF));//SPMI SCL/SDA
                }
                else
                {
                    protocolChannelBytes.Add(0);
                }






                if (configuration.Trigger.ProtocolSelection is ConfigModel_QSPI qspiconfig)
                {
                    int QSPIchannelVal = 0;

                    if (qspiconfig.ChannelIndex_CS != eChannles.None)
                        QSPIchannelVal = (int)qspiconfig.ChannelIndex_CS;

                    if (qspiconfig.ChannelIndex_CLK != eChannles.None)
                        QSPIchannelVal = ((int)qspiconfig.ChannelIndex_CLK << 4) | QSPIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(QSPIchannelVal & 0xFF));


                    QSPIchannelVal = 0;
                    if (qspiconfig.ChannelIndex_D1 != eChannles.None)
                        QSPIchannelVal = (int)qspiconfig.ChannelIndex_D1;

                    if (qspiconfig.ChannelIndex_D0 != eChannles.None)
                        QSPIchannelVal = ((int)qspiconfig.ChannelIndex_D0 << 4) | QSPIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(QSPIchannelVal & 0xFF));


                    QSPIchannelVal = 0;
                    if (qspiconfig.ChannelIndex_D3 != eChannles.None)
                        QSPIchannelVal = (int)qspiconfig.ChannelIndex_D3;

                    if(qspiconfig.ChannelIndex_D2 != eChannles.None)
                        QSPIchannelVal = ((int)qspiconfig.ChannelIndex_D2 << 4) | QSPIchannelVal;

                    protocolChannelBytes.Add(Convert.ToByte(QSPIchannelVal & 0xFF));

                }
                else
                {
                    protocolChannelBytes.Add(0);
                    protocolChannelBytes.Add(0);
                    protocolChannelBytes.Add(0);
                }


            }
            else
            {
                // 11 bytes 
                protocolChannelBytes.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0,0 ,0,0,0});
            }

            return protocolChannelBytes.ToArray();
        }

        static byte[] GetChannelBytes(ConfigModel configuration)
        {
            List<byte> returnbytes = new List<byte>();
            Dictionary<eChannles, int> chanInfo = new Dictionary<eChannles, int>();
            Dictionary<eChannles, int> UnUsedChanInfo = new Dictionary<eChannles, int>();


            if (configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both)
            {
                int comparisonEnable = 0xFFFF;
                returnbytes.Add(Convert.ToByte((comparisonEnable >> 8) & 0xFF));
                returnbytes.Add(Convert.ToByte(comparisonEnable & 0xFF));

            }
            else if (configuration.ConfigurationMode == eConfigMode.LA_Mode)
            {
                int comparisionChannelBytes = 0xffff;
                if (configuration.GeneralPurposeMode == eGeneralPurpose.State)
                {
                    comparisionChannelBytes = 0;
                    if (configuration.SelectedClock1 != eChannles.None)
                        comparisionChannelBytes |= 1 << (int)configuration.SelectedClock1;

                    if (configuration.SelectedClock2 != eChannles.None && configuration.HasTwoClockSources)
                        comparisionChannelBytes |= 1 << (int)configuration.SelectedClock2;
                }

                returnbytes.Add(Convert.ToByte((comparisionChannelBytes >> 8) & 0xFF));
                returnbytes.Add(Convert.ToByte(comparisionChannelBytes & 0xFF));
            }

            var enabledChannels = configuration.ProtocolConfigList.SelectMany(c => c.Channels).Where(c => c.ChannelIndex != eChannles.None).ToList();

            // channels are in 2 byte space for 16 channels
            int channelBytes = 0;

            // Enable clock source 1 and 2
            if (configuration.GroupType == eGroupType.Group)
            {
                if (configuration.SelectedClock1 != eChannles.None)
                    channelBytes |= 1 << (int)configuration.SelectedClock1;

                if (configuration.HasTwoClockSources && configuration.SelectedClock2 != eChannles.None)
                    channelBytes |= 1 << (int)configuration.SelectedClock2;
            }

            if (enabledChannels.Count > 0)
            {
                var channelList = Enum.GetValues(typeof(eChannles)).Cast<eChannles>().ToList();
                for (int ch = 0; ch < channelList.Count; ch++)
                {
                    if (enabledChannels.Any(c => c.ChannelIndex == channelList[ch]))
                    {
                        channelBytes |= 1 << ch;
                    }
                }
            }

            returnbytes.Add(Convert.ToByte((channelBytes >> 8) & 0xFF));
            returnbytes.Add(Convert.ToByte(channelBytes & 0xFF));

            return returnbytes.ToArray();
        }

        static byte GetSampleRate(eSampleRate sampleRate)
        {
            if (sampleRate == eSampleRate.SR_1000)
                return 0x1;
            else if (sampleRate == eSampleRate.SR_500)
                return 0x2;
            else if (sampleRate == eSampleRate.SR_333)
                return 0x4;
            else if (sampleRate == eSampleRate.SR_250)
                return 0x8;
            else if (sampleRate == eSampleRate.SR_200)
                return 0x10;
            else if (sampleRate == eSampleRate.SR_166)
                return 0x20;
            else if (sampleRate == eSampleRate.SR_142)
                return 0x40;
            else if (sampleRate == eSampleRate.SR_125)
                return 0x80;

            return 0;
        }

        static byte GetBaudRateCan(int baudRate)
        {

            if (baudRate == 125)
                return 1;
            if (baudRate == 250)
                return 2;
            if (baudRate == 500)
                return 3;
            else if (baudRate == 1000)
                return 4;
            else if (baudRate == 2048)
                return 5;
            else if (baudRate == 5120)
                return 6;

            return 0;
        }
        static byte GetBaudRateCanBRS(ConfigModel configuration, int baudRate)
        {
            var canconfig = configuration.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            if (canconfig.BRS == true)
            {
                if (baudRate == 2000000)
                    return 1;
                if (baudRate == 5000000)
                    return 2;
                if (baudRate == 8000000)
                    return 3;
            }
            return 0;
        }


        static byte GetBRS(ConfigModel configuration, bool BRSSelecetd)
        {
            var canconfig = configuration.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            if (canconfig.BRS == false)
                return 0;
            else if (canconfig.BRS == true)
                return 1;

            return 0;
        }
        static byte GetBaudrate(int baudRate)
        {
            if (baudRate == 300)
                return 0;
            else if (baudRate == 600)
                return 1;
            else if (baudRate == 1200)
                return 2;
            else if (baudRate == 2400)
                return 3;
            else if (baudRate == 4800)
                return 4;
            else if (baudRate == 9600)
                return 5;
            else if (baudRate == 19200)
                return 6;
            else if (baudRate == 38400)
                return 7;
            else if (baudRate == 57600)
                return 8;
            else if (baudRate == 115200)
                return 9;
            else if (baudRate == 128000)
                return 10;
            else if (baudRate == 256000)
                return 11;
            else if (baudRate == 230400)
                return 12;
            else if (baudRate == 921600)
                return 13;

            return 0;
        }

        static byte GetDataWidth(int dataWidth)
        {
            if (dataWidth == 5)
                return 0;
            else if (dataWidth == 6)
                return 1;
            else if (dataWidth == 7)
                return 2;
            else if (dataWidth == 8)
                return 3;
            return 0;
        }
        //static byte GetSamplingEdge(eEdgeType edge)
        //{
        //    if (edge == eEdgeType.RISING_EDGE)
        //        return 0;
        //    else if (edge == eEdgeType.FALLING_EDGE)
        //        return 1;

        //    return 0;
        //}

        static byte GetSamplingEdge(ConfigModel configuration)
        {
            var spiconfig = configuration.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            byte retVal=0;
            if (configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both)
            {
                if (spiconfig != null)
                {
                    if (spiconfig?.Polarity == eSPIPolarity.High)
                        retVal |= 0x80;


                    if (spiconfig?.Phase == eSPIPhase.High)
                        retVal |= 0x40;
                }
             

            }
            //else if (configuration.ConfigurationMode == eConfigMode.Both)
            //{
            //    if (configuration.ClockPolarityHighLAPA)
            //        retVal |= 0x80;

            //    if (configuration.ClockPhaseHighLAPA)
            //        retVal |= 0x40;

            //}
            return retVal;
        }
    }
}
