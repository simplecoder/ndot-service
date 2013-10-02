using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ndot.Helpers;

namespace Ndot.Tests
{
    [TestClass]
    public class DlBarCodeParserTests
    {
        [TestMethod]
        public void GivenNvBarCode_ReturnsNeededFields()
        {
            var dl = @"@
                ANSI 636049030002DL00410264ZN03050088DLDCAC
                DCBA
                DCDNONE
                DBA03282016
                DCSJONES
                DCTMIKE D
                DBD03102012
                DBB03281988
                DBC1
                DAYBLK
                DAU068 in
                DAG331 MAIN ST
                DAILAS VEGAS
                DAJNV
                DAK891296069  
                DAQ1402041243
                DCF000122820260394621628
                DCGUSA
                DCHNONE
                DAH
                DAZBLACK
                DCE3
                DCK0009682849901
                DCU
                ZNZNAN
                ZNB10102008
                ZNC5'08''
                ZND150
                ZNENCDL
                ZNFNCDL
                ZNGN
                ZNH00096828499
                ZNI00000003399";

            var result = DlBarCodeParser.Parse(dl);

            Assert.AreEqual(result.FirstName, "MIKE");
            Assert.AreEqual(result.MiddleInitial, "D");
            Assert.AreEqual(result.LastName, "JONES");
            Assert.AreEqual(result.Street, "331 MAIN ST");
            Assert.AreEqual(result.City, "LAS VEGAS");
            Assert.AreEqual(result.State, "NV");
            Assert.AreEqual(result.Dob, DateTime.Parse("03/28/1988"));
            Assert.AreEqual(result.DriverLicenseNumber, "1402041243");
            Assert.AreEqual(result.DriverLicenseState, "NV");
        }
    }
}
