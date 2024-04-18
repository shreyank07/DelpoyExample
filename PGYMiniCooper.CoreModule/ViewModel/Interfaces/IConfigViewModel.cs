using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using Prodigy.Business;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PGYMiniCooper.CoreModule.ViewModel.Interfaces
{
    public interface IConfigViewModel
    {
        string Name { get; }

        eProtocol ProtocolType { get; }

        IReadOnlyList<ChannelInfo> Channels { get; }

        IConfigModel Model { get; }
    }
}
