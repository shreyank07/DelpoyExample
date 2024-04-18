using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class StatisticsRepository : ViewModelBase
    {
        private static StatisticsRepository _instance = null;
        public static StatisticsRepository GetInstance()
        {
            if (_instance == null)
            {
                _instance = new StatisticsRepository();
            }

            return _instance;
        }
        public StatisticsRepository()
        {
            writePacketCounter = new ObservableCollection<int>();
            readPacketCounter = new ObservableCollection<int>();
            allPacketCounter = new ObservableCollection<int>();
            FrameCounter = new ObservableCollection<int>();
            ErrorPacket = new ObservableCollection<int>();
            Commandcounter = new ObservableCollection<int>();
            Addresscounter = new ObservableCollection<int>();
            dataPacket1 = new ObservableCollection<int>();
            dataPacket2 = new ObservableCollection<int>();
            remoteFrame = new ObservableCollection<int>();
            overloadFrame = new ObservableCollection<int>();            
            //ReadkBCount = 0;
            //WritekBByte = 0;
            //writecount = 0;
            //Readcount = 0;
            //ReadkBByte1 = 0;
            //WritekBByte1 = 0;
            //writeCount1 = 0;
            //ReadCount1 = 0;
            addressPacket = new ObservableCollection<int>() { 0, 0, 0, 0 };
            crcPacket = new ObservableCollection<int>() { 0, 0, 0, 0 };
            Initialize();
        }


        #region SPMI
        public int CommandErrorPacket
        {
            get
            {
                return cmderrorPacket;
            }
            set
            {
                cmderrorPacket = value;
                RaisePropertyChanged("CommandErrorPacket");
            }
        }
        int cmderrorPacket;

        public int DataErrorPacket
        {
            get
            {
                return dataerrorPacket;
            }
            set
            {
                dataerrorPacket = value;
                RaisePropertyChanged("DataErrorPacket");
            }
        }
        int dataerrorPacket;

        //QSPI
        public int AddrerrorPacket
        {
            get
            {
                return addrerrorPacket;
            }
            set
            {
                addrerrorPacket = value;
                RaisePropertyChanged("AddrerrorPacket");
            }
        }
        int addrerrorPacket;
        Dictionary<eSPMICMDTYPE, int> SPMIframeType;
        public Dictionary<eSPMICMDTYPE, int> SPMIFrameType
        {
            get
            {
                return SPMIframeType;
            }

            set
            {
                SPMIframeType = value;
                RaisePropertyChanged("SPMIFrameType");
            }
        }

        Dictionary<eSPMICMDTYPE, int> SPMIframeType1;
        public Dictionary<eSPMICMDTYPE, int> SPMIFrameType1
        {
            get
            {
                return SPMIframeType1;
            }

            set
            {
                SPMIframeType1 = value;
                RaisePropertyChanged("SPMIFrameType1");
            }
        }


        Dictionary<eSPMICMDTYPE, int> SPMIframeType2;
        public Dictionary<eSPMICMDTYPE, int> SPMIFrameType2
        {
            get
            {
                return SPMIframeType2;
            }

            set
            {
                SPMIframeType2 = value;
                RaisePropertyChanged("SPMIFrameType2");
            }
        }

        Dictionary<eSPMICMDTYPE, int> SPMIframeType3;
        public Dictionary<eSPMICMDTYPE, int> SPMIFrameType3
        {
            get
            {
                return SPMIframeType3;
            }

            set
            {
                SPMIframeType3 = value;
                RaisePropertyChanged("SPMIFrameType3");
            }
        }
     Dictionary<eRFFECMDTYPE, int> RFFEframeType;
        public Dictionary<eRFFECMDTYPE, int> RFFEFrameType
        {
            get
            {
                return RFFEframeType;
            }

            set
            {
                RFFEframeType = value;
                RaisePropertyChanged("RFFEFrameType");
            }
        }
        Dictionary<eRFFECMDTYPE, int> RFFEframeType1;
        public Dictionary<eRFFECMDTYPE, int> RFFEFrameType1
        {
            get
            {
                return RFFEframeType1;
            }

            set
            {
                RFFEframeType1 = value;
                RaisePropertyChanged("RFFEFrameType1");
            }
        }

        Dictionary<eRFFECMDTYPE, int> RFFEframeType2;
        public Dictionary<eRFFECMDTYPE, int> RFFEFrameType2
        {
            get
            {
                return RFFEframeType2;
            }

            set
            {
                RFFEframeType2 = value;
                RaisePropertyChanged("RFFEFrameType2");
            }
        }
        #endregion



        #region I3C

        private readonly object LockI3C_Update = new object();

        public void I3CSDRFrameType_Increment(eMajorFrame frameType)
        {
            lock (LockI3C_Update)
            {
                if (I3CSDRFrameType.ContainsKey(frameType) == false)
                    I3CSDRFrameType.Add(frameType, 0);

                I3CSDRFrameType[frameType]++;
            }
        }

        public void I3CDDRFrameType_Increment(eCommand cmd)
        {
            lock (LockI3C_Update)
            {
                if (I3CDDRFrameType.ContainsKey(cmd) == false)
                    I3CDDRFrameType.Add(cmd, 0);

                I3CDDRFrameType[cmd]++;
            }
        }

        public void I3CTSLFrameType_Increment(eCommand cmd)
        {
            lock (LockI3C_Update)
            {
                if (I3CTSLFrameType.ContainsKey(cmd) == false)
                    I3CTSLFrameType.Add(cmd, 0);

                I3CTSLFrameType[cmd]++;
            }
        }

        public void I3CTSPFrameType_Increment(eCommand cmd)
        {
            lock (LockI3C_Update)
            {
                if (I3CTSPFrameType.ContainsKey(cmd) == false)
                    I3CTSPFrameType.Add(cmd, 0);

                I3CTSPFrameType[cmd]++;
            }
        }

        Dictionary<eMajorFrame, int> I3CSDRframeType;
        public Dictionary<eMajorFrame, int> I3CSDRFrameType
        {
            get
            {
                return I3CSDRframeType;
            }

            set
            {
                I3CSDRframeType = value;
                RaisePropertyChanged("I3CSDRFrameType");
            }
        }

        Dictionary<eCommand, int> I3CDDRframeType;
        public Dictionary<eCommand, int> I3CDDRFrameType
        {
            get
            {
                return I3CDDRframeType;
            }

            set
            {
                I3CDDRframeType = value;
                RaisePropertyChanged("I3CDDRFrameType");
            }
        }
        Dictionary<eCommand, int> I3CTSLframeType;
        public Dictionary<eCommand, int> I3CTSLFrameType
        {
            get
            {
                return I3CTSLframeType;
            }

            set
            {
                I3CTSLframeType = value;
                RaisePropertyChanged("I3CTSLFrameType");
            }
        }
        Dictionary<eCommand, int> I3CTSPframeType;
        public Dictionary<eCommand, int> I3CTSPFrameType
        {
            get
            {
                return I3CTSPframeType;
            }

            set
            {
                I3CTSPframeType = value;
                RaisePropertyChanged("I3CTSPFrameType");
            }
        }

        private int sDRCount;
        public int SDRCount
        {
            get
            {
                return sDRCount;
            }
            set
            {
                sDRCount = value;
                RaisePropertyChanged("SDRCount");
            }
        }
       private int dDRCount;
        public int DDRCount
        {
            get
            {
                return dDRCount;
            }
            set
            {
                dDRCount = value;
                RaisePropertyChanged("DDRCount");
            }
        }
        private int tSLCount;
        public int TSLCount
        {
            get
            {
                return tSLCount;
            }
            set
            {
                tSLCount = value;
                RaisePropertyChanged("TSLCount");
            }
        }
        private int tSPCount;
        public int TSPCount
        {
            get
            {
                return tSPCount;
            }
            set
            {
                tSPCount = value;
                RaisePropertyChanged("TSPCount");
            }
        }


        private int allCommandCount;
        public int AllCommandCount
        {
            get
            {
                return allCommandCount;
            }
            set
            {
                allCommandCount = value;
                RaisePropertyChanged("AllCommandCount");
            }
        }

        private int restartPacketCount;
        public int RestartPacketCount
        {
            get
            {
                return restartPacketCount;

            }
            set
            {
                restartPacketCount = value;
                RaisePropertyChanged("RestartPacketCount");
            }
        }
        int exitPacketCount;
        public int ExitPacketCount
        {
            get
            {
                return exitPacketCount;

            }
            set
            {
                exitPacketCount = value;
                RaisePropertyChanged("ExitPacketCount");
            }
        }

        #endregion


        ObservableCollection<int> writePacketCounter;
        ObservableCollection<int> readPacketCounter;
        ObservableCollection<int> frameCounter;
        ObservableCollection<int> allPacketCounter;

        public void Initialize()
        {
            FrameCounter.Add(0); // for I2C
            FrameCounter.Add(0); // for SPI
            FrameCounter.Add(0); // for UART
            FrameCounter.Add(0); // for I3C
            FrameCounter.Add(0); // for spmi
            FrameCounter.Add(0); //for RFFE
            FrameCounter.Add(0); //For Can
            FrameCounter.Add(0); //for QSPI
            FrameCounter[(int)eProtocol.I2C] = 0;
            FrameCounter[(int)eProtocol.SPI] = 0;
            FrameCounter[(int)eProtocol.UART] = 0;
            FrameCounter[(int)eProtocol.SPMI] = 0;
            FrameCounter[(int)eProtocol.RFFE] = 0;
            FrameCounter[(int)eProtocol.I3C] = 0;
            FrameCounter[(int)eProtocol.CAN] = 0;
            FrameCounter[(int)eProtocol.QSPI] = 0;


            remoteFrame.Add(0); // for I2C
            remoteFrame.Add(0); // for SPI
            remoteFrame.Add(0); // for UART
            remoteFrame.Add(0); // for I3C
            remoteFrame.Add(0); // for spmi
            remoteFrame.Add(0);
            remoteFrame.Add(0);
            remoteFrame[(int)eProtocol.CAN] = 0;


            overloadFrame.Add(0); // for I2C
            overloadFrame.Add(0); // for SPI
            overloadFrame.Add(0); // for UART
            overloadFrame.Add(0); // for I3C
            overloadFrame.Add(0); // for spmi
            overloadFrame.Add(0);
            overloadFrame.Add(0);
            overloadFrame[(int)eProtocol.CAN] = 0;

            ErrorPacket.Add(0); // for I2C
            ErrorPacket.Add(0); // for SPI
            ErrorPacket.Add(0); // for UART
            ErrorPacket.Add(0); // for I3C
            ErrorPacket.Add(0); // for rffe
            ErrorPacket.Add(0);
            ErrorPacket.Add(0);
            ErrorPacket.Add(0); //for CAN
            ErrorPacket[(int)eProtocol.I2C] = 0;
            ErrorPacket[(int)eProtocol.SPI] = 0;
            ErrorPacket[(int)eProtocol.UART] = 0;
            ErrorPacket[(int)eProtocol.I3C] = 0;
            ErrorPacket[(int)eProtocol.CAN] = 0;
            ErrorPacket[(int)eProtocol.QSPI] = 0;

            dataPacket1.Add(0); // for I2C
            dataPacket1.Add(0); // for SPI
            dataPacket1.Add(0); // for UART
            dataPacket1.Add(0); // for I3C
            dataPacket1.Add(0); // for RFFE
            dataPacket1.Add(0); // for SPMI
            dataPacket1.Add(0); //for QSPI
            dataPacket1.Add(0);
            dataPacket1[(int)eProtocol.I2C] = 0;
            dataPacket1[(int)eProtocol.SPI] = 0;
            dataPacket1[(int)eProtocol.UART] = 0;
            dataPacket1[(int)eProtocol.I3C] = 0;
            dataPacket1[(int)eProtocol.CAN] = 0;
            dataPacket1[(int)eProtocol.QSPI] = 0;
            //dataPacket1[(int)eProtocol.RFFE] = 0;

            dataPacket2.Add(0); // for I2C
            dataPacket2.Add(0); // for SPI
            //dataPacket2.Add(0); // for QSPI
            dataPacket2.Add(0); // for UART
            dataPacket2.Add(0); // for i3c
            //dataPacket2.Add(0); // for I2C
            //dataPacket2.Add(0); // for SPI
            dataPacket2[(int)eProtocol.I2C] = 0;
            dataPacket2[(int)eProtocol.SPI] = 0;
            dataPacket2[(int)eProtocol.UART] = 0;
            dataPacket2[(int)eProtocol.I3C] = 0;

            writePacketCounter.Add(0); // for I2C
            writePacketCounter.Add(0);
            writePacketCounter.Add(0);
            writePacketCounter.Add(0);
            writePacketCounter.Add(0);
            writePacketCounter.Add(0);
            writePacketCounter[(int)eProtocol.I2C] = 0;
            writePacketCounter[(int)eProtocol.SPMI] = 0;
            writePacketCounter[(int)eProtocol.RFFE] = 0;


            readPacketCounter.Add(0); // for I2C
            readPacketCounter.Add(0);
            readPacketCounter.Add(0);
            readPacketCounter.Add(0);
            readPacketCounter.Add(0);
            readPacketCounter.Add(0);
            readPacketCounter[(int)eProtocol.I2C] = 0;
            readPacketCounter[(int)eProtocol.SPMI] = 0;
            readPacketCounter[(int)eProtocol.RFFE] = 0;

            allPacketCounter.Add(0);
            allPacketCounter.Add(0);
            allPacketCounter.Add(0);
            allPacketCounter.Add(0);
            allPacketCounter.Add(0);
            allPacketCounter.Add(0);

            allPacketCounter[(int)eProtocol.RFFE] = 0;
            allPacketCounter[(int)eProtocol.SPMI] = 0;
            allPacketCounter[(int)eProtocol.I3C] = 0;
            //I3C

            crcPacket[(int)eProtocol.I3C] = 0;
            addressPacket[(int)eProtocol.I3C] = 0;
            
            
            //QSPI
            
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter.Add(0);
            Commandcounter[(int)eProtocol.QSPI] = 0;



            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter.Add(0);
            Addresscounter[(int)eProtocol.QSPI] = 0;





            SDRCount = 0;
            DDRCount = 0;
            TSLCount = 0;
            TSPCount = 0;
            AllCommandCount = 0;  
            ReadKBByte1 = 0;
            WriteKBByte1 = 0;
            writeCount1 = 0;
            ReadCount1 = 0;
            ReadKBByte = 0;
            WriteKBByte = 0;
            writeCount = 0;
            ReadCount = 0;

            RestartPacketCount = 0;
            ExitPacketCount = 0;
            I3CSDRFrameType = new Dictionary<eMajorFrame, int>();

            I3CDDRFrameType = new Dictionary<eCommand, int>();
            I3CDDRFrameType.Add(eCommand.ENTHDR0, 0);
            I3CTSLFrameType = new Dictionary<eCommand, int>();
            I3CTSLFrameType.Add(eCommand.ENTHDR1, 0);
            I3CTSPFrameType = new Dictionary<eCommand, int>();
            I3CTSPFrameType.Add(eCommand.ENTHDR2, 0);

            //SPMI
            DataErrorPacket = 0;
            AddrerrorPacket = 0;
            CommandErrorPacket = 0;
            SPMIFrameType = new Dictionary<eSPMICMDTYPE, int>();
            SPMIFrameType1 = new Dictionary<eSPMICMDTYPE, int>();
            SPMIFrameType2 = new Dictionary<eSPMICMDTYPE, int>();
            SPMIFrameType3 = new Dictionary<eSPMICMDTYPE, int>();   
            foreach (var item in Enum.GetValues(typeof(eSPMICMDTYPE)).Cast<eSPMICMDTYPE>())
            {
                SPMIFrameType.Add(item, 0);
            }

            //RFFE


            RFFEFrameType = new Dictionary<eRFFECMDTYPE, int>();
            RFFEFrameType1 = new Dictionary<eRFFECMDTYPE, int>();
            RFFEFrameType2 = new Dictionary<eRFFECMDTYPE, int>();
            //foreach (var item in Enum.GetValues(typeof(eRFFECMDTYPE)).Cast<eRFFECMDTYPE>())
            //{
            //    RFFEFrameType2.Add(item, 0);
            //}




        }

        public void Reset()
        {
            writePacketCounter = new ObservableCollection<int>();
            readPacketCounter = new ObservableCollection<int>();
            FrameCounter = new ObservableCollection<int>();
            allPacketCounter = new ObservableCollection<int>();
            ErrorPacket = new ObservableCollection<int>();
            commandcounter = new ObservableCollection<int>();
            dataPacket1 = new ObservableCollection<int>();
            dataPacket2 = new ObservableCollection<int>();
            addressPacket = new ObservableCollection<int>();
            crcPacket = new ObservableCollection<int>();
            remoteFrame = new ObservableCollection<int>();
            overloadFrame = new ObservableCollection<int>();
            I3CSDRFrameType = new Dictionary<eMajorFrame, int>();
            I3CDDRFrameType = new Dictionary<eCommand, int>();
            I3CTSLFrameType = new Dictionary<eCommand, int>();
            I3CTSPFrameType = new Dictionary<eCommand, int>();
        }



        public ObservableCollection<int> ErrorPacket
        {
            get
            {
                return errorPacket;
            }
            set
            {
                errorPacket = value;
                RaisePropertyChanged("ErrorPacket");
            }
        }
        ObservableCollection<int> errorPacket;


        //QSPI
        public ObservableCollection<int> Commandcounter
        {
            get
            {
                return commandcounter;

            }
            set
            {
                commandcounter = value;
                RaisePropertyChanged("Commandcounter");
            }
        }
        ObservableCollection<int> commandcounter;



        public ObservableCollection<int> Addresscounter
        {
            get
            {
                return addresscounter;

            }
            set
            {
                addresscounter = value;
                RaisePropertyChanged("Addresscounter");
            }
        }
        ObservableCollection<int> addresscounter;



        public ObservableCollection<int> DataPacket1
        {
            get
            {
                return dataPacket1;
            }
            set
            {
                dataPacket1 = value;
                RaisePropertyChanged("DataPacket1");
            }
        }




        public ObservableCollection<int> DataPacket2
        {
            get
            {
                return dataPacket2;
            }
            set
            {
                dataPacket2 = value;
                RaisePropertyChanged("DataPacket2");
            }
        }

        public ObservableCollection<int> FrameCounter
        {
            get
            {
                return frameCounter;
            }

            set
            {
                frameCounter = value;
                RaisePropertyChanged("FrameCounter");
            }
        } 

     
        private ObservableCollection<int> addressPacket;

        public ObservableCollection<int> AddressPacket
        {
            get
            {
                return addressPacket;
            }
            set
            {
                addressPacket = value;
                RaisePropertyChanged("AddressPacket");
            }
        }

        private ObservableCollection<int> crcPacket;
        public ObservableCollection<int> CrcPacket
        {
            get
            {
                return crcPacket;
            }
            set
            {
                crcPacket = value;
                RaisePropertyChanged("CrcPacket");
            }
        }
        public ObservableCollection<int> WritePacketCounter
        {
            get
            {
                return writePacketCounter;
            }

            set
            {
                writePacketCounter = value;
                RaisePropertyChanged("WritePacketCounter");
            }
        }

        public ObservableCollection<int> ReadPacketCounter
        {
            get
            {
                return readPacketCounter;
            }

            set
            {
                readPacketCounter = value;
                RaisePropertyChanged("ReadPacketCounter");
            }
        }

        public ObservableCollection<int> AllPacketCounter
        {
            get
            {
                return allPacketCounter;
            }

            set
            {
                allPacketCounter = value;
                RaisePropertyChanged("AllPacketCounter");
            }
        }
        
        
      
        public double ReadKBByte
        {
            get
            {
                return ReadkBCount;
            }

            set
            {
                ReadkBCount = value;
                RaisePropertyChanged("ReadKBByte");
            }
        }

        public double WriteKBByte
        {
            get
            {
                return WritekBByte;
            }

            set
            {
                WritekBByte = value;

                RaisePropertyChanged("WriteKBByte");
            }
        }

        public int writeCount
        {
            get
            {
                return writecount;
            }

            set
            {
                writecount = value;
                RaisePropertyChanged("writeCount");
            }
        }

        public int ReadCount
        {
            get
            {
                return Readcount;
            }

            set
            {
                Readcount = value;
                RaisePropertyChanged("ReadCount");
            }
        }

        public double ReadKBByte1
        {
            get
            {
                return ReadkBByte1;
            }

            set
            {
                ReadkBByte1 = value;
                RaisePropertyChanged("ReadKBByte1");
            }
        }

        public double WriteKBByte1
        {
            get
            {
                return WritekBByte1;
            }

            set
            {
                WritekBByte1 = value;

                RaisePropertyChanged("WriteKBByte1");
            }
        }

        public int writeCount1
        {
            get
            {
                return writecount1;
            }

            set
            {
                writecount1 = value;
                RaisePropertyChanged("writeCount1");
            }
        }

        public int ReadCount1
        {
            get
            {
                return Readcount1;
            }

            set
            {
                Readcount1 = value;
                RaisePropertyChanged("ReadCount1");
            }
        }

      
        public ObservableCollection<int> RemoteFrame
        {
            get
            {
                return remoteFrame;
            }
            set
            {
                remoteFrame = value;
                RaisePropertyChanged("RemoteFrame");
            }
        }

        public ObservableCollection<int> OverloadFrame
        {
            get
            {
                return overloadFrame;
            }
            set
            {
                overloadFrame = value;
                RaisePropertyChanged("OverloadFrame");
            }
        }

        ObservableCollection<int> dataPacket1;
        ObservableCollection<int> dataPacket2;
        ObservableCollection<int> remoteFrame;
        ObservableCollection<int> overloadFrame;    
        double ReadkBCount;
        double WritekBByte;
        int writecount;
        int Readcount;
        double ReadkBByte1;
        double WritekBByte1;
        int writecount1;
        int Readcount1;
    
    }
}
