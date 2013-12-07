namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public abstract class ValidationResult
    {
        private readonly string _message;

        protected ValidationResult(string message)
        {
            _message = message;
        }
        public bool Valid { get { return string.IsNullOrWhiteSpace(Message); } }

        public string Message
        {
            get { return _message; }
        }
    }
}