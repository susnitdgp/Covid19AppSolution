using System;
using System.Collections.Generic;
using Covid19App.Web.Models;

namespace Covid19App.Web.Services
{
    public interface IFeedbackRepository
    {
        void Add(FeedbackTableModel feedback,byte[] binaryContent);
        void Add(FeedbackTableModel feedback);
        IEnumerable<FeedbackTableModel> GetByTiles(string tiles, string subtiles);
        IEnumerable<FeedbackTableModel> GetAll();
    }
}
