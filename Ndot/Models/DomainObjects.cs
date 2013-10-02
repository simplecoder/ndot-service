using System;
using System.Collections.Generic;
using MongoRepository;
using Newtonsoft.Json;

namespace Ndot.Models
{
    public class Sr1ClientFormData
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public List<ClientActor> Actors { get; set; }
    }

    public class ClientActor
    {
        public string ActorType { get; set; }
        public string PlateNum { get; set; }
        public string PlateState { get; set; }
        public string Vin { get; set; }
        public string DlBarCode { get; set; }
        public bool OwnerSameAsDriver { get; set; }
        public string DlBarCodeOwner { get; set; }
    }

    [JsonObject]
    public class Sr1FormData : Entity
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Actor> Actors { get; set; }
    }

    public class Actor
    {
        public string Type { get; set; }
        public bool OwnerSameAsDriver { get; set; }
        public DriverInfo Driver { get; set; }
        public OwnerInfo Owner { get; set; }
    }

    public class DlBarCodeData
    {
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public DateTime Dob { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseState { get; set; }
    }

    public class VinApiData
    {
        public string Year { get; set; }
        public string Make { get; set; }
        public string BodyType { get; set; }   
    }

    public class DriverInfo
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseState { get; set; }
        public DateTime Dob { get; set; }
        public string LicensePlateNumber { get; set; }
        public string LicensePlateState { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string BodyType { get; set; }
        public string Vin { get; set; }
    }

    public class OwnerInfo
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseState { get; set; }
        public DateTime Dob { get; set; }
    }
}