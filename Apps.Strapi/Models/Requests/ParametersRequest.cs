using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;


namespace Apps.Strapi.Models.Requests
{
    public class ParametersRequest
    {
        [Display("Filters")]
        public string? Filters { get; set; }

        [Display("Locale")]
        public string? Locale { get; set; } //TODO: Add json data source? https://github.com/strapi/strapi/blob/main/packages/plugins/i18n/server/src/constants/iso-locales.json

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



        #region Pagination //TODO: these do not belong here, more in a separate request


        [Display("Page")]

        public int Page { get; set; }

        [Display("Page Size")]

        public int PageSize { get; set; }

        [Display("With Count")]

        public bool WithCount { get; set; }

        #endregion
    }
}
