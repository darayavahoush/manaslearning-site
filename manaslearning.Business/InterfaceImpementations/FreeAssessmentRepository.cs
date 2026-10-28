using manaslearning.Business.Interfaces;
using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class FreeAssessmentRepository : IFreeAssessmentRepository
    {
        /// <summary>
        /// Get Free Assessments
        /// </summary>
        /// <returns>FreeAssessment List</returns>
        public async Task<List<FreeAssessment>> GetFreeAssessments() // TODO: run async
        {
            try
            {
                var freeAssessments = new List<FreeAssessment>();

                // Learning
                var freeAssessment = new FreeAssessment
                {
                    AssessmentId = 1,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_LEARNING,
                    AssessmentDescription = "Is your child struggling with reading, writing, speaking, math, one or all? Do the test to understand your child's condition.",
                    LowerLimit = 6,
                    UpperLimit = 8,
                    DefaultMessage = "It looks like your child does not have any problem in learning. If you still have any questions please call us @ 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Hyperactive and Attention Test (Age below 6).
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 2,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_HYPERACTIVE_AND_ATTENTION_AGE_BELOW_6,
                    AssessmentDescription = "Does your child get distracted often or is your child's Hyperactive? Has your child’s behavior become a big concern for you? Do the test to understand why?",
                    LowerLimit = 4,
                    UpperLimit = 6,
                    DefaultMessage = "It looks like your child does not have any problem in attention or activity level. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Hyperactive and Attention Test (Age above 6).
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 3,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_HYPERACTIVE_AND_ATTENTION_AGE_ABOVE_6,
                    AssessmentDescription = "Does your child have trouble focusing on one thing at a time, is your child Hyperactive. Do the test to understand why?",
                    LowerLimit = 6,
                    UpperLimit = 8,
                    DefaultMessage = "It looks like your child does not have any problem in attention or activity level. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Motor Skills
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 4,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_MOTOR_SKILLS,
                    AssessmentDescription = "Is your child weak in sports, Is your child having troubles in co-ordination or tying laces? Do the test to check if your child is at risk.",
                    LowerLimit = 6,
                    UpperLimit = 8,
                    DefaultMessage = " It looks like your child does not have any problem in motor skills. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Sensitivity and Communication
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 5,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_SENSITIVITY_AND_COMMUNICATION,
                    AssessmentDescription = "Does your child have trouble making friends? Is your child having problem expressing. Answer the questions to assess your child.",
                    LowerLimit = 6,
                    UpperLimit = 8,
                    DefaultMessage = "It looks like your child does not have any problem in communication skills. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Anxiety
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 6,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_ANXIETY,
                    AssessmentDescription = "Does your child look anxious or frequently agitated? Is your child restless and confused? Do the test to check if your child is at risk?",
                    LowerLimit = 4,
                    UpperLimit = 6,
                    DefaultMessage = "It looks like your child does not have any anxiety issues. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. Please pay attention to your child’s emotional state and comfort your child. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Avoiding School 
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 7,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_AVOIDING_SCHOOL,
                    AssessmentDescription = "Does your child frequently try to avoid school on purpose? Is your child having trouble socializing with people outside your house? Do the test to check if your child has problem.",
                    LowerLimit = 4,
                    UpperLimit = 6,
                    DefaultMessage = "It looks like your child does not have any school issues. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. Please pay attention to what in the school might be causing your child stress or fear. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                // Sadness
                freeAssessment = new FreeAssessment
                {
                    AssessmentId = 8,
                    AssessmentTitle = StringConstants.SELF_TEST_TYPE_SADNESS,
                    AssessmentDescription = "Does your child lack Self-Confidence? Is your child not able to enjoy regular activities all of a sudden? Do the test to know your child better.",
                    LowerLimit = 4,
                    UpperLimit = 6,
                    DefaultMessage = "It looks like your child does not have any sadness issues. If you still have any questions please call 988403129 for a free consultation.",
                    LowerLimitMessage = "Your child might be vulnerable. Please pay attention to your child's emotional state. If you have any questions call us @ 9884031293.",
                    UpperLimitMessage = "Your child is at risk, consult a professional for appropriate diagnosis. Call us @ 9884031293 for a free consultation."
                };
                freeAssessments.Add(freeAssessment);

                return await Task.FromResult(freeAssessments);
            }
            catch (Exception)
            {
                throw; // TODO: Log exception
            }
        }

        /// <summary>
        /// Get Free Assessment by AssessmentId
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns>FreeAssessment</returns>
        public async Task<FreeAssessment> GetFreeAssessmentById(int assessmentId)
        {
            try
            {
                var freeAssessments = await GetFreeAssessments();
                return freeAssessments.FirstOrDefault(q => q.AssessmentId == assessmentId);
            }
            catch (Exception)
            {
                throw; // TODO: Log exception
            }
        }

        /// <summary>
        /// Get Free Assessment Questions by AssessmentId
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns>FreeAssessmentQuestion List</returns>
        public async Task<List<FreeAssessmentQuestion>> GetFreeAssessmentQuestions(int assessmentId) // TODO: run async
        {
            try
            {
                var assessmentType = (Enums.SelfTestType)assessmentId;
                var questions = new List<FreeAssessmentQuestion>();

                if (assessmentType == Enums.SelfTestType.Learning)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Frequently distracted in the classroom." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Showing poor interest in writing/avoids writing." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Difficulty in remembering in a sequential order." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Discrepancy between the oral and written performance." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Written reversals: b for d, p for q." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Written inversions: u for n, w for m." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "More strikes/over writings in a page." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "No space between words/letters collide with each other." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Mixes lower case letters with capitals." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Making more spelling mistakes in a dictation." });
                    questions.Add(new FreeAssessmentQuestion { Id = 11, Question = "Incomplete written homework." });
                    questions.Add(new FreeAssessmentQuestion { Id = 12, Question = "Rereads line/skips lines or words." });
                    questions.Add(new FreeAssessmentQuestion { Id = 13, Question = "Reads word by word." });
                    questions.Add(new FreeAssessmentQuestion { Id = 14, Question = "Reads very slowly." });
                    questions.Add(new FreeAssessmentQuestion { Id = 15, Question = "Solves problems left to right instead of right to left." });
                    questions.Add(new FreeAssessmentQuestion { Id = 16, Question = "Counts on his finger even after eight years." });
                    questions.Add(new FreeAssessmentQuestion { Id = 17, Question = "Poor attention span-no sustained focus." });
                    questions.Add(new FreeAssessmentQuestion { Id = 18, Question = "Remembers things that have happened long ago but no recent events." });
                    questions.Add(new FreeAssessmentQuestion { Id = 19, Question = "Disorganized- loses his school things like pencil box, books etc." });
                    questions.Add(new FreeAssessmentQuestion { Id = 20, Question = "Difficulty in application of directional and positional concepts." });
                    questions.Add(new FreeAssessmentQuestion { Id = 21, Question = "Forms letters in a strange way." });
                    questions.Add(new FreeAssessmentQuestion { Id = 22, Question = "Letters not on line." });
                    questions.Add(new FreeAssessmentQuestion { Id = 23, Question = "Illegible handwriting." });
                    questions.Add(new FreeAssessmentQuestion { Id = 24, Question = "Holds pencil too tightly: often breaks pencil point/crayons." });
                    questions.Add(new FreeAssessmentQuestion { Id = 25, Question = "Yawns while reading." });
                    questions.Add(new FreeAssessmentQuestion { Id = 26, Question = "Guesses the word from the first letter while reading." });
                    questions.Add(new FreeAssessmentQuestion { Id = 27, Question = "Can do mental math (sums) but fails to write the same in proper steps or order." });
                    questions.Add(new FreeAssessmentQuestion { Id = 28, Question = "Difficulty in remembering spelling for frequently used words." });
                    questions.Add(new FreeAssessmentQuestion { Id = 29, Question = "Incomplete answer sheet especially short answers and essay types." });
                    questions.Add(new FreeAssessmentQuestion { Id = 30, Question = "Difficulty in carrying more than one instruction at a time." });
                }
                else if (assessmentType == Enums.SelfTestType.HyperactiveAndAttention1)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Child does not answer when his name is called the first time." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Child is running all the time." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Child cannot wait for his/her turn while playing games." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Child is always restless and fidgeting?" });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Gets angry very easily." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Needs constant entertainment." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Breaks/spoils most of the toys." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "More scribbling than writing for the age." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Frequent tantrums." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Difficult to control and manage at home." });
                    questions.Add(new FreeAssessmentQuestion { Id = 11, Question = "Frequent complaints from schools." });
                    questions.Add(new FreeAssessmentQuestion { Id = 12, Question = "Talks a lot." });
                }
                else if (assessmentType == Enums.SelfTestType.HyperactiveAndAttention2)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Problem paying attention." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Daydreaming frequently." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Losses things frequently." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Does not seem to listen in the class." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Makes silly mistakes." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Forgets things especially exams and homework." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Disorganised in daily activity/school work/general." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Can’t follow or misses out on multiple instructions." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Tries to avoid things that take more time or more patience or more effort(brain)." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Cannot sit in one place, even if sits keeps fidgeting." });
                    questions.Add(new FreeAssessmentQuestion { Id = 11, Question = "Needs to always keep doing something or the other." });
                    questions.Add(new FreeAssessmentQuestion { Id = 12, Question = "Talks too much." });
                    questions.Add(new FreeAssessmentQuestion { Id = 13, Question = "Cannot play quietly." });
                    questions.Add(new FreeAssessmentQuestion { Id = 14, Question = "Disturbs others in the class." });
                    questions.Add(new FreeAssessmentQuestion { Id = 15, Question = "Keeps running, jumping or climbing most of the time." });
                    questions.Add(new FreeAssessmentQuestion { Id = 16, Question = "Interrupts others." });
                    questions.Add(new FreeAssessmentQuestion { Id = 17, Question = "Gets irritated for small things." });
                    questions.Add(new FreeAssessmentQuestion { Id = 18, Question = "Constant attention seeking." });
                    questions.Add(new FreeAssessmentQuestion { Id = 19, Question = "Always needs to retaliate even fight if required." });
                    questions.Add(new FreeAssessmentQuestion { Id = 20, Question = "Answers before the question is completed." });
                    questions.Add(new FreeAssessmentQuestion { Id = 21, Question = "Commits mistakes in the process of doing things fast." });
                    questions.Add(new FreeAssessmentQuestion { Id = 22, Question = "Cannot wait for their turn." });
                    questions.Add(new FreeAssessmentQuestion { Id = 23, Question = "Wanted likes to break rule." });
                    questions.Add(new FreeAssessmentQuestion { Id = 24, Question = "Acts and does things without thinking." });
                }
                else if (assessmentType == Enums.SelfTestType.MotorSkills)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Poor hand eye co-ordination (can't throw the ball in the right direction, difficulty in playing video games)." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Has difficulty in putting buttons or tying laces." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Banging into things." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Climbs down the stairs very carefully or by holding the railing." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Poor in sports or doesn’t enjoy sports activities." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Difficulties in nonverbal communication." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Slow in copying from board." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Very slow in writing." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Gets distracted easily." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Walks in an odd way." });
                    questions.Add(new FreeAssessmentQuestion { Id = 11, Question = "Poor memory skills." });
                    questions.Add(new FreeAssessmentQuestion { Id = 12, Question = "Problem in following sequence." });
                    questions.Add(new FreeAssessmentQuestion { Id = 13, Question = "Poor in running, cycling etc." });
                    questions.Add(new FreeAssessmentQuestion { Id = 14, Question = "Confusion in left and right." });
                    questions.Add(new FreeAssessmentQuestion { Id = 15, Question = "Problems in focus." });
                    questions.Add(new FreeAssessmentQuestion { Id = 16, Question = "Not comfortable in a group of people." });
                    questions.Add(new FreeAssessmentQuestion { Id = 17, Question = "Difficulty in making friends." });
                    questions.Add(new FreeAssessmentQuestion { Id = 18, Question = "Oversensitive or under sensitive to sound/light/emotions." });
                    questions.Add(new FreeAssessmentQuestion { Id = 19, Question = "Clumsy in general." });
                }
                else if (assessmentType == Enums.SelfTestType.SensitivityAndCommunication)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Does not talk much." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Below 5 years tries to communicate more with actions." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Does not look at people or things." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Very less eye contact or avoids eye contact." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Lack of expression on face when compared to other children of the same age." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Has problem in making friends." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Has problem in socializing." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Cannot bare loud sound." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Does not like to be touched." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Keeps talking about the same thing repetitively." });
                    questions.Add(new FreeAssessmentQuestion { Id = 11, Question = "Difficulty in adjusting to change." });
                    questions.Add(new FreeAssessmentQuestion { Id = 12, Question = "Crying or tantrums when goes to a new place." });
                    questions.Add(new FreeAssessmentQuestion { Id = 13, Question = "Scared of more people or unknown people." });
                    questions.Add(new FreeAssessmentQuestion { Id = 14, Question = "Problem in showing emotions or understanding emotions." });
                    questions.Add(new FreeAssessmentQuestion { Id = 15, Question = "Extreme fear or no fear (Age appropriate)." });
                    questions.Add(new FreeAssessmentQuestion { Id = 16, Question = "Repetitive actions when upset or excited (clapping, flapping, moving fingers,banging head)." });
                    questions.Add(new FreeAssessmentQuestion { Id = 17, Question = "Fascinated by which or fan or things in circular motion, that can’t move their eyes from it." });
                    questions.Add(new FreeAssessmentQuestion { Id = 18, Question = "Fascinated by changing lights/ light passing through glass/ light spectrum." });
                    questions.Add(new FreeAssessmentQuestion { Id = 19, Question = "Loves to stack blocks or toys." });
                    questions.Add(new FreeAssessmentQuestion { Id = 20, Question = "Making weird sounds or actions frequently." });
                }
                else if (assessmentType == Enums.SelfTestType.Anxiety)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Not sleeping properly." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Not eating properly, either eating too little or suddenly eating more." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Missing school or suddenly becoming quite in the class." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Doesn’t want to meet friends or go to school or socialize." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Feeling unwell or tummy ache or some other reason on Monday mornings or just before going to school." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Getting irritable or angry very easily or not able to control their emotions." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Crying, feeling helpless and not knowing what to do." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Restless, fidgeting, walking continuously." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Frequently feeling breathless, increase in heartbeat, butterflies in their tummies." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Worrying about small thing and worrying about thing generally not related to their age." });
                }
                else if (assessmentType == Enums.SelfTestType.AvoidingSchool)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Falling sick before school time and then being normal at home." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Not able to go out of house or socialize with friends." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Continuously trying to avoid school at any cost." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Having headache, stomach pain, breathless ness, fear. (anyone)" });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Not able to concentrate, focus on classes, tests or homework." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "Getting less marks suddenly." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Not able to meet friends or talk to them." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Restlessness, fidgeting or walking." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "Not able sleep or waking up with bad dreams." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "Not eating properly." });
                }
                else if (assessmentType == Enums.SelfTestType.Sadness)
                {
                    questions.Add(new FreeAssessmentQuestion { Id = 1, Question = "Not eating properly, sudden loss of weight or gain of weight." });
                    questions.Add(new FreeAssessmentQuestion { Id = 2, Question = "Not sleeping properly, oversleeping, lying down on bed most of the day." });
                    questions.Add(new FreeAssessmentQuestion { Id = 3, Question = "Lazy whole day, not taking interest in regular fun activities." });
                    questions.Add(new FreeAssessmentQuestion { Id = 4, Question = "Feeling sad, crying, not talking much." });
                    questions.Add(new FreeAssessmentQuestion { Id = 5, Question = "Not able to enjoy, being dull, talking less than normal." });
                    questions.Add(new FreeAssessmentQuestion { Id = 6, Question = "No smile or laugh in more than 15 days." });
                    questions.Add(new FreeAssessmentQuestion { Id = 7, Question = "Physical pain, body pains headaches, stomach pains." });
                    questions.Add(new FreeAssessmentQuestion { Id = 8, Question = "Not going to play, not meeting friends, no socialization." });
                    questions.Add(new FreeAssessmentQuestion { Id = 9, Question = "No self-confidence, low self-esteem." });
                    questions.Add(new FreeAssessmentQuestion { Id = 10, Question = "No enjoyment, no happiness." });
                }
                return await Task.FromResult(questions);
            }
            catch (Exception)
            {
                throw; // TODO: Log exception
            }
        }
    }
}
