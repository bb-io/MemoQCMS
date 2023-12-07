namespace Apps.MemoQCMS.Callbacks.Models.Payload;

public record JobStatusChangedPayload(string Type, JobStatusChangedData Payload);

public record JobStatusChangedData(string ConnectionId, string TranslationJobId, string NewStatus);