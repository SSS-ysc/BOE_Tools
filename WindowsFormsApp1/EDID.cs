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

    public ushort HAdressable;
    public ushort HBlanking;
    public ushort HSyncFront;
    public ushort HSyncWidth;
    public byte HBorder;

    public ushort VAdressable;
    public ushort VBlanking;
    public ushort VSyncFront;
    public ushort VSyncWidth;
    public byte VBorder;

    public ushort VideoSizeH;
    public ushort VideoSizeV;
};
struct EDIDDisplayRangeLimits
{
};
struct EDIDTable
{
    public string EDID_IDManufacturerName;
    public uint EDID_IDProductCode;
    public string EDID_IDSerialNumber;
    public ushort EDID_Week;
    public ushort EDID_Year;
    public uint EDID_Model_Year;
    public EDIDversion EDID_Version;
    public EDIDBasicDisplayParameters EDID_Basic;
    public EDIDColorCharacteristics EDID_Panel_Color;
    public EDIDEstablishedTimings EDID_Established_Timing;
    public EDIDStandardTiming[] EDID_Standard_Timing;
    public EDIDDetailTimingTable EDID_Main_Timing;
    public EDIDDetailTimingTable EDID_Second_Main_Timing;
    public string EDID_Display_Product_Serial_Number;
    public EDIDDisplayRangeLimits EDID_Display_Range_Limits;
    public string EDID_Display_Product_Name;
    public byte EDID_Ex_Block_Count;
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
        private static double GetEDIDColorxy(int xy)
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
        private static EDIDDetailTimingTable FormatDetailTimingData(byte[] EDIDDetailTimingData)
        {
            EDIDDetailTimingTable TimingTable = new EDIDDetailTimingTable(); 

            return TimingTable;
        }
        private static bool FormatBaseBlock(byte[] EDIDData)
        {
            //00-07
            if ((EDIDData[0] != 0x00) || (EDIDData[1] != 0xFF) || (EDIDData[2] != 0xFF) || (EDIDData[3] != 0xFF) || (EDIDData[4] != 0xFF) || (EDIDData[5] != 0xFF) || (EDIDData[6] != 0xFF) || (EDIDData[7] != 0x00))
                return false;

            //18-19 EDID_Version
            if ((EDIDData[18] == 0x01) && ((EDIDData[19] == 0x03) || (EDIDData[19] == 0x04)))
            {
                if (EDIDData[19] == 0x03)
                    EDIDFormData.EDID_Version = EDIDversion.V13;
                else
                    EDIDFormData.EDID_Version = EDIDversion.V14;

                Console.WriteLine("EDID Version: {0}", EDIDFormData.EDID_Version);
            }
            else
                return false;

            //08-09 EDID_IDManufacturerName
            //0001="A",11010="Z",A-Z
            byte[] ID_Data = new byte[3];
            ID_Data[0] = (byte)((EDIDData[8] >> 2) + 0x40);
            ID_Data[1] = (byte)(((EDIDData[8] & 0x03) << 3) + (EDIDData[9] >> 5) + 0x40);
            ID_Data[2] = (byte)((EDIDData[9] & 0x1F) + 0x40);
            EDIDFormData.EDID_IDManufacturerName = Encoding.ASCII.GetString(ID_Data);
            Console.WriteLine("Manufacturer Name: {0}", EDIDFormData.EDID_IDManufacturerName);

            //10-11 EDID_IDProductCode
            EDIDFormData.EDID_IDProductCode = (uint)(EDIDData[10] + (EDIDData[11] << 8));
            Console.WriteLine("ID Product: {0}", Convert.ToString(EDIDFormData.EDID_IDProductCode, 16));

            //12-15 EDID_IDSerialNumber
            if (((EDIDFormData.EDID_Version == EDIDversion.V13) && (EDIDData[12] == 0x01) && (EDIDData[13] == 0x01) && (EDIDData[14] == 0x01) && (EDIDData[15] == 0x01))
                || ((EDIDFormData.EDID_Version == EDIDversion.V14) && (EDIDData[12] == 0x00) && (EDIDData[13] == 0x00) && (EDIDData[14] == 0x00) && (EDIDData[15] == 0x00))
                )
            {
                EDIDFormData.EDID_IDSerialNumber = null;
                Console.WriteLine("ID Serial Number: not used");
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    EDIDFormData.EDID_IDSerialNumber += Convert.ToString(EDIDData[12 + i], 16);
                }
                Console.WriteLine("ID Serial Number: {0}", EDIDFormData.EDID_IDSerialNumber);
            }

