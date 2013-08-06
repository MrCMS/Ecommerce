using System.IO;
using System.Web;

namespace MrCMS.EcommerceApp.Tests.Stubs
{
    public class BasicHttpPostedFileBaseCSV : HttpPostedFileBase
    {
        public override int ContentLength
        {
            get { return 1; }
        }
        public override string ContentType
        {
            get { return "text/CSV"; }
        }
        public override Stream InputStream
        {
            get { return new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}); }
        }
    }
}