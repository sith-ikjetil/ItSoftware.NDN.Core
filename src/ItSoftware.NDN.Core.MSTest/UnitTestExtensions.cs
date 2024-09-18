using ItSoftware.Core.Extensions;
using System.Text;

namespace ItSoftware.NDN.Core.MSTest
{
    [TestClass]
    public class UnitTestExtensions
    {
        [TestMethod]
        public void TestItsWidthExpand()
        {
            var value = "microphone";
            var expectedLeft = "______microphone";
            Assert.AreEqual(expectedLeft, value.ItsWidthExpand(expectedLeft.Length, '_', ItsWidthExpandDirection.Left));

            var expectedRight = "microphone______";
            Assert.AreEqual(expectedRight, value.ItsWidthExpand(expectedRight.Length, '_', ItsWidthExpandDirection.Right));

            var expectedMiddle = "___microphone___";
            Assert.AreEqual(expectedMiddle, value.ItsWidthExpand(expectedMiddle.Length, '_', ItsWidthExpandDirection.Middle));
        }

        [TestMethod]
        public void TestItsToWords()
        {
            Assert.AreEqual(165, System.IO.File.ReadLines("poem.txt").ItsToWords(false).Count());
            Assert.AreEqual(120, System.IO.File.ReadLines("poem.txt").ItsToWords(true).Count());            
        }

        [TestMethod]
        public void TestItsToNumbers()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 XYZ!";
            var expected = 4;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).Count());
        }

        [TestMethod]
        public void TestItsToHexNumbers()
        {
            var value = "ABC 0130, DEF 2010.\n\n2000 1920.20191 0130 XYZ! af4b af5x";
            var expected = 9;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToHexNumbers(false).Count());
        }

        [TestMethod]
        public void TestItsToDouble()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 5;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToDouble(false, new System.Globalization.CultureInfo("en-US")).Count());
        }

        [TestMethod]
        public void TestItsToFloat()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 5;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToFloat(false, new System.Globalization.CultureInfo("en-US")).Count());
        }

        [TestMethod]
        public void TestItsToInt()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 4;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToInt(false).Count());
        }

        [TestMethod]
        public void TestItsToLong()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 4;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToLong(false).Count());
        }

        [TestMethod]
        public void TestItsToShort()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 4;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToShort(false).Count());
        }

        [TestMethod]
        public void TestItsToByte()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 2;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToByte(false).Count());
        }

        [TestMethod]
        public void TestItsToDeciaml()
        {
            var value = "ABC 0130, DEF 2010.\n2000 1920.20191 0130 XYZ!";
            var expected = 5;
            Assert.AreEqual(expected, value.Split("\n").AsEnumerable<string>().ItsToNumbers(false).ItsToDecimal(false, new System.Globalization.CultureInfo("en-US")).Count());
        }

        [TestMethod]
        public void TestItsToSentences()
        {                        
            Assert.AreEqual(13, System.IO.File.ReadLines("poem.txt").ItsToSentences().Count());            
        }

        [TestMethod]
        public void TestItsStringBetweenStrings()
        {
            var value = "_-<h1>test</h1>_-{<h2>test h2</h2>";
            Assert.AreEqual("test", value.ItsStringBetweenStrings("<h1>","</h1>"));
        }

        [TestMethod]
        public void TestItsApplyTagTemplate()
        {
            var source = "test\n<div><a href=\"http://www.itsoftware.no/images/img.png\">picture name</a></div>\ntest<a href=\"http://www.images.com/i.gif\">second name</a>\ntest";
            var tagTemplate = "href=\"{{url}}\">{{name}}</";
            var preTag = "{{";
            var postTag = "}}";            

            var result = source.ItsApplyTagTemplate(tagTemplate, preTag, postTag);
            var countKeys = 0;
            var countValues = 0;
            foreach (var r in result)
            {
                foreach (var k in r.Keys)
                {
                    countKeys++;                    
                    foreach (var v in r[k])
                    {
                        countValues++;
                    }
                }
            }            

            Assert.AreEqual(2, countKeys);
            Assert.AreEqual(4, countValues);
        }

        [TestMethod]
        public void TestItsToDataSizeString()
        {
            var size1 = 1024L * 1024L * 4096L * 1024L;            
            Assert.AreEqual("4 TB", size1.ItsToDataSizeString(0));

            long size2 = 1324L * 1024L * 4096L * 1024L * 3;
            Assert.AreEqual("15,52 TB", size2.ItsToDataSizeString(2));
            Assert.AreEqual("15.52 TB", size2.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
        }

        [TestMethod]
        public void TestItsRenderTimeSpan()
        {
            var value = TimeSpan.FromSeconds(487_965_892);
            
            Assert.AreEqual("15 years 172 days 18 hours 04:52", value.ItsRenderTimeSpan(false));
            Assert.AreEqual("15 years 172 days 18 hours 04:52.000", value.ItsRenderTimeSpan(true));
        }

        [TestMethod]
        public void TestItsHashMD5()
        {
            var value = "__microphone__";
            var expected = "d4cc3fb5dee589855dfacc14a7db13c0";
            Assert.AreEqual(expected, value.ItsHashMD5(Encoding.UTF8));            
        }

        [TestMethod]
        public void TestItsHashSHA1()
        {
            var value = "__microphone__";
            var expected = "4e67e36d8cc9415b5b9742d7379fd1ad6ed47f5b";
            Assert.AreEqual(expected, value.ItsHashSHA1(Encoding.UTF8));
        }

        [TestMethod]
        public void TestItsHashSHA256()
        {
            var value = "__microphone__";
            var expected = "3ed56a46376c9557defc15d8ca57d2882d83e5a407f50981b93aabba8e78791b";
            Assert.AreEqual(expected, value.ItsHashSHA256(Encoding.UTF8));
        }

        [TestMethod]
        public void TestItsHashSHA384()
        {
            var value = "__microphone__";
            var expected = "46758c181e16c9df779006c0d1f2dcfa3e586ce13383d1f87d09e043d78a6e2c91b1c5b1507cd7f84c7f60c80f0c509d";
            Assert.AreEqual(expected, value.ItsHashSHA384(Encoding.UTF8));
        }

        [TestMethod]
        public void TestItsHashSHA512()
        {
            var value = "__microphone__";
            var expected = "e507fd3eb7cb395562678f1c2a85f6c503b98bab3fbfca16e50921c39a647539fa038e6117474f9f96e949eb5622af97c2bde5dba1e03bc7286c5bb4e4bce9ba";
            Assert.AreEqual(expected, value.ItsHashSHA512(Encoding.UTF8));
        }

        [TestMethod]
        public void TestItsNormalizeFilename()
        {
            var value = "M_?:F\\ile|name._%&";
            var expected = "M___F_ile_name._%&";
            Assert.AreEqual(expected, value.ItsNormalizeFileName());
        }

        [TestMethod]
        public void TestItsNormalizeDirectoryName()
        {
            var value = "C:\\\n??||__%&\rvc";
            var expected = "C:\\_??____%&_vc";
            Assert.AreEqual(expected, value.ItsNormalizeDirectoryName());
        }
    }
}