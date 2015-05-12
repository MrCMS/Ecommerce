using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.MessageTemplates;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class SendCorrespondenceRecordMessage : IOnAdded<CorrespondenceRecord>
    {
        private readonly IMessageParser<InitialCustomerServiceMessageTemplate, CorrespondenceRecord> _messageParser;

        public SendCorrespondenceRecordMessage(IMessageParser<InitialCustomerServiceMessageTemplate, CorrespondenceRecord> messageParser)
        {
            _messageParser = messageParser;
        }

        public void Execute(OnAddedArgs<CorrespondenceRecord> args)
        {
            CorrespondenceRecord record = args.Item;

            if (record.CorrespondenceDirection == CorrespondenceDirection.Outgoing)
            {
                var message = _messageParser.GetMessage(record);
                if(message != null)
                    _messageParser.QueueMessage(message);
            }
        }
    }
}