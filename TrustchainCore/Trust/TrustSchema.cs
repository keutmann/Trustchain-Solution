using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustchainCore.Trust
{
    public class TrustSchema
    {

        public List<string> Errors { get; set; }

        protected TrustModel trust { get; set; }

        public TrustSchema(TrustModel t)
        {
            trust = t;
            Errors = new List<string>();
        }


        public bool Validate()
        {
            if(trust.Head == null)
                Errors.Add("Missing Head");

            if (string.IsNullOrEmpty(trust.Head.Script))
                Errors.Add("Missing Head Script");

            if (string.IsNullOrEmpty(trust.Head.Version))
                Errors.Add("Missing Head Version");


            if (trust.Issuer == null)
                Errors.Add("Missing Issuer");

            if (trust.Issuer.IssuerId == null || trust.Issuer.IssuerId.Length == 0)
                Errors.Add("Missing issuer id");

            //if (trust.Issuer.Signature == null)
            //    Errors.Add("Missing issuer signature");

            if (trust.Issuer.Subjects == null || trust.Issuer.Subjects.Count == 0)
                Errors.Add("Missing subject");

            var index = 0;
            foreach (var subject in trust.Issuer.Subjects)
            {
                if (subject.SubjectId == null || subject.SubjectId.Length == 0)
                    Errors.Add("Missing subject id at index: "+index);
                index++;
            }

            return Errors.Count == 0;
        }
    }
}
