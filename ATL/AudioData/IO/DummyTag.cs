using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ATL.AudioData.IO
{
    /// <summary>
    /// Dummy metadata provider
    /// </summary>
    public partial class DummyTag : MetaDataHolder, IMetaDataIO
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DummyTag()
        {
            Logging.LogDelegator.GetLogDelegate()(Logging.Log.LV_DEBUG, "Instancing a Dummy Meta Data Reader");
        }

        /// <inheritdoc/>
        protected override MetaDataIOFactory.TagType getImplementedTagType()
        {
            return MetaDataIOFactory.TagType.ID3V2;
        }

        /// <inheritdoc/>
        public override IList<Format> MetadataFormats
        {
            get { return new List<Format>(new[] { Format.UNKNOWN_FORMAT }); }
        }

        /// <inheritdoc/>
        public bool Exists => true;

        /// <inheritdoc/>
        public long Size => 0;

        /// <inheritdoc/>
        public long PaddingSize => 0;

        /// <inheritdoc/>
        [Zomp.SyncMethodGenerator.CreateSyncVersion]
        public Task<bool> WriteAsync(Stream s, TagData tag, ProgressToken<float> writeProgress = null)
        {
            return Task.FromResult(true);
        }
        /// <inheritdoc/>
        public bool Read(Stream source, MetaDataIO.ReadTagParams readTagParams)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public bool Remove(Stream s)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public Task<bool> RemoveAsync(Stream s)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public void SetEmbedder(IMetaDataEmbedder embedder)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public void Clear()
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc/>
        public string MapField(TagData.Field field) => "";

        /// <inheritdoc/>
        public bool IsValidFieldKey(string key) => false;
    }
}
