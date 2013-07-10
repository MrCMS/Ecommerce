namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class MaxProductNameLengthIs255 : MaxStringLength
    {
        public MaxProductNameLengthIs255()
            : base("Product Name", o => o.Name, 255)
        {
        }
    }
}