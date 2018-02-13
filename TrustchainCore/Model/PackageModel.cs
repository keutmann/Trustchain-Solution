using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    [Table("Package")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Package : DatabaseEntity
    {
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }

        [JsonProperty(PropertyName = "trusts", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Trust> Trusts { get; set; }

        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public Identity Server { get; set; }

        [JsonProperty(PropertyName = "timestamps", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Timestamp> Timestamps { get; set; }

        public bool ShouldSerializeTrusts()
        {
            return Trusts != null && Trusts.Count > 0;
        }

        public bool ShouldSerializeTimestamps()
        {
            return Timestamps != null && Timestamps.Count > 0;
        }

        public Package()
        {
            Algorithm = "merkle.tc1-double256";
        }

    }


    [Table("Trust")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Trust : DatabaseEntity
    {
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }

        [JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        public Identity Issuer { get; set; }

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Subject> Subjects { get; set; }

        [JsonProperty(PropertyName = "claims", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Claim> Claims { get; set; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public TrustTimestamp Timestamp { get; set; }

        public Trust()
        {
            Algorithm = "double256";
        }

        public bool ShouldSerializeSubjects()
        {
            return Subjects != null && Subjects.Count > 0;
        }

        public bool ShouldSerializeClaims()
        {
            return Claims != null && Claims.Count > 0;
        }
    }

    /// <summary>
    /// Signing of an address with data
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="data">The data that is signed</param>
    /// <returns>Signature</returns>
    public delegate byte[] SignDelegate(Identity identity, byte[] data);

    [JsonObject(MemberSerialization.OptIn)]
    public class Identity
    {
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

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

        public Identity()
        {
            Script = "btc-pkh";
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TrustTimestamp
    {
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty(PropertyName = "recipt")]
        public byte[] Recipt { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "timestamps")]
        public string Timestamps { get; set; }

        //[JsonProperty(PropertyName = "timestamps", NullValueHandling = NullValueHandling.Ignore)]
        //public IList<Timestamp> Timestamps { get; set; }

        //public bool ShouldSerializeTimestamps()
        //{
        //    return Timestamps != null && Timestamps.Count > 0;
        //}
    }

    [Table("Subject")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Subject : DatabaseEntity
    {
        [JsonIgnore]
        public int TrustID { get; set; }

        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        [JsonProperty(PropertyName = "claimIndexs", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] ClaimIndexs { get; set; }

        [JsonProperty(PropertyName = "address")]
        public byte[] Address { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    [Table("Claim")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Claim : DatabaseEntity
    {
        [JsonIgnore]
        public int TrustID { get; set; }

        /// <summary>
        /// Optional, specify index, if the possibility of claim object gets reordered.
        /// </summary>
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public short Cost { get; set; }

        [JsonProperty(PropertyName = "activate")]
        public uint Activate { get; set; }

        [JsonProperty(PropertyName = "expire")]
        public uint Expire { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        public bool ShouldSerializeIndex()
        {
            return Index >= 0;
        }

        public bool ShouldSerializeCost()
        {
            return Cost > 0;
        }

        public bool ShouldSerializeActivate()
        {
            return Activate > 0;
        }

        public bool ShouldSerializeExpire()
        {
            return Expire > 0;
        }

        public bool ShouldSerializeNote()
        {
            return Note != null;
        }


    }

    [Table("Timestamp")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Timestamp : DatabaseEntity
    {
        [JsonIgnore]
        public int PackageID { get; set; }

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