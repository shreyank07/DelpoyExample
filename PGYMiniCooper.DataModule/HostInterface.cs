using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Service;
using PGYMiniCooper.DataModule.Structure;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using ProdigyFramework.Model;
using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.DataModule.Interface;

namespace PGYMiniCooper.DataModule
{
    public sealed class HostInterface : IDisposable
    {
        /// <summary>
        /// This property would be set on initialize method
        /// </summary>
        private static ConfigModel configModel;
        IProdigyService prodigyService;
        //ReportGenerator report;

        public HostInterface()
        {
            //report = new ReportGenerator(null, configModel, protocolActivity);
            //prodigyService = OnlineService.GetInstance();
        }

        //public bool Initialize(ConfigModel configModel)
        //{
        //    return prodigyService.Initialize(configModel);
        //}

        bool ConnectionStatus = false;
        public eResponseFlag EstablishConnection()
        {
            //if (prodigyService.CheckConnection())
            //{
            //    ConnectionStatus = true;
            //    return eResponseFlag.Success;
            //}
            //else
            {
                ConnectionStatus = false;
                return eResponseFlag.Connection_Fail;
            }
        }
        public eResponseFlag StartCapture(byte[] triggerBytes = null)
        {
            if (ConnectionStatus)
            {
                if (!SessionConfiguration.IsDecodeActive)
                {
                    if (prodigyService == null)
                        //prodigyService = OnlineService.GetInstance();




                    Task.Factory.StartNew(() =>
                    {
                        //prodigyService.StartCapture(configModel, triggerBytes);
                    });
                    return eResponseFlag.Success;
                }
                else
                {
                    return eResponseFlag.Task_Busy;
                }
            }
            else
            {
                return eResponseFlag.Connection_Fail;
            }
        }

        public eResponseFlag StopCapture()
        {
            if (ConnectionStatus)
            {
                if (prodigyService == null)
                    //prodigyService = OnlineService.GetInstance();

                Task.Factory.StartNew(() =>
                {
                    //prodigyService.StopCapture(configModel);
                });
                return eResponseFlag.Success;
            }
            else
            {
                return eResponseFlag.Connection_Fail;
            }
        }

        public eResponseFlag ResetHardwareDevice()
        {
            if (ConnectionStatus)
            {
                if (prodigyService == null)
                    //prodigyService = OnlineService.GetInstance();

                Task.Factory.StartNew(() =>
                {
                    //prodigyService.ResetHardware();
                });
                return eResponseFlag.Success;
            }
            else
            {
                return eResponseFlag.Connection_Fail;
            }
        }

