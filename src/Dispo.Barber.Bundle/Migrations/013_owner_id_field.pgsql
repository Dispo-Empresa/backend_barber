ALTER TABLE "Companies" ADD COLUMN IF NOT EXISTS "OwnerId" bigint NULL;

ALTER TABLE "Companies" DROP CONSTRAINT IF EXISTS "FK_Companies_Users_OwnerId";
ALTER TABLE "Companies" ADD CONSTRAINT "FK_Companies_Users_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."Users"("Id");
