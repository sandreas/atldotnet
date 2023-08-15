using System.Collections.Generic;

namespace ATL.Entities.AdvancedTracks
{
    class AdvancedTrack : IAdvancedTrack
    {
        private readonly TagFormat preferredTagFormat;
        private readonly Dictionary<TagFormat, Dictionary<string, object>> metadataFormats = new();
        

        public string? Album
        {
            // todo: better use GetField<string?>(Field.ALBUM)
            get => GetField(TagData.Field.ALBUM)?.ToString();
            set => SetField(TagData.Field.ALBUM, value);
        }

        public AdvancedTrack(TagFormat preferredTagFormat)
        
        {
            // todo: this must be loaded from the file or specified on initialisation
            this.preferredTagFormat = preferredTagFormat;
        }

        public object? GetField(TagData.Field field) => GetField(field, preferredTagFormat);

        public object? GetField(TagData.Field field, TagFormat format)
        {
            if (!metadataFormats.ContainsKey(format))
            {
                return null;
            }

            var stringField = MapFieldToString(format, field);

            return stringField != null && metadataFormats[format].ContainsKey(stringField)
                ? metadataFormats[format][stringField]
                : null;
        }


        // todo: this could be stored in IAudioDataIO, so the mapping could be stored and verified at one place
        
        private static string? MapFieldToString(TagFormat tagFormat, TagData.Field field) => tagFormat switch
        {
            TagFormat.Id3V23 => MapId3V23FieldToString(field),
            _ => null
        };

        private static string? MapId3V23FieldToString(TagData.Field field) => field switch
        {
            TagData.Field.ALBUM => "TALB",
            _ => null
        };


        private static TagData.Field? MapStringToField(TagFormat tagFormat, string field) => tagFormat switch
        {
            TagFormat.Id3V23 => MapId3V23StringToField(field),
            _ => null
        };

        private static TagData.Field? MapId3V23StringToField(string field) => field switch
        {
            "TALB" => TagData.Field.ALBUM,
            _ => null
        };


        public FieldType SetField(TagData.Field field, object? value, TagFormat? format = null)
        {
            var tagFormat = format ?? preferredTagFormat;
            var key = MapFieldToString(tagFormat, field);
            if (key == null) return FieldType.Invalid;
            metadataFormats[tagFormat][key] = value;
            return FieldType.Mapped;
        }


        public object? GetCustomField(string field, TagFormat? format)
        {
            var tagFormat = format ?? preferredTagFormat;
            var mappedField = MapStringToField(tagFormat, field);
            if (mappedField != null)
            {
                return GetField((TagData.Field)mappedField, tagFormat);
            }

            return metadataFormats.ContainsKey(tagFormat) && metadataFormats[tagFormat].ContainsKey(field)
                ? metadataFormats[tagFormat][field]
                : null;
        }

        public FieldType SetCustomField(string field, object? value, TagFormat? format)
        {
            var tagFormat = format ?? preferredTagFormat;
            var mappedField = MapStringToField(tagFormat, field);
            if (mappedField != null)
            {
                SetField((TagData.Field)mappedField, value, tagFormat);
                return FieldType.Mapped;
            }

            // todo: validate, if custom field can be set, otherwise return FieldType.Invalid
            metadataFormats[tagFormat][field] = value;
            return FieldType.Custom;
        }

        /*
        public T? GetField<T>(TagData.Field field)
        {
            // todo: return casted field
            return default;
        }
        */
    }
}