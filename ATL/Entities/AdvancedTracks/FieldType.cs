namespace ATL.Entities.AdvancedTracks
{
    enum FieldType
    {
        Invalid, // if a string key is not supported by a specific metadata format
        Mapped, // if a string key is mapped to a generic field (e.g. Album => TALB (id3v23))
        Custom // if a string key is NOT mapped to a generic field (e.g. TXXX:CUSTOM id3v23)
    }
}