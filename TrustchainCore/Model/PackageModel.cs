using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    /// <summary>
    /// Signing of an address with data
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="data">The data that is signed</param>
    /// <returns>Signature</returns>
    public delegate byte[] SignDelegate(byte[] data);


    [Table("Package")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Package : DatabaseEntity
    {
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }
        public bool ShouldSerializeAlgorithm() { return !string.IsNullOrWhiteSpace(Algorithm); }

        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }
        public bool ShouldSerializeId() { return Id != null && Id.Length > 0; }

        [JsonProperty(PropertyName = "trusts", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Trust> Trusts { get; set; }
        public bool ShouldSerializeTrusts() { return Trusts != null && Trusts.Count > 0; }

        //[JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        //public Identity Server { get; set; }
        [JsonProperty(PropertyName = "serverScript")]
        public string ServerScript { get; set; }
        public bool ShouldSerializeServerScript() { return !string.IsNullOrWhiteSpace(ServerScript); }

        [JsonProperty(PropertyName = "serverAddress")]
        public byte[] ServerAddress { get; set; }

        /// <summary>
        /// Internal property for holding the private key to sign with
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public SignDelegate ServerSign { get; set; }

        [JsonProperty(PropertyName = "serverSignature")]
        public byte[] ServerSignature { get; set; }
        public bool ShouldSerializeServerSignature() { return ServerSignature != null && ServerSignature.Length > 0; }

        [JsonProperty(PropertyName = "timestamps", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Timestamp> Timestamps { get; set; }
        public bool ShouldSerializeTimestamps() { return Timestamps != null && Timestamps.Count > 0; }
    }


    [Table("Trust")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Trust : DatabaseEntity
    {
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }
        public bool ShouldSerializeAlgorithm() { return !string.IsNullOrWhiteSpace(Algorithm); }

        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }
        public bool ShouldSerializeId() { return Id != null; }

        //[JsonProperty(PropertyName = "created")]
        //public long Created { get; set; }
        //public bool ShouldSerializeCreated() { return Created > 0; }

        //[JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        //public IssuerIdentity Issuer { get; set; }
        [JsonProperty(PropertyName = "issuerScript")]
        public string IssuerScript { get; set; }
        public bool ShouldSerializeIssuerScript() { return !string.IsNullOrWhiteSpace(IssuerScript); }

        [JsonProperty(PropertyName = "issuerAddress")]
        public byte[] IssuerAddress { get; set; }

        /// <summary>
        /// Internal property for holding the private key to sign with
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public SignDelegate IssuerSign { get; set; }

        [JsonProperty(PropertyName = "issuerSignature")]
        public byte[] IssuerSignature { get; set; }
        public bool ShouldSerializeIssuerSignature() { return IssuerSignature != null && IssuerSignature.Length > 0; }

        //[JsonProperty(PropertyName = "subject", NullValueHandling = NullValueHandling.Ignore)]
        //public SubjectIdentity Subject { get; set; }
        [JsonProperty(PropertyName = "subjectScript")]
        public string SubjectScript { get; set; }
        public bool ShouldSerializeSubjectScript() { return !string.IsNullOrWhiteSpace(SubjectScript); }

        [JsonProperty(PropertyName = "subjectAddress")]
        public byte[] SubjectAddress { get; set; }

        /// <summary>
        /// Internal property for holding the private key to sign with
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public SignDelegate SubjectSign { get; set; }

        [JsonProperty(PropertyName = "subjectSignature")]
        public byte[] SubjectSignature { get; set; }
        public bool ShouldSerializeSubjectSignature() { return SubjectSignature != null && SubjectSignature.Length > 0; }


        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "attributes")]
        public string Attributes { get; set; }

        //[JsonProperty(PropertyName = "scopeType")]
        //public string ScopeType { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        public bool ShouldSerializeScope() { return !string.IsNullOrWhiteSpace(Scope); }

        [JsonProperty(PropertyName = "cost")]
        public short Cost { get; set; }
        public bool ShouldSerializeCost() { return Cost > 0; }

        [JsonProperty(PropertyName = "activate")]
        public uint Activate { get; set; }
        public bool ShouldSerializeActivate() { return Activate > 0; }

        [JsonProperty(PropertyName = "expire")]
        public uint Expire { get; set; }
        public bool ShouldSerializeExpire() { return Expire > 0; }

        //[JsonProperty(PropertyName = "note")]
        //public string Note { get; set; }
        //public bool ShouldSerializeNote() { return Note != null; }

        //[JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        //public TrustTimestamp Timestamp { get; set; }
        [JsonProperty(PropertyName = "timestampAlgorithm")]
        public string TimestampAlgorithm { get; set; }
        public bool ShouldSerializeTimestampAlgorithm() { return !string.IsNullOrWhiteSpace(TimestampAlgorithm); }

        [JsonProperty(PropertyName = "timestampRecipt")]
        public byte[] TimestampRecipt { get; set; }
        public bool ShouldSerializeTimestampRecipt() { return TimestampRecipt != null && TimestampRecipt.Length > 0; }


        [UIHint("JSON")]
        [JsonProperty(PropertyName = "timestamps")]
        public string Timestamps { get; set; }
        public bool ShouldSerializeTimestamps() { return !string.IsNullOrWhiteSpace(Timestamps); }


        [JsonIgnore]
        public int? PackageDatabaseID { get; set; }
    }

    //[JsonObject(MemberSerialization.OptIn)]
    //public class TrustTimestamp
    //{

    //    //[JsonProperty(PropertyName = "timestamps", NullValueHandling = NullValueHandling.Ignore)]
    //    //public IList<Timestamp> Timestamps { get; set; }
    //    //public bool ShouldSerializeTimestamps()
    //    //{
    //    //    return Timestamps != null && Timestamps.Count > 0;
    //    //}
    //}


    [JsonObject(MemberSerialization.OptIn)]
    public class Identity
    {
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }
        public bool ShouldSerializeScript() { return !string.IsNullOrWhiteSpace(Script); }

        [JsonProperty(PropertyName = "address")]
        public byte[] Address { get; set; }

        /// <summary>
        /// Internal property for holding the private key to sign with
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public SignDelegate Sign { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public byte[] Signature { get; set; }
        public bool ShouldSerializeSignature()
        {
            return Signature != null && Signature.Length > 0;
        }

    }


    [Table("Timestamp")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Timestamp : DatabaseEntity
    {
        [JsonIgnore]
        public int PackageDatabaseID { get; set; }

        [JsonProperty(PropertyName = "blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty(PropertyName = "time")]
        public int Time { get; set; }

        [JsonProperty(PropertyName = "recipt")]
        public string Recipt { get; set; }

        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }
    }


}