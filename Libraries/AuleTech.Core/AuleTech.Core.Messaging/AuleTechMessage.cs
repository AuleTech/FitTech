﻿namespace AuleTech.Core.Messaging;

public class AuleTechMessage<TMessage>
{
    public Guid Id { get; init; }
    public TMessage Message { get; init; } = default!;
    public DateTime CreateDate { get; init; }
    public string[] Errors { get; set; } = [];
    public int RetriesCount { get; private set; }

    public bool Succeeded => Errors.Length == 0;

    private AuleTechMessage(TMessage body)
    {
        Message = body;
        CreateDate = DateTime.UtcNow;
        Id = Guid.CreateVersion7();
    }

    public AuleTechMessage()
    {
        
    }

    internal void Retry()
    {
        RetriesCount++;
    }
    
    internal static AuleTechMessage<TMessage> Create(TMessage body) => new (body);
}
