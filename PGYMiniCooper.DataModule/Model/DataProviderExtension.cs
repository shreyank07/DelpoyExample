using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using Prodigy.Interfaces;
using Prodigy.Interfaces.I3C;

public static class DataProviderExtension
{
    public static IFrame ToFrame_I3C(ProtocolFrame protocolFrame)
    {
        FramePattern frame = new FramePattern();
        frame.StartTime = protocolFrame.StartTime;
        frame.StopTime = protocolFrame.StopTime;
        frame.ProtocolType = eProtocol.I3C;
        frame.FrameType = (eMajorFrame)(protocolFrame.Frame);
        frame.FrameIndex = (int)protocolFrame.FrameIndex;
        foreach (var packet in protocolFrame.Packets)
        {
            IMessage msg = null;
            var endMessage = packet.Message.Unpack<Message>();

            if (endMessage.Address != null)
            {
                msg = new AddressMessageModel();
                msg.Frequency = endMessage.Address.Frequency;
                msg.PacketType = (ePacketType)endMessage.Address.PacketType;
                msg.HostDevice = (eHostDevice)endMessage.Address.HostDevice;
                (msg as AddressMessageModel).TransferType = (eTransferType)(endMessage.Address.TransferType);
                (msg as AddressMessageModel).Start = (eStartType)(endMessage.Address.StartType);
                (msg as AddressMessageModel).AckType = (eAcknowledgeType)(endMessage.Address.AckType);
                (msg as AddressMessageModel).Value = (byte)endMessage.Address.AddressValue;
                (msg as AddressMessageModel).Stop = endMessage.HasStop;
            }

            if (endMessage.Command != null)
            {
                msg = new CommandMessageModel();
                msg.Frequency = endMessage.Command.Frequency;
                msg.PacketType = (ePacketType)endMessage.Command.PacketType;
                msg.HostDevice = (eHostDevice)endMessage.Command.HostDevice;
                (msg as CommandMessageModel).Value = (byte)endMessage.Command.CommandValue;
                (msg as CommandMessageModel).Stop = endMessage.HasStop;
            }

            if (endMessage.Data != null && endMessage.Data.Description == DataDescription.Pid)
            {
                msg = new PIDDAAMessageModel();
                msg.Frequency = endMessage.Data.Frequency;
                msg.PacketType = (ePacketType)endMessage.Data.PacketType;
                msg.HostDevice = (eHostDevice)endMessage.Data.HostDevice;
                (msg as PIDDAAMessageModel).Value = (long)endMessage.Data.DataValue;
                (msg as PIDDAAMessageModel).Stop = endMessage.HasStop;
                (msg as PIDDAAMessageModel).TransmitBit = (byte)endMessage.Data.TransmitBit;
            }

            if (endMessage.Data != null && endMessage.Data.Description != DataDescription.Pid)
            {
                msg = new DataMessageModel();
                msg.Frequency = endMessage.Data.Frequency;
                msg.PacketType = (ePacketType)endMessage.Data.PacketType;
                msg.HostDevice = (eHostDevice)endMessage.Data.HostDevice;
                (msg as DataMessageModel).Value = (int)endMessage.Data.DataValue;
                (msg as DataMessageModel).Stop = endMessage.HasStop;
                (msg as DataMessageModel).TransmitBit = (byte)endMessage.Data.TransmitBit;
            }

            if (endMessage.HDR != null)
            {
                msg = new HDRMessageModel();
                msg.Frequency = endMessage.HDR.Frequency;
                msg.PacketType = (ePacketType)endMessage.HDR.PacketType;
                msg.HostDevice = (eHostDevice)endMessage.HDR.HostDevice;
                (msg as HDRMessageModel).Value = (int)endMessage.HDR.HDRValue;
                (msg as HDRMessageModel).Stop = endMessage.HasStop;
            }

            if (msg != null)
            {
                msg.TimeStamp = packet.IndexAndTime.StartTime;
                msg.StopTime = packet.IndexAndTime.StopTime;
                frame.PacketCollection.Add(msg);

            }
        }

        return frame;
    }
}
