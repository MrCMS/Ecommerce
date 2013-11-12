namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    /// <summary>
    /// Response types that could be received from SagePay
    /// </summary>
    public enum ResponseType
    {
        Unknown,
        Ok,
        NotAuthed,
        Abort,
        Rejected,
        Authenticated,
        Registered,
        Malformed,
        Error,
        Invalid,
    }
}