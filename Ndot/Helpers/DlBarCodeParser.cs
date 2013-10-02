using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ndot.Models;

namespace Ndot.Helpers
{
    public class DlBarCodeParser
    {
        public static DlBarCodeData Parse(string rawData)
        {
            var barCodeData = new DlBarCodeData();
            var parts = rawData.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
            var a = parts.FirstOrDefault(c => c.StartsWith("DCT"));
            barCodeData.FirstName = parts.First(c => c.StartsWith("DCT")).Split(' ')[0].Remove(0, 3);
            barCodeData.MiddleInitial = parts.First(c => c.StartsWith("DCT")).Split(' ')[1];
            barCodeData.LastName = parts.First(c => c.StartsWith("DCS")).Remove(0,3);
            barCodeData.Street = parts.First(c => c.StartsWith("DAG")).Remove(0, 3);
            barCodeData.City = parts.First(c => c.StartsWith("DAI")).Remove(0, 3);
            barCodeData.State = parts.First(c => c.StartsWith("DAJ")).Remove(0, 3);
            barCodeData.Dob = DateTime.Parse(parts.First(c => c.StartsWith("DBB")).Remove(0, 3).Insert(2,"/").Insert(5,"/"));
            barCodeData.DriverLicenseNumber = parts.First(c => c.StartsWith("DAQ")).Remove(0, 3);
            barCodeData.DriverLicenseState = parts.First(c => c.StartsWith("DAJ")).Remove(0, 3);
            return barCodeData;
        }
    }
}