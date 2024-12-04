﻿using ATL.AudioData;
using System.Drawing;
using Commons;
using static ATL.PictureInfo;
using ATL.AudioData.IO;

namespace ATL.test.IO.MetaData
{
    /*
     * TODO TECHNICAL TESTS
     * 
     * Add a standard unsupported field => persisted as standard field in tag
     * Add a non-standard unsupported field => persisted as TXXX field
     * 
    */
    [TestClass]
    public class ID3v2 : MetaIOTest
    {
        public ID3v2()
        {
            emptyFile = "MP3/empty.mp3";
            notEmptyFile = "MP3/id3v2.3_UTF16.mp3";

            testData.PublishingDate = DateTime.Parse("1997-06-22T05:05:05");
            testData.SortAlbum = "SortAlbum";
            testData.SortAlbumArtist = "SortAlbumArtist";
            testData.SortArtist = "SortArtist";
            testData.SortTitle = "SortTitle";
            testData.Group = "Group";
            testData.SeriesTitle = "SeriesTitle";
            testData.SeriesPart = "2";
            testData.LongDescription = "LongDescription";
            testData.BPM = 0;
            testData.InvolvedPeople = "me" + ATL.Settings.InternalValueSeparator + "him" + ATL.Settings.InternalValueSeparator + "you" + ATL.Settings.InternalValueSeparator + "her";
            testData.OriginalReleaseDate = new DateTime(2003, 03, 23);

            tagType = MetaDataIOFactory.TagType.ID3V2;
            titleFieldCode = "TIT2";
            var pics = testData.EmbeddedPictures;
            foreach (PictureInfo pic in pics) pic.TagType = tagType;
            testData.EmbeddedPictures = pics;
        }

        [TestMethod]
        public void TagIO_R_ID3v22_simple()
        {
            string location = TestUtils.GetResourceLocationRoot() + "MP3/ID3v2.2 ANSI charset only.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            Assert.IsTrue(theFile.ReadFromFile(true, true));

            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            // Supported fields
            Assert.AreEqual("noTagnoTag", theFile.ID3v2.Title);
            Assert.AreEqual("ALBUM!", theFile.ID3v2.Album);
            Assert.AreEqual("ARTIST", theFile.ID3v2.Artist);
            Assert.AreEqual("ALBUMARTIST", theFile.ID3v2.AlbumArtist);
            Assert.AreEqual("I have no IDE and i must code", theFile.ID3v2.Comment);
            Assert.AreEqual(1997, theFile.ID3v2.Date.Year);
            Assert.AreEqual("House", theFile.ID3v2.Genre);
            Assert.AreEqual(1, theFile.ID3v2.TrackNumber);
            Assert.AreEqual(2, theFile.ID3v2.TrackTotal);
            Assert.AreEqual("COMP!", theFile.ID3v2.Composer);
            Assert.AreEqual(2, theFile.ID3v2.DiscNumber);
            Assert.AreEqual(3, theFile.ID3v2.DiscTotal);

            // Pictures
            Assert.AreEqual(1, theFile.ID3v2.EmbeddedPictures.Count);

#pragma warning disable CA1416
            byte found = 0;
            foreach (PictureInfo pic in theFile.ID3v2.EmbeddedPictures)
            {
                Image picture;
                if (pic.PicType.Equals(PictureInfo.PIC_TYPE.Generic)) // Supported picture
                {
                    using (picture = Image.FromStream(new MemoryStream(pic.PictureData)))
                    {
                        Assert.AreEqual(System.Drawing.Imaging.ImageFormat.Jpeg, picture.RawFormat);
                        Assert.AreEqual(656, picture.Height);
                        Assert.AreEqual(552, picture.Width);
                    }
                    found++;
                }
            }
            Assert.AreEqual(1, found);
#pragma warning restore CA1416
        }

        [TestMethod]
        public void TagIO_R_ID3v22_UTF16()
        {
            // Source : MP3 with existing tag incl. unsupported picture (Conductor); unsupported field (MOOD)
            String location = TestUtils.GetResourceLocationRoot() + "MP3/ID3v2.2 UTF16.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            Assert.IsTrue(theFile.ReadFromFile(true, true));

            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            // Supported fields
            Assert.AreEqual("bébé", theFile.ID3v2.Title);
            Assert.AreEqual("ALBUM!", theFile.ID3v2.Album);
            Assert.AreEqual("父", theFile.ID3v2.Artist);
            Assert.AreEqual("ALBUMARTIST", theFile.ID3v2.AlbumArtist);
            Assert.AreEqual("I have no IDE and i must code bébé 父", theFile.ID3v2.Comment);
            Assert.AreEqual(1997, theFile.ID3v2.Date.Year);
            Assert.AreEqual("House", theFile.ID3v2.Genre);
            Assert.AreEqual(1, theFile.ID3v2.TrackNumber);
            Assert.AreEqual(2, theFile.ID3v2.TrackTotal);
            Assert.AreEqual("COMP!", theFile.ID3v2.Composer);
            Assert.AreEqual(2, theFile.ID3v2.DiscNumber);
            Assert.AreEqual(3, theFile.ID3v2.DiscTotal);

            // Pictures
            Assert.AreEqual(1, theFile.ID3v2.EmbeddedPictures.Count);

#pragma warning disable CA1416
            byte found = 0;
            foreach (PictureInfo pic in theFile.ID3v2.EmbeddedPictures)
            {
                Image picture;
                if (pic.PicType.Equals(PictureInfo.PIC_TYPE.Generic)) // Supported picture
                {
                    using (picture = Image.FromStream(new MemoryStream(pic.PictureData)))
                    {
                        Assert.AreEqual(System.Drawing.Imaging.ImageFormat.Jpeg, picture.RawFormat);
                        Assert.AreEqual(656, picture.Height);
                        Assert.AreEqual(552, picture.Width);
                    }
                    found++;
                }
            }
            Assert.AreEqual(1, found);
#pragma warning restore CA1416
        }

