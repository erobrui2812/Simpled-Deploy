'use client';

import { addDays, format } from 'date-fns';
import { es } from 'date-fns/locale';
import { useMemo, useState } from 'react';

export function useGanttTimeline() {
  const [viewMode, setViewMode] = useState<'day' | 'week' | 'month'>('week');
  const [startDate, setStartDate] = useState<Date>(() => {
    const today = new Date();
    today.setDate(today.getDate() - 7);
    return today;
  });

  const daysToShow = useMemo(() => {
    switch (viewMode) {
      case 'day':
        return 14;
      case 'week':
        return 28;
      case 'month':
        return 60;
      default:
        return 28;
    }
  }, [viewMode]);

  const timelineDates = useMemo(() => {
    return Array.from({ length: daysToShow }, (_, i) => addDays(startDate, i));
  }, [startDate, daysToShow]);

  const timelineHeaders = useMemo(() => {
    if (viewMode === 'day') {
      return timelineDates.map((date) => ({
        label: format(date, 'EEE d', { locale: es }),
        span: 1,
        startDate: date,
      }));
    } else if (viewMode === 'week') {
      const weeks: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const weekKey = format(date, 'w-yyyy');
        if (!weeks[weekKey]) {
          weeks[weekKey] = [];
        }
        weeks[weekKey].push(date);
      });
      return Object.entries(weeks).map(([key, dates]) => ({
        label: `Semana ${key.split('-')[0]}`,
        span: dates.length,
        startDate: dates[0],
      }));
    } else {
      const months: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const monthKey = format(date, 'MMM-yyyy', { locale: es });
        if (!months[monthKey]) {
          months[monthKey] = [];
        }
        months[monthKey].push(date);
      });
      return Object.entries(months).map(([key, dates]) => ({
        label: key.split('-')[0],
        span: dates.length,
        startDate: dates[0],
      }));
    }
  }, [timelineDates, viewMode]);

  const navigateTimeline = (direction: 'prev' | 'next') => {
    const days = direction === 'prev' ? -daysToShow / 2 : daysToShow / 2;
    setStartDate(addDays(startDate, days));
  };

  return {
    viewMode,
    setViewMode,
    startDate,
    setStartDate,
    daysToShow,
    timelineDates,
    timelineHeaders,
    navigateTimeline,
  };
}
