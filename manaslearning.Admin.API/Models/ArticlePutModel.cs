using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace manaslearning.Admin.API.Models
{
    public class ArticlePutModel
    {
        [Required(ErrorMessage = "ArticleId is required")]
        public int ArticleId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title - Max. 200 characters allowed.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Excerpt is required")]
        [StringLength(2000, ErrorMessage = "Excerpt - Max. 2000 characters allowed.")]
        public string Excerpt { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author - Max. 100 characters allowed.")]
        public string Author { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
        public int[] RelatedLinks { get; set; }

        [Required(ErrorMessage = "FeaturedImage is required")]
        [StringLength(100, ErrorMessage = "FeaturedImage - Max. 1000 characters allowed.")]
        public string FeaturedImage { get; set; }

        [Required(ErrorMessage = "MetaTitle is required")]
        [StringLength(200, ErrorMessage = "MetaTitle - Max. 200 characters allowed.")]
        public string MetaTitle { get; set; }

        [Required(ErrorMessage = "MetaUrl is required")]
        [StringLength(200, ErrorMessage = "MetaUrl - Max. 200 characters allowed.")]
        public string MetaUrl { get; set; }

        [Required(ErrorMessage = "MetaKeyWords is required")]
        [StringLength(1000, ErrorMessage = "MetaKeyWords - Max. 1000 characters allowed.")]
        public string MetaKeyWords { get; set; }

        [Required(ErrorMessage = "MetaKeyDescription is required")]
        [StringLength(1000, ErrorMessage = "MetaKeyDescription - Max. 1000 characters allowed.")]
        public string MetaKeyDescription { get; set; }
        public bool IsPendingReview { get; set; }

        public Tuple<bool, string> IsValid()
        {
            bool result = true;
            ICollection<ValidationResult> validationErrors = new List<ValidationResult>();
            StringBuilder errorMessages = new StringBuilder();

            Validator.TryValidateObject(this
            , new ValidationContext(this)
            , validationErrors, true);

            if (validationErrors.Count > 0)
            {
                result = false;
                foreach (var validationError in validationErrors)
                {
                    AppendLine(validationError.ErrorMessage);
                }
            }

            if (!string.IsNullOrEmpty(Title) && string.IsNullOrWhiteSpace(Title?.Trim())) // !string.IsNullOrEmpty(Title) - Added to skip duplicate data annotation error message
            {
                AppendLine("Title is required");
            }

            if (!string.IsNullOrEmpty(Content) && string.IsNullOrWhiteSpace(Content?.Trim()))
            {
                AppendLine("Content is required");
            }            

            return new Tuple<bool, string>(result, errorMessages.ToString());

            void AppendLine(string errorMessage)
            {
                if (errorMessages.Length > 0)
                {
                    errorMessages.AppendLine(errorMessage);
                }
                else { errorMessages.Append(errorMessage); }
            }
        }
    }
}

