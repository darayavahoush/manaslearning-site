using System;
using System.Collections.Generic;
using System.Linq;
using manaslearning.Models;

namespace manaslearning.Business
{
    /// <summary>
    /// Pure availability logic (no database). Pass only active staff who offer the service.
    /// A slot is free for a staff member when it is inside their weekly hours, respects the
    /// service's notice window and buffer, and does not overlap leave or another live booking.
    /// </summary>
    public static class SlotCalculator
    {
        public static List<ConsultSlot> GetSlots(
            ConsultService service,
            IEnumerable<ConsultStaff> staff,
            IEnumerable<ConsultWeeklyHour> weeklyHours,
            IEnumerable<ConsultTimeOff> timeOff,
            IEnumerable<ConsultBooking> bookings,
            DateTime fromDateIst,
            DateTime toDateIst,
            DateTime nowUtc)
        {
            var result = new Dictionary<DateTime, ConsultSlot>();
            var earliest = nowUtc.AddHours(service.MinNoticeHours);
            var latest = nowUtc.AddDays(service.MaxAdvanceDays);
            var duration = TimeSpan.FromMinutes(service.DurationMinutes);
            var buffer = TimeSpan.FromMinutes(service.BufferMinutes);
            var step = service.SlotStepMinutes > 0 ? service.SlotStepMinutes : service.DurationMinutes;

            var hours = weeklyHours.ToList();
            var leave = timeOff.ToList();
            var live = bookings.Where(b =>
                b.Status == ConsultBookingStatus.Confirmed ||
                (b.Status == ConsultBookingStatus.PendingPayment &&
                 (!b.HoldExpiresUtc.HasValue || b.HoldExpiresUtc.Value > nowUtc))).ToList();

            foreach (var member in staff)
            {
                var memberHours = hours.Where(h => h.StaffId == member.Id).ToList();
                var memberLeave = leave.Where(t => t.StaffId == member.Id).ToList();
                var memberBusy = live.Where(b => b.StaffId == member.Id).ToList();

                for (var day = fromDateIst.Date; day <= toDateIst.Date; day = day.AddDays(1))
                {
                    foreach (var window in memberHours.Where(h => h.DayOfWeek == (int)day.DayOfWeek))
                    {
                        for (var m = window.StartMinute; m + service.DurationMinutes <= window.EndMinute; m += step)
                        {
                            var startUtc = ConsultTime.IstToUtc(day.AddMinutes(m));
                            if (startUtc < earliest || startUtc > latest) continue;

                            var endUtc = startUtc + duration;
                            if (memberLeave.Any(t => startUtc < t.EndUtc && t.StartUtc < endUtc)) continue;
                            if (memberBusy.Any(b => startUtc < b.EndUtc + buffer && b.StartUtc < endUtc + buffer)) continue;

                            ConsultSlot slot;
                            if (!result.TryGetValue(startUtc, out slot))
                            {
                                slot = new ConsultSlot { StartUtc = startUtc, EndUtc = endUtc };
                                result[startUtc] = slot;
                            }
                            if (!slot.StaffIds.Contains(member.Id)) slot.StaffIds.Add(member.Id);
                        }
                    }
                }
            }

            return result.Values.OrderBy(s => s.StartUtc).ToList();
        }
    }
}
