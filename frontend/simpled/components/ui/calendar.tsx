'use client';

import { Input } from '@/components/ui/input';
import { CalendarIcon } from 'lucide-react';
import Pikaday from 'pikaday';
import 'pikaday/css/pikaday.css';
import React, { forwardRef, useEffect, useRef } from 'react';

interface LightCalendarProps {
  date?: Date;
  onDateChange: (d: Date) => void;
  placeholder?: string;
  disabled?: boolean;
}

export const LightCalendar = forwardRef<HTMLInputElement, LightCalendarProps>(
  ({ date, onDateChange, placeholder = 'Seleccionar fecha', disabled = false }, ref) => {
    const inputRef = useRef<HTMLInputElement>(null);

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
            'Enero',
            'Febrero',
            'Marzo',
            'Abril',
            'Mayo',
            'Junio',
            'Julio',
            'Agosto',
            'Septiembre',
            'Octubre',
            'Noviembre',
            'Diciembre',
          ],
          weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
          weekdaysShort: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'],
        },
      });
      return () => picker.destroy();
    }, [date, onDateChange]);

    return (
      <div className="relative">
        <Input
          ref={(node) => {
            inputRef.current = node;
            if (typeof ref === 'function') {
              ref(node);
            } else if (ref && typeof ref === 'object' && 'current' in ref) {
              (ref as React.RefObject<HTMLInputElement | null>).current = node;
            }
          }}
          placeholder={placeholder}
          disabled={disabled}
          className="pr-10"
        />
        <CalendarIcon className="pointer-events-none absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2 text-neutral-400" />
      </div>
    );
  },
);
LightCalendar.displayName = 'LightCalendar';