        [TestMethod]
        public void TagIO_R_ID3v22_3pictures()
        {
            // Source : MP3 with existing tag incl. unsupported picture (Conductor); unsupported field (MOOD)
            String location = TestUtils.GetResourceLocationRoot() + "MP3/ID3v2.2 3 pictures.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            Assert.IsTrue(theFile.ReadFromFile(true, true));

            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);


            // Pictures
            Assert.AreEqual(3, theFile.ID3v2.EmbeddedPictures.Count);

#pragma warning disable CA1416
            byte found = 0;
            foreach (PictureInfo pic in theFile.ID3v2.EmbeddedPictures)
            {
                Image picture;
                if (pic.PicType.Equals(PictureInfo.PIC_TYPE.Generic)) // Supported picture
                {
                    using (picture = Image.FromStream(new MemoryStream(pic.PictureData)))
                    {
                        Assert.AreEqual(System.Drawing.Imaging.ImageFormat.Png, picture.RawFormat);
                        Assert.AreEqual(256, picture.Height);
                        Assert.AreEqual(256, picture.Width);
                    }
                    found++;
                }
            }
            Assert.AreEqual(3, found);
#pragma warning restore CA1416
        }

        [TestMethod]
        public void TagIO_R_ID3v23_UTF16()
        {
            // Source : MP3 with existing tag incl. unsupported picture (Conductor); unsupported field (MOOD)
            string location = TestUtils.GetResourceLocationRoot() + "MP3/id3v2.3_UTF16.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            try
            {
                testData.Date = DateTime.Parse("1997-06-20T04:04:00"); // No seconds in ID3v2.3
                testData.PublishingDate = DateTime.MinValue; // No publishing date in ID3v2.3
                testData.OriginalReleaseDate = new DateTime(2003, 01, 01); // Only year in ID3v2.3
                testData.BPM = 440;
                readExistingTagsOnFile(theFile);
            }
            finally
            {
                testData.Date = DateTime.Parse("1997-06-20T04:04:04");
                testData.PublishingDate = DateTime.Parse("1997-06-22T05:05:05");
                testData.OriginalReleaseDate = new DateTime(2003, 03, 23);
                testData.BPM = 0;
            }
        }

