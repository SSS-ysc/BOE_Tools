using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace EDID_Form
{
    enum Support
    {
        unsupported,
        supported,
    };
    enum DecodeError
    {
        Success,
        LengthError,

        HeaderError,
        VersionError,
        NoMainTimingError,
        ChecksumError,

        CEAVersionError,
        CEAChecksumError
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
    struct EDIDDetailedTimingTable
    {
        public uint PixelClk;

        public uint HFrequency;
        public ushort HAdressable;
        public ushort HBlanking;
        public ushort HSyncFront;
        public ushort HSyncWidth;
        public byte HBorder;

        public float VFrequency;
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

        public EDIDDetailedTimingTable MainTiming;
        public EDIDDetailedTimingTable SecondMainTiming;
        public string SN;
        public EDIDDisplayRangeLimits Limits;
        public string Name;

        public byte ExBlockCount;
        public byte Checksum;
    };
    enum CEATagCodeType
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
        Reserved,
    }
    enum VideoCapabilityType
    { 
        NoSupport,
        AlwaysOverscanned,
        AlwaysUnderscanned,
        SupportBothOverAndUnder,
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
    }
    struct VSBlockAMD
    {    
    }
    struct VSBlockHDMIForum
    { 
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
    struct ExBlockYCbCr420
    { 
    
    }
    struct CEABlocksTable
    {
        public CEATagCodeType Block;
        public int BlockPayload;
    }
    struct EDIDTableCEA
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
        public ExBlockYCbCr420 BlockYCbCr420;

        public List<EDIDDetailedTimingTable> CEATimingList;
        public byte Checksum;
    };
    struct EDIDTableDisplayID
    {

    };

    internal class EDID
    {
        public string EDIDText = "";
        public byte[] EDIDByteData;
        public uint EDIDDataLength;
        public EDIDTable EDIDTable;
        public EDIDTableCEA EDIDTableCEA;
        public EDIDTableDisplayID EDIDTableDisplayID;
        public DecodeError EDIDDecodeStatus;

        static public string[] VICcode = {
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
        };

        private byte GetByteBit(byte a, byte X)
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
        private Support GetByteBitSupport(byte a, byte X)
        {
            if (((a & (0x01 << X)) >> X) == 0x01)
                return Support.supported;
            else
                return Support.unsupported;
        }
        private string GetSupportString(string Text, Support S)
        {
            return (S == Support.supported ? Text : "");
        }
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
        private uint MatchOriginalTextEDID(string Text)//standard format
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
        private uint Match0xTextEDID(string Text)//0x.. format
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
        private void FormatStringToByte(string EDIDText)
        {
            byte i = 0;
            MatchCollection mcText = Regex.Matches(EDIDText, @"([0-9]|[A-Z])([0-9]|[A-Z])");

            foreach (Match m in mcText)
            {
                EDIDByteData[i] = byte.Parse(m.ToString(), System.Globalization.NumberStyles.HexNumber);
                i++;
            }
        }
        /*
         * 解析EDID
         */
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
        private EDIDDetailedTimingTable DecodeDetailedTimingData(byte[] Data)
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
                Timing.VFrequency = (float)(Timing.HFrequency / (Timing.VAdressable + Timing.VBlanking));
            }

            Timing.Interface = (InterfaceType)GetByteBit(Data[17],7);

            Timing.StereoFormat = (StereoViewingType)((uint)(Data[17] & 0x60) >> 5);

            Timing.SyncType = (SyncType)((Data[17] & 0x18) >> 3);
            if (Timing.SyncType < SyncType.DigitalComposite)
                Timing.AnalogSync = (AnalogSyncType)(((Data[17] & 0x06) >> 1) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalComposite)
                Timing.DigitalSync = (DigitalSyncType)(((Data[17] & 0x06) >> 1) + 1);// + Undefined
            else if (Timing.SyncType == SyncType.DigitalSeparate)
                Timing.DigitalSync = (DigitalSyncType)(((Data[17] & 0x06) >> 1) + 5);// + VSyncN_HSyncN


            Console.WriteLine("Timing PixelClock: {0:000.00} MHz H:{1}Pixels HB:{2}Pixels V:{3}Lines VB:{4}Lines HSF:{5}Pixels HSW:{6}Pixels VSF:{7}Lines VSW:{8}Lines Hsize:{9}mm Vsize:{10}mm Interface:{11} StereoFormat:{12} SyncType:{13}  AnalogSync:{14}  DigitalSync:{15} HFreq:{16}Khz VFreq:{17}Hz",
                    (float)Timing.PixelClk / 1000000,
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
                    (float)Timing.HFrequency / 1000,
                    Timing.VFrequency);

            return Timing;
        }
        private EDIDDescriptorsType DecodeDisplayDescriptor(byte[] Data)
        {
            if ((Data[0] != 0x00) && (Data[1] != 0x00) && (Data[2] != 0x00))
            {
                EDIDTable.SecondMainTiming = DecodeDetailedTimingData(Data);
                return EDIDDescriptorsType.SecondMainTiming;
            }

            switch (Data[3])
            {
                case 0xFF:
                    EDIDTable.SN = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("SN :{0}", EDIDTable.SN);
                    return EDIDDescriptorsType.ProductSN;
                case 0xFE:
                    Console.WriteLine("AlphanumericData : Unresolved");
                    return EDIDDescriptorsType.AlphanumericData;
                case 0xFD:
                    EDIDTable.Limits.VerticalOffest = (LimitsHVOffsetsType)(Data[4] & 0x03);
                    EDIDTable.Limits.HorizontalOffest = (LimitsHVOffsetsType)((uint)(Data[4] & 0x0C)>>2);
                    EDIDTable.Limits.VerticalMin = (ushort)(Data[5] + (EDIDTable.Limits.VerticalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    EDIDTable.Limits.VerticalMax = (ushort)(Data[6] + (EDIDTable.Limits.VerticalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    EDIDTable.Limits.HorizontalMin = (ushort)(Data[7] + (EDIDTable.Limits.HorizontalOffest == LimitsHVOffsetsType.Max255Min255 ? 255 : 0));
                    EDIDTable.Limits.HorizontalMax = (ushort)(Data[8] + (EDIDTable.Limits.HorizontalOffest >= LimitsHVOffsetsType.Max255MinZero ? 255 : 0));
                    EDIDTable.Limits.PixelClkMax = (ushort)(Data[9] * 10);
                    EDIDTable.Limits.VideoTiming = (VideoTimingType)(Data[10]);
                    Console.WriteLine("RangeLimits : V {0}-{1}Hz, H {2}-{3}KHz, PixelClkMax {4}MHz, VideoTiming {5}", 
                        EDIDTable.Limits.VerticalMin, 
                        EDIDTable.Limits.VerticalMax,
                        EDIDTable.Limits.HorizontalMin,
                        EDIDTable.Limits.HorizontalMax,
                        EDIDTable.Limits.PixelClkMax,
                        EDIDTable.Limits.VideoTiming);
                    return EDIDDescriptorsType.RangeLimits;
                case 0xFC:
                    EDIDTable.Name = Encoding.ASCII.GetString(Data, 5, 13);
                    Console.WriteLine("Name :{0}", EDIDTable.Name);
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
        private DecodeError DecodeBaseBlock()
        {
            //00-07
            if ((EDIDByteData[0] != 0x00) || (EDIDByteData[1] != 0xFF) || (EDIDByteData[2] != 0xFF) || (EDIDByteData[3] != 0xFF) || (EDIDByteData[4] != 0xFF) || (EDIDByteData[5] != 0xFF) || (EDIDByteData[6] != 0xFF) || (EDIDByteData[7] != 0x00))
                return DecodeError.HeaderError;

            //18-19 EDID_Version
            if ((EDIDByteData[18] == 0x01) && ((EDIDByteData[19] == 0x03) || (EDIDByteData[19] == 0x04)))
            {
                if (EDIDByteData[19] == 0x03)
                    EDIDTable.Version = EDIDversion.V13;
                else
                    EDIDTable.Version = EDIDversion.V14;

                Console.WriteLine("EDID Version: {0}", EDIDTable.Version);
            }
            else
                return DecodeError.VersionError;

            //08-09 EDID_IDManufacturerName
            //0001="A",11010="Z",A-Z
            byte[] ID_Data = new byte[3];
            ID_Data[0] = (byte)((EDIDByteData[8] >> 2) + 0x40);
            ID_Data[1] = (byte)(((EDIDByteData[8] & 0x03) << 3) + (EDIDByteData[9] >> 5) + 0x40);
            ID_Data[2] = (byte)((EDIDByteData[9] & 0x1F) + 0x40);
            EDIDTable.IDManufacturerName = Encoding.ASCII.GetString(ID_Data);
            Console.WriteLine("Manufacturer Name: {0}", EDIDTable.IDManufacturerName);

            //10-11 EDID_IDProductCode
            EDIDTable.IDProductCode = (uint)(EDIDByteData[10] + (EDIDByteData[11] << 8));
            Console.WriteLine("ID Product: {0}", Convert.ToString(EDIDTable.IDProductCode, 16));

            //12-15 EDID_IDSerialNumber
            if (   ((EDIDByteData[12] == 0x01) && (EDIDByteData[13] == 0x01) && (EDIDByteData[14] == 0x01) && (EDIDByteData[15] == 0x01))
                || ( (EDIDByteData[12] == 0x00) && (EDIDByteData[13] == 0x00) && (EDIDByteData[14] == 0x00) && (EDIDByteData[15] == 0x00))
                )
            {
                EDIDTable.IDSerialNumber = null;
                Console.WriteLine("ID Serial Number: not used");
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    EDIDTable.IDSerialNumber += Convert.ToString(EDIDByteData[12 + i], 16);
                }
                Console.WriteLine("ID Serial Number: {0}", EDIDTable.IDSerialNumber);
            }

            //16-17 EDID_Week EDID_Year EDID_Model_Year
            if (EDIDByteData[16] <= 54)
            {
                EDIDTable.Week = EDIDByteData[16];
                Console.WriteLine("Week: {0}", EDIDTable.Week);
                EDIDTable.Year = (ushort)(EDIDByteData[17] + 1990);
                Console.WriteLine("Year: {0}", EDIDTable.Year);
            }
            else if ((EDIDTable.Version == EDIDversion.V14) && (EDIDByteData[16] == 0xFF))
            {
                EDIDTable.ModelYear = (ushort)(EDIDByteData[17] + 1990);
                Console.WriteLine("Week: not used");
                Console.WriteLine("Model Year: {0}", EDIDTable.ModelYear);
            }

            //20-24 EDID_Basic
            //20
            EDIDTable.Basic.Video_definition = (EDIDVideoStandard)((EDIDByteData[20] & 0x80) >> 7);
            Console.WriteLine("Video Standard: {0}", EDIDTable.Basic.Video_definition);
            if (EDIDTable.Version == EDIDversion.V14)
            {
                if (EDIDTable.Basic.Video_definition == EDIDVideoStandard.Digital)//EDID1.4 Digital
                {
                    EDIDTable.Basic.DigitalColorDepth = (EDIDColorBitDepth)((EDIDByteData[20] & 0x70) >> 4);
                    Console.WriteLine("Color Bit Depth: {0}", EDIDTable.Basic.DigitalColorDepth);

                    EDIDTable.Basic.DigitalStandard = (EDIDDigitalVideoStandard)(EDIDByteData[20] & 0x0F);
                    Console.WriteLine("Digital Standard: {0}", EDIDTable.Basic.DigitalStandard);
                }
            }
            else
            {
                if (EDIDTable.Basic.Video_definition == EDIDVideoStandard.Digital)//EDID1.3 Digital
                {
                }
                else
                {
                    EDIDTable.Basic.AnalogSignalLevelStandard = (byte)((EDIDByteData[20] & 0x60) >> 5);//?
                    EDIDTable.Basic.AnalogVideoSetup = (byte)((EDIDByteData[20] & 0x10) >> 4);//?
                    EDIDTable.Basic.DigitalColorDepth = (EDIDColorBitDepth)(EDIDByteData[20] & 0x0F);//?
                }
            }
            //21-22
            if ((EDIDByteData[21] != 0x00) && (EDIDByteData[22] != 0x00))
            {
                EDIDTable.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_HV;
                EDIDTable.Basic.ScreenSize.Hsize = EDIDByteData[21];
                EDIDTable.Basic.ScreenSize.Vsize = EDIDByteData[22];
                Console.WriteLine("Screen Size: {0}, H: {1} cm, V: {2} cm", EDIDTable.Basic.ScreenSize.Type, EDIDTable.Basic.ScreenSize.Hsize, EDIDTable.Basic.ScreenSize.Vsize);
            }
            else if (EDIDByteData[22] == 0x00)
            {
                EDIDTable.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDTable.Basic.ScreenSize.Ratio = EDIDByteData[21];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDTable.Basic.ScreenSize.Type, EDIDTable.Basic.ScreenSize.Ratio);
            }
            else if (EDIDByteData[21] == 0x00)
            {
                EDIDTable.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_Ratio;
                EDIDTable.Basic.ScreenSize.Ratio = EDIDByteData[22];   // ?
                Console.WriteLine("Screen Size: {0}, Ratio: {1}", EDIDTable.Basic.ScreenSize.Type, EDIDTable.Basic.ScreenSize.Ratio);
            }
            else
            {
                EDIDTable.Basic.ScreenSize.Type = ScreenSizeType.ScreenSize_undefined;
            }
            //23
            EDIDTable.Basic.Gamma = (float)EDIDByteData[23] / 100 + 1;
            Console.WriteLine("Gamma: {0} ", EDIDTable.Basic.Gamma);
            //24
            EDIDTable.Basic.FeatureSupport.StandbyMode = GetByteBitSupport(EDIDByteData[24], 7);
            EDIDTable.Basic.FeatureSupport.SuspendMode = GetByteBitSupport(EDIDByteData[24], 6);
            EDIDTable.Basic.FeatureSupport.VeryLowPowerMode = GetByteBitSupport(EDIDByteData[24], 5);
            EDIDTable.Basic.FeatureSupport.sRGBStandard = GetByteBitSupport(EDIDByteData[24], 2);
            EDIDTable.Basic.FeatureSupport.PreferredTimingMode = GetByteBitSupport(EDIDByteData[24], 1);
            if (EDIDTable.Version == EDIDversion.V13)
            {
                EDIDTable.Basic.FeatureSupport.DisplayColorType = (ColorType)((EDIDByteData[24] & 0x18) >> 3);
                EDIDTable.Basic.FeatureSupport.GTFstandard = GetByteBitSupport(EDIDByteData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, DisplayColorType: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, GTFstandard: {6}",
                    EDIDTable.Basic.FeatureSupport.StandbyMode,
                    EDIDTable.Basic.FeatureSupport.SuspendMode,
                    EDIDTable.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDTable.Basic.FeatureSupport.DisplayColorType,
                    EDIDTable.Basic.FeatureSupport.sRGBStandard,
                    EDIDTable.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDTable.Basic.FeatureSupport.GTFstandard);
            }
            else
            {
                EDIDTable.Basic.FeatureSupport.ColorEncodingFormat = (ColorEncoding)((EDIDByteData[24] & 0x18) >> 3);
                EDIDTable.Basic.FeatureSupport.ContinuousFrequency = GetByteBitSupport(EDIDByteData[24], 0);
                Console.WriteLine("StandbyMode: {0}, SuspendMode: {1}, LowPowerMode: {2}, ColorEncodingFormat: {3}, sRGBStandard: {4}, PreferredTimingMode: {5}, ContinuousFrequency: {6}",
                    EDIDTable.Basic.FeatureSupport.StandbyMode,
                    EDIDTable.Basic.FeatureSupport.SuspendMode,
                    EDIDTable.Basic.FeatureSupport.VeryLowPowerMode,
                    EDIDTable.Basic.FeatureSupport.ColorEncodingFormat,
                    EDIDTable.Basic.FeatureSupport.sRGBStandard,
                    EDIDTable.Basic.FeatureSupport.PreferredTimingMode,
                    EDIDTable.Basic.FeatureSupport.ContinuousFrequency);
            }

            //25-34 EDID_Color
            EDIDTable.PanelColor.RedX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 7)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 6)) + ((uint)EDIDByteData[27] << 2));
            EDIDTable.PanelColor.RedY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 5)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 4)) + ((uint)EDIDByteData[28] << 2));
            EDIDTable.PanelColor.GreenX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 3)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 2)) + ((uint)EDIDByteData[29] << 2));
            EDIDTable.PanelColor.GreenY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[25], 1)) * 2 + (uint)(GetByteBit(EDIDByteData[25], 0)) + ((uint)EDIDByteData[30] << 2));
            EDIDTable.PanelColor.BlueX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 7)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 6)) + ((uint)EDIDByteData[31] << 2));
            EDIDTable.PanelColor.BlueY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 5)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 4)) + ((uint)EDIDByteData[32] << 2));
            EDIDTable.PanelColor.WhiteX = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 3)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 2)) + ((uint)EDIDByteData[33] << 2));
            EDIDTable.PanelColor.WhiteY = GetEDIDColorxy((uint)(GetByteBit(EDIDByteData[26], 1)) * 2 + (uint)(GetByteBit(EDIDByteData[26], 0)) + ((uint)EDIDByteData[34] << 2));
            Console.WriteLine("Color.Red X: {0} Y: {1}, Color.Green X: {2} Y: {3}, Color.Blue X: {4} Y: {5}, Color.White X: {6} Y: {7}",
                EDIDTable.PanelColor.RedX,
                EDIDTable.PanelColor.RedY,
                EDIDTable.PanelColor.GreenX,
                EDIDTable.PanelColor.GreenY,
                EDIDTable.PanelColor.BlueX,
                EDIDTable.PanelColor.BlueY,
                EDIDTable.PanelColor.WhiteX,
                EDIDTable.PanelColor.WhiteY
                );

            //35-37 EDID_Established_Timing
            EDIDTable.EstablishedTiming.Es720x400_70 = GetByteBitSupport(EDIDByteData[35], 7);
            EDIDTable.EstablishedTiming.Es720x400_88 = GetByteBitSupport(EDIDByteData[35], 6);
            EDIDTable.EstablishedTiming.Es640x480_60 = GetByteBitSupport(EDIDByteData[35], 5);
            EDIDTable.EstablishedTiming.Es640x480_67 = GetByteBitSupport(EDIDByteData[35], 4);
            EDIDTable.EstablishedTiming.Es640x480_72 = GetByteBitSupport(EDIDByteData[35], 3);
            EDIDTable.EstablishedTiming.Es640x480_75 = GetByteBitSupport(EDIDByteData[35], 2);
            EDIDTable.EstablishedTiming.Es800x600_56 = GetByteBitSupport(EDIDByteData[35], 1);
            EDIDTable.EstablishedTiming.Es800x600_60 = GetByteBitSupport(EDIDByteData[35], 0);
                                                                            
            EDIDTable.EstablishedTiming.Es800x600_72 = GetByteBitSupport(EDIDByteData[36], 7);
            EDIDTable.EstablishedTiming.Es800x600_75 = GetByteBitSupport(EDIDByteData[36], 6);
            EDIDTable.EstablishedTiming.Es832x624_75 = GetByteBitSupport(EDIDByteData[36], 5);
            EDIDTable.EstablishedTiming.Es1024x768_87 = GetByteBitSupport(EDIDByteData[36], 4);
            EDIDTable.EstablishedTiming.Es1024x768_60 = GetByteBitSupport(EDIDByteData[36], 3);
            EDIDTable.EstablishedTiming.Es1024x768_70 = GetByteBitSupport(EDIDByteData[36], 2);
            EDIDTable.EstablishedTiming.Es1024x768_75 = GetByteBitSupport(EDIDByteData[36], 1);
            EDIDTable.EstablishedTiming.Es1280x1024_75 = GetByteBitSupport(EDIDByteData[36], 0);

            EDIDTable.EstablishedTiming.Es1152x870_75 = GetByteBitSupport(EDIDByteData[37], 7);

            //38-53 EDID_Standard_Timing
            EDIDTable.StandardTiming = new EDIDStandardTiming[8];
            for (int i = 0; i < 8; i++)
            {
                EDIDTable.StandardTiming[i] = DecodeStandardTimingData(EDIDByteData[38 + i * 2], EDIDByteData[39 + i * 2]);
                if (EDIDTable.StandardTiming[i].TimingSupport == Support.supported)
                    Console.WriteLine("Standard Timing : {0}x{1} Rate:{2}", EDIDTable.StandardTiming[i].TimingWidth, EDIDTable.StandardTiming[i].TimingHeight, EDIDTable.StandardTiming[i].TimingRate);
            }

            byte[] DsecriptorTable = new byte[18];
            //54-71 EDID_Main_Timing (Display Dsecriptor 1)
            if (EDIDByteData[54] == 0x00)
                return DecodeError.NoMainTimingError;
            EDIDTable.Descriptors1 = EDIDDescriptorsType.MainTiming;
            Array.Copy(EDIDByteData, 54, DsecriptorTable, 0, 18);
            EDIDTable.MainTiming = DecodeDetailedTimingData(DsecriptorTable);

            //72-89 Detailed Timing / Display Dsecriptor 2
            if (EDIDByteData[75] == 0x00)
                EDIDTable.Descriptors2 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 72, DsecriptorTable, 0, 18);
                EDIDTable.Descriptors2 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //90-107 Detailed Timing / Display Dsecriptor 3
            if (EDIDByteData[93] == 0x00)
                EDIDTable.Descriptors3 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 90, DsecriptorTable, 0, 18);
                EDIDTable.Descriptors3 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //108-125 Detailed Timing / Display Dsecriptor 4
            if (EDIDByteData[111] == 0x00)
                EDIDTable.Descriptors4 = EDIDDescriptorsType.Undefined;
            else
            {
                Array.Copy(EDIDByteData, 108, DsecriptorTable, 0, 18);
                EDIDTable.Descriptors4 = DecodeDisplayDescriptor(DsecriptorTable);
            }

            //126 EDID_Ex_Block_Count
            EDIDTable.ExBlockCount = EDIDByteData[126];

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += EDIDByteData[i];
            }
            if (checksum != 0x00)
                return DecodeError.ChecksumError;
            else
                EDIDTable.Checksum = EDIDByteData[127];

            return DecodeError.Success;
        }
        /*
         * 解析CEA EDID
         */
        private BlockAudio DecodeCEAAudioBlock(byte[] Data, int index)
        {
            BlockAudio Audio = new BlockAudio();

            Audio.Type = (AudioFormatType)(byte)(Data[index] >> 3);
            Console.WriteLine("Audio Format: {0}", Audio.Type.ToString());

            Audio.Freq192Khz = GetByteBitSupport(Data[index + 1], 6);
            Audio.Freq176_4Khz = GetByteBitSupport(Data[index + 1], 5);
            Audio.Freq96Khz = GetByteBitSupport(Data[ index + 1], 4);
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

                default:break;
            }
            return Audio;
        }
        private CEABlocksTable DecodeCEADataBlocks(byte[] CEAData,int index)
        {
            int i = 0;
            CEABlocksTable Block = new CEABlocksTable();

            Block.BlockPayload = (int)CEAData[index] & 0x1F;
            Block.Block = (CEATagCodeType)((CEAData[index] & 0xE0) >> 5);
            Console.WriteLine("---------------");

            byte[] BlockData = new byte[Block.BlockPayload];
            Array.Copy(CEAData, index + 1, BlockData, 0, Block.BlockPayload);

            switch (Block.Block)
            {
                case CEATagCodeType.Audio:
                    EDIDTableCEA.BlockAudio = new List<BlockAudio>();

                    for (i = 0; i < Block.BlockPayload / 3; i++)
                    {
                        EDIDTableCEA.BlockAudio.Add(DecodeCEAAudioBlock(BlockData, i * 3));
                    }
                    break;

                case CEATagCodeType.Video:
                    EDIDTableCEA.BlockVideoVIC = new List<BlockVideoVIC>();
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
                            VIC.VIC = BlockData[i];
                        }
                        EDIDTableCEA.BlockVideoVIC.Add(VIC);

                        Console.WriteLine("VIC: {0} {1}", 
                            VICcode[EDIDTableCEA.BlockVideoVIC[i].VIC],
                            EDIDTableCEA.BlockVideoVIC[i].NativeCode == Support.supported ? "Native" : ""
                            );
                    }
                    break;

                case CEATagCodeType.VendorSpecific:
                    int VSDB_IEEEID = BlockData[0] + (BlockData[1] << 8) + (BlockData[2] << 16);
                    switch (VSDB_IEEEID)
                    {
                        case 0x000C03:
                            Block.Block = CEATagCodeType.VS_HDMI_LLC;
                            break;

                        case 0x00001A:
                            Block.Block = CEATagCodeType.VS_AMD;
                            break;

                        case 0xC45DD8:
                            Block.Block = CEATagCodeType.VS_HDMI_Forum;
                            break;

                        default:
                            Console.WriteLine("unknow VSDB !!!!!!!!!!!!!!!!");
                            break;
                    }
                    break;

                case CEATagCodeType.SpeakerAllocation:
                    EDIDTableCEA.BlockSpeaker = new List<BlockSpeaker>();
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

                        EDIDTableCEA.BlockSpeaker.Add(Speaker);
                        Console.WriteLine("Speaker: {0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
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
                            GetSupportString("Front Left/Right High,", Speaker.TpFL_TpFH)
                            );
                    }
                    break;

                case CEATagCodeType.VESADisplayTransferCharacteristic:
                    break;

                case CEATagCodeType.Extended:
                    Block.Block = (CEATagCodeType)(BlockData[0] + CEATagCodeType.Ex_Video_Capability);
                    //复制有效数据
                    byte[] BlockExData =  new byte[Block.BlockPayload - 1];
                    Array.Copy(CEAData, index + 2, BlockExData, 0, Block.BlockPayload - 1);

                    switch (Block.Block)
                    {
                        case CEATagCodeType.Ex_Video_Capability:
                            EDIDTableCEA.BlockVideoCapability.QY = GetByteBitSupport(BlockExData[0], 7);
                            EDIDTableCEA.BlockVideoCapability.QS = GetByteBitSupport(BlockExData[0], 6);
                            EDIDTableCEA.BlockVideoCapability.PT = (VideoCapabilityType)((BlockExData[0] & 0x30) >> 4);
                            EDIDTableCEA.BlockVideoCapability.IT = (VideoCapabilityType)((BlockExData[0] & 0x0C) >> 2);
                            EDIDTableCEA.BlockVideoCapability.CE = (VideoCapabilityType)(BlockExData[0] & 0x03);
                            break;

                        case CEATagCodeType.Ex_VS_Video_Capability:
                            int VSVDB_IEEEID = BlockExData[0] + (BlockExData[1] << 8) + (BlockExData[2] << 16);
                            switch (VSVDB_IEEEID)
                            {
                                case 0x00D046:
                                    Block.Block = CEATagCodeType.Ex_VS_Dolby_Version;
                                    break;

                                default:
                                    Console.WriteLine("unknow VSVDB !!!!!!!!!!!!!!!!");
                                    break;
                            }
                            break;

                        case CEATagCodeType.Ex_VESA_Display_Device:
                            break;

                        case CEATagCodeType.Ex_VESA_Video_Timing:
                            break;

                        case CEATagCodeType.Ex_HDMI_Video:
                            break;

                        case CEATagCodeType.Ex_Colorimetry:
                            EDIDTableCEA.BlockColorimetry.BT2020_RGB = GetByteBitSupport(BlockExData[0], 7);
                            EDIDTableCEA.BlockColorimetry.BT2020_YCC = GetByteBitSupport(BlockExData[0], 6);
                            EDIDTableCEA.BlockColorimetry.BT2020_cYCC = GetByteBitSupport(BlockExData[0], 5);
                            EDIDTableCEA.BlockColorimetry.opRGB = GetByteBitSupport(BlockExData[0], 4);
                            EDIDTableCEA.BlockColorimetry.opYCC601 = GetByteBitSupport(BlockExData[0], 3);
                            EDIDTableCEA.BlockColorimetry.sYCC601 = GetByteBitSupport(BlockExData[0], 2);
                            EDIDTableCEA.BlockColorimetry.xvYCC709 = GetByteBitSupport(BlockExData[0], 1);
                            EDIDTableCEA.BlockColorimetry.xvYCC601 = GetByteBitSupport(BlockExData[0], 0);
                            EDIDTableCEA.BlockColorimetry.DCI_P3 = GetByteBitSupport(BlockExData[1], 7);
                            EDIDTableCEA.BlockColorimetry.MD3 = GetByteBitSupport(BlockExData[0], 3);
                            EDIDTableCEA.BlockColorimetry.MD2 = GetByteBitSupport(BlockExData[0], 2);
                            EDIDTableCEA.BlockColorimetry.MD1 = GetByteBitSupport(BlockExData[0], 1);
                            EDIDTableCEA.BlockColorimetry.MD0 = GetByteBitSupport(BlockExData[0], 0);
                            break;

                        case CEATagCodeType.Ex_HDR_Static_Matadata:
                            EDIDTableCEA.BlockHDRStatic.Gamma_SDR = GetByteBitSupport(BlockExData[0], 0);
                            EDIDTableCEA.BlockHDRStatic.Gamma_HDR = GetByteBitSupport(BlockExData[0], 1);
                            EDIDTableCEA.BlockHDRStatic.SMPTE_ST_2084 = GetByteBitSupport(BlockExData[0], 2);
                            EDIDTableCEA.BlockHDRStatic.HLG = GetByteBitSupport(BlockExData[0], 3);
                            EDIDTableCEA.BlockHDRStatic.Static_Metadata_Type1 = GetByteBitSupport(BlockExData[1], 0);
                            if (Block.BlockPayload >= 4)
                                EDIDTableCEA.BlockHDRStatic.Max_Luminance_Data = (float)(50 * Math.Pow(2, (double)(BlockExData[2])/32));
                            if (Block.BlockPayload >= 5)
                                EDIDTableCEA.BlockHDRStatic.Max_Frame_Avg_Lum_Data = (float)(50 * Math.Pow(2, (double)(BlockExData[3]) / 32));
                            if (Block.BlockPayload >= 6)
                                EDIDTableCEA.BlockHDRStatic.Min_Luminance_Data = (float)(Math.Pow((double)BlockExData[4] / 255, 2) / 100);
                            break;

                        case CEATagCodeType.Ex_HDR_Dynamic_Matadata:
                            break;

                        case CEATagCodeType.Ex_Video_Format_Preference:
                            break;

                        case CEATagCodeType.Ex_YCbCr420Video:
                            break;

                        case CEATagCodeType.Ex_YCbCr420CapabilityMap:
                            break;

                        case CEATagCodeType.Ex_CEA_Miscellaneous_Audio_Fields:
                            break;

                        case CEATagCodeType.Ex_VS_Audio:
                            break;

                        case CEATagCodeType.Ex_HDMI_Audio:
                            break;

                        case CEATagCodeType.Ex_Room_Configuration:
                            break;

                        case CEATagCodeType.Ex_Speaker_Location:
                            break;

                        case CEATagCodeType.Ex_Inframe:
                            break;

                        default:
                            Block.Block = CEATagCodeType.Extended;
                            Console.WriteLine("unknow Extended Tag code !!!!!!!!!!!!!!!!");
                            break;
                    }
                    break;
                
                case CEATagCodeType.Reserved:
                case CEATagCodeType.ReReserved:
                default:
                    break;
            }
            Console.WriteLine("/ Decode {0} END, BlockPayload: {1} / ", Block.Block.ToString(), Block.BlockPayload);
            return Block;
        }
        private DecodeError DecodeCEABlock(int index)
        {
            byte[] CEAByteData = new byte[128];
            Array.Copy(EDIDByteData, index, CEAByteData, 0, 128);

            //01 Revision Number
            EDIDTableCEA.Version = CEAByteData[1];
            Console.WriteLine("CEA Version: {0}", EDIDTableCEA.Version);
            if(EDIDTableCEA.Version != 0x03)
                return DecodeError.CEAVersionError;

            //02 
            EDIDTableCEA.DetailedTimingStart = CEAByteData[2];

            //03
            EDIDTableCEA.UnderscranITFormatByDefault = GetByteBitSupport(CEAByteData[3],7);
            EDIDTableCEA.Audio = GetByteBitSupport(CEAByteData[3], 6);
            EDIDTableCEA.YCbCr444 = GetByteBitSupport(CEAByteData[3], 5);
            EDIDTableCEA.YCbCr422 = GetByteBitSupport(CEAByteData[3], 4);
            EDIDTableCEA.NativeVideoFormatNumber = (byte)(CEAByteData[3] & 0x0F);

            //04-... CEA Data Blocks
            if (EDIDTableCEA.DetailedTimingStart != 4)
            {
                int blockindex = 4;
                int blocknumber = 0;
                EDIDTableCEA.CEABlocksList = new List<CEABlocksTable>();

                while (blockindex < EDIDTableCEA.DetailedTimingStart)
                {
                    EDIDTableCEA.CEABlocksList.Add(DecodeCEADataBlocks(CEAByteData, blockindex));

                    blockindex += EDIDTableCEA.CEABlocksList[blocknumber].BlockPayload + 1;//Block Data length + Tag Code
                    blocknumber++;
                }
            }

            //Detailed Timing Blocks
            if (EDIDTableCEA.DetailedTimingStart != 0)
            {
                int DetailedTimingindex = EDIDTableCEA.DetailedTimingStart;
                byte[] TimingByte = new byte[18];
                EDIDTableCEA.CEATimingList = new List<EDIDDetailedTimingTable>();

                while (CEAByteData[DetailedTimingindex] != 0x00 && (DetailedTimingindex + 18) < 127)
                {
                    Array.Copy(CEAByteData, DetailedTimingindex, TimingByte, 0, 18);
                    EDIDTableCEA.CEATimingList.Add(DecodeDetailedTimingData(TimingByte));

                    DetailedTimingindex += 18;
                }
            }

            //127 Checksum
            byte checksum = 0x00;
            for (int i = 0; i < 128; i++)
            {
                checksum += CEAByteData[i];
            }
            if (checksum != 0x00)
                return DecodeError.CEAChecksumError;
            else
                EDIDTableCEA.Checksum = CEAByteData[127];

            return DecodeError.Success;
        }
        /*
         * 解析Display ID
         */
        private DecodeError DecodeDisplayIDBlock()
        {
            return DecodeError.Success;
        }
        public DecodeError Decode(string UnicodeText)
        {
            DecodeError Error;
            EDIDDataLength = 0;
            EDIDText = "";

            EDIDDataLength = MatchOriginalTextEDID(UnicodeText);

            if (EDIDDataLength == 0)
            {
                EDIDDataLength = Match0xTextEDID(UnicodeText);
            }

            if (EDIDDataLength % 128 != 0) return DecodeError.LengthError;

            EDIDByteData = new byte[EDIDDataLength];
            FormatStringToByte(EDIDText);

            Error = DecodeBaseBlock();
            if(Error != DecodeError.Success)
                return Error;

            if (EDIDDataLength >= 256)
            {
                Error = DecodeCEABlock(128);
                if (Error != DecodeError.Success)
                    return Error;
            }
            if (EDIDDataLength >= 384)
            {
                Error = DecodeDisplayIDBlock();
                if (Error != DecodeError.Success)
                    return Error;
            }

            return DecodeError.Success;
        }
        /*
         * 导出txt文件
         */
        //行格式
        private string OutputNotesLineString(string Notes, int ValueOffset, params object[] Value)
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

            return Notes;
        }
        private string OutputNotesLineString(int NotesOffset, string Notes, int ValueOffset, params object[] Value)
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

            return Notes;
        }
        //列格式
        private string OutputNotesListString(string Notes, int Offset, params object[] Value)
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

            return Notes;
        }
        //两列格式
        private string OutputNotesListsString(string Notes, int Offset, object Value, string Notes2, int Offset2, object Value2)
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

            return Notes;
        }
        private string OutputNotesEDIDList(uint EDIDByteDataOffset)
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
                    Notes += "  " + string.Format("{0:X2}", EDIDByteData[EDIDByteDataOffset + LineNumber * 10 + Number]);
                }
                Notes += "\r\n";
            }
            Notes += "______________________________________________________________________\r\n";
            return Notes;
        }
        private string OutputNotesDetailedTiming(EDIDDetailedTimingTable Timing)
        {
            string Notes;
            int list_offset = 8;
            int list_offset2 = 50;

            Notes = OutputNotesLineString(list_offset, "{0}x{1}@{2:.00}Hz   Pixel Clock: {3:.00} MHz", 0, Timing.HAdressable, Timing.VAdressable, Timing.VFrequency, (float)Timing.PixelClk/1000000);
            Notes += "\r\n";
            Notes += OutputNotesListsString("Horizontal Image Size: {0} mm", list_offset, Timing.VideoSizeH, "Vertical Image Size: {0} mm", list_offset2, Timing.VideoSizeV);
            Notes += OutputNotesListsString("Refreshed Mode: {0}", list_offset, Timing.Interface, "Normal Display: {0}", list_offset2, Timing.StereoFormat);
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "Horizontal:", 0);
            Notes += OutputNotesListsString("Active Time: {0} pixels", list_offset, Timing.HAdressable, "Blanking Time: {0} pixels", list_offset2, Timing.HBlanking);
            Notes += OutputNotesListsString("Sync Offset: {0} pixels", list_offset, Timing.HSyncFront, "Sync Pulse Width: {0} pixels", list_offset2, Timing.HSyncWidth);
            Notes += OutputNotesListsString("Border: {0} pixels", list_offset, Timing.HBorder, "Frequency: {0:.00} Khz", list_offset2, (float)Timing.HFrequency/1000);
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "Vertical:", 0);
            Notes += OutputNotesListsString("Active Time: {0} Lines", list_offset, Timing.VAdressable, "Blanking Time: {0} Lines", list_offset2, Timing.VBlanking);
            Notes += OutputNotesListsString("Sync Offset: {0} Lines", list_offset, Timing.VSyncFront, "Sync Pulse Width: {0} Lines", list_offset2, Timing.VSyncWidth);
            Notes += OutputNotesListsString("Border: {0} Lines", list_offset, Timing.VBorder, "Frequency: {0:.00} Hz", list_offset2, Timing.VFrequency); 
            Notes += "\r\n";
            Notes += OutputNotesLineString(list_offset, "{0},{1}{2}", 0, Timing.SyncType,
                Timing.AnalogSync == AnalogSyncType.Undefined ? "" : Timing.AnalogSync.ToString(),
                Timing.DigitalSync == DigitalSyncType.Undefined ? "" : Timing.DigitalSync.ToString()
                );

            return Notes;
        }
        private string OutputNotesDescriptorBlock(EDIDDescriptorsType Type)
        {
            string Notes = "\r\n";
            int list_offset = 8;

            switch (Type)
            {
                case EDIDDescriptorsType.MainTiming:
                    Notes += OutputNotesDetailedTiming(EDIDTable.MainTiming);
                    break;

                case EDIDDescriptorsType.SecondMainTiming:
                    Notes += OutputNotesDetailedTiming(EDIDTable.SecondMainTiming);
                    break;

                case EDIDDescriptorsType.ProductSN:
                    Notes += OutputNotesLineString(list_offset, "Monitor Serial Number:",0);
                    Notes += OutputNotesLineString(list_offset, "{0}", 0, EDIDTable.SN);
                    break;

                case EDIDDescriptorsType.ProductName:
                    Notes += OutputNotesLineString(list_offset, "Monitor Name:", 0);
                    Notes += OutputNotesLineString(list_offset, "{0}", 0, EDIDTable.Name);
                    break;

                case EDIDDescriptorsType.RangeLimits:
                    Notes += OutputNotesLineString(list_offset, "Monitor Range Limits:", 0);
                    Notes += OutputNotesLineString(list_offset, "Vertical Freq: {0} - {1} Hz", 0, EDIDTable.Limits.VerticalMin, EDIDTable.Limits.VerticalMax);
                    Notes += OutputNotesLineString(list_offset, "Horizontal Freq: {0} - {1} KHz", 0, EDIDTable.Limits.HorizontalMin, EDIDTable.Limits.HorizontalMax);
                    Notes += OutputNotesLineString(list_offset, "Pixel Clock: {0} MHz", 0, EDIDTable.Limits.PixelClkMax);
                    Notes += OutputNotesLineString(list_offset, "VideoTimingType: {0}", 0, EDIDTable.Limits.VideoTiming.ToString());
                    break;

                default:
                    Notes += OutputNotesLineString(list_offset, Type.ToString(), 0);
                    break;
            }

            Notes += "\r\n";
            return Notes;
        }
        private string OutputNotesCEABlocks(CEABlocksTable Table)
        {
            string Notes = "";
            int list_offset = 8;

            switch (Table.Block)
            {
                case CEATagCodeType.Audio:
                    Notes += OutputNotesLineString("Audio Data Block, Number of Data Byte to Follow: {0}",0, Table.BlockPayload);
                    foreach (BlockAudio Audio in EDIDTableCEA.BlockAudio)
                    {
                        Notes += OutputNotesLineString(list_offset, "Audio Format: {0}, Channel Number: {1}", 0, Audio.Type, Audio.Channels);
                        Notes += OutputNotesLineString(list_offset, "Sampling: Frequency: {0}{1}{2}{3}{4}{5}{6}", 0, 
                            GetSupportString("192Khz,",Audio.Freq192Khz),
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
                case CEATagCodeType.Video:
                    Notes += OutputNotesLineString("Video Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    foreach (BlockVideoVIC VIC in EDIDTableCEA.BlockVideoVIC)
                    {
                        Notes += OutputNotesLineString(list_offset, "{0} {1} ", 0, VICcode[VIC.VIC], GetSupportString("Native", VIC.NativeCode));
                    }
                    break;
                case CEATagCodeType.SpeakerAllocation:
                    Notes += OutputNotesLineString("Speaker Allocation Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    foreach (BlockSpeaker Speaker in EDIDTableCEA.BlockSpeaker)
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
                case CEATagCodeType.VESADisplayTransferCharacteristic:
                    Notes += OutputNotesLineString("Display Transfer Characteristic Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;

                /* Vendor-Specific Data Block */
                case CEATagCodeType.VendorSpecific:
                    Notes += OutputNotesLineString("unknow Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.VS_HDMI_LLC:
                    Notes += OutputNotesLineString("HDMI-LLC Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.VS_AMD:
                    Notes += OutputNotesLineString("AMD Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.VS_HDMI_Forum:
                    Notes += OutputNotesLineString("HDMI-forum Vendor-Specific Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;

                /* Extended */
                case CEATagCodeType.Extended:
                    Notes += OutputNotesLineString("unknow Extended Tag Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Video_Capability:
                    Notes += OutputNotesLineString("Video Capability Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "CE: {0}", 0, EDIDTableCEA.BlockVideoCapability.CE);
                    Notes += OutputNotesLineString(list_offset, "IT: {0}", 0, EDIDTableCEA.BlockVideoCapability.IT);
                    Notes += OutputNotesLineString(list_offset, "PT: {0}", 0, EDIDTableCEA.BlockVideoCapability.PT);
                    Notes += OutputNotesLineString(list_offset, "RGB Quantization Range: {0}", 0, EDIDTableCEA.BlockVideoCapability.QS == Support.supported ? "Selectable (via AVI Q)" : "No Data");
                    Notes += OutputNotesLineString(list_offset, "YCC Quantization Range: {0}", 0, EDIDTableCEA.BlockVideoCapability.QY == Support.supported ? "Selectable (via AVI YQ)" : "No Data");
                    break;
                case CEATagCodeType.Ex_VESA_Display_Device:
                    Notes += OutputNotesLineString("Display Device Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_VESA_Video_Timing:
                    Notes += OutputNotesLineString("Video Timing Block Extension, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_HDMI_Video:
                    Notes += OutputNotesLineString("HDMI Video Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Colorimetry:
                    Notes += OutputNotesLineString("Colorimetry Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", 0,
                        GetSupportString("BT.2020 RGB,", EDIDTableCEA.BlockColorimetry.BT2020_RGB),
                        GetSupportString("BT.2020 YCC,", EDIDTableCEA.BlockColorimetry.BT2020_YCC),
                        GetSupportString("BT.2020 cYCC,", EDIDTableCEA.BlockColorimetry.BT2020_cYCC),
                        GetSupportString("Adobe RGB,", EDIDTableCEA.BlockColorimetry.opRGB),
                        GetSupportString("Adobe YCC-601,", EDIDTableCEA.BlockColorimetry.opYCC601),
                        GetSupportString("sYCC-601,", EDIDTableCEA.BlockColorimetry.sYCC601),
                        GetSupportString("xvYCC-709,", EDIDTableCEA.BlockColorimetry.xvYCC709),
                        GetSupportString("xvYCC-601,", EDIDTableCEA.BlockColorimetry.xvYCC601),
                        GetSupportString("DCI-P3,", EDIDTableCEA.BlockColorimetry.DCI_P3),
                        GetSupportString("MD3,", EDIDTableCEA.BlockColorimetry.MD3),
                        GetSupportString("MD2,", EDIDTableCEA.BlockColorimetry.MD2),
                        GetSupportString("MD1,", EDIDTableCEA.BlockColorimetry.MD1),
                        GetSupportString("MD0,", EDIDTableCEA.BlockColorimetry.MD0));
                    break;
                case CEATagCodeType.Ex_HDR_Static_Matadata:
                    Notes += OutputNotesLineString("HDR Static Matadata Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    Notes += OutputNotesLineString(list_offset, "{0}{1}{2}{3}{4}", 0,
                        GetSupportString("Gamma-SDR,", EDIDTableCEA.BlockHDRStatic.Gamma_SDR),
                        GetSupportString("Gamma-HDR,", EDIDTableCEA.BlockHDRStatic.Gamma_HDR),
                        GetSupportString("EOTF SMPTE ST 2084,", EDIDTableCEA.BlockHDRStatic.SMPTE_ST_2084),
                        GetSupportString("EOTF HLG,", EDIDTableCEA.BlockHDRStatic.HLG),
                        GetSupportString("Static MetaData Type 1,", EDIDTableCEA.BlockHDRStatic.Static_Metadata_Type1));
                    if (EDIDTableCEA.BlockHDRStatic.Max_Luminance_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Max Luminance: {0:.000}", 0, EDIDTableCEA.BlockHDRStatic.Max_Luminance_Data);
                    if (EDIDTableCEA.BlockHDRStatic.Max_Frame_Avg_Lum_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Max Frame-average Luminance: {0:.000}", 0, EDIDTableCEA.BlockHDRStatic.Max_Frame_Avg_Lum_Data);
                    if(EDIDTableCEA.BlockHDRStatic.Min_Luminance_Data != 0)
                        Notes += OutputNotesLineString(list_offset, "Desired Content Min Luminance: {0:.00000}", 0, EDIDTableCEA.BlockHDRStatic.Min_Luminance_Data);
                    break;
                case CEATagCodeType.Ex_HDR_Dynamic_Matadata:
                    Notes += OutputNotesLineString("HDR Dynamic Matadata Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Video_Format_Preference:
                    Notes += OutputNotesLineString("Video Format Preference Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_YCbCr420Video:
                    Notes += OutputNotesLineString("YCBCR 4:2:0 Video Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_YCbCr420CapabilityMap:
                    Notes += OutputNotesLineString("YCBCR 4:2:0 Capability Map Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_CEA_Miscellaneous_Audio_Fields:
                    Notes += OutputNotesLineString("CTA Miscellaneous Audio Fields, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_VS_Audio:
                    Notes += OutputNotesLineString("Vendor-Specific Audio Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_HDMI_Audio:
                    Notes += OutputNotesLineString("HDMI Audio Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Room_Configuration:
                    Notes += OutputNotesLineString("Room Configuration Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Speaker_Location:
                    Notes += OutputNotesLineString("Speaker Location Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_Inframe:
                    Notes += OutputNotesLineString("InfoFrame Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;

                /* Vendor-Specific Video Data Block */
                case CEATagCodeType.Ex_VS_Video_Capability:
                    Notes += OutputNotesLineString("unknow Vendor-Specific Video Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;
                case CEATagCodeType.Ex_VS_Dolby_Version:
                    Notes += OutputNotesLineString("Dolby Version Vendor-Specific Video Data Block, Number of Data Byte to Follow: {0}", 0, Table.BlockPayload);
                    break;

                default:
                    break;
            }

            Notes += "\r\n";
            return Notes;
        }
        public bool OutputNotesEDIDText(string Path)
        {
            string NoteEDID;
            int ValueOffset = 50;
            int i = 0;

            NoteEDID = "Time:" + System.DateTime.Now.ToString() + "\r\n\r\n";

            if (EDIDDecodeStatus == DecodeError.Success)
            {
                if (EDIDDataLength >= 128)
                {
                    NoteEDID += OutputNotesEDIDList(0);
                    NoteEDID += OutputNotesLineString("(08-09) ID Manufacturer Name:", ValueOffset, EDIDTable.IDManufacturerName);
                    NoteEDID += OutputNotesLineString("(10-11) Product ID Code:", ValueOffset, Convert.ToString(EDIDTable.IDProductCode, 16));
                    if (EDIDTable.IDSerialNumber == null)
                        NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, "not used");
                    else
                        NoteEDID += OutputNotesLineString("(12-15) ID Serial Number:", ValueOffset, EDIDTable.IDSerialNumber);
                    NoteEDID += OutputNotesLineString("(16) Week of Manufacture:", ValueOffset, EDIDTable.Week);
                    NoteEDID += OutputNotesLineString("(17) Yaer of Manufacture:", ValueOffset, EDIDTable.Year);
                    NoteEDID += OutputNotesLineString("(18) EDID Version Number:", ValueOffset, "1");
                    NoteEDID += OutputNotesLineString("(19) EDID Revision Number:", ValueOffset, (3 + EDIDTable.Version));
                    NoteEDID += OutputNotesLineString("(20) Video Input Definition:", ValueOffset, EDIDTable.Basic.Video_definition);
                    if (EDIDTable.Version == EDIDversion.V14)
                        NoteEDID += OutputNotesLineString("     ", 0, EDIDTable.Basic.DigitalStandard.ToString(), "  ", EDIDTable.Basic.DigitalColorDepth.ToString());

                    NoteEDID += OutputNotesLineString("(21) ScreenSize Horizontal:", ValueOffset, EDIDTable.Basic.ScreenSize.Hsize, " cm");
                    NoteEDID += OutputNotesLineString("(22) ScreenSize Vertical:", ValueOffset, EDIDTable.Basic.ScreenSize.Vsize, " cm");
                    NoteEDID += OutputNotesLineString("(23) Display Gamma:", ValueOffset, EDIDTable.Basic.Gamma);
                    NoteEDID += OutputNotesLineString("(24) Power Management and Supported Feature(s):", 0);
                    NoteEDID += OutputNotesLineString("     ", 0,
                        GetSupportString("Standby Mode/", EDIDTable.Basic.FeatureSupport.StandbyMode),
                        GetSupportString("Suspend Mode/", EDIDTable.Basic.FeatureSupport.SuspendMode),
                        GetSupportString("Very Low Power/", EDIDTable.Basic.FeatureSupport.VeryLowPowerMode),
                        EDIDTable.Basic.FeatureSupport.DisplayColorType, "/",
                        GetSupportString("sRGBStandard/", EDIDTable.Basic.FeatureSupport.sRGBStandard),
                        GetSupportString("PreferredTimingMode/", EDIDTable.Basic.FeatureSupport.PreferredTimingMode),
                        GetSupportString("PreferredTimingMode/", EDIDTable.Basic.FeatureSupport.GTFstandard));

                    NoteEDID += OutputNotesLineString("(25-34) Panel Color:", 0);
                    NoteEDID += OutputNotesLineString("(25-34) ".Length, "Red X - {0:0.000} Blue X - {1:0.000} Green X - {2:0.000} White X - {3:0.000}", 0, EDIDTable.PanelColor.RedX, EDIDTable.PanelColor.GreenX, EDIDTable.PanelColor.BlueX, EDIDTable.PanelColor.WhiteX);
                    NoteEDID += OutputNotesLineString("(25-34) ".Length, "Red Y - {0:0.000} Blue Y - {1:0.000} Green Y - {2:0.000} White Y - {3:0.000}", 0, EDIDTable.PanelColor.RedY, EDIDTable.PanelColor.GreenY, EDIDTable.PanelColor.BlueY, EDIDTable.PanelColor.WhiteY);

                    NoteEDID += OutputNotesListString("(35-37) Established Timing:", "(35-37) ".Length,
                        GetSupportString("720x400 @ 70Hz", EDIDTable.EstablishedTiming.Es720x400_70),
                        GetSupportString("720x400 @ 88Hz", EDIDTable.EstablishedTiming.Es720x400_88),
                        GetSupportString("640x480 @ 60Hz", EDIDTable.EstablishedTiming.Es640x480_60),
                        GetSupportString("640x480 @ 67Hz", EDIDTable.EstablishedTiming.Es640x480_67),
                        GetSupportString("640x480 @ 72Hz", EDIDTable.EstablishedTiming.Es640x480_72),
                        GetSupportString("640x480 @ 75Hz", EDIDTable.EstablishedTiming.Es640x480_75),
                        GetSupportString("800x600 @ 56Hz", EDIDTable.EstablishedTiming.Es800x600_56),
                        GetSupportString("800x600 @ 60Hz", EDIDTable.EstablishedTiming.Es800x600_60),
                        GetSupportString("800x600 @ 72Hz", EDIDTable.EstablishedTiming.Es800x600_72),
                        GetSupportString("800x600 @ 75Hz", EDIDTable.EstablishedTiming.Es800x600_75),
                        GetSupportString("832x624 @ 75Hz", EDIDTable.EstablishedTiming.Es832x624_75),
                        GetSupportString("1024x768 @ 87Hz", EDIDTable.EstablishedTiming.Es1024x768_87),
                        GetSupportString("1024x768 @ 60Hz", EDIDTable.EstablishedTiming.Es1024x768_60),
                        GetSupportString("1024x768 @ 70Hz", EDIDTable.EstablishedTiming.Es1024x768_70),
                        GetSupportString("1024x768 @ 75Hz", EDIDTable.EstablishedTiming.Es1024x768_75),
                        GetSupportString("1280x1024 @ 75Hz", EDIDTable.EstablishedTiming.Es1280x1024_75),
                        GetSupportString("1152x870 @ 75Hz", EDIDTable.EstablishedTiming.Es1152x870_75)); 

                    NoteEDID += OutputNotesLineString("(38-53) Standard Timing:", 0);
                    for (i = 0; i < 8; i++)
                    {
                        if (EDIDTable.StandardTiming[i].TimingSupport == Support.supported)
                            NoteEDID += OutputNotesLineString("", "(38-53) ".Length, EDIDTable.StandardTiming[i].TimingWidth, "x", EDIDTable.StandardTiming[i].TimingHeight, " @ ", EDIDTable.StandardTiming[i].TimingRate, "Hz");
                    }
                    NoteEDID += "______________________________________________________________________\r\n";
                    NoteEDID += "(54-71) Descriptor Block 1:\r\n" + OutputNotesDescriptorBlock(EDIDTable.Descriptors1);
                    NoteEDID += "______________________________________________________________________\r\n";
                    NoteEDID += "(72-89) Descriptor Block 2:\r\n" + OutputNotesDescriptorBlock(EDIDTable.Descriptors2);
                    NoteEDID += "______________________________________________________________________\r\n";
                    NoteEDID += "(90-107) Descriptor Block 3:\r\n" + OutputNotesDescriptorBlock(EDIDTable.Descriptors3);
                    NoteEDID += "______________________________________________________________________\r\n";
                    NoteEDID += "(108-125) Descriptor Block 4:\r\n" + OutputNotesDescriptorBlock(EDIDTable.Descriptors4);

                    NoteEDID += OutputNotesLineString("(126) Extension EDID Block(s):", 0, EDIDTable.ExBlockCount);
                    NoteEDID += OutputNotesLineString("(127) CheckSum: OK",0);
                }

                if (EDIDDataLength >= 256)
                {
                    NoteEDID += OutputNotesEDIDList(128);
                    NoteEDID += OutputNotesLineString("(01) CEA Version: {0}", 0, EDIDTableCEA.Version);
                    NoteEDID += OutputNotesListString("(02) General Info:", 8,
                        GetSupportString("Support Underscran", EDIDTableCEA.UnderscranITFormatByDefault),
                        GetSupportString("Support Audio", EDIDTableCEA.Audio),
                        GetSupportString("Support YCbCr 4:4:4", EDIDTableCEA.YCbCr444),
                        GetSupportString("Support YCbCr 4:2:2", EDIDTableCEA.YCbCr422),
                        "Native Format: " + EDIDTableCEA.NativeVideoFormatNumber.ToString()
                        );
                    NoteEDID += OutputNotesLineString("(03) Detailed Timing Start: {0}", 0, EDIDTableCEA.DetailedTimingStart);

                    i = 4;
                    foreach (CEABlocksTable Table in EDIDTableCEA.CEABlocksList)
                    {
                        NoteEDID += "(" + string.Format("{0:D2}", i) + "-" + string.Format("{0:D2}", i + Table.BlockPayload) + ") " + OutputNotesCEABlocks(Table);
                        i += Table.BlockPayload + 1;
                    }

                    int TimingNumber = 0;
                    foreach (EDIDDetailedTimingTable Timing in EDIDTableCEA.CEATimingList)
                    {
                        TimingNumber++;
                        NoteEDID += "______________________________________________________________________\r\n";
                        NoteEDID += "(" + string.Format("{0:D2}",i.ToString()) + "-" + string.Format("{0:D2}", i + 17) + ")" + " Detailed Timing " + TimingNumber.ToString() + ":\r\n\r\n" + OutputNotesDetailedTiming(Timing);
                        i += 18;
                    }

                    if (i != 127)
                    { 
                        NoteEDID += "\r\n(" + string.Format("{0:D2}", i.ToString()) + "-" + 126.ToString() + ") No data"; 
                    }
                    NoteEDID += OutputNotesLineString("\r\n(127) CheckSum: OK", 0);
                }

                if (EDIDDataLength >= 384)
                {
                    NoteEDID += OutputNotesEDIDList(384);
                }
            }
            else
            {
                NoteEDID = "Decode error:" + EDIDDecodeStatus.ToString() + "\r\n";
            }

            using (FileStream fsWrite = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(NoteEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }

            return true;
        }
        public bool Output0xEDIDText(string Path)
        {
            string Note0xEDID = "";

            if (EDIDDecodeStatus == DecodeError.Success)
            {
                for (int i = 0; i < EDIDDataLength; i++)
                {
                    if (i % 16 == 0)
                        Note0xEDID += "\r\n";
                    Note0xEDID += string.Format("0x{0:X2}, ", EDIDByteData[i]);
                }
            }

            using (FileStream fsWrite = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(Note0xEDID);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
            return true;
        }
    }
}
