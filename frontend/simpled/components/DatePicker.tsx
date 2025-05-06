'use client';

import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { CalendarIcon } from 'lucide-react';
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { cn } from '@/lib/utils';

interface DatePickerProps {
  readonly date: Date | undefined;
  readonly onDateChange: (date: Date | undefined) => void;
  readonly placeholder?: string;
  readonly disabled?: boolean;
}

export function DatePicker({
  date,
  onDateChange,
  placeholder = 'Seleccionar fecha',
  disabled = false,
}: DatePickerProps) {
  const [open, setOpen] = useState(false);

  return (
    <Popover open={open} onOpenChange={setOpen} modal={true}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            'w-full justify-start text-left font-normal',
            !date && 'text-muted-foreground',
          )}
          disabled={disabled}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {date ? format(date, 'PPP', { locale: es }) : <span>{placeholder}</span>}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="z-[1100] w-auto p-0" align="start">
        <Calendar mode="single" selected={date} onSelect={onDateChange} locale={es} />
      </PopoverContent>
    </Popover>
  );
}
