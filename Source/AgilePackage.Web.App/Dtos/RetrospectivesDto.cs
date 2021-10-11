using System;

namespace AgilePackage.Web.App.Dtos
{
    public class RetrospectivesDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public static RetrospectivesDto ConvertDomainToDto(Models.Retrospective domain)
        {
            return new RetrospectivesDto
            {
                Id = domain.Id,
                Title = domain.Title,
            };
        }
    }
}
