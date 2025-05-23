﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Domain.Entities;

public class Email 
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }  
    public string ToEmail { get; set; }
    public string TypeMessage { get; set; }
    public string EmailStatus { get; set; }
    
    public Email(Guid externalId, string toEmail, string typeMessage, string emailStatus)
    {
        ExternalId = externalId;
        ToEmail = toEmail;
        TypeMessage = typeMessage;
        EmailStatus = emailStatus;
    }

}