        [TestMethod]
        public void TagIO_R_ID3v24_UTF8()
        {
            // Source : MP3 with existing tag incl. unsupported picture (Conductor); unsupported field (MOOD)
            String location = TestUtils.GetResourceLocationRoot() + "MP3/id3v2.4_UTF8.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            readExistingTagsOnFile(theFile);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_Multiple_Genres()
        {
            // Expected values
            IList<string> expectedGenres = new List<string>
            {
                "Rock",
                "Pop",
                "Country"
            };

            R_ID3v2_Multiple_Genres(TestUtils.GetResourceLocationRoot() + "MP3/id3v2.4_multipleGenres.mp3", expectedGenres);
            R_ID3v2_Multiple_Genres(TestUtils.GetResourceLocationRoot() + "MP3/id3v2.3_UTF16_multipleGenres.mp3", expectedGenres);
            R_ID3v2_Multiple_Genres(TestUtils.GetResourceLocationRoot() + "MP3/id3v2.3_ISO_multipleGenres.mp3", expectedGenres);
        }

        private void R_ID3v2_Multiple_Genres(string location, IList<string> expectedGenres)
        {
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(false, false));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            IList<string> actualGenres = theFile.ID3v2.Genre.Split(ATL.Settings.InternalValueSeparator);
            Assert.AreEqual(expectedGenres.Count, actualGenres.Count);

            for (int i = 0; i < expectedGenres.Count; i++)
                Assert.AreEqual(expectedGenres[i], actualGenres[i]);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_TXXX_MultipleValues()
        {
            // Source : ID3v2 with TXXX MOOD field with multiple values
            String location = TestUtils.GetResourceLocationRoot() + "MP3/id3v2.4_TXXX_multipleValues.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            // Check if field and value are read properly
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            string[] values = theFile.ID3v2.AdditionalFields["MOOD"].Split(ATL.Settings.InternalValueSeparator);
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual("first", values[0]);
            Assert.AreEqual("second", values[1]);
            Assert.AreEqual("third", values[2]);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_TXXX_MultipleValues()
        {
            // Source : ID3v2 with TXXX MOOD field with multiple values
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_TXXX_multipleValues.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // 2- Add and read
            TagData theTag = new TagData();
            theTag.AdditionalFields = new List<MetaFieldInfo>();
            MetaFieldInfo info = new MetaFieldInfo(MetaDataIOFactory.TagType.ID3V2, "MOOD", "a" + ATL.Settings.DisplayValueSeparator + "b" + ATL.Settings.DisplayValueSeparator + "c");
            theTag.AdditionalFields.Add(info);

            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, tagType).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            string[] values = theFile.ID3v2.AdditionalFields["MOOD"].Split(ATL.Settings.InternalValueSeparator);
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual("a", values[0]);
            Assert.AreEqual("b", values[1]);
            Assert.AreEqual("c", values[2]);

            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_WXXX_UnicodeNoDesc()
        {
            // Source : ID3v2 with COM field with unicode encoding on the content description, without any content description
            String location = TestUtils.GetResourceLocationRoot() + "MP3/ID3v2.4-WXX-UnicodeMarkerWithoutDescription.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual("http://remix.evillich.com", theFile.ID3v2.AdditionalFields["WXXX"].Split(ATL.Settings.InternalValueSeparator)[1]);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_WXXX_ManualUpdate()
        {
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_UTF8.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            TagData theTag = new TagData();
            theTag.AdditionalFields = new List<MetaFieldInfo>();
            MetaFieldInfo info = new MetaFieldInfo(MetaDataIOFactory.TagType.ID3V2, "WXXX", "http://justtheurl.com");
            theTag.AdditionalFields.Add(info);

            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, tagType).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(false, true));

            Assert.IsNotNull(theFile.getMeta(tagType));
            IMetaDataIO meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.IsTrue(meta.AdditionalFields.ContainsKey("WXXX"));
            Assert.AreEqual("http://justtheurl.com", meta.AdditionalFields["WXXX"].Split(ATL.Settings.InternalValueSeparator)[1]);

            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v24_Extended()
        {
            ArrayLogger logger = new ArrayLogger();

            // Source : MP3 with extended tag properties (tag restrictions)
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_UTF8_extendedTag.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            try
            {
                testData.PublishingDate = DateTime.MinValue; // Don't wanna re-edit the test file manually to add this one; it has been tested elsewhere

                // Check that the presence of an extended tag does not disrupt field reading
                readExistingTagsOnFile(theFile);

                ATL.Settings.ID3v2_useExtendedHeaderRestrictions = true;

                // Insert a very long field while tag restrictions specify that string shouldn't be longer than 30 characters
                TagHolder theTag = new TagHolder();
                theTag.Conductor = "Veeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeery long field";

                // Insert a large picture while tag restrictions specify that pictures shouldn't be larger than 64x64pixels AND tag size shouldn't be larger than 4 KB
                var testPics = new List<PictureInfo>();
                PictureInfo picInfo = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpg"), PictureInfo.PIC_TYPE.Back);
                testPics.Add(picInfo);
                theTag.EmbeddedPictures = testPics;

                // Insert a gif picture while tag restrictions specify that pictures should be either jpeg or png
                picInfo = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.gif"), PictureInfo.PIC_TYPE.Back);
                testPics.Add(picInfo);
                theTag.EmbeddedPictures = testPics;

                // Insert 20 garbage fields to raise the number of field above maximum required fields (30)
                var testAddFields = new Dictionary<string, string>();
                testAddFields.Add("GA01", "aaa");
                testAddFields.Add("GA02", "aaa");
                testAddFields.Add("GA03", "aaa");
                testAddFields.Add("GA04", "aaa");
                testAddFields.Add("GA05", "aaa");
                testAddFields.Add("GA06", "aaa");
                testAddFields.Add("GA07", "aaa");
                testAddFields.Add("GA08", "aaa");
                testAddFields.Add("GA09", "aaa");
                testAddFields.Add("GA10", "aaa");
                testAddFields.Add("GA11", "aaa");
                testAddFields.Add("GA12", "aaa");
                testAddFields.Add("GA13", "aaa");
                testAddFields.Add("GA14", "aaa");
                testAddFields.Add("GA15", "aaa");
                testAddFields.Add("GA16", "aaa");
                testAddFields.Add("GA17", "aaa");
                testAddFields.Add("GA18", "aaa");
                testAddFields.Add("GA19", "aaa");
                testAddFields.Add("GA20", "aaa");
                theTag.AdditionalFields = testAddFields;


                // Add the new tag and check that it has been indeed added with all the correct information
                Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

                // Get rid of the working copy
                if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
            }
            finally
            {
                testData.PublishingDate = DateTime.Parse("1997-06-22T05:05:05");
                ATL.Settings.ID3v2_useExtendedHeaderRestrictions = false;
            }

            bool isAlertFieldLength = false;
            bool isAlertTagSize = false;
            bool isAlertNbFrames = false;
            bool isAlertPicDimension = false;
            bool isAlertPicType = false;

            foreach (Logging.Log.LogItem logItem in logger.Items)
            {
                if (logItem.Message.Contains("is longer than authorized")) isAlertFieldLength = true;
                if (logItem.Message.StartsWith("Tag is too large")) isAlertTagSize = true;
                if (logItem.Message.StartsWith("Tag has too many frames")) isAlertNbFrames = true;
                if (logItem.Message.EndsWith("does not respect ID3v2 restrictions (exactly 64x64)")) isAlertPicDimension = true;
                if (logItem.Message.EndsWith("does not respect ID3v2 restrictions (jpeg or png required)")) isAlertPicType = true;
            }

            Assert.IsTrue(isAlertFieldLength);
            Assert.IsTrue(isAlertTagSize);
            Assert.IsTrue(isAlertNbFrames);
            Assert.IsTrue(isAlertPicDimension);
            Assert.IsTrue(isAlertPicType);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Empty()
        {
            test_RW_Empty(emptyFile, true, true, true, true);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Existing()
        {
            // Final size and hash checks NOT POSSIBLE YET mainly due to tag order and padding differences
            test_RW_Existing(notEmptyFile, 2, true, false, false);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Unsupported_Empty()
        {
            test_RW_Unsupported_Empty(emptyFile);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_NonStandardField()
        {
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/empty.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Add a field outside ID3v2 standards
            TagData theTag = new TagData();
            theTag.AdditionalFields = new List<MetaFieldInfo>();
            MetaFieldInfo info = new MetaFieldInfo(MetaDataIOFactory.TagType.ID3V2, "BLAHBLAH", "heyheyhey");
            theTag.AdditionalFields.Add(info);

            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, tagType).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.IsTrue(theFile.ID3v2.AdditionalFields.ContainsKey("BLAHBLAH"));
            Assert.AreEqual("heyheyhey", theFile.ID3v2.AdditionalFields["BLAHBLAH"]);

            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        private void checkTrackDiscZeroes(FileStream fs)
        {
            fs.Seek(0, SeekOrigin.Begin);
            Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("TPOS")));
            fs.Seek(7, SeekOrigin.Current);
            string s = StreamUtils.ReadNullTerminatedString(fs, System.Text.Encoding.ASCII);
            Assert.AreEqual("03/04", s);

            fs.Seek(0, SeekOrigin.Begin);
            Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("TRCK")));
            fs.Seek(7, SeekOrigin.Current);
            s = StreamUtils.ReadNullTerminatedString(fs, System.Text.Encoding.ASCII);
            Assert.AreEqual("06/06", s);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_UpdateKeepTrackDiscZeroes()
        {
            StreamDelegate dlg = checkTrackDiscZeroes;
            test_RW_UpdateTrackDiscZeroes("MP3/id3v2.4_UTF8.mp3", false, false, dlg);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_UpdateFormatTrackDiscZeroes()
        {
            StreamDelegate dlg = checkTrackDiscZeroes;
            test_RW_UpdateTrackDiscZeroes("MP3/id3v2.4_UTF8_singleDigitTrackTags.mp3", true, true, dlg);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_iTunCommentFields()
        {
            new ConsoleLogger();

            // Source : MP3 with existing tag incl. comment fields (iTunNORM, iTunPGAP)
            String testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.2_iTunNORM-iTunPGAP.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual(3, theFile.ID3v2.AdditionalFields.Count);

            int found = 0;
            foreach (KeyValuePair<string, string> field in theFile.ID3v2.AdditionalFields)
            {
                if (field.Key.Equals("iTunNORM"))
                {
                    Assert.AreEqual(" 00000099 000000A2 000002F0 000002F4 0000002E 0000002E 00002E6E 00002C5C 00000017 00000017", field.Value); // Why an empty space at the beginning ?!
                    found++;
                }
                else if (field.Key.Equals("iTunPGAP"))
                {
                    Assert.AreEqual("1", field.Value);
                    found++;
                }
            }
            Assert.AreEqual(2, found);


            // Check if they are persisted as comment fields when editing the tag
            TagData theTag = new TagData();
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            // For this we need to open the file in binary mode and check that the two fields belong to a comment field
            byte[] readBytes = new byte[4];
            byte[] expected = Utils.Latin1Encoding.GetBytes("COMM");

            using (FileStream fs = new FileStream(testFileLocation, FileMode.Open, FileAccess.Read))
            {
                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("iTunNORM")));
                fs.Seek(-22, SeekOrigin.Current);
                fs.Read(readBytes, 0, 4);
                Assert.IsTrue(expected.SequenceEqual(readBytes));

                fs.Seek(0, SeekOrigin.Begin);
                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("iTunPGAP")));
                fs.Seek(-22, SeekOrigin.Current);
                fs.Read(readBytes, 0, 4);
                Assert.IsTrue(expected.SequenceEqual(readBytes));
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_FieldCodev22Tov24()
        {
            new ConsoleLogger();

            // Source : MP3 with existing unsupported fields : RVA & TBP
            String testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.2_iTunNORM-iTunPGAP.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual(1997, theFile.ID3v2.Date.Year);

            int found = 0;
            string rvaValue = "";
            foreach (KeyValuePair<string, string> field in theFile.ID3v2.AdditionalFields)
            {
                if (field.Key.Equals("RVA"))
                {
                    rvaValue = field.Value;
                    found++;
                }
            }
            Assert.AreEqual(1, found);

            // Check if they are persisted with proper ID3v2.4 field codes when editing the tag
            TagData theTag = new TagData();
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(false, true));

            // 1/ Check if values are the same
            found = 0;
            foreach (KeyValuePair<string, string> field in theFile.ID3v2.AdditionalFields)
            {
                if (field.Key.Equals("RVA2"))
                {
                    Assert.AreEqual(rvaValue, field.Value);
                    found++;
                }
            }
            Assert.AreEqual(1, found);

            Assert.AreEqual(1997, theFile.ID3v2.Date.Year);


            // 2/ Check if they are indeed persisted as "classic" ID3v2 fields, and not as sub-codes inside a TXXX field
            byte[] readBytes = new byte[4];
            byte[] expected = Utils.Latin1Encoding.GetBytes("TXXX");

            using (FileStream fs = new FileStream(testFileLocation, FileMode.Open, FileAccess.Read))
            {
                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("RVA2")));
                fs.Seek(-15, SeekOrigin.Current);
                fs.Read(readBytes, 0, 4);
                Assert.IsFalse(expected.SequenceEqual(readBytes));

                fs.Seek(0, SeekOrigin.Begin);


                expected = Utils.Latin1Encoding.GetBytes("TDRC");

                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("1997")));
                fs.Seek(-15, SeekOrigin.Current);
                fs.Read(readBytes, 0, 4);
                Assert.IsTrue(expected.SequenceEqual(readBytes));
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Chapters_v3()
        {
            ATL.Settings.ID3v2_tagSubVersion = 3;
            try
            {
                TagIO_RW_ID3v2_Chapters();
            }
            finally
            {
                ATL.Settings.ID3v2_tagSubVersion = 4;
            }
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Chapters_v4()
        {
            TagIO_RW_ID3v2_Chapters();
        }

        private void TagIO_RW_ID3v2_Chapters()
        {
            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsFalse(theFile.ID3v2.Exists);

            Dictionary<uint, ChapterInfo> expectedChaps = new Dictionary<uint, ChapterInfo>();

            TagHolder theTag = new TagHolder();
            theTag.ChaptersTableDescription = "Content֍";
            IList<ChapterInfo> testChapters = new List<ChapterInfo>();
            ChapterInfo ch = new ChapterInfo();
            ch.StartTime = 123;
            ch.StartOffset = 456;
            ch.EndTime = 789;
            ch.EndOffset = 101112;
            ch.UniqueID = "";
            ch.Title = "aaa";
            ch.Subtitle = "bbb";
            ch.Url = new ChapterInfo.UrlInfo("ccc", "ddd");
            ch.Picture = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpeg"));
            ch.Picture.ComputePicHash();

            testChapters.Add(ch);
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 1230;
            ch.StartOffset = 4560;
            ch.EndTime = 7890;
            ch.EndOffset = 1011120;
            ch.UniqueID = "002";
            ch.Title = "aaa0";
            ch.Subtitle = "bbb0";
            ch.Url = new ChapterInfo.UrlInfo("ccc", "ddd0");

            testChapters.Add(ch);
            expectedChaps.Add(ch.StartTime, ch);

            theTag.Chapters = testChapters;

            // Check if they are persisted properly
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual("Content֍", theFile.ID3v2.ChaptersTableDescription);
            Assert.AreEqual(2, theFile.ID3v2.Chapters.Count);

            // Check if values are the same
            int found = 0;
            foreach (ChapterInfo chap in theFile.ID3v2.Chapters)
            {
                if (expectedChaps.ContainsKey(chap.StartTime))
                {
                    found++;
                    if (1 == found) Assert.AreNotEqual(chap.UniqueID, expectedChaps[chap.StartTime].UniqueID); // ID of first chapter was empty; ATL has generated a random ID for it
                    else Assert.AreEqual(chap.UniqueID, expectedChaps[chap.StartTime].UniqueID);
                    Assert.AreEqual(chap.StartTime, expectedChaps[chap.StartTime].StartTime);
                    Assert.AreEqual(chap.StartOffset, expectedChaps[chap.StartTime].StartOffset);
                    Assert.AreEqual(chap.EndOffset, expectedChaps[chap.StartTime].EndOffset);
                    Assert.AreEqual(chap.Title, expectedChaps[chap.StartTime].Title);
                    Assert.AreEqual(chap.Subtitle, expectedChaps[chap.StartTime].Subtitle);
                    if (expectedChaps[chap.StartTime].Url != null)
                    {
                        Assert.AreEqual(chap.Url.Url, expectedChaps[chap.StartTime].Url.Url);
                        Assert.AreEqual(chap.Url.Description, expectedChaps[chap.StartTime].Url.Description);
                    }
                    if (expectedChaps[chap.StartTime].Picture != null)
                    {
                        Assert.IsNotNull(chap.Picture);
                        Assert.AreEqual(expectedChaps[chap.StartTime].Picture.PictureHash, chap.Picture.ComputePicHash());
                    }
                }
                else
                {
                    System.Console.WriteLine(chap.StartTime);
                }
            }
            Assert.AreEqual(2, found);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Chapters_CTOCEdgeCases()
        {
            new ConsoleLogger();

            // Source : empty MP3
            String testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Case 1. Setting Track.ChaptersTableDescription alone without setting any chapter shouldn't write any CTOC frame
            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsFalse(theFile.ID3v2.Exists);

            TagHolder theTag = new TagHolder();
            theTag.ChaptersTableDescription = "aaa";

            // Check if they are persisted properly
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsFalse(theFile.ID3v2.Exists);

            // Read the file itself and check that no CTOC frame has been written
            using (FileStream fs = new FileStream(testFileLocation, FileMode.Open))
            {
                Assert.IsFalse(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("CTOC")));
            }


            //Case 2. If Settings.ID3v2_alwaysWriteCTOCFrame to true and at least 1 chapter without setting Track.ChaptersTableDescription shouldn't write any TIT2 subframe

            // Set a chapter but no description
            ChapterInfo ch = new ChapterInfo();
            ch.StartTime = 123;
            ch.StartOffset = 456;
            ch.EndTime = 789;
            ch.EndOffset = 101112;
            ch.UniqueID = "";
            ch.Subtitle = "bbb";
            theTag.ChaptersTableDescription = "";
            theTag.Chapters = new List<ChapterInfo> { ch };

            // Check if they are persisted properly
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            // Read the file itself
            using (FileStream fs = new FileStream(testFileLocation, FileMode.Open))
            {
                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("CTOC")));
                Assert.IsFalse(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("TIT2")));
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Chapters_Existing()
        {
            new ConsoleLogger();

            // Source : MP3 with existing tag incl. chapters
            String testFileLocation = TestUtils.CopyAsTempTestFile("MP3/chapters.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual("toplevel toc", theFile.ID3v2.ChaptersTableDescription);
            Assert.AreEqual(9, theFile.ID3v2.Chapters.Count);

            Dictionary<uint, ChapterInfo> expectedChaps = new Dictionary<uint, ChapterInfo>();

            ChapterInfo ch = new ChapterInfo();
            ch.StartTime = 0;
            ch.Title = "Intro";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/");
            ch.Picture = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "MP3/chapterImage1.jpg"));
            ch.Picture.ComputePicHash();
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 15000;
            ch.Title = "Creating a new production";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/engine/upload/");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 22000;
            ch.Title = "Sound analysis";
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 34000;
            ch.Title = "Adaptive leveler";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/audio_examples%23leveler");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 45000;
            ch.Title = "Global loudness normalization";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/audio_examples%23loudnorm");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 60000;
            ch.Title = "Audio restoration algorithms";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/audio_examples%23denoise");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 76000;
            ch.Title = "Output file formats";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "http://auphonic.com/blog/5/");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 94000;
            ch.Title = "External services";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "http://auphonic.com/blog/16/");
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 111500;
            ch.Title = "Get a free account!";
            ch.Url = new ChapterInfo.UrlInfo("chapter url", "https://auphonic.com/accounts/register");
            expectedChaps.Add(ch.StartTime, ch);

            int found = 0;
            foreach (ChapterInfo chap in theFile.ID3v2.Chapters)
            {
                if (expectedChaps.ContainsKey(chap.StartTime))
                {
                    found++;
                    Assert.AreEqual(chap.StartTime, expectedChaps[chap.StartTime].StartTime);
                    Assert.AreEqual(chap.Title, expectedChaps[chap.StartTime].Title);
                    if (expectedChaps[chap.StartTime].Url != null)
                    {
                        Assert.AreEqual(chap.Url.Url, expectedChaps[chap.StartTime].Url.Url);
                        Assert.AreEqual(chap.Url.Description, expectedChaps[chap.StartTime].Url.Description);
                    }
                    if (expectedChaps[chap.StartTime].Picture != null)
                    {
                        Assert.IsNotNull(chap.Picture);
                        Assert.AreEqual(expectedChaps[chap.StartTime].Picture.PictureHash, chap.Picture.ComputePicHash());
                    }
                }
                else
                {
                    System.Console.WriteLine(chap.StartTime);
                }
            }
            Assert.AreEqual(9, found);


            // Modify elements
            TagHolder theTag = new TagHolder();
            theTag.ChaptersTableDescription = "Content֍";
            expectedChaps.Clear();

            IList<ChapterInfo> testChapters = new List<ChapterInfo>();
            ch = new ChapterInfo();
            ch.StartTime = 123;
            ch.StartOffset = 456;
            ch.EndOffset = 101112;
            ch.UniqueID = "";
            ch.Title = "aaa";
            ch.Subtitle = "bbb";
            ch.Url = new ChapterInfo.UrlInfo("ccc", "ddd");
            ch.Picture = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpeg"));
            ch.Picture.ComputePicHash();

            testChapters.Add(ch);
            expectedChaps.Add(ch.StartTime, ch);

            ch = new ChapterInfo();
            ch.StartTime = 1230;
            ch.StartOffset = 4560;
            ch.EndOffset = 1011120;
            ch.UniqueID = "002";
            ch.Title = "aaa0";
            ch.Subtitle = "bbb0";
            ch.Url = new ChapterInfo.UrlInfo("ccc", "ddd0");

            testChapters.Add(ch);
            expectedChaps.Add(ch.StartTime, ch);

            theTag.Chapters = testChapters;

            // Check if they are persisted properly
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual("Content֍", theFile.ID3v2.ChaptersTableDescription);
            Assert.AreEqual(2, theFile.ID3v2.Chapters.Count);

            // Check if values are the same
            found = 0;
            foreach (ChapterInfo chap in theFile.ID3v2.Chapters)
            {
                if (expectedChaps.ContainsKey(chap.StartTime))
                {
                    found++;
                    if (1 == found) Assert.AreNotEqual(chap.UniqueID, expectedChaps[chap.StartTime].UniqueID); // ID of first chapter was empty; ATL has generated a random ID for it
                    else Assert.AreEqual(chap.UniqueID, expectedChaps[chap.StartTime].UniqueID);
                    Assert.AreEqual(chap.StartTime, expectedChaps[chap.StartTime].StartTime);
                    Assert.AreEqual(chap.StartOffset, expectedChaps[chap.StartTime].StartOffset);
                    Assert.AreEqual(chap.EndOffset, expectedChaps[chap.StartTime].EndOffset);
                    Assert.AreEqual(chap.Title, expectedChaps[chap.StartTime].Title);
                    Assert.AreEqual(chap.Subtitle, expectedChaps[chap.StartTime].Subtitle);
                    if (expectedChaps[chap.StartTime].Url != null)
                    {
                        Assert.AreEqual(expectedChaps[chap.StartTime].Url.Url, chap.Url.Url);
                        Assert.AreEqual(expectedChaps[chap.StartTime].Url.Description, chap.Url.Description);
                    }
                    if (expectedChaps[chap.StartTime].Picture != null)
                    {
                        Assert.IsNotNull(chap.Picture);
                        Assert.AreEqual(expectedChaps[chap.StartTime].Picture.PictureHash, chap.Picture.ComputePicHash());
                    }
                }
                else
                {
                    System.Console.WriteLine(chap.StartTime);
                }
            }
            Assert.AreEqual(2, found);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_UrlFrames()
        {
            new ConsoleLogger();

            // Source : MP3 with existing tag incl. chapters
            String testFileLocation = TestUtils.CopyAsTempTestFile("MP3/chapters.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Check if the two fields are indeed accessible
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.IsTrue(theFile.ID3v2.AdditionalFields.ContainsKey("WPUB"));
            Assert.AreEqual("http://auphonic.com/", theFile.ID3v2.AdditionalFields["WPUB"]);

            // Check if URLs are persisted properly, i.e. without encoding byte
            TagData theTag = new TagData();
            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

            Assert.IsTrue(theFile.ReadFromFile(false, true));

            // 1/ Check value through ATL
            Assert.IsTrue(theFile.ID3v2.AdditionalFields.ContainsKey("WPUB"));
            Assert.AreEqual("http://auphonic.com/", theFile.ID3v2.AdditionalFields["WPUB"]);

            // 2/ Check absence of encoding field in the file itself
            using (FileStream fs = new FileStream(testFileLocation, FileMode.Open, FileAccess.Read))
            {
                Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("WPUB")));
                fs.Seek(6, SeekOrigin.Current);
                Assert.IsTrue(fs.ReadByte() > 10); // i.e. byte is not a 'text encoding descriptor'
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_Rating()
        {
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/0.mp3", 0, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/0.5.mp3", 0.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/1.mp3", 1.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/1.5.mp3", 1.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/2.mp3", 2.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/2.5.mp3", 2.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/3.mp3", 3.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/3.5.mp3", 3.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/4.mp3", 4.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/4.5.mp3", 4.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/5.mp3", 1, MetaDataIOFactory.TagType.ID3V2);

            assumeRatingInFile("_Ratings/musicBee_3.1.6512/0.mp3", 0, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/0.5.mp3", 0.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/1.mp3", 1.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/1.5.mp3", 1.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/2.mp3", 2.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/2.5.mp3", 2.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/3.mp3", 3.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/3.5.mp3", 3.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/4.mp3", 4.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/4.5.mp3", 4.5 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/5.mp3", 1, MetaDataIOFactory.TagType.ID3V2);

            assumeRatingInFile("_Ratings/windows7/0.mp3", 0, MetaDataIOFactory.TagType.ID3V2, true);
            assumeRatingInFile("_Ratings/windows7/1.mp3", 1.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/windows7/2.mp3", 2.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/windows7/3.mp3", 3.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/windows7/4.mp3", 4.0 / 5, MetaDataIOFactory.TagType.ID3V2);
            assumeRatingInFile("_Ratings/windows7/5.mp3", 1, MetaDataIOFactory.TagType.ID3V2);
        }

        [TestMethod]
        public void TagIO_R_ID3v2_HugeTrackAlbumNum()
        {
            String location = TestUtils.GetResourceLocationRoot() + "MP3/hugeAlbumTrackNumber.mp3";
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            Assert.IsTrue(theFile.ReadFromFile());

            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            // Supported fields
            Assert.AreEqual(0, theFile.ID3v2.DiscNumber);
            Assert.AreEqual(0, theFile.ID3v2.TrackNumber);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Lyrics_Unsynched()
        {
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/ID3v2.4-USLT_JP_eng.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Read
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.IsTrue(theFile.ID3v2.Lyrics.UnsynchronizedLyrics.StartsWith("JAPANESE:\r\n\r\n煙と雲\r\n\r\n世の中を"));

            // Write
            TagData theTag = new TagData();
            theTag.Lyrics = new LyricsInfo();
            theTag.Lyrics.LanguageCode = "rus";
            theTag.Lyrics.Description = "anthem";
            theTag.Lyrics.UnsynchronizedLyrics = "Государственный гимн\r\nРоссийской Федерации";

            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());
            Assert.IsTrue(theFile.ReadFromFile(false, true));

            Assert.AreEqual(theTag.Lyrics.LanguageCode, theFile.ID3v2.Lyrics.LanguageCode);
            Assert.AreEqual(theTag.Lyrics.Description, theFile.ID3v2.Lyrics.Description);
            Assert.AreEqual(theTag.Lyrics.UnsynchronizedLyrics, theFile.ID3v2.Lyrics.UnsynchronizedLyrics);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Lyrics_Synched()
        {
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/ID3v2.4-SYLT_cn.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Read
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual("eng", theFile.ID3v2.Lyrics.LanguageCode);
            Assert.AreEqual("CompuPhase SYLT Editor", theFile.ID3v2.Lyrics.Description);
            Assert.AreEqual(LyricsInfo.LyricsType.LYRICS, theFile.ID3v2.Lyrics.ContentType);
            Assert.AreEqual(58, theFile.ID3v2.Lyrics.SynchronizedLyrics.Count);

            Assert.AreEqual("成都", theFile.ID3v2.Lyrics.SynchronizedLyrics[0].Text);
            Assert.AreEqual(1340, theFile.ID3v2.Lyrics.SynchronizedLyrics[0].TimestampMs);


            // Write
            TagData theTag = new TagData();
            theTag.Lyrics = new LyricsInfo();
            theTag.Lyrics.ContentType = LyricsInfo.LyricsType.LYRICS;
            theTag.Lyrics.LanguageCode = "jap";
            theTag.Lyrics.Description = "song";
            theTag.Lyrics.SynchronizedLyrics.Add(new LyricsInfo.LyricsPhrase(12000, "世の"));
            theTag.Lyrics.SynchronizedLyrics.Add(new LyricsInfo.LyricsPhrase(18000, "中を"));

            Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());
            Assert.IsTrue(theFile.ReadFromFile(false, true));

            Assert.AreEqual(theTag.Lyrics.ContentType, theFile.ID3v2.Lyrics.ContentType);
            Assert.AreEqual(theTag.Lyrics.LanguageCode, theFile.ID3v2.Lyrics.LanguageCode);
            Assert.AreEqual(theTag.Lyrics.Description, theFile.ID3v2.Lyrics.Description);
            Assert.AreEqual(theTag.Lyrics.SynchronizedLyrics.Count, theFile.ID3v2.Lyrics.SynchronizedLyrics.Count);
            for (int i = 0; i < theTag.Lyrics.SynchronizedLyrics.Count; i++)
            {
                Assert.AreEqual(theTag.Lyrics.SynchronizedLyrics[i].TimestampMs, theFile.ID3v2.Lyrics.SynchronizedLyrics[i].TimestampMs);
                Assert.AreEqual(theTag.Lyrics.SynchronizedLyrics[i].Text, theFile.ID3v2.Lyrics.SynchronizedLyrics[i].Text);
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Invalidv4SizeDesc()
        {
            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_CTOC_invalidSizeDesc.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // Read
            Assert.IsTrue(theFile.ReadFromFile(false, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsTrue(theFile.ID3v2.Exists);

            Assert.AreEqual(61, theFile.ID3v2.Chapters.Count);
            Assert.AreEqual("Chapter 61", theFile.ID3v2.Chapters[60].Title);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_WriteID3v2_3()
        {
            ATL.Settings.ID3v2_tagSubVersion = 3;
            try
            {
                string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_UTF8.mp3");
                AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

                // Check if the two fields are indeed accessible
                Assert.IsTrue(theFile.ReadFromFile(true, true));
                Assert.IsNotNull(theFile.ID3v2);
                Assert.IsTrue(theFile.ID3v2.Exists);

                readExistingTagsOnFile(theFile);

                // Check if they are persisted with proper ID3v2.3 field codes when editing the tag
                MetaFieldInfo urlLinkMeta = new MetaFieldInfo(MetaDataIOFactory.TagType.ID3V2, "WOAR", "http://moar.minera.ls");
                KeyValuePair<string, string> urlLinkAdd = new KeyValuePair<string, string>("WOAR", "http://moar.minera.ls");
                TagData theTag = new TagData();
                theTag.AdditionalFields.Add(urlLinkMeta);
                Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());

                Assert.IsTrue(theFile.ReadFromFile(true, true));

                testData.Date = DateTime.Parse("1997-06-20T04:04:00"); // No seconds in ID3v2.3
                testData.PublishingDate = DateTime.MinValue; // No publising date in ID3v2.3
                testData.OriginalReleaseDate = new DateTime(2003, 01, 01); // Only year in ID3v2.3
                testData.AdditionalFields.Add(urlLinkAdd);
                readExistingTagsOnFile(theFile);

                // Get rid of the working copy
                if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
            }
            finally
            {
                ATL.Settings.ID3v2_tagSubVersion = 4;
                testData.Date = DateTime.Parse("1997-06-20T04:04:04");
                testData.PublishingDate = DateTime.Parse("1997-06-22T05:05:05");
                testData.OriginalReleaseDate = new DateTime(2003, 03, 23);
            }
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Multiple_Genres()
        {
            string writtenGenre = "Rock" + ATL.Settings.DisplayValueSeparator + "Pop" + ATL.Settings.DisplayValueSeparator + "Country";
            string expectedGenre = writtenGenre.Replace(ATL.Settings.DisplayValueSeparator, ATL.Settings.InternalValueSeparator);

            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));
            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsFalse(theFile.ID3v2.Exists);

            TagHolder newTag = new TagHolder();
            newTag.Genre = writtenGenre;

            theFile.UpdateTagInFileAsync(newTag.tagData, tagType).GetAwaiter().GetResult();

            Assert.IsTrue(theFile.ReadFromFile(false, true));

            Assert.IsNotNull(theFile.getMeta(tagType));
            IMetaDataIO meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.AreEqual(expectedGenre, meta.Genre);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v24_Multiple_Values()
        {
            string writtenValues = "AA" + ATL.Settings.DisplayValueSeparator + "BB" + ATL.Settings.DisplayValueSeparator + "CC";
            string expectedValues = writtenValues.Replace(ATL.Settings.DisplayValueSeparator, ATL.Settings.InternalValueSeparator);

            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            testMultipleValues(theFile, writtenValues, expectedValues);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v23_Multiple_Values()
        {
            string writtenValues = "AA" + ATL.Settings.DisplayValueSeparator + "BB" + ATL.Settings.DisplayValueSeparator + "CC";
            string expectedValues = writtenValues.Replace(ATL.Settings.DisplayValueSeparator, ATL.Settings.InternalValueSeparator);

            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            ATL.Settings.ID3v2_tagSubVersion = 3;
            try
            {
                testMultipleValues(theFile, writtenValues, expectedValues);
            }
            finally
            {
                ATL.Settings.ID3v2_tagSubVersion = 4;
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        private void testMultipleValues(AudioDataManager theFile, string writtenValues, string expectedValues)
        {
            Assert.IsTrue(theFile.ReadFromFile(true, true));
            Assert.IsNotNull(theFile.ID3v2);
            Assert.IsFalse(theFile.ID3v2.Exists);

            TagHolder newTag = new TagHolder();
            newTag.Composer = writtenValues;

            theFile.UpdateTagInFileAsync(newTag.tagData, tagType).GetAwaiter().GetResult();

            Assert.IsTrue(theFile.ReadFromFile(false, true));

            Assert.IsNotNull(theFile.getMeta(tagType));
            IMetaDataIO meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.AreEqual(expectedValues, meta.Composer);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_Picture_Description()
        {
            string picDescription = "now that's a nice pic!";
            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            // 1- Save new
            TagData theTag = new TagData();
            theTag.Pictures = new List<PictureInfo>();
            // 0x03 : front cover according to ID3v2 conventions
            PictureInfo pic = PictureInfo.fromBinaryData(File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpeg"), PIC_TYPE.Unsupported, tagType, 0x03);
            pic.Description = picDescription;
            theTag.Pictures.Add(pic);

            theFile.UpdateTagInFileAsync(theTag, tagType).GetAwaiter().GetResult();

            Assert.IsTrue(theFile.ReadFromFile(true, false));

            Assert.IsNotNull(theFile.getMeta(tagType));
            IMetaDataIO meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.AreEqual(1, meta.EmbeddedPictures.Count);
            pic = meta.EmbeddedPictures[0];
            Assert.AreEqual(picDescription, pic.Description);


            // 2- Update
            theTag.Pictures[0].Description = picDescription + "!!";
            theFile.UpdateTagInFileAsync(theTag, tagType).GetAwaiter().GetResult();

            Assert.IsTrue(theFile.ReadFromFile(true, false));

            Assert.IsNotNull(theFile.getMeta(tagType));
            meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.AreEqual(1, meta.EmbeddedPictures.Count);
            pic = meta.EmbeddedPictures[0];
            Assert.AreEqual(picDescription + "!!", pic.Description);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_AdditionalDateField()
        {
            DateTime now = DateTime.Now;
            new ConsoleLogger();

            // Source : empty MP3
            string testFileLocation = TestUtils.CopyAsTempTestFile(emptyFile);
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            TagHolder theTag = new TagHolder();
            theTag.AdditionalFields = new Dictionary<string, string>
            {
                { "TEST", MetaDataHolder.DATETIME_PREFIX + now.ToFileTime() }
            };
            theFile.UpdateTagInFileAsync(theTag.tagData, tagType).GetAwaiter().GetResult();

            Assert.IsTrue(theFile.ReadFromFile(true, true));

            Assert.IsNotNull(theFile.getMeta(tagType));
            IMetaDataIO meta = theFile.getMeta(tagType);
            Assert.IsTrue(meta.Exists);

            Assert.AreEqual(1, meta.AdditionalFields.Count);
            Assert.IsTrue(meta.AdditionalFields.ContainsKey("TEST"));
            Assert.AreEqual(TrackUtils.FormatISOTimestamp(now), meta.AdditionalFields["TEST"]);

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_PRIV_Existing()
        {
            new ConsoleLogger();

            string testFileLocation = TestUtils.CopyAsTempTestFile("MP3/id3v2.4_PRIV_multipleValues.mp3");
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

            Assert.IsTrue(theFile.ReadFromFile(true, true));

            AudioDataManager theFile2 = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));
            TagHolder theTag = new TagHolder();
            theTag.Comment = "something";
            Assert.IsTrue(theFile2.UpdateTagInFileAsync(theTag.tagData, MetaDataIOFactory.TagType.ID3V2).GetAwaiter().GetResult());
            Assert.IsTrue(theFile2.ReadFromFile(true, true));

            Assert.AreEqual(theFile.ID3v2.AdditionalFields.Count, theFile2.ID3v2.AdditionalFields.Count);
            foreach (var v in theFile.ID3v2.AdditionalFields)
            {
                Assert.IsTrue(theFile2.ID3v2.AdditionalFields.ContainsKey(v.Key));
                Assert.AreEqual(theFile2.ID3v2.AdditionalFields[v.Key], v.Value);
            }

            // Get rid of the working copy
            if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_ID3v1()
        {
            test_RW_Cohabitation(MetaDataIOFactory.TagType.ID3V2, MetaDataIOFactory.TagType.ID3V1);
        }

        [TestMethod]
        public void TagIO_RW_ID3v2_APE()
        {
            test_RW_Cohabitation(MetaDataIOFactory.TagType.ID3V2, MetaDataIOFactory.TagType.APE);
        }
    }
}
