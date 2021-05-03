using System;
using System.Collections.Generic;

namespace Covid19App.Web.Models
{
    public interface IFeedbackRepository
    {
        void Add(FeedbackTableModel feedback,byte[] binaryContent);
        void Add(FeedbackTableModel feedback);
        IEnumerable<FeedbackTableModel> GetByTiles(string tiles, string subtiles);
        IEnumerable<FeedbackTableModel> GetAll();
    }
}
