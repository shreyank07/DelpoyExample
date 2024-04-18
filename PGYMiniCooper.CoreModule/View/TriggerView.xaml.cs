using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PGYMiniCooper.CoreModule.View
{
    /// <summary>
    /// Interaction logic for TriggerView.xaml
    /// </summary>
    public partial class TriggerView : UserControl
    {
        public TriggerView()
        {
            InitializeComponent();
        }

        private void cmbCommandrffe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCommandrffe.SelectedItem != null)
            {
                txtByteCountrffe.Visibility = Visibility.Collapsed;
                lblbytecountrffe.Visibility = Visibility.Collapsed;

                txtDataMaskrffe.Visibility = Visibility.Collapsed;
                lblDataMaskrffe.Visibility = Visibility.Collapsed;

                txtregAddrrffe.Visibility = Visibility.Visible;
                lblregAddrrffe.Visibility = Visibility.Visible;

                txtDatarffe.Visibility = Visibility.Visible;
                lblDatarffe.Visibility = Visibility.Visible;

                txtinterrrupt.Visibility = Visibility.Collapsed;
                lblinterrupts.Visibility = Visibility.Collapsed;

                cmbAckNackrffe.Visibility = Visibility.Collapsed;
                lblackrffe.Visibility = Visibility.Collapsed;

                txtrxMIDrffe.Visibility = Visibility.Collapsed;
                lblrxMIDffe.Visibility = Visibility.Collapsed;

                if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.MASKED_WRITE)
                {
                    txtDataMaskrffe.Visibility = Visibility.Visible;
                    lblDataMaskrffe.Visibility = Visibility.Visible;
                    txtDatarffe.Visibility = Visibility.Visible;
                    lblDatarffe.Visibility = Visibility.Visible;
                }
                else if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.EXT_REG_WRITE || ((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.EXT_REG_READ ||
                    ((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.EXT_REG_WRITE_LONG || ((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.EXT_REG_READ_LONG)
                {
                    txtByteCountrffe.Visibility = Visibility.Visible; lblbytecountrffe.Visibility = Visibility.Visible;
                    txtregAddrrffe.Visibility = Visibility.Visible; lblregAddrrffe.Visibility = Visibility.Visible;
                    txtDatarffe.Visibility = Visibility.Visible; lblDatarffe.Visibility = Visibility.Visible;
                    txtDataMaskrffe.Visibility = Visibility.Collapsed; lblDataMaskrffe.Visibility = Visibility.Collapsed;
                }
                else if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE || ((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ)
                {
                    txtByteCountrffe.Visibility = Visibility.Visible; lblbytecountrffe.Visibility = Visibility.Visible;
                    txtregAddrrffe.Visibility = Visibility.Visible; lblregAddrrffe.Visibility = Visibility.Visible;
                    txtDatarffe.Visibility = Visibility.Visible; lblDatarffe.Visibility = Visibility.Visible;
                    txtDataMaskrffe.Visibility = Visibility.Collapsed; lblDataMaskrffe.Visibility = Visibility.Collapsed;
                }
                else if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.REG_ZERO_WRITE)
                {
                    txtregAddrrffe.Visibility = Visibility.Collapsed; lblregAddrrffe.Visibility = Visibility.Collapsed;
                    txtDatarffe.Visibility = Visibility.Visible; lblDatarffe.Visibility = Visibility.Visible;
                    txtDataMaskrffe.Visibility = Visibility.Collapsed; lblDataMaskrffe.Visibility = Visibility.Collapsed;
                }
                else if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.INT_SUMMARY_IDENT)
                {
                    txtinterrrupt.Visibility = Visibility.Visible; lblinterrupts.Visibility = Visibility.Visible;
                    txtregAddrrffe.Visibility = Visibility.Collapsed; lblregAddrrffe.Visibility = Visibility.Collapsed;
                    txtDatarffe.Visibility = Visibility.Collapsed; lblDatarffe.Visibility = Visibility.Collapsed;
                    txtDataMaskrffe.Visibility = Visibility.Collapsed; lblDataMaskrffe.Visibility = Visibility.Collapsed;
                }
                else if (((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
                {
                    txtinterrrupt.Visibility = Visibility.Collapsed; lblinterrupts.Visibility = Visibility.Collapsed;
                    txtregAddrrffe.Visibility = Visibility.Collapsed; lblregAddrrffe.Visibility = Visibility.Collapsed;
                    txtDatarffe.Visibility = Visibility.Collapsed; lblDatarffe.Visibility = Visibility.Collapsed;
                    txtDataMaskrffe.Visibility = Visibility.Collapsed; lblDataMaskrffe.Visibility = Visibility.Collapsed;
                    txtrxMIDrffe.Visibility = Visibility.Visible; lblrxMIDffe.Visibility = Visibility.Visible;
                    cmbAckNackrffe.Visibility = Visibility.Visible; lblackrffe.Visibility = Visibility.Visible;
                }
            }
        }

        private void cmbCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCommand.SelectedItem != null)
            {
                txtByteCount.Visibility = Visibility.Collapsed;
                lblbytecount.Visibility = Visibility.Collapsed;
                txtregAddr.Visibility = Visibility.Visible;
                lblregAddr.Visibility = Visibility.Visible;
                txtData.Visibility = Visibility.Visible;
                lblData.Visibility = Visibility.Visible;
                txtSlaveAddr.Visibility = Visibility.Visible;
                lblslaveaddr.Visibility = Visibility.Visible;

                if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.DEFAULT || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.AUTHENTICATE)
                {
                    txtSlaveAddr.Visibility = Visibility.Visible;
                    lblslaveaddr.Visibility = Visibility.Visible;
                    txtByteCount.Visibility = Visibility.Collapsed;
                    lblbytecount.Visibility = Visibility.Collapsed;
                    txtregAddr.Visibility = Visibility.Collapsed;
                    lblregAddr.Visibility = Visibility.Collapsed;
                    txtData.Visibility = Visibility.Collapsed;
                    lblData.Visibility = Visibility.Collapsed;
                }
                else if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.TRFR_BUS_OWNERSHIP)
                {
                    txtSlaveAddr.Visibility = Visibility.Collapsed;
                    lblslaveaddr.Visibility = Visibility.Collapsed;
                    txtByteCount.Visibility = Visibility.Collapsed;
                    lblbytecount.Visibility = Visibility.Collapsed;
                    txtregAddr.Visibility = Visibility.Collapsed;
                    lblregAddr.Visibility = Visibility.Collapsed;
                    txtData.Visibility = Visibility.Collapsed;
                    lblData.Visibility = Visibility.Collapsed;
                }
                else if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.EXT_REG_WRITE || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.EXT_REG_READ ||
                    ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.EXT_REG_WRITE_LONG || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.EXT_REG_READ_LONG)
                {
                    txtByteCount.Visibility = Visibility.Visible;
                    lblbytecount.Visibility = Visibility.Visible;
                    txtregAddr.Visibility = Visibility.Visible;
                    lblregAddr.Visibility = Visibility.Visible;
                    txtData.Visibility = Visibility.Visible;
                    lblData.Visibility = Visibility.Visible;
                }
                else if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.REG_ZERO_WRITE)
                {
                    txtregAddr.Visibility = Visibility.Collapsed;
                    lblregAddr.Visibility = Visibility.Collapsed;
                    txtData.Visibility = Visibility.Visible;
                    lblData.Visibility = Visibility.Visible;
                }
                else if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.TRFR_BUS_OWNERSHIP)
                {
                    txtregAddr.Visibility = Visibility.Collapsed;
                    lblregAddr.Visibility = Visibility.Collapsed;
                    txtData.Visibility = Visibility.Collapsed;
                    lblData.Visibility = Visibility.Collapsed;
                }
                if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.SLEEP || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.RESET || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.WAKEUP || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.SHUTDOWN)
                {
                    txtregAddr.Visibility = Visibility.Collapsed;
                    lblregAddr.Visibility = Visibility.Collapsed;
                    txtData.Visibility = Visibility.Collapsed;
                    lblData.Visibility = Visibility.Collapsed;
                    txtByteCount.Visibility = Visibility.Collapsed;
                    lblbytecount.Visibility = Visibility.Collapsed;
                }
                if (((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.SLEEP || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.RESET ||
                    ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.WAKEUP || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.SHUTDOWN ||
                    ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.REG_WRITE || ((eSPMICMDTYPE)cmbCommand.SelectedItem) == eSPMICMDTYPE.REG_ZERO_WRITE)
                {
                    if (SessionConfiguration.Version == eVersion.one)
                    {
                        cmbAckNack.Visibility = Visibility.Collapsed;
                        lblack.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        cmbAckNack.Visibility = Visibility.Visible;
                        lblack.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    cmbAckNack.Visibility = Visibility.Collapsed;
                    lblack.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void cmbAckNackrffe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAckNackrffe.SelectedItem != null && ((eRFFECMDTYPE)cmbCommandrffe.SelectedItem) == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
            {
                if (((eAcknowledgeType)cmbAckNackrffe.SelectedItem) == eAcknowledgeType.ACK)
                {
                    lblrxMIDffe.Visibility = Visibility.Visible;
                    txtrxMIDrffe.Visibility = Visibility.Visible;
                }
                else
                {
                    lblrxMIDffe.Visibility = Visibility.Collapsed;
                    txtrxMIDrffe.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void cmb7EAck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb7EAck.SelectedItem != null)
            {
                if (((eAcknowledgeType)cmb7EAck.SelectedItem) == eAcknowledgeType.ACK && brdcast.IsChecked == false)
                {
                    grdslave.Visibility = Visibility.Visible;
                }
                else
                {
                    grdslave.Visibility = Visibility.Collapsed;
                    lbli3cdata.Visibility = Visibility.Collapsed;
                    i3cdata.Visibility = Visibility.Collapsed;
                    lbli3chdrcmd.Visibility = Visibility.Collapsed;
                    i3chdrcmd.Visibility = Visibility.Collapsed;
                    lbli3chdrdata.Visibility = Visibility.Collapsed;
                    i3chdrdata.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void broadcast_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbi3cslaveAck.SelectedItem = "NACK";
            grdslave.Visibility = Visibility.Collapsed;
            if (brdcast.IsChecked == true && cmbbroadcast.SelectedItem != null && ((eAcknowledgeType)cmb7EAck.SelectedItem) == eAcknowledgeType.ACK)
            {
                if (((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR0 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR1 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR2 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR3 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR4 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR5 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR6 || ((eBroadcastCCC)cmbbroadcast.SelectedItem) == eBroadcastCCC.ENTHDR7)
                {
                    lbli3cdata.Visibility = Visibility.Collapsed;
                    i3cdata.Visibility = Visibility.Collapsed;
                    lbli3chdrcmd.Visibility = Visibility.Visible;
                    i3chdrcmd.Visibility = Visibility.Visible;
                    lbli3chdrdata.Visibility = Visibility.Visible;
                    i3chdrdata.Visibility = Visibility.Visible;
                }
                else
                {
                    lbli3cdata.Visibility = Visibility.Collapsed;
                    i3cdata.Visibility = Visibility.Collapsed;
                    lbli3chdrcmd.Visibility = Visibility.Collapsed;
                    i3chdrcmd.Visibility = Visibility.Collapsed;
                    lbli3chdrdata.Visibility = Visibility.Collapsed;
                    i3chdrdata.Visibility = Visibility.Collapsed;

                    var hasData = ProtocolInfoRepository.GetCommandInfo((int)((eBroadcastCCC)cmbbroadcast.SelectedItem)).HasData;
                    if (hasData)
                    {
                        i3cdata.Visibility = Visibility.Visible;
                        lbli3cdata.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        i3cdata.Visibility = Visibility.Collapsed;
                        lbli3cdata.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void directed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (directed.IsChecked == true && cmbdirected.SelectedItem != null)
            {
                lbli3cslaveAck.Visibility = Visibility.Visible;
                cmbi3cslaveAck.Visibility = Visibility.Visible;
                cmbi3cslavetransfer.IsEnabled = false;
                i3cdata.Visibility = Visibility.Collapsed;
                lbli3cdata.Visibility = Visibility.Collapsed;
                cmbi3cslavetransfer.IsEnabled = false;
                cmbi3cslaveAck.SelectedItem = "NACK";
                grdslave.Visibility = Visibility.Visible;
                var transferType = ProtocolInfoRepository.GetCommandInfo((int)((eBroadcastCCC)cmbdirected.SelectedItem)).TransferType;
                if (transferType == eTransferType.WR)
                    cmbi3cslavetransfer.SelectedItem = "WR";
                else
                    cmbi3cslavetransfer.SelectedItem = "RD";
            }
        }

        private void private_Checked(object sender, RoutedEventArgs e)
        {
            grdslave.Visibility = Visibility.Visible;
            i3cdata.Visibility = Visibility.Collapsed;
            lbli3cdata.Visibility = Visibility.Collapsed;
            cmbi3cslavetransfer.IsEnabled = true;
            cmbi3cslaveAck.SelectedItem = "NACK";
            lbli3chdrcmd.Visibility = Visibility.Collapsed;
            i3chdrcmd.Visibility = Visibility.Collapsed;
            lbli3chdrdata.Visibility = Visibility.Collapsed;
            i3chdrdata.Visibility = Visibility.Collapsed;
        }

        private void cmbi3cslaveAck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbi3cslaveAck.SelectedItem != null)
            {
                if (((eAcknowledgeType)cmbi3cslaveAck.SelectedItem) == eAcknowledgeType.ACK)
                {
                    lbli3cdata.Visibility = Visibility.Visible;
                    i3cdata.Visibility = Visibility.Visible;
                }
                else
                {
                    lbli3cdata.Visibility = Visibility.Collapsed;
                    i3cdata.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void directed_Checked(object sender, RoutedEventArgs e)
        {
            cmbi3cslavetransfer.IsEnabled = false;
            cmbi3cslaveAck.SelectedItem = "NACK";
            cmb7EAck.SelectedItem = "NACK";
            lbli3cdata.Visibility = Visibility.Collapsed;
            i3cdata.Visibility = Visibility.Collapsed;
            lbli3chdrcmd.Visibility = Visibility.Collapsed;
            i3chdrcmd.Visibility = Visibility.Collapsed;
            lbli3chdrdata.Visibility = Visibility.Collapsed;
            i3chdrdata.Visibility = Visibility.Collapsed;
            grdslave.Visibility = Visibility.Collapsed;
        }

        private void brdcast_Checked(object sender, RoutedEventArgs e)
        {
            grdslave.Visibility = Visibility.Collapsed;
            cmbi3cslavetransfer.IsEnabled = false;
            cmb7EAck.SelectedItem = "NACK";
            cmbi3cslaveAck.SelectedItem = "NACK";
            lbli3cdata.Visibility = Visibility.Collapsed;
            i3cdata.Visibility = Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Prototype.Visibility = Visibility.Visible;
        }
    }
}
