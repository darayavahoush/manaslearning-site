using manaslearning.Models;

namespace manaslearning.UI.Helpers
{
    public static class SelfTestHelper
    {
        /// <summary>
        /// Get self test title.
        /// </summary>
        /// <param name="selfTestType">Enums.SelfTestType</param>
        /// <returns>string</returns>
        public static string GetTitle(Enums.SelfTestType selfTestType)
        {
            switch (selfTestType)
            {
                case Enums.SelfTestType.Learning:
                    return "Learning Test";
                case Enums.SelfTestType.HyperactiveAndAttention1:
                    return "Hyperactivity and Attention Test (Age below 6)";
                case Enums.SelfTestType.HyperactiveAndAttention2:
                    return "Hyperactivity and Attention Test (Age above 6)";
                case Enums.SelfTestType.MotorSkills:
                    return "Motor Skills Test";
                case Enums.SelfTestType.SensitivityAndCommunication:
                    return "Sensitivity and Communication Test";
                default: return "Test";
            }
        }
    }
}
