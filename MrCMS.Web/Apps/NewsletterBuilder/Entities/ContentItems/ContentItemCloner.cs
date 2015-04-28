namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems
{
    public abstract class ContentItemCloner<T> : CloneContentItemBase where T : ContentItem, new()
    {
        public abstract T Clone(T entity);

        public override ContentItem Clone(ContentItem contentItem)
        {
            return Clone(contentItem as T);
        }
    }
}