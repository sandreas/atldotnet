﻿using ATL.AudioData;
using Commons;
using static ATL.PictureInfo;
using System.Text;
using ATL.AudioData.IO;

namespace ATL.test.IO.MetaData
{
    [TestClass]
    public class APE : MetaIOTest
    {
        public APE()
        {
            emptyFile = "MP3/empty.mp3";
            notEmptyFile = "MP3/APE.mp3";
            tagType = MetaDataIOFactory.TagType.APE;
            titleFieldCode = "TITLE";

            // Initialize specific test data (Publisher and Description fields not supported in APE tag)
            testData.Publisher = null;
            testData.GeneralDescription = null;
            testData.Date = DateTime.Parse("01/01/1997");
            testData.Publisher = "abc publishing";
            testData.EncodedBy = "enKoder";
            testData.ISRC = "111-ABC-222-DEF";

            // Initialize specific test data (Picture native codes are strings)
            IList<PictureInfo> testPictureInfos = new List<PictureInfo>();
            PictureInfo pic = PictureInfo.fromBinaryData(
                File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpeg"),
                PIC_TYPE.Unsupported,
                MetaDataIOFactory.TagType.ANY,
                "COVER ART (FRONT)");
            pic.ComputePicHash();
            testPictureInfos.Add(pic);

            pic = PictureInfo.fromBinaryData(
                File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.png"),
                PIC_TYPE.Unsupported,
                MetaDataIOFactory.TagType.ANY,
                "COVER ART (BACK)");
            pic.ComputePicHash();
            testPictureInfos.Add(pic);

            testData.EmbeddedPictures = testPictureInfos;
            supportsExtraEmbeddedPictures = false;
        }


        [TestMethod]
        public void TagIO_R_APE() // My deepest apologies for this dubious method name
        {
            string location = TestUtils.GetResourceLocationRoot() + notEmptyFile;
            AudioDataManager theFile = new AudioDataManager(ATL.AudioData.AudioDataIOFactory.GetInstance().GetFromPath(location));

            readExistingTagsOnFile(theFile);
        }

        [TestMethod]
        public void TagIO_RW_APE_Empty()
        {
            test_RW_Empty(emptyFile, true,true, true, true);
        }

        [TestMethod]
        public void TagIO_RW_APE_Existing()
        {
            // Hash check NOT POSSIBLE YET mainly due to tag order differences
            test_RW_Existing(notEmptyFile, 2, true, true, false);
        }

        [TestMethod]
        public void TagIO_RW_APE_Unsupported_Empty()
        {
            test_RW_Unsupported_Empty(emptyFile);
        }

        [TestMethod]
        public void TagIO_RW_APE_GB18030()
        {
            new ConsoleLogger();

            Encoding initialEncoding = ATL.Settings.DefaultTextEncoding;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ATL.Settings.DefaultTextEncoding = Encoding.GetEncoding("GB18030");
            try
            {
                // Source : totally metadata-free file
                string testFileLocation = TestUtils.CopyAsTempTestFile("APE/GB18030_tags.ape");
                AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(testFileLocation));

                // Check that it is indeed tag-free
                Assert.IsTrue(theFile.ReadFromFile());

                Assert.IsNotNull(theFile.getMeta(tagType));
                IMetaDataIO meta = theFile.getMeta(tagType);

                Assert.AreEqual("03嘿!你写日记吗?", meta.Title);
                Assert.AreEqual("小虎队", meta.Artist);

                TagHolder theTag = new TagHolder();
                theTag.Artist = "小虎队队队";

                Assert.IsTrue(theFile.UpdateTagInFileAsync(theTag.tagData, tagType).GetAwaiter().GetResult());

                Assert.IsTrue(theFile.ReadFromFile(false, true));

                Assert.IsNotNull(theFile.getMeta(tagType));
                meta = theFile.getMeta(tagType);

                Assert.AreEqual("03嘿!你写日记吗?", meta.Title);
                Assert.AreEqual("小虎队队队", meta.Artist);

                // Get rid of the working copy
                if (Settings.DeleteAfterSuccess) File.Delete(testFileLocation);
            }
            finally
            {
                ATL.Settings.DefaultTextEncoding = initialEncoding;
            }
        }

        private void checkTrackDiscZeroes(FileStream fs)
        {
            fs.Seek(0, SeekOrigin.Begin);
            Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("DISCNUMBER")));
            fs.Seek(1, SeekOrigin.Current);
            string s = StreamUtils.ReadNullTerminatedString(fs, Encoding.ASCII);
            Assert.AreEqual("03/04", s.Substring(0, s.Length - 1));

            fs.Seek(0, SeekOrigin.Begin);
            Assert.IsTrue(StreamUtils.FindSequence(fs, Utils.Latin1Encoding.GetBytes("TRACK")));
            fs.Seek(1, SeekOrigin.Current);
            s = StreamUtils.ReadNullTerminatedString(fs, Encoding.ASCII);
            Assert.AreEqual("06/06", s.Substring(0, s.Length - 1));
        }

        [TestMethod]
        public void TagIO_RW_APE_UpdateKeepTrackDiscZeroes()
        {
            StreamDelegate dlg = checkTrackDiscZeroes;
            test_RW_UpdateTrackDiscZeroes(notEmptyFile, false, false, dlg);
        }

        [TestMethod]
        public void TagIO_RW_APE_UpdateFormatTrackDiscZeroes()
        {
            StreamDelegate dlg = checkTrackDiscZeroes;
            test_RW_UpdateTrackDiscZeroes(notEmptyFile, true, true, dlg);
        }

        [TestMethod]
        public void TagIO_R_APE_Rating()
        {
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/0.ape", 0, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/0.5.ape", 0.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/1.ape", 1.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/1.5.ape", 1.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/2.ape", 2.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/2.5.ape", 2.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/3.ape", 3.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/3.5.ape", 3.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/4.ape", 4.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/4.5.ape", 4.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/mediaMonkey_4.1.19.1859/5.ape", 1, MetaDataIOFactory.TagType.APE);

            assumeRatingInFile("_Ratings/musicBee_3.1.6512/0.ape", 0, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/0.5.ape", 0.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/1.ape", 1.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/1.5.ape", 1.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/2.ape", 2.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/2.5.ape", 2.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/3.ape", 3.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/3.5.ape", 3.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/4.ape", 4.0 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/4.5.ape", 4.5 / 5, MetaDataIOFactory.TagType.APE);
            assumeRatingInFile("_Ratings/musicBee_3.1.6512/5.ape", 1, MetaDataIOFactory.TagType.APE);
        }

        [TestMethod]
        public void TagIO_RW_APE_ID3v1()
        {
            test_RW_Cohabitation(MetaDataIOFactory.TagType.APE, MetaDataIOFactory.TagType.ID3V1);
        }

        [TestMethod]
        public void TagIO_RW_APE_ID3v2()
        {
            test_RW_Cohabitation(MetaDataIOFactory.TagType.APE, MetaDataIOFactory.TagType.ID3V2);
        }
    }
}
