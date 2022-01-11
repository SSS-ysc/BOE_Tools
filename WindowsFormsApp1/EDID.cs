using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

enum Support
{
    unsupported,
    supported,
};
enum FormatError
{ 
    Success,
    HeaderError,
    VersionError,
    NoMainTimingError,
    ChecksumError,
};
enum EDIDversion
{
    V13,
    V14,
};
enum EDIDColorBitDepth
{
    color_undefined,
    color_6bit,
    color_8bit,
    color_10bit,
    color_12bit,
    color_14bit,
    color_16bit,
};
enum EDIDVideoStandard
{
    EDIDVideoAnalog,
    EDIDVideoDigital,
};
enum EDIDDigitalVideoStandard
{
    Digital_undefined,
    Digital_DVI,
    Digital_HDMI_a,
    Digital_HDMI_b,
    Digital_MDDI,
    Digital_DisplayPort,
};
enum ScreenSizeType
{
    ScreenSize_undefined,
    ScreenSize_HV,
    ScreenSize_Ratio,
};
enum ColorType
{
    Grayscale,
    RGB,
    NonRGB,
    ColorType_undefined,
};
enum ColorEncoding
{
    RGB444,
    RGB444YCrCb444,
    RGB444YCrCb422,
    RGB444YCrCb444YCrCb422,
};
enum StandardTimingRatio
{
    Ratio16x10,
    Ratio4x3,
    Ratio5x4,
    Ratio16x9,
};
enum InterfaceType
{    
    p_Timing,
    i_Timing,
};
enum StereoViewingType
{
    Normal,
    FieldRightimage,
    FieldLeftimage,
    TwoWayRightimage,
    TwoWayLeftimage,
    FourWay,
    SidebySide,
};
enum SyncType
{
    AnalogComposite,
    BipolarAnalogComposite,
    DigitalComposite,
    DigitalSeparate,
};
enum AnalogSyncType
{
    Undefined,
    WithoutSerrations_SyncOnGreenOnly,
    WithoutSerrations_SyncOnRGB,
    WithSerrations_SyncOnGreenOnly,
    WithSerrations_SyncOnRGB,
};
enum DigitalSyncType
{
    Undefined,
    WithoutSerrations_HSyncN,
    WithoutSerrations_HSyncP,
    WithSerrations_HSyncN,
    WithSerrations_HSyncP,
    VSyncN_HSyncN,
    VSyncN_HSyncP,
    VSyncP_HSyncN,
    VSyncP_HSyncP,
};
enum EDIDDescriptorsType
{
    Undefined,
    ProductSN,//FF
    AlphanumericData,//FE
    RangeLimits,//FD
    ProductName,//FC
    ColorData,//FB
    StandardTiming,//FA
    DCMdata,//F9
    CVT3ByteTiming,//F8
    DummyDescriptors,//11-F6
    EstablishedTiming,//F7
    ManufacturerDescriptors,//00-0F

    DetailTiming,
}
struct EDIDStandardTiming
{
    public Support TimingSupport;
    public StandardTimingRatio TimingRatio;
    public ushort TimingWidth;
    public ushort TimingHeight;
    public byte TimingRate;
};
struct EDIDFeatureSupport
{
    public Support StandbyMode;
    public Support SuspendMode;
    public Support VeryLowPowerMode;
    public ColorType DisplayColorType;
    public ColorEncoding ColorEncodingFormat;//(EDID1.4)
    public Support sRGBStandard;
    public Support PreferredTimingMode;
    public Support ContinuousFrequency;//(EDID1.4)
    public Support GTFstandard;//(EDID1.3)
};
struct EDIDBasicScreenSize
{
    public ScreenSizeType Type;
    public byte Hsize;
    public byte Vsize;
    public byte Ratio;
};
struct EDIDBasicDisplayParameters
{
    public EDIDVideoStandard Video_definition;

    public byte AnalogSignalLevelStandard; //(EDID1.3 VGA)
    public byte AnalogVideoSetup;
    public byte AnalogSyncInputsSupported;

    public EDIDColorBitDepth DigitalColorDepth; //(EDID1.4 HDMI)
    public EDIDDigitalVideoStandard DigitalStandard;

    public EDIDBasicScreenSize ScreenSize;

    public float Gamma;

