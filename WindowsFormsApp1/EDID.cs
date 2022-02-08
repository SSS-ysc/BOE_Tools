using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace EDIDApp
{
    /************************** Common  ************************/
    #region
    enum Support
    {
        unsupported,
        supported,
    };
    enum DecodeError
    {
        NoDecode,
        Success,
        LengthError,

        HeaderError,
        VersionError,
        NoMainTimingError,
        ChecksumError,

        CEAVersionError,
        CEAChecksumError,

        DisplayIDVersionError,
        DisplayIDSizeError,
        DisplayIDTypeError,
        DisplayIDChecksumError,
    };
    enum DecompileError
    {
        NoDecompile,
        Success,

        VersionError,
    }
    enum BlockTagType
    {
        Base,
        CEA,
        DisplayID,
        BaseDisplayID,
    }
    enum InterfaceType
    {
        NonInterlaced,
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
        Analog_Composite,
        Bipolar_Analog_Composite,
        Digital_Composite,
        Digital_Separate,
    };
    enum AnalogSyncType
    {
        SyncOnGreenOnly,
        SyncOnRGB,
        Undefined,
    }
    enum SerrationsType
    {
        WithoutSerrations,
        WithSerrations,
        Undefined,
    }
    enum HVSyncType
    {
        Negative,
        Positive,
        Undefined,
    }
    struct EDIDDetailedTimingTable
    {
        public uint PixelClk; //Hz

        public uint HFrequency; //Hz
        public ushort HAdressable;
        public ushort HBlanking;
        public ushort HSyncFront;
        public ushort HSyncWidth;
        public byte HBorder;

        public float VFrequency; //Hz
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
        public HVSyncType HSync;
        public HVSyncType VSync;
        public SerrationsType Serrations;
    };
    #endregion
    internal class EDIDCommon
    {
        string[] DetailTimingAnalogSyncType = { "Sync On GreenOnly,", "Sync On RGB,", "" };
        string[] DetailTimingSerrationsType = { "Without Serrations,", "With Serrations,", "", };
        string[] DetailTimingHSyncType = { "Horizontal Polarity (-),", "Horizontal Polarity (+),", "" };
        string[] DetailTimingVSyncType = { "Vertical Polarity (-)", "Vertical Polarity (+)", "" };

        protected byte GetByteBit(byte a, byte X)
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
        protected Support GetByteBitSupport(byte a, byte X)
        {
            if (((a & (0x01 << X)) >> X) == 0x01)
                return Support.supported;
            else
                return Support.unsupported;
        }
        protected string GetSupportString(string Text, Support S)
        {
            return (S == Support.supported ? Text : "");
        }
        protected EDIDDetailedTimingTable DecodeDetailedTimingData(byte[] Data)
        {
            EDIDDetailedTimingTable Timing = new EDIDDetailedTimingTable();

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

            if ((Timing.PixelClk != 0) && (Timing.HAdressable != 0) && (Timing.VAdressable != 0))
            {
                Timing.HFrequency = (uint)(Timing.PixelClk / (Timing.HAdressable + Timing.HBlanking));
                Timing.VFrequency = (float)Timing.HFrequency / (Timing.VAdressable + Timing.VBlanking);
            }

            Timing.Interface = (InterfaceType)GetByteBit(Data[17], 7);

            Timing.StereoFormat = (StereoViewingType)((uint)(Data[17] & 0x60) >> 5);

            Timing.SyncType = (SyncType)((Data[17] & 0x18) >> 3);

            if (Timing.SyncType < SyncType.Digital_Composite)
            {
                Timing.AnalogSync = (AnalogSyncType)GetByteBit(Data[17], 1);
                Timing.Serrations = (SerrationsType)GetByteBit(Data[17], 2);
                Timing.HSync = HVSyncType.Undefined;
                Timing.VSync = HVSyncType.Undefined;
            }
            else if (Timing.SyncType == SyncType.Digital_Composite)
            {
                Timing.AnalogSync = AnalogSyncType.Undefined;
                Timing.Serrations = (SerrationsType)GetByteBit(Data[17], 2);
                Timing.HSync = (HVSyncType)GetByteBit(Data[17], 1);
                Timing.VSync = HVSyncType.Undefined;
            }
            else if (Timing.SyncType == SyncType.Digital_Separate)
            {
                Timing.AnalogSync = AnalogSyncType.Undefined;
                Timing.Serrations = SerrationsType.Undefined;
                Timing.HSync = (HVSyncType)GetByteBit(Data[17], 1);
                Timing.VSync = (HVSyncType)GetByteBit(Data[17], 2);
            }

            return Timing;
        }
        protected string OutputNotesLineString(string Notes, int ValueOffset, params object[] Value)
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
            if (Notes.Length < ValueOffset)
            {
                Notes += new string(' ', ValueOffset - Notes.Length);
            }
            //后置数据显示
            for (int i = 0; i < (Value.Length - Index); i++)
            {
                Notes += Value[i + Index].ToString();
            }

            Notes += "\r\n";

            Notes = Notes.Replace("_", " ");//替换某些枚举成员名称的 _ 符号

            return Notes;
        }
        protected string OutputNotesLineString(int NotesOffset, string Notes, int ValueOffset, params object[] Value)
        {
            uint Index = 0;

            Notes = new string(' ', NotesOffset) + Notes;

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
            if (Notes.Length < ValueOffset)
            {
                Notes += new string(' ', ValueOffset - Notes.Length);
            }
            //后置数据显示
            for (int i = 0; i < (Value.Length - Index); i++)
            {
                Notes += Value[i + Index].ToString();
            }

            Notes += "\r\n";

            Notes = Notes.Replace("_", " ");//替换某些枚举成员名称的 _ 符号

            return Notes;
        }
        protected string OutputNotesListString(string Notes, int Offset, params object[] Value)
        {
            Notes += "\r\n";

            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i].ToString() != "")
                {
                    Notes += new string(' ', Offset);
                    Notes += Value[i].ToString() + "\r\n";
                }
            }
            Notes = Notes.Replace("_", " ");//替换某些枚举成员名称的 _ 符号
            return Notes;
        }
        protected string OutputNotesListsString(string Notes, int Offset, object Value, string Notes2, int Offset2, object Value2)
        {
            //第一列数据
            Notes = new string(' ', Offset) + string.Format(Notes, Value);
            //第一列偏移
            if (Notes.Length < Offset2)
            {
                Notes += new string(' ', Offset2 - Notes.Length);
            }
            Notes += string.Format(Notes2, Value2);
            Notes += "\r\n";
            Notes = Notes.Replace("_", " ");//替换某些枚举成员名称的 _ 符号
            return Notes;
        }
        protected string OutputNotesEDIDList(byte[] Data)
        {
            string Notes = "";
            Notes += "         0   1   2   3   4   5   6   7   8   9\r\n";
            Notes += "      ________________________________________\r\n";

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
                    Notes += "  " + string.Format("{0:X2}", Data[LineNumber * 10 + Number]);
                }
                Notes += "\r\n";
            }
            Notes += "______________________________________________________________________\r\n";
            return Notes;
        }
        protected string OutputNotesDetailedTiming(EDIDDetailedTimingTable Timing)
        {
            string Notes;
            int list_offset = 8;
            int list_offset2 = 50;

            Notes = OutputNotesLineString(list_offset, "{0}x{1}@{2:.00}Hz   Pixel Clock: {3:.00} MHz", 0, Timing.HAdressable, Timing.VAdressable, Timing.VFrequency, (float)Timing.PixelClk / 1000000);
            Notes += "\r\n";
            Notes += OutputNotesListsString("Horizontal Image Size: {0} mm", list_offset, Timing.VideoSizeH, "Vertical Image Size: {0} mm", list_offset2, Timing.VideoSizeV);
            Notes += OutputNotesListsString("Refreshed Mode: {0}", list_offset, Timing.Interface, "Normal Display: {0}", list_offset2, Timing.StereoFormat);
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "Horizontal:", 0);
            Notes += OutputNotesListsString("Active Time: {0} pixels", list_offset, Timing.HAdressable, "Blanking Time: {0} pixels", list_offset2, Timing.HBlanking);
            Notes += OutputNotesListsString("Sync Offset: {0} pixels", list_offset, Timing.HSyncFront, "Sync Pulse Width: {0} pixels", list_offset2, Timing.HSyncWidth);
            Notes += OutputNotesListsString("Border: {0} pixels", list_offset, Timing.HBorder, "Frequency: {0:.00} Khz", list_offset2, (float)Timing.HFrequency / 1000);
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "Vertical:", 0);
            Notes += OutputNotesListsString("Active Time: {0} Lines", list_offset, Timing.VAdressable, "Blanking Time: {0} Lines", list_offset2, Timing.VBlanking);
            Notes += OutputNotesListsString("Sync Offset: {0} Lines", list_offset, Timing.VSyncFront, "Sync Pulse Width: {0} Lines", list_offset2, Timing.VSyncWidth);
            Notes += OutputNotesListsString("Border: {0} Lines", list_offset, Timing.VBorder, "Frequency: {0:.00} Hz", list_offset2, Timing.VFrequency);
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "{0},{1}{2}{3}{4}", 0, Timing.SyncType,
                DetailTimingAnalogSyncType[(int)Timing.AnalogSync],
                DetailTimingSerrationsType[(int)Timing.Serrations],
                DetailTimingHSyncType[(int)Timing.HSync],
                DetailTimingVSyncType[(int)Timing.VSync]
                );

            return Notes;
        }
    }
    /************************** Base Block *********************/
    #region
    struct BaseTable
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

        public EDIDDescriptorsType[] Descriptors;

        public EDIDDetailedTimingTable MainTiming;
        public EDIDDetailedTimingTable SecondMainTiming;
        public string SN;
        public EDIDDisplayRangeLimits Limits;
        public string Name;

        public byte ExBlockCount;
        public byte Checksum;
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
        Gray_scale,
        RGB,
        Non_RGB,
        ColorType_undefined,
    };
    enum ColorEncoding
    {
        RGB444,
        RGB444_YCrCr444,
        RGB444_YCrCr422,
        RGB444_YCrCr,
    };
    enum StandardTimingRatio
    {
        Ratio16x10,
        Ratio4x3,
        Ratio5x4,
        Ratio16x9,
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
        Default_GTF,
        Range_Limits_Only,
        Secondary_GTF,
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

        public byte AnalogSignalLevelStandard; //(Analog)
        public byte AnalogVideoSetup;
        public byte AnalogSeparateSyncSupport;
        public byte AnalogCompositeSyncSupport;
        public byte AnalogSOGSupport;
        public byte AnalogSerrationOnVsync;

        public byte DigitalDFP1X; //EDID 1.3 Digital

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
    #endregion
    internal class EDIDBase : EDIDCommon
    {
        internal BaseTable Table;
        internal byte[] Data;
        private double GetEDIDColorxy(uint xy)
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
        private EDIDStandardTiming DecodeStandardTimingData(byte Data0, byte Data1)
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
        private EDIDDescriptorsType DecodeDisplayDescriptor(byte[] Data)
        {
            if (!((Data[0] == 0) && (Data[1] == 0) && (Data[2] == 0)))
            {
                Table.SecondMainTiming = DecodeDetailedTimingData(Data);
                return EDIDDescriptorsType.SecondMainTiming;
            }

            switch (Data[3])
            {
                case 0xFF:
                    Table.SN = Encoding.ASCII.GetString(Data, 5, 13);
                    return EDIDDescriptorsType.ProductSN;
                case 0xFE:
                    Console.WriteLine("AlphanumericData : Unresolved");
                    return EDIDDescriptorsType.AlphanumericData;
                case 0xFD:
                    Table.Limits.VerticalOffest = (LimitsHVOffsetsType)(Data[4] & 0x03);
                    Table.Limits.HorizontalOffest = (LimitsHVOffsetsType)((uint)(Data[4] & 0x0C) >> 2);
                    Table.Limits.VerticalMin = (ushort)(Data[5] + (Table.Limits.VerticalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    Table.Limits.VerticalMax = (ushort)(Data[6] + (Table.Limits.VerticalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    Table.Limits.HorizontalMin = (ushort)(Data[7] + (Table.Limits.HorizontalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    Table.Limits.HorizontalMax = (ushort)(Data[8] + (Table.Limits.HorizontalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    Table.Limits.PixelClkMax = (ushort)(Data[9] * 10);
                    Table.Limits.VideoTiming = (VideoTimingType)(Data[10]);
                    return EDIDDescriptorsType.RangeLimits;
                case 0xFC:
                    Table.Name = Encoding.ASCII.GetString(Data, 5, 13);
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
        internal DecodeError DecodeBaseBlock()
        {
            Table = new BaseTable();
            //00-07
            if ((Data[0] != 0x00) || (Data[1] != 0xFF) || (Data[2] != 0xFF) || (Data[3] != 0xFF) || (Data[4] != 0xFF) || (Data[5] != 0xFF) || (Data[6] != 0xFF) || (Data[7] != 0x00))
                return DecodeError.HeaderError;

            //18-19 EDID_Version
            if ((Data[18] == 0x01) && ((Data[19] == 0x03) || (Data[19] == 0x04)))
            {
                if (Data[19] == 0x03)
                    Table.Version = EDIDversion.V13;
                else
                    Table.Version = EDIDversion.V14;
            }
            else
                return DecodeError.VersionError;

            //08-09 EDID_IDManufacturerName
            //0001="A",11010="Z",A-Z
            byte[] ID_Data = new byte[3];
            ID_Data[0] = (byte)((Data[8] >> 2) + 0x40);
            ID_Data[1] = (byte)(((Data[8] & 0x03) << 3) + (Data[9] >> 5) + 0x40);
            ID_Data[2] = (byte)((Data[9] & 0x1F) + 0x40);
            Table.IDManufacturerName = Encoding.ASCII.GetString(ID_Data);

            //10-11 EDID_IDProductCode
            Table.IDProductCode = (uint)(Data[10] + (Data[11] << 8));

            //12-15 EDID_IDSerialNumber
            if (((Data[12] == 0x01) && (Data[13] == 0x01) && (Data[14] == 0x01) && (Data[15] == 0x01))
                || ((Data[12] == 0x00) && (Data[13] == 0x00) && (Data[14] == 0x00) && (Data[15] == 0x00))
                )
            {
                Table.IDSerialNumber = null;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Table.IDSerialNumber += string.Format("{0:X2}", Data[15 - i]);
                }
            }

            //16-17 EDID_Week EDID_Year EDID_Model_Year
            if (Data[16] <= 54)
            {
                Table.Week = Data[16];
                Table.Year = (ushort)(Data[17] + 1990);
            }
            else if ((Table.Version == EDIDversion.V14) && (Data[16] == 0xFF))
            {
                Table.ModelYear = (ushort)(Data[17] + 1990);
            }

            //20-24 EDID_Basic
            //20
            Table.Basic.Video_definition = (EDIDVideoStandard)((Data[20] & 0x80) >> 7);
            if (Table.Basic.Video_definition == EDIDVideoStandard.Analog)
            {
                Table.Basic.AnalogSignalLevelStandard = (byte)((Data[20] & 0x60) >> 5);
                Table.Basic.AnalogVideoSetup = (byte)((Data[20] & 0x10) >> 4);
                Table.Basic.AnalogSeparateSyncSupport = (byte)((Data[20] & 0x08) >> 3);
                Table.Basic.AnalogCompositeSyncSupport = (byte)((Data[20] & 0x04) >> 2);
                Table.Basic.AnalogSOGSupport = (byte)((Data[20] & 0x02) >> 1);
                Table.Basic.AnalogSerrationOnVsync = (byte)((Data[20] & 0x01));
            }
            else
            {
                if (Table.Version == EDIDversion.V14)
                {
                    Table.Basic.DigitalColorDepth = (EDIDColorBitDepth)((Data[20] & 0x70) >> 4);
                    Table.Basic.DigitalStandard = (EDIDDigitalVideoStandard)(Data[20] & 0x0F);
                }
                else
                {
                    Table.Basic.DigitalDFP1X = (byte)((Data[20] & 0x01));
                }
            }
            //21-22
            if ((Data[21] != 0x00) && (Data[22] != 0x00))
            {
                Table.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_HV;
                Table.Basic.ScreenSize.Hsize = Data[21];
                Table.Basic.ScreenSize.Vsize = Data[22];
            }
            else if (Data[22] == 0x00)
            {
                Table.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                Table.Basic.ScreenSize.Ratio = Data[21];   // ?
            }
            else if (Data[21] == 0x00)
            {
                Table.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                Table.Basic.ScreenSize.Ratio = Data[22];   // ?
            }
            else
            {
                Table.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_undefined;
            }
            //23
            Table.Basic.Gamma = (float)Data[23] / 100 + 1;
            //24
            Table.Basic.FeatureSupport.StandbyMode = GetByteBitSupport(Data[24], 7);
            Table.Basic.FeatureSupport.SuspendMode = GetByteBitSupport(Data[24], 6);
            Table.Basic.FeatureSupport.VeryLowPowerMode = GetByteBitSupport(Data[24], 5);
            Table.Basic.FeatureSupport.sRGBStandard = GetByteBitSupport(Data[24], 2);
            Table.Basic.FeatureSupport.PreferredTimingMode = GetByteBitSupport(Data[24], 1);
            if (Table.Version == EDIDversion.V13)
            {
                Table.Basic.FeatureSupport.DisplayColorType = (ColorType)((Data[24] & 0x18) >> 3);
                Table.Basic.FeatureSupport.GTFstandard = GetByteBitSupport(Data[24], 0);
            }
            else
            {
                if(Table.Basic.Video_definition == EDIDVideoStandard.Analog)
                {
                    Table.Basic.FeatureSupport.DisplayColorType = (ColorType)((Data[24] & 0x18) >> 3);
                    Table.Basic.FeatureSupport.GTFstandard = GetByteBitSupport(Data[24], 0);
                }
                else
                {
                    Table.Basic.FeatureSupport.ColorEncodingFormat = (ColorEncoding)((Data[24] & 0x18) >> 3);
                    Table.Basic.FeatureSupport.ContinuousFrequency = GetByteBitSupport(Data[24], 0);
                }
            }

            //25-34 EDID_Color
            Table.PanelColor.RedX = GetEDIDColorxy((uint)(GetByteBit(Data[25], 7)) * 2 + (uint)(GetByteBit(Data[25], 6)) + ((uint)Data[27] << 2));
            Table.PanelColor.RedY = GetEDIDColorxy((uint)(GetByteBit(Data[25], 5)) * 2 + (uint)(GetByteBit(Data[25], 4)) + ((uint)Data[28] << 2));
            Table.PanelColor.GreenX = GetEDIDColorxy((uint)(GetByteBit(Data[25], 3)) * 2 + (uint)(GetByteBit(Data[25], 2)) + ((uint)Data[29] << 2));
            Table.PanelColor.GreenY = GetEDIDColorxy((uint)(GetByteBit(Data[25], 1)) * 2 + (uint)(GetByteBit(Data[25], 0)) + ((uint)Data[30] << 2));
            Table.PanelColor.BlueX = GetEDIDColorxy((uint)(GetByteBit(Data[26], 7)) * 2 + (uint)(GetByteBit(Data[26], 6)) + ((uint)Data[31] << 2));
            Table.PanelColor.BlueY = GetEDIDColorxy((uint)(GetByteBit(Data[26], 5)) * 2 + (uint)(GetByteBit(Data[26], 4)) + ((uint)Data[32] << 2));
            Table.PanelColor.WhiteX = GetEDIDColorxy((uint)(GetByteBit(Data[26], 3)) * 2 + (uint)(GetByteBit(Data[26], 2)) + ((uint)Data[33] << 2));
            Table.PanelColor.WhiteY = GetEDIDColorxy((uint)(GetByteBit(Data[26], 1)) * 2 + (uint)(GetByteBit(Data[26], 0)) + ((uint)Data[34] << 2));

            //35-37 EDID_Established_Timing
            Table.EstablishedTiming.Es720x400_70 = GetByteBitSupport(Data[35], 7);
            Table.EstablishedTiming.Es720x400_88 = GetByteBitSupport(Data[35], 6);
            Table.EstablishedTiming.Es640x480_60 = GetByteBitSupport(Data[35], 5);
            Table.EstablishedTiming.Es640x480_67 = GetByteBitSupport(Data[35], 4);
            Table.EstablishedTiming.Es640x480_72 = GetByteBitSupport(Data[35], 3);
            Table.EstablishedTiming.Es640x480_75 = GetByteBitSupport(Data[35], 2);
            Table.EstablishedTiming.Es800x600_56 = GetByteBitSupport(Data[35], 1);
            Table.EstablishedTiming.Es800x600_60 = GetByteBitSupport(Data[35], 0);

            Table.EstablishedTiming.Es800x600_72 = GetByteBitSupport(Data[36], 7);
            Table.EstablishedTiming.Es800x600_75 = GetByteBitSupport(Data[36], 6);
            Table.EstablishedTiming.Es832x624_75 = GetByteBitSupport(Data[36], 5);
            Table.EstablishedTiming.Es1024x768_87 = GetByteBitSupport(Data[36], 4);
            Table.EstablishedTiming.Es1024x768_60 = GetByteBitSupport(Data[36], 3);
            Table.EstablishedTiming.Es1024x768_70 = GetByteBitSupport(Data[36], 2);
            Table.EstablishedTiming.Es1024x768_75 = GetByteBitSupport(Data[36], 1);
            Table.EstablishedTiming.Es1280x1024_75 = GetByteBitSupport(Data[36], 0);

            Table.EstablishedTiming.Es1152x870_75 = GetByteBitSupport(Data[37], 7);

            //38-53 EDID_Standard_Timing
            Table.StandardTiming = new EDIDStandardTiming[8];
            for (int i = 0; i < 8; i++)
            {
                Table.StandardTiming[i] = DecodeStandardTimingData(Data[38 + i * 2], Data[39 + i * 2]);
            }

            Table.Descriptors = new EDIDDescriptorsType[4];
            byte[] DsecriptorTable = new byte[18];
            //54-71 EDID_Main_Timing (Display Dsecriptor 1)
            if (Data[54] == 0x00)
                return DecodeError.NoMainTimingError;
            Table.Descriptors[0] = EDIDDescriptorsType.MainTiming;
            Array.Copy(Data, 54, DsecriptorTable, 0, 18);
            Table.MainTiming = DecodeDetailedTimingData(DsecriptorTable);

            //72-89 Detailed Timing / Display Dsecriptor 2
            if (Data[75] == 0x00)
                Table.Descriptors[1] = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(Data, 72, DsecriptorTable, 0, 18);
                Table.Descriptors[1] = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //90-107 Detailed Timing / Display Dsecriptor 3
            if (Data[93] == 0x00)
                Table.Descriptors[2] = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(Data, 90, DsecriptorTable, 0, 18);
                Table.Descriptors[2] = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //108-125 Detailed Timing / Display Dsecriptor 4
            if (Data[111] == 0x00)
                Table.Descriptors[3] = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(Data, 108, DsecriptorTable, 0, 18);
                Table.Descriptors[3] = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //126 EDID_Ex_Block_Count
            Table.ExBlockCount = Data[126];

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += Data[i];
            }
            if (checksum != 0x00)
                return DecodeError.ChecksumError;
            else
                Table.Checksum = Data[127];

            return DecodeError.Success;
        }
        internal DecompileError DecompileBaseBlock()
        {
            Data = new byte[128];
            if (Table.Version == EDIDversion.V13)
            {
                Data[18] = 0x01;
                Data[19] = 0x03;
            }
            else if (Table.Version == EDIDversion.V14)
            {
                Data[18] = 0x01;
                Data[19] = 0x04;
            }
            else
                return DecompileError.VersionError;

            Data[0] = 0x00;
            Data[1] = 0xFF;
            Data[2] = 0xFF;
            Data[3] = 0xFF;
            Data[4] = 0xFF;
            Data[5] = 0xFF;
            Data[6] = 0xFF;
            Data[7] = 0x00;
            if (Table.IDManufacturerName != null)
            {
                int a = (int)Table.IDManufacturerName[0];
                int b = (int)Table.IDManufacturerName[1];
                int c = (int)Table.IDManufacturerName[2];
                Data[8] = (byte)(((a & 0x1F) << 2) + ((b & 0x18) >> 3));
                Data[9] = (byte)(((b & 0x07) << 5) + (c & 0x1F));
            }

            Data[10] = (byte)Table.IDProductCode;
            Data[11] = (byte)(Table.IDProductCode >> 8);

            Data[12] = Convert.ToByte(Table.IDSerialNumber.Substring(6, 2), 16);
            Data[13] = Convert.ToByte(Table.IDSerialNumber.Substring(4, 2), 16);
            Data[14] = Convert.ToByte(Table.IDSerialNumber.Substring(2, 2), 16);
            Data[15] = Convert.ToByte(Table.IDSerialNumber.Substring(0, 2), 16);

            if (Table.ModelYear == 0)
            {
                Data[16] = (byte)Table.Week;
                Data[17] = (byte)(Table.Year - 1990);
            }
            else
            {
                Data[16] = 0xFF;
                Data[17] = (byte)(Table.ModelYear - 1990);
            }
           


            return DecompileError.Success;
        }
        private string OutputNotesDescriptorBlock(EDIDDescriptorsType Type)
        {
            string Notes = "\r\n";
            int list_offset = 8;

            switch (Type)
            {
                case EDIDDescriptorsType.MainTiming:
                    Notes += OutputNotesDetailedTiming(Table.MainTiming);
                    break;

                case EDIDDescriptorsType.SecondMainTiming:
                    Notes += OutputNotesDetailedTiming(Table.SecondMainTiming);
                    break;

                case EDIDDescriptorsType.ProductSN:
                    Notes += OutputNotesLineString(list_offset, "Monitor Serial Number:", 0);
                    Notes += OutputNotesLineString(list_offset, "{0}", 0, Table.SN);
                    break;

                case EDIDDescriptorsType.ProductName:
                    Notes += OutputNotesLineString(list_offset, "Monitor Name:", 0);
                    Notes += OutputNotesLineString(list_offset, "{0}", 0, Table.Name);
                    break;

                case EDIDDescriptorsType.RangeLimits:
                    Notes += OutputNotesLineString(list_offset, "Monitor Range Limits:", 0);
                    Notes += OutputNotesLineString(list_offset, "Vertical Freq: {0} - {1} Hz", 0, Table.Limits.VerticalMin, Table.Limits.VerticalMax);
                    Notes += OutputNotesLineString(list_offset, "Horizontal Freq: {0} - {1} KHz", 0, Table.Limits.HorizontalMin, Table.Limits.HorizontalMax);
                    Notes += OutputNotesLineString(list_offset, "Pixel Clock: {0} MHz", 0, Table.Limits.PixelClkMax);
                    Notes += OutputNotesLineString(list_offset, "VideoTimingType: {0}", 0, Table.Limits.VideoTiming.ToString());
                    break;

                default:
                    Notes += OutputNotesLineString(list_offset, Type.ToString(), 0);
                    break;
            }
            return Notes;
        }
        internal string OutputNotesEDIDBase()
        {
            int ValueOffset = 50;
            int i;
            string NoteEDID = "\r\nBlock Type: Externded Display Identification Data\r\n";

            NoteEDID += OutputNotesEDIDList(Data);
            NoteEDID += OutputNotesLineString("(08-09) ID Manufacturer Name:", ValueOffset, Table.IDManufacturerName);
            NoteEDID += OutputNotesLineString("(10-11) Product ID Code:", ValueOffset, string.Format("{0:X}", Table.IDProductCode));
            if (Table.IDSerialNumber == null)
                NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, "not used");
            else
                NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, Table.IDSerialNumber);
            NoteEDID += OutputNotesLineString("(16) Week of Manufacture:", ValueOffset, Table.Week);
            NoteEDID += OutputNotesLineString("(17) Yaer of Manufacture:", ValueOffset, Table.Year);
            NoteEDID += OutputNotesLineString("(18) EDID Version Number:", ValueOffset, "1");
            NoteEDID += OutputNotesLineString("(19) EDID Revision Number:", ValueOffset, (3 + Table.Version));
            NoteEDID += OutputNotesLineString("(20) Video Input Definition:", ValueOffset, Table.Basic.Video_definition);
            if (Table.Version == EDIDversion.V14)
                NoteEDID += OutputNotesLineString("     ", 0, Table.Basic.DigitalStandard.ToString(), "  ", Table.Basic.DigitalColorDepth.ToString());

            NoteEDID += OutputNotesLineString("(21) ScreenSize Horizontal:", ValueOffset, Table.Basic.ScreenSize.Hsize, " cm");
            NoteEDID += OutputNotesLineString("(22) ScreenSize Vertical:", ValueOffset, Table.Basic.ScreenSize.Vsize, " cm");
            NoteEDID += OutputNotesLineString("(23) Display Gamma:", ValueOffset, Table.Basic.Gamma);
            NoteEDID += OutputNotesLineString("(24) Power Management and Supported Feature(s):", 0);
            if (Table.Version == EDIDversion.V13)
                NoteEDID += OutputNotesLineString("     ", 0,
                    GetSupportString("Standby Mode/ ", Table.Basic.FeatureSupport.StandbyMode),
                    GetSupportString("Suspend Mode/ ", Table.Basic.FeatureSupport.SuspendMode),
                    GetSupportString("Very Low Power/ ", Table.Basic.FeatureSupport.VeryLowPowerMode),
                    Table.Basic.FeatureSupport.DisplayColorType, "/ ",
                    GetSupportString("sRGB Standard/ ", Table.Basic.FeatureSupport.sRGBStandard),
                    GetSupportString("Preferred Timing Mode/ ", Table.Basic.FeatureSupport.PreferredTimingMode),
                    GetSupportString("GTF standard", Table.Basic.FeatureSupport.GTFstandard));
            else
                NoteEDID += OutputNotesLineString("     ", 0,
                    GetSupportString("Standby Mode/ ", Table.Basic.FeatureSupport.StandbyMode),
                    GetSupportString("Suspend Mode/ ", Table.Basic.FeatureSupport.SuspendMode),
                    GetSupportString("Very Low Power/ ", Table.Basic.FeatureSupport.VeryLowPowerMode),
                    Table.Basic.FeatureSupport.ColorEncodingFormat, "/ ",
                    GetSupportString("sRGB Standard/ ", Table.Basic.FeatureSupport.sRGBStandard),
                    GetSupportString("Preferred Timing Mode/ ", Table.Basic.FeatureSupport.PreferredTimingMode),
                    GetSupportString("Continuous Frequency", Table.Basic.FeatureSupport.ContinuousFrequency));
            NoteEDID += OutputNotesLineString("(25-34) Panel Color:", 0);
            NoteEDID += OutputNotesLineString("(25-34) ".Length, "Red X - {0:0.000} Blue X - {1:0.000} Green X - {2:0.000} White X - {3:0.000}", 0, Table.PanelColor.RedX, Table.PanelColor.GreenX, Table.PanelColor.BlueX, Table.PanelColor.WhiteX);
            NoteEDID += OutputNotesLineString("(25-34) ".Length, "Red Y - {0:0.000} Blue Y - {1:0.000} Green Y - {2:0.000} White Y - {3:0.000}", 0, Table.PanelColor.RedY, Table.PanelColor.GreenY, Table.PanelColor.BlueY, Table.PanelColor.WhiteY);

            NoteEDID += OutputNotesListString("(35-37) Established Timing:", "(35-37) ".Length,
                GetSupportString("720x400 @ 70Hz", Table.EstablishedTiming.Es720x400_70),
                GetSupportString("720x400 @ 88Hz", Table.EstablishedTiming.Es720x400_88),
                GetSupportString("640x480 @ 60Hz", Table.EstablishedTiming.Es640x480_60),
                GetSupportString("640x480 @ 67Hz", Table.EstablishedTiming.Es640x480_67),
                GetSupportString("640x480 @ 72Hz", Table.EstablishedTiming.Es640x480_72),
                GetSupportString("640x480 @ 75Hz", Table.EstablishedTiming.Es640x480_75),
                GetSupportString("800x600 @ 56Hz", Table.EstablishedTiming.Es800x600_56),
                GetSupportString("800x600 @ 60Hz", Table.EstablishedTiming.Es800x600_60),
                GetSupportString("800x600 @ 72Hz", Table.EstablishedTiming.Es800x600_72),
                GetSupportString("800x600 @ 75Hz", Table.EstablishedTiming.Es800x600_75),
                GetSupportString("832x624 @ 75Hz", Table.EstablishedTiming.Es832x624_75),
                GetSupportString("1024x768 @ 87Hz", Table.EstablishedTiming.Es1024x768_87),
                GetSupportString("1024x768 @ 60Hz", Table.EstablishedTiming.Es1024x768_60),
                GetSupportString("1024x768 @ 70Hz", Table.EstablishedTiming.Es1024x768_70),
                GetSupportString("1024x768 @ 75Hz", Table.EstablishedTiming.Es1024x768_75),
                GetSupportString("1280x1024 @ 75Hz", Table.EstablishedTiming.Es1280x1024_75),
                GetSupportString("1152x870 @ 75Hz", Table.EstablishedTiming.Es1152x870_75));

            NoteEDID += OutputNotesLineString("(38-53) Standard Timing:", 0);
            for (i = 0; i < 8; i++)
            {
                if (Table.StandardTiming[i].TimingSupport == Support.supported)
                    NoteEDID += OutputNotesLineString("", "(38-53) ".Length, Table.StandardTiming[i].TimingWidth, "x", Table.StandardTiming[i].TimingHeight, " @ ", Table.StandardTiming[i].TimingRate, "Hz");
            }
            NoteEDID += "______________________________________________________________________\r\n";
            NoteEDID += "(54-71) Descriptor Block 1:\r\n" + OutputNotesDescriptorBlock(Table.Descriptors[0]);
            NoteEDID += "______________________________________________________________________\r\n";
            NoteEDID += "(72-89) Descriptor Block 2:\r\n" + OutputNotesDescriptorBlock(Table.Descriptors[1]);
            NoteEDID += "______________________________________________________________________\r\n";
            NoteEDID += "(90-107) Descriptor Block 3:\r\n" + OutputNotesDescriptorBlock(Table.Descriptors[2]);
            NoteEDID += "______________________________________________________________________\r\n";
            NoteEDID += "(108-125) Descriptor Block 4:\r\n" + OutputNotesDescriptorBlock(Table.Descriptors[3]);

            NoteEDID += OutputNotesLineString("(126) Extension EDID Block(s):", 0, Table.ExBlockCount);
            NoteEDID += OutputNotesLineString("(127) CheckSum: OK", 0);

            return NoteEDID;
        }
    }
    /************************** CEA Block **********************/
    #region
    struct CEATable
    {
        public byte Version;
        public byte DetailedTimingStart;
        public Support UnderscranITFormatByDefault;
        public Support Audio;
        public Support YCbCr444;
        public Support YCbCr422;
        public byte NativeVideoFormatNumber;
        public List<CEABlocksTable> CEABlocksList;

        public List<BlockAudio> BlockAudio;
        public List<BlockVideoVIC> BlockVideoVIC;
        public List<BlockSpeaker> BlockSpeaker;

        public VSBlockHDMILLC BlockHDMILLC;
        public VSBlockAMD BlockAMD;
        public VSBlockHDMIForum BlockHDMIForum;

        public ExBlockVideoCapability BlockVideoCapability;
        public ExBlockColorimetry BlockColorimetry;
        public ExBlockHDRStatic BlockHDRStatic;
        public List<BlockVideoVIC> BlockYCbCr420VIC;

        public List<EDIDDetailedTimingTable> CEATimingList;
        public byte Checksum;
        public bool IsDataBlockInList(CEATagType tagtype)
        {
            foreach (CEABlocksTable cealist in CEABlocksList)
            {
                if (tagtype == cealist.Block)
                {
                    return true;
                }
            }
            return false;
        }
        public byte DataBlockIndex(CEATagType tagtype)
        {
            if(CEABlocksList.Count == 0)
            {
                return 255;
            }
            for(byte i=0;i<CEABlocksList.Count;i++)
            {
                if(tagtype == CEABlocksList[i].Block)
                {
                    return i;
                }
            }
            return 255;
        }
    };
    enum CEATagType
    {
        Reserved,
        Audio,
        Video,
        VendorSpecific,
        SpeakerAllocation,
        VESADisplayTransferCharacteristic,
        ReReserved,
        Extended,

        VS_HDMI_LLC,//Vendor Specific
        VS_AMD,
        VS_HDMI_Forum,
        VS_Mstar,
        VS_Realtek,

        Ex_Video_Capability,//Extended
        Ex_VS_Video_Capability,
        Ex_VESA_Display_Device,
        Ex_VESA_Video_Timing,
        Ex_HDMI_Video,
        Ex_Colorimetry,

        Ex_HDR_Static_Matadata,// 8-12 : Reserved for video-related blocks
        Ex_HDR_Dynamic_Matadata,

        Ex_Video_Format_Preference = Ex_Video_Capability + 13,
        Ex_YCbCr420Video,
        Ex_YCbCr420CapabilityMap,
        Ex_CEA_Miscellaneous_Audio_Fields,
        Ex_VS_Audio,
        Ex_HDMI_Audio,
        Ex_Room_Configuration,
        Ex_Speaker_Location,

        // 21-31 : Reserved for audio-related blocks

        Ex_Inframe = Ex_Video_Capability + 32,// 32

        Ex_VS_Dolby_Version,//Extended Vendor Specific
        Ex_VS_HDR10Plus,
    }
    enum AudioFormatType
    {
        Reserved,
        L_PCM,
        AC_3,
        MPEG_1,
        MP3,
        MPEG2,
        AACLC,
        DTS,
        ATRAC,
        OneBitAudio,
        EnhanecdAC_3,
        DTS_HD,
        MAT,
        DST,
        WMA_Pro,
        Extension,
    }
    enum AudioFormatExType
    {
        Reserved1,
        Reserved2,
        Reserved3,
        Reserved4,
        MPEG4_HE_AAC,
        MPEG4_HE_AAC_V2,
        MPEG4_HE_AAC_LC,
        DRA,
        MPEG4_HE_AAC_Surround,
        Reserved5,
        MPEG4_HE_AAC_LC_Surround,
        MPEG_H_3D_Audio,
        AC_4,
        LPCM_3D_Audio,
    }
    enum VideoCapabilityType
    {
        NoSupport,
        Always_Over_scanned,
        Always_Under_scanned,
        Support_Both_Over_And_Under,
    }
    enum HDMIFRLType
    {
        Nosupport_FRL,
        _3G_3Lane,
        _6G_3Lane,
        _6G_4Lane,
        _8G_4Lane,
        _10G_4Lane,
        _12G_4Lane,

        Reserved,
    }
    enum HDMIDSCMaxSlicesType
    {
        Nosupport_DSC_12a,
        up_to_1_Slice_and_up_to_340M,
        up_to_2_Slice_and_up_to_340M,
        up_to_4_Slice_and_up_to_340M,
        up_to_8_Slice_and_up_to_340M,
        up_to_8_Slice_and_up_to_400M,
        up_to_12_Slice_and_up_to_400M,
        up_to_16_Slice_and_up_to_400M,
        Reserved,
    }
    enum HDMIDSCMaxFRLType
    {
        Nosupport,
        _3G,
        _6G_3Lane,
        _6G_4Lane,
        _8G,
        _10G,
        _12G,
        Reserved,
    }
    struct BlockAudio
    {
        public AudioFormatType Type;
        public AudioFormatExType ExType;
        public int Channels;

        public Support Freq192Khz;
        public Support Freq176_4Khz;
        public Support Freq96Khz;
        public Support Freq88_2Khz;
        public Support Freq48Khz;
        public Support Freq44_1Khz;
        public Support Freq32Khz;
        //Type 1 (L_PCM)
        public Support Size16Bit;
        public Support Size20Bit;
        public Support Size24Bit;
        //Type 2-8
        public int Maxbit;
        //Type 9-13
        public int DependentValue;
        //Type 14 (WMA Pro)
        public int Profile;
    }
    struct BlockVideoVIC
    {
        public Support NativeCode;
        public byte VIC;
    }
    struct BlockSpeaker
    {
        public Support FLW_FRW; //Byte1 bit7
        public Support RLC_RRC;
        public Support FLC_FRC;
        public Support BC;
        public Support BL_BR;
        public Support FC;
        public Support LFE;
        public Support FL_FR;

        public Support TpFC; //Byte2 bit2
        public Support TpC;
        public Support TpFL_TpFH;
    }
    struct VSBlockHDMILLC
    {
        public byte PhyAddressA;
        public byte PhyAddressB;
        public byte PhyAddressC;
        public byte PhyAddressD;
        public Support ExtensionFields;
        public Support AllFeature;
        public Support DC_48bit;
        public Support DC_36bit;
        public Support DC_30bit;
        public Support DC_Y444;
        public Support DVI_Dual;
        public uint MaxTMDSClk;
        public Support EnableFlag;
        public Support LatencyFieldsPresent;
        public Support ILatencyFieldsPresent;
        public Support HDMIVideoPresent;
        public Support CN3;
        public Support CN2;
        public Support CN1;
        public Support CN0;
        public byte VideoLatency;
        public byte AudioLatency;
        public byte IVideoLatency;
        public byte IAudioLatency;
        public Support HDMI3DPresent;
        public uint HDMIVICLength;
        public uint HDMI3DLength;
        public List<byte> HDMIVIC;
    }
    struct VSBlockAMD
    {
        public byte Version;
        public Support LocalDimmingControl;
        public Support NativeColorSpaceSet;
        public Support FreeSync;
        public uint MinRefreshRate;
        public uint MaxRefreshRate;
        public byte MCCSVCPCode;
        public Support Gamma22EOTF;
        public float MaxBrightness_MaxBL;
        public float MinBrightness_MaxBL;
        public float MaxBrightness_MinBL;
        public float MinBrightness_MinBL;
        public uint MaxRefreshRate255;
    }
    struct VSBlockHDMIForum
    {
        public byte Version;

        public uint MaxTMDSRate;

        public Support SCDC_Present;
        public Support RR_Capable;
        public Support CABLE_STATUS;
        public Support CCBPCI;
        public Support LTE_340Mcsc_scramble;
        public Support _3D_Independent_View;
        public Support _3D_Dual_View;
        public Support _3D_OSD_Disparity;

        public Support UHD_VIC;
        public Support DC_48bit_420;
        public Support DC_36bit_420;
        public Support DC_30bit_420;
        public HDMIFRLType FRLRate;

        public Support FAPA_start_location;
        public Support ALLM;
        public Support FVA;
        public Support CNMVRR;
        public Support CinemaVRR;
        public Support M_Delta;

        public uint VRRMin;
        public uint VRRMax;

        public Support DSC_10bpc;
        public Support DSC_12bpc;
        public Support DSC_16bpc;
        public Support DSC_All_bpp;
        public Support DSC_Native_42;
        public Support DSC_1p2;

        public HDMIDSCMaxSlicesType DSCMaxSlices;
        public HDMIDSCMaxFRLType DSCMaxFRL;

        public byte DSC_TotalChunkkBytes;
    }
    struct ExBlockVideoCapability
    {
        public Support QY;
        public Support QS;
        public VideoCapabilityType PT;
        public VideoCapabilityType IT;
        public VideoCapabilityType CE;
    }
    struct ExBlockColorimetry
    {
        public Support BT2020_RGB;
        public Support BT2020_YCC;
        public Support BT2020_cYCC;
        public Support opRGB;
        public Support opYCC601;
        public Support sYCC601;
        public Support xvYCC709;
        public Support xvYCC601;
        public Support DCI_P3;
        public Support MD3;
        public Support MD2;
        public Support MD1;
        public Support MD0;
    }
    struct ExBlockHDRStatic
    {
        public Support Gamma_SDR;
        public Support Gamma_HDR;
        public Support SMPTE_ST_2084;
        public Support HLG;
        public Support Static_Metadata_Type1;
        public float Max_Luminance_Data;
        public float Max_Frame_Avg_Lum_Data;
        public float Min_Luminance_Data;
    }
    struct CEABlocksTable
    {
        public CEATagType Block;
        public int BlockPayload;

        public byte UnknowExtendedCode;
        public int UnknowIEEEID;
    }
    #endregion
    internal class EDIDCEA : EDIDCommon
    {
        internal CEATable Table;
        internal byte[] Data;
        string[] VICcode = {
                                    "No VIC",
                                    "640x480p@59.94Hz/60Hz 4:3",
                                    "720x480p@59.94Hz/60Hz 4:3",
                                    "720x480p@59.94Hz/60Hz 16:9",
                                    "1280x720p@59.94Hz/60Hz 16:9",
                                    "1920x1080i@59.94Hz/60Hz 16:9",
                                    "720(1440)x480i@59.94Hz/60Hz 4:3",
                                    "720(1440)x480i@59.94Hz/60Hz 16:9",
                                    "720(1440)x240p@59.94Hz/60Hz 4:3",
                                    "720(1440)x240p@59.94Hz/60Hz 16:9",
                                    "2880x480i@59.94Hz/60Hz 4:3",
                                    "2880x480i@59.94Hz/60Hz 16:9",
                                    "2880x240p@59.94Hz/60Hz 4:3",
                                    "2880x240p@59.94Hz/60Hz 16:9",
                                    "1440x480p@59.94Hz/60Hz 4:3",
                                    "1440x480p@59.94Hz/60Hz 16:9",
                                    "1920x1080p@59.94Hz/60Hz 16:9",
                                    "720x576p@50Hz 4:3",
                                    "720x576p@50Hz 16:9",
                                    "1280x720p@50Hz 16:9",
                                    "1920x1080i@50Hz 16:9",
                                    "720(1440)x576i@50Hz 4:3",
                                    "720(1440)x576i@50Hz 16:9",
                                    "720(1440)x288p@50Hz 4:3",
                                    "720(1440)x288p@50Hz 16:9",
                                    "2880x576i@50Hz 4:3",
                                    "2880x576i@50Hz 16:9",
                                    "2880x288p@50Hz 4:3",
                                    "2880x288p@50Hz 16:9",
                                    "1440x576p@50Hz 4:3",
                                    "1440x576p@50Hz 16:9",
                                    "1920x1080p@50Hz 16:9",
                                    "1920x1080p@23.98Hz/24Hz 16:9",
                                    "1920x1080p@25Hz 16:9",
                                    "1920x1080p@29.97Hz/30Hz 16:9",
                                    "2880x480p@59.94Hz/60Hz 4:3",
                                    "2880x480p@59.94Hz/60Hz 16:9",
                                    "2880x576p@50Hz 4:3",
                                    "2880x576p@50Hz 16:9",
                                    "1920x1080i (1250 total)@50Hz 16:9",
                                    "1920x1080i@100Hz 16:9",
                                    "1280x720p@100Hz 16:9",
                                    "720x576p@100Hz 4:3",
                                    "720x576p@100Hz 16:9",
                                    "720(1440)x576i@100Hz 4:3",
                                    "720(1440)x576i@100Hz 16:9",
                                    "1920x1080i@119.88/120Hz 16:9",
                                    "1280x720p@119.88/120Hz 16:9",
                                    "720x480p@119.88/120Hz 4:3",
                                    "720x480p@119.88/120Hz 16:9",
                                    "720(1440)x480i@119.88/120Hz 4:3",
                                    "720(1440)x480i@119.88/120Hz 16:9",
                                    "720x576p@200Hz 4:3",
                                    "720x576p@200Hz 16:9",
                                    "720(1440)x576i@200Hz 4:3",
                                    "720(1440)x576i@200Hz 16:9",
                                    "720x480p@239.76/240Hz 4:3",
                                    "720x480p@239.76/240Hz 16:9",
                                    "720(1440)x480i@239.76/240Hz 4:3",
                                    "720(1440)x480i@239.76/240Hz 16:9", // VIC 59 ,CEA-861-D

                                    "1280x720p@239.76/240Hz 16:9",
                                    "1280x720p@25Hz 16:9",
                                    "1280x720p@29.97Hz/30Hz 16:9",
                                    "1920x1080p@119.88/120Hz 16:9",
                                    "1920x1080p@100Hz 16:9", // VIC 64 ,CEA-861-E

                                    "1280x720p@23.98Hz/24Hz 64:27",
                                    "1280x720p@25Hz 64:27",
                                    "1280x720p@29.97Hz/30Hz 64:27",
                                    "1280x720p@50Hz 64:27",
                                    "1280x720p@59.94Hz/60Hz 64:27",
                                    "1280x720p@100Hz 64:27",
                                    "1280x720p@119.88/120Hz 64:27",
                                    "1920x1080p@23.98Hz/24Hz 64:27",
                                    "1920x1080p@25Hz 64:27",
                                    "1920x1080p@29.97Hz/30Hz 64:27",
                                    "1920x1080p@50Hz 64:27",
                                    "1920x1080p@59.94Hz/60Hz 64:27",
                                    "1920x1080p@100Hz 64:27",
                                    "1920x1080p@119.88/120Hz 64:27",
                                    "1680x720p@23.98Hz/24Hz 64:27",
                                    "1680x720p@25Hz 64:27",
                                    "1680x720p@29.97Hz/30Hz 64:27",
                                    "1680x720p@50Hz 64:27",
                                    "1680x720p@59.94Hz/60Hz 64:27",
                                    "1680x720p@100Hz 64:27",
                                    "1680x720p@119.88/120Hz 64:27",
                                    "2560x1080p@23.98Hz/24Hz 64:27",
                                    "2560x1080p@25Hz 64:27",
                                    "2560x1080p@29.97Hz/30Hz 64:27",
                                    "2560x1080p@50Hz 64:27",
                                    "2560x1080p@59.94Hz/60Hz 64:27",
                                    "2560x1080p@100Hz 64:27",
                                    "2560x1080p@119.88/120Hz 64:27",
                                    "3840x2160p@23.98Hz/24Hz 16:9",
                                    "3840x2160p@25Hz 16:9",
                                    "3840x2160p@29.97Hz/30Hz 16:9",
                                    "3840x2160p@50Hz 16:9",
                                    "3840x2160p@59.94Hz/60Hz 16:9",
                                    "4096x2160p@23.98Hz/24Hz 256:135",
                                    "4096x2160p@25Hz 256:135",
                                    "4096x2160p@29.97Hz/30Hz 256:135",
                                    "4096x2160p@50Hz 256:135",
                                    "4096x2160p@59.94Hz/60Hz 256:135",
                                    "3840x2160p@23.98Hz/24Hz 64:27",
                                    "3840x2160p@25Hz 64:27",
                                    "3840x2160p@29.97Hz/30Hz 64:27",
                                    "3840x2160p@50Hz 64:27",
                                    "3840x2160p@59.94Hz/60Hz 64:27", // VIC 107 ,CEA-861-F

                                    "1280x720p@47.95Hz/48Hz16:9",
                                    "1280x720p@47.95Hz/48Hz64:27",
                                    "1680x720p@47.95Hz/48Hz64:27",
                                    "1920x1080p@47.95Hz/48Hz16:9",
                                    "1920x1080p@47.95Hz/48Hz64:27",
                                    "2560x1080p@47.95Hz/48Hz64:27",
                                    "3840x2160p@47.95Hz/48Hz16:9",
                                    "4096x2160p@47.95Hz/48Hz256:135",
                                    "3840x2160p@47.95Hz/48Hz64:27",
                                    "3840x2160p@100Hz16:9",
                                    "3840x2160p@119.88/120Hz16:9",
                                    "3840x2160p@100Hz64:27",
                                    "3840x2160p@119.88/120Hz64:27",
                                    "5120x2160p@23.98Hz/24Hz64:27",
                                    "5120x2160p@25Hz64:27",
                                    "5120x2160p@29.97Hz/30Hz64:27",
                                    "5120x2160p@47.95Hz/48Hz64:27",
                                    "5120x2160p@50Hz64:27",
                                    "5120x2160p@59.94Hz/60Hz64:27",
                                    "5120x2160p@100Hz64:27", // VIC 127 ,CTA-861-G

                                    "Forbidden",
                                    "5120x2160p@119.88Hz/120Hz64:27",// VIC 193
                                    "7680x4320p@23.98Hz/24Hz16:9",
                                    "7680x4320p@25Hz16:9",
                                    "7680x4320p@29.97Hz/30Hz16:9",
                                    "7680x4320p@47.95Hz/48Hz16:9",
                                    "7680x4320p@50Hz16:9",
                                    "7680x4320p@59.94Hz/60Hz16:9",
                                    "7680x4320p@100Hz16:9",
                                    "7680x4320p@119.88Hz/120Hz16:9",
                                    "7680x4320p@23.98Hz/24Hz64:27",
                                    "7680x4320p@25Hz64:27",
                                    "7680x4320p@29.97Hz/30Hz64:27",
                                    "7680x4320p@47.95Hz/48Hz64:27",
                                    "7680x4320p@50Hz64:27",
                                    "7680x4320p@59.94Hz/60Hz64:27",
                                    "7680x4320p@100Hz64:27",
                                    "7680x4320p@119.88Hz/120Hz64:27",
                                    "10240x4320p@23.98Hz/24Hz64:27",
                                    "10240x4320p@25Hz64:27",
                                    "10240x4320p@29.97Hz/30Hz64:27",
                                    "10240x4320p@47.95Hz/48Hz64:27",
                                    "10240x4320p@50Hz64:27",
                                    "10240x4320p@59.94Hz/60Hz64:27",
                                    "10240x4320p@100Hz64:27",
                                    "10240x4320p@119.88Hz/120Hz64:27",
                                    "4096x2160p@100Hz256:135",
                                    "4096x2160p@119.88Hz/120Hz256:135",
        };
        string[] HDMIVICcode = { "", "3840x2160p@29.97Hz/30Hz", "3840x2160p@25Hz", "3840x2160p@23.98Hz/24Hz", "4096x2160p@23.98Hz/24Hz" };
        private BlockAudio DecodeCEAAudioBlock(byte[] Data, int index)
        {
            BlockAudio Audio = new BlockAudio();

            Audio.Type = (AudioFormatType)(byte)(Data[index] >> 3);
            Audio.Channels = (byte)(Data[index] & 0x07);
            Audio.Freq192Khz = GetByteBitSupport(Data[index + 1], 6);
            Audio.Freq176_4Khz = GetByteBitSupport(Data[index + 1], 5);
            Audio.Freq96Khz = GetByteBitSupport(Data[index + 1], 4);
            Audio.Freq88_2Khz = GetByteBitSupport(Data[index + 1], 3);
            Audio.Freq48Khz = GetByteBitSupport(Data[index + 1], 2);
            Audio.Freq44_1Khz = GetByteBitSupport(Data[index + 1], 1);
            Audio.Freq32Khz = GetByteBitSupport(Data[index + 1], 0);

            switch (Audio.Type)
            {
                case AudioFormatType.L_PCM:
                    Audio.Size24Bit = GetByteBitSupport(Data[index + 2], 2);
                    Audio.Size20Bit = GetByteBitSupport(Data[index + 2], 1);
                    Audio.Size16Bit = GetByteBitSupport(Data[index + 2], 0);
                    break;

                case AudioFormatType.AC_3:
                case AudioFormatType.MPEG_1:
                case AudioFormatType.MP3:
                case AudioFormatType.MPEG2:
                case AudioFormatType.AACLC:
                case AudioFormatType.DTS:
                case AudioFormatType.ATRAC:
                    break;

                case AudioFormatType.OneBitAudio:
                case AudioFormatType.EnhanecdAC_3:
                case AudioFormatType.DTS_HD:
                case AudioFormatType.MAT:
                case AudioFormatType.DST:
                    break;

                case AudioFormatType.WMA_Pro:
                    break;

                case AudioFormatType.Extension:
                    break;

                default: break;
            }
            return Audio;
        }
        private CEABlocksTable DecodeCEADataBlock(int index)
        {
            int i;
            CEABlocksTable Block = new CEABlocksTable();

            Block.BlockPayload = (int)Data[index] & 0x1F;
            Block.Block = (CEATagType)((Data[index] & 0xE0) >> 5);

            byte[] BlockData = new byte[Block.BlockPayload];
            Array.Copy(Data, index + 1, BlockData, 0, Block.BlockPayload);

            switch (Block.Block)
            {
                case CEATagType.Audio:
                    Table.BlockAudio = new List<BlockAudio>();

                    for (i = 0; i < Block.BlockPayload / 3; i++)
                    {
                        Table.BlockAudio.Add(DecodeCEAAudioBlock(BlockData, i * 3));
                    }
                    break;

                case CEATagType.Video:
                    Table.BlockVideoVIC = new List<BlockVideoVIC>();
                    BlockVideoVIC VIC = new BlockVideoVIC();

                    for (i = 0; i < Block.BlockPayload; i++)
                    {
                        if ((BlockData[i] & 0x7F) <= 64)
                        {
                            VIC.NativeCode = GetByteBitSupport(BlockData[i], 7);
                            VIC.VIC = (byte)(BlockData[i] & 0x7F);
                        }
                        else
                        {
                            VIC.NativeCode = Support.unsupported;
                            if (BlockData[i] > 127)
                                VIC.VIC = (byte)(BlockData[i] - 64);
                            else
                                VIC.VIC = BlockData[i];
                        }
                        Table.BlockVideoVIC.Add(VIC);
                    }
                    break;

                case CEATagType.VendorSpecific:
                    int VSDB_IEEEID = BlockData[0] + (BlockData[1] << 8) + (BlockData[2] << 16);
                    switch (VSDB_IEEEID)
                    {
                        case 0x000C03:
                            Block.Block = CEATagType.VS_HDMI_LLC;
                            Table.BlockHDMILLC.PhyAddressA = (byte)((BlockData[3] & 0xF0) >> 4);
                            Table.BlockHDMILLC.PhyAddressB = (byte)(BlockData[3] & 0x0F);
                            Table.BlockHDMILLC.PhyAddressC = (byte)((BlockData[4] & 0xF0) >> 4);
                            Table.BlockHDMILLC.PhyAddressD = (byte)(BlockData[4] & 0x0F);
                            if (Block.BlockPayload >= 5)
                            {
                                Table.BlockHDMILLC.ExtensionFields = Support.supported;
                                Table.BlockHDMILLC.AllFeature = GetByteBitSupport(BlockData[5], 7);
                                Table.BlockHDMILLC.DC_48bit = GetByteBitSupport(BlockData[5], 6);
                                Table.BlockHDMILLC.DC_36bit = GetByteBitSupport(BlockData[5], 5);
                                Table.BlockHDMILLC.DC_30bit = GetByteBitSupport(BlockData[5], 4);
                                Table.BlockHDMILLC.DC_Y444 = GetByteBitSupport(BlockData[5], 3);
                                Table.BlockHDMILLC.DVI_Dual = GetByteBitSupport(BlockData[5], 0);
                                if (Block.BlockPayload >= 6)
                                    Table.BlockHDMILLC.MaxTMDSClk = (uint)(BlockData[6] * 5);
                                if (Block.BlockPayload >= 7)
                                {
                                    Table.BlockHDMILLC.EnableFlag = Support.supported;
                                    Table.BlockHDMILLC.LatencyFieldsPresent = GetByteBitSupport(BlockData[7], 7);
                                    Table.BlockHDMILLC.ILatencyFieldsPresent = GetByteBitSupport(BlockData[7], 6);
                                    Table.BlockHDMILLC.HDMIVideoPresent = GetByteBitSupport(BlockData[7], 5);
                                    Table.BlockHDMILLC.CN3 = GetByteBitSupport(BlockData[7], 3);
                                    Table.BlockHDMILLC.CN2 = GetByteBitSupport(BlockData[7], 2);
                                    Table.BlockHDMILLC.CN1 = GetByteBitSupport(BlockData[7], 1);
                                    Table.BlockHDMILLC.CN0 = GetByteBitSupport(BlockData[7], 0);

                                    i = 8;
                                    if (Table.BlockHDMILLC.LatencyFieldsPresent == Support.supported)
                                    {
                                        Table.BlockHDMILLC.VideoLatency = BlockData[i];
                                        Table.BlockHDMILLC.AudioLatency = BlockData[i + 1];
                                        i += 2;
                                    }
                                    if (Table.BlockHDMILLC.ILatencyFieldsPresent == Support.supported)
                                    {
                                        Table.BlockHDMILLC.IVideoLatency = BlockData[i];
                                        Table.BlockHDMILLC.IAudioLatency = BlockData[i + 1];
                                        i += 2;
                                    }
                                    if (Table.BlockHDMILLC.HDMIVideoPresent == Support.supported)
                                    {
                                        Table.BlockHDMILLC.HDMI3DPresent = GetByteBitSupport(BlockData[i], 7);
                                        Table.BlockHDMILLC.HDMIVICLength = (uint)((BlockData[i + 1] & 0xE0) >> 5);
                                        Table.BlockHDMILLC.HDMI3DLength = (uint)(BlockData[i + 1] & 0x1F);
                                        i += 2;
                                    }
                                    if (Table.BlockHDMILLC.HDMIVICLength != 0)
                                    {
                                        int j;
                                        Table.BlockHDMILLC.HDMIVIC = new List<byte>();

                                        for (j = 0; j < Table.BlockHDMILLC.HDMIVICLength; j++)
                                        {
                                            Table.BlockHDMILLC.HDMIVIC.Add(BlockData[i + j]);
                                        }
                                    }
                                }
                                else
                                    Table.BlockHDMILLC.EnableFlag = Support.unsupported;
                            }
                            else
                                Table.BlockHDMILLC.ExtensionFields = Support.unsupported;
                            break;
                        case 0x00001A:
                            Block.Block = CEATagType.VS_AMD;
                            Table.BlockAMD.Version = BlockData[3];
                            if ((Table.BlockAMD.Version >= 1) && (Block.BlockPayload >= 8))
                            {
                                Table.BlockAMD.FreeSync = GetByteBitSupport(BlockData[4], 0);
                                Table.BlockAMD.NativeColorSpaceSet = GetByteBitSupport(BlockData[4], 1);
                                Table.BlockAMD.LocalDimmingControl = GetByteBitSupport(BlockData[4], 2);
                                Table.BlockAMD.MinRefreshRate = BlockData[5];
                                Table.BlockAMD.MaxRefreshRate = BlockData[6];
                                Table.BlockAMD.MCCSVCPCode = BlockData[7];
                            }
                            if ((Table.BlockAMD.Version >= 2) && (Block.BlockPayload >= 13))
                            {
                                Table.BlockAMD.Gamma22EOTF = GetByteBitSupport(BlockData[8], 2);
                                Table.BlockAMD.MaxBrightness_MaxBL = (float)(50 * Math.Pow(2, (double)(BlockData[9]) / 32));
                                Table.BlockAMD.MinBrightness_MaxBL = Table.BlockAMD.MaxBrightness_MaxBL * (float)(Math.Pow((double)BlockData[10] / 255, 2) / 100);
                                Table.BlockAMD.MaxBrightness_MinBL = (float)(50 * Math.Pow(2, (double)(BlockData[11]) / 32));
                                Table.BlockAMD.MinBrightness_MinBL = Table.BlockAMD.MaxBrightness_MinBL * (float)(Math.Pow((double)BlockData[12] / 255, 2) / 100);
                            }
                            if ((Table.BlockAMD.Version >= 3) && (Block.BlockPayload >= 18))
                            {
                                Table.BlockAMD.MaxRefreshRate255 = (uint)(BlockData[13] + (BlockData[14] & 0x03 << 8));
                            }
                            break;
                        case 0xC45DD8:
                            Block.Block = CEATagType.VS_HDMI_Forum;
                            Table.BlockHDMIForum.Version = BlockData[3];
                            Table.BlockHDMIForum.MaxTMDSRate = (uint)(BlockData[4] * 5);
                            Table.BlockHDMIForum.SCDC_Present = GetByteBitSupport(BlockData[5], 7);
                            Table.BlockHDMIForum.RR_Capable = GetByteBitSupport(BlockData[5], 6);
                            Table.BlockHDMIForum.CABLE_STATUS = GetByteBitSupport(BlockData[5], 5);
                            Table.BlockHDMIForum.CCBPCI = GetByteBitSupport(BlockData[5], 4);
                            Table.BlockHDMIForum.LTE_340Mcsc_scramble = GetByteBitSupport(BlockData[5], 3);
                            Table.BlockHDMIForum._3D_Independent_View = GetByteBitSupport(BlockData[5], 2);
                            Table.BlockHDMIForum._3D_Dual_View = GetByteBitSupport(BlockData[5], 1);
                            Table.BlockHDMIForum._3D_OSD_Disparity = GetByteBitSupport(BlockData[5], 0);

                            Table.BlockHDMIForum.UHD_VIC = GetByteBitSupport(BlockData[6], 3);
                            Table.BlockHDMIForum.DC_48bit_420 = GetByteBitSupport(BlockData[6], 2);
                            Table.BlockHDMIForum.DC_36bit_420 = GetByteBitSupport(BlockData[6], 1);
                            Table.BlockHDMIForum.DC_30bit_420 = GetByteBitSupport(BlockData[6], 0);
                            Table.BlockHDMIForum.FRLRate = (HDMIFRLType)((BlockData[6] & 0xF0) >> 4);

                            if (Block.BlockPayload >= 8)
                            {
                                Table.BlockHDMIForum.FAPA_start_location = GetByteBitSupport(BlockData[7], 0);
                                Table.BlockHDMIForum.ALLM = GetByteBitSupport(BlockData[7], 1);
                                Table.BlockHDMIForum.FVA = GetByteBitSupport(BlockData[7], 2);
                                Table.BlockHDMIForum.CNMVRR = GetByteBitSupport(BlockData[7], 3);
                                Table.BlockHDMIForum.CinemaVRR = GetByteBitSupport(BlockData[7], 4);
                                Table.BlockHDMIForum.M_Delta = GetByteBitSupport(BlockData[7], 5);
                            }
                            if (Block.BlockPayload >= 9)
                            {
                                //?BlockData[8]
                            }
                            if (Block.BlockPayload >= 10)
                            {
                                //?BlockData[9]
                            }
                            if ((Block.BlockPayload >= 11) && (Table.BlockHDMIForum.FRLRate != HDMIFRLType.Nosupport_FRL))
                            {
                                Table.BlockHDMIForum.DSC_10bpc = GetByteBitSupport(BlockData[10], 0);
                                Table.BlockHDMIForum.DSC_12bpc = GetByteBitSupport(BlockData[10], 1);
                                Table.BlockHDMIForum.DSC_16bpc = GetByteBitSupport(BlockData[10], 2);
                                Table.BlockHDMIForum.DSC_All_bpp = GetByteBitSupport(BlockData[10], 3);

                                Table.BlockHDMIForum.DSC_Native_42 = GetByteBitSupport(BlockData[10], 6);
                                Table.BlockHDMIForum.DSC_1p2 = GetByteBitSupport(BlockData[10], 7);
                                if (Block.BlockPayload >= 12)
                                {
                                    Table.BlockHDMIForum.DSCMaxSlices = (HDMIDSCMaxSlicesType)(BlockData[10] & 0x0F);
                                    Table.BlockHDMIForum.DSCMaxFRL = (HDMIDSCMaxFRLType)((BlockData[10] & 0xF0) >> 4);
                                }
                                if (Block.BlockPayload >= 12)
                                {
                                    Table.BlockHDMIForum.DSC_TotalChunkkBytes = (byte)(BlockData[11] & 0x3F);
                                }
                            }
                            break;
                        default:
                            Block.UnknowIEEEID = VSDB_IEEEID;
                            break;
                    }
                    break;

                case CEATagType.SpeakerAllocation:
                    Table.BlockSpeaker = new List<BlockSpeaker>();
                    BlockSpeaker Speaker = new BlockSpeaker();

                    for (i = 0; i < Block.BlockPayload / 3; i++)
                    {
                        Speaker.FLW_FRW = GetByteBitSupport(BlockData[i * 3], 7);
                        Speaker.RLC_RRC = GetByteBitSupport(BlockData[i * 3], 6);
                        Speaker.FLC_FRC = GetByteBitSupport(BlockData[i * 3], 5);
                        Speaker.BC = GetByteBitSupport(BlockData[i * 3], 4);
                        Speaker.BL_BR = GetByteBitSupport(BlockData[i * 3], 3);
                        Speaker.FC = GetByteBitSupport(BlockData[i * 3], 2);
                        Speaker.LFE = GetByteBitSupport(BlockData[i * 3], 1);
                        Speaker.FL_FR = GetByteBitSupport(BlockData[i * 3], 0);
                        Speaker.TpFC = GetByteBitSupport(BlockData[i * 3 + 1], 2);
                        Speaker.TpC = GetByteBitSupport(BlockData[i * 3 + 1], 1);
                        Speaker.TpFL_TpFH = GetByteBitSupport(BlockData[i * 3 + 1], 0);

                        Table.BlockSpeaker.Add(Speaker);
                    }
                    break;

                case CEATagType.VESADisplayTransferCharacteristic:
                    break;

                case CEATagType.Extended:
                    Block.Block = (CEATagType)(BlockData[0] + CEATagType.Ex_Video_Capability);
                    //复制有效数据
                    byte[] BlockExData = new byte[Block.BlockPayload - 1];
                    Array.Copy(Data, index + 2, BlockExData, 0, Block.BlockPayload - 1);

                    switch (Block.Block)
                    {
                        case CEATagType.Ex_Video_Capability:
                            Table.BlockVideoCapability.QY = GetByteBitSupport(BlockExData[0], 7);
                            Table.BlockVideoCapability.QS = GetByteBitSupport(BlockExData[0], 6);
                            Table.BlockVideoCapability.PT = (VideoCapabilityType)((BlockExData[0] & 0x30) >> 4);
                            Table.BlockVideoCapability.IT = (VideoCapabilityType)((BlockExData[0] & 0x0C) >> 2);
                            Table.BlockVideoCapability.CE = (VideoCapabilityType)(BlockExData[0] & 0x03);
                            break;

                        case CEATagType.Ex_VS_Video_Capability:
                            int VSVDB_IEEEID = BlockExData[0] + (BlockExData[1] << 8) + (BlockExData[2] << 16);
                            switch (VSVDB_IEEEID)
                            {
                                case 0x00D046:
                                    Block.Block = CEATagType.Ex_VS_Dolby_Version;
                                    break;
                                case 0x90848B:
                                    Block.Block = CEATagType.Ex_VS_HDR10Plus;
                                    break;
                                default:
                                    Block.UnknowIEEEID = VSVDB_IEEEID;
                                    break;
                            }
                            break;

                        case CEATagType.Ex_VESA_Display_Device:
                            break;

                        case CEATagType.Ex_VESA_Video_Timing:
                            break;

                        case CEATagType.Ex_HDMI_Video:
                            break;

                        case CEATagType.Ex_Colorimetry:
                            Table.BlockColorimetry.BT2020_RGB = GetByteBitSupport(BlockExData[0], 7);
                            Table.BlockColorimetry.BT2020_YCC = GetByteBitSupport(BlockExData[0], 6);
                            Table.BlockColorimetry.BT2020_cYCC = GetByteBitSupport(BlockExData[0], 5);
                            Table.BlockColorimetry.opRGB = GetByteBitSupport(BlockExData[0], 4);
                            Table.BlockColorimetry.opYCC601 = GetByteBitSupport(BlockExData[0], 3);
                            Table.BlockColorimetry.sYCC601 = GetByteBitSupport(BlockExData[0], 2);
                            Table.BlockColorimetry.xvYCC709 = GetByteBitSupport(BlockExData[0], 1);
                            Table.BlockColorimetry.xvYCC601 = GetByteBitSupport(BlockExData[0], 0);
                            Table.BlockColorimetry.DCI_P3 = GetByteBitSupport(BlockExData[1], 7);
                            Table.BlockColorimetry.MD3 = GetByteBitSupport(BlockExData[0], 3);
                            Table.BlockColorimetry.MD2 = GetByteBitSupport(BlockExData[0], 2);
                            Table.BlockColorimetry.MD1 = GetByteBitSupport(BlockExData[0], 1);
                            Table.BlockColorimetry.MD0 = GetByteBitSupport(BlockExData[0], 0);
                            break;

                        case CEATagType.Ex_HDR_Static_Matadata:
                            Table.BlockHDRStatic.Gamma_SDR = GetByteBitSupport(BlockExData[0], 0);
                            Table.BlockHDRStatic.Gamma_HDR = GetByteBitSupport(BlockExData[0], 1);
                            Table.BlockHDRStatic.SMPTE_ST_2084 = GetByteBitSupport(BlockExData[0], 2);
                            Table.BlockHDRStatic.HLG = GetByteBitSupport(BlockExData[0], 3);
                            Table.BlockHDRStatic.Static_Metadata_Type1 = GetByteBitSupport(BlockExData[1], 0);
                            if (Block.BlockPayload >= 4)
                                Table.BlockHDRStatic.Max_Luminance_Data = (float)(50 * Math.Pow(2, (double)(BlockExData[2]) / 32));
                            if (Block.BlockPayload >= 5)
                                Table.BlockHDRStatic.Max_Frame_Avg_Lum_Data = (float)(50 * Math.Pow(2, (double)(BlockExData[3]) / 32));
                            if (Block.BlockPayload >= 6)
                                Table.BlockHDRStatic.Min_Luminance_Data = Table.BlockHDRStatic.Max_Luminance_Data * (float)(Math.Pow((double)BlockExData[4] / 255, 2) / 100);
                            break;

                        case CEATagType.Ex_HDR_Dynamic_Matadata:
                            break;

                        case CEATagType.Ex_Video_Format_Preference:
                            break;

                        case CEATagType.Ex_YCbCr420Video:
                            break;

                        case CEATagType.Ex_YCbCr420CapabilityMap:
                            Table.BlockYCbCr420VIC = new List<BlockVideoVIC>();
                            for (i = 0; i < Block.BlockPayload - 1; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (GetByteBit(BlockExData[i], (byte)j) == 1)
                                    {
                                        if ((i * 8 + j) < Table.BlockVideoVIC.Count)
                                            Table.BlockYCbCr420VIC.Add(Table.BlockVideoVIC[i * 8 + j]);
                                    }
                                }
                            }
                            break;

                        case CEATagType.Ex_CEA_Miscellaneous_Audio_Fields:
                            break;

                        case CEATagType.Ex_VS_Audio:
                            break;

                        case CEATagType.Ex_HDMI_Audio:
                            break;

                        case CEATagType.Ex_Room_Configuration:
                            break;

                        case CEATagType.Ex_Speaker_Location:
                            break;

                        case CEATagType.Ex_Inframe:
                            break;

                        default:
                            Block.Block = CEATagType.Extended;
                            Block.UnknowExtendedCode = BlockData[0];
                            break;
                    }
                    break;

                case CEATagType.Reserved:
                case CEATagType.ReReserved:
                default:
                    break;
            }
            return Block;
        }
        internal DecodeError DecodeCEABlock()
        {
            Table = new CEATable();
            //01 Revision Number
            Table.Version = Data[1];
            if (Table.Version != 0x03)
                return DecodeError.CEAVersionError;

            //02 
            Table.DetailedTimingStart = Data[2];

            //03
            Table.UnderscranITFormatByDefault = GetByteBitSupport(Data[3], 7);
            Table.Audio = GetByteBitSupport(Data[3], 6);
            Table.YCbCr444 = GetByteBitSupport(Data[3], 5);
            Table.YCbCr422 = GetByteBitSupport(Data[3], 4);
            Table.NativeVideoFormatNumber = (byte)(Data[3] & 0x0F);

            //04-... CEA Data Blocks
            if (Table.DetailedTimingStart != 4)
            {
                int blockindex = 4;
                int blocknumber = 0;
                Table.CEABlocksList = new List<CEABlocksTable>();

                while (blockindex < Table.DetailedTimingStart)
                {
                    Table.CEABlocksList.Add(DecodeCEADataBlock(blockindex));

                    blockindex += Table.CEABlocksList[blocknumber].BlockPayload + 1;//Block Data length + Tag Code
                    blocknumber++;
                }
            }

            //Detailed Timing Blocks
            if (Table.DetailedTimingStart != 0)
            {
                int DetailedTimingindex = Table.DetailedTimingStart;
                byte[] TimingByte = new byte[18];
                Table.CEATimingList = new List<EDIDDetailedTimingTable>();

                while (Data[DetailedTimingindex] != 0x00 && (DetailedTimingindex + 18) < 127)
                {
                    Array.Copy(Data, DetailedTimingindex, TimingByte, 0, 18);
                    Table.CEATimingList.Add(DecodeDetailedTimingData(TimingByte));

                    DetailedTimingindex += 18;
                }
            }

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += Data[i];
            }
            if (checksum != 0x00)
                return DecodeError.CEAChecksumError;
            else
                Table.Checksum = Data[127];

            return DecodeError.Success;
        }
        internal DecompileError DecompileCEABlock()
        {
            Data = new byte[0];

            return DecompileError.Success;
        }
        private string OutputNotesCEABlocks(CEABlocksTable BlocksTable)
        {
            string Notes = "";
            int list_offset = 8;

            switch (BlocksTable.Block)
            {
                case CEATagType.Audio:
                    Notes += OutputNotesLineString("Audio Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    foreach (BlockAudio Audio in Table.BlockAudio)
                    {
                        Notes += OutputNotesLineString(list_offset, "Audio Format: {0}, Channel Number: {1}", 0, Audio.Type, Audio.Channels);
                        Notes += OutputNotesLineString(list_offset, "Sampling: Frequency: {0}{1}{2}{3}{4}{5}{6}", 0,
                            GetSupportString("192Khz,", Audio.Freq192Khz),
                            GetSupportString("176.4Khz,", Audio.Freq176_4Khz),
                            GetSupportString("96Khz,", Audio.Freq96Khz),
                            GetSupportString("88.2Khz,", Audio.Freq88_2Khz),
                            GetSupportString("48Khz,", Audio.Freq48Khz),
                            GetSupportString("44.1Khz,", Audio.Freq44_1Khz),
                            GetSupportString("32Khz,", Audio.Freq32Khz));
                        if (Audio.Type == AudioFormatType.L_PCM)
                        {
                            Notes += OutputNotesLineString(list_offset, "Sample: Size: {0}{1}{2}", 0,
                                GetSupportString("16 bit,", Audio.Size16Bit),
                                GetSupportString("20 bit,", Audio.Size20Bit),
                                GetSupportString("24 bit,", Audio.Size24Bit));
                        }
                    }
                    break;
                case CEATagType.Video:
                    Notes += OutputNotesLineString("Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    foreach (BlockVideoVIC VIC in Table.BlockVideoVIC)
                    {
                        Notes += OutputNotesLineString(list_offset, "{0} {1} ", 0, VICcode[VIC.VIC], GetSupportString("Native", VIC.NativeCode));
                    }
                    break;
                case CEATagType.SpeakerAllocation:
                    Notes += OutputNotesLineString("Speaker Allocation Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    foreach (BlockSpeaker Speaker in Table.BlockSpeaker)
                    {
                        Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", 0,
                            GetSupportString("Front Left/Right Wide,", Speaker.FLW_FRW),
                            GetSupportString("Rear Left/Right Center,", Speaker.RLC_RRC),
                            GetSupportString("Front Left/Right Center,", Speaker.FLC_FRC),
                            GetSupportString("Rear Center,", Speaker.BC),
                            GetSupportString("Rear Left/Right,", Speaker.BL_BR),
                            GetSupportString("Front Center,", Speaker.FC),
                            GetSupportString("Low Frequency Effect,", Speaker.LFE),
                            GetSupportString("Front Left/Right High,", Speaker.FL_FR),
                            GetSupportString("Front Center High,", Speaker.TpFC),
                            GetSupportString("Top Center,", Speaker.TpC),
                            GetSupportString("Front Left/Right High,", Speaker.TpFL_TpFH));
                    }
                    break;
                case CEATagType.VESADisplayTransferCharacteristic:
                    Notes += OutputNotesLineString("Display Transfer Characteristic Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;

                /* Vendor-Specific Data Block */
                case CEATagType.VendorSpecific:
                    Notes += OutputNotesLineString("unknow Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "IEEE ID: {0:X6}", 0, BlocksTable.UnknowIEEEID);
                    break;
                case CEATagType.VS_HDMI_LLC:
                    Notes += OutputNotesLineString("HDMI-LLC Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "Physical Address: 0x{0:X}{1:X}{2:X}{3:X}", 0, Table.BlockHDMILLC.PhyAddressA, Table.BlockHDMILLC.PhyAddressB, Table.BlockHDMILLC.PhyAddressC, Table.BlockHDMILLC.PhyAddressD);
                    if (Table.BlockHDMILLC.ExtensionFields == Support.supported)
                    {
                        Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}{5}", 0,
                            GetSupportString("Support All Feature,", Table.BlockHDMILLC.AllFeature),
                            GetSupportString("DC 48bit,", Table.BlockHDMILLC.DC_48bit),
                            GetSupportString("DC 36bit,", Table.BlockHDMILLC.DC_36bit),
                            GetSupportString("DC 30bit,", Table.BlockHDMILLC.DC_30bit),
                            GetSupportString("DC Y444,", Table.BlockHDMILLC.DC_Y444),
                            GetSupportString("DC Dual,", Table.BlockHDMILLC.DVI_Dual));
                        Notes += OutputNotesLineString(list_offset, "Max TMDS Clock: {0}MHz", 0, Table.BlockHDMILLC.MaxTMDSClk);
                    }
                    if (Table.BlockHDMILLC.EnableFlag == Support.supported)
                    {
                        if (Table.BlockHDMILLC.LatencyFieldsPresent == Support.supported)
                        {
                            Notes += OutputNotesLineString(list_offset, "Video Latency: {0}", 0, Table.BlockHDMILLC.VideoLatency);
                            Notes += OutputNotesLineString(list_offset, "Audio Latency: {0}", 0, Table.BlockHDMILLC.AudioLatency);
                        }
                        if (Table.BlockHDMILLC.ILatencyFieldsPresent == Support.supported)
                        {
                            Notes += OutputNotesLineString(list_offset, "I Video Latency: {0}", 0, Table.BlockHDMILLC.IVideoLatency);
                            Notes += OutputNotesLineString(list_offset, "I Audio Latency: {0}", 0, Table.BlockHDMILLC.IAudioLatency);
                        }
                        if (Table.BlockHDMILLC.HDMIVideoPresent == Support.supported)
                        {
                            if (Table.BlockHDMILLC.HDMI3DPresent == Support.supported)
                                Notes += OutputNotesLineString(list_offset, "Support 3D present,", 0);
                            if (Table.BlockHDMILLC.HDMIVICLength > 0)
                            {
                                Notes += OutputNotesLineString(list_offset, "HDMI VIC:", 0);
                                foreach (byte VIC in Table.BlockHDMILLC.HDMIVIC)
                                {
                                    Notes += OutputNotesLineString(list_offset, "{0}", 0, HDMIVICcode[VIC]);
                                }
                            }
                        }
                    }
                    break;
                case CEATagType.VS_AMD:
                    Notes += OutputNotesLineString("AMD Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "Version: {0}", 0, Table.BlockAMD.Version);
                    Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}", 0,
                        GetSupportString("FreeSync Supported, ", Table.BlockAMD.FreeSync),
                        GetSupportString("Native Color Space Set Supported, ", Table.BlockAMD.NativeColorSpaceSet),
                        GetSupportString("Local Dimming Control Supported, ", Table.BlockAMD.LocalDimmingControl),
                        GetSupportString("Camma 2.2 EOTF Supported, ", Table.BlockAMD.Gamma22EOTF)
                        );
                    Notes += OutputNotesLineString(list_offset, "Min Refresh Rate: {0}Hz", 0, Table.BlockAMD.MinRefreshRate);
                    Notes += OutputNotesLineString(list_offset, "Max Refresh Rate: {0}Hz", 0, Table.BlockAMD.MaxRefreshRate);
                    Notes += OutputNotesLineString(list_offset, "MCCS VCP Code: 0x{0:X2}", 0, Table.BlockAMD.MCCSVCPCode);
                    if (Table.BlockAMD.Version >= 2)
                    {
                        Notes += OutputNotesLineString(list_offset, "Maximum luminance (Max Backlight): {0:0.000} cd/m2", 0, Table.BlockAMD.MaxBrightness_MaxBL);
                        Notes += OutputNotesLineString(list_offset, "Minimum luminance (Max Backlight): {0:0.00000} cd/m2", 0, Table.BlockAMD.MinBrightness_MaxBL);
                        Notes += OutputNotesLineString(list_offset, "Maximum luminance (Min Backlight): {0:0.000} cd/m2", 0, Table.BlockAMD.MaxBrightness_MinBL);
                        Notes += OutputNotesLineString(list_offset, "Minimum luminance (Min Backlight): {0:0.00000} cd/m2", 0, Table.BlockAMD.MinBrightness_MinBL);
                    }
                    if (Table.BlockAMD.Version >= 3)
                    {
                        Notes += OutputNotesLineString(list_offset, "Version 3 Max Refresh Rate: {0}Hz", 0, Table.BlockAMD.MaxRefreshRate255);
                    }
                    break;
                case CEATagType.VS_HDMI_Forum:
                    Notes += OutputNotesLineString("HDMI-forum Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "Version: {0}", 0, Table.BlockHDMIForum.Version);
                    Notes += OutputNotesLineString(list_offset, "Max TMDS Rate: {0} MHz", 0, Table.BlockHDMIForum.MaxTMDSRate);
                    Notes += OutputNotesLineString(list_offset, "Support:{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}", 0,
                        GetSupportString("3D OSD Disparity,", Table.BlockHDMIForum._3D_OSD_Disparity),
                        GetSupportString("3D Dual View,", Table.BlockHDMIForum._3D_Dual_View),
                        GetSupportString("3D Independent View,", Table.BlockHDMIForum._3D_Independent_View),
                        GetSupportString("LTE 340Mcsc scramble,", Table.BlockHDMIForum.LTE_340Mcsc_scramble),
                        GetSupportString("CCBPCI,", Table.BlockHDMIForum.CCBPCI),
                        GetSupportString("CABLE STSTUS,", Table.BlockHDMIForum.CABLE_STATUS),
                        GetSupportString("RR Cabpable,", Table.BlockHDMIForum.RR_Capable),
                        GetSupportString("SCDC Present,", Table.BlockHDMIForum.SCDC_Present),
                        GetSupportString("DC 30bit 420,", Table.BlockHDMIForum.DC_30bit_420),
                        GetSupportString("DC 36bit 420,", Table.BlockHDMIForum.DC_36bit_420),
                        GetSupportString("DC 48bit 420,", Table.BlockHDMIForum.DC_48bit_420),
                        GetSupportString("UHD VIC,", Table.BlockHDMIForum.UHD_VIC),
                        GetSupportString("FAPA start location,", Table.BlockHDMIForum.FAPA_start_location),
                        GetSupportString("ALLM,", Table.BlockHDMIForum.ALLM),
                        GetSupportString("FVA,", Table.BlockHDMIForum.FVA),
                        GetSupportString("CNMVRR,", Table.BlockHDMIForum.CNMVRR),
                        GetSupportString("CinemaVRR,", Table.BlockHDMIForum.CinemaVRR),
                        GetSupportString("MDelta,", Table.BlockHDMIForum.M_Delta));
                    Notes += OutputNotesLineString(list_offset, "Max FRL Rate: {0}", 0, Table.BlockHDMIForum.FRLRate);
                    if ((Table.BlockHDMIForum.VRRMin != 0) || (Table.BlockHDMIForum.VRRMax != 0))
                        Notes += OutputNotesLineString(list_offset, "VRRMin: {0} VRRMax: {1}", 0, Table.BlockHDMIForum.VRRMin, Table.BlockHDMIForum.VRRMax);
                    if (Table.BlockHDMIForum.FRLRate != HDMIFRLType.Nosupport_FRL)
                    {
                        Notes += OutputNotesLineString(list_offset, "DSC Support:{0}{1}{2}{3}{4}{5}", 0,
                            GetSupportString("10bpc,", Table.BlockHDMIForum.DSC_10bpc),
                            GetSupportString("12bpc,", Table.BlockHDMIForum.DSC_12bpc),
                            GetSupportString("16bpc,", Table.BlockHDMIForum.DSC_16bpc),
                            GetSupportString("All bpp,", Table.BlockHDMIForum.DSC_All_bpp),
                            GetSupportString("Native 42,", Table.BlockHDMIForum.DSC_Native_42),
                            GetSupportString("1p2,", Table.BlockHDMIForum.DSC_1p2));
                        Notes += OutputNotesLineString(list_offset, "DSC MaxSlices: {0}", 0, Table.BlockHDMIForum.DSCMaxSlices);
                        Notes += OutputNotesLineString(list_offset, "DSC Max FRL Rate: {0}", 0, Table.BlockHDMIForum.DSCMaxFRL);
                        Notes += OutputNotesLineString(list_offset, "DSC TotalChunkkBytes: {0}", 0, Table.BlockHDMIForum.DSC_TotalChunkkBytes);
                    }
                    break;

                /* Extended */
                case CEATagType.Extended:
                    Notes += OutputNotesLineString("unknow Extended Tag Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString("Extended Tag Code: {0:X2}", 0, BlocksTable.UnknowExtendedCode);
                    break;
                case CEATagType.Ex_Video_Capability:
                    Notes += OutputNotesLineString("Video Capability Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "CE: {0}", 0, Table.BlockVideoCapability.CE);
                    Notes += OutputNotesLineString(list_offset, "IT: {0}", 0, Table.BlockVideoCapability.IT);
                    Notes += OutputNotesLineString(list_offset, "PT: {0}", 0, Table.BlockVideoCapability.PT);
                    Notes += OutputNotesLineString(list_offset, "RGB Quantization Range: {0}", 0, Table.BlockVideoCapability.QS == Support.supported ? "Selectable (via AVI Q)" : "No Data");
                    Notes += OutputNotesLineString(list_offset, "YCC Quantization Range: {0}", 0, Table.BlockVideoCapability.QY == Support.supported ? "Selectable (via AVI YQ)" : "No Data");
                    break;
                case CEATagType.Ex_VESA_Display_Device:
                    Notes += OutputNotesLineString("Display Device Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_VESA_Video_Timing:
                    Notes += OutputNotesLineString("Video Timing Block Extension, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_HDMI_Video:
                    Notes += OutputNotesLineString("HDMI Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_Colorimetry:
                    Notes += OutputNotesLineString("Colorimetry Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", 0,
                        GetSupportString("BT.2020 RGB,", Table.BlockColorimetry.BT2020_RGB),
                        GetSupportString("BT.2020 YCC,", Table.BlockColorimetry.BT2020_YCC),
                        GetSupportString("BT.2020 cYCC,", Table.BlockColorimetry.BT2020_cYCC),
                        GetSupportString("Adobe RGB,", Table.BlockColorimetry.opRGB),
                        GetSupportString("Adobe YCC-601,", Table.BlockColorimetry.opYCC601),
                        GetSupportString("sYCC-601,", Table.BlockColorimetry.sYCC601),
                        GetSupportString("xvYCC-709,", Table.BlockColorimetry.xvYCC709),
                        GetSupportString("xvYCC-601,", Table.BlockColorimetry.xvYCC601),
                        GetSupportString("DCI-P3,", Table.BlockColorimetry.DCI_P3),
                        GetSupportString("MD3,", Table.BlockColorimetry.MD3),
                        GetSupportString("MD2,", Table.BlockColorimetry.MD2),
                        GetSupportString("MD1,", Table.BlockColorimetry.MD1),
                        GetSupportString("MD0,", Table.BlockColorimetry.MD0));
                    break;
                case CEATagType.Ex_HDR_Static_Matadata:
                    Notes += OutputNotesLineString("HDR Static Matadata Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}", 0,
                        GetSupportString("Gamma-SDR,", Table.BlockHDRStatic.Gamma_SDR),
                        GetSupportString("Gamma-HDR,", Table.BlockHDRStatic.Gamma_HDR),
                        GetSupportString("EOTF SMPTE ST 2084,", Table.BlockHDRStatic.SMPTE_ST_2084),
                        GetSupportString("EOTF HLG,", Table.BlockHDRStatic.HLG),
                        GetSupportString("Static MetaData Type 1,", Table.BlockHDRStatic.Static_Metadata_Type1));
                    if (Table.BlockHDRStatic.Max_Luminance_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Max Luminance: {0:0.000} cd/m2", 0, Table.BlockHDRStatic.Max_Luminance_Data);
                    if (Table.BlockHDRStatic.Max_Frame_Avg_Lum_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Max Frame-average Luminance: {0:0.000} cd/m2", 0, Table.BlockHDRStatic.Max_Frame_Avg_Lum_Data);
                    if (Table.BlockHDRStatic.Min_Luminance_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Min Luminance: {0:0.00000} cd/m2", 0, Table.BlockHDRStatic.Min_Luminance_Data);
                    break;
                case CEATagType.Ex_HDR_Dynamic_Matadata:
                    Notes += OutputNotesLineString("HDR Dynamic Matadata Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_Video_Format_Preference:
                    Notes += OutputNotesLineString("Video Format Preference Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_YCbCr420Video:
                    Notes += OutputNotesLineString("YCBCR 4:2:0 Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_YCbCr420CapabilityMap:
                    Notes += OutputNotesLineString("YCBCR 4:2:0 Capability Map Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    foreach (BlockVideoVIC VIC in Table.BlockYCbCr420VIC)
                    {
                        Notes += OutputNotesLineString(list_offset, "{0}", 0, VICcode[VIC.VIC]);
                    }
                    break;
                case CEATagType.Ex_CEA_Miscellaneous_Audio_Fields:
                    Notes += OutputNotesLineString("CTA Miscellaneous Audio Fields, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_VS_Audio:
                    Notes += OutputNotesLineString("Vendor-Specific Audio Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_HDMI_Audio:
                    Notes += OutputNotesLineString("HDMI Audio Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_Room_Configuration:
                    Notes += OutputNotesLineString("Room Configuration Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_Speaker_Location:
                    Notes += OutputNotesLineString("Speaker Location Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_Inframe:
                    Notes += OutputNotesLineString("InfoFrame Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;

                /* Vendor-Specific Video Data Block */
                case CEATagType.Ex_VS_Video_Capability:
                    Notes += OutputNotesLineString("unknow Vendor-Specific Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "IEEE ID: {0:X6}", 0, BlocksTable.UnknowIEEEID);
                    break;
                case CEATagType.Ex_VS_Dolby_Version:
                    Notes += OutputNotesLineString("Dolby Version Vendor-Specific Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case CEATagType.Ex_VS_HDR10Plus:
                    Notes += OutputNotesLineString("HDR10+ Vendor-Specific Video Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                default:
                    break;
            }

            Notes += "\r\n";
            return Notes;
        }
        internal string OutputNotesEDIDCEA()
        {
            int i;
            string NoteEDID = "\r\nBlock Type: CTA Extension Data(CTA-861-G)\r\n";
            NoteEDID += OutputNotesEDIDList(Data);
            NoteEDID += OutputNotesLineString("(01) CEA Version: {0}", 0, Table.Version);
            NoteEDID += OutputNotesListString("(02) General Info:", 8,
                GetSupportString("Support Underscran", Table.UnderscranITFormatByDefault),
                GetSupportString("Support Audio", Table.Audio),
                GetSupportString("Support YCbCr 4:4:4", Table.YCbCr444),
                GetSupportString("Support YCbCr 4:2:2", Table.YCbCr422),
                "Native Format: " + Table.NativeVideoFormatNumber.ToString()
                );
            NoteEDID += OutputNotesLineString("(03) Detailed Timing Start: {0}", 0, Table.DetailedTimingStart);

            i = 4;
            if (Table.DetailedTimingStart != 4)
                foreach (CEABlocksTable Table in Table.CEABlocksList)
                {
                    NoteEDID += "(" + string.Format("{0:D2}", i) + "-" + string.Format("{0:D2}", i + Table.BlockPayload) + ") " + OutputNotesCEABlocks(Table);
                    i += Table.BlockPayload + 1;
                }

            int TimingNumber = 0;
            if (Table.DetailedTimingStart != 0)
                foreach (EDIDDetailedTimingTable Timing in Table.CEATimingList)
                {
                    TimingNumber++;
                    NoteEDID += "______________________________________________________________________\r\n";
                    NoteEDID += "(" + string.Format("{0:D2}", i.ToString()) + "-" + string.Format("{0:D2}", i + 17) + ")" + " Detailed Timing " + TimingNumber.ToString() + ":\r\n\r\n" + OutputNotesDetailedTiming(Timing);
                    i += 18;
                }

            if (i != 127)
            {
                NoteEDID += "\r\n(" + string.Format("{0:D2}", i.ToString()) + "-" + 126.ToString() + ") No data";
            }
            NoteEDID += OutputNotesLineString("\r\n(127) CheckSum: OK", 0);

            return NoteEDID;
        }
    }
    /************************** DisplayID **********************/
    #region
    struct DisplayIDTable
    {
        public byte Version;
        public byte SectionSize;
        public ProductType Type;
        public byte ExCount;
        public List<DisplayIDBlocksTable> DisplayIDBlocksList;

        public List<TypeIDetailedTiming> TypeIDetailTimingList;

        public byte Checksum;
    };
    enum ProductType
    {
        Extension,
    }
    enum DisplayIDTagType
    {
        ProductIdentification,
        DisplayParameters,
        Color,
        TypeI,   //Detailed
        TypeII,  //Detailed
        TypeIII, //Short
        TypeIV,  //DMT ID Code
        VESATiming,
        CEATiming,
        RangeLimits,
        SN,
        ASCII,
        DisplayDevice,
        InterfacePowerSequencing,
        TransferCharacteristics,
        DisplayInterface,
        StereoDisplayInterface,
        TypeIII_11h,
        TypeIII_12h,
        TypeII_13h,

        VS = 0x7F,

        Reserved = 0x80,

        CEA_Reserved,
    }
    enum _3DStereoType
    {
        no_stereo,
        stereo,
        mono_or_stereo,
        Reserved,
    }
    enum AspectRatioType
    { 
        _1_1,
        _5_4,
        _4_3,
        _15_9,
        _16_9,
        _16_10,
        _64_27,
        _256_135,
        No_defined,
        Reserved,
    }
    struct DisplayIDBlocksTable
    {
        public DisplayIDTagType Block;
        public byte BlockRevision;
        public int BlockPayload;
    }
    struct TypeIDetailedTiming
    {
        public uint PixelClk; //Hz
        public Support Preferred;
        public HVSyncType HSync;
        public HVSyncType VSync;
        public _3DStereoType Stereo;
        public InterfaceType Interface;
        public AspectRatioType Ratio;

        public uint HFrequency; //Hz
        public uint HAdressable;
        public uint HBlanking;
        public uint HSyncFront;
        public uint HSyncWidth;

        public float VFrequency; //Hz
        public uint VAdressable;
        public uint VBlanking;
        public uint VSyncFront;
        public uint VSyncWidth;
    }
    #endregion
    internal class EDIDDisplayID : EDIDCommon
    {
        internal DisplayIDTable Table;
        internal byte[] Data;
        int i;
        private DisplayIDBlocksTable DecodeDisplayIDDataBlock(int index)
        {
            DisplayIDBlocksTable Block = new DisplayIDBlocksTable();
            Block.Block = (DisplayIDTagType)Data[index];
            Block.BlockRevision = Data[index + 1];
            Block.BlockPayload = Data[index + 2];
            byte[] BlockData = new byte[Block.BlockPayload];
            Array.Copy(Data, index + 3, BlockData, 0, Block.BlockPayload);

            switch (Block.Block)
            {
                case DisplayIDTagType.ProductIdentification:
                    break;
                case DisplayIDTagType.DisplayParameters:
                    break;
                case DisplayIDTagType.Color:
                    break;
                case DisplayIDTagType.TypeI:
                    Table.TypeIDetailTimingList = new List<TypeIDetailedTiming>();
                    for (i = 0; i < Block.BlockPayload / 0x14; i++)
                    {
                        TypeIDetailedTiming Timing = new TypeIDetailedTiming();
                        Timing.PixelClk = (uint)(BlockData[i * 0x14] + (BlockData[i * 0x14 + 1] << 8) + (BlockData[i * 0x14 + 2] << 16) + 1) * 10000;
                        Timing.Preferred = GetByteBitSupport(BlockData[i * 0x14 + 3], 7);
                        Timing.Stereo = (_3DStereoType)((BlockData[i * 0x14 + 3] & 0x60) >> 5);
                        Timing.Interface = (InterfaceType)GetByteBit(BlockData[i * 0x14 + 3], 4);
                        Timing.Ratio = (AspectRatioType)(BlockData[i * 0x14 + 3] & 0x0F);

                        Timing.HAdressable = (uint)(BlockData[i * 0x14 + 4] + (BlockData[i * 0x14 + 5] << 8) + 1);
                        Timing.HBlanking = (uint)(BlockData[i * 0x14 + 6] + (BlockData[i * 0x14 + 7] << 8) + 1);
                        Timing.HSyncFront = (uint)(BlockData[i * 0x14 + 8] + ((BlockData[i * 0x14 + 9] & 0x7F) << 8) + 1);
                        Timing.HSync = (HVSyncType)GetByteBit(BlockData[i * 0x14 + 9], 7);
                        Timing.HSyncWidth = (uint)(BlockData[i * 0x14 + 10] + (BlockData[i * 0x14 + 11] << 8) + 1);

                        Timing.VAdressable = (uint)(BlockData[i * 0x14 + 12] + (BlockData[i * 0x14 + 13] << 8) + 1);
                        Timing.VBlanking = (uint)(BlockData[i * 0x14 + 14] + (BlockData[i * 0x14 + 15] << 8) + 1);
                        Timing.VSyncFront = (uint)(BlockData[i * 0x14 + 16] + ((BlockData[i * 0x14 + 17] & 0x7F) << 8) + 1);
                        Timing.VSync = (HVSyncType)GetByteBit(BlockData[i * 0x14 + 17], 7);
                        Timing.VSyncWidth = (uint)(BlockData[i * 0x14 + 18] + (BlockData[i * 0x14 + 19] << 8) + 1);

                        if ((Timing.PixelClk != 0) && (Timing.HAdressable != 0) && (Timing.VAdressable != 0))
                        {
                            Timing.HFrequency = (uint)(Timing.PixelClk / (Timing.HAdressable + Timing.HBlanking));
                            Timing.VFrequency = (float)Timing.HFrequency / (Timing.VAdressable + Timing.VBlanking);
                        }

                        Table.TypeIDetailTimingList.Add(Timing);
                    }
                    break;
                case DisplayIDTagType.TypeII:
                    break;
                case DisplayIDTagType.TypeIII:
                    break;
                case DisplayIDTagType.TypeIV:
                    break;
                case DisplayIDTagType.VESATiming:
                    break;
                case DisplayIDTagType.CEATiming:
                    break;
                case DisplayIDTagType.RangeLimits:
                    break;
                case DisplayIDTagType.SN:
                    break;
                case DisplayIDTagType.ASCII:
                    break;
                case DisplayIDTagType.DisplayDevice:
                    break;
                case DisplayIDTagType.InterfacePowerSequencing:
                    break;
                case DisplayIDTagType.TransferCharacteristics:
                    break;
                case DisplayIDTagType.DisplayInterface:
                    break;
                case DisplayIDTagType.StereoDisplayInterface:
                    break;
                case DisplayIDTagType.TypeIII_11h:
                    break;
                case DisplayIDTagType.TypeIII_12h:
                    break;
                case DisplayIDTagType.TypeII_13h:
                    break;
                case DisplayIDTagType.VS:
                    break;
                case DisplayIDTagType.CEA_Reserved:
                    break;
                
                case DisplayIDTagType.Reserved:
                default:
                    break;
            }
            return Block;
        }
        internal DecodeError DecodeDisplayIDBlock()
        {
            Table = new DisplayIDTable();

            Table.Version = Data[1];
            if (Table.Version != 0x12)
                return DecodeError.DisplayIDVersionError;

            Table.SectionSize = Data[2];

            Table.Type = (ProductType)Data[3];
            if(Table.Type != ProductType.Extension)
                return DecodeError.DisplayIDTypeError;

            Table.ExCount = Data[4]; // Not use in Ex type

            int blockIndex = 5;
            if (Data[blockIndex + 2] != 0x00)
            {
                Table.DisplayIDBlocksList = new List<DisplayIDBlocksTable>();
                int blocknumber = 0;
                while ((Data[blockIndex + 2] != 0x00) && (blockIndex < Table.SectionSize - 4)) // BlockPayload != 0
                {
                    Console.WriteLine("{0}", blockIndex);
                    Table.DisplayIDBlocksList.Add(DecodeDisplayIDDataBlock(blockIndex));

                    blockIndex += Table.DisplayIDBlocksList[blocknumber].BlockPayload + 3;//Block Data length + Tag Code + Revision + Payload
                    blocknumber++;
                }
            }

            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += Data[i];
            }
            if (checksum != 0x00)
                return DecodeError.DisplayIDChecksumError;
            else
                Table.Checksum = Data[127];
            return DecodeError.Success;
        }
        internal DecompileError DecompileDisplayIDBlock()
        {
            Data = new byte[0];
            return DecompileError.Success;
        }
        private string OutputNotesDisplayIDBlocks(DisplayIDBlocksTable BlocksTable)
        {
            string Notes = "";
            int list_offset = 8;
            int list_offset2 = 50;
            switch (BlocksTable.Block)
            {
                case DisplayIDTagType.ProductIdentification:
                    Notes += OutputNotesLineString("Product Identification Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.DisplayParameters:
                    Notes += OutputNotesLineString("Display Parameters Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.Color:
                    Notes += OutputNotesLineString("Color Characteristics, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeI:
                    string[] DetailTimingHSyncType = { "Horizontal Polarity (-),", "Horizontal Polarity (+),", "" };
                    string[] DetailTimingVSyncType = { "Vertical Polarity (-)", "Vertical Polarity (+)", "" };
                    string[] DetailTimingRatioType = { "1:1", "5:4", "4:3", "15:9", "16:9", "16:10", "64:27", "256:135", "No define", "Reserved" };
                    i = 0;
                    Notes += OutputNotesLineString("Type I Timing - Detail, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    foreach (TypeIDetailedTiming Timing in Table.TypeIDetailTimingList)
                    {
                        Notes += "\r\n";
                        Notes += OutputNotesLineString("Timing {0}:", 0, i + 1);
                        Notes += OutputNotesLineString(list_offset, "{0}x{1}@{2:.00}Hz   Pixel Clock: {3:.00} MHz", 0, Timing.HAdressable, Timing.VAdressable, Timing.VFrequency, (float)Timing.PixelClk / 1000000);
                        Notes += OutputNotesListsString("Ratio: {0}", list_offset, DetailTimingRatioType[(int)Timing.Ratio], "{0}", list_offset2, GetSupportString("Preferred", Timing.Preferred));
                        Notes += OutputNotesListsString("Refreshed Mode: {0}", list_offset, Timing.Interface, "Normal Display: {0}", list_offset2, Timing.Stereo);
                        Notes += "\r\n";
                        Notes += OutputNotesLineString(list_offset, "Horizontal:", 0);
                        Notes += OutputNotesListsString("Active Time: {0} pixels", list_offset, Timing.HAdressable, "Blanking Time: {0} pixels", list_offset2, Timing.HBlanking);
                        Notes += OutputNotesListsString("Sync Offset: {0} pixels", list_offset, Timing.HSyncFront, "Sync Pulse Width: {0} pixels", list_offset2, Timing.HSyncWidth);
                        Notes += OutputNotesLineString(list_offset2, "Frequency: {0:.00} Khz", 0, (float)Timing.HFrequency / 1000);
                        Notes += OutputNotesLineString(list_offset, "Vertical:", 0);
                        Notes += OutputNotesListsString("Active Time: {0} Lines", list_offset, Timing.VAdressable, "Blanking Time: {0} Lines", list_offset2, Timing.VBlanking);
                        Notes += OutputNotesListsString("Sync Offset: {0} Lines", list_offset, Timing.VSyncFront, "Sync Pulse Width: {0} Lines", list_offset2, Timing.VSyncWidth);
                        Notes += OutputNotesLineString(list_offset2, "Frequency: {0:.00} Hz", 0, Timing.VFrequency);
                        Notes += OutputNotesLineString(list_offset, "{0}{1}", 0,DetailTimingHSyncType[(int)Timing.HSync],DetailTimingVSyncType[(int)Timing.VSync]);
                        i++;
                    }
                    break;
                case DisplayIDTagType.TypeII:
                    Notes += OutputNotesLineString("Type II Timing - Detail, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeIII:
                    Notes += OutputNotesLineString("Type III Timing - Short, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeIV:
                    Notes += OutputNotesLineString("Type IV Timing–DMT ID Code, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.VESATiming:
                    Notes += OutputNotesLineString("VESA Timing Standard, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.CEATiming:
                    Notes += OutputNotesLineString("CEA Timing Standard, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.RangeLimits:
                    Notes += OutputNotesLineString("Video Timing Range Limits, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.SN:
                    Notes += OutputNotesLineString("Product Serial Number, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.ASCII:
                    Notes += OutputNotesLineString("General Purpose ASCII String, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.DisplayDevice:
                    Notes += OutputNotesLineString("Display Device Data, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.InterfacePowerSequencing:
                    Notes += OutputNotesLineString("Interface Power Sequencing Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TransferCharacteristics:
                    Notes += OutputNotesLineString("Transfer Characteristics Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.DisplayInterface:
                    Notes += OutputNotesLineString("Display Interface Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.StereoDisplayInterface:
                    Notes += OutputNotesLineString("Stereo Display Interface Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeIII_11h:
                    Notes += OutputNotesLineString("Type III Timing - Short, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeIII_12h:
                    Notes += OutputNotesLineString("Type III Timing - Short, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.TypeII_13h:
                    Notes += OutputNotesLineString("Type II Timing - Detailed, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.VS:
                    Notes += OutputNotesLineString("Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;
                case DisplayIDTagType.CEA_Reserved:
                    Notes += OutputNotesLineString("CEA-defined Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;

                case DisplayIDTagType.Reserved:
                default:
                    Notes += OutputNotesLineString("Unknow Data Block, Number of Data Byte to Follow: {0}", 0, BlocksTable.BlockPayload);
                    break;          
            }

            Notes += "\r\n";
            return Notes;
        }
        internal string OutputNotesEDIDDisplay()
        {
            string NoteEDID = "\r\nBlock Type: Display Identification Data\r\n";

            NoteEDID += OutputNotesEDIDList(Data);
            NoteEDID += OutputNotesLineString("(01) Version:      0x{0:X2}", 0, Table.Version);
            NoteEDID += OutputNotesLineString("(02) Section Size: {0}", 0, Table.SectionSize);
            NoteEDID += OutputNotesLineString("(03) Type:         {0}", 0, Table.Type);
            NoteEDID += OutputNotesLineString("(04) Ex Count:     {0}", 0, Table.ExCount);

            int i = 5;
            foreach (DisplayIDBlocksTable Table in Table.DisplayIDBlocksList)
            {
                NoteEDID += "______________________________________________________________________\r\n";
                NoteEDID += "(" + string.Format("{0:D2}", i) + "-" + string.Format("{0:D2}", i + Table.BlockPayload + 2) + ") " + OutputNotesDisplayIDBlocks(Table);
                i += Table.BlockPayload + 3;
            }

            if (i != 127)
            {
                NoteEDID += "(" + string.Format("{0:D2}", i.ToString()) + "-" + 126.ToString() + ") No data";
            }
            NoteEDID += OutputNotesLineString("\r\n(127) CheckSum: OK", 0);

            return NoteEDID;
        }
    }
    /************************** EDID ***************************/
    #region
    struct EDIDTable
    {
        public byte[] Data;
        public uint Length;
        public DecodeError Error;
        public DecompileError R_Error;

        public List<BlockTagType> List;

        public BaseTable Base;
        public CEATable CEA;
        public DisplayIDTable DisplayID;
    }
    #endregion
    internal class EDID : EDIDCommon
    {
        private byte[] MatchOriginalText(string Text)
        {
            string EDIDText = "";
            uint Length = 0;
            byte[] Data = new byte[0];

            MatchCollection mcText1 = Regex.Matches(Text, @"\|  ([0-9]|[A-Z])([0-9]|[A-Z])  \w\w  \w\w  \w\w  \w\w  \w\w  \w\w  \w\w((  \w\w  \w\w)|\s)");//厂内格式

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
            else
            {
                MatchCollection mcText = Regex.Matches(Text, @"([0-9]|[A-Z])([0-9]|[A-Z])");//0x.. format

                foreach (Match m in mcText)
                {
                    EDIDText += m.ToString();
                    EDIDText += " ";
                    Length++;
                }
                Console.WriteLine(EDIDText);
                Console.WriteLine("EDID Length: {0}", Length.ToString());
            }

            if (Length != 0)
            {
                Data = new byte[Length];
                uint i = 0;
                MatchCollection mcText = Regex.Matches(EDIDText, @"([0-9]|[A-Z])([0-9]|[A-Z])");

                foreach (Match m in mcText)
                {
                    Data[i] = byte.Parse(m.ToString(), System.Globalization.NumberStyles.HexNumber);
                    i++;
                }
            }
            return Data;
        }
        public EDIDTable Decode(string Text)
        {
           return Decode(MatchOriginalText(Text));
        }
        public EDIDTable Decode(byte[] Data)
        {
            EDIDTable EDIDInfo = new EDIDTable();
            EDIDInfo.Data = Data;
            EDIDInfo.Length = (uint)EDIDInfo.Data.Length;

            if (EDIDInfo.Length < 128) 
            {
                EDIDInfo.Error = DecodeError.LengthError;
                return EDIDInfo;
            }
            byte[] BlockData = new byte[128];

            // decode Base Block 
            Array.Copy(EDIDInfo.Data, 0, BlockData, 0, 128);
            EDIDBase EDIDBase = new EDIDBase() { Data = BlockData };
            EDIDInfo.Error = EDIDBase.DecodeBaseBlock();
            if (EDIDInfo.Error == DecodeError.Success)
            {
                EDIDInfo.List = new List<BlockTagType>();
                EDIDInfo.List.Add(BlockTagType.Base);
                EDIDInfo.Base = EDIDBase.Table;
            }
            else
                return EDIDInfo;

            // decode Extension Block
            if (EDIDInfo.Base.ExBlockCount > 0)
            {
                uint BlockNumber = EDIDInfo.Base.ExBlockCount;
                uint Index = 128;
                for (; BlockNumber > 0; BlockNumber--)
                {
                    if (Index >= EDIDInfo.Length)
                        break;

                    switch (EDIDInfo.Data[Index])
                    {
                        case 0x02:
                            Array.Copy(EDIDInfo.Data, Index, BlockData, 0, 128);
                            EDIDCEA EDIDCEA = new EDIDCEA() { Data = BlockData };
                            EDIDInfo.Error = EDIDCEA.DecodeCEABlock();
                            if (EDIDInfo.Error == DecodeError.Success)
                            {
                                EDIDInfo.List.Add(BlockTagType.CEA);
                                EDIDInfo.CEA = EDIDCEA.Table;
                                Index += 128;
                            }
                            else
                                return EDIDInfo;
                            break;

                        case 0x70:
                            Array.Copy(EDIDInfo.Data, Index, BlockData, 0, 128);
                            EDIDDisplayID EDIDDisplayID = new EDIDDisplayID() { Data = BlockData };
                            EDIDInfo.Error = EDIDDisplayID.DecodeDisplayIDBlock();
                            if (EDIDInfo.Error == DecodeError.Success)
                            {
                                EDIDInfo.List.Add(BlockTagType.DisplayID);
                                EDIDInfo.DisplayID = EDIDDisplayID.Table;
                                Index += 128;
                            }
                            else
                                return EDIDInfo;
                            break;

                        default:
                            break;
                    }
                }
            }

            return EDIDInfo;
        }
        public (byte[], DecompileError) Decompile(BaseTable Base)
        {
            DecompileError Error;
            EDIDBase EDIDBase = new EDIDBase() { Table = Base };

            Error = EDIDBase.DecompileBaseBlock();

            return (EDIDBase.Data, Error);
        }
        public (byte[], DecompileError) Decompile(CEATable CEA)
        {
            DecompileError Error;
            EDIDCEA EDIDCEA = new EDIDCEA() { Table = CEA };

            Error = EDIDCEA.DecompileCEABlock();

            return (EDIDCEA.Data, Error);
        }
        public (byte[], DecompileError) Decompile(DisplayIDTable DisplayID)
        {
            DecompileError Error;
            EDIDDisplayID EDIDDisplayID = new EDIDDisplayID() { Table = DisplayID };

            Error = EDIDDisplayID.DecompileDisplayIDBlock();

            return (EDIDDisplayID.Data, Error);
        }
        public EDIDTable Decompile(EDIDTable EDIDInfo)
        {
            var ResultBase = Decompile(EDIDInfo.Base);
            var ResultCEA = Decompile(EDIDInfo.CEA);
            var ResultDisplayID = Decompile(EDIDInfo.DisplayID);

            EDIDInfo.Data = new byte[ResultBase.Item1.Length + ResultCEA.Item1.Length + ResultDisplayID.Item1.Length];

            if (ResultBase.Item2 == DecompileError.Success)
            {
                ResultBase.Item1.CopyTo(EDIDInfo.Data, 0);
            }
            else if(ResultBase.Item2 != DecompileError.Success)
            {
                EDIDInfo.R_Error = ResultBase.Item2;
                return EDIDInfo;
            }

            if (ResultCEA.Item2 != DecompileError.NoDecompile)
            {
                if (ResultCEA.Item2 == DecompileError.Success)
                    ResultCEA.Item1.CopyTo(EDIDInfo.Data, ResultBase.Item1.Length);
                else
                {
                    EDIDInfo.R_Error = ResultCEA.Item2;
                    return EDIDInfo;
                }
            }

            if (ResultDisplayID.Item2 != DecompileError.NoDecompile)
            {
                if (ResultDisplayID.Item2 == DecompileError.Success)
                    ResultDisplayID.Item1.CopyTo(EDIDInfo.Data, ResultBase.Item1.Length + ResultCEA.Item1.Length);
                else
                {
                    EDIDInfo.R_Error = ResultDisplayID.Item2;
                    return EDIDInfo;
                }
            }

            EDIDInfo.R_Error = DecompileError.Success;
            return EDIDInfo;
        }
        public void OutputNotesEDIDText(EDIDTable EDIDInfo, string Path)
        {
            string NoteEDID;
            byte[] BlockData = new byte[128];

            NoteEDID = "Time:" + System.DateTime.Now.ToString() + "\r\n";

            if (EDIDInfo.Error == DecodeError.Success)
            {
                uint Index = 0;
                foreach (BlockTagType Block in EDIDInfo.List)
                {
                    switch (Block)
                    {
                        case BlockTagType.Base:
                            Array.Copy(EDIDInfo.Data, Index, BlockData, 0, 128);
                            EDIDBase EDIDBase = new EDIDBase() { Data = BlockData, Table = EDIDInfo.Base };
                            NoteEDID += EDIDBase.OutputNotesEDIDBase();
                            Index += 128;
                            break;

                        case BlockTagType.CEA:
                            Array.Copy(EDIDInfo.Data, Index, BlockData, 0, 128);
                            EDIDCEA EDIDCEA = new EDIDCEA() { Data = BlockData, Table = EDIDInfo.CEA };
                            NoteEDID += EDIDCEA.OutputNotesEDIDCEA();
                            Index += 128;
                            break;

                        case BlockTagType.DisplayID:
                            Array.Copy(EDIDInfo.Data, Index, BlockData, 0, 128);
                            EDIDDisplayID EDIDDisplayID = new EDIDDisplayID() { Data = BlockData, Table = EDIDInfo.DisplayID };
                            NoteEDID += EDIDDisplayID.OutputNotesEDIDDisplay();
                            Index += 128;
                            break;

                        default:break;
                    }
                }
            }
            else
            {
                NoteEDID = "Decode error:" + EDIDInfo.Error.ToString() + "\r\n";
            }

            using (FileStream fsWrite = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(NoteEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }
        public void Output0xEDIDText(EDIDTable EDIDInfo, string Path)
        {
            string Note0xEDID = "";

            if (EDIDInfo.Error == DecodeError.Success)
            {
                for (int i = 0; i < EDIDInfo.Length; i++)
                {
                    if (i % 16 == 0)
                        Note0xEDID += "\r\n";
                    Note0xEDID += string.Format("0x{0:X2}, ", EDIDInfo.Data[i]);
                }
            }

            using (FileStream fsWrite = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(Note0xEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
