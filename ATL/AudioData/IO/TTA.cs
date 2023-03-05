using ATL.Logging;
using System.IO;
using static ATL.AudioData.AudioDataManager;
using Commons;
using static ATL.ChannelsArrangements;

namespace ATL.AudioData.IO
{
    /// <summary>
    /// Class for True Audio files manipulation (extensions : .TTA)
    /// </summary>
	class TTA : IAudioDataIO
    {
        private const string TTA_SIGNATURE = "TTA1";

        // Private declarations
        private uint audioFormat;
        private uint bitsPerSample;
        private uint sampleRate;
        private uint samplesSize;
        private uint cRC32;

        private double bitrate;
        private double duration;
        private ChannelsArrangement channelsArrangement;
        private bool isValid;

        private SizeInfo sizeInfo;
        private readonly string filePath;


        // Public declarations    
        public double CompressionRatio => getCompressionRatio();
        public uint Samples => samplesSize;

        // ---------- INFORMATIVE INTERFACE IMPLEMENTATIONS & MANDATORY OVERRIDES

        public int SampleRate => (int)sampleRate;
        public bool IsVBR => false;
        public Format AudioFormat
        {
            get;
        }
        public int CodecFamily => AudioDataIOFactory.CF_LOSSY;
        public string FileName => filePath;
        public double BitRate => bitrate;
        public int BitDepth => (int)bitsPerSample;
        public double Duration => duration;
        public ChannelsArrangement ChannelsArrangement => channelsArrangement;
        public bool IsMetaSupported(MetaDataIOFactory.TagType metaDataType)
        {
            return (metaDataType == MetaDataIOFactory.TagType.APE) || (metaDataType == MetaDataIOFactory.TagType.ID3V1) || (metaDataType == MetaDataIOFactory.TagType.ID3V2);
        }
        public long AudioDataOffset { get; set; }
        public long AudioDataSize { get; set; }


        // ---------- CONSTRUCTORS & INITIALIZERS

        private void resetData()
        {
            duration = 0;
            bitrate = 0;
            isValid = false;

            audioFormat = 0;
            bitsPerSample = 0;
            sampleRate = 0;
            samplesSize = 0;
            cRC32 = 0;

            AudioDataOffset = -1;
            AudioDataSize = 0;
        }

        public TTA(string filePath, Format format)
        {
            this.filePath = filePath;
            AudioFormat = format;
            resetData();
        }


        // ---------- SUPPORT METHODS

        private double getCompressionRatio()
        {
            // Get compression ratio
            if (isValid)
                return (double)sizeInfo.FileSize / (samplesSize * (channelsArrangement.NbChannels * bitsPerSample / 8) + 44) * 100;
            else
                return 0;
        }

        public bool Read(Stream source, SizeInfo sizeInfo, MetaDataIO.ReadTagParams readTagParams)
        {
            this.sizeInfo = sizeInfo;
            resetData();
            source.Seek(sizeInfo.ID3v2Size, SeekOrigin.Begin);

            bool result = false;

            byte[] buffer = new byte[4];
            source.Read(buffer, 0, buffer.Length);
            if (TTA_SIGNATURE.Equals(Utils.Latin1Encoding.GetString(buffer)))
            {
                isValid = true;

                AudioDataOffset = source.Position - 4;
                AudioDataSize = sizeInfo.FileSize - sizeInfo.APESize - sizeInfo.ID3v1Size - AudioDataOffset;

                source.Read(buffer, 0, 2);
                audioFormat = StreamUtils.DecodeUInt16(buffer);
                source.Read(buffer, 0, 2);
                channelsArrangement = GuessFromChannelNumber(StreamUtils.DecodeUInt16(buffer));
                source.Read(buffer, 0, 2);
                bitsPerSample = StreamUtils.DecodeUInt16(buffer);
                source.Read(buffer, 0, 4);
                sampleRate = StreamUtils.DecodeUInt32(buffer);
                source.Read(buffer, 0, 4);
                samplesSize = StreamUtils.DecodeUInt32(buffer);
                source.Read(buffer, 0, 4);
                cRC32 = StreamUtils.DecodeUInt32(buffer);

                bitrate = (sizeInfo.FileSize - sizeInfo.TotalTagSize) * 8.0 / (samplesSize * 1000.0 / sampleRate);
                duration = samplesSize * 1000.0 / sampleRate;

                result = true;
            }

            return result;
        }


    }
}
