using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;


namespace Apps.Strapi.Models.Requests.Documents
{
    public class ParametersRequest
    {
        [Display("Filters")]
        public string? Filters { get; set; }

        [Display("Locale"),DataSource(typeof(LanguageDataHandler))]
        public string? Locale { get; set; } 
        
        [Display("Status"),DataSource(typeof(StatusDataHandler))]
        public string? Status { get; set; }

        [Display("Populate Fields")]

        public string? PopulateFields { get; set; }

        [Display("Select Fields")]

        public string? SelectFields { get; set; }


        [Display("Connect Relations")]

        public string? ConnectRelations { get; set; } //TODO: this does not belong here

        [Display("Disconnect Relations")]

        public string? DisconnectRelations { get; set; } //TODO: this does not belong here


        [Display("Set Relations")]

        public string? SetRelations { get; set; } //TODO: this does not belong here

        [Display("Sort")]

        public string? Sort { get; set; } //TODO: Don't forget ascending, descending

        //TODO: these do not belong here, more in a separate request

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
