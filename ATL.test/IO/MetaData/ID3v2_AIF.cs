﻿ using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATL.AudioData;

namespace ATL.test.IO.MetaData
{
    [TestClass]
    public class ID3v2_AIF : MetaIOTest
    {
        public ID3v2_AIF()
        {
            emptyFile = "AIF/aiff_empty.aif";
            notEmptyFile = "AIF/aifc_tagged.aif";
            tagType = MetaDataIOFactory.TAG_ID3V2;
        }

        [TestMethod]
        public void TagIO_R_AIF_ID3v2()
        {
            // Source : MP3 with existing tag incl. unsupported picture (Conductor); unsupported field (MOOD)
            String location = TestUtils.GetResourceLocationRoot() + notEmptyFile;
            AudioDataManager theFile = new AudioDataManager(AudioData.AudioDataIOFactory.GetInstance().GetDataReader(location));

            readExistingTagsOnFile(theFile);
        }
        
        [TestMethod]
        public void TagIO_RW_AIF_ID3v2_Existing()
        {
            test_RW_Existing(notEmptyFile, 2, true, true);
        }

        [TestMethod]
        public void TagIO_RW_AIF_ID3v2_Empty()
        {
            test_RW_Empty(emptyFile, true, true, true);
        }
    }
}
