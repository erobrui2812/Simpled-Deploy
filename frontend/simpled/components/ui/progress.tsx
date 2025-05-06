import * as React from 'react';

import { cn } from '@/lib/utils';

const Progress = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement> & {
    value?: number;
    max?: number;
    getValueLabel?: (value: number, max: number) => string;
  }
>(({ className, value = 0, max = 100, getValueLabel, ...props }, ref) => {
  const percentage = value && max ? (value / max) * 100 : 0;

  return (
    <div
      ref={ref}
      role="progressbar"
      aria-valuemin={0}
      aria-valuemax={max}
      aria-valuenow={value}
      aria-valuetext={getValueLabel ? getValueLabel(value, max) : `${percentage}%`}
      className={cn('bg-primary/10 relative h-2 w-full overflow-hidden rounded-full', className)}
      {...props}
    >
      <div
        className="bg-primary h-full w-full flex-1 transition-all"
        style={{ transform: `translateX(-${100 - percentage}%)` }}
      />
    </div>
  );
});
Progress.displayName = 'Progress';

export { Progress };
