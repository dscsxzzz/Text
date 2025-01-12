BEGIN;

-- Create Users Table
CREATE TABLE IF NOT EXISTS "User" (
    "UserId" UUID PRIMARY KEY DEFAULT gen_random_uuid(),       -- GUID field for UserId
    "Username" VARCHAR(255) UNIQUE,  -- Username field
    "Password" VARCHAR(255),         -- Password field
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP -- Created timestamp
);

-- Create Chats Table
CREATE TABLE IF NOT EXISTS "Chat" (
    "ChatId" UUID PRIMARY KEY DEFAULT gen_random_uuid(),   
    "Name" VARCHAR(255) NOT NULL,       -- GUID field for ChatId
    "UserId" UUID NOT NULL,          -- UserId (foreign key from Users)
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP, -- Created timestamp
    FOREIGN KEY ("UserId") REFERENCES "User"("UserId")  -- Foreign key to Users table
);

CREATE INDEX "IDX_Chat_UserId" ON "Chat" ("UserId");

-- Create Messages Table
CREATE TABLE IF NOT EXISTS "Message" (
    "MessageId" UUID PRIMARY KEY DEFAULT gen_random_uuid(),    -- GUID field for MessageId
    "ChatId" UUID NOT NULL,          -- ChatId (foreign key from Chats)
    "MessageText" TEXT NOT NULL,         -- Message content
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "Type" VARCHAR(4) NOT NULL,
    FOREIGN KEY ("ChatId") REFERENCES "Chat"("ChatId")   -- Foreign key to Chats table
);

CREATE INDEX "IDX_Message_ChatId" ON "Message" ("ChatId");

CREATE INDEX "IDX_Message_CreatedAt" ON "Message" ("CreatedAt");

CREATE INDEX "IDX_Chat_CreatedAt" ON "Chat" ("CreatedAt");

COMMIT;