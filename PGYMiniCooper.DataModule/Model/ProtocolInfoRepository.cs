using PGYMiniCooper.DataModule.Structure;
using ProtocolCoreModule.Structure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public static class ProtocolInfoRepository
    {
        private static ConfigModel configModel;
     public static  bool QSPIAddressByteCount;
        public static void Initialize(ConfigModel config)
        {
            configModel = config;
            CreateCommandInfo();
           // CreateQSPICommandInfo();
                  

            if (configModel.ProtocolConfigList.Any(c => c is ConfigModel_QSPI))
            {
                var configModelQSPI = configModel.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();


                if (configModelQSPI.QSPIByteAddresssType == eQSPIAddress.ThreeByte)
                {

                    QSPIAddressByteCount = true;

                }
                else if (configModelQSPI.QSPIByteAddresssType == eQSPIAddress.FourByte)
                {
                    QSPIAddressByteCount = false;
                }

               
                CreateQSPICommandInfo(QSPIAddressByteCount);
            }
        }
     
        static ProtocolInfoRepository()

        {
            CreateCommandInfo();
            CreateQSPICommandInfo(QSPIAddressByteCount);

        }
        public static byte GetRFFECmdId(int cmd)
        {
            switch (cmd)
            {
                case (int)eRFFECMDTYPE.EXT_REG_WRITE:
                    return 0x0;
                case (int)eRFFECMDTYPE.EXT_REG_RESERVED:
                    return 0x10;
                case (int)eRFFECMDTYPE.EXT_REG_READ:
                    return 0x20;
                case (int)eRFFECMDTYPE.EXT_REG_WRITE_LONG:
                    return 0x30;
                case (int)eRFFECMDTYPE.EXT_REG_READ_LONG:
                    return 0x38;
                case (int)eRFFECMDTYPE.REG_WRITE:
                    return 0x40;
                case (int)eRFFECMDTYPE.REG_READ:
                    return 0x60;
                case (int)eRFFECMDTYPE.REG_ZERO_WRITE:
                    return 0x80;
                case (int)eRFFECMDTYPE.MASKED_WRITE:
                    return 0x19;
                case (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ:
                    return 0x1A;
                case (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE:
                    return 0x1B;
                case (int)eRFFECMDTYPE.MASTER_READ:
                    return 0x1C;
                case (int)eRFFECMDTYPE.MASTER_WRITE:
                    return 0x1D;
                case (int)eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER:
                    return 0x1E;
                case (int)eRFFECMDTYPE.INT_SUMMARY_IDENT:
                    return 0x1F;
                default:
                    {
                        return 0x0;
                    }
            }
        }

        public static byte GetSPMICmdId(int cmd)
        {
            switch (cmd)
            {
                case (int)eSPMICMDTYPE.EXT_REG_WRITE:
                    return 0x00;
                case (int)eSPMICMDTYPE.RESET:
                    return 0x10;
                case (int)eSPMICMDTYPE.SLEEP:
                    return 0x11;
                case (int)eSPMICMDTYPE.SHUTDOWN:
                    return 0x12;
                case (int)eSPMICMDTYPE.WAKEUP:
                    return 0x13;
                case (int)eSPMICMDTYPE.AUTHENTICATE:
                    return 0x14;
                case (int)eSPMICMDTYPE.MASTER_READ:
                    return 0x15;
                case (int)eSPMICMDTYPE.MASTER_WRITE:
                    return 0x16;
                case (int)eSPMICMDTYPE.TRFR_BUS_OWNERSHIP:
                    return 0x1A;
                case (int)eSPMICMDTYPE.DDB_MA_R:
                    return 0x1B;
                case (int)eSPMICMDTYPE.DDB_SL_R:
                    return 0x1C;
                case (int)eSPMICMDTYPE.EXT_REG_READ:
                    return 0x20;
                case (int)eSPMICMDTYPE.EXT_REG_WRITE_LONG:
                    return 0x30;
                case (int)eSPMICMDTYPE.EXT_REG_READ_LONG:
                    return 0x38;
                case (int)eSPMICMDTYPE.REG_WRITE:
                    return 0x40;
                case (int)eSPMICMDTYPE.REG_READ:
                    return 0x60;
                case (int)eSPMICMDTYPE.REG_ZERO_WRITE:
                    return 0x80;
                default:
                    {
                        return 0x0;
                    }
            }
        }

        public static eSPMICMDTYPE GetSPMICmdType(byte Cmd)
        {

            if (Cmd >= 0x0 && Cmd <= 0x0F)
                return eSPMICMDTYPE.EXT_REG_WRITE;

            else if (Cmd == 0x10)
                return eSPMICMDTYPE.RESET;

            else if (Cmd == 0x11)
                return eSPMICMDTYPE.SLEEP;

            else if (Cmd == 0x12)
                return eSPMICMDTYPE.SHUTDOWN;

            else if (Cmd == 0x13)
                return eSPMICMDTYPE.WAKEUP;

            else if (Cmd == 0x14)
                return eSPMICMDTYPE.AUTHENTICATE;

            else if (Cmd == 0x15)
                return eSPMICMDTYPE.MASTER_READ;

            else if (Cmd == 0x16)
                return eSPMICMDTYPE.MASTER_WRITE;

            else if (Cmd == 0x1A)
                return eSPMICMDTYPE.TRFR_BUS_OWNERSHIP;

            else if (Cmd == 0x1B)
                return eSPMICMDTYPE.DDB_MA_R;

            else if (Cmd == 0x1C)
                return eSPMICMDTYPE.DDB_SL_R;

            else if (Cmd >= 0x17 && Cmd <= 0x19 || Cmd >= 0x1D && Cmd <= 0x1F)
                return eSPMICMDTYPE.EXT_REG_RESERVED;

            else if (Cmd >= 0x20 && Cmd <= 0x2F)
                return eSPMICMDTYPE.EXT_REG_READ;

            else if (Cmd >= 0x30 && Cmd <= 0x37)
                return eSPMICMDTYPE.EXT_REG_WRITE_LONG;

            else if (Cmd >= 0x38 && Cmd <= 0x3F)
                return eSPMICMDTYPE.EXT_REG_READ_LONG;

            else if (Cmd >= 0x40 && Cmd <= 0x5F)
                return eSPMICMDTYPE.REG_WRITE;
            else if (Cmd >= 0x60 && Cmd <= 0x7F)
                return eSPMICMDTYPE.REG_READ;
            else if (Cmd >= 0x80 && Cmd <= 0xFF)
                return eSPMICMDTYPE.REG_ZERO_WRITE;
            else
                return eSPMICMDTYPE.EXT_REG_RESERVED;

        }

        public static eRFFECMDTYPE GetRFFECmdType(byte Cmd)
        {

            if (Cmd >= 0x0 && Cmd <= 0x0F)
                return eRFFECMDTYPE.EXT_REG_WRITE;

            if (Cmd >= 0x10 && Cmd <= 0x18)
                return eRFFECMDTYPE.EXT_REG_RESERVED;

            if (Cmd == 0x19)
                return eRFFECMDTYPE.MASKED_WRITE;

            if (Cmd == 0x1A)
                return eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ;

            if (Cmd == 0x1B)
                return eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE;

            if (Cmd == 0x1C)
                return eRFFECMDTYPE.MASTER_READ;

            if (Cmd == 0x1D)
                return eRFFECMDTYPE.MASTER_WRITE;

            if (Cmd == 0x1E)
                return eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER;

            if (Cmd == 0x1F)
                return eRFFECMDTYPE.INT_SUMMARY_IDENT;

            if (Cmd >= 0x20 && Cmd <= 0x2F)
                return eRFFECMDTYPE.EXT_REG_READ;

            if (Cmd >= 0x30 && Cmd <= 0x37)
                return eRFFECMDTYPE.EXT_REG_WRITE_LONG;

            if (Cmd >= 0x38 && Cmd <= 0x3F)
                return eRFFECMDTYPE.EXT_REG_READ_LONG;

            if (Cmd >= 0x40 && Cmd <= 0x5F)
                return eRFFECMDTYPE.REG_WRITE;

            if (Cmd >= 0x60 && Cmd <= 0x7F)
                return eRFFECMDTYPE.REG_READ;

            if (Cmd >= 0x80 && Cmd <= 0xFF)
                return eRFFECMDTYPE.REG_ZERO_WRITE;

            return eRFFECMDTYPE.EXT_REG_RESERVED;

        }

        public static List<int> I2CAddressList = new List<int>() { 0x3, 0x4, 0x5, 0x6, 0x7, 0x78, 0x79, 0x7B, 0x7C };
        public static CommandInfo GetCommandInfo(int command)
        {
            if (CommandDirectory.ContainsKey(command))
                return CommandDirectory[command];
            else
                return new CommandInfo();
        }

        public static eCommand GetCommand(int intCmd)
        {
            switch (intCmd)
            {
                case 0x00:
                    return eCommand.ENEC;
                case 0x01:
                    return eCommand.DISEC;
                case 0x02:
                    return eCommand.ENTAS0;
                case 0x03:
                    return eCommand.ENTAS1;
                case 0x04:
                    return eCommand.ENTAS2;
                case 0x05:
                    return eCommand.ENTAS3;
                case 0x06:
                    return eCommand.RSTDAA;
                case 0x07:
                    return eCommand.ENTDAA;
                case 0x08:
                    return eCommand.DEFSLVS;
                case 0x09:
                    return eCommand.SETMWL;
                case 0x0A:
                    return eCommand.SETMRL;
                case 0x0B:
                    return eCommand.ENTTM;
                case 0x0c:
                    return eCommand.SETBUSCON;
                case 0x12:
                    return eCommand.ENDXFER;
                case 0x20:
                    return eCommand.ENTHDR0;
                case 0x21:
                    return eCommand.ENTHDR1;
                case 0x22:
                    return eCommand.ENTHDR2;
                case 0x23:
                    return eCommand.ENTHDR3;
                case 0x24:
                    return eCommand.ENTHDR4;
                case 0x25:
                    return eCommand.ENTHDR5;
                case 0x26:
                    return eCommand.ENTHDR6;
                case 0x27:
                    return eCommand.ENTHDR7;
                case 0x28:
                    return eCommand.SETXTIME;
                case 0x29:
                    return eCommand.SETAASA;
                case 0x2A:
                    return eCommand.RSTACT;
                case 0x2B:
                    return eCommand.DEFGRPA;
                case 0x2C:
                    return eCommand.RSTGRPA;
                case 0x2D:
                    return eCommand.MLANE;
                case 0x80:
                    return eCommand.ENEC;
                case 0x81:
                    return eCommand.DISEC;
                case 0x82:
                    return eCommand.ENTAS0;
                case 0x83:
                    return eCommand.ENTAS1;
                case 0x84:
                    return eCommand.ENTAS2;
                case 0x85:
                    return eCommand.ENTAS3;
                case 0x86:
                    return eCommand.RSTDAA;
                case 0x87:
                    return eCommand.SETDASA;
                case 0x88:
                    return eCommand.SETNEWDA;
                case 0x89:
                    return eCommand.SETMWL;
                case 0x8A:
                    return eCommand.SETMRL;
                case 0x8B:
                    return eCommand.GETMWL;
                case 0x8C:
                    return eCommand.GETMRL;
                case 0x8D:
                    return eCommand.GETPID;
                case 0x8E:
                    return eCommand.GETBCR;
                case 0x8F:
                    return eCommand.GETDCR;
                case 0x90:
                    return eCommand.GETSTATUS;
                case 0x91:
                    return eCommand.GETACCMST;
                case 0x92:
                    return eCommand.ENDXFER;
                case 0x93:
                    return eCommand.SETBRGTGT;
                case 0x94:
                    return eCommand.GETMXDS;
                case 0x95:
                    return eCommand.GETCAPS;
                case 0x96:
                    return eCommand.SETROUTE;
                case 0x97:
                    return eCommand.D2DXFER;
                case 0x98:
                    return eCommand.SETXTIME;
                case 0x99:
                    return eCommand.GETXTIME;
                case 0x9A:
                    return eCommand.RSTACT;
                case 0x9B:
                    return eCommand.SETGRPA;
                case 0x9C:
                    return eCommand.RSTGRPA;
                case 0x9D:
                    return eCommand.MLANE;
                case 0xE0:
                    return eCommand.DEVCAP;
                case 0x61:
                    return eCommand.SETHID;
                case 0x62:
                    return eCommand.DEVCTRL;
                default:
                    {
                        if (intCmd >= 0x0C && 0x1F <= intCmd)
                            return eCommand.MIPI_RSVD;
                        else if (intCmd >= 0x29 && intCmd <= 0x48)
                            return eCommand.Sensor_RSVD_BC_CCC;
                        else if (intCmd >= 0x49 && intCmd <= 0x60)
                            return eCommand.RSVD_BC_CCC;
                        else if (intCmd >= 0x61 && intCmd <= 0x7F)
                            return eCommand.Vendor_Ext_BC_CCC;
                        else if (intCmd >= 0x9A && intCmd <= 0xBF)
                            return eCommand.Sensor_RSVD_DR_CCC;
                        else if (intCmd >= 0xC0 && intCmd <= 0xDF)
                            return eCommand.RSVD_DR_CCC;
                        else if (intCmd >= 0x61 && intCmd <= 0x7F)
                            return eCommand.Vendor_Ext_DR_CCC;
                        else
                            return eCommand.MIPI_RSVD;
                    }

            }
        }

        public static int GetI2CAddress(string address)
        {
            int returnAddress = 0;
            eI2CAddressType type = eI2CAddressType._7bAddress;
            if (address != null)
            {
                var splitAddress = address.Split('h');

                if (splitAddress.Count() == 2)
                {
                    switch (splitAddress[0])
                    {
                        case "7":
                            type = eI2CAddressType._7bAddress;
                            break;
                        case "8":
                            type = eI2CAddressType._8bAddress;
                            break;
                        case "10":
                            type = eI2CAddressType._10bAddress;
                            break;
                    }
                    int.TryParse(splitAddress[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out returnAddress);
                }
                else if (splitAddress.Count() == 1)
                {
                    type = eI2CAddressType._7bAddress;
                    int.TryParse(address, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out returnAddress);
                }
            }
            else
            {
                type = eI2CAddressType._7bAddress;
            }
            return returnAddress;
        }

        public static IEnumerable<eFrameType> GetFrameTypeList()
        {
            return Enum.GetValues(typeof(eFrameType))
                        .Cast<eFrameType>();
        }


        public static eAddressType GetAddressInfo(int value)
        {
            if (value == 0)
                return eAddressType.I3C_Reserved;
            else if (value == 1)
                return eAddressType.Static_Address_SETDASA;
            else if (value == 2)
                return eAddressType.Hot_Joint_address;
            else if (value == 3)
                return eAddressType.I2C_Slave_Address;
            else if ((value >= 4 && value <= 7) || value == 0x78 || value == 0x79 || value == 0x7B || value == 0x7D)
                return eAddressType.I3C_Slave_OR_Legacy_I2C;
            else if ((value >= 8 && value <= 0x3D) || (value >= 0x5F && value <= 0x6D) || (value >= 0x6F && value <= 0x75) || (value == 0x77))
                return eAddressType.I3C_Slave_Address;
            else if (value == 0x3E || value == 0x5E || value == 0x6E || value == 0x76 || value == 0x7A || value == 0x7C || value == 0x7F)
                return eAddressType.BroadCast_Address_Error;
            else if (value == 0x7E)
                return eAddressType.BroadCast_Address;
            else
                return eAddressType.NA;
        }

        private static Dictionary<int, CommandInfo> CommandDirectory;

        static void CreateCommandInfo()
        {
            CommandDirectory = new Dictionary<int, CommandInfo>();
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENEC, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = true, DataCount = 1 });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_DISEC, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = true, DataCount = 1 });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTAS0, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTAS1, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTAS2, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTAS3, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_RSTDAA, new CommandInfo() { TransferType = eTransferType.WR, HasData = false, hasPECWithoutData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTDAA, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 9 });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETMWL, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETMRL, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_DEFSLVS, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 4, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTTM, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = true, DataCount = 1 });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETBUSCON, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR0, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR1, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR2, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR3, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR4, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR5, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR6, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENTHDR7, new CommandInfo() { TransferType = eTransferType.Both, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETXTIME, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 1, IsDataLimited = true });
            //i3c mipi 1.1 spec
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETAASA, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_RSTACT, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_SETHID, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_DEVCTRL, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_DEFGRPA, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 4, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_ENDXFER, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Broadcast_RSTGRPA, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            //
            CommandDirectory.Add((int)eMajorFrame.Directed_ENEC, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = true, DataCount = 1 });
            CommandDirectory.Add((int)eMajorFrame.Directed_DISEC, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = true, DataCount = 1 });
            CommandDirectory.Add((int)eMajorFrame.Directed_ENTAS0, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Directed_ENTAS1, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Directed_ENTAS2, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Directed_ENTAS3, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
            CommandDirectory.Add((int)eMajorFrame.Directed_RSTDAA, new CommandInfo() { TransferType = eTransferType.WR, HasData = false, hasPECWithoutData = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETMWL, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETMWL, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETMRL, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETMRL, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETDASA, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETNEWDA, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETPID, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 6, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETBCR, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETDCR, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETSTATUS, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 2, IsDataLimited = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETACCMST, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 2, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETBRGTGT, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, IsDataLimited = false });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETMXDS, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 2, IsDataLimited = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETCAPS, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 1, IsDataLimited = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETXTIME, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_GETXTIME, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, DataCount = 4, IsDataLimited = true });
            //jedec spec 1.1
            CommandDirectory.Add((int)eMajorFrame.Directed_DEVCAP, new CommandInfo() { TransferType = eTransferType.RD, HasData = true });
            //CommandDirectory.Add((int)eMajorFrame.Directed_RSTACT, new CommandInfo() { TransferType = eTransferType.WR, HasData = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_RSTACT, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_D2DXFER, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_ENDXFER, new CommandInfo() { TransferType = eTransferType.RD, HasData = true, hasMultiplePattern = true, Pattern = eCommandPattern.B });
            CommandDirectory.Add((int)eMajorFrame.Directed_SETGRPA, new CommandInfo() { TransferType = eTransferType.WR, HasData = true, DataCount = 1, IsDataLimited = true });
            CommandDirectory.Add((int)eMajorFrame.Directed_RSTGRPA, new CommandInfo() { TransferType = eTransferType.WR, HasData = false });
        }

        public static QSPICommandInfo GetQSPICommandInfo(int command)
        {
            if (QSPICommandDictionary.ContainsKey(command))
                return QSPICommandDictionary[command];
            else
                return new QSPICommandInfo();
        }

        public static Dictionary<int, QSPICommandInfo> QSPICommandDictionary;

        static void CreateQSPICommandInfo(bool QSPIAddressByteCount1)
        {
            if (QSPIAddressByteCount1 == true)
            {
                QSPICommandDictionary = new Dictionary<int, QSPICommandInfo>();
                QSPICommandDictionary.Add(0x66, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Enable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x66) });
                QSPICommandDictionary.Add(0x99, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Memory, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x99) });
                QSPICommandDictionary.Add(0x9E, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_ID_9E, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9E) });
                QSPICommandDictionary.Add(0x9F, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_ID_9F, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9F) });
                QSPICommandDictionary.Add(0xAF, new QSPICommandInfo { QSPICommands = eQSPICommands.Mulitple_IO_Read_ID, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xAF) });
                QSPICommandDictionary.Add(0x5A, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Serial_Flash_Discovery_Parameter, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x5A) });
                QSPICommandDictionary.Add(0x03, new QSPICommandInfo { QSPICommands = eQSPICommands.Read, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x03) });
                QSPICommandDictionary.Add(0x0B, new QSPICommandInfo { QSPICommands = eQSPICommands.Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x0B) });
                QSPICommandDictionary.Add(0x3B, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x3B) });
                QSPICommandDictionary.Add(0xBB, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Input_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0xBB) });
                QSPICommandDictionary.Add(0x6B, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x6B) });
                QSPICommandDictionary.Add(0xEB, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 10, Dlines = GetDataLines(0xEB) });
                QSPICommandDictionary.Add(0x0D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Fast_Read, AddressByteCount = 3, DummyCycles = 6, Dlines = GetDataLines(0x0D) });
                QSPICommandDictionary.Add(0x3D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Dual_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 6, Dlines = GetDataLines(0x3D) });
                QSPICommandDictionary.Add(0xBD, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Dual_Input_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 6, Dlines = GetDataLines(0xBD) });
                QSPICommandDictionary.Add(0x6D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Quad_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 6, Dlines = GetDataLines(0x6D) });
                QSPICommandDictionary.Add(0xED, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Quad_Input_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0xED) });
                QSPICommandDictionary.Add(0xE7, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Output_Word_Read, AddressByteCount = 3, DummyCycles = 4, Dlines = GetDataLines(0xE7) });
                QSPICommandDictionary.Add(0x13, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Read, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x13) });
                QSPICommandDictionary.Add(0x0C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x0C) });
                QSPICommandDictionary.Add(0x3C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Dual_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x3C) });
                QSPICommandDictionary.Add(0xBC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0xBC) });
                QSPICommandDictionary.Add(0x6C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x6C) });
                QSPICommandDictionary.Add(0xEC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 10, Dlines = GetDataLines(0xEC) });
                QSPICommandDictionary.Add(0x0E, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0x0E) });
                QSPICommandDictionary.Add(0xBE, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0xBE) });
                QSPICommandDictionary.Add(0xEE, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Quad_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0xEE) });
                QSPICommandDictionary.Add(0x06, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Enable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x06) });
                QSPICommandDictionary.Add(0x04, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Disable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x04) });
                QSPICommandDictionary.Add(0x05, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x05) });
                QSPICommandDictionary.Add(0x70, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Flag_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x70) });
                QSPICommandDictionary.Add(0xB5, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Non_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB5) });
                QSPICommandDictionary.Add(0x85, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x85) });
                QSPICommandDictionary.Add(0x65, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Enhanced_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x65) });
                QSPICommandDictionary.Add(0xC8, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Extended_Address_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC8) });
                QSPICommandDictionary.Add(0x01, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x01) });
                QSPICommandDictionary.Add(0xB1, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Non_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB1) });
                QSPICommandDictionary.Add(0x81, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x81) });
                QSPICommandDictionary.Add(0x61, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Enhanced_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x61) });
                QSPICommandDictionary.Add(0xC5, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Extended_Address_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC5) });
                QSPICommandDictionary.Add(0x50, new QSPICommandInfo { QSPICommands = eQSPICommands.Clear_Flag_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x50) });
                QSPICommandDictionary.Add(0x02, new QSPICommandInfo { QSPICommands = eQSPICommands.Page_Program, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x02) });
                QSPICommandDictionary.Add(0xA2, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Input_Fast_Program, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0xA2) });
                QSPICommandDictionary.Add(0xD2, new QSPICommandInfo { QSPICommands = eQSPICommands.Extended_Dual_Input_Fast_Program, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0xD2) });
                QSPICommandDictionary.Add(0x33, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Fast_Program, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x33) });
                QSPICommandDictionary.Add(0x38, new QSPICommandInfo { QSPICommands = eQSPICommands.Extended_Quad_Input_Fast_Program, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x38) });
                QSPICommandDictionary.Add(0x12, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Page_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x12) });
                QSPICommandDictionary.Add(0x34, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x34) });
                QSPICommandDictionary.Add(0x3E, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Extended_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x3E) });
                QSPICommandDictionary.Add(0x52, new QSPICommandInfo { QSPICommands = eQSPICommands._32KB_Subsector_Erase, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x52) });
                QSPICommandDictionary.Add(0x20, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_4KB_Subsector_Erase, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x20) });
                QSPICommandDictionary.Add(0xD8, new QSPICommandInfo { QSPICommands = eQSPICommands.Sector_Erase, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0xD8) });
                QSPICommandDictionary.Add(0xC7, new QSPICommandInfo { QSPICommands = eQSPICommands.Bulk_Erase, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC7) });
                QSPICommandDictionary.Add(0xDC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Sector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xDC) });
                QSPICommandDictionary.Add(0x21, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_4KB_Subsector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x21) });
                QSPICommandDictionary.Add(0x75, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Erase_Suspend, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x75) });
                QSPICommandDictionary.Add(0x7A, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Erase_Resume, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x7A) });
                QSPICommandDictionary.Add(0x4B, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_OTP_Array, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x4B) });
                QSPICommandDictionary.Add(0x42, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_OTP_Array, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0x42) });
                QSPICommandDictionary.Add(0xB7, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_4Byte_Address_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB7) });
                QSPICommandDictionary.Add(0xE9, new QSPICommandInfo { QSPICommands = eQSPICommands.Exit_4Byte_Address_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xE9) });
                QSPICommandDictionary.Add(0x35, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_Quad_Input_Output_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x35) });
                QSPICommandDictionary.Add(0xF5, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Quad_Input_Output_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xF5) });
                QSPICommandDictionary.Add(0xB9, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_Deep_Power_Down, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB9) });
                QSPICommandDictionary.Add(0xAB, new QSPICommandInfo { QSPICommands = eQSPICommands.Release_From_Deep_Power_Down, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xAB) });
                QSPICommandDictionary.Add(0x2D, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Sector_Protection, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x2D) });
                QSPICommandDictionary.Add(0x2C, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Sector_Protection, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x2C) });
                QSPICommandDictionary.Add(0xE8, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Volatile_Lock_Bits, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0xE8) });
                QSPICommandDictionary.Add(0xE5, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Volatile_Lock_Bits, AddressByteCount = 3, DummyCycles = 0, Dlines = GetDataLines(0xE5) });
                QSPICommandDictionary.Add(0xE2, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Non_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE2) });
                QSPICommandDictionary.Add(0xE3, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Non_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE3) });
                QSPICommandDictionary.Add(0xE4, new QSPICommandInfo { QSPICommands = eQSPICommands.Erase_Non_Volatile_Lock_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xE4) });
                QSPICommandDictionary.Add(0xA7, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Global_Freeze_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xA7) });
                QSPICommandDictionary.Add(0xA6, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Global_Freeze_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xA6) });
                QSPICommandDictionary.Add(0x27, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x27) });
                QSPICommandDictionary.Add(0x28, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x28) });
                QSPICommandDictionary.Add(0x29, new QSPICommandInfo { QSPICommands = eQSPICommands.Unlock_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x29) });
                QSPICommandDictionary.Add(0xE0, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Read_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE0) });
                QSPICommandDictionary.Add(0xE1, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Write_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE1) });
                QSPICommandDictionary.Add(0x9B, new QSPICommandInfo { QSPICommands = eQSPICommands.Interface_Activation, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9B) });
            }

            else if (QSPIAddressByteCount1 == false)
            {
                QSPICommandDictionary = new Dictionary<int, QSPICommandInfo>();
                QSPICommandDictionary.Add(0x66, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Enable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x66) });
                QSPICommandDictionary.Add(0x99, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Memory, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x99) });
                QSPICommandDictionary.Add(0x9E, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_ID_9E, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9E) });
                QSPICommandDictionary.Add(0x9F, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_ID_9F, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9F) });
                QSPICommandDictionary.Add(0xAF, new QSPICommandInfo { QSPICommands = eQSPICommands.Mulitple_IO_Read_ID, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xAF) });
                QSPICommandDictionary.Add(0x5A, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Serial_Flash_Discovery_Parameter, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0x5A) });
                QSPICommandDictionary.Add(0x03, new QSPICommandInfo { QSPICommands = eQSPICommands.Read, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x03) });
                QSPICommandDictionary.Add(0x0B, new QSPICommandInfo { QSPICommands = eQSPICommands.Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x0B) });
                QSPICommandDictionary.Add(0x3B, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x3B) });
                QSPICommandDictionary.Add(0xBB, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0xBB) });
                QSPICommandDictionary.Add(0x6B, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x6B) });
                QSPICommandDictionary.Add(0xEB, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 10, Dlines = GetDataLines(0xEB) });
                QSPICommandDictionary.Add(0x0D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0x0D) });
                QSPICommandDictionary.Add(0x3D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Dual_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0x3D) });
                QSPICommandDictionary.Add(0xBD, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0xBD) });
                QSPICommandDictionary.Add(0x6D, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Quad_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0x6D) });
                QSPICommandDictionary.Add(0xED, new QSPICommandInfo { QSPICommands = eQSPICommands.DTR_Quad_Input_Output_Fast_Read, AddressByteCount = 3, DummyCycles = 8, Dlines = GetDataLines(0xED) });
                QSPICommandDictionary.Add(0xE7, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Output_Word_Read, AddressByteCount = 4, DummyCycles = 4, Dlines = GetDataLines(0xE7) });
                QSPICommandDictionary.Add(0x13, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Read, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x13) });
                QSPICommandDictionary.Add(0x0C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x0C) });
                QSPICommandDictionary.Add(0x3C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Dual_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x3C) });
                QSPICommandDictionary.Add(0xBC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0xBC) });
                QSPICommandDictionary.Add(0x6C, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x6C) });
                QSPICommandDictionary.Add(0xEC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 10, Dlines = GetDataLines(0xEC) });
                QSPICommandDictionary.Add(0x0E, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0x0E) });
                QSPICommandDictionary.Add(0xBE, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Dual_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 6, Dlines = GetDataLines(0xBE) });
                QSPICommandDictionary.Add(0xEE, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_DTR_Quad_Input_Output_Fast_Read, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0xEE) });
                QSPICommandDictionary.Add(0x06, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Enable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x06) });
                QSPICommandDictionary.Add(0x04, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Disable, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x04) });
                QSPICommandDictionary.Add(0x05, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x05) });
                QSPICommandDictionary.Add(0x70, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Flag_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x70) });
                QSPICommandDictionary.Add(0xB5, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Non_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB5) });
                QSPICommandDictionary.Add(0x85, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x85) });
                QSPICommandDictionary.Add(0x65, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Enhanced_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x65) });
                QSPICommandDictionary.Add(0xC8, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Extended_Address_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC8) });
                QSPICommandDictionary.Add(0x01, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x01) });
                QSPICommandDictionary.Add(0xB1, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Non_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB1) });
                QSPICommandDictionary.Add(0x81, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x81) });
                QSPICommandDictionary.Add(0x61, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Enhanced_Volatile_Configuration_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x61) });
                QSPICommandDictionary.Add(0xC5, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Extended_Address_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC5) });
                QSPICommandDictionary.Add(0x50, new QSPICommandInfo { QSPICommands = eQSPICommands.Clear_Flag_Status_Register, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x50) });
                QSPICommandDictionary.Add(0x02, new QSPICommandInfo { QSPICommands = eQSPICommands.Page_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x02) });
                QSPICommandDictionary.Add(0xA2, new QSPICommandInfo { QSPICommands = eQSPICommands.Dual_Input_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xA2) });
                QSPICommandDictionary.Add(0xD2, new QSPICommandInfo { QSPICommands = eQSPICommands.Extended_Dual_Input_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xD2) });
                QSPICommandDictionary.Add(0x33, new QSPICommandInfo { QSPICommands = eQSPICommands.Quad_Input_Fast_Program, AddressByteCount = 34, DummyCycles = 0, Dlines = GetDataLines(0x33) });
                QSPICommandDictionary.Add(0x38, new QSPICommandInfo { QSPICommands = eQSPICommands.Extended_Quad_Input_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x38) });
                QSPICommandDictionary.Add(0x12, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Page_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x12) });
                QSPICommandDictionary.Add(0x34, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x34) });
                QSPICommandDictionary.Add(0x3E, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Quad_Input_Extended_Fast_Program, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x3E) });
                QSPICommandDictionary.Add(0x52, new QSPICommandInfo { QSPICommands = eQSPICommands._32KB_Subsector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x52) });
                QSPICommandDictionary.Add(0x20, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_4KB_Subsector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x20) });
                QSPICommandDictionary.Add(0xD8, new QSPICommandInfo { QSPICommands = eQSPICommands.Sector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xD8) });
                QSPICommandDictionary.Add(0xC7, new QSPICommandInfo { QSPICommands = eQSPICommands.Bulk_Erase, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xC7) });
                QSPICommandDictionary.Add(0xDC, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Sector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xDC) });
                QSPICommandDictionary.Add(0x21, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_4KB_Subsector_Erase, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x21) });
                QSPICommandDictionary.Add(0x75, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Erase_Suspend, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x75) });
                QSPICommandDictionary.Add(0x7A, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Erase_Resume, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x7A) });
                QSPICommandDictionary.Add(0x4B, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_OTP_Array, AddressByteCount = 4, DummyCycles = 8, Dlines = GetDataLines(0x4B) });
                QSPICommandDictionary.Add(0x42, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_OTP_Array, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0x42) });
                QSPICommandDictionary.Add(0xB7, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_4Byte_Address_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB7) });
                QSPICommandDictionary.Add(0xE9, new QSPICommandInfo { QSPICommands = eQSPICommands.Exit_4Byte_Address_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xE9) });
                QSPICommandDictionary.Add(0x35, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_Quad_Input_Output_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x35) });
                QSPICommandDictionary.Add(0xF5, new QSPICommandInfo { QSPICommands = eQSPICommands.Reset_Quad_Input_Output_Mode, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xF5) });
                QSPICommandDictionary.Add(0xB9, new QSPICommandInfo { QSPICommands = eQSPICommands.Enter_Deep_Power_Down, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xB9) });
                QSPICommandDictionary.Add(0xAB, new QSPICommandInfo { QSPICommands = eQSPICommands.Release_From_Deep_Power_Down, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xAB) });
                QSPICommandDictionary.Add(0x2D, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Sector_Protection, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x2D) });
                QSPICommandDictionary.Add(0x2C, new QSPICommandInfo { QSPICommands = eQSPICommands.Program_Sector_Protection, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x2C) });
                QSPICommandDictionary.Add(0xE8, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE8) });
                QSPICommandDictionary.Add(0xE5, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE5) });
                QSPICommandDictionary.Add(0xE2, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Non_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE2) });
                QSPICommandDictionary.Add(0xE3, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Non_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE3) });
                QSPICommandDictionary.Add(0xE4, new QSPICommandInfo { QSPICommands = eQSPICommands.Erase_Non_Volatile_Lock_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xE4) });
                QSPICommandDictionary.Add(0xA7, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Global_Freeze_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xA7) });
                QSPICommandDictionary.Add(0xA6, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Global_Freeze_Bits, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0xA6) });
                QSPICommandDictionary.Add(0x27, new QSPICommandInfo { QSPICommands = eQSPICommands.Read_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x27) });
                QSPICommandDictionary.Add(0x28, new QSPICommandInfo { QSPICommands = eQSPICommands.Write_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x28) });
                QSPICommandDictionary.Add(0x29, new QSPICommandInfo { QSPICommands = eQSPICommands.Unlock_Password, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x29) });
                QSPICommandDictionary.Add(0xE0, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Read_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE0) });
                QSPICommandDictionary.Add(0xE1, new QSPICommandInfo { QSPICommands = eQSPICommands._4Byte_Write_Volatile_Lock_Bits, AddressByteCount = 4, DummyCycles = 0, Dlines = GetDataLines(0xE1) });
                QSPICommandDictionary.Add(0x9B, new QSPICommandInfo { QSPICommands = eQSPICommands.Interface_Activation, AddressByteCount = 0, DummyCycles = 0, Dlines = GetDataLines(0x9B) });
            }
        }

            private static Dictionary<eQSPIMode, CMDLines> GetDataLines(int ikey)
        {
            Dictionary<eQSPIMode, CMDLines> _lines = new Dictionary<eQSPIMode, CMDLines>();
            switch (ikey)
            {


                case 0x66:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x99:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x9E:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0x9F:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0xAF:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x5A:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0x03:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0x0B:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0x3B:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    break;

                case 0xBB:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines {CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    break;

                case 0x6B:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 4, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0xEB:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0x0D:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0x3D:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 2, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    break;

                case 0xBD:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    break;
                case 0x6D:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 4, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0xED:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0xE7:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 4 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 4 });
                    break;

                case 0x13:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0x0C:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0x3C:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    break;

                case 0xBC:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    break;


                case 0x6C:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 4, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0xEC:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0x0E:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0xBE:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 6 });
                    break;

                case 0xEE:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 8 });
                    break;

                case 0x06:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x04:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x05:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x70:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xB5:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x85:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x65:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xC8:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;


                case 0x01:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xB1:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x81:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;
                case 0x9B:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;


                case 0xE0:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xE1:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x7A:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x4B:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 8 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 10 });
                    break;

                case 0x42:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xB7:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xE9:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;
                case 0x35:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xF5:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xB9:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xAB:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x2D:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x2C:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;
                case 0xE8:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;
                case 0xE5:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xE2:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xE3:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;
                case 0xE4:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xA7:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0xA6:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x27:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    break;

                case 0x28:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x29:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x61:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xC5:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x50:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x02:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0xA2:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    break;

                case 0xD2:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    break;

                case 0x32:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 4, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;
                case 0x38:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;
                case 0x12:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 1, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 2, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x34:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 4, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;

                case 0x3E:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 4, DummyClkCycles = 0 });
                    break;


                case 0x52:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;


                case 0x20:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;


                case 0xD8:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xC7:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0xDC:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x21:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 1, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 2, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 4, Dataline = 0, DummyClkCycles = 0 });
                    break;

                case 0x75:
                    _lines.Add(eQSPIMode.Extended, new CMDLines { CMDline = 1, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Dual, new CMDLines { CMDline = 2, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    _lines.Add(eQSPIMode.Quad, new CMDLines { CMDline = 4, Addrline = 0, Dataline = 0, DummyClkCycles = 0 });
                    break;


            }
            return _lines;
        }
    }
}
