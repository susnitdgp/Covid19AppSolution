using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Covid19App.Web.Models;
using Covid19App.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects,
// visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Covid19App.Web.Controllers
{

    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository feedbackRepository;

        public FeedbackController(IFeedbackRepository _feedbackRepository)
        {
            feedbackRepository = _feedbackRepository;

            
        }

        /*
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FeedbackModel>> ListAll()
        {
            List<FeedbackModel> models = new List<FeedbackModel>();

            var tables = feedbackRepository.GetAll();

            foreach (FeedbackTableModel table in tables)
            {
                FeedbackModel model = new FeedbackModel()
                {
                    EmpId = table.EmpId,
                    Email = table.Email,
                    TilesName = table.TilesName,
                    Comments = table.Comments,
                    FileName = table.BlobUrl
                };

                models.Add(model);
            }

            return models;
        }
        */

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<FeedbackModel>>ListByTiles([FromQuery(Name = "tiles")] string tiles,
            [FromQuery(Name = "subtiles")] string subtiles)
        {
            IEnumerable<FeedbackTableModel> tables=null;

            List<FeedbackModel> models = new List<FeedbackModel>();

            if(tiles !=null && !String.IsNullOrEmpty(tiles))
            {
              tables = feedbackRepository.GetByTiles(tiles,subtiles);
            }
            else
            {
                tables = feedbackRepository.GetAll();
            }

                //var tables = feedbackRepository.GetByTiles(tiles);

                foreach (FeedbackTableModel table in tables)
                {
                    FeedbackModel model = new FeedbackModel()
                    {
                        EmpId = table.EmpId,
                        Email = table.Email,
                        TilesName = table.TilesName,
                        SubTilesName=table.SubTilesName,
                        Comments = table.Comments,
                        FileName = table.FileName,
                        UploadTime = table.Timestamp.ToString("dd/MM/yy H:mm:ss"),
                        FileUrl=table.BlobUrl,
                        Link=table.Link
                   };

                    models.Add(model);
                }
                return models;
   
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SuccessModel> Create([FromForm] FeedbackModel feedback)
        {
            SuccessModel success = null;
            //DateTime.Now.
            try
            {

                if(feedback.BinaryContent!=null && feedback.BinaryContent.Length > 0)
                {
                    FeedbackTableModel model = new FeedbackTableModel(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd"), Guid.NewGuid().ToString())
                    {
                        Email = feedback.Email,
                        Comments = feedback.Comments,
                        EmpId = feedback.EmpId,
                        Link = feedback.Link,
                        TilesName = feedback.TilesName,
                        SubTilesName = feedback.SubTilesName,
                        FileName = feedback.BinaryContent.FileName
                    };

                    using (var ms = new MemoryStream())
                    {
                        feedback.BinaryContent.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data

                        feedbackRepository.Add(model, Convert.FromBase64String(s));
                    }

                }
                else{

                    FeedbackTableModel model = new FeedbackTableModel(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd"), Guid.NewGuid().ToString())
                    {
                        Email = feedback.Email,
                        Comments = feedback.Comments,
                        EmpId = feedback.EmpId,
                        Link = feedback.Link,
                        TilesName = feedback.TilesName,
                        SubTilesName = feedback.SubTilesName,
                        FileName = "N/A",
                        BlobUrl="N/A"
                    };



                     feedbackRepository.Add(model);
                    

                }


                success= new SuccessModel { ResponseCode = "200", ResponseMessage = "Data Uploaded OK" };
            }
            catch(Exception ex)
            {
                success = new SuccessModel { ResponseCode = "400", ResponseMessage = ex.Message };
            }

            
            return success;

           
        }
        

    }
}
