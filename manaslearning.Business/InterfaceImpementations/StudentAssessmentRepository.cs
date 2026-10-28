using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class StudentAssessmentRepository : IStudentAssessmentRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public StudentAssessmentRepository(IApplicationConnection applicationConnection, ILogger<StudentAssessmentRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<StudentAssessment> GetStudentAssessmentByIdAsync(long studentAssessmentId)
        {
            string procedureName = "[dbo].[GetStudentAssessmentById]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { StudentAssessmentId = studentAssessmentId };

                    return await connection.QueryFirstOrDefaultAsync<StudentAssessment>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.GetStudentAssessmentByIdAsync(), StudentAssessmentId={studentAssessmentId}");
                return null;
            }
        }

        public async Task<int> CreateStudentAssessmentAsync(Guid id, int studentId, int languageId, long userId)
        {
            string procedureName = "[dbo].[CreateStudentAssessment]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Id", id, dbType: DbType.Guid);
                    dynamicParameters.Add("@StudentId", studentId, dbType: DbType.Int32);
                    dynamicParameters.Add("@LanguageId", languageId, dbType: DbType.Int32);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);

                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.CreateStudentAssessmentAsync(), Id={id}, StudentId = {studentId}, LanguageId={languageId}, UserId={userId}");
                return -1;
            }
        }

        public async Task<Tuple<int, StudentAssessmentQuestion, IEnumerable<StudentAssessmentQuestionOption>>> GetStudentAssessmentNextQuestionAsync(int assessmentId, int studentId, int languageId, long userId)
        {
            string procedureName = "[dbo].[GetStudentAssessmentNextQuestion]";
            StudentAssessmentQuestion assessmentQuestion = null;
            IEnumerable<StudentAssessmentQuestionOption> assessmentQuestionOption = null;
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@AssessmentId", assessmentId, dbType: DbType.Int32);
                    dynamicParameters.Add("@StudentId", studentId, dbType: DbType.Int32);
                    dynamicParameters.Add("@LanguageId", languageId, dbType: DbType.Int32);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    dynamicParameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var result = await connection.QueryMultipleAsync(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);

                    if (!result.IsConsumed)
                        assessmentQuestion = await result?.ReadFirstOrDefaultAsync<StudentAssessmentQuestion>();

                    if (!result.IsConsumed)
                        assessmentQuestionOption = await result?.ReadAsync<StudentAssessmentQuestionOption>();

                    var returnValue = dynamicParameters.Get<int>("@ReturnValue");

                    return new Tuple<int, StudentAssessmentQuestion, IEnumerable<StudentAssessmentQuestionOption>>(
                        returnValue,
                        assessmentQuestion,
                        assessmentQuestionOption?.OrderBy(x => x.Sno));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.GetStudentAssessmentNextQuestionAsync(), AssessmentId={assessmentId}, StudentId = {studentId}, LanguageId={languageId}, UserId={userId}");
                return new Tuple<int, StudentAssessmentQuestion, IEnumerable<StudentAssessmentQuestionOption>>(-1, null, null);
            }
        }

        public async Task<int> UpdateStudentAssessmentQuestionAsync(long studentAssessmentId, int questionId, Guid questionGuidId, Guid questionOptionGuidId, long userId)
        {
            string procedureName = "[dbo].[UpdateStudentAssessmentQuestion]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@StudentAssessmentId", studentAssessmentId, dbType: DbType.Int64);
                    dynamicParameters.Add("@QuestionId", questionId, dbType: DbType.Int32);
                    dynamicParameters.Add("@QuestionGuidId", questionGuidId, dbType: DbType.Guid);
                    dynamicParameters.Add("@QuestionOptionGuidId", questionOptionGuidId, dbType: DbType.Guid);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    dynamicParameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    // TODO: Replace with QueryAsync
                    await connection.QueryMultipleAsync(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                    return dynamicParameters.Get<int>("@ReturnValue");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.UpdateStudentAssessmentQuestionAsync(), StudentAssessmentId={studentAssessmentId}, QuestionId={questionId}, QuestionGuidId={questionGuidId}, QuestionOptionGuidId={questionOptionGuidId}");
                return -1;
            }
        }

        /// <summary>
        /// Get Assessment Result
        /// </summary>
        /// <param name="id">StudentAssessment.GuidId</param>
        /// <param name="userId"></param>
        /// <returns>StudentAssessmentResult</returns>
        public async Task<StudentAssessmentResult> GetAssessmentResultAsync(Guid id, int languageId, long userId)
        {
            string procedureName = "[dbo].[GetAssessmentResult]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Id", id, dbType: DbType.Guid);
                    dynamicParameters.Add("@LanguageId", languageId, dbType: DbType.Int32);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);

                    return await connection.QueryFirstOrDefaultAsync<StudentAssessmentResult>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.GetAssessmentResultAsync(), Id={id}, UserId={userId}");
                return null;
            }
        }

        /// <summary>
        /// Generate and Save Assessment Result
        /// </summary>
        /// <param name="id">StudentAssessment.GuidId</param>
        /// <param name="userId"></param>
        /// <returns>int</returns>
        public async Task<int> GenerateAndSaveAssessmentResultAsync(Guid id, long userId)
        {
            string procedureName = "[dbo].[GetStudentAssessmentData]"; // Note: This is procedure is used in Reporting function as well.
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Id", id, dbType: DbType.Guid);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);

                    var result = await connection.QueryMultipleAsync(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);

                    var studentAssessment = await result.ReadFirstOrDefaultAsync<StudentAssessment>();
                    var studentAssessmentQuestion = await result.ReadAsync<StudentAssessmentQuestion>();
                    var studentAssessmentQuestionOption = await result.ReadAsync<StudentAssessmentQuestionOption>();

                    if (studentAssessment == null
                        || studentAssessmentQuestion == null
                        || studentAssessmentQuestionOption == null)
                        return -2; // NotFound

                    var studentAssessmentResult = CalculateAssessmentResult(studentAssessment, studentAssessmentQuestion, studentAssessmentQuestionOption);

                    var isSaved = await SaveAssessmentResultAsync(studentAssessmentResult, studentAssessment.AssessmentId, userId);

                    if (isSaved) return 1;
                    else return -3; // Error while saving assessment result
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.GenerateAssessmentResultAsync(), Id={id}, UserId={userId}");
                return -1;
            }
        }

        #region Assessment Result Calculation

        /// <summary>
        /// Calculate Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateAssessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            if (assessment == null || questions == null || options == null)
                return null;

            switch (assessment.Code)
            {
                case "ADHD":
                    return CalculateADHDAssessmentResult(assessment, questions, options);
                case "AUTISM":
                    return CalculateAutismAsessmentResult(assessment, questions, options);
                case "DYSLEXIA":
                    return CalculateDyslexiaAsessmentResult(assessment, questions, options);
                case "DYSCALCULIA":
                    return CalculateDyscalculiaAsessmentResult(assessment, questions, options);
                case "DYSGRAPHIA":
                    return CalculateDysgraphiaAsessmentResult(assessment, questions, options);
                case "DYSPRAXIA":
                    return CalculateDyspraxiaAsessmentResult(assessment, questions, options);
                case "ATTENTION":
                    return CalculateAttentionAsessmentResult(assessment, questions, options);
                case "MEMORY":
                    return CalculateMemoryAsessmentResult(assessment, questions, options);
                case "EMOTION":
                    return CalculateEmotionalWellnessAsessmentResult(assessment, questions, options);
                case "DYSORTHOGRAPHIA":
                    return CalculateDysorthographiaAsessmentResult(assessment, questions, options);
                case "LD":
                    return CalculateLDAsessmentResult(assessment, questions, options);
            }

            return null;
        }

        /// <summary>
        /// Calculate ADHD Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateADHDAssessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var hyperactivityScore = data.Where(x => x.Category == "Hyperactivity" && x.IsSelected == true).Sum(x => x.Score);
            var attensionScore = data.Where(x => x.Category == "Attension" && x.IsSelected == true).Sum(x => x.Score);
            var impulsiveScore = data.Where(x => x.Category == "Impulsive" && x.IsSelected == true).Sum(x => x.Score);
            var opositionalBehaviourScore = data.Where(x => x.Category == "Oppositional Behaviour" && x.IsSelected == true).Sum(x => x.Score);

            // 1:
            // hyperactivity less than or equal to 11
            // Attention equal or less than 11
            // Imulsivity less than or equal to 21
            // OPP B equal to or less than 11
            if (hyperactivityScore <= 11
                && attensionScore <= 11
                && impulsiveScore <= 21
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_001";
            }

            // 2:
            // hyperactivity less than or equal to 12
            // Attention equal or less than 12
            // Imulsivity less than or equal to 21
            // Opp B equal to or more 12
            else if (hyperactivityScore <= 12
                && attensionScore <= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_002";
            }

            // 3:
            // hyperactivity less than or equal to 13
            // Attention equal or less than 13
            // impulsivity equal to or more than 22
            // OPP B equal to or less than 11
            else if (hyperactivityScore <= 13
                    && attensionScore <= 13
                    && impulsiveScore >= 22
                    && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_003";
            }

            // 4:
            // hyperactivity less than or equal to 14
            // Attention equal or less than 14
            // impulsivity equal to or more than 22
            // Opp B equal to or more 12
            else if (hyperactivityScore <= 14
                && attensionScore <= 14
                && impulsiveScore >= 22
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_004";
            }

            // 5:
            // hyperactivity less than or equal to 15
            // Attention equal to 12 or more
            // Imulsivity less than or equal to 21
            // OPP B equal to or less than 11
            else if (hyperactivityScore <= 15
                && attensionScore >= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_005";
            }

            // 6:
            // hyperactivity less than or equal to 16
            // Attention equal to 12 or more
            // Imulsivity less than or equal to 21
            // Opp B equal to or more 12
            else if (hyperactivityScore <= 16
                && attensionScore >= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_006";
            }

            // 7:
            // hyperactivity less than or equal to 17
            // Attention equal to 12 or more
            // impulsivity equal to or more than 22
            // OPP B equal to or less than 11
            else if (hyperactivityScore <= 17
                && attensionScore >= 12
                && impulsiveScore >= 22
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_007";
            }

            // 8:
            // hyperactivity less than or equal to 18
            // Attention equal to 12 or more
            // impulsivity equal to or more than 22
            // Opp B equal to or more 12
            else if (hyperactivityScore <= 18
                && attensionScore >= 12
                && impulsiveScore >= 22
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_008";
            }

            // 9:
            // hyperactivity equal to 12 or more
            // Attention equal or less than 11
            // Imulsivity less than or equal to 21
            // OPP B equal to or less than 11
            else if (hyperactivityScore >= 12
                && attensionScore <= 11
                && impulsiveScore <= 21
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_009";
            }

            // 10:
            // hyperactivity equal to 12 or more
            // Attention equal or less than 12
            // Imulsivity less than or equal to 21
            // Opp B equal to or more 12
            else if (hyperactivityScore >= 12
                && attensionScore <= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_010";
            }

            // 11:
            // hyperactivity equal to 12 or more
            // Attention equal or less than 13
            // impulsivity  equal to or more than 22
            // OPP B equal to or less than 11
            else if (hyperactivityScore >= 12
                && attensionScore <= 13
                && impulsiveScore >= 22
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_011";
            }

            // 12:
            // hyperactivity equal to 12 or more
            // Attention equal or less than 14
            // impulsivity equal to or more than 22
            // Opp B equal to or more 12
            else if (hyperactivityScore >= 12
                && attensionScore <= 14
                && impulsiveScore >= 22
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_012";
            }

            // 13:
            // hyperactivity equal to 12 or more
            // Attention equal to 12 or more
            // Imulsivity less than or equal to 21
            // OPP B equal to or less than 11
            else if (hyperactivityScore >= 12
                && attensionScore >= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_013";
            }

            // 14:
            // hyperactivity equal to 12 or more
            // Attention equal to 12 or more
            // Imulsivity less than or equal to 21
            // Opp B equal to or more 12
            else if (hyperactivityScore >= 12
                && attensionScore >= 12
                && impulsiveScore <= 21
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_014";
            }

            // 15:
            // hyperactivity equal to 12 or more
            // Attention equal to 12 or more
            // impulsivity equal to or more than 22
            // OPP B equal to or less than 11
            else if (hyperactivityScore >= 12
                && attensionScore >= 12
                && impulsiveScore >= 22
                && opositionalBehaviourScore <= 11)
            {
                resultCode = "ADHD_015";
            }

            // 16:
            // hyperactivity equal to 12 or more
            // Attention equal to 12 or more
            // impulsivity equal to or more than 22
            // Opp B equal to or more 12
            else if (hyperactivityScore >= 12
                && attensionScore >= 12
                && impulsiveScore >= 22
                && opositionalBehaviourScore >= 12)
            {
                resultCode = "ADHD_016";
            }
            else
            {
                hasProblem = false;
                resultCode = "ADHD_000";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Autism Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateAutismAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 160)
            {
                resultCode = "AUTISM_001";
            }
            else if (score >= 161 && score <= 200)
            {
                resultCode = "AUTISM_002";
            }
            else if (score >= 201)
            {
                resultCode = "AUTISM_003";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Dyslexia Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateDyslexiaAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 50)
            {
                resultCode = "LD_DYSLEXIA_001";
            }
            else if (score >= 51)
            {
                resultCode = "LD_DYSLEXIA_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Dyscalculia Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateDyscalculiaAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 50)
            {
                resultCode = "LD_DYSCALCULIA_001";
            }
            else if (score >= 51)
            {
                resultCode = "LD_DYSCALCULIA_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Dysgraphia Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateDysgraphiaAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 65)
            {
                resultCode = "LD_DYSGRAPHIA_001";
            }
            else if (score >= 66)
            {
                resultCode = "LD_DYSGRAPHIA_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Dyspraxia Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateDyspraxiaAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 32)
            {
                resultCode = "LD_DYSPRAXIA_001";
            }
            else if (score >= 33)
            {
                resultCode = "LD_DYSPRAXIA_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Attention Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateAttentionAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 17)
            {
                resultCode = "LD_ATTENTION_001";
            }
            else if (score >= 18)
            {
                resultCode = "LD_ATTENTION_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Memory Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateMemoryAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 29)
            {
                resultCode = "LD_MEMORY_001";
            }
            else if (score >= 30)
            {
                resultCode = "LD_MEMORY_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Emotional Wellness Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateEmotionalWellnessAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 20)
            {
                resultCode = "LD_EMOTIONAL_001";
            }
            else if (score >= 21)
            {
                resultCode = "LD_EMOTIONAL_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate Dysautographia Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateDysorthographiaAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var score = data.Where(x => x.IsSelected == true).Sum(x => x.Score);

            if (score <= 23)
            {
                resultCode = "LD_DYSORTHOGRAPHIA_001";
            }
            else if (score >= 24)
            {
                resultCode = "LD_DYSORTHOGRAPHIA_002";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        /// <summary>
        /// Calculate LD Assessment Result
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="questions"></param>
        /// <param name="options"></param>
        /// <returns>StudentAssessmentResult</returns>
        private StudentAssessmentResult CalculateLDAsessmentResult(
            StudentAssessment assessment,
            IEnumerable<StudentAssessmentQuestion> questions,
            IEnumerable<StudentAssessmentQuestionOption> options)
        {
            bool hasProblem = true;
            string resultCode = "";
            string message = "";

            var data = from q in questions
                       join o in options on new { q.QuestionId } equals new { o.QuestionId }
                       select new { q.Category, o.Score, o.IsSelected };

            var attention = data.Where(x => x.Category == "Attention" && x.IsSelected == true).Sum(x => x.Score);
            var memory = data.Where(x => x.Category == "Memory" && x.IsSelected == true).Sum(x => x.Score);
            var emotional = data.Where(x => x.Category == "Emotional" && x.IsSelected == true).Sum(x => x.Score);
            var dyspraxia = data.Where(x => x.Category == "Dyspraxia" && x.IsSelected == true).Sum(x => x.Score);
            var spellings = data.Where(x => x.Category == "Spellings" && x.IsSelected == true).Sum(x => x.Score);
            var reading = data.Where(x => x.Category == "Reading" && x.IsSelected == true).Sum(x => x.Score);
            var writing = data.Where(x => x.Category == "Writing" && x.IsSelected == true).Sum(x => x.Score);
            var maths = data.Where(x => x.Category == "Maths" && x.IsSelected == true).Sum(x => x.Score);

            if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_001"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_002"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_003"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_004"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_005"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_006"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_007"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_008"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_009"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_010"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_011"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_012"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_013"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_014"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_015"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_016"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_017"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_018"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_019"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_020"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_021"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_022"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_023"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_024"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_025"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_026"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_027"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_028"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_029"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_030"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_031"; }
            else if (attention >= 18 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_032"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_033"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_034"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_035"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_036"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_037"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_038"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_039"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_040"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_041"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_042"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_043"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_044"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_045"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_046"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_047"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_048"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_049"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_050"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_051"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_052"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_053"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_054"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_055"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_056"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_057"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_058"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_059"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_060"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_061"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_062"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_063"; }
            else if (attention >= 18 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_064"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_065"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_066"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_067"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_068"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_069"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_070"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_071"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_072"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_073"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_074"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_075"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_076"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_077"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_078"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_079"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_080"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_081"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_082"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_083"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_084"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_085"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_086"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_087"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_088"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_089"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_090"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_091"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_092"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_093"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_094"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_095"; }
            else if (attention >= 18 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_096"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_097"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_098"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_099"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_100"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_101"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_102"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_103"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_104"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_105"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_106"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_107"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_108"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_109"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_110"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_111"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_112"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_113"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_114"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_115"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_116"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_117"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_118"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_119"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_120"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_121"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_122"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_123"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_124"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_125"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_126"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_127"; }
            else if (attention >= 18 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_128"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_129"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_130"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_131"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_132"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_133"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_134"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_135"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_136"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_137"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_138"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_139"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_140"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_141"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_142"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_143"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_144"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_145"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_146"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_147"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_148"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_149"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_150"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_151"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_152"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_153"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_154"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_155"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_156"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_157"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_158"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_159"; }
            else if (attention <= 17 && memory >= 18 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_160"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_161"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_162"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_163"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_164"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_165"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_166"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_167"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_168"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_169"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_170"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_171"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_172"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_173"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_174"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_175"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_176"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_177"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_178"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_179"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_180"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_181"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_182"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_183"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_184"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_185"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_186"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_187"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_188"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_189"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_190"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_191"; }
            else if (attention <= 17 && memory >= 18 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_192"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_193"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_194"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_195"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_196"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_197"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_198"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_199"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_200"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_201"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_202"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_203"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_204"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_205"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_206"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_207"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_208"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_209"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_210"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_211"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_212"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_213"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_214"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_215"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_216"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_217"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_218"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_219"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_220"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_221"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_222"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_223"; }
            else if (attention <= 17 && memory <= 17 && emotional >= 18 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_224"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_225"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_226"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_227"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_228"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_229"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_230"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_231"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_232"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_233"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_234"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_235"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_236"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_237"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_238"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_239"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia >= 18 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_240"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_241"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_242"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_243"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_244"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_245"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_246"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_247"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings >= 24 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_248"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths >= 51) { resultCode = "LD_249"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing >= 66 && maths <= 50) { resultCode = "LD_250"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths >= 51) { resultCode = "LD_251"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading >= 51 && writing <= 65 && maths <= 50) { resultCode = "LD_252"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths >= 51) { resultCode = "LD_253"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing >= 66 && maths <= 50) { resultCode = "LD_254"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths >= 51) { resultCode = "LD_255"; }
            else if (attention <= 17 && memory <= 17 && emotional <= 17 && dyspraxia <= 17 && spellings <= 23 && reading <= 50 && writing <= 65 && maths <= 50) { resultCode = "LD_256"; }
            else
            {
                hasProblem = false;
                resultCode = "LD_000";
            }

            return new StudentAssessmentResult
            {
                StudentAssessmentId = assessment.StudentAssessmentId,
                HasProblem = hasProblem,
                ResultCode = resultCode,
                Message = message
            };
        }

        #endregion Assessment Result Calculation

        /// <summary>
        /// Save Assessment Result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        private async Task<bool> SaveAssessmentResultAsync(StudentAssessmentResult result, int assessmentId, long userId)
        {
            string procedureName = "[dbo].[SaveStudentAssessmentResult]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@StudentAssessmentId", result.StudentAssessmentId, dbType: DbType.Int64);
                    dynamicParameters.Add("@AssessmentId", assessmentId, dbType: DbType.Int32);
                    dynamicParameters.Add("@HasProblem", result.HasProblem, dbType: DbType.Boolean);
                    dynamicParameters.Add("@ResultCode", result.ResultCode, dbType: DbType.String);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    dynamicParameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var response = await connection.QueryFirstOrDefaultAsync<StudentAssessmentResult>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                    var returnValue = dynamicParameters.Get<int>("@ReturnValue");

                    return returnValue == 200;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.SaveAssessmentResultAsync(), StudentAssessmentId={result.StudentAssessmentId}, HasProblem={result.HasProblem}, UserId={userId}");
                return false;
            }
        }

        public async Task<StudentAssessment> GetExistingStudentAssessmentAsync(Guid id, int studentId, long userId)
        {
            var procedureName = "[dbo].[GetExistingStudentAssessment]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Id", id, dbType: DbType.Guid);
                    dynamicParameters.Add("@StudentId", studentId, dbType: DbType.Int32);
                    dynamicParameters.Add("@UserId", userId, dbType: DbType.Int64);
                    return await connection.QueryFirstOrDefaultAsync<StudentAssessment>(procedureName, param: dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.GetExistingStudentAssessmentAsync(), Id={id}, StudentId = {studentId}, UserId={userId}");
                return null;
            }
        }

        public async Task<int> DeleteStudentAssessmentByIdAsync(long studentAssessmentId)
        {
            string procedureName = "[dbo].[DeleteStudentAssessmentById]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new { StudentAssessmentId = studentAssessmentId };

                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=StudentAssessmentRepository.DeleteStudentAssessmentById(), StudentAssessmentId={studentAssessmentId}");
                return -1;
            }
        }
    }
}