    public EDIDFeatureSupport FeatureSupport;
}
struct EDIDColorCharacteristics
{
    public double RedX;
    public double RedY;
    public double GreenX;
    public double GreenY;
    public double BlueX;
    public double BlueY;
    public double WhiteX;
    public double WhiteY;
};
struct EDIDEstablishedTimings
{
    public Support Es720x400_70;
    public Support Es720x400_88;
    public Support Es640x480_60;
    public Support Es640x480_67;
    public Support Es640x480_72;
    public Support Es640x480_75;
    public Support Es800x600_56;
    public Support Es800x600_60;

    public Support Es800x600_72;
    public Support Es800x600_75;
    public Support Es832x624_75;
    public Support Es1024x768_87;
    public Support Es1024x768_60;
    public Support Es1024x768_70;
    public Support Es1024x768_75;
    public Support Es1280x1024_75;

    public Support Es1152x870_75;
};
struct EDIDDetailTimingTable
{
    public ushort PixelClk;

    public ushort HFrequency;
    public ushort HAdressable;
    public ushort HBlanking;
    public ushort HSyncFront;
    public ushort HSyncWidth;
    public byte HBorder;

    public ushort VFrequency;
    public ushort VAdressable;
    public ushort VBlanking;
    public ushort VSyncFront;
    public ushort VSyncWidth;
    public byte VBorder;

    public ushort VideoSizeH;
    public ushort VideoSizeV;

    public InterfaceType Interface;

    public StereoViewingType StereoFormat;

    public SyncType SyncType;
    public AnalogSyncType AnalogSync;
    public DigitalSyncType DigitalSync;
};
struct EDIDDisplayRangeLimits
{
};
struct EDIDTable
{
    public string IDManufacturerName;
    public uint IDProductCode;
    public string IDSerialNumber;
    public ushort Week;
    public ushort Year;
    public uint ModelYear;
    public EDIDversion Version;
    public EDIDBasicDisplayParameters Basic;
    public EDIDColorCharacteristics PanelColor;
    public EDIDEstablishedTimings EstablishedTiming;
    public EDIDStandardTiming[] StandardTiming;

    public EDIDDescriptorsType Descriptors1;
    public EDIDDescriptorsType Descriptors2;
    public EDIDDescriptorsType Descriptors3;
    public EDIDDescriptorsType Descriptors4;

    public EDIDDetailTimingTable MainTiming;
    public EDIDDetailTimingTable SecondMainTiming;
    public string SN;
    public EDIDDisplayRangeLimits Limits;
    public string Name;

    public byte ExBlockCount;
    public byte Checksum;
};
struct EDIDTable_CEA
{
    public EDIDDetailTimingTable[] CEA_Timing;
};
struct EDIDTable_DisplayID
{

};
namespace EDID_Form
{
    internal static class EDID
    {
        public static string EDIDText = "";
        static EDIDTable EDIDFormData;

