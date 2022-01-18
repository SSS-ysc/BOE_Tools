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
enum DecodeError
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
    Analog,
    Digital,
};
enum EDIDDigitalVideoStandard
{
    undefined,
    DVI,
    HDMI_a,
    HDMI_b,
    MDDI,
    DisplayPort,
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
    Non_Interlaced,
    Interlaced,
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

    MainTiming,
    SecondMainTiming,
}
enum LimitsHVOffsetsType
{ 
    Zero,
    Reserved,
    Max255MinZero,
    Max255Min255,
}
enum VideoTimingType
{ 
    DefaultGTF,
    RangeLimitsOnly,
    SecondaryGTF,
    CVT,
    Reserved,
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
    public uint PixelClk;

    public uint HFrequency;
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
    public LimitsHVOffsetsType VerticalOffest;
    public ushort VerticalMin;
    public ushort VerticalMax;
    public LimitsHVOffsetsType HorizontalOffest;
    public ushort HorizontalMin;
    public ushort HorizontalMax;
    public ushort PixelClkMax;
    public VideoTimingType VideoTiming;
};
struct EDIDTable
{
    public string IDManufacturerName;
    public uint IDProductCode;
    public string IDSerialNumber;
    public ushort Week;
    public ushort Year;
    public ushort ModelYear;
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
        public static byte[] EDIDByteData;
        public static uint EDIDDataLength;
        public static EDIDTable EDIDDTableData;
        public static DecodeError EDIDDecodeStatus;

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
        public static uint MatchOriginalTextEDID(string Text)//standard format
        {
            uint Length = 0;

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
        public static uint Match0xTextEDID(string Text)//0x.. format
        {
            uint Length = 0;

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
        private static void FormatStringToByte(string EDIDText)
        {
            byte i = 0;
            MatchCollection mcText = Regex.Matches(EDIDText, @"([0-9]|[A-Z])([0-9]|[A-Z])");

            foreach (Match m in mcText)
            {
                EDIDByteData[i] = byte.Parse(m.ToString(), System.Globalization.NumberStyles.HexNumber);
                i++;
            }
        }
        private static EDIDStandardTiming DecodeStandardTimingData(byte Data0, byte Data1)
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
        private static EDIDDetailTimingTable DecodeDetailTimingData(byte[] Data)
        {
            EDIDDetailTimingTable Timing = new EDIDDetailTimingTable();

            Timing.PixelClk = (uint)((((uint)Data[1] << 8) + Data[0]) * 10000);

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

            Timing.HFrequency = (uint)(Timing.PixelClk / (Timing.HAdressable + Timing.HBlanking));
            Timing.VFrequency = (ushort)(Timing.HFrequency / (Timing.VAdressable + Timing.VBlanking));

            Timing.Interface = (InterfaceType)GetByteBit(Data[17],7);

            Timing.StereoFormat = (StereoViewingType)((uint)(Data[17] & 0x60) >> 5);

            Timing.SyncType = (SyncType)((uint)(Data[17] & 0x0C) >> 2);
            if (Timing.SyncType < SyncType.DigitalComposite)
                Timing.AnalogSync = (AnalogSyncType)((Data[17] & 0x03) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalComposite)
                Timing.DigitalSync = (DigitalSyncType)((Data[17] & 0x03) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalSeparate)
                Timing.DigitalSync = (DigitalSyncType)((Data[17] & 0x03) + 5);// + VSyncN_HSyncN


            Console.WriteLine("Timing PixelClock: {0:000.00} H :{1} HB :{2} V :{3} VB :{4} HSF :{5} HSW :{6} VSF :{7} VSW :{8} Hsize :{9} Vsize :{10} Interface :{11} StereoFormat :{12} SyncType :{13}  AnalogSync :{14}  DigitalSync :{15} HFreq :{16} VFreq :{17}",
                    Timing.PixelClk / 1000000,
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
                    Timing.DigitalSync,
                    Timing.HFrequency,
                    Timing.VFrequency);

            return Timing;
        }
        private static EDIDDescriptorsType DecodeDisplayDescriptor(byte[] Data)
        {
            if ((Data[0] != 0x00) && (Data[1] != 0x00) && (Data[2] != 0x00))
            {
                EDIDDTableData.SecondMainTiming = DecodeDetailTimingData(Data);
                return EDIDDescriptorsType.SecondMainTiming;
            }

            switch (Data[3])
            {
                case 0xFF:
                    EDIDDTableData.SN = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("SN :{0}", EDIDDTableData.SN);
                    return EDIDDescriptorsType.ProductSN;
                case 0xFE:
                    Console.WriteLine("AlphanumericData : Unresolved");
                    return EDIDDescriptorsType.AlphanumericData;
                case 0xFD:
                    EDIDDTableData.Limits.VerticalOffest = (LimitsHVOffsetsType)(Data[4] & 0x03);
                    EDIDDTableData.Limits.HorizontalOffest = (LimitsHVOffsetsType)((uint)(Data[4] & 0x0C)>>2);
                    EDIDDTableData.Limits.VerticalMin = (ushort)(Data[5] + (EDIDDTableData.Limits.VerticalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    EDIDDTableData.Limits.VerticalMax = (ushort)(Data[6] + (EDIDDTableData.Limits.VerticalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    EDIDDTableData.Limits.HorizontalMin = (ushort)(Data[7] + (EDIDDTableData.Limits.HorizontalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    EDIDDTableData.Limits.HorizontalMax = (ushort)(Data[8] + (EDIDDTableData.Limits.HorizontalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    EDIDDTableData.Limits.PixelClkMax = (ushort)(Data[9] * 10);
                    EDIDDTableData.Limits.VideoTiming = (VideoTimingType)(Data[10]);
                    Console.WriteLine("RangeLimits : V {0}-{1}Hz, H {2}-{3}KHz, PixelClkMax {4}MHz, VideoTiming {5}", 
                        EDIDDTableData.Limits.VerticalMin, 
                        EDIDDTableData.Limits.VerticalMax,
                        EDIDDTableData.Limits.HorizontalMin,
                        EDIDDTableData.Limits.HorizontalMax,
                        EDIDDTableData.Limits.PixelClkMax,
                        EDIDDTableData.Limits.VideoTiming);
                    return EDIDDescriptorsType.RangeLimits;
                case 0xFC:
                    EDIDDTableData.Name = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("Name :{0}", EDIDDTableData.Name);
                    return EDIDDescriptorsType.ProductName;
                case 0xFB:
                    Console.WriteLine("ColorData : Unresolved");
                    return EDIDDescriptorsType.ColorData;
                case 0xFA:
                    Console.WriteLine("StandardTiming : Unresolved");
                    return EDIDDescriptorsType.StandardTiming;
                case 0xF9:
                    Console.WriteLine("DCMdata : Unresolved");
                    return EDIDDescriptorsType.DCMdata;
                case 0xF8:
                    Console.WriteLine("CVT3ByteTiming : Unresolved");
                    return EDIDDescriptorsType.CVT3ByteTiming;
                case 0xF7:
                    Console.WriteLine("EstablishedTiming : Unresolved");
                    return EDIDDescriptorsType.EstablishedTiming;
                default:            
                    return EDIDDescriptorsType.Undefined;
            }
        }
        private static DecodeError DecodeBaseBlock()
        {
            //00-07
            if ((EDIDByteData[0] != 0x00) || (EDIDByteData[1] != 0xFF) || (EDIDByteData[2] != 0xFF) || (EDIDByteData[3] != 0xFF) || (EDIDByteData[4] != 0xFF) || (EDIDByteData[5] != 0xFF) || (EDIDByteData[6] != 0xFF) || (EDIDByteData[7] != 0x00))
                return DecodeError.HeaderError;

            //18-19 EDID_Version
            if ((EDIDByteData[18] == 0x01) && ((EDIDByteData[19] == 0x03) || (EDIDByteData[19] == 0x04)))
            {
                if (EDIDByteData[19] == 0x03)
                    EDIDDTableData.Version = EDIDversion.V13;
                else
                    EDIDDTableData.Version = EDIDversion.V14;

                Console.WriteLine("EDID Version: {0}", EDIDDTableData.Version);
            }
            else
                return DecodeError.VersionError;

            //08-09 EDID_IDManufacturerName
            //0001="A",11010="Z",A-Z
            byte[] ID_Data = new byte[3];
            ID_Data[0] = (byte)((EDIDByteData[8] >> 2) + 0x40);
            ID_Data[1] = (byte)(((EDIDByteData[8] & 0x03) << 3) + (EDIDByteData[9] >> 5) + 0x40);
            ID_Data[2] = (byte)((EDIDByteData[9] & 0x1F) + 0x40);
            EDIDDTableData.IDManufacturerName = Encoding.ASCII.GetString(ID_Data);
            Console.WriteLine("Manufacturer Name: {0}", EDIDDTableData.IDManufacturerName);

            //10-11 EDID_IDProductCode
            EDIDDTableData.IDProductCode = (uint)(EDIDByteData[10] + (EDIDByteData[11] << 8));
            Console.WriteLine("ID Product: {0}", Convert.ToString(EDIDDTableData.IDProductCode, 16));

            //12-15 EDID_IDSerialNumber
            if (((EDIDDTableData.Version == EDIDversion.V13) && (EDIDByteData[12] == 0x01) && (EDIDByteData[13] == 0x01) && (EDIDByteData[14] == 0x01) && (EDIDByteData[15] == 0x01))
                || ((EDIDDTableData.Version == EDIDversion.V14) && (EDIDByteData[12] == 0x00) && (EDIDByteData[13] == 0x00) && (EDIDByteData[14] == 0x00) && (EDIDByteData[15] == 0x00))
                )
            {
                EDIDDTableData.IDSerialNumber = null;
                Console.WriteLine("ID Serial Number: not used");
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    EDIDDTableData.IDSerialNumber += Convert.ToString(EDIDByteData[12 + i], 16);
                }
                Console.WriteLine("ID Serial Number: {0}", EDIDDTableData.IDSerialNumber);
            }

            //16-17 EDID_Week EDID_Year EDID_Model_Year
            if (EDIDByteData[16] <= 54)
            {
                EDIDDTableData.Week = EDIDByteData[16];
                Console.WriteLine("Week: {0}", EDIDDTableData.Week);
                EDIDDTableData.Year = (ushort)(EDIDByteData[17] + 1990);
                Console.WriteLine("Year: {0}", EDIDDTableData.Year);
            }
            else if ((EDIDDTableData.Version == EDIDversion.V14) && (EDIDByteData[16] == 0xFF))
            {
                EDIDDTableData.ModelYear = (ushort)(EDIDByteData[17] + 1990);
                Console.WriteLine("Week: not used");
                Console.WriteLine("Model Year: {0}", EDIDDTableData.ModelYear);
            }

            //20-24 EDID_Basic
            //20
            EDIDDTableData.Basic.Video_definition = (EDIDVideoStandard)((EDIDByteData[20] & 0x80) >> 7);
            Console.WriteLine("Video Standard: {0}", EDIDDTableData.Basic.Video_definition);
            if (EDIDDTableData.Version == EDIDversion.V14)
            {
                if (EDIDDTableData.Basic.Video_definition == EDIDVideoStandard.Digital)//EDID1.4 Digital
                {
                    EDIDDTableData.Basic.DigitalColorDepth = (EDIDColorBitDepth)((EDIDByteData[20] & 0x70) >> 4);
                    Console.WriteLine("Color Bit Depth: {0}", EDIDDTableData.Basic.DigitalColorDepth);

                    EDIDDTableData.Basic.DigitalStandard = (EDIDDigitalVideoStandard)(EDIDByteData[20] & 0x0F);
                    Console.WriteLine("Digital Standard: {0}", EDIDDTableData.Basic.DigitalStandard);
                }
            }
            else
            {
                if (EDIDDTableData.Basic.Video_definition == EDIDVideoStandard.Digital)//EDID1.3 Digital
                {
                }
                else
                {
                    EDIDDTableData.Basic.AnalogSignalLevelStandard = (byte)((EDIDByteData[20] & 0x60) >> 5);//?
                    EDIDDTableData.Basic.AnalogVideoSetup = (byte)((EDIDByteData[20] & 0x10) >> 4);//?
                    EDIDDTableData.Basic.DigitalColorDepth = (EDIDColorBitDepth)(EDIDByteData[20] & 0x0F);//?
                }
            }
            //21-22
            if ((EDIDByteData[21] != 0x00) && (EDIDByteData[22] != 0x00))
            {
                EDIDDTableData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_HV;
                EDIDDTableData.Basic.ScreenSize.Hsize = EDIDByteData[21];
                EDIDDTableData.Basic.ScreenSize.Vsize = EDIDByteData[22];
                Console.WriteLine("Screen Size: {0}, H: {1} cm, V: {2} cm", EDIDDTableData.Basic.ScreenSize.Type, EDIDDTableData.Basic.ScreenSize.Hsize, EDIDDTableData.Basic.ScreenSize.Vsize);
            }
            else if (EDIDByteData[22] == 0x00)
            {
                EDIDDTableData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDDTableData.Basic.ScreenSize.Ratio = EDIDByteData[21];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDDTableData.Basic.ScreenSize.Type, EDIDDTableData.Basic.ScreenSize.Ratio);
            }
            else if (EDIDByteData[21] == 0x00)
            {
                EDIDDTableData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDDTableData.Basic.ScreenSize.Ratio = EDIDByteData[22];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDDTableData.Basic.ScreenSize.Type, EDIDDTableData.Basic.ScreenSize.Ratio);
            }
            else
            {
                EDIDDTableData.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_undefined;
            }
            //23
            EDIDDTableData.Basic.Gamma = (float)EDIDByteData[23] / 100 + 1;
            Console.WriteLine("Gamma: {0} ", EDIDDTableData.Basic.Gamma);
            //24
            EDIDDTableData.Basic.FeatureSupport.StandbyMode = GetByteBitSupport(EDIDByteData[24], 7);
            EDIDDTableData.Basic.FeatureSupport.SuspendMode = GetByteBitSupport(EDIDByteData[24], 6);
            EDIDDTableData.Basic.FeatureSupport.VeryLowPowerMode = GetByteBitSupport(EDIDByteData[24], 5);
            EDIDDTableData.Basic.FeatureSupport.sRGBStandard = GetByteBitSupport(EDIDByteData[24], 2);
            EDIDDTableData.Basic.FeatureSupport.PreferredTimingMode = GetByteBitSupport(EDIDByteData[24], 1);
            if (EDIDDTableData.Version == EDIDversion.V13)
            {
                EDIDDTableData.Basic.FeatureSupport.DisplayColorType = (ColorType)((EDIDByteData[24] & 0x18) >> 3);
                EDIDDTableData.Basic.FeatureSupport.GTFstandard = GetByteBitSupport(EDIDByteData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, DisplayColorType: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, GTFstandard: {6}",
                    EDIDDTableData.Basic.FeatureSupport.StandbyMode,
                    EDIDDTableData.Basic.FeatureSupport.SuspendMode,
                    EDIDDTableData.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDDTableData.Basic.FeatureSupport.DisplayColorType,
                    EDIDDTableData.Basic.FeatureSupport.sRGBStandard,
                    EDIDDTableData.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDDTableData.Basic.FeatureSupport.GTFstandard);
            }
            else
            {
                EDIDDTableData.Basic.FeatureSupport.ColorEncodingFormat = (ColorEncoding)((EDIDByteData[24] & 0x18) >> 3);
                EDIDDTableData.Basic.FeatureSupport.ContinuousFrequency = GetByteBitSupport(EDIDByteData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, ColorEncodingFormat: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, ContinuousFrequency: {6}",
                    EDIDDTableData.Basic.FeatureSupport.StandbyMode,
                    EDIDDTableData.Basic.FeatureSupport.SuspendMode,
                    EDIDDTableData.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDDTableData.Basic.FeatureSupport.ColorEncodingFormat,
                    EDIDDTableData.Basic.FeatureSupport.sRGBStandard,
                    EDIDDTableData.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDDTableData.Basic.FeatureSupport.ContinuousFrequency);
            }

            //25-34 EDID_Color
            EDIDDTableData.PanelColor.RedX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 7)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 6)) + ((uint)EDIDByteData[27] << 2));
            EDIDDTableData.PanelColor.RedY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 5)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 4)) + ((uint)EDIDByteData[28] << 2));
            EDIDDTableData.PanelColor.GreenX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 3)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 2)) + ((uint)EDIDByteData[29] << 2));
            EDIDDTableData.PanelColor.GreenY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 1)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 0)) + ((uint)EDIDByteData[30] << 2));
            EDIDDTableData.PanelColor.BlueX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 7)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 6)) + ((uint)EDIDByteData[31] << 2));
            EDIDDTableData.PanelColor.BlueY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 5)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 4)) + ((uint)EDIDByteData[32] << 2));
            EDIDDTableData.PanelColor.WhiteX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 3)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 2)) + ((uint)EDIDByteData[33] << 2));
            EDIDDTableData.PanelColor.WhiteY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 1)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 0)) + ((uint)EDIDByteData[34] << 2));
            Console.WriteLine("Color.Red X: {0} Y: {1}, Color.Green X: {2} Y: {3}, Color.Blue X: {4} Y: {5}, Color.White X: {6} Y: {7}",
                EDIDDTableData.PanelColor.RedX,
                EDIDDTableData.PanelColor.RedY,
                EDIDDTableData.PanelColor.GreenX,
                EDIDDTableData.PanelColor.GreenY,
                EDIDDTableData.PanelColor.BlueX,
                EDIDDTableData.PanelColor.BlueY,
                EDIDDTableData.PanelColor.WhiteX,
                EDIDDTableData.PanelColor.WhiteY
                );

            //35-37 EDID_Established_Timing
            EDIDDTableData.EstablishedTiming.Es720x400_70 = GetByteBitSupport(EDIDByteData[35], 7);
            EDIDDTableData.EstablishedTiming.Es720x400_88 = GetByteBitSupport(EDIDByteData[35], 6);
            EDIDDTableData.EstablishedTiming.Es640x480_60 = GetByteBitSupport(EDIDByteData[35], 5);
            EDIDDTableData.EstablishedTiming.Es640x480_67 = GetByteBitSupport(EDIDByteData[35], 4);
            EDIDDTableData.EstablishedTiming.Es640x480_72 = GetByteBitSupport(EDIDByteData[35], 3);
            EDIDDTableData.EstablishedTiming.Es640x480_75 = GetByteBitSupport(EDIDByteData[35], 2);
            EDIDDTableData.EstablishedTiming.Es800x600_56 = GetByteBitSupport(EDIDByteData[35], 1);
            EDIDDTableData.EstablishedTiming.Es800x600_60 = GetByteBitSupport(EDIDByteData[35], 0);
                                                                            
            EDIDDTableData.EstablishedTiming.Es800x600_72 = GetByteBitSupport(EDIDByteData[36], 7);
            EDIDDTableData.EstablishedTiming.Es800x600_75 = GetByteBitSupport(EDIDByteData[36], 6);
            EDIDDTableData.EstablishedTiming.Es832x624_75 = GetByteBitSupport(EDIDByteData[36], 5);
            EDIDDTableData.EstablishedTiming.Es1024x768_87 = GetByteBitSupport(EDIDByteData[36], 4);
            EDIDDTableData.EstablishedTiming.Es1024x768_60 = GetByteBitSupport(EDIDByteData[36], 3);
            EDIDDTableData.EstablishedTiming.Es1024x768_70 = GetByteBitSupport(EDIDByteData[36], 2);
            EDIDDTableData.EstablishedTiming.Es1024x768_75 = GetByteBitSupport(EDIDByteData[36], 1);
            EDIDDTableData.EstablishedTiming.Es1280x1024_75 = GetByteBitSupport(EDIDByteData[36], 0);

            EDIDDTableData.EstablishedTiming.Es1152x870_75 = GetByteBitSupport(EDIDByteData[37], 7);

            //38-53 EDID_Standard_Timing
            EDIDDTableData.StandardTiming = new EDIDStandardTiming[8];
            for (int i = 0; i < 8; i++)
            {
                EDIDDTableData.StandardTiming[i] = DecodeStandardTimingData(EDIDByteData[38 + i * 2], EDIDByteData[39 + i * 2]);
                if (EDIDDTableData.StandardTiming[i].TimingSupport == Support.supported)
                    Console.WriteLine("Standard Timing : {0}x{1} Rate:{2}", EDIDDTableData.StandardTiming[i].TimingWidth, EDIDDTableData.StandardTiming[i].TimingHeight, EDIDDTableData.StandardTiming[i].TimingRate);
            }

            byte[] DsecriptorTable = new byte[18];
            //54-71 EDID_Main_Timing (Display Dsecriptor 1)
            if (EDIDByteData[54] == 0x00)
                return DecodeError.NoMainTimingError;
            EDIDDTableData.Descriptors1 = EDIDDescriptorsType.MainTiming;
            Array.Copy(EDIDByteData, 54, DsecriptorTable, 0, 18);
            EDIDDTableData.MainTiming = DecodeDetailTimingData(DsecriptorTable);

            //72-89 Detailed Timing / Display Dsecriptor 2
            if (EDIDByteData[75] == 0x00)
                EDIDDTableData.Descriptors2 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 72, DsecriptorTable, 0, 18);
                EDIDDTableData.Descriptors2 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //90-107 Detailed Timing / Display Dsecriptor 3
            if (EDIDByteData[93] == 0x00)
                EDIDDTableData.Descriptors3 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 90, DsecriptorTable, 0, 18);
                EDIDDTableData.Descriptors3 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //108-125 Detailed Timing / Display Dsecriptor 4
            if (EDIDByteData[111] == 0x00)
                EDIDDTableData.Descriptors4 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 108, DsecriptorTable, 0, 18);
                EDIDDTableData.Descriptors4 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //126 EDID_Ex_Block_Count
            EDIDDTableData.ExBlockCount = EDIDByteData[126];

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += EDIDByteData[i];
            }
            if (checksum != 0x00)
                return DecodeError.ChecksumError;
            else
                EDIDDTableData.Checksum = EDIDByteData[127];

            return DecodeError.Success;
        }
        //private static CEABlock FormCEADataBlock(byte[] Data)
        //{ 
        
        //}
        private static DecodeError DecodeCEABlock()
        {
            return DecodeError.Success;
        }
        private static DecodeError DecodeDisplayIDBlock()
        {
            return DecodeError.Success;
        }
        public static DecodeError Decode(string UnicodeText)
        {
            DecodeError Error;
            EDIDDataLength = 0;
            EDIDText = "";

            EDIDDataLength = MatchOriginalTextEDID(UnicodeText);

            if (EDIDDataLength == 0)
            {
                EDIDDataLength = Match0xTextEDID(UnicodeText);
            }

            EDIDByteData = new byte[EDIDDataLength];
            FormatStringToByte(EDIDText);

            if (EDIDDataLength >= 128)
            {
                Error = DecodeBaseBlock();
                if(Error != DecodeError.Success)
                    return Error;
            }
            if (EDIDDataLength >= 256)
            {
                DecodeCEABlock();
            }
            if (EDIDDataLength >= 384)
            {
                DecodeDisplayIDBlock();
            }

            return DecodeError.Success;
        }
        private static string OutputNotesLineString(string Notes, int Offset, params object[] Value)
        {
            uint Index = 0;

            //{}格式数据插入
            if (Regex.IsMatch(Notes, @"\{") == true)
            {
                MatchCollection mcNotes = Regex.Matches(Notes, @"\{");

                foreach (Match m in mcNotes)
                {
                    Index++;
                }
                Notes = string.Format(Notes, Value);
            }
            //后置数据匹配偏移量
            if (Notes.Length < Offset)
            {
                Notes += new string(' ', Offset - Notes.Length);
            }
            //后置数据显示
            for (int i = 0; i < (Value.Length - Index); i++)
            {
                Notes += Value[i + Index].ToString();
            }

            Notes += "\n";

            return Notes;
        }
        private static string OutputNotesListString(string Notes, int Offset, params object[] Value)
        {
            Notes += "\n";

            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i].ToString() != "")
                {
                    Notes += new string(' ', Offset);
                    Notes += Value[i].ToString() + "\n";
                }
            }

            return Notes;
        }
        private static string OutputNotesEDIDList(uint EDIDByteDataOffset)
        {
            string Notes = "";
            Notes += "         0   1   2   3   4   5   6   7   8   9\n";
            Notes += "      ________________________________________\n";

            for (int LineNumber = 0; LineNumber <= 12; LineNumber++)
            {
                if (LineNumber == 0)
                    Notes += new string(' ', 2) + "0  |";
                else if (LineNumber >= 10)
                    Notes += new string(' ', 0) + LineNumber.ToString() + "0  |";
                else
                    Notes += new string(' ', 1) + LineNumber.ToString() + "0  |"; ;

                for (int Number = 0; Number < (LineNumber == 12 ? 8 : 10); Number++)
                {
                    Notes += "  " + string.Format("{0:X2}", EDIDByteData[EDIDByteDataOffset + LineNumber * 10 + Number]);
                }
                Notes += "\n";
            }
            Notes += "______________________________________________________________________\n";
            return Notes;
        }
        private static string OutputNotesDetailTiming(EDIDDetailTimingTable Timing)
        {
            string Notes;

            Notes = OutputNotesLineString("         Timing:{0}x{1}@{2}Hz   Pixel Clock: {3:000.00} MHz", 0, Timing.HAdressable, Timing.VAdressable, Timing.VFrequency, Timing.PixelClk/1000000);
            Notes += "\n";
            return Notes;
        }
        private static string OutputNotesDescriptorBlock(EDIDDescriptorsType Type)
        {
            string Notes = "\n";

            switch (Type)
            {
                case EDIDDescriptorsType.MainTiming:
                    Notes += OutputNotesDetailTiming(EDIDDTableData.MainTiming);
                    break;

                case EDIDDescriptorsType.SecondMainTiming:
                    Notes += OutputNotesDetailTiming(EDIDDTableData.SecondMainTiming);
                    break;

                case EDIDDescriptorsType.ProductSN:
                    Notes += "         Monitor Serial Number:\n";
                    Notes += "         " + EDIDDTableData.SN;
                    break;

                case EDIDDescriptorsType.ProductName:
                    Notes += "         Monitor Name:\n";
                    Notes += "         " + EDIDDTableData.Name;
                    break;

                case EDIDDescriptorsType.RangeLimits:
                    Notes += "         Monitor Range Limits:\n";
                   
                    break;

                default:
                    Notes += Type.ToString()+ "\n";
                    break;
            }

            Notes += "\n";
            return Notes;
        }
        public static bool OutputNotesEDIDText(string Path)
        {
            string NoteEDID;
            int ValueOffset = 50;

            NoteEDID = "        Time:" + System.DateTime.Now.ToString() + "\n";

            if (EDIDDecodeStatus == DecodeError.Success)
            {
                if (EDIDDataLength >= 128)
                {
                    NoteEDID += OutputNotesEDIDList(0);
                    NoteEDID += OutputNotesLineString("(08-09) ID Manufacturer Name:", ValueOffset, EDIDDTableData.IDManufacturerName);
                    NoteEDID += OutputNotesLineString("(10-11) Product ID Code:", ValueOffset, Convert.ToString(EDIDDTableData.IDProductCode, 16));
                    if (EDIDDTableData.IDSerialNumber == null)
                        NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, "not used");
                    else
                        NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, EDIDDTableData.IDSerialNumber);
                    NoteEDID += OutputNotesLineString("(16) Week of Manufacture:", ValueOffset, EDIDDTableData.Week);
                    NoteEDID += OutputNotesLineString("(17) Yaer of Manufacture:", ValueOffset, EDIDDTableData.Year);
                    NoteEDID += OutputNotesLineString("(18) EDID Version Number:", ValueOffset, "1");
                    NoteEDID += OutputNotesLineString("(19) EDID Revision Number:", ValueOffset, (3 + EDIDDTableData.Version));
                    NoteEDID += OutputNotesLineString("(20) Video Input Definition:", ValueOffset, EDIDDTableData.Basic.Video_definition);
                    if (EDIDDTableData.Version == EDIDversion.V14)
                        NoteEDID += OutputNotesLineString("     ", 0, EDIDDTableData.Basic.DigitalStandard.ToString(), "  ", EDIDDTableData.Basic.DigitalColorDepth.ToString());

                    NoteEDID += OutputNotesLineString("(21) ScreenSize Horizontal:", ValueOffset, EDIDDTableData.Basic.ScreenSize.Hsize, " cm");
                    NoteEDID += OutputNotesLineString("(22) ScreenSize Vertical:", ValueOffset, EDIDDTableData.Basic.ScreenSize.Vsize, " cm");
                    NoteEDID += OutputNotesLineString("(23) Display Gamma:", ValueOffset, EDIDDTableData.Basic.Gamma);
                    NoteEDID += OutputNotesLineString("(24) Power Management and Supported Feature(s):", 0);
                    NoteEDID += OutputNotesLineString("     ", 0,
                        (EDIDDTableData.Basic.FeatureSupport.StandbyMode == Support.supported ? "Standby Mode/":""),
                        (EDIDDTableData.Basic.FeatureSupport.SuspendMode == Support.supported ? "Suspend Mode/" : ""),
                        (EDIDDTableData.Basic.FeatureSupport.VeryLowPowerMode == Support.supported ? "Very Low Power/" : ""),
                        EDIDDTableData.Basic.FeatureSupport.DisplayColorType, "/",
                        (EDIDDTableData.Basic.FeatureSupport.sRGBStandard == Support.supported ? "sRGBStandard/" : ""),
                        (EDIDDTableData.Basic.FeatureSupport.PreferredTimingMode == Support.supported ? "PreferredTimingMode/" : ""),
                        (EDIDDTableData.Basic.FeatureSupport.GTFstandard == Support.supported ? "GTFstandard" : ""));

                    NoteEDID += OutputNotesLineString("(25-34) Panel Color:", 0);
                    NoteEDID += OutputNotesLineString("        Red X - {0} Blue X - {1} Green X - {2} White X - {3}", 0, EDIDDTableData.PanelColor.RedX, EDIDDTableData.PanelColor.GreenX, EDIDDTableData.PanelColor.BlueX, EDIDDTableData.PanelColor.WhiteX);
                    NoteEDID += OutputNotesLineString("        Red Y - {0} Blue Y - {1} Green Y - {2} White Y - {3}", 0, EDIDDTableData.PanelColor.RedY, EDIDDTableData.PanelColor.GreenY, EDIDDTableData.PanelColor.BlueY, EDIDDTableData.PanelColor.WhiteY);

                    NoteEDID += OutputNotesListString("(35-37) Established Timing:", "(35-37) ".Length,
                        (EDIDDTableData.EstablishedTiming.Es720x400_70 == Support.supported ? "720x400 @ 70Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es720x400_88 == Support.supported ? "720x400 @ 88Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es640x480_60 == Support.supported ? "640x480 @ 60Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es640x480_67 == Support.supported ? "640x480 @ 67Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es640x480_72 == Support.supported ? "640x480 @ 72Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es640x480_75 == Support.supported ? "640x480 @ 75Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es800x600_56 == Support.supported ? "800x600 @ 56Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es800x600_60 == Support.supported ? "800x600 @ 60Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es800x600_72 == Support.supported ? "800x600 @ 72Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es800x600_75 == Support.supported ? "800x600 @ 75Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es832x624_75 == Support.supported ? "832x624 @ 75Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1024x768_87 == Support.supported ? "1024x768 @ 87Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1024x768_60 == Support.supported ? "1024x768 @ 60Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1024x768_70 == Support.supported ? "1024x768 @ 70Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1024x768_75 == Support.supported ? "1024x768 @ 75Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1280x1024_75 == Support.supported ? "1280x1024 @ 75Hz" : ""),
                        (EDIDDTableData.EstablishedTiming.Es1152x870_75 == Support.supported ? "1152x870 @ 75Hz" : ""));

                    NoteEDID += OutputNotesLineString("(38-53) Standard Timing:", 0);
                    for (int i = 0; i < 8; i++)
                    {
                        if (EDIDDTableData.StandardTiming[i].TimingSupport == Support.supported)
                            NoteEDID += OutputNotesLineString("", "(38-53) ".Length, EDIDDTableData.StandardTiming[i].TimingWidth, "x", EDIDDTableData.StandardTiming[i].TimingHeight, " @ ", EDIDDTableData.StandardTiming[i].TimingRate, "Hz");
                    }
                    NoteEDID += "______________________________________________________________________\n";
                    NoteEDID += "(54-71) Descriptor Block 1:\n" + OutputNotesDescriptorBlock(EDIDDTableData.Descriptors1);
                    NoteEDID += "______________________________________________________________________\n";
                    NoteEDID += "(72-89) Descriptor Block 2:\n" + OutputNotesDescriptorBlock(EDIDDTableData.Descriptors2);
                    NoteEDID += "______________________________________________________________________\n";
                    NoteEDID += "(90-107) Descriptor Block 3:\n" + OutputNotesDescriptorBlock(EDIDDTableData.Descriptors3);
                    NoteEDID += "______________________________________________________________________\n";
                    NoteEDID += "(108-125) Descriptor Block 4:\n" + OutputNotesDescriptorBlock(EDIDDTableData.Descriptors4);

                    NoteEDID += OutputNotesLineString("(126) Extension EDID Block(s):", 0, EDIDDTableData.ExBlockCount);
                    NoteEDID += OutputNotesLineString("(127) CheckSum: OK",0);
                }

                if (EDIDDataLength >= 256)
                {
                    NoteEDID += OutputNotesEDIDList(128);
                }

                if (EDIDDataLength >= 384)
                {
                    NoteEDID += OutputNotesEDIDList(384);
                }
            }
            else
            {
                NoteEDID = "Decode error:" + EDIDDecodeStatus.ToString() + "\n";
            }

            using (FileStream fsWrite = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(NoteEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }

            return true;
        }
    }
}
