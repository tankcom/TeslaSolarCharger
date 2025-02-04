﻿using TeslaSolarCharger.Shared.Enums;

namespace TeslaSolarCharger.Shared.Dtos;

public class Issue
{
    public Issue()
    {
        IssueMessage = string.Empty;
    }

    public string IssueMessage { get; set; }
    public string[] PossibleSolutions { get; set; } = new string[]{};
    public IssueType? IssueType { get; set; }
}
