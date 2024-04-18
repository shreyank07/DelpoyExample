using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.SPIStructure
{
    public enum BitStatus { LOW = 0, HIGH = 1, NP = 2, NA = 3 };
    public class SPIDataByte
    {
        private string m_MOSIData;
        private string m_MISOData;
        private BitStatus m_StartBit;
        private BitStatus m_StopBit;
        private double m_StartWfmIndex;
        private double m_StopWfmIndex;
        public String _AckErrorDescrip;
        private long m_csEdgeIndex;
        private double m_Maximum_voltage;
        private double m_Minimum_voltage;
        private string m_Time;

        public bool ProtcolTestPassed;
        public int NakCount;
        private double m_SampleInterval;
        
        public SPIDataByte()
        {

        }
        public long CSedgeindex
        {
            get
            {
                return m_csEdgeIndex;
            }
            set
            {
                m_csEdgeIndex = value;
            }
        }

        public double SampleInterval
        {
            get { return m_SampleInterval; }
            set
            {
                m_SampleInterval = value;
            }
        }

        public int IntMOSIData
        {
            get
            {
                int retVal = -1;
                try
                {
                    retVal = int.Parse(m_MOSIData);
                }
                catch
                {
                }
                return retVal;
            }
            set
            {
                m_MOSIData = value.ToString();
            }
        }

        public int IntMISOData
        {
            get
            {
                int retVal = -1;
                try
                {
                    retVal = int.Parse(m_MISOData);
                }
                catch
                {
                }
                return retVal;
            }
            set
            {
                m_MISOData = value.ToString();
            }
        }


        public String AckErrorDescrip
        {
            get
            {
                return _AckErrorDescrip;
            }
            set
            {
                _AckErrorDescrip = value;
            }
        }
        

        public String MOSIData
        {
            get
            {
                string retString;
                if (m_MOSIData.Length >= 1)
                {
                    try
                    {
                        if (m_MOSIData.StartsWith("0x") || m_MOSIData == "-")
                        {
                            retString = m_MOSIData;
                            return retString;
                        }
                        else
                        {
                            //  int numberOfBitsPerData = int.Parse(m_setting.BitData);
                            // retString = int.Parse(m_MOSIData).ConvertFormat(NumberFormat.Hex);
                            retString = "0";
                        }
                    }
                    catch
                    {
                        retString = m_MOSIData;
                    }
                }
                else
                {
                    retString = "-";
                }
                return retString;
            }
            set
            {
                try
                {
                    m_MOSIData = value.ToString();
                }
                catch
                {
                }

            }
        }

        public string MISOData
        {
            get
            {
                string retString;
                if (m_MISOData.Length >= 1)
                {
                    try
                    {
                        // int numberOfBitsPerData = int.Parse(m_setting.BitData);
                        //retString = int.Parse(m_MISOData).ConvertFormat(NumberFormat.Hex);
                        retString = "0";
                    }
                    catch
                    {
                        retString = m_MISOData;
                    }
                }
                else
                {
                    retString = "-";
                }
                return retString;
            }
            set
            {
                try
                {
                    m_MISOData = value.ToString();
                }
                catch
                {
                }

            }
        }


        public BitStatus StartBit
        {
            get
            {
                return m_StartBit;
            }
            set
            {
                m_StartBit = value;
            }
        }

        public BitStatus StopBit
        {
            get
            {
                return m_StopBit;
            }
            set
            {
                m_StopBit = value;
            }
        }

        public String StringStartBit
        {
            get
            {
                String retVal;
                if ((int)m_StartBit <= 1)
                {
                    retVal = ((int)m_StartBit).ToString();
                }
                else
                {
                    retVal = m_StartBit.ToString();
                }
                return retVal;


            }
            set
            {
            }
        }

        public String StringStopBit
        {
            get
            {
                String retVal;
                if ((int)m_StopBit <= 1)
                {
                    retVal = ((int)m_StopBit).ToString();
                }
                else
                {
                    retVal = m_StopBit.ToString();
                }
                return retVal;
            }
            set
            {
            }
        }


        public double WfmStartIndex
        {
            get
            {
                return m_StartWfmIndex;
            }
            set
            {
                m_StartWfmIndex = value;
            }
        }

        public double WfmStopIndex
        {
            get
            {
                return m_StopWfmIndex;
            }
            set
            {
                m_StopWfmIndex = value;
            }
        }

        public string Time
        {
            get
            {
                return m_Time;
            }
            set
            {
                m_Time = value;
            }

        }
        public double Maximum_Voltage
        {
            get
            {
                return m_Maximum_voltage;
            }
            set
            {
                m_Maximum_voltage = value;
            }
        }

        public double Minimum_Voltage
        {
            get
            {
                return m_Minimum_voltage;
            }
            set
            {
                m_Minimum_voltage = value;
            }
        }
    }
}