        public eResponseFlag ConfigureMode(eConfigMode mode)
        {
            configModel.ConfigurationMode = mode;
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureSampleRate(eSampleRate sampleRate)
        {
            configModel.SampleRateLAPA = sampleRate;
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureStateModeCLOCK1(eChannles CLOCK1 = eChannles.None, eGrouping group = eGrouping.Group1, eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None, eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.State;

            if (CLOCK1 != eChannles.None && group == eGrouping.Group1)
            {
                configModel.SelectedClock1 = CLOCK1;
                if (configModel.SelectedClock1 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
                if (configModel.SelectedClock1 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }

                else
                if (configModel.SelectedClock1 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }

                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK1 != CH1 && !configModel.SelectedCLK1_GRP1.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK1 != CH2 && !configModel.SelectedCLK1_GRP1.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK1 != CH3 && !configModel.SelectedCLK1_GRP1.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK1 != CH4 && !configModel.SelectedCLK1_GRP1.Contains(CH4))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK1 != CH5 && !configModel.SelectedCLK1_GRP1.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK1 != CH6 && !configModel.SelectedCLK1_GRP1.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK1 != CH7 && !configModel.SelectedCLK1_GRP1.Contains(CH7))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK1 != CH8 && !configModel.SelectedCLK1_GRP1.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK1 != CH9 && !configModel.SelectedCLK1_GRP1.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //   config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (CLOCK1 != CH10 && !configModel.SelectedCLK1_GRP1.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH11 == eChannles.None))
                {
                    if (CLOCK1 != CH11 && !configModel.SelectedCLK1_GRP1.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH12 == eChannles.None))
                {
                    if (CLOCK1 != CH12 && !configModel.SelectedCLK1_GRP1.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH13 == eChannles.None))
                {
                    if (CLOCK1 != CH13 && !configModel.SelectedCLK1_GRP1.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH14 == eChannles.None))
                {
                    if (CLOCK1 != CH14 && !configModel.SelectedCLK1_GRP1.Contains(CH14))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH15 == eChannles.None))
                {
                    if (CLOCK1 != CH15 && !configModel.SelectedCLK1_GRP1.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH16 == eChannles.None))
                {
                    if (CLOCK1 != CH16 && !configModel.SelectedCLK1_GRP1.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
            }
            else if (CLOCK1 != eChannles.None && group == eGrouping.Group2)
            {
                configModel.SelectedClock1 = CLOCK1;
                if (configModel.SelectedClock1 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
             if (configModel.SelectedClock1 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                if (configModel.SelectedClock1 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }

                else
             if (configModel.SelectedClock1 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }

                else
             if (configModel.SelectedClock1 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }

                else
             if (configModel.SelectedClock1 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }
                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK1 != CH1 && !configModel.SelectedCLK1_GRP2.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK1 != CH2 && !configModel.SelectedCLK1_GRP2.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                // config.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK1 != CH3 && !configModel.SelectedCLK1_GRP2.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK1 != CH4 && !configModel.SelectedCLK1_GRP2.Contains(CH4))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                ///   config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK1 != CH5 && !configModel.SelectedCLK1_GRP2.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK1 != CH6 && !configModel.SelectedCLK1_GRP2.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK1 != CH7 && !configModel.SelectedCLK1_GRP2.Contains(CH7))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK1 != CH8 && !configModel.SelectedCLK1_GRP2.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //    config.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK1 != CH9 && !configModel.SelectedCLK1_GRP2.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {

                    if (CLOCK1 != CH10 && !configModel.SelectedCLK1_GRP2.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH11 == eChannles.None))
                {

                    if (CLOCK1 != CH11 && !configModel.SelectedCLK1_GRP2.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH12 == eChannles.None))
                {

                    if (CLOCK1 != CH12 && !configModel.SelectedCLK1_GRP2.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH13 == eChannles.None))
                {

                    if (CLOCK1 != CH13 && !configModel.SelectedCLK1_GRP2.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH14 == eChannles.None))
                {

                    if (CLOCK1 != CH14 && !configModel.SelectedCLK1_GRP2.Contains(CH14))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH15 == eChannles.None))
                {

                    if (CLOCK1 != CH15 && !configModel.SelectedCLK1_GRP2.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH16 == eChannles.None))
                {

                    if (CLOCK1 != CH16 && !configModel.SelectedCLK1_GRP2.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                //else
                // config.IsCh10Visible = false;
            }
            else if (CLOCK1 != eChannles.None && group == eGrouping.Group3)
            {
                configModel.SelectedClock1 = CLOCK1;

                if (configModel.SelectedClock1 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
             if (configModel.SelectedClock1 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                      if (configModel.SelectedClock1 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }
                else
             if (configModel.SelectedClock1 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }

                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK1 != CH1 && !configModel.SelectedCLK1_GRP3.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK1 != CH2 && !configModel.SelectedCLK1_GRP3.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh2Visible = false;
                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK1 != CH3 && !configModel.SelectedCLK1_GRP3.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK1 != CH4 && !configModel.SelectedCLK1_GRP3.Contains(CH4))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK1 != CH5 && !configModel.SelectedCLK1_GRP3.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK1 != CH6 && !configModel.SelectedCLK1_GRP3.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK1 != CH7 && !configModel.SelectedCLK1_GRP3.Contains(CH7))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK1 != CH8 && !configModel.SelectedCLK1_GRP3.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK1 != CH9 && !configModel.SelectedCLK1_GRP3.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (CLOCK1 != CH10 && !configModel.SelectedCLK1_GRP3.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH11 == eChannles.None))
                {
                    if (CLOCK1 != CH11 && !configModel.SelectedCLK1_GRP3.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH12 == eChannles.None))
                {
                    if (CLOCK1 != CH12 && !configModel.SelectedCLK1_GRP3.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH13 == eChannles.None))
                {
                    if (CLOCK1 != CH13 && !configModel.SelectedCLK1_GRP3.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH14 == eChannles.None))
                {
                    if (CLOCK1 != CH14 && !configModel.SelectedCLK1_GRP3.Contains(CH14))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH15 == eChannles.None))
                {
                    if (CLOCK1 != CH15 && !configModel.SelectedCLK1_GRP3.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH16 == eChannles.None))
                {
                    if (CLOCK1 != CH16 && !configModel.SelectedCLK1_GRP3.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

            }
            return eResponseFlag.Success;
        }

        public eResponseFlag ClearAllChannelSelection()
        {
            configModel.ResetChannels();
            return eResponseFlag.Success;
        }





        public eResponseFlag ConfigureStateModeCLOCK2(eChannles CLOCK2 = eChannles.None, eGrouping group = eGrouping.Group1, eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None, eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.State;

            if (CLOCK2 != eChannles.None && group == eGrouping.Group1)
            {
                configModel.SelectedClock2 = CLOCK2;
                if (configModel.SelectedClock2 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
                if (configModel.SelectedClock2 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }
                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK2 != CH1 && !configModel.SelectedCLK2_GRP1.Contains(CH1))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //   config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK2 != CH2 && !configModel.SelectedCLK2_GRP1.Contains(CH2))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK2 != CH3 && !configModel.SelectedCLK2_GRP1.Contains(CH3))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK2 != CH4 && !configModel.SelectedCLK2_GRP1.Contains(CH4))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                ///  config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK2 != CH5 && !configModel.SelectedCLK2_GRP1.Contains(CH5))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //   else
                //   config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK2 != CH6 && !configModel.SelectedCLK2_GRP1.Contains(CH6))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //   config.IsCh6Visible = false;


                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK2 != CH7 && !configModel.SelectedCLK2_GRP1.Contains(CH7))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK2 != CH8 && !configModel.SelectedCLK2_GRP1.Contains(CH8))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //   else
                // config.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK2 != CH9 && !configModel.SelectedCLK2_GRP1.Contains(CH9))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (CLOCK2 != CH10 && !configModel.SelectedCLK2_GRP1.Contains(CH10))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH11 == eChannles.None))
                {
                    if (CLOCK2 != CH11 && !configModel.SelectedCLK2_GRP1.Contains(CH11))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH12 == eChannles.None))
                {
                    if (CLOCK2 != CH12 && !configModel.SelectedCLK2_GRP1.Contains(CH12))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                if (!(CH13 == eChannles.None))
                {
                    if (CLOCK2 != CH13 && !configModel.SelectedCLK2_GRP1.Contains(CH13))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH14 == eChannles.None))
                {
                    if (CLOCK2 != CH14 && !configModel.SelectedCLK2_GRP1.Contains(CH14))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH15 == eChannles.None))
                {
                    if (CLOCK2 != CH15 && !configModel.SelectedCLK2_GRP1.Contains(CH15))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

                if (!(CH16 == eChannles.None))
                {
                    if (CLOCK2 != CH16 && !configModel.SelectedCLK2_GRP1.Contains(CH16))
                    {
                        configModel.SelectedCLK2_GRP1.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }

            }
            else if (CLOCK2 != eChannles.None && group == eGrouping.Group2)
            {
                configModel.SelectedClock2 = CLOCK2;
                if (configModel.SelectedClock2 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
         if (configModel.SelectedClock2 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                if (configModel.SelectedClock2 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }

                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK2 != CH1 && !configModel.SelectedCLK2_GRP2.Contains(CH1))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK2 != CH2 && !configModel.SelectedCLK2_GRP2.Contains(CH2))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //     config.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK2 != CH3 && !configModel.SelectedCLK2_GRP2.Contains(CH3))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //    config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK2 != CH4 && !configModel.SelectedCLK2_GRP2.Contains(CH4))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                ///   config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK2 != CH5 && !configModel.SelectedCLK2_GRP2.Contains(CH5))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK2 != CH6 && !configModel.SelectedCLK2_GRP2.Contains(CH6))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK2 != CH7 && !configModel.SelectedCLK2_GRP2.Contains(CH7))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK2 != CH8 && !configModel.SelectedCLK2_GRP2.Contains(CH8))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK2 != CH9 && !configModel.SelectedCLK2_GRP2.Contains(CH9))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //    config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (CLOCK2 != CH10 && !configModel.SelectedCLK2_GRP2.Contains(CH10))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }



                if (!(CH11 == eChannles.None))
                {
                    if (CLOCK2 != CH11 && !configModel.SelectedCLK2_GRP2.Contains(CH11))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh1Visible = false;

                if (!(CH12 == eChannles.None))
                {
                    if (CLOCK2 != CH12 && !configModel.SelectedCLK2_GRP2.Contains(CH12))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //     config.IsCh2Visible = false;

                if (!(CH13 == eChannles.None))
                {
                    if (CLOCK2 != CH13 && !configModel.SelectedCLK2_GRP2.Contains(CH13))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //    config.IsCh3Visible = false;

                if (!(CH14 == eChannles.None))
                {
                    if (CLOCK2 != CH14 && !configModel.SelectedCLK2_GRP2.Contains(CH14))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                ///   config.IsCh4Visible = false;

                if (!(CH15 == eChannles.None))
                {
                    if (CLOCK2 != CH15 && !configModel.SelectedCLK2_GRP2.Contains(CH15))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh5Visible = false;

                if (!(CH16 == eChannles.None))
                {
                    if (CLOCK2 != CH16 && !configModel.SelectedCLK2_GRP2.Contains(CH16))
                    {
                        configModel.SelectedCLK2_GRP2.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //  config.IsCh10Visible = false;
            }
            else if (CLOCK2 != eChannles.None && group == eGrouping.Group3)
            {
                configModel.SelectedClock2 = CLOCK2;
                if (configModel.SelectedClock2 == eChannles.CH1)
                {
                    configModel.IsCh1ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH2)
                {
                    configModel.IsCh2ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH3)
                {
                    configModel.IsCh3ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH4)
                {
                    configModel.IsCh4ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH5)
                {
                    configModel.IsCh5ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH6)
                {
                    configModel.IsCh6ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH7)
                {
                    configModel.IsCh7ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH8)
                {
                    configModel.IsCh8ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH9)
                {
                    configModel.IsCh9ClkVisible = true;

                }
                else
         if (configModel.SelectedClock2 == eChannles.CH10)
                {
                    configModel.IsCh10ClkVisible = true;
                }
                else
                 if (configModel.SelectedClock2 == eChannles.CH11)
                {
                    configModel.IsCh11ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH12)
                {
                    configModel.IsCh12ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH13)
                {
                    configModel.IsCh13ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH14)
                {
                    configModel.IsCh14ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH15)
                {
                    configModel.IsCh15ClkVisible = true;
                }
                else
         if (configModel.SelectedClock2 == eChannles.CH16)
                {
                    configModel.IsCh16ClkVisible = true;
                }

                if (!(CH1 == eChannles.None))
                {
                    if (CLOCK2 != CH1 && !configModel.SelectedCLK2_GRP3.Contains(CH1))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (CLOCK2 != CH2 && !configModel.SelectedCLK2_GRP3.Contains(CH2))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //config.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (CLOCK2 != CH3 && !configModel.SelectedCLK2_GRP3.Contains(CH3))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (CLOCK2 != CH4 && !configModel.SelectedCLK2_GRP3.Contains(CH4))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (CLOCK2 != CH5 && !configModel.SelectedCLK2_GRP3.Contains(CH5))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //   config.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (CLOCK2 != CH6 && !configModel.SelectedCLK2_GRP3.Contains(CH6))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                // config.IsCh6Visible = false;
                if (!(CH7 == eChannles.None))
                {
                    if (CLOCK2 != CH7 && !configModel.SelectedCLK2_GRP3.Contains(CH7))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (CLOCK2 != CH8 && !configModel.SelectedCLK2_GRP3.Contains(CH8))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh8Visible = false;
                if (!(CH9 == eChannles.None))
                {
                    if (CLOCK2 != CH9 && !configModel.SelectedCLK2_GRP3.Contains(CH9))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH9);
                        configModel.IsCh9Visible = true;

                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (CLOCK2 != CH10 && !configModel.SelectedCLK2_GRP3.Contains(CH10))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }


                if (!(CH11 == eChannles.None))
                {
                    if (CLOCK2 != CH11 && !configModel.SelectedCLK2_GRP3.Contains(CH11))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //  else
                //  config.IsCh1Visible = false;

                if (!(CH12 == eChannles.None))
                {
                    if (CLOCK2 != CH12 && !configModel.SelectedCLK2_GRP3.Contains(CH12))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //config.IsCh2Visible = false;

                if (!(CH13 == eChannles.None))
                {
                    if (CLOCK2 != CH13 && !configModel.SelectedCLK2_GRP3.Contains(CH13))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                // config.IsCh3Visible = false;

                if (!(CH14 == eChannles.None))
                {
                    if (CLOCK2 != CH14 && !configModel.SelectedCLK2_GRP3.Contains(CH14))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //  config.IsCh4Visible = false;

                if (!(CH15 == eChannles.None))
                {
                    if (CLOCK2 != CH15 && !configModel.SelectedCLK2_GRP3.Contains(CH15))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                // else
                //   config.IsCh5Visible = false;

                if (!(CH16 == eChannles.None))
                {
                    if (CLOCK2 != CH16 && !configModel.SelectedCLK2_GRP3.Contains(CH16))
                    {
                        configModel.SelectedCLK2_GRP3.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                //else
                //config.IsCh10Visible = false;
            }


            return eResponseFlag.Success;
        }

        public eResponseFlag configureStateModeIndividualCLOCK1(eChannles CLOCK1 = eChannles.None, eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None, eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.State;
            configModel.GroupType = eGroupType.Individual;
            configModel.SelectedClock1 = CLOCK1;

            if (!(CH1 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH1))
                {
                    configModel.SelectedIndividualChannels.Add(CH1);
                    configModel.IsCh1Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh1Visible = false;

            if (!(CH2 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH2))
                {
                    configModel.SelectedIndividualChannels.Add(CH2);
                    configModel.IsCh2Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh2Visible = false;

            if (!(CH3 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH3))
                {
                    configModel.SelectedIndividualChannels.Add(CH3);
                    configModel.IsCh3Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh3Visible = false;


            if (!(CH4 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH4))
                {
                    configModel.SelectedIndividualChannels.Add(CH4);
                    configModel.IsCh4Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh4Visible = false;

            if (!(CH5 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH5))
                {
                    configModel.SelectedIndividualChannels.Add(CH5);
                    configModel.IsCh5Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh5Visible = false;


            if (!(CH6 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH6))
                {
                    configModel.SelectedIndividualChannels.Add(CH6);
                    configModel.IsCh6Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh6Visible = false;



            if (!(CH7 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH7))
                {
                    configModel.SelectedIndividualChannels.Add(CH7);
                    configModel.IsCh7Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh7Visible = false;


            if (!(CH8 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH8))
                {
                    configModel.SelectedIndividualChannels.Add(CH8);
                    configModel.IsCh8Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh8Visible = false;


            if (!(CH9 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH9))
                {
                    configModel.SelectedIndividualChannels.Add(CH9);
                    configModel.IsCh9Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh9Visible = false;



            if (!(CH10 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH10))
                {
                    configModel.SelectedIndividualChannels.Add(CH10);
                    configModel.IsCh10Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh10Visible = false;




            if (!(CH11 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH11))
                {
                    configModel.SelectedIndividualChannels.Add(CH11);
                    configModel.IsCh11Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh11Visible = false;


            if (!(CH12 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH12))
                {
                    configModel.SelectedIndividualChannels.Add(CH12);
                    configModel.IsCh12Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh12Visible = false;


            if (!(CH13 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH13))
                {
                    configModel.SelectedIndividualChannels.Add(CH13);
                    configModel.IsCh13Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh13Visible = false;



            if (!(CH14 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH14))
                {
                    configModel.SelectedIndividualChannels.Add(CH14);
                    configModel.IsCh14Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh14Visible = false;


            if (!(CH15 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH15))
                {
                    configModel.SelectedIndividualChannels.Add(CH15);
                    configModel.IsCh15Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh15Visible = false;



            if (!(CH16 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH16))
                {
                    configModel.SelectedIndividualChannels.Add(CH16);
                    configModel.IsCh16Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh16Visible = false;

            return eResponseFlag.Success;
        }


        public eResponseFlag configureStateModeIndividualCLOCK2(eChannles CLOCK1 = eChannles.None, eChannles CLOCK2 = eChannles.None, eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None,
            eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.State;

            configModel.SelectedClock1 = CLOCK1;
            configModel.SelectedClock2 = CLOCK2;
            configModel.GroupType = eGroupType.Individual;

            if (!(CH1 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH1))
                {
                    configModel.SelectedIndividualChannels.Add(CH1);
                    configModel.IsCh1Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //   else
            //  config.IsCh1Visible = false;

            if (!(CH2 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH2))
                {
                    configModel.SelectedIndividualChannels.Add(CH2);
                    configModel.IsCh2Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //  else
            //  config.IsCh2Visible = false;

            if (!(CH3 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH3))
                {
                    configModel.SelectedIndividualChannels.Add(CH3);
                    configModel.IsCh3Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            // else
            //   config.IsCh3Visible = false;


            if (!(CH4 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH4))
                {
                    configModel.SelectedIndividualChannels.Add(CH4);
                    configModel.IsCh4Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //  else
            //   config.IsCh4Visible = false;

            if (!(CH5 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH5))
                {
                    configModel.SelectedIndividualChannels.Add(CH5);
                    configModel.IsCh5Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //  else
            //    config.IsCh5Visible = false;


            if (!(CH6 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH6))
                {
                    configModel.SelectedIndividualChannels.Add(CH6);
                    configModel.IsCh6Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //  else
            //  config.IsCh6Visible = false;



            if (!(CH7 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH7))
                {
                    configModel.SelectedIndividualChannels.Add(CH7);
                    configModel.IsCh7Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //else
            //  config.IsCh7Visible = false;


            if (!(CH8 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH8))
                {
                    configModel.SelectedIndividualChannels.Add(CH8);
                    configModel.IsCh8Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //else
            //   config.IsCh8Visible = false;


            if (!(CH9 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH9))
                {
                    configModel.SelectedIndividualChannels.Add(CH9);
                    configModel.IsCh9Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            //else
            //  config.IsCh9Visible = false;



            if (!(CH10 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH10))
                {
                    configModel.SelectedIndividualChannels.Add(CH10);
                    configModel.IsCh10Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            // else
            //   config.IsCh10Visible = false;


            if (!(CH11 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH11))
                {
                    configModel.SelectedIndividualChannels.Add(CH11);
                    configModel.IsCh11Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh11Visible = false;


            if (!(CH12 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH12))
                {
                    configModel.SelectedIndividualChannels.Add(CH12);
                    configModel.IsCh12Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh12Visible = false;


            if (!(CH13 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH13))
                {
                    configModel.SelectedIndividualChannels.Add(CH13);
                    configModel.IsCh13Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh13Visible = false;



            if (!(CH14 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH14))
                {
                    configModel.SelectedIndividualChannels.Add(CH14);
                    configModel.IsCh14Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh14Visible = false;


            if (!(CH15 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH15))
                {
                    configModel.SelectedIndividualChannels.Add(CH15);
                    configModel.IsCh15Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh15Visible = false;



            if (!(CH16 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH16))
                {
                    configModel.SelectedIndividualChannels.Add(CH16);
                    configModel.IsCh16Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh16Visible = false;
            return eResponseFlag.Success;
        }
        public eResponseFlag configureTimingModeIndividual(eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None, eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.Timing;
            configModel.GroupType = eGroupType.Individual;
            if (!(CH1 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH1))
                {
                    configModel.SelectedIndividualChannels.Add(CH1);
                    configModel.IsCh1Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh1Visible = false;

            if (!(CH2 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH2))
                {
                    configModel.SelectedIndividualChannels.Add(CH2);
                    configModel.IsCh2Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh2Visible = false;

            if (!(CH3 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH3))
                {
                    configModel.SelectedIndividualChannels.Add(CH3);
                    configModel.IsCh3Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh3Visible = false;


            if (!(CH4 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH4))
                {
                    configModel.SelectedIndividualChannels.Add(CH4);
                    configModel.IsCh4Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh4Visible = false;

            if (!(CH5 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH5))
                {
                    configModel.SelectedIndividualChannels.Add(CH5);
                    configModel.IsCh5Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh5Visible = false;


            if (!(CH6 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH6))
                {
                    configModel.SelectedIndividualChannels.Add(CH6);
                    configModel.IsCh6Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh6Visible = false;



            if (!(CH7 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH7))
                {
                    configModel.SelectedIndividualChannels.Add(CH7);
                    configModel.IsCh7Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh7Visible = false;


            if (!(CH8 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH8))
                {
                    configModel.SelectedIndividualChannels.Add(CH8);
                    configModel.IsCh8Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh8Visible = false;


            if (!(CH9 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH9))
                {
                    configModel.SelectedIndividualChannels.Add(CH9);
                    configModel.IsCh9Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh9Visible = false;



            if (!(CH10 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH10))
                {
                    configModel.SelectedIndividualChannels.Add(CH10);
                    configModel.IsCh10Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh10Visible = false;



            if (!(CH11 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH11))
                {
                    configModel.SelectedIndividualChannels.Add(CH11);
                    configModel.IsCh11Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh11Visible = false;


            if (!(CH12 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH12))
                {
                    configModel.SelectedIndividualChannels.Add(CH12);
                    configModel.IsCh12Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh12Visible = false;


            if (!(CH13 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH13))
                {
                    configModel.SelectedIndividualChannels.Add(CH13);
                    configModel.IsCh13Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh13Visible = false;



            if (!(CH14 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH14))
                {
                    configModel.SelectedIndividualChannels.Add(CH14);
                    configModel.IsCh14Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh14Visible = false;


            if (!(CH15 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH15))
                {
                    configModel.SelectedIndividualChannels.Add(CH15);
                    configModel.IsCh15Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh15Visible = false;



            if (!(CH16 == eChannles.None))
            {

                if (!configModel.SelectedIndividualChannels.Contains(CH16))
                {
                    configModel.SelectedIndividualChannels.Add(CH16);
                    configModel.IsCh16Visible = true;
                }

                else
                    return eResponseFlag.Duplicate_Channel;

            }
            else
                configModel.IsCh16Visible = false;



            return eResponseFlag.Success;
        }







        public eResponseFlag ConfigureTimingMode(eGrouping group = eGrouping.Group1, eChannles CH1 = eChannles.None, eChannles CH2 = eChannles.None, eChannles CH3 = eChannles.None, eChannles CH4 = eChannles.None, eChannles CH5 = eChannles.None, eChannles CH6 = eChannles.None, eChannles CH7 = eChannles.None, eChannles CH8 = eChannles.None, eChannles CH9 = eChannles.None, eChannles CH10 = eChannles.None, eChannles CH11 = eChannles.None, eChannles CH12 = eChannles.None, eChannles CH13 = eChannles.None,
            eChannles CH14 = eChannles.None, eChannles CH15 = eChannles.None, eChannles CH16 = eChannles.None)
        {
            configModel.ConfigurationMode = eConfigMode.LA_Mode;
            configModel.GeneralPurposeMode = eGeneralPurpose.Timing;
            configModel.GroupType = eGroupType.Group;
            if (group == eGrouping.Group1)
            {

                if (!(CH1 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP1.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;

                }
                else
                    configModel.IsCh1Visible = false;


                if (!(CH2 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH4))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP1.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH7))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh8Visible = false;

                if (!(CH8 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP1.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else

                    configModel.IsCh9Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh10Visible = false;

                if (!(CH11 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP1.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;

                }
                else
                    configModel.IsCh11Visible = false;


                if (!(CH12 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh12Visible = false;

                if (!(CH13 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh13Visible = false;

                if (!(CH14 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH14))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh14Visible = false;

                if (!(CH15 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP1.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh15Visible = false;

                if (!(CH16 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP1.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP1.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh16Visible = false;
            }
            else if (group == eGrouping.Group2)
            {
                if (!(CH1 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh1Visible = false;

                if (!(CH2 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh3Visible = false;

                if (!(CH4 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP2.Contains(CH4))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH4);
                        configModel.IsCh4Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh4Visible = false;

                if (!(CH5 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH7))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh7Visible = false;

                if (!(CH8 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh8Visible = false;

                if (!(CH9 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh9Visible = false;

                if (!(CH10 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh10Visible = false;

                if (!(CH11 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh11Visible = false;

                if (!(CH12 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh12Visible = false;

                if (!(CH13 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh13Visible = false;

                if (!(CH14 == eChannles.None))
                {

                    if (!configModel.SelectedCLK1_GRP2.Contains(CH14))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH14);
                        configModel.IsCh14Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh14Visible = false;

                if (!(CH15 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH15);
                        configModel.IsCh15Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh15Visible = false;

                if (!(CH16 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP2.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP2.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh16Visible = false;
            }
            else if (group == eGrouping.Group3)
            {
                if (!(CH1 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH1))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH1);
                        configModel.IsCh1Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh1Visible = false;


                if (!(CH2 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH2))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH2);
                        configModel.IsCh2Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh2Visible = false;

                if (!(CH3 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH3))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH3);
                        configModel.IsCh3Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh3Visible = false;
                if (!(CH4 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH4))
                    {

                        configModel.SelectedCLK1_GRP3.Add(CH4);
                        configModel.IsCh4Visible = true;

                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh4Visible = false;


                if (!(CH5 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH5))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH5);
                        configModel.IsCh5Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh5Visible = false;

                if (!(CH6 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH6))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH6);
                        configModel.IsCh6Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh6Visible = false;

                if (!(CH7 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH7))
                    {

                        configModel.SelectedCLK1_GRP3.Add(CH7);
                        configModel.IsCh7Visible = true;
                    }


                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh7Visible = false;
                if (!(CH8 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH8))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH8);
                        configModel.IsCh8Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh8Visible = false;
                if (!(CH9 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH9))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH9);
                        configModel.IsCh9Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh9Visible = false;
                if (!(CH10 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH10))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH10);
                        configModel.IsCh10Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh10Visible = false;

                if (!(CH11 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH11))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH11);
                        configModel.IsCh11Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh11Visible = false;


                if (!(CH12 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH12))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH12);
                        configModel.IsCh12Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh12Visible = false;

                if (!(CH13 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH13))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH13);
                        configModel.IsCh13Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh13Visible = false;
                if (!(CH14 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH14))
                    {

                        configModel.SelectedCLK1_GRP3.Add(CH14);
                        configModel.IsCh14Visible = true;

                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh14Visible = false;


                if (!(CH15 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH15))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH5);
                        configModel.IsCh15Visible = true;
                    }

                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh15Visible = false;

                if (!(CH16 == eChannles.None))
                {
                    if (!configModel.SelectedCLK1_GRP3.Contains(CH16))
                    {
                        configModel.SelectedCLK1_GRP3.Add(CH16);
                        configModel.IsCh16Visible = true;
                    }
                    else
                        return eResponseFlag.Duplicate_Channel;
                }
                else
                    configModel.IsCh16Visible = false;
            }
            return eResponseFlag.Success;
        }
        public eResponseFlag ConfigureSPI(eChannles SPI_Clock, eChannles SPI_CS, eChannles SPI_MOSI, eChannles SPI_MISO, eSPIPolarity polarity = eSPIPolarity.Low, eSPIPhase phase = eSPIPhase.Low)
        {
           // configModel.IsSPISelected = true;
           // configModel.SelectedSPI_CLK = SPI_Clock;
          //  configModel.SelectedSPI_CS = SPI_CS;
           // configModel.SelectedSPI_MOSI = SPI_MOSI;
           // configModel.SelectedSPI_MISO = SPI_MISO;
            //if (configModel.ConfigurationMode == eConfigMode.PA_Mode)
            //{
            //    if (polarity == eSPIPolarity.High)
            //        //configModel.ClockPolarityHighPA = true;
            //    else
            //        //configModel.ClockPolarityHighPA = false;
            //    if (phase == eSPIPhase.High)
            //        //configModel.ClockPhaseHighPA = true;
            //    else
            //        //configModel.ClockPhaseHighPA = false;
            //}
            //else if (configModel.ConfigurationMode == eConfigMode.Both)
            //{
            //    if (polarity == eSPIPolarity.High)
            //        configModel.ClockPolarityHighLAPA = true;
            //    else
            //        configModel.ClockPolarityHighLAPA = false;
            //    if (phase == eSPIPhase.High)
            //        configModel.ClockPhaseHighLAPA = true;
            //    else
            //        configModel.ClockPhaseHighLAPA = false;
            //}
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureUART(eChannles uART_Tx, eChannles uART_Rx = eChannles.None, int baudRate = 9600)
        {
          //  configModel.IsUARTSelected = true;
           // configModel.SelectedUART_TX = uART_Tx;
           // configModel.SelectedUART_RX = uART_Rx;
            //configModel.BaudRate = baudRate;
            return eResponseFlag.Success;
        }
        public eResponseFlag ConfigureI2C(eChannles I2C_SCL, eChannles I2C_SDA)
        {
            //configModel.IsI2CSelected = true;
           // configModel.SelectedI2C_SCL = I2C_SCL;
           // configModel.SelectedI2C_SDA = I2C_SDA;
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureI3C(eChannles I3C_SCL, eChannles I3C_SDA)
        {
           // configModel.IsI3CSelected = true;
           // configModel.SelectedI3C_SCL = I3C_SCL;
          //  configModel.SelectedI3C_SDA = I3C_SDA;
            return eResponseFlag.Success;
        }
        public eResponseFlag ConfigureCAN(eCANType cANType = eCANType.CAN, eChannles Can_CH = eChannles.CH1, int baudRate = 125000, bool ISBRS = false, int BRSbaudrate = 2000000)
        {
           //// configModel.IsCANSelected = true;
           //// configModel.SelectedCAN_RX = eChannles.CH1;
           // if (cANType == eCANType.CAN)
           // {
           //     configModel.CANPA = true;
           //     //configModel.BaudRateCAN = baudRate;
           // }
           // else if (cANType == eCANType.CANFD)
           // {
           //     configModel.CANFDPA = true;
           //     //configModel.BaudRateCANFD = baudRate;
           //     if (ISBRS == true)
           //     {
           //         configModel.BRSselected = true;
           //        // configModel.BaudRateCANFDBRS = BRSbaudrate;
           //     }
           //     else
           //     {
           //         configModel.BRSselected = false;
           //     }
           // }

            return eResponseFlag.Success;
        }
        public eResponseFlag ConfigureSPMI(eChannles SPMI_CLK, eChannles SPMI_DATA)
        {
            //configModel.IsSPMISelected = true;
            //configModel.SelectedSPMI_SCL = SPMI_CLK;
            //configModel.SelectedSPMI_SDA = SPMI_DATA;
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureRFFE(eChannles RFFE_CLK, eChannles RFFE_DATA)
        {
            //configModel.IsRFFESelected = true;
           // configModel.SelectedRFFE_SCL = RFFE_CLK;
            //configModel.SelectedRFFE_SDA = RFFE_DATA;
            return eResponseFlag.Success;
        }

        public eResponseFlag ConfigureThresholdVoltage(eProbeType probeType, eVoltage CH1_CH2, eVoltage CH3_CH4, eVoltage CH5_CH6, eVoltage CH7_CH8, eVoltage CH9_CH10, eVoltage CH11_CH12, eVoltage CH13_CH14, eVoltage CH15_CH16)
        {
            configModel.ProbeType = probeType;
            configModel.SignalAmpCh1_2 = CH1_CH2;
            configModel.SignalAmpCh3_4 = CH3_CH4;
            configModel.SignalAmpCh5_6 = CH5_CH6;
            configModel.SignalAmpCh7_8 = CH7_CH8;
            configModel.SignalAmpCh9_10 = CH9_CH10;
            configModel.SignalAmpCh11_12 = CH11_CH12;
            configModel.SignalAmpCh13_14 = CH13_CH14;
            configModel.SignalAmpCh15_16 = CH15_CH16;
            return eResponseFlag.Success;
        }

        public eResponseFlag Initialize_Online(ConfigModel config)
        {
            configModel = config;

            try
            {
                if (prodigyService == null)
                    prodigyService = new OnlineService(Ioc.Default.GetService<IApiService>());

                if (prodigyService.Initialize(config))
                    Ioc.Default.GetService<IDataProvider>().Initialize(config);
                else
                    return eResponseFlag.Connection_Fail;
            }
            catch
            {
                return eResponseFlag.Fail;
            }

            return eResponseFlag.Success;
        }

        public void Reset()
        {
            Ioc.Default.GetService<IDataProvider>().Reset();
        }

        public eResponseFlag Initialize_Offline(ConfigModel config, string traceFilePath)
        {
            configModel = config;

            if (prodigyService == null)
            prodigyService = new OfflineService(Ioc.Default.GetService<IApiService>());

            if (!SessionConfiguration.IsDecodeActive)
            {
                try
                {
                    SessionConfiguration.TraceFilePath = @traceFilePath;
                    SessionConfiguration.SourceType = eSourceType.Offline;

                    if (prodigyService.Initialize(config))
                        Ioc.Default.GetService<IDataProvider>().Initialize(config);
                    else
                        return eResponseFlag.Connection_Fail;
                }
                catch
                {
                    return eResponseFlag.Fail;
                }

                return eResponseFlag.Success;
            }
            else
            {
                return eResponseFlag.Task_Busy;
            }
        }

        public void StartRun()
        {
            prodigyService.StartRun();
        }

        public void StopRun()
        {
            prodigyService.StopRun();
        }

        public eResponseFlag Savetracefile()
        {

            //if (!string.IsNullOrEmpty(SessionConfiguration.TraceFilePath))
            //{
            string filePath = SessionConfiguration.TraceFilePath;
            filePath += ".xml";
            configModel.TriggerType = TriggerModel.GetInstance().TriggerType;
            configModel.ProtocolTypeIndex = TriggerModel.GetInstance().ProtocolTypeIndex;
            configModel.TimingTriggerTypeSelected = TriggerModel.GetInstance().TimingTriggerTypeSelected;
            SerializeObject<ConfigModel>.SerializeData(configModel, filePath);
            return eResponseFlag.Success;
            //}
            //else
            //{
            ////    return eResponseFlag.Fail;
            //}
        }


        public eResponseFlag SaveDefaultConfiguration()
        {

            string fileName = MiniCooperDirectoryInfo.DefaultSettingFile;
            configModel.TriggerType = TriggerModel.GetInstance().TriggerType;
            configModel.ProtocolTypeIndex = TriggerModel.GetInstance().ProtocolTypeIndex;
            configModel.TimingTriggerTypeSelected = TriggerModel.GetInstance().TimingTriggerTypeSelected;
            SerializeObject<ConfigModel>.SerializeData(configModel, fileName);
            return eResponseFlag.Success;

        }


        public eResponseFlag SaveAsTraceFileOption(string path)
        {

            if (!string.IsNullOrEmpty(SessionConfiguration.TraceFilePath))
            {
                //Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                // saveDialog.DefaultExt = ".xml";
                //saveDialog.Filter = "XML Configuration (.xml)|*.xml";
                //saveDialog.RestoreDirectory = true;
                string fileName = path;
                // saveDialog.ShowDialog();
                //if (!string.IsNullOrEmpty(saveDialog.FileName))
                //{
                Task.Factory.StartNew(() =>
                {
                    // fileName = saveDialog.FileName;
                    // fileName = path;
                    string folderPath = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName);
                    var source = new DirectoryInfo(SessionConfiguration.TraceFilePath);
                    var target = new DirectoryInfo(folderPath);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    foreach (FileInfo files in source.GetFiles())
                    {
                        files.CopyTo(Path.Combine(target.FullName, files.Name), true);
                    }
                }).Wait();
                configModel.TriggerType = TriggerModel.GetInstance().TriggerType;
                configModel.ProtocolTypeIndex = TriggerModel.GetInstance().ProtocolTypeIndex;
                configModel.TimingTriggerTypeSelected = TriggerModel.GetInstance().TimingTriggerTypeSelected;
                SerializeObject<ConfigModel>.SerializeData(configModel, fileName);


                //}
                return eResponseFlag.Success;
            }


            else
            {
                return eResponseFlag.Fail;

            }

        }

        public eResponseFlag SaveAsConfiguration(string path)
        {

            //Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            //saveDialog.DefaultExt = ".xml";
            //saveDialog.Filter = "XML Configuration (.xml)|*.xml";
            //saveDialog.RestoreDirectory = true;
            string fileName = path;
            //saveDialog.ShowDialog();
            //if (!string.IsNullOrEmpty(saveDialog.FileName))
            //{
            //fileName = saveDialog.FileName;
            configModel.TriggerType = TriggerModel.GetInstance().TriggerType;
            configModel.ProtocolTypeIndex = TriggerModel.GetInstance().ProtocolTypeIndex;
            configModel.TimingTriggerTypeSelected = TriggerModel.GetInstance().TimingTriggerTypeSelected;
            SerializeObject<ConfigModel>.SerializeData(configModel, fileName);
            return eResponseFlag.Success;
            //}
        }
        //public eResponseFlag GenerateReportCSV(string path, int startIndex = -1, int stopIndex = -1)
        //{

        //    return report.GenerateReportCSV(path);
        //   // return eResponseFlag.Success;

        //}

        //public eResponseFlag GenerateReportPDF(string path, int startIndex = -1, int stopIndex = -1)
        //{

        //    return report.GenerateReportPDF(path);
        //    // return eResponseFlag.Success;

        //}

        #region trigger

        public eResponseFlag GetTriggerType(eTriggerTypeList typeList)
        {


            TriggerModel.GetInstance().TriggerType = typeList;
            return eResponseFlag.Success;
        }


        public eResponseFlag GetPattern(ePatternFormat ePattern, string value)
        {
            //var config = ConfigModel.GetInstance();
            TriggerModel.GetInstance().PatternFormat = ePattern;
            //config.ConfigurationMode = eConfigMode.LA_Mode;
            //config.GeneralPurposeMode = eGeneralPurpose.State;
            TriggerModel.GetInstance().PatternText = value;
            return eResponseFlag.Success;

        }


        //public static eResponseFlag GetProtocolAware(eProtocolTypeList protocolType)
        //{
        //    var config = ConfigModel.GetInstance();
        //    TriggerModel.GetInstance().ProtocolTypeIndex = protocolType;
        //    return eResponseFlag.Success;
        //}
        public  eResponseFlag I2Ctriggerselection(eI2CTriggerAtList triggerAtList)
        {
            TriggerModel.GetInstance().I2CTriggerAtSelected = triggerAtList;
            return eResponseFlag.Success;
        }



        public eResponseFlag AddressTriggerI2C(eComparisonList comparlist, ePatternFormat patternFormat, string AddValue, eTransferType transferType)
        {

            TriggerModel.GetInstance().AddressComparison = comparlist;
            TriggerModel.GetInstance().AddressPattern = patternFormat;
           // TriggerModel.GetInstance().AddressValue = AddValue;
            TriggerModel.GetInstance().TransferType = transferType;

            return eResponseFlag.Success;
        }


        public eResponseFlag DataTriggerI2C(eComparisonList comparisonList, ePatternFormat patternFormat, string datavalue)
        {

            //TriggerModel.GetInstance().DataComparison = comparisonList;
            //TriggerModel.GetInstance().DataPattern = patternFormat;
            //TriggerModel.GetInstance().DataValue = datavalue;

            return eResponseFlag.Success;
        }

        public eResponseFlag MosiSPITrigger(eComparisonList comparisonList, ePatternFormat patternFormat, String MosiData)

        {
            if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
            {
                TriggerModel.GetInstance().IsMOSIChecked = true;
                //  TriggerModel.GetInstance().data

                TriggerModel.GetInstance().MOSIComparison = comparisonList;
                TriggerModel.GetInstance().MOSIPattern = patternFormat;
                TriggerModel.GetInstance().MOSIData = MosiData;


            }
            return eResponseFlag.Success;
        }

        public eResponseFlag MisoSPITrigger(eComparisonList comparisonList, ePatternFormat patternFormat, String MisoData)

        {
            if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
            {
                TriggerModel.GetInstance().IsMISOChecked = true;

                if (TriggerModel.GetInstance().IsMISOChecked == true)
                {
                    TriggerModel.GetInstance().MISOComparison = comparisonList;
                    TriggerModel.GetInstance().MISOPattern = patternFormat;
                    TriggerModel.GetInstance().MISOData = MisoData;
                }

            }
            return eResponseFlag.Success;
        }


        public eResponseFlag UartTrigger(eComparisonList comparisonList, ePatternFormat patternFormat, string UartData, bool fall = true, bool raise = true, bool isdata = true)
        {
            TriggerModel.GetInstance().UARTStartFallingSelected = fall;
            TriggerModel.GetInstance().UARTStartRisingSelected = raise;

            TriggerModel.GetInstance().IsUARTDataChecked = isdata;
            if (isdata == true)
            {
                TriggerModel.GetInstance().UARTDataComparison = comparisonList;
                TriggerModel.GetInstance().UARTDataPattern = patternFormat;
                TriggerModel.GetInstance().UARTDataValue = UartData;

            }
            return eResponseFlag.Success;
        }


        public eResponseFlag RaiseUartTrigger(eComparisonList comparisonList, ePatternFormat patternFormat, string UartData)
        {
            TriggerModel.GetInstance().UARTStartFallingSelected = true;
            // TriggerModel.GetInstance().UARTStartRisingSelected = raise;

            TriggerModel.GetInstance().IsUARTDataChecked = true;
            if (TriggerModel.GetInstance().IsUARTDataChecked == true)
            {
                TriggerModel.GetInstance().UARTDataComparison = comparisonList;
                TriggerModel.GetInstance().UARTDataPattern = patternFormat;
                TriggerModel.GetInstance().UARTDataValue = UartData;

            }
            return eResponseFlag.Success;
        }
        public eResponseFlag FallUartTrigger(eComparisonList comparisonList, ePatternFormat patternFormat, string UartData)
        {
            // TriggerModel.GetInstance().UARTStartFallingSelected = true;
            TriggerModel.GetInstance().UARTStartRisingSelected = true;

            TriggerModel.GetInstance().IsUARTDataChecked = true;
            if (TriggerModel.GetInstance().IsUARTDataChecked == true)
            {
                TriggerModel.GetInstance().UARTDataComparison = comparisonList;
                TriggerModel.GetInstance().UARTDataPattern = patternFormat;
                TriggerModel.GetInstance().UARTDataValue = UartData;

            }
            return eResponseFlag.Success;
        }
        public eResponseFlag SPMITrigger(eSPMICMDTYPE sPMICMDTYPE, String SlaveAddress, string ByteCount, string regadd, string datavalue, eAcknowledgeType acknowledgeType)
        {

            TriggerModel.GetInstance().SelSPMICommand = sPMICMDTYPE;
            if (sPMICMDTYPE == eSPMICMDTYPE.EXT_REG_WRITE || sPMICMDTYPE == eSPMICMDTYPE.EXT_REG_READ || sPMICMDTYPE == eSPMICMDTYPE.EXT_REG_READ_LONG || sPMICMDTYPE == eSPMICMDTYPE.EXT_REG_WRITE_LONG)

            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().SPMIBytecount = ByteCount;
                TriggerModel.GetInstance().SPMIregAddr = regadd;
                TriggerModel.GetInstance().SPMIData = datavalue;
            }
            else
                if (sPMICMDTYPE == eSPMICMDTYPE.RESET || sPMICMDTYPE == eSPMICMDTYPE.SLEEP || sPMICMDTYPE == eSPMICMDTYPE.SHUTDOWN || sPMICMDTYPE == eSPMICMDTYPE.WAKEUP)
            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
                //TriggerModel.GetInstance().AckNckI3C = acknowledgeType;
            }
            else
                if (sPMICMDTYPE == eSPMICMDTYPE.AUTHENTICATE || sPMICMDTYPE == eSPMICMDTYPE.DEFAULT)
            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
            }
            else
                if (sPMICMDTYPE == eSPMICMDTYPE.MASTER_READ || sPMICMDTYPE == eSPMICMDTYPE.MASTER_WRITE || sPMICMDTYPE == eSPMICMDTYPE.DDB_MA_R || sPMICMDTYPE == eSPMICMDTYPE.DDB_SL_R || sPMICMDTYPE == eSPMICMDTYPE.EXT_REG_RESERVED || sPMICMDTYPE == eSPMICMDTYPE.REG_READ || sPMICMDTYPE == eSPMICMDTYPE.NO_RESPONSE)
            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().SPMIregAddr = regadd;
                TriggerModel.GetInstance().SPMIData = datavalue;
            }

            else
                if (sPMICMDTYPE == eSPMICMDTYPE.REG_WRITE)
            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().SPMIregAddr = regadd;
                TriggerModel.GetInstance().SPMIData = datavalue;
                //TriggerModel.GetInstance().AckNckI3C = acknowledgeType;
            }
            else
                if (sPMICMDTYPE == eSPMICMDTYPE.REG_ZERO_WRITE)
            {
                TriggerModel.GetInstance().SPMISlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().SPMIData = datavalue;
              //  TriggerModel.GetInstance().AckNckI3C = acknowledgeType;
            }

            return eResponseFlag.Success;
        }


        public eResponseFlag RFFETrigger(eRFFECMDTYPE RFFECMDTYPE, String SlaveAddress, string ByteCount, string regadd, string datavalue, eAcknowledgeType acknowledgeType, string DataMask, int RX_MID, eInterruptSlot interruptSlot)
        {

            TriggerModel.GetInstance().SelRFFECommand = RFFECMDTYPE;
            if (RFFECMDTYPE == eRFFECMDTYPE.EXT_REG_WRITE || RFFECMDTYPE == eRFFECMDTYPE.EXT_REG_READ || RFFECMDTYPE == eRFFECMDTYPE.EXT_REG_READ_LONG || RFFECMDTYPE == eRFFECMDTYPE.EXT_REG_WRITE_LONG || RFFECMDTYPE == eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ || RFFECMDTYPE == eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE)

            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFEBytecount = ByteCount;
                TriggerModel.GetInstance().RFFEregAddr = regadd;
                TriggerModel.GetInstance().RFFEData = datavalue;
            }
            else
                    if (RFFECMDTYPE == eRFFECMDTYPE.REG_WRITE || RFFECMDTYPE == eRFFECMDTYPE.REG_READ || RFFECMDTYPE == eRFFECMDTYPE.MASTER_READ || RFFECMDTYPE == eRFFECMDTYPE.MASTER_WRITE || RFFECMDTYPE == eRFFECMDTYPE.EXT_REG_RESERVED || RFFECMDTYPE == eRFFECMDTYPE.TRIGGER || RFFECMDTYPE == eRFFECMDTYPE.None)
            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFEregAddr = regadd;
                TriggerModel.GetInstance().RFFEData = datavalue;
            }
            else
                if (RFFECMDTYPE == eRFFECMDTYPE.REG_ZERO_WRITE)
            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFEData = datavalue;
            }
            else
                if (RFFECMDTYPE == eRFFECMDTYPE.MASKED_WRITE)
            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFEregAddr = regadd;
                TriggerModel.GetInstance().RFFEData = datavalue;
                TriggerModel.GetInstance().RFFEMaskData = DataMask;
            }
            else
                if (RFFECMDTYPE == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFESlaveAck = acknowledgeType;
                TriggerModel.GetInstance().RFFERxMID = RX_MID;

            }
            else
                if (RFFECMDTYPE == eRFFECMDTYPE.INT_SUMMARY_IDENT)
            {
                TriggerModel.GetInstance().RFFESlaveAddress = SlaveAddress;
                TriggerModel.GetInstance().RFFEISIEnable = true;
                if (TriggerModel.GetInstance().RFFEISIEnable == true)
                {
                    TriggerModel.GetInstance().RFFEInterrupt = interruptSlot;
                }
            }
            return eResponseFlag.Success;
        }

        public eResponseFlag CANTrigger( String CANID, string IDE, string dlc, string datavalue)
        {

            //TriggerModel.GetInstance().CANID = CANID;
            //TriggerModel.GetInstance().CANEXID = IDE;
            //TriggerModel.GetInstance().CANDLC = dlc;
            //TriggerModel.GetInstance().CANData = datavalue;
            return eResponseFlag.Success;
        }
        public eResponseFlag I3CBroadcasteTrigger(eI3CMessage i3CMessage, eAcknowledgeType acknowledgeType, eBroadcastCCC broadcastCCC, String Datavalue, string hdrcommand, string hdrData)
        {
            //TriggerModel.GetInstance().SelI3CMessage = i3CMessage;



            //if (i3CMessage == eI3CMessage.Broadcast)
            //{

            //    TriggerModel.GetInstance().AckNckI3C = acknowledgeType;
            //    if (acknowledgeType == eAcknowledgeType.ACK)
            //    {
            //        TriggerModel.GetInstance().SelCommandBrd = broadcastCCC;
            //        if (broadcastCCC == eBroadcastCCC.ENEC || broadcastCCC == eBroadcastCCC.DISEC || broadcastCCC == eBroadcastCCC.ENTDAA || broadcastCCC == eBroadcastCCC.DEFSLVS || broadcastCCC == eBroadcastCCC.SETMWL || broadcastCCC == eBroadcastCCC.SETMRL || broadcastCCC == eBroadcastCCC.ENTTM || broadcastCCC == eBroadcastCCC.SETXTIME || broadcastCCC == eBroadcastCCC.RSTACT || broadcastCCC == eBroadcastCCC.SETHID || broadcastCCC == eBroadcastCCC.DEVCTRL)
            //        {
            //            TriggerModel.GetInstance().I3CData = Datavalue;
            //        }
            //        else
            //            if (broadcastCCC == eBroadcastCCC.ENTHDR0 || broadcastCCC == eBroadcastCCC.ENTHDR1 || broadcastCCC == eBroadcastCCC.ENTHDR2 || broadcastCCC == eBroadcastCCC.ENTHDR3 || broadcastCCC == eBroadcastCCC.ENTHDR4 || broadcastCCC == eBroadcastCCC.ENTHDR5 || broadcastCCC == eBroadcastCCC.ENTHDR6 || broadcastCCC == eBroadcastCCC.ENTHDR7)
            //        {
            //            TriggerModel.GetInstance().HdrCommand = hdrcommand;
            //            TriggerModel.GetInstance().HdrData = hdrData;
            //        }
            //    }
            //}
            return eResponseFlag.Success;
        }

        public eResponseFlag I3CDirectedTrigger(eI3CMessage i3CMessage, eAcknowledgeType acknowledgeType, eDirectedCCC directedCCC, string slaveAdd, eAcknowledgeType acknowledgeType1, string datavalue)
        {
            //TriggerModel.GetInstance().SelI3CMessage = i3CMessage;
            //if (i3CMessage == eI3CMessage.Directed)
            //{
            //    TriggerModel.GetInstance().AckNckI3C = acknowledgeType;
            //    if (acknowledgeType == eAcknowledgeType.ACK)
            //    {
            //        TriggerModel.GetInstance().SelCommandDir = directedCCC;
            //        if (directedCCC == eDirectedCCC.ENEC || directedCCC == eDirectedCCC.DISEC || directedCCC == eDirectedCCC.ENTAS0 || directedCCC == eDirectedCCC.ENTAS1 || directedCCC == eDirectedCCC.ENTAS2 || directedCCC == eDirectedCCC.ENTAS3 || directedCCC == eDirectedCCC.RSTDAA || directedCCC == eDirectedCCC.SETDASA || directedCCC == eDirectedCCC.SETNEWDA || directedCCC == eDirectedCCC.SETMWL || directedCCC == eDirectedCCC.SETMRL || directedCCC == eDirectedCCC.ENDXFER || directedCCC == eDirectedCCC.SETBRGTGT || directedCCC == eDirectedCCC.SETROUTE || directedCCC == eDirectedCCC.D2DXFER || directedCCC == eDirectedCCC.SETXTIME || directedCCC == eDirectedCCC.RSTACT || directedCCC == eDirectedCCC.SETGRPA || directedCCC == eDirectedCCC.RSTGRPA)

            //        {
            //            TriggerModel.GetInstance().I3CSlaveAddress = slaveAdd;
            //            TriggerModel.GetInstance().I3CSlaveTransfer = eTransferType.WR;
            //            TriggerModel.GetInstance().I3CSlaveAck = acknowledgeType1;
            //            if (acknowledgeType1 == eAcknowledgeType.ACK)
            //            {
            //                TriggerModel.GetInstance().I3CData = datavalue;
            //            }

            //        }
            //        else
            //            if (directedCCC == eDirectedCCC.GETMWL || directedCCC == eDirectedCCC.GETMRL || directedCCC == eDirectedCCC.GETPID || directedCCC == eDirectedCCC.GETBCR || directedCCC == eDirectedCCC.GETDCR || directedCCC == eDirectedCCC.GETSTATUS || directedCCC == eDirectedCCC.GETACCMST || directedCCC == eDirectedCCC.GETMXDS || directedCCC == eDirectedCCC.GETCAPS || directedCCC == eDirectedCCC.GETXTIME || directedCCC == eDirectedCCC.DEVCAP)
            //        {
            //            TriggerModel.GetInstance().I3CSlaveAddress = slaveAdd;
            //            TriggerModel.GetInstance().I3CSlaveTransfer = eTransferType.RD;
            //            TriggerModel.GetInstance().I3CSlaveAck = acknowledgeType1;
            //            if (acknowledgeType1 == eAcknowledgeType.ACK)
            //            {
            //                TriggerModel.GetInstance().I3CData = datavalue;
            //            }

            //        }
            //    }
            //}
            return eResponseFlag.Success;
        }

        public eResponseFlag I3CPrivateTrigger(eI3CMessage i3CMessage, string slaveAdd, eTransferType typeList, eAcknowledgeType acknowledgeType, string datavalue)
        {
            //TriggerModel.GetInstance().SelI3CMessage = i3CMessage;
            //if (i3CMessage == eI3CMessage.Private)
            //{
            //    TriggerModel.GetInstance().I3CSlaveAddress = slaveAdd;
            //    TriggerModel.GetInstance().I3CSlaveTransfer = typeList;
            //    TriggerModel.GetInstance().I3CSlaveAck = acknowledgeType;
            //    if (acknowledgeType == eAcknowledgeType.ACK)
            //    {
            //        TriggerModel.GetInstance().I3CData = datavalue;
            //    }
            //}
            return eResponseFlag.Success;
        }


        public eResponseFlag TimingPulsewidthPositive(eTimingTriggerTypeList triggerTypeList, eChannelList channelList, ePulseComparisonList pulseComparisonList, string widthcount)
        {
            TriggerModel.GetInstance().TimingTriggerTypeSelected = triggerTypeList;


            TriggerModel.GetInstance().TimingPulsePositiveSelected = true;

            // TriggerModel.GetInstance().TimingPulseNegativeSelected = negative;

            TriggerModel.GetInstance().PulseWidthChannel = channelList;
            TriggerModel.GetInstance().PulseComparisonSelected = pulseComparisonList;
            TriggerModel.GetInstance().PulseWidthCount = widthcount;




            return eResponseFlag.Success;
        }


        public eResponseFlag TimingPulsewidthNegative(eTimingTriggerTypeList triggerTypeList, eChannelList channelList, ePulseComparisonList pulseComparisonList, string widthcount)
        {
            TriggerModel.GetInstance().TimingTriggerTypeSelected = triggerTypeList;


            // TriggerModel.GetInstance().TimingPulsePositiveSelected = true;

            TriggerModel.GetInstance().TimingPulseNegativeSelected = true;

            TriggerModel.GetInstance().PulseWidthChannel = channelList;
            TriggerModel.GetInstance().PulseComparisonSelected = pulseComparisonList;
            TriggerModel.GetInstance().PulseWidthCount = widthcount;




            return eResponseFlag.Success;
        }
        public eResponseFlag TimingDelayWidth(eTimingTriggerTypeList triggerTypeList, eChannelList channelList, eChannelList channelList1, ePulseComparisonList comparisonList, String delaycount)


        {
            TriggerModel.GetInstance().TimingTriggerTypeSelected = triggerTypeList;
            if (triggerTypeList == eTimingTriggerTypeList.Delay)
            {


                TriggerModel.GetInstance().DelayChannel1 = channelList;
                TriggerModel.GetInstance().DelayChannel2 = channelList1;
                TriggerModel.GetInstance().DelayComparisonSelected = comparisonList;
                TriggerModel.GetInstance().DelayCount = delaycount;


            }

            return eResponseFlag.Success;
        }
        #endregion




        public string GetResponseDescription(eResponseFlag response)
        {
            System.Reflection.FieldInfo fi = response.GetType().GetField(response.ToString());
            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
                else
                    return response.ToString();
            }
            else
                return response.ToString();
        }

        void CloseCaptureApp()
        {
            Process[] GetPArry = Process.GetProcesses();
            foreach (Process process in GetPArry)
            {
                string ProcessName = process.ProcessName;

                ProcessName = ProcessName.ToLower().Trim();
                if (ProcessName.CompareTo("prodigycapturetrayapplication") == 0)
                {
                    process.Kill();
                    break;
                }
            }
        }

        public void Dispose()
        {
            CloseCaptureApp();
        }

    }
}
