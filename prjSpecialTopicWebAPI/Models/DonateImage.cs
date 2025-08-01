using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class DonateImage
{
    public int DonateImageId { get; set; }

    public int DonateProjectId { get; set; }

    public string DonateImagePath { get; set; } = null!;

    public bool? IsMain { get; set; }

    public virtual DonateProject DonateProject { get; set; } = null!;
}
