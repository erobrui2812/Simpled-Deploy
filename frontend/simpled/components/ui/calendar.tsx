'use client';

import type { Locale } from 'date-fns';
import { es } from 'date-fns/locale';
import type * as React from 'react';
import { DayPicker } from 'react-day-picker';

import { buttonVariants } from '@/components/ui/button';
import { cn } from '@/lib/utils';

export type CalendarProps = React.ComponentProps<typeof DayPicker>;

function Calendar({
  className,
  classNames,
  showOutsideDays = true,
  locale = es,
  ...props
}: CalendarProps & { locale?: Locale }) {
  return (
    <DayPicker
      showOutsideDays={showOutsideDays}
      className={cn('p-3', className)}
      locale={locale}
      {...props}
    />
  );
}
Calendar.displayName = 'Calendar';

export { Calendar };
