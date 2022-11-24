using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSC.Api.Core.Entities;
public class SignalRGroup
{
    /// <summary>
    /// Empty constructor is needed for the EF
    /// </summary>
    public SignalRGroup()
    {
    }

    public SignalRGroup(string groupName)
    {
        GroupName = groupName;
    }

    /// <summary>
    /// GroupName is the key and it will be indexed as well
    /// </summary>
    [Key]
    public string GroupName { get; set; }

    public ICollection<SignalRConnection> Connections { get; set; } = new List<SignalRConnection>();
}