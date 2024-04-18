using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Enums
{
    public enum TypeOfMeasurement
    {
        //Amplitude measurement 0 - 20
        [Description("Amplitude")]
        AMPLITUDE = 0,
        [Description("High")]
        HIGH,
        [Description("Low")]
        LOW,
        [Description("RMS")]
        RMS,
        [Description("Max")]
        MAXIMUM,
        [Description("Min")]
        MINIMUM,
        [Description("Pk to Pk")]
        PK2PK,
        [Description("+Overshoot")]
        POVERSHOOT,
        [Description("-Overshoot")]
        NOVERSHOOT,

        //Time measurement 21-40
        RISE = 21,
        FALL,
        PWIDTH,
        NWIDTH,
        PDUTY,
        NDUTY,
        PERIOD,
        FREQUENCY,
        //Trend plot Measurements Parameter 41-60
        RISE_TIME_TREND = 41,
        FALL_TIME_TREND,
        AMPLITUDE_TREND,
        POSITIVE_OVERSHOOT_TREND,
        NEGATIVE_OVERSHOOT_TREND,
        MAXIMUM_TREND,
        MINIMUM_TREND,
        RMS_TREND,
        POSITIVE_WIDTH_TREND,
        NEGATIVE_WIDTH_TREND,
        PERIOD_TREND,
        POSITIVE_DUTY_CYCLE_TREND,
        NEGATIVE_DUTY_CYCLE_TREND,
        FREQUENCY_TREND
    }
}