            //16-17 EDID_Week EDID_Year EDID_Model_Year
            if (EDIDData[16] <= 54)
            {
                EDIDFormData.EDID_Week = EDIDData[16];
                Console.WriteLine("Week: {0}", EDIDFormData.EDID_Week);
                EDIDFormData.EDID_Year = (ushort)(EDIDData[17] + 1990);
                Console.WriteLine("Year: {0}", EDIDFormData.EDID_Year);
            }
            else if ((EDIDFormData.EDID_Version == EDIDversion.V14) && (EDIDData[16] == 0xFF))
            {
                EDIDFormData.EDID_Model_Year = (uint)(EDIDData[17] + 1990);
                Console.WriteLine("Week: not used");
                Console.WriteLine("Model Year: {0}", EDIDFormData.EDID_Model_Year);
            }

            //20-24 EDID_Basic
            //20
            EDIDFormData.EDID_Basic.Video_definition = (EDIDVideoStandard)((EDIDData[20] & 0x80) >> 7);
            Console.WriteLine("Video Standard: {0}", EDIDFormData.EDID_Basic.Video_definition);
            if (EDIDFormData.EDID_Version == EDIDversion.V14)
            {
                if (EDIDFormData.EDID_Basic.Video_definition == EDIDVideoStandard.EDIDVideoDigital)//EDID1.4 Digital
                {
                    EDIDFormData.EDID_Basic.DigitalColorDepth = (EDIDColorBitDepth)((EDIDData[20] & 0x70) >> 4);
                    Console.WriteLine("Color Bit Depth: {0}", EDIDFormData.EDID_Basic.DigitalColorDepth);

                    EDIDFormData.EDID_Basic.DigitalStandard = (EDIDDigitalVideoStandard)(EDIDData[20] & 0x0F);
                    Console.WriteLine("Digital Standard: {0}", EDIDFormData.EDID_Basic.DigitalStandard);
                }
            }
            else
            {
                if (EDIDFormData.EDID_Basic.Video_definition == EDIDVideoStandard.EDIDVideoDigital)//EDID1.3 Digital
                {
                }
                else
                {
                    EDIDFormData.EDID_Basic.AnalogSignalLevelStandard = (byte)((EDIDData[20] & 0x60) >> 5);
                    EDIDFormData.EDID_Basic.AnalogVideoSetup = (byte)((EDIDData[20] & 0x10) >> 4);
                    EDIDFormData.EDID_Basic.DigitalColorDepth = (EDIDColorBitDepth)(EDIDData[20] & 0x0F);
                }
            }
            //21-22
            if ((EDIDData[21] != 0x00) && (EDIDData[22] != 0x00))
            {
                EDIDFormData.EDID_Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_HV;
                EDIDFormData.EDID_Basic.ScreenSize.Hsize = EDIDData[21];
                EDIDFormData.EDID_Basic.ScreenSize.Vsize = EDIDData[22];
                Console.WriteLine("Screen Size: {0}, H: {1} cm, V: {2} cm", EDIDFormData.EDID_Basic.ScreenSize.Type, EDIDFormData.EDID_Basic.ScreenSize.Hsize, EDIDFormData.EDID_Basic.ScreenSize.Vsize);
            }
            else if (EDIDData[22] == 0x00)
            {
                EDIDFormData.EDID_Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDFormData.EDID_Basic.ScreenSize.Ratio = EDIDData[21];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDFormData.EDID_Basic.ScreenSize.Type, EDIDFormData.EDID_Basic.ScreenSize.Ratio);
            }
            else if (EDIDData[21] == 0x00)
            {
                EDIDFormData.EDID_Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDFormData.EDID_Basic.ScreenSize.Ratio = EDIDData[22];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDFormData.EDID_Basic.ScreenSize.Type, EDIDFormData.EDID_Basic.ScreenSize.Ratio);
            }
            else
            {
                EDIDFormData.EDID_Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_undefined;
            }
            //23
            EDIDFormData.EDID_Basic.Gamma = (float)EDIDData[23] / 100 + 1;
            Console.WriteLine("Gamma: {0} ", EDIDFormData.EDID_Basic.Gamma);
            //24
            EDIDFormData.EDID_Basic.FeatureSupport.StandbyMode = GetByteBitSupport(EDIDData[24], 7);
            EDIDFormData.EDID_Basic.FeatureSupport.SuspendMode = GetByteBitSupport(EDIDData[24], 6);
            EDIDFormData.EDID_Basic.FeatureSupport.VeryLowPowerMode = GetByteBitSupport(EDIDData[24], 5);
            EDIDFormData.EDID_Basic.FeatureSupport.sRGBStandard = GetByteBitSupport(EDIDData[24], 2);
            EDIDFormData.EDID_Basic.FeatureSupport.PreferredTimingMode = GetByteBitSupport(EDIDData[24], 1);
            if (EDIDFormData.EDID_Version == EDIDversion.V13)
            {
                EDIDFormData.EDID_Basic.FeatureSupport.DisplayColorType = (ColorType)((EDIDData[24] & 0x18) >> 3);
                EDIDFormData.EDID_Basic.FeatureSupport.GTFstandard = GetByteBitSupport(EDIDData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, DisplayColorType: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, GTFstandard: {6}",
                    EDIDFormData.EDID_Basic.FeatureSupport.StandbyMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.SuspendMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.DisplayColorType,
                    EDIDFormData.EDID_Basic.FeatureSupport.sRGBStandard,
                    EDIDFormData.EDID_Basic.FeatureSupport.PreferredTimingMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.GTFstandard);
            }
            else
            {
                EDIDFormData.EDID_Basic.FeatureSupport.ColorEncodingFormat = (ColorEncoding)((EDIDData[24] & 0x18) >> 3);
                EDIDFormData.EDID_Basic.FeatureSupport.ContinuousFrequency = GetByteBitSupport(EDIDData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, ColorEncodingFormat: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, ContinuousFrequency: {6}",
                    EDIDFormData.EDID_Basic.FeatureSupport.StandbyMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.SuspendMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.ColorEncodingFormat,
                    EDIDFormData.EDID_Basic.FeatureSupport.sRGBStandard,
                    EDIDFormData.EDID_Basic.FeatureSupport.PreferredTimingMode,
                    EDIDFormData.EDID_Basic.FeatureSupport.ContinuousFrequency);
            }

            //25-34 EDID_Color
            EDIDFormData.EDID_Panel_Color.RedX = GetEDIDColorxy((int)(GetByteBit(EDIDData[25], 7)) * 2 + (int)(GetByteBit(EDIDData[25], 6)) + ((int)EDIDData[27] << 2));
            EDIDFormData.EDID_Panel_Color.RedY = GetEDIDColorxy((int)(GetByteBit(EDIDData[25], 5)) * 2 + (int)(GetByteBit(EDIDData[25], 4)) + ((int)EDIDData[28] << 2));
            EDIDFormData.EDID_Panel_Color.GreenX = GetEDIDColorxy((int)(GetByteBit(EDIDData[25], 3)) * 2 + (int)(GetByteBit(EDIDData[25], 2)) + ((int)EDIDData[29] << 2));
            EDIDFormData.EDID_Panel_Color.GreenY = GetEDIDColorxy((int)(GetByteBit(EDIDData[25], 1)) * 2 + (int)(GetByteBit(EDIDData[25], 0)) + ((int)EDIDData[30] << 2));
            EDIDFormData.EDID_Panel_Color.BlueX = GetEDIDColorxy((int)(GetByteBit(EDIDData[26], 7)) * 2 + (int)(GetByteBit(EDIDData[26], 6)) + ((int)EDIDData[31] << 2));
            EDIDFormData.EDID_Panel_Color.BlueY = GetEDIDColorxy((int)(GetByteBit(EDIDData[26], 5)) * 2 + (int)(GetByteBit(EDIDData[26], 4)) + ((int)EDIDData[32] << 2));
            EDIDFormData.EDID_Panel_Color.WhiteX = GetEDIDColorxy((int)(GetByteBit(EDIDData[26], 3)) * 2 + (int)(GetByteBit(EDIDData[26], 2)) + ((int)EDIDData[33] << 2));
            EDIDFormData.EDID_Panel_Color.WhiteY = GetEDIDColorxy((int)(GetByteBit(EDIDData[26], 1)) * 2 + (int)(GetByteBit(EDIDData[26], 0)) + ((int)EDIDData[34] << 2));
            Console.WriteLine("Color.Red X: {0} Y: {1}, Color.Green X: {2} Y: {3}, Color.Blue X: {4} Y: {5}, Color.White X: {6} Y: {7}",
                EDIDFormData.EDID_Panel_Color.RedX,
                EDIDFormData.EDID_Panel_Color.RedY,
                EDIDFormData.EDID_Panel_Color.GreenX,
                EDIDFormData.EDID_Panel_Color.GreenY,
                EDIDFormData.EDID_Panel_Color.BlueX,
                EDIDFormData.EDID_Panel_Color.BlueY,
                EDIDFormData.EDID_Panel_Color.WhiteX,
                EDIDFormData.EDID_Panel_Color.WhiteY
                );

            //35-37 EDID_Established_Timing
            EDIDFormData.EDID_Established_Timing.Es720x400_70 = GetByteBitSupport(EDIDData[35], 7);
            EDIDFormData.EDID_Established_Timing.Es720x400_88 = GetByteBitSupport(EDIDData[35], 6);
            EDIDFormData.EDID_Established_Timing.Es640x480_60 = GetByteBitSupport(EDIDData[35], 5);
            EDIDFormData.EDID_Established_Timing.Es640x480_67 = GetByteBitSupport(EDIDData[35], 4);
            EDIDFormData.EDID_Established_Timing.Es640x480_72 = GetByteBitSupport(EDIDData[35], 3);
            EDIDFormData.EDID_Established_Timing.Es640x480_75 = GetByteBitSupport(EDIDData[35], 2);
            EDIDFormData.EDID_Established_Timing.Es800x600_56 = GetByteBitSupport(EDIDData[35], 1);
            EDIDFormData.EDID_Established_Timing.Es800x600_60 = GetByteBitSupport(EDIDData[35], 0);

            EDIDFormData.EDID_Established_Timing.Es800x600_72 = GetByteBitSupport(EDIDData[36], 7);
            EDIDFormData.EDID_Established_Timing.Es800x600_75 = GetByteBitSupport(EDIDData[36], 6);
            EDIDFormData.EDID_Established_Timing.Es832x624_75 = GetByteBitSupport(EDIDData[36], 5);
            EDIDFormData.EDID_Established_Timing.Es1024x768_87 = GetByteBitSupport(EDIDData[36], 4);
            EDIDFormData.EDID_Established_Timing.Es1024x768_60 = GetByteBitSupport(EDIDData[36], 3);
            EDIDFormData.EDID_Established_Timing.Es1024x768_70 = GetByteBitSupport(EDIDData[36], 2);
            EDIDFormData.EDID_Established_Timing.Es1024x768_75 = GetByteBitSupport(EDIDData[36], 1);
            EDIDFormData.EDID_Established_Timing.Es1280x1024_75 = GetByteBitSupport(EDIDData[36], 0);

            EDIDFormData.EDID_Established_Timing.Es1152x870_75 = GetByteBitSupport(EDIDData[37], 7);

            //38-53 EDID_Standard_Timing
            EDIDFormData.EDID_Standard_Timing = new EDIDStandardTiming[8];
            for (int i = 0; i< 8; i++)
            {
                EDIDFormData.EDID_Standard_Timing[i] = FormatStandardTimingData(EDIDData[38 + i * 2], EDIDData[39 + i * 2]);
                if (EDIDFormData.EDID_Standard_Timing[i].TimingSupport == Support.supported)
                    Console.WriteLine("Standard Timing : {0}x{1} Rate:{2}", EDIDFormData.EDID_Standard_Timing[i].TimingWidth, EDIDFormData.EDID_Standard_Timing[i].TimingHeight, EDIDFormData.EDID_Standard_Timing[i].TimingRate);
             }

            //54-71 EDID_Main_Timing

            //72-125 Detailed Timing / Display Dsecriptor

            //126 EDID_Ex_Block_Count

            //127 Checksum

            // EDID_Main_Timing
            if (EDIDData[0x4B] == 0xFF)
            {
                for (int i = 0; i < 13; i++)
                {
                    EDIDFormData.EDID_Display_Product_Serial_Number += Encoding.ASCII.GetString(EDIDData, 0x4D + i, 1);
                }
            }
            return true;
        }
        private static bool FormatCEABlock(byte[] EDIDData)
        {
            return true;
        }
        private static bool FormatDisplayIDBlock(byte[] EDIDData)
        {
            return true;
        }
        public static void Format(string UnicodeText)
        {
            int Length;
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
                FormatBaseBlock(Data);
            }
            if (Length >= 256)
            {
                FormatCEABlock(Data);
            }
            if (Length >= 384)
            {
                FormatDisplayIDBlock(Data);
            }
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
