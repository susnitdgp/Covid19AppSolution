using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Covid19App.Web.Models
{
    public class FeedbackModel
    {

        [Required]
        public string TilesName { get; set; }

        [Required]
        public string SubTilesName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Comments { get; set; }

        [Required]
        public string EmpId { get; set; }

        public string UploadTime { get; set; }

        public string Link { get; set; }

        public string FileName { get; set; }

        public string FileUrl { get; set; }

        public IFormFile BinaryContent { get; set; }

       

    }
}
