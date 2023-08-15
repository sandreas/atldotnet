namespace ATL.Entities.AdvancedTracks
{
    interface IAdvancedTrack
    {
        public object? GetField(TagData.Field field);
        // public T? GetField<T>(TagData.Field field);
        public FieldType SetField(TagData.Field field, object? value, TagFormat? format);

        public object? GetCustomField(string field, TagFormat? format);
        public FieldType SetCustomField(string field, object? value, TagFormat? format);
    }
}