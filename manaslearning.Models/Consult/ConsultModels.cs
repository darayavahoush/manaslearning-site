using System;
using System.Collections.Generic;

namespace manaslearning.Models
{
    public enum ConsultBookingStatus : byte
    {
        PendingPayment = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3,
        NoShow = 4
    }

    /// <summary>All consultation times are stored in UTC and shown in IST (UTC+5:30, no daylight saving).</summary>
    public static class ConsultTime
    {
        public static readonly TimeSpan IstOffset = new TimeSpan(5, 30, 0);

        public static DateTime UtcToIst(DateTime utc)
        {
            return DateTime.SpecifyKind(utc.Add(IstOffset), DateTimeKind.Unspecified);
        }

        public static DateTime IstToUtc(DateTime ist)
        {
            return DateTime.SpecifyKind(ist.Subtract(IstOffset), DateTimeKind.Utc);
        }
    }

    public class ConsultService
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int SlotStepMinutes { get; set; }
        public int BufferMinutes { get; set; }
        public int MinNoticeHours { get; set; }
        public int MaxAdvanceDays { get; set; }
        public decimal PriceInr { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public bool IsPaid { get { return PriceInr > 0; } }
    }

    public class ConsultStaff
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }

    public class ConsultWeeklyHour
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int DayOfWeek { get; set; }   // 0 = Sunday
        public int StartMinute { get; set; } // minutes after midnight IST
        public int EndMinute { get; set; }
    }

    public class ConsultTimeOff
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Reason { get; set; }
    }

    public class ConsultBooking
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public ConsultBookingStatus Status { get; set; }
        public string ParentName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ChildName { get; set; }
        public string Notes { get; set; }
        public string PaymentOrderId { get; set; }
        public DateTime? HoldExpiresUtc { get; set; }
        public Guid ManageToken { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

    public class ConsultSlot
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public List<int> StaffIds { get; set; } = new List<int>();
    }
}
