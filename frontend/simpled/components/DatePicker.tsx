'use client';

import { Input } from '@/components/ui/input';
import { CalendarIcon } from 'lucide-react';
import Pikaday from 'pikaday';
import 'pikaday/css/pikaday.css';
import { forwardRef, Ref, useEffect, useRef } from 'react';

interface DatePickerProps {
  readonly date: Date | undefined;
  readonly onDateChange: (date: Date | undefined) => void;
  readonly placeholder?: string;
  readonly disabled?: boolean;
}

export const DatePicker = forwardRef(
  (
    { date, onDateChange, placeholder = 'Seleccionar fecha', disabled = false }: DatePickerProps,
    ref: Ref<HTMLInputElement>,
  ) => {
    const inputRef = useRef<HTMLInputElement>(null);
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
      if (!inputRef.current || !containerRef.current) return;
      const picker = new Pikaday({
        field: inputRef.current,
        container: containerRef.current,
        defaultDate: date,
        setDefaultDate: Boolean(date),
        onSelect: (d: Date) => onDateChange(d),
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

    const setRefs = (node: HTMLInputElement) => {
      inputRef.current = node;
      if (typeof ref === 'function') ref(node);
      else if (ref) (ref as React.MutableRefObject<HTMLInputElement>).current = node;
    };

    return (
      <div ref={containerRef} className="relative">
        <Input
          ref={setRefs}
          type="text"
          placeholder={placeholder}
          disabled={disabled}
          className="w-full pr-10"
        />
        <CalendarIcon className="pointer-events-none absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2 text-neutral-400" />
      </div>
    );
  },
);
DatePicker.displayName = 'DatePicker';
