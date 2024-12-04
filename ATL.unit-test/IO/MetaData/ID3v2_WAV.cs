﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATL.AudioData;
using System.Collections.Generic;
using ATL.AudioData.IO;

namespace ATL.test.IO.MetaData
{
    [TestClass]
    public class ID3v2_WAV : MetaIOTest
    {
        public ID3v2_WAV()
        {
            emptyFile = "WAV/empty.wav";
            notEmptyFile = "WAV/audacityTags.wav";
            tagType = MetaDataIOFactory.TagType.ID3V2;

            // Initialize specific test data
            testData = new TagHolder();

            testData.Title = "yeah";
            testData.Artist = "artist";
            testData.AdditionalFields.Add(new KeyValuePair<string, string>("TES2", "Test父"));
        }

        [TestMethod]
        public void TagIO_R_WAV_ID3v2()
        {
            string location = TestUtils.GetResourceLocationRoot() + notEmptyFile;
            AudioDataManager theFile = new AudioDataManager(AudioDataIOFactory.GetInstance().GetFromPath(location));

            readExistingTagsOnFile(theFile);
        }

        [TestMethod]
        public void TagIO_RW_WAV_ID3v2_Existing()
        {
            test_RW_Existing(notEmptyFile, 0, true, false, false); // Not same size after edit because original ID3v2.3 is remplaced by ATL ID3v2.4
        }

        [TestMethod]
        public void TagIO_RW_WAV_ID3v2_Empty()
        {
            test_RW_Empty(emptyFile, true, true, true, true);
        }
    }
}
