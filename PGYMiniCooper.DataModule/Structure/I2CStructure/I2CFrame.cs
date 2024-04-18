//**********************************************************
// Author: Vib
// Created: 31/7/2020
// Modified: 4/8/2020
// Copyright (c) 2020 Prodigy Technovations Pvt Ltd
// Description: I2C packet details storage class
//*********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using ProdigyFramework.Behavior;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class I2CFrame : IFrame, INotifyPropertyChanged
    {
      
        public I2CFrame()
        {
            dataBytes = new List<byte>();
            ackData = new List<byte>();
            addressSecond = 0x00;
            error = eErrorType.None;
        }
        private int index;

        public int FrameIndex
        {
            get { return index; }
            set { index = value; }
        }

        private double startTime;

        public double StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        private eStartType start;

        public eStartType Start
        {
            get { return start; }
            set { start = value; }
        }
        private byte addressFirst;

        public byte AddressFirst
        {
            get { return addressFirst; }
            set { addressFirst = value; }
        }
        private byte addressSecond;

        public byte AddressSecond
        {
            get { return addressSecond; }
            set { addressSecond = value; }
        }
        private byte ackAddr;
        //0xXF - NACK - Addr first byte
        //0xX0 - ACK - Addr first byte
        //0xFX - NACK - Addr second byte
        //0x0X - ACK - Addr second byte
        public byte AckAddr
        {
            get { return ackAddr; }
            set { ackAddr = value; }
        }
        private List<byte> dataBytes;

        public List<byte> DataBytes
        {
            get { return dataBytes; }
            set { dataBytes = value; }
        }
        private List<byte> ackData;

        public List<byte> AckData
        {
            get { return ackData; }
            set { ackData = value; }
        }
        private eStartStop stop;

        public eStartStop Stop
        {
            get { return stop; }
            set { stop = value; }
        }
        private double stopTime;

        public double StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }
        private double frequency;

        public double Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }
        private eErrorType error;

        public eErrorType ErrorType
        {
            get { return error; }
            set { error = value; }
        }

        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }

        private bool isHighlighted;
        public bool IsHighlighted
        {
            get => isHighlighted;
            set
            {
                if (value == isHighlighted) return;
                isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public string ProtocolName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class AddressHelper
    {
        public object ConvertAddress(object[] values)
        {
            if (System.Convert.ToInt32(values[1]) == 0)
            {
                //7b Address
                var firstaddr = System.Convert.ToInt32(values[0]);
                firstaddr = firstaddr >> 1;
                return firstaddr;
            }
            else
            {
                //10b Address
                var firstaddr = System.Convert.ToInt32(values[0]);
                firstaddr = firstaddr >> 1;
                firstaddr = firstaddr & 0x3;
                var secondAddr = System.
                    Convert.ToInt32(values[1]);
                return (firstaddr << 8 | secondAddr);

            }
        }

        public string ConvertAddressAcK(object value, int parameter)
        {
            object result = ConvertAddressAckCore(value, parameter);
            if (result is int)
            {
                switch ((int)result)
                {
                    case 0:
                        return "WR";
                    case 1:
                        return "RD";
                }
            }

            return result.ToString();
        }

        private static object ConvertAddressAckCore(object value, int parameter)
        {
            if (value == null)
                return "";
            if (Convert.ToInt32(parameter) == 0x01)
                return System.Convert.ToInt32(value) >> 1;
            else if ((System.Convert.ToInt32(parameter) == 0x02))
                return System.Convert.ToInt32(value) & 0x01;
            else
            {
                if ((System.Convert.ToInt32(value) & 0x0F) == 0x0F)
                    return "NACK";
                else
                    return "ACK";
            }
        }

    }
}
