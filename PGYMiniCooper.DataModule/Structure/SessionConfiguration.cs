using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PGYMiniCooper.DataModule.Structure
{
    [Serializable]
    public sealed class SessionConfiguration
    {
        public static eAnalyzerMode AnalyzerMode = eAnalyzerMode.I3C;
        public static bool SupportI2C;
        [NonSerialized]
        public static eSourceType SourceType;
        [NonSerialized]
        public static bool ConnectionStatus;
        [NonSerialized]
        public static eBoard PCBboard;
        [XmlIgnore]
        public static bool StopCapture;
        [XmlIgnore]
        public static bool StopDecode;
        [XmlIgnore]
        public static bool StopForcedDecode;
        [XmlIgnore]
        private static bool _IsDecocdeActive;

        public static double TriggerTime = 0;

        public static bool Triggerset = false;

        public static bool TriggerProtcolset = false;

        public static bool TriggersetTView = false;

        public static int TriggerIndex = -1;

        public static bool isBufferOverflow = false;

        public static eVersion Version = eVersion.two;

        private static int hardwareVersion;
        public static int HardwareVersion
        {
            get
            {
                return hardwareVersion;
            }
            set
            {
                hardwareVersion = value;
                if (PublishDecodeStatus != null)
                    PublishDecodeStatus(value);
            }
        }

        public static bool IsDecodeActive
        {
            get
            {
                return _IsDecocdeActive;
            }
            set
            {
                _IsDecocdeActive = value;
            }
        }

        static string traceFilePath;


        public static bool IsSaveas
        {
            get
            {
                return _IsSaveas;
            }
            set
            {
                _IsSaveas = value;
            }
        }

        static bool _IsSaveas = false;


        public static bool IsSave
        {
            get
            {
                return _IsSave;
            }
            set
            {
                _IsSave = value;
            }
        }

        static bool _IsSave = false;


        public static bool IsSaveasname
        {
            get
            {
                return _IsSavename;
            }
            set
            {
                _IsSavename = value;
            }
        }

        static bool _IsSavename = false;
        public static string TraceFilePath
        {
            get
            {
                return traceFilePath;
            }
            set
            {
                traceFilePath = value;
            }
        }

        public string DumpFilePath
        {
            get
            {
                return traceFilePath;
            }
            set
            {
                traceFilePath = value;
            }
        }
        public static Action<bool> OnAnimate;
        private static bool isAnimate;
        public static bool IsAnimate
        {
            get
            {
                return isAnimate;
            }
            set
            {
                isAnimate = value;
                if (OnAnimate != null)
                    OnAnimate(value);
            }
        }

        public delegate void OnDecodeStatusEventHandler(int isDecodeActive);
        public static event OnDecodeStatusEventHandler PublishDecodeStatus;

    }
}
