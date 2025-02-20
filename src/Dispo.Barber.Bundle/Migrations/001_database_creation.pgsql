﻿-- public."Companies" definition

CREATE TABLE IF NOT EXISTS public."Companies" (
    "Id" BIGINT GENERATED BY DEFAULT AS IDENTITY,
    "Name" TEXT NOT NULL,
    "Logo" TEXT NOT NULL,
    CONSTRAINT "PK_Companies" PRIMARY KEY ("Id")
);


-- public."Customers" definition

CREATE TABLE IF NOT EXISTS public."Customers" (
    "Id" BIGINT GENERATED BY DEFAULT AS IDENTITY (INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    "Name" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

-- public."Services" definition

CREATE TABLE IF NOT EXISTS public."Services" (
    "Id" BIGINT GENERATED BY DEFAULT AS IDENTITY (INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    "Description" TEXT NOT NULL,
    "Price" DOUBLE PRECISION NOT NULL,
    "Duration" INT DEFAULT 0 NOT NULL,
    CONSTRAINT "PK_Services" PRIMARY KEY ("Id")
);

-- public."BusinessUnities" definition

CREATE TABLE IF NOT EXISTS public."BusinessUnities" (
    "Id" BIGINT GENERATED BY DEFAULT AS IDENTITY (INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    "CNPJ" TEXT NULL,
    "Phone" TEXT NULL,
    "Country" TEXT NOT NULL,
    "City" TEXT NOT NULL,
    "District" TEXT NOT NULL,
    "CEP" TEXT NOT NULL,
    "Street" TEXT NOT NULL,
    "Number" TEXT NULL,
    "Complement" TEXT NULL,
    "CompanyId" BIGINT NOT NULL,
    CONSTRAINT "PK_BusinessUnities" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_BusinessUnities_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES public."Companies"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_BusinessUnities_CompanyId" ON public."BusinessUnities" ("CompanyId");



-- public."CompanyServices" definition

CREATE TABLE IF NOT EXISTS public."CompanyServices" (
	"CompanyId" bigint NOT NULL,
	"ServiceId" bigint NOT NULL,
	"Id" bigint NOT NULL,
	CONSTRAINT "PK_CompanyServices" PRIMARY KEY ("CompanyId", "ServiceId"),
	CONSTRAINT "FK_CompanyServices_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES public."Companies"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_CompanyServices_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES public."Services"("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_CompanyServices_ServiceId" ON public."CompanyServices" USING btree ("ServiceId");


-- public."Users" definition

CREATE TABLE IF NOT EXISTS public."Users" (
	"Id" bigint GENERATED BY DEFAULT AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"Name" text NOT NULL,
	"Surname" text NOT NULL,
	"Email" text NULL,
	"Password" text NULL,
	"Phone" text NOT NULL,
	"Role" int4 NOT NULL,
	"BusinessUnityId" bigint NULL,
	"Status" int4 DEFAULT 0 NOT NULL,
	CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_Users_BusinessUnities_BusinessUnityId" FOREIGN KEY ("BusinessUnityId") REFERENCES public."BusinessUnities"("Id")
);
CREATE INDEX IF NOT EXISTS "IX_Users_BusinessUnityId" ON public."Users" USING btree ("BusinessUnityId");


-- public."Appointments" definition

CREATE TABLE IF NOT EXISTS public."Appointments" (
	"Id" bigint GENERATED BY DEFAULT AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"Date" timestamptz NOT NULL,
	"AccomplishedDate" timestamptz NOT NULL,
	"CustomerObservation" text NULL,
	"AcceptedUserObservation" text NULL,
	"BusinessUnityId" bigint NOT NULL,
	"CustomerId" bigint NOT NULL,
	"AcceptedUserId" bigint NULL,
	CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_Appointments_BusinessUnities_BusinessUnityId" FOREIGN KEY ("BusinessUnityId") REFERENCES public."BusinessUnities"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_Appointments_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_Appointments_Users_AcceptedUserId" FOREIGN KEY ("AcceptedUserId") REFERENCES public."Users"("Id")
);
CREATE INDEX IF NOT EXISTS "IX_Appointments_AcceptedUserId" ON public."Appointments" USING btree ("AcceptedUserId");
CREATE INDEX IF NOT EXISTS "IX_Appointments_BusinessUnityId" ON public."Appointments" USING btree ("BusinessUnityId");
CREATE INDEX IF NOT EXISTS "IX_Appointments_CustomerId" ON public."Appointments" USING btree ("CustomerId");


-- public."UserSchedules" definition

CREATE TABLE IF NOT EXISTS public."UserSchedules" (
	"Id" bigint GENERATED BY DEFAULT AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"DayOfWeek" int4 NOT NULL,
	"StartDate" text NOT NULL,
	"EndDate" text NOT NULL,
	"IsRest" bool NOT NULL,
	"DayOff" bool NOT NULL,
	"UserId" bigint DEFAULT 0 NOT NULL,
	CONSTRAINT "PK_UserSchedules" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_UserSchedules_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_UserSchedules_UserId" ON public."UserSchedules" USING btree ("UserId");


-- public."UserServices" definition

CREATE TABLE IF NOT EXISTS public."UserServices" (
	"UserId" bigint NOT NULL,
	"ServiceId" bigint NOT NULL,
	"Id" bigint NOT NULL,
	CONSTRAINT "PK_UserServices" PRIMARY KEY ("UserId", "ServiceId"),
	CONSTRAINT "FK_UserServices_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES public."Services"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_UserServices_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_UserServices_ServiceId" ON public."UserServices" USING btree ("ServiceId");
