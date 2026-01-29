
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace FoodSphere.Common.Options
{
    public class EnvConnectionStrings
    {
        public const string SectionName = "ConnectionStrings";

        [Required]
        public required string @default { get; init; }
    }

    public class EnvGoogle
    {
        public const string SectionName = "Google";

        [Required]
        public required string client_id { get; init; }
        [Required]
        public required string client_secret { get; init; }
    }

    public class EnvMagicLink
    {
        public const string SectionName = "MagicLink";

        [Required]
        public required string signing_key { get; init; }
        [Required]
        public required string encryption_key { get; init; }
    }

    public class EnvKeyVault
    {
        public const string SectionName = "KeyVault";

        [Required]
        public required string uri { get; init; }
    }

    public class NestedDomain
    {
        [Required]
        public required string hostname { get; init; }

        [Required]
        public required string signing_key { get; init; }
    }

    public class EnvDomain
    {
        public const string SectionName = "Domain";

        [ValidateObjectMembers]
        public required NestedDomain api { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain resource { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain pos { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain master { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain consumer { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain ordering { get; init; }
    }

    public class EnvDomainApi : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "api";
    }

    public class EnvDomainResource : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "resource";
    }

    public class EnvDomainPos : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "pos";
    }

    public class EnvDomainMaster : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "master";
    }

    public class EnvDomainConsumer : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "consumer";
    }

    public class EnvDomainOrdering : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "ordering";
    }
}