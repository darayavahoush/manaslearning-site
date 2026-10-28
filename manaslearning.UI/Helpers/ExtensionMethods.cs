using System;

namespace manaslearning.UI.Helpers
{
    public static class ExtensionMethods
    {
        public static DateTime ToTimeZone(this DateTime dateTime, string timeZone)
        {
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, cstZone);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"{timeZone} : Timezone not found");
            }
            catch (InvalidTimeZoneException)
            {
                throw new Exception($"{timeZone} : Invalid Timezone");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static DateTime ToUTCTime(this DateTime dateTime, string timeZone)
        {
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, cstZone);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"{timeZone} : Timezone not found");
            }
            catch (InvalidTimeZoneException)
            {
                throw new Exception($"{timeZone} : Invalid Timezone");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
