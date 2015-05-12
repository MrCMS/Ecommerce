using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public class UpdateFeedbackRecord : IUpdateFeedbackRecord
    {
        private readonly ISession _session;

        public UpdateFeedbackRecord(ISession session)
        {
            _session = session;
        }

        public void Update(List<FeedbackFacetRecordModel> records)
        {
            List<Feedback> toUpdate = new List<Feedback>();

            foreach (var feedbackFacetRecordModel in records)
            {
                var record = _session.Get<Feedback>(feedbackFacetRecordModel.Id);
                if (record != null)
                {
                    record.Rating = feedbackFacetRecordModel.Rating;
                    record.Message = feedbackFacetRecordModel.Message;
                    toUpdate.Add(record);
                    record.FeedbackRecord.IsCompleted = true;
                }
            }

            _session.Transact(session =>
            {
                foreach (var feedbackFacetRecord in toUpdate)
                {
                    session.Update(feedbackFacetRecord);
                }
            });

        }
    }
}