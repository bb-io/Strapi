using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;


namespace Apps.Strapi.Models.Requests.Documents
{
    public class ParametersRequest
    {
        [Display("Filters")]
        public string? Filters { get; set; }

        [Display("Locale"), StaticDataSource(typeof(LanguageDataHandler))]
        public string? Locale { get; set; } 
        
        [Display("Status"),DataSource(typeof(StatusDataHandler))]
        public string? Status { get; set; }

        [Display("Populate Fields")]

        public string? PopulateFields { get; set; }

        [Display("Select Fields")]

        public string? SelectFields { get; set; }

        #region Relations //TODO: this does not belong here

        [Display("Connect Relations")]

        public string? ConnectRelations { get; set; }

        [Display("Disconnect Relations")]

        public string? DisconnectRelations { get; set; }


        [Display("Set Relations")]

        public string? SetRelations { get; set; }
        #endregion


        [Display("Sort")]

        public string? Sort { get; set; } //TODO: Don't forget ascending, descending

        #region Pagination 


        [Display("Page")]

        public int Page { get; set; }

        [Display("Page Size")]

        public int PageSize { get; set; }

        [Display("With Count")]

        public bool WithCount { get; set; }

        #endregion
    }
}
