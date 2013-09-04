using System.Runtime.Serialization;
using Microsoft.IdentityServer.PowerShell.Resources;

namespace VflIt.Samples.AdfsSnapIn
{
    public class SerializableClaimDescription
    {
        [DataMember]
        public string ClaimType { get; set; }

        [DataMember]
        public bool IsAccepted { get; set; }

        [DataMember]
        public bool IsOffered { get; set; }

        [DataMember]
        public bool IsRequired { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Notes { get; set; }

        public static SerializableClaimDescription FromClaimDescription(ClaimDescription claimDescription)
        {
            var source = claimDescription;
            var target = new SerializableClaimDescription();

            Map(target, source);

            return target;
        }

        //public ClaimDescription ToClaimDescription()
        //{
        //    var claimDescription = new ClaimDescription();
        //    Map();   
        //}

        private static void Map(SerializableClaimDescription target, ClaimDescription source)
        {
            target.ClaimType = source.ClaimType;
            target.IsAccepted = source.IsAccepted;
            target.IsOffered = source.IsOffered;
            target.IsRequired = source.IsRequired;
            target.Name = source.Name;
            target.Notes = source.Notes;
        }
    }
}