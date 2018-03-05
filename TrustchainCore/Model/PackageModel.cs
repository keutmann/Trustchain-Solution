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
        public bool ShouldSerializeAlgorithm() { return !string.IsNullOrWhiteSpace(Algorithm); }

        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }
        public bool ShouldSerializeId() { return Id != null && Id.Length > 0; }

        [JsonProperty(PropertyName = "trusts", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Trust> Trusts { get; set; }
        public bool ShouldSerializeTrusts() { return Trusts != null && Trusts.Count > 0; }

        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public Identity Server { get; set; }

        [JsonProperty(PropertyName = "timestamps", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Timestamp> Timestamps { get; set; }
        public bool ShouldSerializeTimestamps() {  return Timestamps != null && Timestamps.Count > 0; }
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

        [JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        public Identity Issuer { get; set; }

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Subject> Subjects { get; set; }
        public bool ShouldSerializeSubjects() { return Subjects != null && Subjects.Count > 0; }

        [JsonProperty(PropertyName = "claims", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Claim> Claims { get; set; }
        public bool ShouldSerializeClaims() { return Claims != null && Claims.Count > 0; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public TrustTimestamp Timestamp { get; set; }

        [JsonIgnore]
        public int? PackageDatabaseID { get; set; }

        public Trust()
        {
            Algorithm = "double256";
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
        public int TrustDatabaseID { get; set; }

        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        [JsonIgnore]
        [Column(name:"ClaimIndexs")]
        public byte[] DBCI {
            get
            {
                byte[] result = new byte[ClaimIndexs.Length * sizeof(int)];
                Buffer.BlockCopy(ClaimIndexs, 0, result, 0, result.Length);
                return result;
            }
            set
            {
                var bytes = value;
                var size = bytes.Length / sizeof(int);
                var ints = new int[size];
                for (var index = 0; index < size; index++)
                    ints[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
                ClaimIndexs = ints;            
            }
        }

        [JsonProperty(PropertyName = "claimIndexs", NullValueHandling = NullValueHandling.Ignore)]
        [NotMapped]
        public int[] ClaimIndexs { get; set; }

        [JsonProperty(PropertyName = "address")]
        public byte[] Address { get; set; }

        //[JsonProperty(PropertyName = "type")]
        //public string Type { get; set; }
    }

    [Table("Claim")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Claim : DatabaseEntity
    {
        [JsonIgnore]
        public int TrustDatabaseID { get; set; }

        /// <summary>
        /// Optional, specify index, if the possibility of claim object gets reordered.
        /// </summary>
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }
        public bool ShouldSerializeIndex() { return Index >= 0; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public short Cost { get; set; }
        public bool ShouldSerializeCost() { return Cost > 0; }

        [JsonProperty(PropertyName = "activate")]
        public uint Activate { get; set; }
        public bool ShouldSerializeActivate() { return Activate > 0; }

        [JsonProperty(PropertyName = "expire")]
        public uint Expire { get; set; }
        public bool ShouldSerializeExpire() { return Expire > 0; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        public bool ShouldSerializeScope()  { return !string.IsNullOrWhiteSpace(Scope); }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        public bool ShouldSerializeNote() { return Note != null; }
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