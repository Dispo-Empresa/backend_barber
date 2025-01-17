ALTER TABLE public."UserSchedules" ADD COLUMN IF NOT EXISTS "StartDay" DATE NULL;
ALTER TABLE public."UserSchedules" ADD COLUMN IF NOT EXISTS "EndDay" DATE NULL;

ALTER TABLE public."UserSchedules" ALTER "StartDate" DROP NOT NULL;
ALTER TABLE public."UserSchedules" ALTER "EndDate" DROP NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'UK_Schedule'
    ) THEN
        ALTER TABLE public."UserSchedules" 
        ADD CONSTRAINT "UK_Schedule" 
        UNIQUE ("DayOfWeek", "StartDate", "EndDate", "IsRest", "DayOff", "UserId", "StartDay", "EndDay");
    END IF;
END $$;