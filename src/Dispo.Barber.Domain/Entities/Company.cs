﻿namespace Dispo.Barber.Domain.Entities
{
    public class Company : EntityBase
    {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string Slug { get; set; }
        public long? OwnerId { get; set; }

        public User Owner { get; set; }
        public IList<ServiceCompany> ServicesCompany { get; set; }
        public IList<BusinessUnity> BusinessUnities { get; set; }
    }
}