        private static byte GetByteBit(byte a, byte X)
        {
            switch (X)
            {
                case 7:
                    return (byte)((a & 0x80) >> 7);
                case 6:
                    return (byte)((a & 0x40) >> 6);
                case 5:
                    return (byte)((a & 0x20) >> 5);
                case 4:
                    return (byte)((a & 0x10) >> 4);
                case 3:
                    return (byte)((a & 0x08) >> 3);
                case 2:
                    return (byte)((a & 0x04) >> 2);
                case 1:
                    return (byte)((a & 0x02) >> 1);
                case 0:
                    return (byte)((a & 0x01) >> 0);
                default:
                    return a;
            }
        }
        private static Support GetByteBitSupport(byte a, byte X)
        {
            if (((a & (0x01 << X)) >> X) == 0x01)
                return Support.supported;
            else
                return Support.unsupported;
        }
        private static double GetEDIDColorxy(uint xy)
        {
            double xyValue = 0;

            xyValue += (double)GetByteBit((byte)(xy >> 8), 1) * Math.Pow(2, -1);
            xyValue += (double)GetByteBit((byte)(xy >> 8), 0) * Math.Pow(2, -2);
            xyValue += (double)GetByteBit((byte)xy, 7) * Math.Pow(2, -3);
            xyValue += (double)GetByteBit((byte)xy, 6) * Math.Pow(2, -4);
            xyValue += (double)GetByteBit((byte)xy, 5) * Math.Pow(2, -5);
            xyValue += (double)GetByteBit((byte)xy, 4) * Math.Pow(2, -6);
            xyValue += (double)GetByteBit((byte)xy, 3) * Math.Pow(2, -7);
            xyValue += (double)GetByteBit((byte)xy, 2) * Math.Pow(2, -8);
            xyValue += (double)GetByteBit((byte)xy, 1) * Math.Pow(2, -9);
            xyValue += (double)GetByteBit((byte)xy, 0) * Math.Pow(2, -10);

            return Math.Round(xyValue, 3);
        }
        public static int MatchOriginalTextEDID(string Text)//standard format
        {
            int Length = 0;

            MatchCollection mcText1 = Regex.Matches(Text, @"\|  ([0-9]|[A-Z])([0-9]|[A-Z])  \w\w  \w\w  \w\w  \w\w  \w\w  \w\w  \w\w((  \w\w  \w\w)|\s)");

            foreach (Match m1 in mcText1)
            {
                string data = m1.ToString();
                MatchCollection mcText2 = Regex.Matches(data, @"([0-9]|[A-Z])([0-9]|[A-Z])");

                foreach (Match m2 in mcText2)
                {
                    EDIDText += m2.ToString();
                    EDIDText += " ";
                    Length++;
                }
            }
            if (Length != 0)
            {
                Console.WriteLine(EDIDText);
                Console.WriteLine("EDID Length: {0}", Length.ToString());
            }
            return Length;
        }
        public static int Match0xTextEDID(string Text)//0x.. format
        {
            int Length = 0;

            MatchCollection mcText = Regex.Matches(Text, @"([0-9]|[A-Z])([0-9]|[A-Z])");

            foreach (Match m in mcText)
            {
                EDIDText += m.ToString();
                EDIDText += " ";
                Length++;
            }
            Console.WriteLine(EDIDText);
            Console.WriteLine("EDID Length: {0}", Length.ToString());
            return Length;
        }
        private static void FormatStringToByte(string EDIDText, byte[] EDIDByte)
        {
            byte i = 0;
            MatchCollection mcText = Regex.Matches(EDIDText, @"([0-9]|[A-Z])([0-9]|[A-Z])");

            foreach (Match m in mcText)
            {
                EDIDByte[i] = byte.Parse(m.ToString(), System.Globalization.NumberStyles.HexNumber);
                i++;
            }
        }
        private static EDIDStandardTiming FormatStandardTimingData(byte Data0, byte Data1)
        {
            EDIDStandardTiming StandardTimingTable = new EDIDStandardTiming();

            if (Data0 == 0x01)
                StandardTimingTable.TimingSupport = Support.unsupported;
            else
            {
                StandardTimingTable.TimingSupport = Support.supported;
                StandardTimingTable.TimingWidth = (ushort)((Data0 + 31) * 8);
                StandardTimingTable.TimingRatio = (StandardTimingRatio)(Data1 >> 6);
                StandardTimingTable.TimingRate = (byte)((byte)(Data1 & 0x3F) + 60);
            }

            switch (StandardTimingTable.TimingRatio)
            {
                case StandardTimingRatio.Ratio16x10:
                    StandardTimingTable.TimingHeight = (ushort)(StandardTimingTable.TimingWidth / 16 * 10);
                    break;

                case StandardTimingRatio.Ratio4x3:
                    StandardTimingTable.TimingHeight = (ushort)(StandardTimingTable.TimingWidth / 4 * 3);
                    break;

                case StandardTimingRatio.Ratio5x4:
                    StandardTimingTable.TimingHeight = (ushort)(StandardTimingTable.TimingWidth / 5 * 4);
                    break;

                case StandardTimingRatio.Ratio16x9:
                    StandardTimingTable.TimingHeight = (ushort)(StandardTimingTable.TimingWidth / 16 * 9);
                    break;
            }

            return StandardTimingTable;
        }
        private static EDIDDetailTimingTable FormatDetailTimingData(byte[] Data)
        {
            EDIDDetailTimingTable Timing = new EDIDDetailTimingTable();

            Timing.PixelClk = (ushort)(((uint)Data[1] << 8) + Data[0]);

            Timing.HAdressable = (ushort)(((uint)(Data[4] & 0xF0) << 4) + Data[2]);
            Timing.HBlanking = (ushort)(((uint)(Data[4] & 0x0F) << 8) + Data[3]);

            Timing.VAdressable = (ushort)(((uint)(Data[7] & 0xF0) << 4) + Data[5]);
            Timing.VBlanking = (ushort)(((uint)(Data[7] & 0x0F) << 8) + Data[6]);

            Timing.HSyncFront = (ushort)(((uint)(Data[11] & 0xC0) << 2) + Data[8]);
            Timing.HSyncWidth = (ushort)(((uint)(Data[11] & 0x30) << 4) + Data[9]);
            Timing.VSyncFront = (ushort)(((uint)(Data[11] & 0x0C) << 2) + ((uint)(Data[10] & 0xF0) >> 4));
            Timing.VSyncWidth = (ushort)(((uint)(Data[11] & 0x03) << 4) + (Data[10] & 0x0F));

            Timing.VideoSizeH = (ushort)(((uint)(Data[14] & 0xF0) << 4) + Data[12]);
            Timing.VideoSizeV = (ushort)(((uint)(Data[14] & 0x0F) << 8) + Data[13]);

            Timing.HBorder = Data[15];
            Timing.VBorder = Data[16];

            Timing.Interface = (InterfaceType)GetByteBit(Data[17],7);

            Timing.StereoFormat = (StereoViewingType)((uint)(Data[17] & 0x60) >> 5);

            Timing.SyncType = (SyncType)((uint)(Data[17] & 0x0C) >> 2);
            if (Timing.SyncType < SyncType.DigitalComposite)
                Timing.AnalogSync = (AnalogSyncType)((Data[17] & 0x03) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalComposite)
                Timing.DigitalSync = (DigitalSyncType)((Data[17] & 0x03) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalSeparate)
                Timing.DigitalSync = (DigitalSyncType)((Data[17] & 0x03) + 5);// + VSyncN_HSyncN


            Console.WriteLine("Timing PixelClock: {0} H :{1} HB :{2} V :{3} VB :{4} HSF :{5} HSW :{6} VSF :{7} VSW :{8} Hsize :{9} Vsize :{10} Interface :{11} StereoFormat :{12} SyncType :{13}  AnalogSync :{14}  DigitalSync :{15}",
                    Timing.PixelClk,
                    Timing.HAdressable,
                    Timing.HBlanking,
                    Timing.VAdressable,
                    Timing.VBlanking,
                    Timing.HSyncFront,
                    Timing.HSyncWidth,
                    Timing.VSyncFront,
                    Timing.VSyncWidth,
                    Timing.VideoSizeH,
                    Timing.VideoSizeV,
                    Timing.Interface,
                    Timing.StereoFormat,
                    Timing.SyncType,
                    Timing.AnalogSync,
                    Timing.DigitalSync);

            return Timing;
        }
        private static EDIDDescriptorsType FormatDisplayDescriptor(byte[] Data)
        {
            if ((Data[0] != 0x00) && (Data[1] != 0x00) && (Data[2] != 0x00))
            {
                EDIDFormData.SecondMainTiming = FormatDetailTimingData(Data);
                return EDIDDescriptorsType.DetailTiming;
            }

            switch (Data[3])
            {
                case 0xFF:
                    EDIDFormData.SN = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("SN :{0}", EDIDFormData.SN);
                    return EDIDDescriptorsType.ProductSN;

                case 0xFD:
                    Console.WriteLine("RangeLimits :");
                    return EDIDDescriptorsType.RangeLimits;

                case 0xFC:
                    EDIDFormData.Name = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("Name :{0}", EDIDFormData.Name);
                    return EDIDDescriptorsType.ProductName;

                default:            
                    return EDIDDescriptorsType.Undefined;
            }
        }
        private static FormatError FormatBaseBlock(byte[] EDIDData)
        {
            //00-07
            if ((EDIDData[0] != 0x00) || (EDIDData[1] != 0xFF) || (EDIDData[2] != 0xFF) || (EDIDData[3] != 0xFF) || (EDIDData[4] != 0xFF) || (EDIDData[5] != 0xFF) || (EDIDData[6] != 0xFF) || (EDIDData[7] != 0x00))
                return FormatError.HeaderError;

            //18-19 EDID_Version
            if ((EDIDData[18] == 0x01) && ((EDIDData[19] == 0x03) || (EDIDData[19] == 0x04)))
            {
                if (EDIDData[19] == 0x03)
                    EDIDFormData.Version = EDIDversion.V13;
                else
                    EDIDFormData.Version = EDIDversion.V14;

                Console.WriteLine("EDID Version: {0}", EDIDFormData.Version);
            }
            else
                return FormatError.VersionError;

            //08-09 EDID_IDManufacturerName
            //0001="A",11010="Z",A-Z
            byte[] ID_Data = new byte[3];
            ID_Data[0] = (byte)((EDIDData[8] >> 2) + 0x40);
            ID_Data[1] = (byte)(((EDIDData[8] & 0x03) << 3) + (EDIDData[9] >> 5) + 0x40);
            ID_Data[2] = (byte)((EDIDData[9] & 0x1F) + 0x40);
            EDIDFormData.IDManufacturerName = Encoding.ASCII.GetString(ID_Data);
            Console.WriteLine("Manufacturer Name: {0}", EDIDFormData.IDManufacturerName);

            //10-11 EDID_IDProductCode
            EDIDFormData.IDProductCode = (uint)(EDIDData[10] + (EDIDData[11] << 8));
            Console.WriteLine("ID Product: {0}", Convert.ToString(EDIDFormData.IDProductCode, 16));

            //12-15 EDID_IDSerialNumber
            if (((EDIDFormData.Version == EDIDversion.V13) && (EDIDData[12] == 0x01) && (EDIDData[13] == 0x01) && (EDIDData[14] == 0x01) && (EDIDData[15] == 0x01))
                || ((EDIDFormData.Version == EDIDversion.V14) && (EDIDData[12] == 0x00) && (EDIDData[13] == 0x00) && (EDIDData[14] == 0x00) && (EDIDData[15] == 0x00))
                )
            {
                EDIDFormData.IDSerialNumber = null;
                Console.WriteLine("ID Serial Number: not used");
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    EDIDFormData.IDSerialNumber += Convert.ToString(EDIDData[12 + i], 16);
                }
                Console.WriteLine("ID Serial Number: {0}", EDIDFormData.IDSerialNumber);
            }

