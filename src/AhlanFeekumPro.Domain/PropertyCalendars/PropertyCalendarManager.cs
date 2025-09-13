using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class PropertyCalendarManagerBase : DomainService
    {
        protected IPropertyCalendarRepository _propertyCalendarRepository;

        public PropertyCalendarManagerBase(IPropertyCalendarRepository propertyCalendarRepository)
        {
            _propertyCalendarRepository = propertyCalendarRepository;
        }

        public virtual async Task<PropertyCalendar> CreateAsync(
        Guid sitePropertyId, DateOnly date, bool isAvailable, float? price = null, string? note = null)
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));

            var propertyCalendar = new PropertyCalendar(
             GuidGenerator.Create(),
             sitePropertyId, date, isAvailable, price, note
             );

            return await _propertyCalendarRepository.InsertAsync(propertyCalendar);
        }

        public virtual async Task<PropertyCalendar> UpdateAsync(
            Guid id,
            Guid sitePropertyId, DateOnly date, bool isAvailable, float? price = null, string? note = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));

            var propertyCalendar = await _propertyCalendarRepository.GetAsync(id);

            propertyCalendar.SitePropertyId = sitePropertyId;
            propertyCalendar.Date = date;
            propertyCalendar.IsAvailable = isAvailable;
            propertyCalendar.Price = price;
            propertyCalendar.Note = note;

            propertyCalendar.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _propertyCalendarRepository.UpdateAsync(propertyCalendar);
        }

    }
}