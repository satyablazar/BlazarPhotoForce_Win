using PhotoForce.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoForce.Extensions
{
    public class JsonFileData
    {
        public Gallery gallery { get; set; }
        public string publishState { get; set; }
    }
    public class StandardJsonFileData
    {
        public gallery gallery { get; set; }
        public string publishState { get; set; }
    }

    public class JsonGroupImages
    {
        public int oID { get; set; }
        public string name { get; set; }
        public List<string> images { get; set; }
    }

    public class Gallery
    {
        public string galleryType { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string jobType { get; set; }
        public string eventDate { get; set; }
        public string expirationDate { get; set; }
        public string retakeDate { get; set; }
        public string startShipDate { get; set; }
        public string keyword { get; set; }
        public string welcomeMessage { get; set; }
        public string welcomeImage { get; set; }
        public bool hidden { get; set; }
        public string password { get; set; }
        public string galleryConfig { get; set; }
        public string customDataSpec { get; set; }
        public string priceSheet { get; set; }
        public bool isGreenScreen { get; set; }
        public bool isPreOrder { get; set; }
        public string reference { get; set; }
        public string imageSize { get; set; }

        public List<JsonGroupImages> groups { get; set; }

        public List<Subjects> subjects { get; set; }

    }

    public class Subjects
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string code { get; set; }
        public int subjectID { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string email { get; set; }
        public string notes { get; set; }
        public string organization { get; set; }
        public string refNo { get; set; }
        public string mother { get; set; }
        public string father { get; set; }
        public string year { get; set; }
        public string grade { get; set; }
        public string teacher { get; set; }
        public string homeroom { get; set; }
        public string personalization { get; set; }
        public string jerseyNumber { get; set; }
        public bool yearbookSelection1 { get; set; }
        public bool yearbookSelection2 { get; set; }
        public string expirationDate { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }
        public int galleryGroupOID { get; set; }

        public List<string> images { get; set; }
    }

    public class gallery
    {
        public string galleryType { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string jobType { get; set; }
        public string eventDate { get; set; }
        public string expirationDate { get; set; }
        public string retakeDate { get; set; }
        public string startShipDate { get; set; }
        public string keyword { get; set; }
        public string welcomeMessage { get; set; }
        public string welcomeImage { get; set; }
        public bool hidden { get; set; }
        public string password { get; set; }
        public string galleryConfig { get; set; }
        public string customDataSpec { get; set; }
        public string priceSheet { get; set; }
        public bool isGreenScreen { get; set; }
        public bool isPreOrder { get; set; }
        public string reference { get; set; }
        public string imageSize { get; set; }

        public List<JsonGroupImages> groups { get; set; }
    }
}
