﻿namespace FitTech.Application.Auth.Dtos;

public abstract record SelfValidatedDto
{
    public abstract Result Validate();
}
