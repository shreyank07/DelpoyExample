using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule
{
    public enum eVersion { two, one }
    public enum eSPMIErrorType : byte
    {
        [Description("No Error")]
        Pass,
        [Description("Command Parity Error")]
        Cp,
        [Description("Address Parity Error")]
        Ap,
        [Description("Data Parity Error")]
        Dp,
        [Description("ByteCount Parity Error")]
        Bp,
        [Description("ByteCount MisMatch Error")]
        Mis,
        [Description("Nack Error")]
        Nack,
    }


    public enum eParity : byte { Even, Odd };

    public enum eTransfer
    {
        Any,
        WR,
        RD
    }

    public enum eType
    {
        Any,
        ACK,
        NACK
    }

    public enum eI2CList
    {

        [Description("Address")]
        Address,
        [Description("Data")]
        Data,

        [Description("Read/Write")]
        Read_Write,

        [Description("Ack/Nack")]
        Ack_Nack,

        [Description("Start/Repeated Start/Stop")]
        Start_Repeated_Start_Stop,
    }
    public enum eRFFEErrorType : byte
    {
        [Description("No Error")]
        Pass,
        [Description("Command Parity Error")]
        Cp,
        [Description("Address Parity Error")]
        Ap,
        [Description("Data Parity Error")]
        Dp,
        [Description("ByteCount Parity Error")]
        Bp,
        [Description("ByteCount MisMatch Error")]
        Mis,
        [Description("Interrupt Slots Overflow Error")]
        Intr,
    }

    public enum eDataFormat
    {
        Binary,
        Decimal,
        Hex,
        Octal,
        ASCII,
        Gray_Code,
        _2s_Complement
    }
    public enum eChannelList
    {
        CH1,
        CH2,
        CH3,
        CH4,
        CH5,
        CH6,
        CH7,
        CH8,
        CH9,
        CH10,
        CH11,
        CH12,
        CH13,
        CH14,
        CH15,
        CH16
    }
    public enum eBitOrder { LSBFirst, MSBFirst }
    public enum eBufferSize
    {
        [Description("100 KB")]
        _100KB,
        [Description("500 KB")]
        _500KB,
        [Description("1 MB")]
        _1MB,
        [Description("2 MB")]
        _2MB,
        [Description("Continuous (> 2MB)")]
        Continuous_2MB
    }

    public enum eConfigMode { LA_Mode, PA_Mode, Both }

    public enum eProbeType { Type1, Type2 }
    public enum eGeneralPurpose { State, Timing }
    public enum eChannles { None = -1, CH1, CH2, CH3, CH4, CH5, CH6, CH7, CH8, CH9, CH10, CH11, CH12, CH13, CH14, CH15, CH16 }

    public enum eGrouping { Group1, Group2, Group3 }

    public enum eVoltage
    {
        [Description("1.0V")]
        _1_0V,
        [Description("1.2V")]
        _1_2V,
        [Description("1.8V")]
        _1_8V,
        [Description("2.5V")]
        _2_5V,
        [Description("3.3V")]
        _3_3V,
        [Description("5.0V")]
        _5_0V
    }
    public enum eResponseFlag : int
    {
        [Description("Operation Success")]
        Success,
        [Description("FX3 Detect or Status Fail")]
        FX3Fail,
        [Description("Operation Fail")]
        Fail,
        [Description("Connection not establish with PGY Hardware")]
        Connection_Fail,
        Device_Not_Found,
        [Description("Reaches the maximum limit")]
        Device_Err_Max,
        [Description("One or more device having same unique Id (ex: PID, Static)")]
        Device_Err_Conflict,
        [Description("Input data format is invalid")]
        Invalid_Data,
        [Description("Task is currently busy, either stop capture or wait until the session")]
        Task_Busy,
        [Description("Given path doesnot exist")]
        File_Not_Exist,
        [Description("Invalid Trace File - Either no files or some other file found along with folder")]
        Trace_Invalid,
        Not_Implemented,
        [Description("No result found")]
        No_Result,
        [Description("Time Out")]
        TimeOut_Error,
        File_Write_Fail,
        Directory_Not_Exist,
        Application_Exception,
        Session_Not_Found,
        CaptureAppNotFound,
        Invalid_DataLength,
        [Description("Duplicate channel already selected")]
        Duplicate_Channel
    }
    public enum CaptureState
    {
        RUN,
        STOPACQ,
        STOP
    }

    public enum eDataWidth { _5_bit, _6_bit, _7_bit, _8_bit }
    public enum eIntType : byte { Start, Idle };
 
    public enum eSourceType { Live, Offline };
    public enum eCustomStatus { Success, Failure, Information };

    public enum eAnalyzerMode { I3C, I2C_only }

    public enum eBoard : byte { HWBoard2_0, HWBoard1_0, HWBoard4_0 }

    public enum eRxStatus { Wait, Success }

    public enum eNodeType : byte { Master = 1, Sec_Master, I3C_Slave, I2C_Slave }

    public enum eTrafficType : byte { I2C_Message, I3C_Private_Message, I3C_Directed, I3C_BroadCast, I3C_DDR, I3C_TSP, I3C_TSL }

    public enum eAddressType : byte { NA, BroadCast_Address, I3C_Slave_Address, I3C_Reserved, I2C_Slave_Address, BroadCast_Address_Error, Hot_Joint_address, I3C_Slave_OR_Legacy_I2C, Static_Address_SETDASA }

    public enum eDataType : byte { Data, PID_Data, BCR_Data, DCR_Data, LVR_Data, MSCL_Data, Status_Data, XTIME_Data, MIDI_Data, ENEC_Data, DISEC_Data, HDRCAP_Data, ENTTM_Data }

    public enum eEdgeType : byte { FALLING_EDGE, RISING_EDGE, NO_EDGE };

    public enum eGroupType { Group, Individual }

    public enum eSampleRate { SR_125, SR_142, SR_166, SR_200, SR_250, SR_333, SR_500, SR_1000 }

    public enum eProtocolState : byte { START, STOP, BUSY };

    public enum eAnnotationInfo
    {
        ADDRESS = 0,
        DATA = 1,
        ACK = 2,
        START = 3,
        STOP = 4,
        RW = 5,
        NONE
    }

    public enum eframeformates : byte
    {
        [Description("11 Bit Identifier")]
        STD,
        [Description("29 Bit Identifier")]
        EXT
    }
    public enum eWfmState : byte { LOW = 0, HIGH = 1, QUASI = 2 };
    public enum eErrorType :byte 
    {
        [Description("No Error")]
        None,
        [Description("Broadcast Address/W")]
        S0,
        [Description("CCC Code")]
        S1,
        [Description("WR Data")]
        S2,
        [Description("Assigned Address during Dynamic Address Arbitration")]
        S3,
        [Description("7’h7E/R after Sr during Dynamic Address Arbitration")]
        S4,
        [Description("Transaction after detecting CCC")]
        S5,
        [Description("Monitoring Error")]
        S6,
        [Description("Transaction after sending CCC")]
        M0,
        [Description("Monitoring Error")]
        M1,
        [Description("No response to Broadcast Address")]
        M2,
        [Description("Insufficient bit to complete word")]
        InSufficientBits,
        [Description("Preamble Error")]
        Preamble,
        [Description("Command Parity Error")]
        Cp,
        [Description("Address Parity Error")]
        Ap,
        [Description("Data Parity Error")]
        Dp,
        [Description("ByteCount Parity Error")]
        Bp,
        [Description("ByteCount MisMatch Error")]
        ByteCountMismatch,
        [Description("Nack Error")]
        Nack,
        [Description("Interrupt Slots Overflow Error")]
        Intr,
        [Description("CRC Error")]
        CRCError,
        [Description("Form Error")]
        FormError,
        [Description("BitStuff Error")]
        BitStuffError,
        [Description("BitError")]
        BitError,
        [Description("ACK Error")]
        AckError,
        [Description("Parity Check Failed")]
        Parity_Error,
        No_HDRExit,
    }
    public enum eProtocolMode : byte
    {
        SDR = 0,
        HDR_DDR = 1,
        HDR_TSP = 2,
        HDR_TSL = 3,
        HDR,
        NA
    };

    public enum eDataDescription : byte
    {
        Data,
        BCR,
        DCR,
        BCRValue,
        DCRValue,
        Count,
        DAADynamicAddress,
        DynamicAddress,
        BridgeMSB,
        BridgeLSB,
        StaticAddress
    }

    public enum eFrameType : byte
    {
        BROADCAST,
        DIRECTED,
        PRIVATE,
        HOTJOIN,
        I2C_MESSAGE,
        MatershipRequest,
        IBI,
        NA,
        HDRExitPattern,
        SlaveResetPattern,
        IBI_Response
    }

    public enum ePacketType : byte
    {
        Command,
        Address,
        Data,
        PIDDAA,
        HDR_Command,
        HDR_Data,
        HDR_CRC,
        HDR_Exit,
        HDR_Restart,
        Slave_Reset,
        None
    }

    public enum eProtocolType : byte
    { I3C, I2C };

    public enum eI3CMessage:byte
    {
        Broadcast,
        Directed,
        Private
    }
    public enum eAcknowledgeType
    { ACK, NACK, NA = -1 }

    public enum eHostDevice : byte
    {
        NA, Master, Slave
    }

    public enum ePreambleState : byte
    {
        Command, Read_Command, Read_Data, Write_Command, Write_Data, CRC, NA
    }

    public enum eTransferType : sbyte
    {
         WR, RD, Both, RegisterRead, AUTHENTICATE, MOH, NA = -1,
    }
    public enum eCommandPattern { A, B }
    public enum eI2CAddressType { _7bAddress, _8bAddress, _10bAddress }

    public enum eStartType : byte { S, Sr };
    public enum eStartStop : byte { P, NA }
    public enum eReportType { CSV, PDF }

    public enum eSPIPhase : byte { Low, High };

    public enum eSPIPolarity : byte { Low, High };




    //QSPI
    public enum eQSPIMode { Extended = 0, Dual = 1, Quad = 2 };

    public enum eQSPIPolarity : byte { Low, High };


    public enum eQSPIAddress : sbyte { FourByte = 0x04, ThreeByte = 0x03 };

    public enum eQSPIPhase : byte { Low, High };

    public enum eQSPIChipSelect : sbyte  {NA = -1, Low, High };

    

    public enum eQSPITransferRate
    {
        STR,
        DTR
    }

    public enum eQSPICommands
    {
         [Description("32KB SUBSECTOR ERASE")]
        _32KB_Subsector_Erase,
        [Description("4-BYTE 4KB SUBSECTOR ERASE")]
        _4Byte_4KB_Subsector_Erase,
        [Description("4-BYTE DTR DUAL INPUT/OUTPUT FAST READ")]
        _4Byte_DTR_Dual_Input_Output_Fast_Read,
        [Description("4-BYTE DTR FAST READ")]
        _4Byte_DTR_Fast_Read,
        [Description("4-BYTE DTR QUAD INPUT/ OUTPUT FAST READ")]
        _4Byte_DTR_Quad_Input_Output_Fast_Read,
        [Description("4-BYTE DUAL INPUT/OUTPUT FAST READ")]
        _4Byte_Dual_Input_Output_Fast_Read,
        [Description("4-BYTE DUAL OUTPUT FAST READ")]
        _4Byte_Dual_Output_Fast_Read,
        [Description("4-BYTE PAGE PROGRAM")]
        _4Byte_Page_Program,
        [Description("4-BYTE QUAD INPUT EXTENDED FAST PROGRAM")]
        _4Byte_Quad_Input_Extended_Fast_Program,
        [Description("4-BYTE QUAD INPUT FAST PROGRAM")]
        _4Byte_Quad_Input_Fast_Program,
        [Description("4-BYTE QUAD INPUT FAST READ")]
        _4Byte_Quad_Input_Output_Fast_Read,
        [Description("4-BYTE QUAD OUTPUT FAST READ")]
        _4Byte_Quad_Output_Fast_Read,
        [Description("4-BYTE FAST READ")]
        _4Byte_Fast_Read,
        [Description("4-BYTE READ")]
        _4Byte_Read,
        [Description("4-BYTE READ VOLATILE LOCK BITS")]
        _4Byte_Read_Volatile_Lock_Bits,
        [Description("4-BYTE WRITE VOLATILE LOCK BITS")]
        _4Byte_Write_Volatile_Lock_Bits,
        [Description("4-BYTE SECTOR ERASE")]
        _4Byte_Sector_Erase,
        [Description("4-BYTE VOLATILE CONFIGURATION REGISTER")]
        _4Byte_Volatile_Configuration_Register,
        [Description("BULK ERASE")]
        Bulk_Erase,
        [Description("CLEAR FLAG STATUS REGISTER")]
        Clear_Flag_Status_Register,
        [Description("CYCLIC REDUNDANCY CHECK")]
        Cyclic_Redundancy_Check,
        [Description("DTR DUAL INPUT/OUTPUT FAST READ")]
        DTR_Dual_Input_Output_Fast_Read,
        [Description("DTR DUAL OUTPUT FAST READ")]
        DTR_Dual_Output_Fast_Read,
        [Description("DTR FAST READ")]
        DTR_Fast_Read,
        [Description("DTR QUAD INPUT/OUTPUT FAST READ")]
        DTR_Quad_Input_Output_Fast_Read,
        [Description("DTR QUAD OUTPUT FAST READ")]
        DTR_Quad_Output_Fast_Read,
        [Description("DUAL INPUT/OUTPUT FAST READ")]
        Dual_Input_Output_Fast_Read,
        [Description("DUAL INPUT FAST PROGRAM")]
        Dual_Input_Fast_Program,
        [Description("DUAL OUTPUT FAST READ")]
        Dual_Output_Fast_Read,
        [Description("ENTER 4-BYTE ADDRESS MODE")]
        Enter_4Byte_Address_Mode,
        [Description("ENTER DEEP POWER DOWN")]
        Enter_Deep_Power_Down,
        [Description("ENTER QUAD INPUT/OUTPUT MODE")]
        Enter_Quad_Input_Output_Mode,
        [Description("ERASE NONVOLATILE LOCK BITS")]
        Erase_Non_Volatile_Lock_Bits,
        [Description("EXIT 4-BYTE ADDRESS MODE")]
        Exit_4Byte_Address_Mode,
        [Description("EXTENDED DUAL INPUT FAST PROGRAM")]
        Extended_Dual_Input_Fast_Program,
        [Description("EXTENDED QUAD INPUT FAST PROGRAM")]
        Extended_Quad_Input_Fast_Program,
        [Description("FAST READ")]
        Fast_Read,
        [Description("INTERFACE ACTIVATION")]
        Interface_Activation,
        [Description("MULTIPLE I/O READ ID")]
        Mulitple_IO_Read_ID,
        [Description("PAGE PROGRAM")]
        Page_Program,
        [Description("PROGRAM/ERASE RESUME")]
        Program_Erase_Resume,
        [Description("PROGRAM/ERASE SUSPEND")]
        Program_Erase_Suspend,
        [Description("PROGRAM SECTOR PROTECTION")]
        Program_Sector_Protection,
        [Description("PROGRAM OTP ARRAY")]
        Program_OTP_Array,
        [Description("QUAD INPUT/OUTPUT FAST READ")]
        Quad_Input_Output_Fast_Read,
        [Description("QUAD INPUT/OUTPUT WORD READ")]
        Quad_Input_Output_Word_Read,
        [Description("QUAD INPUT FAST PROGRAM")]
        Quad_Input_Fast_Program,
        [Description("QUAD OUTPUT FAST READ")]
        Quad_Output_Fast_Read,
        [Description("READ")]
        Read,
        [Description("READ ENHANCED VOLATILE CONFIGURATION REGISTER")]
        Read_Enhanced_Volatile_Configuration_Register,
        [Description("READ EXTENDED ADDRESS REGISTER")]
        Read_Extended_Address_Register,
        [Description("READ FLAG STATUS REGISTER")]
        Read_Flag_Status_Register,
        [Description("READ GLOBAL FREEZE BIT")]
        Read_Global_Freeze_Bits,
        [Description("READ ID_9E")]
        Read_ID_9E,
        [Description("READ ID_9F")]
        Read_ID_9F,
        [Description("READ NONVOLATILE LOCK BITS")]
        Read_Non_Volatile_Lock_Bits,
        [Description("READ MEMORY")]
        Read_Memory,
        [Description("READ NONVOLATILE CONFIGURATION REGISTER")]
        Read_Non_Volatile_Configuration_Register,
        [Description("READ OTP ARRAY")]
        Read_OTP_Array,
        [Description("READ PASSWORD")]
        Read_Password,
        [Description("READ SERIAL FLASH DISCOVERY PARAMETER")]
        Read_Serial_Flash_Discovery_Parameter,
        [Description("READ SECTOR PROTECTION")]
        Read_Sector_Protection,
        [Description("READ STATUS REGISTER")]
        Read_Status_Register,
        [Description("READ VOLATILE CONFIGURATION REGISTER")]
        Read_Volatile_Configuration_Register,
        [Description("READ VOLATILE LOCK BITS")]
        Read_Volatile_Lock_Bits,
        [Description("READ WRITE OTP ARRAY")]
        Read_Write_OTP_Array,
        [Description("RELEASE FROM DEEP POWERDOWN")]
        Release_From_Deep_Power_Down,
        [Description("RESET ENABLE")]
        Reset_Enable,
        [Description("RESET MEMORY")]
        Reset_Memory,
        [Description("RESET QUAD INPUT/OUTPUT MODE")]
        Reset_Quad_Input_Output_Mode,
        [Description("SECTOR ERASE")]
        Sector_Erase,
        [Description("UNLOCK PASSWORD")]
        Unlock_Password,
        [Description("WRITE DISABLE")]
        Write_Disable,
        [Description("WRITE GLOBAL FREEZE BIT")]
        Write_Global_Freeze_Bits,
        [Description("WRITE ENABLE")]
        Write_Enable,
        [Description("WRITE ENHANCED VOLATILE CONFIGURATION REGISTER")]
        Write_Enhanced_Volatile_Configuration_Register,
        [Description("WRITE EXTENDED ADDRESS REGISTER")]
        Write_Extended_Address_Register,
        [Description("WRITE NONVOLATILE CONFIGURATION REGISTER")]
        Write_Non_Volatile_Configuration_Register,
        [Description("WRITE NONVOLATILE LOCK BITS")]
        Write_Non_Volatile_Lock_Bits,
        [Description("WRITE OTP ARRAY")]
        Write_OTP_Array,
        [Description("WRITE PASSWORD")]
        Write_Password,
        [Description("WRITE STATUS REGISTER")]
        Write_Status_Register,
        [Description("WRITE VOLATILE CONFIGURATION REGISTER")]
        Write_Volatile_Configuration_Register,
        [Description("WRITE VOLATILE LOCK BITS")]
        Write_Volatile_Lock_Bits,
        Unknown,
        Any_Command
    }





    public enum eCANType : byte {  CAN, CANFD};
    public enum eFrameFormat : byte { standard, Extended};
    public enum eBroadcastCCC : byte
    {
        [Description("Enable Events Command")]
        ENEC,
        [Description("Disable Events Command")]
        DISEC,
        [Description("Enter Activity State 0")]
        ENTAS0,
        [Description("Enter Activity State 1")]
        ENTAS1,
        [Description("Enter Activity State 2")]
        ENTAS2,
        [Description("Enter Activity State 3")]
        ENTAS3,
        [Description("Reset Dynamic Address Assignment")]
        RSTDAA,
        [Description("Enter Dynamic Address Assignment")]
        ENTDAA,
        [Description("Define List of Slaves")]
        DEFSLVS,
        [Description("Set Max Write Length")]
        SETMWL,
        [Description("Set Max Write Length")]
        SETMRL,
        [Description("Enter Test Mode")]
        ENTTM,
        [Description("SET BUS CON")]
        SETBUSCON,
        [Description("ENDXFER")]
        ENDXFER = 0x12,

        [Description("Enter HDR Mode 0")]
        ENTHDR0 = 0x20,
        [Description("Enter HDR Mode 1")]
        ENTHDR1,
        [Description("Enter HDR Mode 2")]
        ENTHDR2,
        [Description("Enter HDR Mode 3")]
        ENTHDR3,
        [Description("Enter HDR Mode 4")]
        ENTHDR4,
        [Description("Enter HDR Mode 5")]
        ENTHDR5,
        [Description("Enter HDR Mode 6")]
        ENTHDR6,
        [Description("Enter HDR Mode 7")]
        ENTHDR7,
        [Description("Exchange Timing Information")]
        SETXTIME,
        SETAASA,
        RSTACT,
        DEFGRPA,
        RSTGRPA,
        SETHID = 0x61,
        DEVCTRL
    }

    public enum eDirectedCCC : byte
    {
        [Description("Enable Events Command")]
        ENEC = 0x80,
        [Description("Disable Events Command")]
        DISEC,
        [Description("Enter Activity State 0")]
        ENTAS0,
        [Description("Enter Activity State 1")]
        ENTAS1,
        [Description("Enter Activity State 2")]
        ENTAS2,
        [Description("Enter Activity State 3")]
        ENTAS3,
        [Description("Reset Dynamic Address Assignment")]
        RSTDAA,
        [Description("Set Dynamic Address from Static Address")]
        SETDASA,
        [Description("Set New Dynamic Address")]
        SETNEWDA,
        [Description("Set Max Write Length")]
        SETMWL,
        [Description("Set Max Write Length")]
        SETMRL,
        [Description("Get Max Write Length")]
        GETMWL,
        [Description("Get Max Read Length")]
        GETMRL,
        [Description("Get Provisional ID")]
        GETPID,
        [Description("Get Bus Characteristics Register")]
        GETBCR,
        [Description("Get Device Characteristics Register")]
        GETDCR,
        [Description("Get Device Status")]
        GETSTATUS,
        [Description("Get Accept Mastership")]
        GETACCMST,
        ENDXFER,
        SETBRGTGT,
        GETMXDS,
        GETCAPS,
        SETROUTE,
        D2DXFER,
        SETXTIME,
        GETXTIME,
        RSTACT,
        SETGRPA,
        RSTGRPA,
        //MLANE,
        DEVCAP = 0xE0
    }

    public enum eMajorFrame
    {
        [Description("Enable Events Command")]
        Broadcast_ENEC,
        [Description("Disable Events Command")]
        Broadcast_DISEC,
        [Description("Enter Activity State 0")]
        Broadcast_ENTAS0,
        [Description("Enter Activity State 1")]
        Broadcast_ENTAS1,
        [Description("Enter Activity State 2")]
        Broadcast_ENTAS2,
        [Description("Enter Activity State 3")]
        Broadcast_ENTAS3,
        [Description("Reset Dynamic Address Assignment")]
        Broadcast_RSTDAA,
        [Description("Enter Dynamic Address Assignment")]
        Broadcast_ENTDAA,
        [Description("Define List of Slaves")]
        Broadcast_DEFSLVS,
        [Description("Set Max Write Length")]
        Broadcast_SETMWL,
        [Description("Set Max Read Length")]
        Broadcast_SETMRL,
        [Description("Enter Test Mode")]
        Broadcast_ENTTM,

        Broadcast_SETBUSCON = 0x0c,
        Broadcast_MIPI_RESERVED_0x0D,
        Broadcast_MIPI_RESERVED_0x0E,
        Broadcast_MIPI_RESERVED_0x0F,
        Broadcast_MIPI_RESERVED_0x10,
        Broadcast_MIPI_RESERVED_0x11,
        Broadcast_ENDXFER,
        Broadcast_MIPI_RESERVED_0x13,
        Broadcast_MIPI_RESERVED_0x14,
        Broadcast_MIPI_RESERVED_0x15,
        Broadcast_MIPI_RESERVED_0x16,
        Broadcast_MIPI_RESERVED_0x17,
        Broadcast_MIPI_RESERVED_0x18,
        Broadcast_MIPI_RESERVED_0x19,
        Broadcast_MIPI_RESERVED_0x1A,
        Broadcast_MIPI_RESERVED_0x1B,
        Broadcast_MIPI_RESERVED_0x1C,
        Broadcast_MIPI_RESERVED_0x1D,
        Broadcast_MIPI_RESERVED_0x1E,
        Broadcast_MIPI_RESERVED_0x1F,

        [Description("Enter HDR Mode 0")]
        Broadcast_ENTHDR0 = 0x20,
        [Description("Enter HDR Mode 1")]
        Broadcast_ENTHDR1,
        [Description("Enter HDR Mode 2")]
        Broadcast_ENTHDR2,
        [Description("Enter HDR Mode 3")]
        Broadcast_ENTHDR3,
        [Description("Enter HDR Mode 4")]
        Broadcast_ENTHDR4,
        [Description("Enter HDR Mode 5")]
        Broadcast_ENTHDR5,
        [Description("Enter HDR Mode 6")]
        Broadcast_ENTHDR6,
        [Description("Enter HDR Mode 7")]
        Broadcast_ENTHDR7,
        [Description("Exchange Timing Information")]
        Broadcast_SETXTIME,

        Broadcast_SETAASA,
        Broadcast_RSTACT,
        Broadcast_DEFGRPA,
        Broadcast_RSTGRPA,
        Broadcast_MLANE,
        Broadcast_SENSOR_WG_0X2E,
        Broadcast_SENSOR_WG_0X2F,
        Broadcast_SENSOR_WG_0X30,
        Broadcast_SENSOR_WG_0X31,
        Broadcast_SENSOR_WG_0X32,
        Broadcast_SENSOR_WG_0X33,
        Broadcast_SENSOR_WG_0X34,
        Broadcast_SENSOR_WG_0X35,
        Broadcast_SENSOR_WG_0X36,
        Broadcast_SENSOR_WG_0X37,
        Broadcast_SENSOR_WG_0X38,
        Broadcast_SENSOR_WG_0X39,
        Broadcast_SENSOR_WG_0X3A,
        Broadcast_SENSOR_WG_0X3B,
        Broadcast_SENSOR_WG_0X3C,
        Broadcast_SENSOR_WG_0X3D,
        Broadcast_SENSOR_WG_0X3E,
        Broadcast_SENSOR_WG_0X3F,
        Broadcast_SENSOR_WG_0X40,
        Broadcast_SENSOR_WG_0X41,
        Broadcast_SENSOR_WG_0X42,
        Broadcast_SENSOR_WG_0X43,
        Broadcast_SENSOR_WG_0X44,
        Broadcast_SENSOR_WG_0X45,
        Broadcast_SENSOR_WG_0X46,
        Broadcast_SENSOR_WG_0X47,
        Broadcast_SENSOR_WG_0X48,

        Broadcast_NON_SENSOR_WG_0X49,
        Broadcast_NON_SENSOR_WG_0X4A,
        Broadcast_NON_SENSOR_WG_0X4B,
        Broadcast_NON_SENSOR_WG_0X4C,
        Broadcast_NON_SENSOR_WG_0X4D,
        Broadcast_NON_SENSOR_WG_0X4E,
        Broadcast_NON_SENSOR_WG_0X4F,
        Broadcast_NON_SENSOR_WG_0X50,
        Broadcast_NON_SENSOR_WG_0X51,
        Broadcast_NON_SENSOR_WG_0X52,
        Broadcast_NON_SENSOR_WG_0X53,
        Broadcast_NON_SENSOR_WG_0X54,
        Broadcast_NON_SENSOR_WG_0X55,
        Broadcast_NON_SENSOR_WG_0X56,
        Broadcast_NON_SENSOR_WG_0X57,

        Broadcast_DEBUG_WG_0X58,
        Broadcast_DEBUG_WG_0X59,
        Broadcast_DEBUG_WG_0X5A,
        Broadcast_DEBUG_WG_0X5B,

        Broadcast_RIO_WG_0X5C,
        Broadcast_RIO_WG_0X5D,
        Broadcast_RIO_WG_0X5E,
        Broadcast_RIO_WG_0X5F,
        Broadcast_RIO_WG_0X60,

        Broadcast_SETHID,
        Broadcast_DEVCTRL,
        Broadcast_VENDOR_EXT_63,
        Broadcast_VENDOR_EXT_64,
        Broadcast_VENDOR_EXT_65,
        Broadcast_VENDOR_EXT_66,
        Broadcast_VENDOR_EXT_67,
        Broadcast_VENDOR_EXT_68,
        Broadcast_VENDOR_EXT_69,
        Broadcast_VENDOR_EXT_6A,
        Broadcast_VENDOR_EXT_6B,
        Broadcast_VENDOR_EXT_6C,
        Broadcast_VENDOR_EXT_6D,
        Broadcast_VENDOR_EXT_6E,
        Broadcast_VENDOR_EXT_6F,
        Broadcast_VENDOR_EXT_70,
        Broadcast_VENDOR_EXT_71,
        Broadcast_VENDOR_EXT_72,
        Broadcast_VENDOR_EXT_73,
        Broadcast_VENDOR_EXT_74,
        Broadcast_VENDOR_EXT_75,
        Broadcast_VENDOR_EXT_76,
        Broadcast_VENDOR_EXT_77,
        Broadcast_VENDOR_EXT_78,
        Broadcast_VENDOR_EXT_79,
        Broadcast_VENDOR_EXT_7A,
        Broadcast_VENDOR_EXT_7B,
        Broadcast_VENDOR_EXT_7C,
        Broadcast_VENDOR_EXT_7D,
        Broadcast_VENDOR_EXT_7E,
        Broadcast_VENDOR_EXT_7F,

        [Description("Enable Events Command")]
        Directed_ENEC = 0x80,
        [Description("Disable Events Command")]
        Directed_DISEC,
        [Description("Enter Activity State 0")]
        Directed_ENTAS0,
        [Description("Enter Activity State 1")]
        Directed_ENTAS1,
        [Description("Enter Activity State 2")]
        Directed_ENTAS2,
        [Description("Enter Activity State 3")]
        Directed_ENTAS3,
        [Description("Reset Dynamic Address Assignment")]
        Directed_RSTDAA,
        [Description("Set Dynamic Address from Static Address")]
        Directed_SETDASA,
        [Description("Set New Dynamic Address")]
        Directed_SETNEWDA,
        [Description("Set Max Write Length")]
        Directed_SETMWL,
        [Description("Set Max Read Length")]
        Directed_SETMRL,
        [Description("Get Max Write Length")]
        Directed_GETMWL,
        [Description("Get Max Read Length")]
        Directed_GETMRL,
        [Description("Get Provisional ID")]
        Directed_GETPID,
        [Description("Get Bus Characteristics Register")]
        Directed_GETBCR,
        [Description("Get Device Characteristics Register")]
        Directed_GETDCR,
        [Description("Get Device Status")]
        Directed_GETSTATUS,
        [Description("Get Accept Mastership")]
        Directed_GETACCMST,
        Directed_ENDXFER,
        Directed_SETBRGTGT,
        Directed_GETMXDS,
        Directed_GETCAPS,
        Directed_SETROUTE,
        Directed_D2DXFER,
        Directed_SETXTIME,
        Directed_GETXTIME,
        Directed_RSTACT,
        Directed_SETGRPA,
        Directed_RSTGRPA,
        Directed_MLANE,
        Directed_SENSOR_WG_0x9E,
        Directed_SENSOR_WG_0x9F,
        Directed_SENSOR_WG_0xA0,
        Directed_SENSOR_WG_0xA1,
        Directed_SENSOR_WG_0xA2,
        Directed_SENSOR_WG_0xA3,
        Directed_SENSOR_WG_0xA4,
        Directed_SENSOR_WG_0xA5,
        Directed_SENSOR_WG_0xA6,
        Directed_SENSOR_WG_0xA7,
        Directed_SENSOR_WG_0xA8,
        Directed_SENSOR_WG_0xA9,
        Directed_SENSOR_WG_0xAA,
        Directed_SENSOR_WG_0xAB,
        Directed_SENSOR_WG_0xAC,
        Directed_SENSOR_WG_0xAD,
        Directed_SENSOR_WG_0xAE,
        Directed_SENSOR_WG_0xAF,
        Directed_SENSOR_WG_0xB0,
        Directed_SENSOR_WG_0xB1,
        Directed_SENSOR_WG_0xB2,
        Directed_SENSOR_WG_0xB3,
        Directed_SENSOR_WG_0xB4,
        Directed_SENSOR_WG_0xB5,
        Directed_SENSOR_WG_0xB6,
        Directed_SENSOR_WG_0xB7,
        Directed_SENSOR_WG_0xB8,
        Directed_SENSOR_WG_0xB9,
        Directed_SENSOR_WG_0xBA,
        Directed_SENSOR_WG_0xBB,
        Directed_SENSOR_WG_0xBC,
        Directed_SENSOR_WG_0xBD,
        Directed_SENSOR_WG_0xBE,
        Directed_SENSOR_WG_0xBF,

        Directed_NON_SENSOR_WG_0xC0,
        Directed_NON_SENSOR_WG_0xC1,
        Directed_NON_SENSOR_WG_0xC2,
        Directed_NON_SENSOR_WG_0xC3,
        Directed_NON_SENSOR_WG_0xC4,
        Directed_NON_SENSOR_WG_0xC5,
        Directed_NON_SENSOR_WG_0xC6,
        Directed_NON_SENSOR_WG_0xC7,
        Directed_NON_SENSOR_WG_0xC8,
        Directed_NON_SENSOR_WG_0xC9,
        Directed_NON_SENSOR_WG_0xCA,
        Directed_NON_SENSOR_WG_0xCB,
        Directed_NON_SENSOR_WG_0xCC,
        Directed_NON_SENSOR_WG_0xCD,
        Directed_NON_SENSOR_WG_0xCE,
        Directed_NON_SENSOR_WG_0xCF,
        Directed_NON_SENSOR_WG_0xD0,
        Directed_NON_SENSOR_WG_0xD1,
        Directed_NON_SENSOR_WG_0xD2,
        Directed_NON_SENSOR_WG_0xD3,
        Directed_NON_SENSOR_WG_0xD4,
        Directed_NON_SENSOR_WG_0xD5,
        Directed_NON_SENSOR_WG_0xD6,

        Directed_DEBUG_WG_0xD7,
        Directed_DEBUG_WG_0xD8,
        Directed_DEBUG_WG_0xD9,
        Directed_DEBUG_WG_0xDA,

        Directed_RIO_WG_0xDB,
        Directed_RIO_WG_0xDC,
        Directed_RIO_WG_0xDD,
        Directed_RIO_WG_0xDE,
        Directed_RIO_WG_0xDF,

        Directed_DEVCAP,
        Directed_VENDOR_E1,
        Directed_VENDOR_E2,
        Directed_VENDOR_E3,
        Directed_VENDOR_E4,
        Directed_VENDOR_E5,
        Directed_VENDOR_E6,
        Directed_VENDOR_E7,
        Directed_VENDOR_E8,
        Directed_VENDOR_E9,
        Directed_VENDOR_EA,
        Directed_VENDOR_EB,
        Directed_VENDOR_EC,
        Directed_VENDOR_ED,
        Directed_VENDOR_EE,
        Directed_VENDOR_EF,
        Directed_VENDOR_F0,
        Directed_VENDOR_F1,
        Directed_VENDOR_F2,
        Directed_VENDOR_F3,
        Directed_VENDOR_F4,
        Directed_VENDOR_F5,
        Directed_VENDOR_F6,
        Directed_VENDOR_F7,
        Directed_VENDOR_F8,
        Directed_VENDOR_F9,
        Directed_VENDOR_FA,
        Directed_VENDOR_FB,
        Directed_VENDOR_FC,
        Directed_VENDOR_FD,
        Directed_VENDOR_FE,
        Directed_SENSOR_WG_FF,

        Hot_Join = 257,
        Private_Message = 258,
        IBI_OR_PVT_Message = 259,
        I2C_Message = 260,
        HDR_Exit_Pattern = 261,
        Slave_Reset_Pattern = 262,
        Broadcast_Address = 263,
        NACK_Message = 264,
        Address_Message = 265,
        SPI_Message,
        UART_Message,
        NA = -1

    }




    public enum eCommand
    {

        [Description("Disable Events Command")]
        DISEC,
        [Description("Enable Events Command")]
        ENEC,
        [Description("Enter Activity State 0")]
        ENTAS0,
        [Description("Enter Activity State 1")]
        ENTAS1,
        [Description("Enter Activity State 2")]
        ENTAS2,
        [Description("Enter Activity State 3")]
        ENTAS3,
        [Description("Enter Dynamic Address Assignment")]
        ENTDAA,
        [Description("Enter HDR Mode 0")]
        ENTHDR0,
        [Description("Enter HDR Mode 1")]
        ENTHDR1,
        [Description("Enter HDR Mode 2")]
        ENTHDR2,
        [Description("Enter HDR Mode 3")]
        ENTHDR3,
        [Description("Enter HDR Mode 4")]
        ENTHDR4,
        [Description("Enter HDR Mode 5")]
        ENTHDR5,
        [Description("Enter HDR Mode 6")]
        ENTHDR6,
        [Description("Enter HDR Mode 7")]
        ENTHDR7,
        [Description("Enter Test Mode")]
        ENTTM,
        ENDXFER,
        [Description("Get Max Write Length")]
        GETMWL,
        [Description("Get Max Read Length")]
        GETMRL,
        [Description("Define List of Slaves")]
        DEFSLVS,
        SETXTIME,
        SETAASA,
        RSTACT,
        SETGRPA,
        DEFGRPA,
        RSTGRPA,
        MLANE,
        [Description("Get Provisional ID")]
        GETPID,
        [Description("Get Bus Characteristics Register")]
        GETBCR,
        [Description("Get Device Characteristics Register")]
        GETDCR,
        [Description("Get Device Status")]
        GETSTATUS,
        [Description("Get Max SCL")]
        GETMXDS,
        [Description("Get Read Delay")]
        GETRDDELAY,
        [Description("Get HDR Capability")]
        GETCAPS,
        SETROUTE,
        D2DXFER,
        [Description("Get Maximum P2P Length")]
        GETMP2PL,
        [Description("MIPI Reserved")]
        MIPI_RSVD,
        [Description("Request Master Handoff")]
        GETACCMST,
        [Description("Reset Dynamic Address Assignment")]
        RSTDAA,
        [Description("MIPI Reserved for other WG’s")]
        RSVD_BC_CCC,
        [Description("Set Dynamic Address from Static Address")]
        SETDASA,
        [Description("Set New Dynamic Address")]
        SETNEWDA,
        [Description("Set Max Write Length")]
        SETMRL,
        [Description("Set Max Write Length")]
        SETMWL,
        [Description("Set Maximum P2P Length")]
        SETMP2PL,
        [Description("Set P2P Target")]
        SETP2PTGT,
        [Description("Set Bridge Targets")]
        SETBRGTGT,
        [Description("MIPI Sensor WG Reserved")]
        Sensor_RSVD_BC_CCC,
        [Description("MIPI Sensor WG Reserved for Directed CCC")]
        Sensor_RSVD_DR_CCC,
        [Description("MIPI Reserved for other WG’s for Directed CCC")]
        RSVD_DR_CCC,
        [Description("Vendor Extension for Broadcasted CCC")]
        Vendor_Ext_BC_CCC,
        [Description("Vendor Extension for Directed CCC")]
        Vendor_Ext_DR_CCC,
        [Description("Exchange Timing Information")]
        GETXTIME,
        [Description("Set Maximum Data Speed")]
        SETMXDS,
        DEVCAP,
        SETHID,
        DEVCTRL,
        SETBUSCON
    }

    public enum eTriggerTypeList
    {
        [Description("Auto")]
        Auto,
        [Description("Pattern")]
        Pattern,
        [Description("Protocol Aware")]
        Protocol,
        [Description("If-then-else-if")]
        If_then_else_if,
        [Description("Timing")]
        Timing
    }

    public enum ePatternFormat
    {
        [Description("Binary")]
        Binary,
        [Description("Hex")]
        Hex,
        [Description("Octal")]
        Octal,
        [Description("Decimal")]
        Decimal
    }

    public enum eProtocolTypeList
    {
        [Description("I2C")]
        I2C,
        [Description("SPI")]
        SPI,
        [Description("UART")]
        UART,
        [Description("I3C")]
        I3C,
        [Description("SPMI")]
        SPMI,
        [Description("RFFE")]
        RFFE,
        [Description("CAN")]
        CAN,
        [Description("QSPI")]
        QSPI,


    }

    public enum eI2CTriggerAtList
    {
        [Description("Start")]
        Start,
        [Description("Address")]
        Address,
        [Description("Data")]
        Data,
        [Description("Address + Data")]
        Address_Data,
        [Description("Ack")]
        Ack,
        [Description("Nack")]
        Nack,
        [Description("Repeated Start")]
        Repeated_Start,
        [Description("Stop")]
        Stop,
    }

    public enum eI2CSearchAtList
    {
        [Description("Start")]
        Start,
        [Description("Address")]
        Address,
        [Description("Data")]
        Data,
        [Description("Address + Data")]
        Address_Data,
        [Description("Ack")]
        Ack,
        [Description("Nack")]
        Nack,
        [Description("Repeated Start")]
        Repeated_Start,
        [Description("Stop")]
        Stop,
    }

    public enum eComparisonList
    {
        [Description("Equal to")]
        Equal_to,
        [Description("Not equal to")]
        Not_equal_to
    }

    public enum eUARTTypeList
    {
        [Description("TX")]
        TX,
        [Description("RX")]
        RX
    }

    public enum eTimingTriggerTypeList
    {
        [Description("Pulse Width")]
        Pulse_Width,
        [Description("Delay")]
        Delay
    }

    public enum ePulseComparisonList
    {
        [Description("Greater than")]
        Greater_than,
        [Description("Less than")]
        Less_than
    }
    public enum eProtocol
    {
        [Description("I2C")]
        I2C,
        [Description("SPI")]
        SPI,
        [Description("UART")]
        UART,
        [Description("I3C")]
        I3C,
        [Description("SPMI")]
        SPMI,
        [Description("RFFE")]
        RFFE,
        [Description("CAN")]
       CAN,
        [Description("QSPI")]
        QSPI
    }

    public enum eBuses
    {
        C1G1,C1G2,C1G3,C2G1,C2G2,C2G3
    }

    public enum eRFFECMDTYPE
    {
        EXT_REG_WRITE,
        EXT_REG_READ,
        EXT_REG_WRITE_LONG,
        EXT_REG_READ_LONG,
        REG_WRITE,
        REG_READ,
        REG_ZERO_WRITE,
        MASKED_WRITE,
        MASTER_CXT_TRANSFER_READ,
        MASTER_CXT_TRANSFER_WRITE,
        MASTER_READ,
        MASTER_WRITE,
        MASTER_OWNERSHIP_HANDOVER,
        INT_SUMMARY_IDENT,
        EXT_REG_RESERVED,
        TRIGGER,
        None
    }

    public enum eSPMICMDTYPE : byte
    {
        EXT_REG_WRITE,
        RESET,
        SLEEP,
        SHUTDOWN,
        WAKEUP,
        AUTHENTICATE,
        MASTER_READ,
        MASTER_WRITE,
        TRFR_BUS_OWNERSHIP,
        DDB_MA_R,
        DDB_SL_R,
        EXT_REG_RESERVED,
        EXT_REG_READ,
        EXT_REG_WRITE_LONG,
        EXT_REG_READ_LONG,
        REG_WRITE,
        REG_READ,
        REG_ZERO_WRITE,
        NO_RESPONSE,
        DEFAULT
    }

    public enum eInterruptSlot { INT0, INT1, INT2, INT3, INT4, INT5, INT6, INT7, INT8, INT9, INT10, INT11, INT12, INT13, INT14, INT15, NONE }

    public enum eArbitration
    {
        A_bit,
        Sr_bit
    }

    public enum eSlaveType
    {
        Parity_Error,
        Interrrupt,
        Normal_Operation,
        Request
    }

    public enum eSlaveID
    {
        Broadcast_Slave_ID,
        Spare1,
        Spare2,
        LNA_Module1,
        Spare4,
        Power_Control,
        Spare6,
        Antenna_Tuning,
        Spare8,
        Spare9,
        Antenna_Switch_Module2,
        Antenna_Switch_Module1,
        SpareC,
        SpareD,
        PA_Module1,
        PA_Module2
    }

    public enum ERROR_CODE
    {
        Pass = 0,
        DataParity = 1,
        CommandParity = 2,
        BusPark = 4

    }
    public enum RESULT
    {
        Fail = 0,
        Pass = 1,
        NA = 2
    }
    public enum AnnotationType
    {
        ADDRESS = 0,
        DATA = 1,
        PARITY = 2,
        COMMAND = 3,
        BUS_PARK = 4,
        ADDRESS_WORD = 6,
        DATA_WORD = 7,
        SSC = 8,
        NONE = 9
    }


    public enum eFrameTypeCAN : byte
    {
        [Description("Data Frame")]
        DataFrame,
        [Description("Error Frame")]
        ErrorFrame,
        [Description("Remote Frame")]
        RemoteFrame,
        [Description("Overload Frame")]
        OverloadFrame
    }

}
