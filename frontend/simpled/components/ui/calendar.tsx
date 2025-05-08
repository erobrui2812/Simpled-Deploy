'use client';

import React, { useRef, useEffect, forwardRef } from 'react';
import Pikaday from 'pikaday';
import 'pikaday/css/pikaday.css';
import { Input } from '@/components/ui/input';
import { CalendarIcon } from 'lucide-react';

interface LightCalendarProps {
  date?: Date;
  onDateChange: (d: Date) => void;
  placeholder?: string;
  disabled?: boolean;
}

export const LightCalendar = forwardRef<HTMLInputElement, LightCalendarProps>(
  ({ date, onDateChange, placeholder = 'Seleccionar fecha', disabled = false }, ref) => {
    const inputRef = useRef<HTMLInputElement>(null);

    // Permite que Pikaday se monte sobre este <input>
    useEffect(() => {
      if (!inputRef.current) return;
      const picker = new Pikaday({
        field: inputRef.current,
        defaultDate: date,
        setDefaultDate: Boolean(date),
        onSelect: onDateChange,
        i18n: {
          previousMonth: 'Anterior',
          nextMonth: 'Siguiente',
          months: [
            'Enero','Febrero','Marzo','Abril','Mayo','Junio',
            'Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'
          ],
          weekdaysShort: ['Do','Lu','Ma','Mi','Ju','Vi','Sa'],
        },
      });
      return () => picker.destroy();
    }, [date, onDateChange]);

    return (
      <div className="relative">
        <Input
          ref={(node) => {
            inputRef.current = node;
            if (typeof ref === 'function') ref(node);
            else if (ref) (ref as React.MutableRefObject<HTMLInputElement>).current = node;
          }}
          placeholder={placeholder}
          disabled={disabled}
          className="pr-10"  {/* espacio para el icono */}
        />
        <CalendarIcon className="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 text-neutral-400" />
      </div>
    );
  }
);
LightCalendar.displayName = 'LightCalendar';
