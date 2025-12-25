using System.Globalization;
using Microsoft.VisualBasic;
using MudBlazor;

namespace Heron.MudCalendar;

public class CalendarDateRange : DateRange
{
    public CalendarView View { get; }

    private readonly DateTime _currentDay;
    private static CultureInfo _culture = CultureInfo.InvariantCulture;
    private readonly Calendar _calendar;

    public CalendarDateRange(DateTime currentDay, CalendarView view, CultureInfo culture, DayOfWeek? firstDayOfWeek = null):base(SetStart(firstDayOfWeek, view, currentDay, culture.Calendar), SetEnd(firstDayOfWeek, view, currentDay, culture.Calendar))
    {
        _culture = culture;
        _calendar = culture.Calendar;
        _currentDay = currentDay;
        View = view;
    }

    private static DateTime SetStart(DayOfWeek? firstDayOfWeek, CalendarView view, DateTime currentDay, Calendar calendar)
    {
        switch (view)
        {
            case CalendarView.Day:
                return currentDay.Date;
            case CalendarView.Week:
            case CalendarView.WorkWeek:
                return GetFirstWeekDate(currentDay, firstDayOfWeek);
            case CalendarView.Month:
            default:
                return GetFirstMonthDate(currentDay, firstDayOfWeek, calendar);
        }
    }

    private static DateTime SetEnd(DayOfWeek? firstDayOfWeek, CalendarView view, DateTime currentDay, Calendar calendar)
    {
        switch (view)
        {
            case CalendarView.Day:
                return currentDay.Date;
            case CalendarView.Week:
                return GetLastWeekDate(currentDay, firstDayOfWeek);
            case CalendarView.WorkWeek:
                return GetLastWorkWeekDate(currentDay, firstDayOfWeek);
            case CalendarView.Month:
            default:
                return GetLastMonthDate(currentDay, firstDayOfWeek, calendar);
        }
    }
    
    public static DateTime GetFirstMonthDate(DateTime day, DayOfWeek? firstDayOfWeek, Calendar calendar)
    {
        // Get the year and month in the target calendar system
        var year = calendar.GetYear(day);
        var month = calendar.GetMonth(day);

        // Get the first day of the month in the target calendar
        var firstDayOfMonth = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);

        // Adjust to the start of the week
        firstDayOfMonth = firstDayOfMonth.AddDays(GetDayOfWeek(firstDayOfMonth, firstDayOfWeek) * -1);

        return firstDayOfMonth;
    }
    
    public static DateTime GetLastMonthDate(DateTime day, DayOfWeek? firstDayOfWeek, Calendar calendar)
    {
        
        // Get the year and month in the target calendar system
        var year = calendar.GetYear(day);
        var month = calendar.GetMonth(day);

        // Get the number of days in this month
        var daysInMonth = calendar.GetDaysInMonth(year, month);

        // Get the last day of the month in the target calendar
        var lastDayOfMonth = calendar.ToDateTime(year, month, daysInMonth, 0, 0, 0, 0);

        // Adjust to the end of the week
        lastDayOfMonth = lastDayOfMonth.AddDays(6 - GetDayOfWeek(lastDayOfMonth, firstDayOfWeek));

        return lastDayOfMonth;
    }

    public static DateTime GetFirstWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get the first day of the week
        return day.AddDays(GetDayOfWeek(day, firstDayOfWeek) * -1);
    }

    public static DateTime GetLastWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get the last day of the week
        return day.AddDays(6 - GetDayOfWeek(day, firstDayOfWeek));
    }

    public static DateTime GetLastWorkWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get the last day of the work week
        return day.AddDays(4 - GetDayOfWeek(day, firstDayOfWeek));
    }

    public static int GetDayOfWeek(DateTime date, DayOfWeek? firstDayOfWeek = null)
    {
        // Get day as an integer. First day of the week = 0, last day = 6
        var firstDay = firstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek;
        var day = (int)date.DayOfWeek;
        day -= (int)firstDay;
        if (day < 0)
        {
            day = 7 + day;
        }

        return day;
    }
}