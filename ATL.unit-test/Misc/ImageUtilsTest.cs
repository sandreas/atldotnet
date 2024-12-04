﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Commons;
using System;

namespace ATL.test
{
    [TestClass]
    public class ImageUtilsTest
    {
        [TestMethod]
        public void ImgUtils_MimeAndFormat()
        {
            Assert.IsTrue(ImageFormat.Bmp == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Bmp)));
            Assert.IsTrue(ImageFormat.Tiff == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Tiff)));
            Assert.IsTrue(ImageFormat.Png == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Png)));
            Assert.IsTrue(ImageFormat.Gif == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Gif)));
            Assert.IsTrue(ImageFormat.Jpeg == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Jpeg)));
            Assert.IsTrue(ImageFormat.Webp == ImageUtils.GetImageFormatFromMimeType(ImageUtils.GetMimeTypeFromImageFormat(ImageFormat.Webp)));
        }

        [TestMethod]
        public void ImgUtils_FormatFromHeaderException()
        {
            try
            {
                ImageUtils.GetImageFormatFromPictureHeader(new byte[2]);
                Assert.Fail();
            }
            catch
            {
                // Nothing
            }
        }

        [TestMethod]
        public void ImgUtils_LoadJpeg()
        {
            // THis one has multiple image data segments; never figured out why
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.jpg");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(900, props.Width);
            Assert.AreEqual(600, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(24, props.ColorDepth);

            // This one is plain and simple
            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic2.jpg");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(900, props.Width);
            Assert.AreEqual(290, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(24, props.ColorDepth);
        }

        [TestMethod]
        public void ImgUtils_LoadPng()
        {
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.png");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(175, props.Width);
            Assert.AreEqual(168, props.Height);
            Assert.AreEqual(15, props.NumColorsInPalette);
            Assert.AreEqual(8, props.ColorDepth);
        }

        [TestMethod]
        public void ImgUtils_LoadBmp()
        {
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.bmp");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(175, props.Width);
            Assert.AreEqual(168, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(8, props.ColorDepth);
        }

        [TestMethod]
        public void ImgUtils_LoadGif()
        {
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/pic1.gif");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(175, props.Width);
            Assert.AreEqual(168, props.Height);
            Assert.AreEqual(256, props.NumColorsInPalette);
            Assert.AreEqual(24, props.ColorDepth);
        }

        [TestMethod]
        public void ImgUtils_LoadTiff()
        {
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/bilevel.tif");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(1728, props.Width);
            Assert.AreEqual(2376, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(1, props.ColorDepth);

            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/rgb.tif");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(124, props.Width);
            Assert.AreEqual(124, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(24, props.ColorDepth);

            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/palette.tif");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(2147, props.Width);
            Assert.AreEqual(1027, props.Height);
            Assert.AreEqual(8, props.NumColorsInPalette);
            Assert.AreEqual(8, props.ColorDepth);

            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/bigEndian.tif");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(2464, props.Width);
            Assert.AreEqual(3248, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(1, props.ColorDepth);
        }

        [TestMethod]
        public void ImgUtils_LoadWebp()
        {
            byte[] data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/lossy.webp");
            ImageProperties props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(550, props.Width);
            Assert.AreEqual(368, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(32, props.ColorDepth);


            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/lossless.webp");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(386, props.Width);
            Assert.AreEqual(395, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(32, props.ColorDepth);


            data = File.ReadAllBytes(TestUtils.GetResourceLocationRoot() + "_Images/extended.webp");
            props = ImageUtils.GetImageProperties(data);

            Assert.AreEqual(386, props.Width);
            Assert.AreEqual(395, props.Height);
            Assert.AreEqual(0, props.NumColorsInPalette);
            Assert.AreEqual(32, props.ColorDepth);
        }
    }
}
