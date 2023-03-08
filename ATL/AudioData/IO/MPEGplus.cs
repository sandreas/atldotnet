using Commons;
using System;
using System.IO;
using static ATL.AudioData.AudioDataManager;
using static ATL.ChannelsArrangements;

namespace ATL.AudioData.IO
{
    /// <summary>
    /// Class for MusePack / MPEGplus files manipulation (extensions : .MPC, .MP+)
    /// </summary>
	class MPEGplus : IAudioDataIO
    {
        // Sample frequencies
        private static readonly int[] MPP_SAMPLERATES = new int[4] { 44100, 48000, 37800, 32000 };

        // ID code for stream version > 6
        private const long STREAM_VERSION_7_ID = 0x4D502B07;  // 'MP+' + #7
        private const long STREAM_VERSION_71_ID = 0x4D502B17; // 'MP+' + #23
        private const long STREAM_VERSION_8_ID = 0x4D50434B;  // 'MPCK'


        private int frameCount;
        private int sampleRate;

        private double bitrate;
        private double duration;
        private ChannelsArrangement channelsArrangement;

        private SizeInfo sizeInfo;
        private readonly string filePath;


        // File header data - for internal use
        private sealed class HeaderRecord
        {
            public byte[] ByteArray = new byte[32];               // Data as byte array
            public int[] IntegerArray = new int[8];            // Data as integer array
            private int version = 0;

            public static int GetVersion(byte[] data)
            {
                if (data.Length < 4) return 0;
                int dataAsInt = StreamUtils.DecodeBEInt32(data);

                // Get MPEGplus stream version
                if (STREAM_VERSION_7_ID == dataAsInt) return 70;
                else if (STREAM_VERSION_71_ID == dataAsInt) return 71;
                else if (STREAM_VERSION_8_ID == dataAsInt) return 80;
                else
                {
                    switch (data[1] % 32 / 2) // Int division
                    {
                        case 3: return 40;
                        case 7: return 50;
                        case 11: return 60;
                        default: return 0;
                    }
                }
            }

            public void computeVersion()
            {
                version = GetVersion(ByteArray);
            }

            public int Version => version;
        }

        // ---------- INFORMATIVE INTERFACE IMPLEMENTATIONS & MANDATORY OVERRIDES

        public bool IsVBR => true;
        public Format AudioFormat
        {
            get;
        }
        public int CodecFamily => AudioDataIOFactory.CF_LOSSY;
        public string FileName => filePath;
        public double BitRate => bitrate;
        public int BitDepth => -1; // Irrelevant for lossy formats
        public double Duration => duration;
        public ChannelsArrangement ChannelsArrangement => channelsArrangement;
        public int SampleRate => sampleRate;
        public bool IsMetaSupported(MetaDataIOFactory.TagType metaDataType)
        {
            return (metaDataType == MetaDataIOFactory.TagType.ID3V1) || (metaDataType == MetaDataIOFactory.TagType.ID3V2) || (metaDataType == MetaDataIOFactory.TagType.APE);
        }
        public long AudioDataOffset { get; set; }
        public long AudioDataSize { get; set; }



        // ---------- CONSTRUCTORS & INITIALIZERS

        private void resetData()
        {
            frameCount = 0;
            sampleRate = 0;
            AudioDataOffset = -1;
            AudioDataSize = 0;
        }

        public MPEGplus(string filePath, Format format)
        {
            this.filePath = filePath;
            AudioFormat = format;
            resetData();
        }


        // ---------- SUPPORT METHODS

        public static bool IsValidHeader(byte[] data)
        {
            return HeaderRecord.GetVersion(data) > 60; // <v7 not auto-detected (no specs available)
        }

