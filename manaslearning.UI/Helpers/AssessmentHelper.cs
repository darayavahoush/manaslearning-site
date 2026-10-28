using System.Collections.Generic;

namespace manaslearning.UI.Helpers
{
    public static class AssessmentHelper
    {
        /// <summary>
        /// Get Assessment Symptoms
        /// </summary>
        /// <param name="assessmentCode"></param>
        /// <returns>List</returns>
        public static List<string> GetAssessmentSymptoms(string assessmentCode)
        {
            List<string> symptoms;

            switch (assessmentCode)
            {
                case "ADHD":
                    symptoms = new List<string>
                    {
                        "Can't sit in their seat.",
                        "Overactive and do things too fast.",
                        "Very short attention span.",
                        "Having behavioural problems."
                    };
                    break;
                case "AUTISM":
                    symptoms = new List<string>
                    {
                        "Not able to make friends and socialize with people.",
                        "Does not talk or communicate properly.",
                        "Does not look at you while talking.",
                        "Has behavioural problems."
                    };
                    break;
                case "LD":
                    symptoms = null;
                    break;
                case "DYSLEXIA":
                    symptoms = new List<string>
                    {
                        "Can't read words.",
                        "Guesses the common words.",
                        "Difficulty in reading/ Reading aloud/ making sounds while reading.",
                        "Reads without expressions/ ignores punctuations.",
                        "Reads very slowly/ difficulty to understand or comprehend what s/he reads."
                    };
                    break;
                case "DYSCALCULIA":
                    symptoms = new List<string>
                    {
                        "Gets less marks in maths/ makes silly mistakes.",
                        "Poor in addition/ subtraction/ multiplication/ division or any of these.",
                        "Can’t do mental math/ needs to count on fingers or use other methods."
                    };
                    break;
                case "DYSGRAPHIA":
                    symptoms = new List<string>
                    {
                        "Problem in thinking and writing at the same time.",
                        "Giving excuses/ not doing homework.",
                        "Bad handwriting/ poor legibility/ improper spacing.",
                        "Very slow in writing exams/ prefers to write short answers only.",
                        "Difficulty in copying from board/ incomplete notes."
                    };
                    break;
                case "DYSPRAXIA":
                    symptoms = new List<string>
                    {
                        "Problems in fine motor skills (odd pencil hold or difficulty in tying shoelace or buttoning).",
                        "Problems in gross motor skills (coordination, catching throwing or hopping).",
                        "Confusion between left/ right, up/down, front/back.",
                        "Problem in judging distance direction and speed."
                    };
                    break;
                case "ATTENTION":
                    symptoms = new List<string>
                    {
                        "Can't do any activity for more than 10 minutes.",
                        "Is unorganised in daily activities.",
                        "Forget things, daydreaming, zoning out, falling asleep.",
                        "Not responding until called 3 or 4 times."
                    };
                    break;
                case "MEMORY":
                    symptoms = new List<string>
                    {
                        "Forgetting what is studied.",
                        "Trouble following different direction and order of doing different things.",
                        "Forgetting to make note of homework or loosing their personal things(books, pencil, eraser, water bottle, etc)."
                    };
                    break;
                case "EMOTION":
                    symptoms = new List<string>
                    {
                        "Behaviour problems or tantrums.",
                        "Not participating in social activities."
                    };
                    break;
                case "DYSORTHOGRAPHIA":
                    symptoms = new List<string>
                    {
                        "Lot of spelling mistakes and corrections in note.",
                        "Difficulty reading new or unknown words.",
                        "Stammering while reading and writing slowly because of spelling confusion."
                    };
                    break;
                default:
                    symptoms = null;
                    break;
            }

            return symptoms;
        }

        /// <summary>
        /// Get Assessment Image Name
        /// </summary>
        /// <param name="assessmentCode"></param>
        /// <returns>string</returns>
        public static string GetAssessmentImageName(string assessmentCode)
        {
            var imageName = "";

            switch (assessmentCode)
            {
                case "ADHD":
                    imageName = "adhd-thumbnail.jpg";
                    break;
                case "AUTISM":
                    imageName = "autism-thumbnail.jpg";
                    break;
                case "LD":
                    imageName = "ld-thumbnail.jpg";
                    break;
                case "DYSLEXIA":
                    imageName = "dyslexia-thumbnail.jpg";
                    break;
                case "DYSCALCULIA":
                    imageName = "dyscalculia-thumbnail.jpg";
                    break;
                case "DYSGRAPHIA":
                    imageName = "dysgraphia-thumbnail.jpg";
                    break;
                case "DYSPRAXIA":
                    imageName = "dyspraxia-thumbnail.jpg";
                    break;
                case "ATTENTION":
                    imageName = "attention-thumbnail.jpg";
                    break;
                case "MEMORY":
                    imageName = "memory-thumbnail.jpg";
                    break;
                case "EMOTION":
                    imageName = "emotional-thumbnail.jpg";
                    break;
                case "DYSORTHOGRAPHIA":
                    imageName = "dysorthographia-thumbnail.jpg";
                    break;
                default:
                    imageName = "";
                    break;
            }

            return imageName;
        }
    }
}
