ALTER TABLE public."Users" ALTER COLUMN "Surname" DROP NOT NULL;

-- Drop foreign key constraint between Appointments and Services
ALTER TABLE "Appointments" DROP CONSTRAINT IF EXISTS "FK_Appointments_Services_ServiceId";

-- Drop the index on ServiceId in the Appointments table
DROP INDEX IF EXISTS "IX_Appointments_ServiceId";

-- Drop the ServiceId column from the Appointments table
ALTER TABLE "Appointments" DROP COLUMN IF EXISTS "ServiceId";

-- Create the new AppointmentServices table
CREATE TABLE IF NOT EXISTS "AppointmentServices" (
    "AppointmentId" bigint NOT NULL,
    "ServiceId" bigint NOT NULL,
    "Id" bigint NOT NULL,
    PRIMARY KEY ("AppointmentId", "ServiceId"),
    CONSTRAINT "FK_AppointmentServices_Appointments_AppointmentId" FOREIGN KEY ("AppointmentId") REFERENCES "Appointments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AppointmentServices_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE CASCADE
);

-- Create the index on ServiceId in the AppointmentServices table
CREATE INDEX IF NOT EXISTS "IX_AppointmentServices_ServiceId" ON "AppointmentServices" ("ServiceId");
