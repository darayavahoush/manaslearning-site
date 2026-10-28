using manaslearning.Models;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace manaslearning.UI.Controllers
{
    [Route("sitemap.xml")]
    [ApiController]
    public class SiteMapController : ControllerBase
    {

        // GET: SiteMap.xml
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            List<SitemapNode> nodes = null;
            try
            {
                nodes = new List<SitemapNode>
                {
                    // Home: ""
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_HOME), Frequency = Enums.SitemapFrequency.Weekly, Priority = 1 },

                    // Services: "/services"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_SERVICES), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.7 },

                    // Child Academic Problems: "/child-academic-problems"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Attention and Hyperactivity: "/child-academic-problems/attention-and-hyperactivity"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_ATTENTION_AND_HYPERACTIVITY), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Reading: "/child-academic-problems/understanding-your-childs-reading-problems"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_READING), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Writing: "/child-academic-problems/understanding-your-childs-writing-problems"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_WRITING), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Maths: "/child-academic-problems/understanding-your-childs-trouble-with-math"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_MATHS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Spellings: "/child-academic-problems/understanding-your-childs-difficulties-with-spelling"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_SPELLING), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Motor Skills: "/child-academic-problems/understanding-your-childs-difficulty-with-movement-and-coordination"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_MOTOR_SKILLS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Executive Functions: "/child-academic-problems/understanding-your-childs-difficulty-with-executive-functions"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_EXECUTIVE_FUNCTIONS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Nonverbal Skills: "/child-academic-problems/understanding-your-childs-nonverbal-learning-problem"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_NONVERBAL_SKILLS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Listening Skills: "/child-academic-problems/understanding-your-childs-trouble-with-listening"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_LISTENING_SKILLS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },
                    // Visual Skills: "/child-academic-problems/understanding-your-childs-visual-perceptual-problems"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CHILD_ACADEMIC_PROBLEMS_VISUAL_SKILLS), Frequency = Enums.SitemapFrequency.Daily, Priority = 0.9 },

                    // Assessments: "/assessments"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_ASSESSMENTS), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.7 },
                    // Free Assessments: "/free-assessments"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS), Frequency = Enums.SitemapFrequency.Daily, Priority = 1 },
                    // Free Assessments > Learning Test: "/self-test/learning-test/1"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_LEARNING_TEST), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Hyperactive and Attention Test (Age below 6): "/self-test/learning-test/2"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_HYPERACTIVE_AND_ATTENTION_TEST_AGE_BELOW_6), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Hyperactive and Attention Test (Age above 6): "/self-test/learning-test/3"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_HYPERACTIVE_AND_ATTENTION_TEST_AGE_ABOVE_6), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Motor Skills Test: "/self-test/learning-test/4"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_MOTOR_SKILLS_TEST), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Sensitivity and Communication Test: "/self-test/learning-test/5"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_SENSITIVITY_AND_COMMUNICATION_TEST), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Anxiety: "/self-test/learning-test/6"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_ANXIETY), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Avoiding School : "/self-test/learning-test/7"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_AVOIDING_SCHOOL), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },
                    // Free Assessments > Sadness : "/self-test/learning-test/8"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_FREE_ASSESSMENTS_SADNESS), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.8 },

                    // About Us: "/about-us"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_ABOUT_US), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.5 },

                    // Contact Us: "/contact-us"
                    new SitemapNode{ Url = GetAbsolutePath(UrlConstants.PAGE_CONTACT_US), Frequency = Enums.SitemapFrequency.Weekly, Priority = 0.5 },
                    // Not required: Terms Of Service, Privacy Policy
                };

                return Content(this.GetSitemapDocument(nodes), "application/xml", Encoding.UTF8);
            }
            catch
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get sitemap document
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns>string</returns>
        private string GetSitemapDocument(IEnumerable<SitemapNode> nodes)
        {
            List<SitemapNode> sitemapNodes = null;
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");
            try
            {
                sitemapNodes = new List<SitemapNode>();
                sitemapNodes = nodes.ToList();

                foreach (SitemapNode sitemapNode in nodes)
                {
                    XElement urlElement = null;
                    try
                    {
                        urlElement = new XElement(
                            xmlns + "url",
                            new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                            sitemapNode.LastModified == null ? null : new XElement(
                                xmlns + "lastmod",
                                sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                            sitemapNode.Frequency == null ? null : new XElement(
                                xmlns + "changefreq",
                                sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                            sitemapNode.Priority == null ? null : new XElement(
                                xmlns + "priority",
                                sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                        root.Add(urlElement);
                    }
                    finally
                    {
                        if (urlElement != null)
                            urlElement = null;
                    }
                }

                XDocument document = new XDocument(root);
                return document.ToString();
            }
            catch
            {
                // TODO: Log exception
                return null;
            }
        }

        /// <summary>
        /// Get Absolute Path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>string</returns>
        private string GetAbsolutePath(string path)
        {
            var basePath = Url.PageLink().Replace(Request.Path, "");
            return $"{basePath}{path}";
        }
    }
}
