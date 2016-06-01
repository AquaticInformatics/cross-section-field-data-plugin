using System;
using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class FileNames
    {
        public static string GaugingSummary = "gf_gauging_summary.xml";
        public static string SiteDetails = "gf_site_details.xml";
        public static string Panels = "gf_panels.xml";
        public static string Verticals = "gf_verticals.xml";
        public static string MeterDetails = "gf_meter_details.xml";
        public static string MeterCalibrations = "gf_meter_calib.xml";

        public static readonly IReadOnlyDictionary<Type, string> FileNameTypeMap = new Dictionary<Type, string>
        {
            { typeof(GaugingSummary), GaugingSummary },
            { typeof(MeterDetails), MeterDetails },
            { typeof(MeterCalibration), MeterCalibrations },
            { typeof(Panels), Panels },
            { typeof(Verticals), Verticals }
        };
    }
}