        private bool readHeader(Stream source, ref HeaderRecord header)
        {
            bool result = true;
            source.Seek(sizeInfo.ID3v2Size, SeekOrigin.Begin);

            // Read header and get file size
            source.Read(header.ByteArray, 0, header.ByteArray.Length);

            // if transfer is not complete
            byte[] temp = new byte[4];
            for (int i = 0; i < header.IntegerArray.Length; i++)
            {
                Array.Copy(header.ByteArray, i * 4, temp, 0, 4);
                header.IntegerArray[i] = StreamUtils.DecodeInt32(temp);
            }
            header.computeVersion();

            // If VS8 file, looks for the (mandatory) stream header packet
            if (80 == header.Version)
            {
                string packetKey;
                bool headerFound = false;

                // Let's go back right after the 32-bit version marker
                source.Seek(sizeInfo.ID3v2Size + 4, SeekOrigin.Begin);

                byte[] buffer = new byte[2];
                while (!headerFound)
                {
                    long initialPos = source.Position;
                    source.Read(buffer, 0, 2);
                    packetKey = Utils.Latin1Encoding.GetString(buffer);

                    readVariableSizeInteger(source); // Packet size (unused)

                    // SV8 stream header packet
                    if (packetKey.Equals("SH"))
                    {
                        AudioDataOffset = initialPos;
                        // Skip CRC-32 and stream version
                        source.Seek(5, SeekOrigin.Current);
                        long sampleCount = readVariableSizeInteger(source);
                        readVariableSizeInteger(source); // Skip beginning silence

                        source.Read(buffer, 0, 1);// Sample frequency (3) + Max used bands (5)
                        sampleRate = MPP_SAMPLERATES[(buffer[0] & 0b11100000) >> 5]; // First 3 bits

                        source.Read(buffer, 0, 1); // Channel count (4) + Mid/Side Stereo used (1) + Audio block frames (3)
                        int channelCount = (buffer[0] & 0b11110000) >> 4; // First 4 bits
                        bool isMidSideStereo = (buffer[0] & 0b00001000) > 0; // First 4 bits
                        if (isMidSideStereo) channelsArrangement = JOINT_STEREO_MID_SIDE;
                        else channelsArrangement = GuessFromChannelNumber(channelCount);

                        // MPC has variable bitrate; only MPC versions < 7 display fixed bitrate
                        duration = sampleCount * 1000.0 / sampleRate;
                        bitrate = calculateAverageBitrate(duration);

                        headerFound = true;
                    }
                    // Continue searching for header
                    source.Seek(initialPos + 2, SeekOrigin.Begin);
                }
            }
            else
            {
                AudioDataOffset = sizeInfo.ID3v2Size;
            }
            AudioDataSize = sizeInfo.FileSize - sizeInfo.APESize - sizeInfo.ID3v1Size - AudioDataOffset;

            return result;
        }

        /* Get samplerate from header
            Note: this is the same byte where profile is stored
        */
        private static int getSV7SampleRate(HeaderRecord header)
        {
            if (header.Version > 50)
            {
                return MPP_SAMPLERATES[header.ByteArray[10] & 3];
            }
            else
            {
                return 44100; // Fixed to 44.1 Khz before SV5
            }
        }

        private static ChannelsArrangement getSV7ChannelsArrangement(HeaderRecord header)
        {
            ChannelsArrangement result;

            if ((70 == header.Version) || (71 == header.Version))
                // Get channel mode for stream version 7
                if ((header.ByteArray[11] % 128) < 64) result = STEREO;
                else result = JOINT_STEREO; // TODO - could actually be either intensity stereo or mid/side stereo; however that code is obscure....
            else
                // Get channel mode for stream version 4-6
                if (0 == (header.ByteArray[2] % 128)) result = STEREO;
            else result = JOINT_STEREO;

            return result;
        }

        private static int getSV7FrameCount(HeaderRecord header)
        {
            int result;

            if (40 == header.Version) result = header.IntegerArray[1] >> 16;
            else if ((50 <= header.Version) && (header.Version <= 71))
            {
                result = header.IntegerArray[1];
            }
            else result = 0;

            return result;
        }

        private double getSV7BitRate()
        {
            return calculateAverageBitrate(getSV7Duration());
        }

        private double calculateAverageBitrate(double duration)
        {
            double result = 0;
            long CompressedSize;

            CompressedSize = sizeInfo.FileSize - sizeInfo.TotalTagSize;

            if (duration > 0) result = Math.Round(CompressedSize * 8.0 / duration);

            return result;
        }

        private double getSV7Duration()
        {
            // Calculate duration time
            if (sampleRate > 0) return (frameCount * 1152.0 * 1000.0 / sampleRate);
            else return 0;
        }


        public bool Read(Stream source, SizeInfo sizeInfo, MetaDataIO.ReadTagParams readTagParams)
        {
            HeaderRecord header = new HeaderRecord();
            bool result;
            this.sizeInfo = sizeInfo;

            resetData();

            // Load header from file to variable
            result = readHeader(source, ref header);
            // Process data if loaded and file valid
            if (result && (sizeInfo.FileSize > 0) && (header.Version > 0))
            {
                if (header.Version < 80)
                {
                    // Fill properties with SV7 header data
                    sampleRate = getSV7SampleRate(header);
                    channelsArrangement = getSV7ChannelsArrangement(header);
                    frameCount = getSV7FrameCount(header);
                    bitrate = getSV7BitRate();
                    duration = getSV7Duration();
                }
                else
                {
                    // SV8 data already read
                }
            }

            return result;
        }

        // Specific to MPC SV8
        // See specifications
        private static long readVariableSizeInteger(Stream source)
        {
            long result = 0;
            byte b = 128;
            byte[] buffer = new byte[1];

            // Data is coded with a Big-endian, 7-byte variable-length record
            while ((b & 128) > 0)
            {
                source.Read(buffer, 0, 1);
                b = buffer[0];
                result = (result << 7) + (b & 127); // Big-endian
            }

            return result;
        }
    }
}