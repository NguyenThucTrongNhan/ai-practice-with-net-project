CREATE SCHEMA IF NOT EXISTS audit;

CREATE TABLE audit.AuditLogs (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    CorrelationId UUID,
    UserId UUID,
    Action VARCHAR(50) NOT NULL,
    EntitySchema VARCHAR(100) NOT NULL,
    EntityName VARCHAR(100) NOT NULL,
    EntityId UUID,
    Changes JSONB,
    Metadata JSONB,
    Source VARCHAR(100),
    OccurredAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Processed BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE audit.OutboxMessages (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OccurredAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Destination VARCHAR(200) NOT NULL,
    Payload JSONB NOT NULL,
    Headers JSONB,
    Processed BOOLEAN NOT NULL DEFAULT FALSE,
    ProcessedAt TIMESTAMPTZ
);
