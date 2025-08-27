using System;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.SiteProperties
{
    public class SitePropertyPropertyFeature : Entity
    {

        public Guid SitePropertyId { get; protected set; }

        public Guid PropertyFeatureId { get; protected set; }

        private SitePropertyPropertyFeature()
        {

        }

        public SitePropertyPropertyFeature(Guid sitePropertyId, Guid propertyFeatureId)
        {
            SitePropertyId = sitePropertyId;
            PropertyFeatureId = propertyFeatureId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    SitePropertyId,
                    PropertyFeatureId
                };
        }
    }
}