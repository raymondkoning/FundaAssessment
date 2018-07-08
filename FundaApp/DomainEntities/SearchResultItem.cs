namespace FundaApp.DomainEntities
{
    /// <summary>
    /// Data entity containing aggregated group results.
    /// </summary>
    public class SearchResultItem : GroupingItem
    {
        public int Count { get; set; }
    }
}