            //16-17 EDID_Week EDID_Year EDID_Model_Year
            if (EDIDData[16] <= 54)
            {
                EDIDFormData.Week = EDIDData[16];
                Console.WriteLine("Week: {0}", EDIDFormData.Week);
                EDIDFormData.Year = (ushort)(EDIDData[17] + 1990);
                Console.WriteLine("Year: {0}", EDIDFormData.Year);
            }
            else if ((EDIDFormData.Version == EDIDversion.V14) && (EDIDData[16] == 0xFF))
            {
                EDIDFormData.ModelYear = (uint)(EDIDData[17] + 1990);
                Console.WriteLine("Week: not used");
                Console.WriteLine("Model Year: {0}", EDIDFormData.ModelYear);
            }

            //20-24 EDID_Basic
            //20
            EDIDFormData.Basic.Video_definition = (EDIDVideoStandard)((EDIDData[20] & 0x80) >> 7);
            Console.WriteLine("Video Standard: {0}", EDIDFormData.Basic.Video_definition);
            if (EDIDFormData.Version == EDIDversion.V14)
            {
                if (EDIDFormData.Basic.Video_definition == EDIDVideoStandard.EDIDVideoDigital)//EDID1.4 Digital
                {
                    EDIDFormData.Basic.DigitalColorDepth = (EDIDColorBitDepth)((EDIDData[20] & 0x70) >> 4);
                    Console.WriteLine("Color Bit Depth: {0}", EDIDFormData.Basic.DigitalColorDepth);

                    EDIDFormData.Basic.DigitalStandard = (EDIDDigitalVideoStandard)(EDIDData[20] & 0x0F);
                    Console.WriteLine("Digital Standard: {0}", EDIDFormData.Basic.DigitalStandard);
                }
            }
            else
            {
                if (EDIDFormData.Basic.Video_definition == EDIDVideoStandard.EDIDVideoDigital)//EDID1.3 Digital
                {
                }
                else
                {
                    EDIDFormData.Basic.AnalogSignalLevelStandard = (byte)((EDIDData[20] & 0x60) >> 5);
                    EDIDFormData.Basic.AnalogVideoSetup = (byte)((EDIDData[20] & 0x10) >> 4);
                    EDIDFormData.Basic.DigitalColorDepth = (EDIDColorBitDepth)(EDIDData[20] & 0x0F);
                }
            }
            //21-22
            if ((EDIDData[21] != 0x00) && (EDIDData[22] != 0x00))
            {
                EDIDFormData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_HV;
                EDIDFormData.Basic.ScreenSize.Hsize = EDIDData[21];
                EDIDFormData.Basic.ScreenSize.Vsize = EDIDData[22];
                Console.WriteLine("Screen Size: {0}, H: {1} cm, V: {2} cm", EDIDFormData.Basic.ScreenSize.Type, EDIDFormData.Basic.ScreenSize.Hsize, EDIDFormData.Basic.ScreenSize.Vsize);
            }
            else if (EDIDData[22] == 0x00)
            {
                EDIDFormData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDFormData.Basic.ScreenSize.Ratio = EDIDData[21];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDFormData.Basic.ScreenSize.Type, EDIDFormData.Basic.ScreenSize.Ratio);
            }
            else if (EDIDData[21] == 0x00)
            {
                EDIDFormData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDFormData.Basic.ScreenSize.Ratio = EDIDData[22];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDFormData.Basic.ScreenSize.Type, EDIDFormData.Basic.ScreenSize.Ratio);
            }
            else
            {
                EDIDFormData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_undefined;
            }
            //23
            EDIDFormData.Basic.Gamma = (float)EDIDData[23] / 100 + 1;
            Console.WriteLine("Gamma: {0} ", EDIDFormData.Basic.Gamma);
            //24
            EDIDFormData.Basic.FeatureSupport.StandbyMode = GetByteBitSupport(EDIDData[24], 7);
            EDIDFormData.Basic.FeatureSupport.SuspendMode = GetByteBitSupport(EDIDData[24], 6);
            EDIDFormData.Basic.FeatureSupport.VeryLowPowerMode = GetByteBitSupport(EDIDData[24], 5);
            EDIDFormData.Basic.FeatureSupport.sRGBStandard = GetByteBitSupport(EDIDData[24], 2);
            EDIDFormData.Basic.FeatureSupport.PreferredTimingMode = GetByteBitSupport(EDIDData[24], 1);
            if (EDIDFormData.Version == EDIDversion.V13)
            {
                EDIDFormData.Basic.FeatureSupport.DisplayColorType = (ColorType)((EDIDData[24] & 0x18) >> 3);
                EDIDFormData.Basic.FeatureSupport.GTFstandard = GetByteBitSupport(EDIDData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, DisplayColorType: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, GTFstandard: {6}",
                    EDIDFormData.Basic.FeatureSupport.StandbyMode,
                    EDIDFormData.Basic.FeatureSupport.SuspendMode,
                    EDIDFormData.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDFormData.Basic.FeatureSupport.DisplayColorType,
                    EDIDFormData.Basic.FeatureSupport.sRGBStandard,
                    EDIDFormData.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDFormData.Basic.FeatureSupport.GTFstandard);
            }
            else
            {
                EDIDFormData.Basic.FeatureSupport.ColorEncodingFormat = (ColorEncoding)((EDIDData[24] & 0x18) >> 3);
                EDIDFormData.Basic.FeatureSupport.ContinuousFrequency = GetByteBitSupport(EDIDData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, ColorEncodingFormat: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, ContinuousFrequency: {6}",
                    EDIDFormData.Basic.FeatureSupport.StandbyMode,
                    EDIDFormData.Basic.FeatureSupport.SuspendMode,
                    EDIDFormData.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDFormData.Basic.FeatureSupport.ColorEncodingFormat,
                    EDIDFormData.Basic.FeatureSupport.sRGBStandard,
                    EDIDFormData.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDFormData.Basic.FeatureSupport.ContinuousFrequency);
            }

            //25-34 EDID_Color
            EDIDFormData.PanelColor.RedX = GetEDIDColorxy((uint)(GetByteBit(EDIDData[25], 7)) * 2 + (uint)(GetByteBit(EDIDData[25], 6)) + ((uint)EDIDData[27] << 2));
            EDIDFormData.PanelColor.RedY = GetEDIDColorxy((uint)(GetByteBit(EDIDData[25], 5)) * 2 + (uint)(GetByteBit(EDIDData[25], 4)) + ((uint)EDIDData[28] << 2));
            EDIDFormData.PanelColor.GreenX = GetEDIDColorxy((uint)(GetByteBit(EDIDData[25], 3)) * 2 + (uint)(GetByteBit(EDIDData[25], 2)) + ((uint)EDIDData[29] << 2));
            EDIDFormData.PanelColor.GreenY = GetEDIDColorxy((uint)(GetByteBit(EDIDData[25], 1)) * 2 + (uint)(GetByteBit(EDIDData[25], 0)) + ((uint)EDIDData[30] << 2));
            EDIDFormData.PanelColor.BlueX = GetEDIDColorxy((uint)(GetByteBit(EDIDData[26], 7)) * 2 + (uint)(GetByteBit(EDIDData[26], 6)) + ((uint)EDIDData[31] << 2));
            EDIDFormData.PanelColor.BlueY = GetEDIDColorxy((uint)(GetByteBit(EDIDData[26], 5)) * 2 + (uint)(GetByteBit(EDIDData[26], 4)) + ((uint)EDIDData[32] << 2));
            EDIDFormData.PanelColor.WhiteX = GetEDIDColorxy((uint)(GetByteBit(EDIDData[26], 3)) * 2 + (uint)(GetByteBit(EDIDData[26], 2)) + ((uint)EDIDData[33] << 2));
            EDIDFormData.PanelColor.WhiteY = GetEDIDColorxy((uint)(GetByteBit(EDIDData[26], 1)) * 2 + (uint)(GetByteBit(EDIDData[26], 0)) + ((uint)EDIDData[34] << 2));
            Console.WriteLine("Color.Red X: {0} Y: {1}, Color.Green X: {2} Y: {3}, Color.Blue X: {4} Y: {5}, Color.White X: {6} Y: {7}",
                EDIDFormData.PanelColor.RedX,
                EDIDFormData.PanelColor.RedY,
                EDIDFormData.PanelColor.GreenX,
                EDIDFormData.PanelColor.GreenY,
                EDIDFormData.PanelColor.BlueX,
                EDIDFormData.PanelColor.BlueY,
                EDIDFormData.PanelColor.WhiteX,
                EDIDFormData.PanelColor.WhiteY
                );

            //35-37 EDID_Established_Timing
            EDIDFormData.EstablishedTiming.Es720x400_70 = GetByteBitSupport(EDIDData[35], 7);
            EDIDFormData.EstablishedTiming.Es720x400_88 = GetByteBitSupport(EDIDData[35], 6);
            EDIDFormData.EstablishedTiming.Es640x480_60 = GetByteBitSupport(EDIDData[35], 5);
            EDIDFormData.EstablishedTiming.Es640x480_67 = GetByteBitSupport(EDIDData[35], 4);
            EDIDFormData.EstablishedTiming.Es640x480_72 = GetByteBitSupport(EDIDData[35], 3);
            EDIDFormData.EstablishedTiming.Es640x480_75 = GetByteBitSupport(EDIDData[35], 2);
            EDIDFormData.EstablishedTiming.Es800x600_56 = GetByteBitSupport(EDIDData[35], 1);
            EDIDFormData.EstablishedTiming.Es800x600_60 = GetByteBitSupport(EDIDData[35], 0);

            EDIDFormData.EstablishedTiming.Es800x600_72 = GetByteBitSupport(EDIDData[36], 7);
            EDIDFormData.EstablishedTiming.Es800x600_75 = GetByteBitSupport(EDIDData[36], 6);
            EDIDFormData.EstablishedTiming.Es832x624_75 = GetByteBitSupport(EDIDData[36], 5);
            EDIDFormData.EstablishedTiming.Es1024x768_87 = GetByteBitSupport(EDIDData[36], 4);
            EDIDFormData.EstablishedTiming.Es1024x768_60 = GetByteBitSupport(EDIDData[36], 3);
            EDIDFormData.EstablishedTiming.Es1024x768_70 = GetByteBitSupport(EDIDData[36], 2);
            EDIDFormData.EstablishedTiming.Es1024x768_75 = GetByteBitSupport(EDIDData[36], 1);
            EDIDFormData.EstablishedTiming.Es1280x1024_75 = GetByteBitSupport(EDIDData[36], 0);

            EDIDFormData.EstablishedTiming.Es1152x870_75 = GetByteBitSupport(EDIDData[37], 7);

            //38-53 EDID_Standard_Timing
            EDIDFormData.StandardTiming = new EDIDStandardTiming[8];
            for (int i = 0; i < 8; i++)
            {
                EDIDFormData.StandardTiming[i] = FormatStandardTimingData(EDIDData[38 + i * 2], EDIDData[39 + i * 2]);
                if (EDIDFormData.StandardTiming[i].TimingSupport == Support.supported)
                    Console.WriteLine("Standard Timing : {0}x{1} Rate:{2}", EDIDFormData.StandardTiming[i].TimingWidth, EDIDFormData.StandardTiming[i].TimingHeight, EDIDFormData.StandardTiming[i].TimingRate);
            }

            byte[] DsecriptorTable = new byte[18];
            //54-71 EDID_Main_Timing (Display Dsecriptor 1)
            if (EDIDData[54] == 0x00)
                return FormatError.NoMainTimingError;
            EDIDFormData.Descriptors1 = EDIDDescriptorsType.DetailTiming;
            Array.Copy(EDIDData, 54, DsecriptorTable, 0, 18);
            EDIDFormData.MainTiming = FormatDetailTimingData(DsecriptorTable);

            //72-89 Detailed Timing / Display Dsecriptor 2
            if (EDIDData[75] == 0x00)
                EDIDFormData.Descriptors2 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDData, 72, DsecriptorTable, 0, 18);
                EDIDFormData.Descriptors2 = FormatDisplayDescriptor(DsecriptorTable);
            }

            //90-107 Detailed Timing / Display Dsecriptor 3
            if (EDIDData[93] == 0x00)
                EDIDFormData.Descriptors3 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDData, 90, DsecriptorTable, 0, 18);
                EDIDFormData.Descriptors3 = FormatDisplayDescriptor(DsecriptorTable);
            }

            //108-125 Detailed Timing / Display Dsecriptor 4
            if (EDIDData[111] == 0x00)
                EDIDFormData.Descriptors4 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDData, 108, DsecriptorTable, 0, 18);
                EDIDFormData.Descriptors4 = FormatDisplayDescriptor(DsecriptorTable);
            }

            //126 EDID_Ex_Block_Count
            EDIDFormData.ExBlockCount = EDIDData[126];

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += EDIDData[i];
            }
            if (checksum != 0x00)
                return FormatError.ChecksumError;
            else
                EDIDFormData.Checksum = EDIDData[127];

            return FormatError.Success;
        }
        private static bool FormatCEABlock(byte[] EDIDData)
        {
            return true;
        }
        private static bool FormatDisplayIDBlock(byte[] EDIDData)
        {
            return true;
        }
        public static FormatError Format(string UnicodeText)
        {
            int Length;
            FormatError Error;
            EDIDText = "";

            Length = MatchOriginalTextEDID(UnicodeText);

            if (Length == 0)
            {
                Length = Match0xTextEDID(UnicodeText);
            }

            byte[] Data = new byte[Length];

            FormatStringToByte(EDIDText, Data);

            if (Length >= 128)
            {
                Error = FormatBaseBlock(Data);
                if(Error != FormatError.Success)
                    return Error;
            }
            if (Length >= 256)
            {
                FormatCEABlock(Data);
            }
            if (Length >= 384)
            {
                FormatDisplayIDBlock(Data);
            }

            return FormatError.Success;
        }

        public static bool OutputNotesEDIDText(string Path)
        {
            string NoteEDID;
            //Format(EDIDText,256);

            NoteEDID = "        Time:";
            NoteEDID += System.DateTime.Now.ToString();
            NoteEDID += "\n";
            NoteEDID += EDIDText;

            using (FileStream fsWrite = new FileStream(Path, FileMode.Open, FileAccess.Write))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(NoteEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }

            return true;
        }
    }
}
