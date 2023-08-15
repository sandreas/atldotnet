using System.Collections.Generic;

namespace ATL.Entities.AdvancedTracks
{
    class TagField
    {
        public TagFormat format { get; set; }
        public string Key { get; set; }
        public TagData.Field? GenericField { get; set; }
        public object? Value { get; set; }
        public object? OriginalValue { get; set; }
        public bool MarkedForDeletion { get; set; }
        public bool IsDirty => EqualityComparer<object>.Default.Equals(OriginalValue, Value);
    }
}